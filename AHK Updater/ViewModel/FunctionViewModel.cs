using AHKUpdater.Library;
using AHKUpdater.Library.Enums;
using AHKUpdater.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Xml.Serialization;

namespace AHKUpdater.ViewModel
{
    /// <summary> Model containing all data for functions </summary>
    public class FunctionViewModel : INotifyPropertyChanged, IViewModel
    {
        private ICommand _cmdAddParameter;
        private ICommand _cmdRemove;
        private ICommand _cmdRemoveParameter;
        private AhkFunction _currentlyActive;
        private MessageCollection _messageQueue = new MessageCollection();

        public FunctionViewModel ()
        {
            FunctionList = new ObservableCollection<AhkFunction>();
            CurrentlyActive = null;
            Parameter.OnItemChanged += OnParameterChanged;
        }

        public FunctionViewModel ( ObservableCollection<AhkFunction> functions )
        {
            FunctionList = functions;
            CurrentlyActive = null;
            Parameter.OnItemChanged += OnParameterChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand CmdAddParameter => _cmdAddParameter ??= new RelayCommand<AhkFunction>( AddParameter );

        public ICommand CmdRemove => _cmdRemove ??= new RelayCommand( x => { Remove(); }, p => AnySelected() );

        public ICommand CmdRemoveParameter => _cmdRemoveParameter ??= new RelayCommand<Parameter>( RemoveParameter );

        [XmlIgnore]
        public AhkFunction CurrentlyActive
        {
            get => _currentlyActive;
            set
            {
                _currentlyActive = value;
                OnPropertyChanged( "CurrentlyActive" );
                OnPropertyChanged( "FunctionHeader" );
            }
        }

        [XmlIgnore]
        public MessageCollection MessageQueue
        {
            get => _messageQueue;
            set
            {
                _messageQueue = value;
                OnPropertyChanged( "MessageQueue" );
            }
        }

        [XmlIgnore]
        public string FunctionHeader
        {
            get
            {
                string header;

                if ( CurrentlyActive == null || CurrentlyActive.ParameterList == null )
                {
                    header = "";
                }
                else if ( CurrentlyActive.ParameterList.Count > 0 )
                {
                    string parameters = "";
                    foreach ( Parameter p in CurrentlyActive.ParameterList )
                    {
                        parameters = string.IsNullOrEmpty( parameters )
                              ? $"{ p.Name }"
                              : $"{ parameters }, { p.Name }";
                    }
                    header = $"{ CurrentlyActive.Name }( { parameters } )";
                }
                else
                {
                    header = $"{ CurrentlyActive.Name }";
                }
                return header;
            }
        }

        /// <summary> Returns all saved functions as a list </summary>
        /// <returns>Functions as a list</returns>
        public ObservableCollection<AhkFunction> FunctionList
        {
            get;
        }

        [XmlIgnore]
        public bool FunctionsUpdated
        {
            get; set;
        }

        [XmlIgnore]
        public bool Unchanged
        {
            get
            {
                try
                {
                    if ( FunctionList.Count > 0 )
                    {
                        AhkFunction temp = FunctionList.First( x => x.Id.Equals( CurrentlyActive.Id ) );
                        if ( temp.Equal( CurrentlyActive ) )
                        {
                            return true;
                        }
                    }
                }
                catch ( InvalidOperationException ) { }

                return false;
            }
        }

        /// <summary> Adds a new function </summary>
        /// <param name="item">A new function</param>
        public void Add ( object item )
        {
            FunctionList.Add( (AhkFunction) item );
            FunctionsUpdated = true;
            OnPropertyChanged( "FunctionsList" );
        }

        /// <summary> Adds a new function </summary>
        /// <param name="item">A new function</param>
        public void Add ( AhkFunctionToImport item )
        {
            Add( new AhkFunction( item ) );
        }

        /// <summary> Verify if a function is selected </summary>
        /// <returns>Boolean for if any function is selected</returns>
        public bool AnySelected ()
        {
            return CurrentlyActive != null;
        }

        /// <summary> Check if a functionname exists </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool NameExists ( string name )
        {
            return CurrentlyActive == null
                ? FunctionList.Any( x => x.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) )
                : FunctionList.Where( x => !x.Id.Equals( CurrentlyActive.Id ) ).Any( x => x.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) );
        }

        /// <summary> Remove the currently active function </summary>
        public void Remove ()
        {
            _ = FunctionList.Remove( FunctionList.First( x => x.Id.Equals( CurrentlyActive.Id ) ) );
            FunctionsUpdated = true;
            OnPropertyChanged( "FunctionList" );
        }

        /// <summary> Save the currently active function </summary>
        public void SaveCurrentlyActive ()
        {
            if ( CurrentlyActive.IsNew )
            {
                Add( CurrentlyActive );
                CurrentlyActive.IsNew = false;
            }
            else
            {
                FunctionList.First( x => x.Id.Equals( CurrentlyActive.Id ) ).Update( CurrentlyActive );
            }

            FunctionsUpdated = true;
            OnPropertyChanged( "FunctionsList" );
            OnPropertyChanged( "Name" );
        }

        public bool VerifyValid ()
        {
            MessageQueue.Clear();
            if ( CurrentlyActive == null )
            {
                return false;
            }
            else
            {
                #region CheckName
                if ( string.IsNullOrEmpty( CurrentlyActive.Name ) )
                {
                    MessageQueue.Add( new Message( Localization.Localization.ValidationErrorFunctionNameEmpty ) );
                }
                else
                {
                    MessageQueue.RemoveMessage( Localization.Localization.ValidationErrorFunctionNameEmpty );
                }

                if ( string.IsNullOrEmpty( CurrentlyActive.Value ) )
                {
                    MessageQueue.Add( new Message( Localization.Localization.ValidationErrorFunctionCodeEmpty ) );
                }
                else
                {
                    MessageQueue.RemoveMessage( Localization.Localization.ValidationErrorFunctionCodeEmpty );
                }

                if ( CurrentlyActive.Name == null )
                { }
                else if ( Regex.IsMatch( CurrentlyActive.Name, "\\s+" ) )
                {
                    MessageQueue.Add( new Message( Localization.Localization.ValidationErrorFunctionNameContainsWhiteSpace ) );
                }
                else
                {
                    MessageQueue.RemoveMessage( Localization.Localization.ValidationErrorFunctionNameContainsWhiteSpace );
                }
                #endregion

                #region CheckParameters
                foreach ( string p in CurrentlyActive.ParameterList.Select( x => x.Name ).Distinct() )
                {
                    if ( CurrentlyActive.ParameterList.Count( x => x.Name.Equals( p, StringComparison.OrdinalIgnoreCase ) ) > 1 )
                    {
                        MessageQueue.Add( new Message( $"{ Localization.Localization.ValidationErrorParameterNameAlreadyInUse } '{ p }'" ) );
                    }
                }
                #endregion

                return !MessageQueue.Any( x => x.Type == MessageType.Error ) && !Unchanged;
            }
        }

        private void AddParameter ( AhkFunction fun )
        {
            fun.AddParameter();
        }

        private void OnParameterChanged ( Parameter obj )
        {
            OnPropertyChanged( "FunctionHeader" );
        }

        private void OnPropertyChanged ( string PropertyName )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( PropertyName ) );
        }

        private void RemoveParameter ( Parameter obj )
        {
            _ = CurrentlyActive.ParameterList.Remove( CurrentlyActive.ParameterList.First( x => x.Name.Equals( obj.Name, StringComparison.OrdinalIgnoreCase ) ) );
            OnPropertyChanged( "FunctionHeader" );
        }
    }
}
