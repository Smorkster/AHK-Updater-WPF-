using System.ComponentModel;
using System.Xml.Serialization;

namespace AHK_Updater.Models
{
	/// <summary>
	/// Description of Function.
	/// </summary>
	public class Function : IType
	{
		public Function () { }
		/// <summary>
		/// Create Function object
		/// </summary>
		/// <param name="Name">Name of function</param>
		/// <param name="Text">Codetext of function</param>
		public Function ( string Name, string Text )
		{
			this.Name = Name;
			this.Value = Text;
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
				OnPropertyChanged( "Text" );
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
