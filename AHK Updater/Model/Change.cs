using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AHK_Updater.Models
{
	/// <summary>
	/// Contains one entry of the changelog 
	/// </summary>
	public class Change : IComparable<Change>, IType
	{
		public Change () { }

		/// <summary>
		/// Create a ChangelogEntry entry
		/// </summary>
		/// <param name="Date">Changedate of the entry</param>
		/// <param name="ChangeText">Changetext</param>
		public Change ( string Date, string ChangeText )
		{
			this.Name = Date;
			this.Value = ChangeText;
		}

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

		public string this[string propertyName] => throw new NotImplementedException();

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged ( string propertyName )
		{
			if ( PropertyChanged != null )
			{
				PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
			}
		}

		public int CompareTo ( Change item )
		{
			if ( Name == item.Name )
			{
				return 0;
			}
			return Name.CompareTo( item.Name );
		}
	}
}
