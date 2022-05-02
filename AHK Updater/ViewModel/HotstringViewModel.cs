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
    /// <summary> ViewModel for hotstring and operations </summary>
    public class HotstringViewModel : INotifyPropertyChanged, IViewModel
    {
        private ICommand _cmdRemove;
        private ICommand _cmdSaveCurrentlyActive;
        private AhkHotstring _currentlyActive;
        private ObservableCollection<Message> _messageQueue = new ObservableCollection<Message>();
        private ObservableCollection<AhkHotstring> _hotstringList = new ObservableCollection<AhkHotstring>();

        public HotstringViewModel ()
        {
            HotstringsUpdated = false;
            CurrentlyActive = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand CmdRemove => _cmdRemove ??= new RelayCommand( x => { Remove(); }, predicate => AnySelected() );

        public ICommand CmdSaveCurrentlyActive => _cmdSaveCurrentlyActive ??= new RelayCommand( x => { SaveCurrentlyActive(); }, predicate => VerifyValid() );

        /// <summary> Property for the hotstring that is currently being edited </summary>
        [XmlIgnore]
        public AhkHotstring CurrentlyActive
        {
            get
            {
                return _currentlyActive;
            }
            set
            {
                /*if ( _currentlyActive != null )
                {
                    if ( !Unchanged )
                    {
                        string q = _currentlyActive.IsNew
                            ? Localization.Localization.HotstringVerifySaveUnsavedEditNew
                            : Localization.Localization.HotstringVerifySaveUnsavedEdit;

                        if ( !string.IsNullOrEmpty( q ) )
                        {
                            if ( MessageBox.Show( q, "", MessageBoxButton.YesNo ) == MessageBoxResult.Yes )
                            {
                                SaveCurrentlyActive();
                            }
                        }
                    }
                }*/
                _currentlyActive = value;

                OnPropertyChanged( "CurrentlyActive" );
                OnPropertyChanged( "HotstringSystemList" );
                OnPropertyChanged( "HotstringList" );
                OnPropertyChanged( "SelectedSystem" );
                OnPropertyChanged( "SelectedHotstring" );
                OnPropertyChanged( "HsType" );
            }
        }

        /// <summary> Contains any errorinformation about values for the current hotstring </summary>
        [XmlIgnore]
        public ObservableCollection<Message> MessageQueue
        {
            get => _messageQueue;
            set
            {
                _messageQueue = value;
                OnPropertyChanged( "MessageQueue" );
            }
        }

        /// <summary> Property for the list of hotstrings</summary>
        public ObservableCollection<AhkHotstring> HotstringList
        {
            get => _hotstringList;
            set
            {
                _hotstringList = value;
                OnPropertyChanged( "HotstringList" );
            }
        }

        /// <summary> Property for if any hotstrings have been updated </summary>
        [XmlIgnore]
        public bool HotstringsUpdated
        {
            get; set;
        }

        /// <summary> Property for a list of systems for all the hotstrings </summary>
        [XmlIgnore]
        public ObservableCollection<string> HotstringSystemList => new ObservableCollection<string>( HotstringList.Select( x => x.System ).Distinct() );

        /// <summary> Property for the currently selected hotstring </summary>
        [XmlIgnore]
        public AhkHotstring SelectedHotstring
        {
            get
            {
                try
                {
                    return HotstringList.First( x => x.Id.Equals( CurrentlyActive.Id ) );
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

        /// <summary> Property for the currently selected system </summary>
        [XmlIgnore]
        public string SelectedSystem => CurrentlyActive == null
                    ? ""
                    : string.IsNullOrEmpty( CurrentlyActive.System )
                        ? ""
                        : CurrentlyActive.System;

        /// <summary> Check if there have been any changes </summary>
        /// <returns>True if no changes was made</returns>
        public bool Unchanged
        {
            get
            {
                try
                {
                    AhkHotstring temp = HotstringList.First( x => x.Id == CurrentlyActive.Id );
                    if ( temp.Equal( CurrentlyActive ) )
                    {
                        return true;
                    }
                }
                catch ( InvalidOperationException ) { }
                return false;
            }
        }

        /// <summary> Save new hotstring </summary>
        /// <param name="item">New hotstring to add</param>
        public void Add ( AhkHotstring item )
        {
            HotstringList.Add( item );
            HotstringsUpdated = true;
            OnPropertyChanged( "HotstringSystemList" );
            OnPropertyChanged( "HotstringList" );
            OnPropertyChanged( "SelectedSystem" );
            OnPropertyChanged( "SelectedHotstring" );
        }

        /// <summary> Save new hotstring </summary>
        /// <param name="item">New hotstring to add</param>
        public void Add ( AhkHotstringToImport item )
        {
            Add( new AhkHotstring( item ) );
        }

        /// <summary> Save new hotstring </summary>
        /// <param name="name">Name of hotstring</param>
        /// <param name="text">Text of hotstring</param>
        /// <param name="system">System specifying type of hotstring</param>
        /// <param name="menuTitle">Title for this hotstring in the contextmenu</param>
        public void Add ( string name, string text, string system, string menuTitle )
        {
            Add( new AhkHotstring( name, text, system, menuTitle ) );
        }

        /// <summary> Predicate to check if a hotstring is selected </summary>
        /// <returns>If a hotstring is selected</returns>
        public bool AnySelected ()
        {
            return CurrentlyActive != null;
        }

        /// <summary> Check if name is used in any hotstring </summary>
        /// <param name="name"> Name to be checked </param>
        /// <returns> True if the name is already in use for a hotstring </returns>
        public bool NameExists ( string name )
        {
            return CurrentlyActive == null
                ? HotstringList.Any( x => x.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) )
                : HotstringList.Where( x => !x.Id.Equals( CurrentlyActive.Id ) ).Any( x => x.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) );
        }

        /// <summary> Removes the currently active hotstring </summary>
        public void Remove ()
        {
            if ( !CurrentlyActive.IsNew )
            {
                _ = HotstringList.Remove( HotstringList.First( x => x.Id.Equals( CurrentlyActive.Id ) ) );

                OnPropertyChanged( "Name" );
                OnPropertyChanged( "HotstringSystemList" );
                OnPropertyChanged( "HotstringList" );
            }
            OnPropertyChanged( "CurrentlyActive" );
            CurrentlyActive = null;
        }

        /// <summary> Save the updated hotstring </summary>
        public void SaveCurrentlyActive ()
        {
            if ( CurrentlyActive.IsNew )
            {
                CurrentlyActive.IsNew = false;
                Add( CurrentlyActive );
            }
            else
            {
                HotstringList.First( x => x.Id.Equals( CurrentlyActive.Id ) ).Update( CurrentlyActive );
            }

            HotstringsUpdated = true;
        }

        /// <summary> Delegate to test if the values for the hotstring are valid, otherwise button is disabled </summary>
        /// <returns> True/false for if values are valid </returns>
        public bool VerifyValid ()
        {
            if ( CurrentlyActive == null )
            {
                MessageQueue.Clear();
            }
            else
            {
                TestName();
                TestSystem();
                TestCode();
                TestMenuTitle();
            }
            return !MessageQueue.Any( x => x.Type == MessageType.Error ) && !Unchanged;
        }

        private void OnPropertyChanged ( string PropertyName )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( PropertyName ) );
        }

        /// <summary> Check if code for hotstring is valid </summary>
        /// <param name="validationResult"> Reference to set hotstring valid </param>
        private void TestCode ()
        {
            if ( string.IsNullOrWhiteSpace( CurrentlyActive.Value ) )
            {
                MessageQueue.Add( new Message( Localization.Localization.ValidationErrorHotstringCodeEmpty ) );
            }
            else
            {
                if ( CurrentlyActive.HsTypeIsAdvanced == true )
                {
                    if ( !CurrentlyActive.Value[ ( CurrentlyActive.Value.IndexOf( '(' ) + 1 ).. ].Contains( ')' ) )
                    {
                        MessageQueue.Add( new Message( Localization.Localization.ValidationErrorHotstringCodeNoParenthesis ) );
                    }
                }
            }
        }

        /// <summary> Inform if menutitle is empty </summary>
        private void TestMenuTitle ()
        {
            if ( string.IsNullOrEmpty( CurrentlyActive.MenuTitle ) )
            {
                MessageQueue.Add( new Message( MessageType.Warning, Localization.Localization.ValidationWarningHotstringMenuTitleEmpty ) );
            }
        }

        /// <summary> Verify name </summary>
        private void TestName ()
        {
            // Can not be empty
            if ( string.IsNullOrEmpty( CurrentlyActive.Name ) )
            {
                MessageQueue.Add( new Message( Localization.Localization.ValidationErrorHotstringNameEmpty ) );
            }

            // Can not contain whitespace-characters
            if ( Regex.IsMatch( CurrentlyActive.Name, "\\s+" ) )
            {
                MessageQueue.Add( new Message( Localization.Localization.ValidationErrorHotstringNameContainsWhiteSpace ) );
            }
        }

        /// <summary> Verify system </summary>
        private void TestSystem ()
        {
            // Can not be empty
            if ( string.IsNullOrWhiteSpace( CurrentlyActive.System ) )
            {
                MessageQueue.Add( new Message( Localization.Localization.ValidationErrorHotstringSystemEmpty ) );
            }

            // Can not contain whitespace-character
            if ( Regex.IsMatch( CurrentlyActive.System, "\\s+" ) )
            {
                MessageQueue.Add( new Message( Localization.Localization.ValidationErrorHotstringSystemContainsWhiteSpace ) );
            }
        }
    }
}
