using System.ComponentModel;
using System.Xml.Serialization;

namespace AHK_Updater.Models
{
	/// <summary>
	/// Description of Variable.
	/// </summary>
	public class Variable : IType
	{
		public Variable () { }
		public Variable ( string Name, string Value )
		{
			this.Name = Name;
			this.Value = Value;
		}

		public string this[string propertyName] => throw new System.NotImplementedException();

		[XmlAttribute( "Name" )]
		public string Name
		{
			get
			{
				return Name;
			}

			set
			{
				Name = value;
				OnPropertyChanged( "Name" );
			}
		}

		[XmlAttribute( "Value" )]
		public string Value
		{
			get
			{
				return Value;
			}

			set
			{
				Value = value;
				OnPropertyChanged( "Value" );
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged ( string PropertyName )
		{
			if ( PropertyChanged != null )
			{
				PropertyChanged( this, new PropertyChangedEventArgs( PropertyName ) );
			}
		}
	}
}
