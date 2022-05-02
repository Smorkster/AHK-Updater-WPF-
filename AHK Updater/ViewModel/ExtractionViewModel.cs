using AHKUpdater.Library;
using AHKUpdater.Library.Enums;
using AHKUpdater.Model;
using AHKUpdater.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Xml.Serialization;

namespace AHKUpdater.ViewModel
{
    public class ExtractionViewModel : INotifyPropertyChanged
    {
        internal static Action<object> action;
        private ICommand _cmdCancelExtraction;
        private ICommand _cmdExtractToScript;
        private ICommand _cmdExtractToXml;
        private ICommand _cmdRemoveFromExtraction;
        private DataViewModel _extractionDvm = new DataViewModel();
        private MessageCollection _messageQueue = new MessageCollection();

        public ExtractionViewModel () { }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand CmdCancelExtraction => _cmdCancelExtraction ??= new RelayCommand( CancelExtraction, VerifyExtraction );

        public ICommand CmdExtractToScript => _cmdExtractToScript ??= new RelayCommand( ExtractToScript, VerifyExtraction );

        public ICommand CmdExtractToXml => _cmdExtractToXml ??= new RelayCommand( ExtractToXml, VerifyExtraction );

        public ICommand CmdRemoveFromExtraction => _cmdRemoveFromExtraction ??= new RelayCommand<object>( RemoveFromExtraction, VerifyRemoveFromExtraction );

        [XmlIgnore]
        public MessageCollection MessageQueue
        {
            get => _messageQueue;
            set
            {
                _messageQueue = value;
                Message.OnTimeElapsed += MessageInvalidated;

                OnPropertyChanged( "MessageQueue" );
            }
        }

        [XmlIgnore]
        public ObservableCollection<object> ToExtract
        {
            get
            {
                ObservableCollection<object> collection = new ObservableCollection<object>();
                int i = 0;
                if ( ExtractionDvm != null )
                {
                    for ( ; i < ExtractionDvm.HotstringVM.HotstringList.Count; i++ )
                    {
                        AhkHotstring h = ExtractionDvm.HotstringVM.HotstringList[ i ];
                        collection.Add( h );
                    }
                    for ( i = 0; i < ExtractionDvm.FunctionVM.FunctionList.Count; i++ )
                    {
                        AhkFunction h = ExtractionDvm.FunctionVM.FunctionList[ i ];
                        collection.Add( h );
                    }
                    for ( i = 0; i < ExtractionDvm.VariableVM.VariableList.Count; i++ )
                    {
                        AhkVariable h = ExtractionDvm.VariableVM.VariableList[ i ];
                        collection.Add( h );
                    }
                }
                return collection;
            }
        }

        internal DataViewModel ExtractionDvm
        {
            get { return _extractionDvm; }
            set { _extractionDvm = value; }
        }

        internal void AddToExtraction ( object o )
        {
            switch ( o.GetType().Name )
            {
                case nameof( AhkFunction ):
                    ExtractionDvm.FunctionVM.FunctionList.Add( (AhkFunction) o );
                    break;
                case nameof( AhkHotstring ):
                    ExtractionDvm.HotstringVM.HotstringList.Add( (AhkHotstring) o );
                    break;
                case nameof( AhkVariable ):
                    ExtractionDvm.VariableVM.VariableList.Add( (AhkVariable) o );
                    break;
            }
            OnPropertyChanged( "ToExtract" );
        }

        internal void InsertSettingsForExtraction ( SettingViewModel svm )
        {
            if ( ExtractionDvm.SettingVM.SettingList.Count == 0 )
            {
                for ( int i = 0; i < svm.SettingList.Count; i++ )
                {
                    Setting s = svm.SettingList[ i ];
                    ExtractionDvm.SettingVM.Add( s );
                }
            }
            ExtractionDvm.SettingVM.GetSetting( "Files", "FileName" ).Value = "ExtractedAhk";
        }

        private static string SetExtractionTarget ()
        {
            DirectorySelector ds = new DirectorySelector( Environment.GetEnvironmentVariable( "USERPROFILE" ) );
            ds.ShowDialog();
            if ( (bool) ds.DialogResult )
            {
                return $"{ ( (DirectorySelectorViewModel) ds.DataContext ).SelectedPath.Path.FullName }";
            }
            return Environment.GetEnvironmentVariable( "USERPROFILE" );
        }

        private void CancelExtraction ( object obj )
        {
            foreach ( AhkFunction f in ExtractionDvm.FunctionVM.FunctionList )
            { action.Invoke( f ); }
            ExtractionDvm.FunctionVM.FunctionList.Clear();

            foreach ( AhkHotstring h in ExtractionDvm.HotstringVM.HotstringList )
            { action.Invoke( h ); }
            ExtractionDvm.HotstringVM.HotstringList.Clear();

            foreach ( AhkVariable v in ExtractionDvm.VariableVM.VariableList )
            { action.Invoke( v ); }
            ExtractionDvm.VariableVM.VariableList.Clear();

            OnPropertyChanged( "ToExtract" );
        }

        private void ExtractToScript ( object o )
        {
            ExtractionDvm.SettingVM.GetSetting( "Files", "ScriptFileLocation" ).Value = SetExtractionTarget();
            string extractionPath = FileHandler.WriteScript( _extractionDvm );
            MessageQueue.Add( new Message( MessageType.Success, $"{ Localization.Localization.MsgExtractedTo }\r\n{ extractionPath }" ) );
        }

        private void ExtractToXml ( object o )
        {
            ExtractionDvm.SettingVM.GetSetting( "Files", "XmlFileLocationForExtraction" ).Value = SetExtractionTarget();
            FileHandler.WriteExtractedXml( ExtractionDvm );
        }

        private void MessageInvalidated ( Message obj )
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
                delegate
                {
                    _ = MessageQueue.Remove( obj );
                } );
            OnPropertyChanged( "MessageQueue" );
        }

        private void OnPropertyChanged ( string PropertyName )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( PropertyName ) );
        }

        private void RemoveFromExtraction ( object obj )
        {
            if ( obj.GetType() == typeof( AhkFunction ) )
            {
                ExtractionDvm.FunctionVM.FunctionList.Remove( (AhkFunction) obj );
                //FunctionVM.FunctionList.Single( x => x.Id.Equals( ( (AhkFunction) obj ).Id ) ).UpForExtraction = false;
            }
            else if ( obj.GetType() == typeof( AhkHotstring ) )
            {
                ExtractionDvm.HotstringVM.HotstringList.Remove( (AhkHotstring) obj );
                //HotstringVM.HotstringList.Single( x => x.Id.Equals( ( (AhkHotstring) obj ).Id ) ).UpForExtraction = false;
            }
            else
            {
                ExtractionDvm.VariableVM.VariableList.Remove( (AhkVariable) obj );
                //VariableVM.VariableList.Single( x => x.Id.Equals( ( (AhkVariable) obj ).Id ) ).UpForExtraction = false;
            }
            action.Invoke( obj );
            OnPropertyChanged( "ToExtract" );
        }

        private bool VerifyExtraction ( object obj )
        {
            if ( ExtractionDvm == null )
            {
                return false;
            }
            else
            {
                return ExtractionDvm.FunctionVM.FunctionList.Count + ExtractionDvm.HotstringVM.HotstringList.Count + ExtractionDvm.VariableVM.VariableList.Count > 0;
            }
        }

        private bool VerifyRemoveFromExtraction ( object obj ) => obj != null;
    }
}
