using System.ComponentModel;
using System.Xml.Serialization;

namespace AHK_Updater.Models
{
	public class Setting : IType
	{
		public Setting () { }
		public Setting ( string Name, string Text, string DefaultValue )
		{
			this.Name = Name;
			this.Value = Text;
			this.DefaultValue = DefaultValue;
		}

		public string this[string propertyName] => throw new System.NotImplementedException();

		[XmlAttribute( "Name" )]
		public string Name 
		{
			get { return Name; }
			set 
			{
				Name = value;
				OnPropertyChanged( "Name" );
			}
		}

		[XmlAttribute( "Value" )]
		public string Value
		{
			get { return Value; }
			set
			{
				Value = value;
				OnPropertyChanged( "SettingValue" );
			}
		}

		[XmlAttribute( "DefaultValue" )]
		public string DefaultValue
		{
			get;
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
