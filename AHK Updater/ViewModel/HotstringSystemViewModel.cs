using AHK_Updater.Library;
using AHK_Updater.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Xml.Serialization;

namespace AHK_Updater.ViewModel
{
	/// <summary> ViewModel for hotstring and operations </summary>
	public class HotstringSystemViewModel : IData, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		ICommand _cmdHotstringSave;
		Hotstring _activeHotstring;
		string _hotstringError;

		public ICommand CmdHotstringSave { get { return _cmdHotstringSave ?? ( _cmdHotstringSave = new RelayCommand( x => { SaveHotstring(); }, predicate => TestHotstring() ) ); } }
		[XmlIgnore]
		public Hotstring ActiveHotstring { get { return _activeHotstring; } set { _activeHotstring = value; OnPropertyChanged( "ActiveHotstring" ); } }
		public ObservableCollection<HotstringSystem> HotstringSystems { get; set; }
		[XmlIgnore]
		public bool HotstringsUpdated { get; set; }

		[XmlIgnore]
		public string HotstringError
		{
			get { return _hotstringError; }
			set
			{
				if ( value.Equals( "" ) )
				{ _hotstringError = value; }
				else
				{
					if ( _hotstringError.Equals( "" ) )
						_hotstringError = value;
					else
						_hotstringError = $"{ _hotstringError }\n{ value }";
				}
				OnPropertyChanged( "HotstringError" );
			}
		}


		/// <summary>
		/// Initiate the list
		/// </summary>
		public HotstringSystemViewModel ()
		{
			HotstringSystems = new ObservableCollection<HotstringSystem>();
			HotstringsUpdated = false;
		}

		/// <summary>
		/// Save new hotstring
		/// </summary>
		/// <param name="name">Name of hotstring</param>
		/// <param name="text">Text of hotstring</param>
		/// <param name="system">System specifying type of hotstring</param>
		/// <param name="menuTitle">Title for this hotstring in the contextmenu</param>
		public void Add ( string name, string text, string system, string menuTitle )
		{
			Add( new Hotstring( name, text, system, menuTitle ) );
		}

		/// <summary>
		/// Save new hotstring
		/// </summary>
		/// <param name="item">New hotstring to add</param>
		public void Add ( object item )
		{
			Hotstring newHotstring = ( Hotstring ) item;
			if ( !HotstringSystems.Any( x => x.SystemName.Equals( newHotstring.System ) ) )
			{ HotstringSystems.Add( new HotstringSystem( newHotstring ) ); }
			else
			{ HotstringSystems.First( x => x.SystemName.Equals( newHotstring.System ) ).HotstringList.Add( newHotstring ); }
		}

		/// <summary>
		/// Add a new system
		/// </summary>
		/// <param name="system">The new system to add</param>
		public void AddSystem ( HotstringSystem system )
		{
			AddSystem( system );
		}

		/// <summary>
		/// Deletes hotstring from list
		/// If there are no more item with this system, clear it from autocompletionlist
		/// </summary>
		/// <param name="name">Name of hotstring to be deleted</param>
		public void Delete ( string name )
		{
			//Hotstring item = HotstringsList.Single( r => r.Name.Equals( name ) );
			//HotstringsList.Remove( item );
		}

		/// <summary>
		/// Checks if a command exists with same name 
		/// </summary>
		/// <param name="commandName">Name of command to be checked</param>
		/// <returns>True if another hotstring with same name exists</returns>
		public bool Exists ( string commandName )
		{
			foreach ( HotstringSystem s in HotstringSystems )
			{
				if ( s.HotstringList.Where( x => x.Id != _activeHotstring.Id ).Any( x => x.Name.Equals( _activeHotstring.Name ) ) )
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Look up hotstring by name
		/// </summary>
		/// <param name="name">Name of hotstring to be searched</param>
		/// <returns>Hotstring being looked for</returns>
		public Hotstring Get ( string hsName )
		{
			foreach ( HotstringSystem s in HotstringSystems )
			{
				Hotstring t = s.HotstringList.First( x => x.Name.Equals( hsName ) );
				if ( t != null )
				{ return t; }
			}
			return null;
		}

		/// <summary>
		/// Check if there have been any changes
		/// </summary>
		/// <returns>Return if changes was made</returns>
		private bool Unchanged ()
		{
			try
			{
				var temp = HotstringSystems.First( x => x.SystemName.Equals( _activeHotstring.System ) ).HotstringList.First( x => x.Id == _activeHotstring.Id );
				if ( temp.Equal( _activeHotstring ) )
				{ return true; }
			}
			catch { }
			return false;
		}

		/// <summary>
		/// Update an existing hotstring 
		/// </summary>
		/// <param name="old">Name of hotstring to be updated</param>
		/// <param name="item">Updated hotstring</param>
		public void Update ( string old, object item )
		{
			/*try
			{
				Hotstring ExistingHotstring = HotstringsList.First( x => x.Name.Equals( old ) );
				int index = HotstringsList.IndexOf( ExistingHotstring );
				HotstringsList[index].Name = ( item as Hotstring ).Name;
				HotstringsList[index].Value = ( item as Hotstring ).Value;
				HotstringsList[index].System = ( item as Hotstring ).System;
				HotstringsList[index].MenuTitle = ( item as Hotstring ).MenuTitle;
			}
			catch
			{
				HotstringsList.Add( ( Hotstring ) item );
			}*/
		}

		private void SaveHotstring ()
		{
			HotstringsUpdated = true;
		}

		/// <summary>
		/// Delegate to test if the values for the hotstring are valid
		/// Otherwise button to save is disabled
		/// </summary>
		/// <returns>True/false for if values are valid</returns>
		private bool TestHotstring ()
		{
			bool validationResult = true;
			HotstringError = "";
			if ( _activeHotstring == null )
			{ validationResult = false; }
			else
			{
				if ( Unchanged() )
				{
					validationResult = false;
				}
				else
				{
					TestName( ref validationResult );

					TestSystem( ref validationResult );
					TestMenuTitle();
					TestCode( ref validationResult );
				}
			}
			return validationResult;
		}

		/// <summary>
		/// Check if code for hotstring is valid
		/// </summary>
		/// <param name="validationResult">Reference to set hotstring valid</param>
		private void TestCode ( ref bool validationResult )
		{
			if ( _activeHotstring.Value.Equals( "" ) )
			{
				validationResult = false;
				HotstringError = "Code can not be empty.";
			}
		}

		/// <summary>
		/// Inform if menutitle is empty
		/// </summary>
		private void TestMenuTitle ()
		{
			if ( _activeHotstring.MenuTitle.Equals( "" ) )
			{
				HotstringError = "An empty 'MenuTitle' will result in a default string.";
			}
		}

		private void TestSystem ( ref bool validationResult )
		{
			if ( _activeHotstring.System.Equals( "" ) )
			{
				validationResult = false;
				HotstringError = "System must be specified.";
			}
		}

		private void TestName ( ref bool validationResult )
		{
			if ( Regex.IsMatch( _activeHotstring.Name, "\\s+" ) )
			{
				validationResult = false;
				HotstringError = "Name can not contain white-space-characters.";
			}
			else if ( _activeHotstring.Name.Equals( "" ) )
			{
				validationResult = false;
				HotstringError = "Name must be set.";
			}
			else if ( Exists( _activeHotstring.Name ) )
			{
				validationResult = false;
				HotstringError = "Name is already in use.";
			}
		}

		private void OnPropertyChanged ( string PropertyName )
		{
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( PropertyName ) );
		}
	}
}
