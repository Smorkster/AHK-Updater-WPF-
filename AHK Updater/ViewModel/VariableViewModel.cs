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
    /// <summary> Model containing all data for variables </summary>
    public class VariableViewModel : INotifyPropertyChanged, IViewModel
    {
        private ICommand _cmdRemove;
        private AhkVariable _currentlyActive;
        private ObservableCollection<Message> _variableError = new ObservableCollection<Message>();
        private ObservableCollection<AhkVariable> _variableList = new ObservableCollection<AhkVariable>();

        public VariableViewModel ()
        {
            CurrentlyActive = null;
        }

        public VariableViewModel ( ObservableCollection<AhkVariable> variables )
        {
            VariableList = variables;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand CmdRemove => _cmdRemove ??= new RelayCommand( x => { Remove(); }, predicate => AnySelected() );

        [XmlIgnore]
        public AhkVariable CurrentlyActive
        {
            get => _currentlyActive;
            set
            {
                _currentlyActive = value;
                OnPropertyChanged( "CurrentlyActive" );
            }
        }

        [XmlIgnore]
        public ObservableCollection<Message> MessageQueue
        {
            get => _variableError;
            set
            {
                _variableError = value;
                OnPropertyChanged( "MessageQueue" );
            }
        }

        [XmlIgnore]
        public AhkVariable SelectedVariable
        {
            get
            {
                try
                {
                    return VariableList.First( x => x.Id.Equals( CurrentlyActive.Id ) );
                }
                catch ( InvalidOperationException )
                {
                    return null;
                }
                catch ( NullReferenceException )
                {
                    return null;
                }
            }
        }

        public bool Unchanged
        {
            get
            {
                try
                {
                    if ( VariableList.Count > 0 )
                    {
                        AhkVariable temp = VariableList.First( x => x.Id == CurrentlyActive.Id );
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

        /// <summary> Get the variable list </summary>
        /// <returns>The variable list</returns>
        public ObservableCollection<AhkVariable> VariableList
        {
            get => _variableList;
            set
            {
                _variableList = value;
                OnPropertyChanged( "VariableList" );
            }
        }

        [XmlIgnore]
        public bool VariablesUpdated
        {
            get; set;
        }

        /// <summary> Add a new variable </summary>
        /// <param name="item">The variable to be added</param>
        public void Add ( AhkVariable item )
        {
            VariableList.Add( item );
            VariablesUpdated = true;
            OnPropertyChanged( "VariableList" );
        }

        /// <summary> Add a new variable </summary>
        /// <param name="item">The variable to be added</param>
        public void Add ( AhkVariableToImport item )
        {
            Add( new AhkVariable( item ) );
        }

        public bool AnySelected ()
        {
            return CurrentlyActive != null;
        }

        public bool NameExists ( string name )
        {
            return CurrentlyActive == null
                ? VariableList.Any( x => x.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) )
                : VariableList.Where( x => !x.Id.Equals( CurrentlyActive.Id ) ).Any( x => x.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) );
        }

        public void Remove ()
        {
            if ( CurrentlyActive.IsNew )
            {
                CurrentlyActive = null;
                OnPropertyChanged( "CurrentlyActive" );
            }
            else
            {
                _ = VariableList.Remove( VariableList.First( x => x.Id.Equals( CurrentlyActive.Id ) ) );

                OnPropertyChanged( "Name" );
                OnPropertyChanged( "VariableList" );
            }
        }

        public void SaveCurrentlyActive ()
        {
            if ( CurrentlyActive.IsNew )
            {
                Add( CurrentlyActive );
            }
            else
            {
                VariableList.First( x => x.Id.Equals( CurrentlyActive.Id ) ).Update( CurrentlyActive );
            }

            VariablesUpdated = true;
            OnPropertyChanged( "VariableList" );
            OnPropertyChanged( "SelectedVariable" );
        }

        public bool VerifyValid ()
        {
            if ( CurrentlyActive == null )
            {
                MessageQueue.Clear();
            }
            else
            {
                if ( string.IsNullOrEmpty( CurrentlyActive.Name ) )
                {
                    MessageQueue.Add( new Message( Localization.Localization.ValidationErrorVariableNameEmpty ) );
                }

                if ( Regex.IsMatch( CurrentlyActive.Name, "\\s+" ) )
                {
                    MessageQueue.Add( new Message( Localization.Localization.ValidationErrorVariableNameContainsWhiteSpace ) );
                }

                if ( string.IsNullOrEmpty( CurrentlyActive.Value ) )
                {
                    MessageQueue.Add( new Message( Localization.Localization.ValidationWarningVariableValueEmpty ) );
                }
            }

            return !MessageQueue.Any( x => x.Type == MessageType.Error ) && !Unchanged;
        }

        private void OnPropertyChanged ( string PropertyName )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( PropertyName ) );
        }
    }
}
