using System.ComponentModel;
using System.Xml.Serialization;

namespace AHK_Updater.Models
{
	/// <summary>
	/// Contains one AutoHotKey-command with name, text and system
	/// Also contains functions for returning command formated for script- and XML-file
	/// </summary>
	public class Hotstring : IType, IDataErrorInfo
	{
		string _Name;
		string _Value;
		string _System;
		string _MenuTitle;
		int _id;

		public Hotstring () { }
		/// <summary>
		/// Create a AHKCommand object
		/// </summary>
		/// <param name="name">Name of the command</param>
		/// <param name="value">Codetext of the command</param>
		/// <param name="system">System for the command</param>
		public Hotstring ( string name, string value, string system, string menuTitle )
		{
			Name = name;
			Value = value;
			System = system;
			MenuTitle = menuTitle;
			_id = ( name + value + system + menuTitle ).GetHashCode();
		}

		/// <summary>
		/// Create a AHKCommand object from a AHKCommand
		/// </summary>
		/// <param name="NewHotstring">Created hotstring to be copied</param>
		public Hotstring ( Hotstring NewHotstring )
		{
			Name = NewHotstring.Name;
			Value = NewHotstring.Value;
			System = NewHotstring.System;
			MenuTitle = NewHotstring.MenuTitle;
			_id = NewHotstring.Id;
		}

		public int Id
		{
			get { return _id; }
		}

		[XmlAttribute( "MenuTitle" )]
		public string MenuTitle
		{
			get { return _MenuTitle; }
			set
			{
				_MenuTitle = value;
				OnPropertyChanged( "MenuTitle" );
			}
		}

		[XmlAttribute("Name")]
		public string Name
		{
			get
			{
				return _Name;
			}
			set
			{
				_Name = value;
				OnPropertyChanged( "Name" );
			}
		}

		[XmlAttribute( "System" )]
		public string System
		{
			get
			{
				return _System;
			}
			set
			{
				_System = value;
				OnPropertyChanged( "System" );
			}
		}

		[XmlAttribute( "Value" )]
		public string Value
		{
			get
			{
				return _Value;
			}
			set
			{
				_Value = value;
				OnPropertyChanged( "Code" );
			}
		}
		public bool Equal(Hotstring other)
		{
			if ( other.Name.Equals( _Name ) && other.System.Equals( _System ) && other.Value.Equals( _Value ) && other.MenuTitle.Equals( _MenuTitle ) && other.Id == _id )
				return true;
			return false;
		}
		public string Error => throw new System.NotImplementedException();

		public string this[string propertyName] => throw new System.NotImplementedException();

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