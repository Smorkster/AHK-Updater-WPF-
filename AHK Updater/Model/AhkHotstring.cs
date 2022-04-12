using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Xml.Serialization;

namespace AHKUpdater.Model
{
    /// <summary> Contains one AutoHotKey-command with name, text and system </summary>
    public class AhkHotstring : INotifyPropertyChanged, IType
    {
        private Guid _id = Guid.NewGuid();
        private string _menuTitle = "";
        private string _name = "";
        private string _system = "";
        private bool _upForExtraction = false;
        private string _value = "";
        private bool _hsTypeIsAdvanced = false;

        public AhkHotstring ()
        {
        }

        /// <summary> Create a AHKCommand object from a AHKCommand </summary>
        /// <param name="OldHotstring">Existing hotstring to be copied</param>
        public AhkHotstring ( AhkHotstring OldHotstring )
        {
            Contract.Requires( OldHotstring != null );
            Name = OldHotstring.Name;
            Value = OldHotstring.Value;
            System = OldHotstring.System;
            MenuTitle = OldHotstring.MenuTitle;
            HsTypeIsAdvanced = OldHotstring.HsTypeIsAdvanced;
            UpForExtraction = OldHotstring.UpForExtraction;
            _id = OldHotstring.Id;
            IsNew = false;
        }

        public AhkHotstring ( bool v ) => IsNew = v;

        /// <summary> Create a AHKCommand object </summary>
        /// <param name="name">Name of the command</param>
        /// <param name="value">Codetext of the command</param>
        /// <param name="system">System for the command</param>
        public AhkHotstring ( string name, string value, string system, string menuTitle )
        {
            Name = name;
            Value = value;
            System = system;
            MenuTitle = menuTitle;
            HsTypeIsAdvanced = false;
            IsNew = false;
        }

        [XmlAttribute( "HsTypeIsAdvanced" )]
        public bool HsTypeIsAdvanced
        {
            get => _hsTypeIsAdvanced;
            set
            {
                _hsTypeIsAdvanced = value;
                OnPropertyChanged( "HsTypeIsAdvanced" );
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [XmlIgnore]
        public Guid Id => _id;

        [XmlIgnore]
        public bool IsNew { get; set; } = false;

        [XmlAttribute( "MenuTitle" )]
        public string MenuTitle
        {
            get => _menuTitle;
            set
            {
                _menuTitle = value;
                OnPropertyChanged( "MenuTitle" );
            }
        }

        [XmlAttribute( "Name" )]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged( "Name" );
            }
        }

        [XmlAttribute( "System" )]
        public string System
        {
            get => _system;
            set
            {
                _system = value;
                OnPropertyChanged( "System" );
            }
        }

        [XmlIgnore]
        public bool UpForExtraction
        {
            get => _upForExtraction;
            internal set
            {
                _upForExtraction = value;
                OnPropertyChanged( "UpForExtraction" );
            }
        }

        [XmlAttribute( "Value" )]
        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged( "Code" );
            }
        }

        public AhkHotstring CopyThis () => new AhkHotstring( this );

        public bool Equal ( AhkHotstring other )
        {
            Contract.Requires( other != null );
            return other.Name.Equals( Name, StringComparison.Ordinal )
                && other.System.Equals( System, StringComparison.Ordinal )
                && other.Value.Equals( Value, StringComparison.Ordinal )
                && other.MenuTitle.Equals( MenuTitle, StringComparison.Ordinal )
                && other.HsTypeIsAdvanced.Equals( HsTypeIsAdvanced );
        }

        public void Update ( AhkHotstring newHotstring )
        {
            Contract.Requires( newHotstring != null );
            Name = newHotstring.Name;
            Value = newHotstring.Value;
            System = newHotstring.System;
            MenuTitle = newHotstring.MenuTitle;
            HsTypeIsAdvanced = newHotstring.HsTypeIsAdvanced;

            OnPropertyChanged( "MenuTitle" );
            OnPropertyChanged( "Name" );
            OnPropertyChanged( "System" );
            OnPropertyChanged( "Value" );
            OnPropertyChanged( "HsLvlType" );
        }

        private void OnPropertyChanged ( string PropertyName )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( PropertyName ) );
        }
    }
}