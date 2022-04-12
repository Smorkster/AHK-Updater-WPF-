using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml.Serialization;

namespace AHKUpdater.Model
{
    /// <summary> Description of Function </summary>
    public class AhkFunction : INotifyPropertyChanged, IType
    {
        private Guid _id = Guid.NewGuid();
        private string _name;
        private ObservableCollection<Parameter> _parameterList;
        private bool _upForExtraction = false;
        private string _value;

        public AhkFunction () => Parameter.OnItemChanged += OnParameterChanged;

        public AhkFunction ( AhkFunction OldFunction )
        {
            Contract.Requires( OldFunction != null );
            Name = OldFunction.Name;
            Value = OldFunction.Value;
            _id = OldFunction.Id;
            IsNew = false;
            UpForExtraction = OldFunction.UpForExtraction;

            ParameterList = new ObservableCollection<Parameter>();
            foreach ( Parameter p in OldFunction.ParameterList )
            {
                ParameterList.Add( new Parameter( p ) );
            }
            Parameter.OnItemChanged += OnParameterChanged;
        }

        public AhkFunction ( bool v )
        {
            IsNew = v;
            Value = $"{{\r\n\r\n;{ Localization.Localization.CodeNewFunctionInfo }\r\n\r\n}}";
            _parameterList = new ObservableCollection<Parameter>();
            Parameter.OnItemChanged += OnParameterChanged;
        }

        /// <summary> Create Function object </summary>
        /// <param name="Name">Name of function</param>
        /// <param name="Value">Codetext of function</param>
        public AhkFunction ( string Name, string Value )
        {
            _name = Name;
            _value = Value;
            _parameterList = new ObservableCollection<Parameter>();
            Parameter.OnItemChanged += OnParameterChanged;
        }

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

        [XmlArray( "ParameterList" )]
        public ObservableCollection<Parameter> ParameterList
        {
            get => _parameterList;
            set
            {
                _parameterList = value;
                OnPropertyChanged( "ParameterList" );
                OnPropertyChanged( "FunctionHeader" );
            }
        }

        [XmlIgnore]
        public bool UpForExtraction
        {
            get
            {
                return _upForExtraction;
            }
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
                OnPropertyChanged( "Value" );
            }
        }

        public override bool Equals ( object obj )
        {
            return obj is AhkFunction function &&
                     _name == function._name &&
                     _value == function._value &&
                    EqualityComparer<ObservableCollection<Parameter>>.Default.Equals( _parameterList, function._parameterList ) &&
                    _id.Equals( function._id );
        }

        public override int GetHashCode () => HashCode.Combine( _name, _value, _parameterList, _id );

        public AhkFunction CopyThis ()
        {
            AhkFunction f = new AhkFunction( this );
            return f;
        }

        public void Update ( AhkFunction newFunction )
        {
            Contract.Requires( newFunction != null );
            _name = newFunction.Name;
            _value = newFunction.Value;
            _parameterList.Clear();
            foreach ( Parameter p in newFunction.ParameterList )
            {
                _parameterList.Add( p );
            }

            OnPropertyChanged( "FunctionList" );
        }

        internal void AddParameter ()
        {
            int countNew = ParameterList.Count( x => x.Name.Contains( Localization.Localization.CodeNewParameter, StringComparison.OrdinalIgnoreCase ) );
            if ( countNew > 0 )
            {
                ParameterList.Add( new Parameter( $"{ Localization.Localization.CodeNewParameter }{ countNew + 1 }" ) );
            }
            else
            {
                ParameterList.Add( new Parameter( $"{ Localization.Localization.CodeNewParameter }" ) );
            }

            OnPropertyChanged( "CurrentlyActive" );
            OnPropertyChanged( "FunctionHeader" );
        }

        internal void AddParameter ( string name ) => ParameterList.Add( new Parameter( name ) );

        internal bool Equal ( AhkFunction currentlyActive )
        {
            bool ret = true;
            if ( !string.Equals( Name, currentlyActive.Name, StringComparison.OrdinalIgnoreCase ) )
            {
                ret = false;
            }

            if ( !string.Equals( Value, currentlyActive.Value, StringComparison.OrdinalIgnoreCase ) )
            {
                ret = false;
            }

            if ( currentlyActive.ParameterList.Count != ParameterList.Count )
            {
                ret = false;
            }
            else
            {
                foreach ( Parameter p in ParameterList )
                {
                    if ( !currentlyActive.ParameterList.Contains( p ) )
                    {
                        ret = false;
                    }
                }
            }

            return ret;
        }

        private void OnParameterChanged ( Parameter obj ) => OnPropertyChanged( "FunctionHeader" );

        private void OnPropertyChanged ( string PropertyName ) => PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( PropertyName ) );
    }
}
