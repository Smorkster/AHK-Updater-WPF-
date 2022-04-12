using AHKUpdater.Library;
using AHKUpdater.Model;
using AHKUpdater.View;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;

namespace AHKUpdater.ViewModel
{
    [XmlRoot( "Ahk" )]
    public class DataViewModel : INotifyPropertyChanged
    {
        private ICommand _cmdAddFunForExtraction;
        private ICommand _cmdAddHsForExtraction;
        private ICommand _cmdAddVarForExtraction;
        private ICommand _cmdNew;
        private ICommand _cmdNewFunction;
        private ICommand _cmdNewHotstring;
        private ICommand _cmdNewVariable;
        private ICommand _cmdSaveCurrentlyActiveFunction;
        private ICommand _cmdSaveCurrentlyActiveHotstring;
        private ICommand _cmdSaveCurrentlyActiveVariable;
        private ICommand _cmdSaveToFile;

        public DataViewModel ()
        {
            HotstringVM = new HotstringViewModel();
            VariableVM = new VariableViewModel();
            FunctionVM = new FunctionViewModel();
            SettingVM = new SettingViewModel();
        }

        public void InitiateFull ()
        {
            EditorVM = new EditorViewModel();
            ExtractionVM = new ExtractionViewModel();
            ExtractionVM.InsertSettingsForExtraction( SettingVM );
            ExtractionViewModel.action += RemovedFromExtraction;
        }

        private void RemovedFromExtraction ( object obj )
        {
            if ( obj.GetType() == typeof( AhkFunction ) )
            {
                FunctionVM.FunctionList.Single( x => x.Id.Equals( ( (AhkFunction) obj ).Id ) ).UpForExtraction = false;
                if ( FunctionVM.CurrentlyActive.Id.Equals( ( (AhkFunction) obj ).Id ) )
                { FunctionVM.CurrentlyActive.UpForExtraction = false; }
            }
            else if ( obj.GetType() == typeof( AhkHotstring ) )
            {
                HotstringVM.HotstringList.Single( x => x.Id.Equals( ( (AhkHotstring) obj ).Id ) ).UpForExtraction = false;
                if ( HotstringVM.CurrentlyActive.Id.Equals( ( (AhkHotstring) obj ).Id ) )
                { HotstringVM.CurrentlyActive.UpForExtraction = false; }
            }
            else if ( obj.GetType() == typeof( AhkVariable ) )
            {
                VariableVM.VariableList.Single( x => x.Id.Equals( ( (AhkVariable) obj ).Id ) ).UpForExtraction = false;
                if ( VariableVM.CurrentlyActive.Id.Equals( ( (AhkVariable) obj ).Id ) )
                { VariableVM.CurrentlyActive.UpForExtraction = false; }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand CmdAddFunctionForExtraction => _cmdAddFunForExtraction ??= new RelayCommand<AhkFunction>( AddFunToExtract, CheckFunctionInExtractionList );

        public ICommand CmdAddHotstringForExtraction => _cmdAddHsForExtraction ??= new RelayCommand<AhkHotstring>( AddHsToExtract, CheckHotstringInExtractionList );

        public ICommand CmdAddVariableForExtraction => _cmdAddVarForExtraction ??= new RelayCommand<AhkVariable>( AddVarToExtract, CheckVariableInExtractionList );

        public ICommand CmdNew => _cmdNew ??= new RelayCommand<int>( New );

        public ICommand CmdNewFunction => _cmdNewFunction ??= new RelayCommand( x => { NewFunction(); } );

        public ICommand CmdNewHotstring => _cmdNewHotstring ??= new RelayCommand( x => { NewHotstring(); } );

        public ICommand CmdNewVariable => _cmdNewVariable ??= new RelayCommand( x => { NewVariable(); } );

        public ICommand CmdSaveCurrentFunction => _cmdSaveCurrentlyActiveFunction ??= new RelayCommand( x => { SaveCurrentFunction(); }, _ => VerifyValidFunction() );

        public ICommand CmdSaveCurrentHotstring => _cmdSaveCurrentlyActiveHotstring ??= new RelayCommand( x => { SaveCurrentHotstring(); }, _ => VerifyValidHotstring() );

        public ICommand CmdSaveCurrentVariable => _cmdSaveCurrentlyActiveVariable ??= new RelayCommand( x => { SaveCurrentVariable(); }, _ => VerifyValidVariable() );

        public ICommand CmdSaveToFile => _cmdSaveToFile ??= new RelayCommand( x => { SaveToFile(); }, p => CheckAnythingUpdated() );

        [XmlIgnore]
        public EditorViewModel EditorVM
        {
            get; set;
        }

        [XmlIgnore]
        public ExtractionViewModel ExtractionVM { get; private set; }

        [XmlElement( "Functions" )]
        public FunctionViewModel FunctionVM
        {
            get; set;
        }

        [XmlElement( "Hotstrings" )]
        public HotstringViewModel HotstringVM
        {
            get; set;
        }

        [XmlElement( "Settings" )]
        public SettingViewModel SettingVM
        {
            get; set;
        }

        [XmlElement( "Variables" )]
        public VariableViewModel VariableVM
        {
            get; set;
        }

        [XmlIgnore]
        public string XmlFile
        {
            get; internal set;
        }

        public void MainWindow_Closing ( object sender, CancelEventArgs e )
        {
            if ( CheckAnythingUpdated() )
            {
                if ( MessageBox.Show( Localization.Localization.MsgSaveBeforeClosing, "", MessageBoxButton.YesNo ) == MessageBoxResult.Yes )
                {
                    CustomMessageBox cmd = new CustomMessageBox( Localization.Localization.MsgQSaveBeforeClosing,
                                                                Localization.Localization.MsgQSaveBeforeClosingTitle,
                                                                new string[] { Localization.Localization.MsgQSaveBeforeClosingBtn1,
                                                                Localization.Localization.MsgQSaveBeforeClosingBtn2,
                                                                Localization.Localization.MsgQSaveBeforeClosingBtn3 } );
                    cmd.ShowDialog();
                    if ( cmd.Answer() != 2 )
                    {
                        SaveToFile();
                    }
                }
            }
        }

        private void AddFunToExtract ( AhkFunction o )
        {
            o.UpForExtraction = true;
            FunctionVM.FunctionList.Single( x => x.Id.Equals( o.Id ) ).UpForExtraction = true;
            ExtractionVM.AddToExtraction( o );
        }

        private void AddHsToExtract ( AhkHotstring o )
        {
            o.UpForExtraction = true;
            HotstringVM.HotstringList.Single( x => x.Id.Equals( o.Id ) ).UpForExtraction = true;
            ExtractionVM.AddToExtraction( o );
        }

        private void AddVarToExtract ( AhkVariable o )
        {
            o.UpForExtraction = true;
            VariableVM.VariableList.First( x => x.Id.Equals( o.Id ) ).UpForExtraction = true;
            ExtractionVM.AddToExtraction( o );
        }

        /// <summary>Check if anything have been updated</summary>
        /// <returns>True if anything have been updated</returns>
        private bool CheckAnythingUpdated ()
        {
            return HotstringVM.HotstringsUpdated || FunctionVM.FunctionsUpdated || VariableVM.VariablesUpdated || SettingVM.SettingsUpdated;
        }

        private bool CheckFunctionInExtractionList ( AhkFunction obj ) => obj != null && !obj.UpForExtraction;

        private bool CheckHotstringInExtractionList ( AhkHotstring obj ) => obj != null && !obj.UpForExtraction;

        private bool CheckVariableInExtractionList ( AhkVariable obj ) => obj != null && !obj.UpForExtraction;

        /// <summary>Creates a new object, depending on which tab is selected</summary>
        /// <param name="selectedTabIndex">Index of the tabitem that is currently selected</param>
        private void New ( int selectedTabIndex )
        {
            if ( selectedTabIndex == 0 )
            {
                NewHotstring();
            }
            else if ( selectedTabIndex == 1 )
            {
                NewVariable();
            }
            else if ( selectedTabIndex == 2 )
            {
                NewFunction();
            }
        }

        /// <summary>Creates a new, empty function</summary>
        private void NewFunction ()
        {
            FunctionVM.CurrentlyActive = new AhkFunction( true );
        }

        /// <summary>Creates a new, empty hotstring</summary>
        private void NewHotstring ()
        {
            HotstringVM.CurrentlyActive = new AhkHotstring( true );
        }

        /// <summary>Creates a new, empty variable</summary>
        private void NewVariable ()
        {
            VariableVM.CurrentlyActive = new AhkVariable( true );
        }

        private void OnPropertyChanged ( string PropertyName )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( PropertyName ) );
        }

        private void SaveCurrentFunction ()
        {
            FunctionVM.SaveCurrentlyActive();
        }

        private void SaveCurrentHotstring ()
        {
            HotstringVM.SaveCurrentlyActive();
        }

        private void SaveCurrentVariable ()
        {
            VariableVM.SaveCurrentlyActive();
        }

        /// <summary>Save data to XML-file</summary>
        private void SaveToFile ()
        {
            FileHandler.WriteXml( this );
            FileHandler.WriteScript( this );

            FunctionVM.FunctionsUpdated = HotstringVM.HotstringsUpdated = VariableVM.VariablesUpdated = SettingVM.SettingsUpdated = false;
        }

        private bool VerifyValidFunction ()
        {
            FunctionVM.MessageQueue.Clear();
            if ( FunctionVM.CurrentlyActive == null )
            {
                return false;
            }
            else if ( HotstringVM.NameExists( FunctionVM.CurrentlyActive.Name ) )
            {
                FunctionVM.MessageQueue.Add( new Message( Localization.Localization.ValidationErrorNameAlreadyInUseAsHotstring ) );
                return false;
            }
            else if ( VariableVM.NameExists( FunctionVM.CurrentlyActive.Name ) )
            {
                FunctionVM.MessageQueue.Add( new Message( Localization.Localization.ValidationErrorNameAlreadyInUseAsVariable ) );
                return false;
            }
            else if ( FunctionVM.NameExists( FunctionVM.CurrentlyActive.Name ) )
            {
                FunctionVM.MessageQueue.Add( new Message( Localization.Localization.ValidationErrorFunctionNameAlreadyInUse ) );
                return false;
            }
            else if ( SettingVM.NameExists( FunctionVM.CurrentlyActive.Name ) )
            {
                FunctionVM.MessageQueue.Add( new Message( Localization.Localization.ValidationErrorNameAlreadyInUseAsSetting ) );
                return false;
            }
            else
            {
                return FunctionVM.VerifyValid();
            }
        }

        private bool VerifyValidHotstring ()
        {
            HotstringVM.MessageQueue.Clear();
            if ( HotstringVM.CurrentlyActive == null )
            {
                return false;
            }
            else if ( HotstringVM.NameExists( HotstringVM.CurrentlyActive.Name ) )
            {
                HotstringVM.MessageQueue.Add( new Message( Localization.Localization.ValidationErrorNameAlreadyInUseAsHotstring ) );
                return false;
            }
            else if ( VariableVM.NameExists( HotstringVM.CurrentlyActive.Name ) )
            {
                HotstringVM.MessageQueue.Add( new Message( Localization.Localization.ValidationErrorNameAlreadyInUseAsVariable ) );
                return false;
            }
            else if ( FunctionVM.NameExists( HotstringVM.CurrentlyActive.Name ) )
            {
                HotstringVM.MessageQueue.Add( new Message( Localization.Localization.ValidationErrorFunctionNameAlreadyInUse ) );
                return false;
            }
            else if ( SettingVM.NameExists( HotstringVM.CurrentlyActive.Name ) )
            {
                HotstringVM.MessageQueue.Add( new Message( Localization.Localization.ValidationErrorNameAlreadyInUseAsSetting ) );
                return false;
            }
            else
            {
                return HotstringVM.VerifyValid();
            }
        }

        private bool VerifyValidVariable ()
        {
            VariableVM.MessageQueue.Clear();
            if ( VariableVM.CurrentlyActive == null )
            {
                return false;
            }
            else if ( HotstringVM.NameExists( VariableVM.CurrentlyActive.Name ) )
            {
                VariableVM.MessageQueue.Add( new Message( Localization.Localization.ValidationErrorNameAlreadyInUseAsHotstring ) );
                return false;
            }
            else if ( VariableVM.NameExists( VariableVM.CurrentlyActive.Name ) )
            {
                VariableVM.MessageQueue.Add( new Message( Localization.Localization.ValidationErrorNameAlreadyInUseAsVariable ) );
                return false;
            }
            else if ( FunctionVM.NameExists( VariableVM.CurrentlyActive.Name ) )
            {
                VariableVM.MessageQueue.Add( new Message( Localization.Localization.ValidationErrorFunctionNameAlreadyInUse ) );
                return false;
            }
            else if ( SettingVM.NameExists( VariableVM.CurrentlyActive.Name ) )
            {
                VariableVM.MessageQueue.Add( new Message( Localization.Localization.ValidationErrorNameAlreadyInUseAsSetting ) );
                return false;
            }
            else
            {
                return VariableVM.VerifyValid();
            }
        }
    }
}
