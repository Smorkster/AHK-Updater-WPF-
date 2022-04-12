using AHKUpdater.Library;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AHKUpdater.Model
{
    public class Setting : INotifyPropertyChanged, IType
    {
        internal static Action OnItemChanged;
        private ObservableCollection<string> _availableValues = new ObservableCollection<string>();
        private Guid _id = Guid.NewGuid();
        private string _value;

        public Setting () { }

        public event PropertyChangedEventHandler PropertyChanged;

        [XmlIgnore]
        public ObservableCollection<string> AvailableValues
        {
            get => _availableValues;
            set
            {
                _availableValues = value;
                OnPropertyChanged( "AvailableValues" );
            }
        }

        [XmlAttribute( "DefaultValue" )]
        public string DefaultValue { get; set; }

        [XmlIgnore]
        public string DisplayName => Name.ToDisplayName();

        [XmlIgnore]
        public Guid Id => _id;

        [XmlIgnore]
        public bool IsNew { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        [XmlAttribute( "Name" )]
        public string Name { get; set; }

        [XmlIgnore]
        public string SavedValue { get; set; }

        [XmlAttribute( "SettingGroup" )]
        public string SettingGroup { get; set; }

        [XmlAttribute( "SettingType" )]
        public string SettingType { get; set; }

        [XmlAttribute( "Value" )]
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                if ( string.IsNullOrEmpty( SavedValue ) )
                { SavedValue = value; }
                _value = value;
                OnPropertyChanged( "Value" );
                OnItemChanged.Invoke();
            }
        }

        public void ResetToDefault ()
        {
            Value = DefaultValue;
            OnPropertyChanged( "Value" );
        }

        internal void Update ( string s )
        {
            Value = s;
            SavedValue = s;
        }

        private void OnPropertyChanged ( string PropertyName ) { PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( PropertyName ) ); }
    }
}
