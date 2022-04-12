using AHKUpdater.Library;
using AHKUpdater.Library.Enums;
using AHKUpdater.Model;
using AHKUpdater.View;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;

namespace AHKUpdater.ViewModel
{
    public class SettingViewModel : INotifyPropertyChanged, IViewModel
    {
        internal bool _reloadUI = false;
        private ICommand _cmdGetDirectory;
        private ICommand _cmdResetDefault;
        private ICommand _cmdSaveSettings;
        private ObservableCollection<Message> _messageQueue = new ObservableCollection<Message>();
        private ObservableCollection<Setting> _settingList;

        public SettingViewModel ()
        {
            SettingList = new ObservableCollection<Setting>();
            SettingsUpdated = false;
            SettingsChanged = false;
            Setting.OnItemChanged += OnSettingsChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary> Property for a list of available cultures </summary>
        [XmlIgnore]
        public ObservableCollection<string> AvailableCultures
        {
            get; private set;
        }

        public ICommand CmdGetDirectory => _cmdGetDirectory ??= new RelayCommand<Setting>( GetDirectory );
        public ICommand CmdRemove => throw new NotImplementedException();
        public ICommand CmdResetDefault => _cmdResetDefault ??= new RelayCommand<Setting>( ResetDefault, VerifyChanged );
        public ICommand CmdSaveSettings => _cmdSaveSettings ??= new RelayCommand<ItemsControl>( SaveSettings, VerifyValid );

        /// <summary> Property for a list of errors </summary>
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

        /// <summary> Property for if a gui should be included in the scriptfile </summary>
        [XmlIgnore]
        public bool IncludeMenu
        {
            get; set;
        }

        /// <summary> Property to return the path for where to save scriptfile </summary>
        [XmlIgnore]
        public string ScriptLocation
        {
            get
            {
                FileInfo f = new FileInfo( $"{ GetSetting( "Files", "ScriptFileLocation" ).Value }\\{ GetSetting( "Files", "FileName" ).Value }" );
                return f.FullName;
            }
            internal set
            {
            }
        }

        /// <summary> Property to return a list of scriptgroups </summary>
        [XmlIgnore]
        public ObservableCollection<string> SettingGroupsList => new ObservableCollection<string>( SettingList.Select( x => x.SettingGroup ).Distinct() ).GetDisplayNameSettingGroups();

        /// <summary> Property to return the list of settings </summary>
        public ObservableCollection<Setting> SettingList
        {
            get => _settingList;
            set
            {
                _settingList = value;
                OnPropertyChanged( "SettingList" );
            }
        }

        /// <summary> Property for if settings have changed </summary>
        [XmlIgnore]
        public bool SettingsChanged
        {
            get; private set;
        }

        /// <summary> Property for if settings have been updated</summary>
        [XmlIgnore]
        public bool SettingsUpdated
        {
            get; set;
        }

        /// <summary> Unused </summary>
        [XmlIgnore]
        public bool Unchanged => throw new NotImplementedException();

        /// <summary> Add setting </summary>
        /// <param name="setting">Setting to be added</param>
        public void Add ( Setting setting )
        {
            SettingList.Add( setting );
        }

        /// <summary> Unused </summary>
        public bool AnySelected ()
        {
            throw new NotImplementedException();
        }

        /// <summary> Unused </summary>
        public void Remove ()
        {
            throw new NotImplementedException();
        }

        /// <summary> Unused </summary>
        public void SaveCurrentlyActive ()
        {
            throw new NotImplementedException();
        }

        /// <summary> Update a setting </summary>
        /// <param name="settingName">Name of setting to update</param>
        /// <param name="newText">New setting value</param>
        public void Update ( string settingName, string newText )
        {
            int i = SettingList.IndexOf( SettingList.First( x => x.Name.Equals( settingName, StringComparison.OrdinalIgnoreCase ) ) );
            SettingList[ i ].Value = newText;
        }

        /// <summary> Unused </summary>
        public bool VerifyValid ()
        {
            throw new NotImplementedException();
        }

        /// <summary> Return the desired setting </summary>
        /// <param name="group">Settinggroup for the setting to retrieve</param>
        /// <param name="name">Name of setting to retrieve</param>
        /// <returns>The setting that was asked for</returns>
        internal Setting GetSetting ( Setting s )
        {
            return SettingList.First( x => x.SettingGroup.Equals( s.SettingGroup, StringComparison.OrdinalIgnoreCase ) && x.Name.Equals( s.Name, StringComparison.OrdinalIgnoreCase ) );
        }

        /// <summary> Return the desired setting </summary>
        /// <param name="group">Settinggroup for the setting to retrieve</param>
        /// <param name="name">Name of setting to retrieve</param>
        /// <returns>The setting that was asked for</returns>
        internal Setting GetSetting ( string group, string name )
        {
            return SettingList.First( x => x.SettingGroup.Equals( group, StringComparison.OrdinalIgnoreCase ) && x.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) );
        }

        /// <summary> Check if the name is used for any setting </summary>
        /// <param name="name">Name to check</param>
        /// <returns>True if the name is used for any setting</returns>
        internal bool NameExists ( string name )
        {
            return SettingList.Any( x => x.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) );
        }

        /// <summary> Reset all settings to their default value </summary>
        internal void ResetAllDefault ()
        {
            foreach ( Type t in new Type[] { typeof( Library.DefaultSettings.Application ), typeof( Library.DefaultSettings.Files ), typeof( Library.DefaultSettings.ScriptOperations ), typeof( Library.DefaultSettings.ScriptSettings ) } )
            {
                InsertSettings( t );
            }
        }

        /// <summary> Set some default value at VM initialization </summary>
        internal void SetSomeDefaults ()
        {
            FileInfo userProfile = new FileInfo( Environment.GetEnvironmentVariable( "USERPROFILE" ) );

            GetSetting( "Application", "GlobalCulture" ).AvailableValues = Extras.GetAvailableCultures();
            GetSetting( "Files", "ScriptFileLocation" ).DefaultValue = userProfile.FullName;

            // Add directoryparents from defaultfile, upwards
            foreach ( string location in new[] { userProfile.FullName, GetSetting( "Files", "ScriptFileLocation" ).SavedValue } )
            {
                DirectoryInfo parent = new DirectoryInfo( location );
                do
                {
                    if ( !GetSetting( "Files", "ScriptFileLocation" ).AvailableValues.Contains( parent.FullName ) )
                    {
                        GetSetting( "Files", "ScriptFileLocation" ).AvailableValues.Add( parent.FullName );
                    }

                    parent = parent.Parent;
                } while ( !parent.FullName.Equals( parent.Root.FullName, StringComparison.Ordinal ) );
            }
        }

        /// <summary> Open a DirectorySelector-window </summary>
        /// <param name="s">Setting to be used as data for the window</param>
        private void GetDirectory ( Setting s )
        {
            DirectorySelector ds = new DirectorySelector( s.Value );
            ds.ShowDialog();
            if ( (bool) ds.DialogResult )
            {
                GetSetting( "Files", "ScriptFileLocation" ).Update( ( (DirectorySelectorViewModel) ds.DataContext ).SelectedPath.Path.FullName );
                SettingsUpdated = true;
            }
        }

        /// <summary> Used for setting the default values </summary>
        /// <param name="t">Type of the settinggroup to have its values reset</param>
        private void InsertSettings ( Type t )
        {
            ResourceManager defaultSettings = new ResourceManager( t );

            ResourceSet resourceSet = defaultSettings.GetResourceSet( CultureInfo.CurrentUICulture, true, true );
            foreach ( DictionaryEntry entry in resourceSet )
            {
                string resourceKey = entry.Key.ToString();
                string resource = (string) entry.Value;

                Setting s = new Setting() { Name = resourceKey, Value = resource, DefaultValue = resource, SettingGroup = t.Name };
                SettingList.Add( s );
            }
        }

        /// <summary> Unused </summary>
        bool IViewModel.NameExists ( string name )
        {
            throw new NotImplementedException();
        }

        private void OnPropertyChanged ( string PropertyName )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( PropertyName ) );
        }

        /// <summary> A setting was changed, check if different than the value from file. If so, set SettingsChanged </summary>
        private void OnSettingsChanged ()
        {
            SettingsChanged = SettingList.Any( x => !x.Value.Equals( x.SavedValue, StringComparison.OrdinalIgnoreCase ) );
        }

        /// <summary> Reset a setting to its default value </summary>
        /// <param name="o">The setting to reset</param>
        private void ResetDefault ( Setting o )
        {
            o.ResetToDefault();
            OnPropertyChanged( "SettingList" );
            OnPropertyChanged( "Value" );
        }

        /// <summary> Save any changed setting </summary>
        /// <param name="ic">ItemsControl with the settings to save</param>
        private void SaveSettings ( ItemsControl ic )
        {
            foreach ( Setting s in ic.Items )
            {
                if ( s.Name.Equals( "GlobalCulture", StringComparison.OrdinalIgnoreCase ) && !s.Value.Equals( s.SavedValue, StringComparison.OrdinalIgnoreCase ) )
                {
                    _ = MessageBox.Show( Localization.Localization.MsgCultureChanged, "", MessageBoxButton.YesNo ).Equals( MessageBoxResult.Yes );
                }
                SettingList.First( x => x.Name.Equals( s.Name, StringComparison.OrdinalIgnoreCase ) ).Update( s.Value );
            }

            SettingsUpdated = true;
        }

        /// <summary> Return if the setting have a value different than its default value</summary>
        /// <param name="s">Setting to check</param>
        /// <returns>True if the value of the setting is different than its default</returns>
        private bool VerifyChanged ( Setting s )
        {
            return s != null && !s.Value.Equals( s.DefaultValue, StringComparison.OrdinalIgnoreCase );
        }

        /// <summary> Verify if the entered values are valied </summary>
        /// <param name="ic">ItemsControl for the settings to verify</param>
        /// <returns>True if all settings displayed are valid</returns>
        private bool VerifyValid ( ItemsControl ic )
        {
            MessageQueue.Clear();

            if ( ic != null )
            {
                foreach ( Setting s in ic.Items )
                {
                    if ( !s.Value.Equals( s.SavedValue, StringComparison.OrdinalIgnoreCase ) )
                    {
                        if ( string.IsNullOrEmpty( s.Value ) )
                        {
                            MessageQueue.Add( new Message( $"'{ s.Name }' { Localization.Localization.ValidationErrorSettingValueEmpty }" ) );
                        }
                        else
                        {
                            switch ( s.SettingType )
                            {
                                case "FileInfo":
                                    if ( !File.Exists( s.Value ) )
                                    {
                                        MessageQueue.Add( new Message( $"'{ s.Value }' { Localization.Localization.ValidationErrorSettingDirectoryDoesNotExist }" ) );
                                    }
                                    break;
                                case "List":
                                    if ( !s.AvailableValues.Contains( s.Value ) )
                                    {
                                        MessageQueue.Add( new Message( $"'{ s.Value }' { Localization.Localization.ValidationErrorSettingNotOneOfAvailableValues }" ) );
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }

            if ( MessageQueue.Any( x => x.Type == MessageType.Error ) )
            {
                return false;
            }
            else
            {
                foreach ( Setting s in ic.Items )
                {
                    if ( !s.Value.Equals( s.SavedValue ) )
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
