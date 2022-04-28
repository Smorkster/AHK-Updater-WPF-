using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Xml.Serialization;

namespace AHKUpdater.Model
{
    /// <summary> Description of Variable </summary>
    public class AhkVariable : INotifyPropertyChanged, IType
    {
        private Guid _id = Guid.NewGuid();
        private string _name = "";
        private bool _upForExtraction = false;
        private string _value = "";

        public AhkVariable () { }

        public AhkVariable ( bool v ) { IsNew = v; }

        public AhkVariable ( AhkVariable OldVariable )
        {
            Contract.Requires( OldVariable != null );
            Name = OldVariable.Name;
            Value = OldVariable.Value;
            UpForExtraction = OldVariable.UpForExtraction;
            _id = OldVariable.Id;
        }

        public AhkVariable ( AhkVariableToImport OldVariable )
        {
            Contract.Requires( OldVariable != null );
            Name = OldVariable.Name;
            Value = OldVariable.Value;
            UpForExtraction = OldVariable.UpForExtraction;
            _id = OldVariable.Id;
        }

        /// <summary> Create Variable object </summary>
        /// <param name="Name">Name of variable</param>
        /// <param name="Value">Value of variable</param>
        public AhkVariable ( string Name, string Value ) { _name = Name; _value = Value; }

        public event PropertyChangedEventHandler PropertyChanged;

        [XmlIgnore]
        public Guid Id => _id;

        [XmlIgnore]
        public bool IsNew { get; set; } = false;

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

        [XmlIgnore] public bool UpForExtraction { get { return _upForExtraction; } internal set { _upForExtraction = value; OnPropertyChanged( "UpForExtraction" ); } }

        [XmlAttribute( "Value" )]
        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged( "Value" );
            }
        }

        public AhkVariable CopyThis () { return new AhkVariable( this ); }

        public bool Equal ( AhkVariable other )
        {
            Contract.Requires( other != null );
            return other.Name.Equals( _name, StringComparison.Ordinal ) && other.Value.Equals( _value, StringComparison.Ordinal );
        }

        public void Update ( AhkVariable newVariable )
        {
            Contract.Requires( newVariable != null );
            _name = newVariable.Name;
            _value = newVariable.Value;
            OnPropertyChanged( "Name" );
        }

        internal void OnPropertyChanged ( string PropertyName ) { PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( PropertyName ) ); }
    }
}
