using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace AHKUpdater.Model
{
    public class Parameter : IEquatable<Parameter>, INotifyPropertyChanged
    {
        internal static Action<Parameter> OnItemChanged;
        private Guid _id = Guid.NewGuid();
        private string _name;

        public Parameter () { }

        public Parameter ( string v ) => Name = v;

        public Parameter ( Parameter p ) { Name = p.Name; }

        public event PropertyChangedEventHandler PropertyChanged;

        [XmlIgnore]
        public Guid Id => _id;

        [XmlAttribute( "Name" )]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged( "Name" );
                OnItemChanged.Invoke( this );
            }
        }

        public override bool Equals ( object obj ) => Equals( obj as Parameter );

        public override int GetHashCode () => HashCode.Combine( _name, _id );

        public bool Equals ( [AllowNull] Parameter other ) => other != null && other.Name.Equals( Name, StringComparison.Ordinal );

        private void OnPropertyChanged ( string PropertyName ) => PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( PropertyName ) );
    }
}
