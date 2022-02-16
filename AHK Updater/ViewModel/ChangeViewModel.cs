using System;
using System.Collections.ObjectModel;
using System.Linq;
using AHK_Updater.Library;

namespace AHK_Updater.Models
{
	/// <summary>
	/// Model containing all data for Changelog
	/// </summary>
	public class ChangeViewModel
	{
		public string TodaysDate;

		public ChangeViewModel () { ChangesList = new ObservableCollection<Change>(); }
		public ChangeViewModel ( ObservableCollection<Change> changes )
		{
			ChangesList = changes;
			TodaysDate = DateTime.Today.ToString( "yyyy-MM-dd" );
		}

		/// <summary>
		/// Adds change to changelog 
		/// </summary>
		/// <param name="version">Versionnumber of entry</param>
		/// <param name="entry">Text for changelogentry</param>
		public void Add ( object item )
		{
			ChangesList.Add( ( Change ) item );
		}

		/// <summary>
		/// Get a change for specific version
		/// </summary>
		/// <param name="changeVersion"></param>
		/// <returns>The reqested change</returns>
		public Change Get ( string name )
		{
			return ChangesList.First( x => x.Name.Equals( name ) );
		}

		/// <summary>
		/// Return the whole changelog as an listobject
		/// </summary>
		/// <returns>Changelog as a list</returns>
		public ObservableCollection<Change> ChangesList
		{
			get;
		}

		/// <summary>
		/// Update an existing hotstring 
		/// </summary>
		/// <param name="old">Name of hotstring to be updated</param>
		/// <param name="item">Updated hotstring</param>
		public void Update ( string old, object item )
		{
			try
			{
				Change c = ChangesList.First( x => x.Name.Equals( old ) );
				int index = ChangesList.IndexOf( c );
				ChangesList[index].Name = ( item as Change ).Name;
				ChangesList[index].Value = ( item as Change ).Value;
			}
			catch
			{
				ChangesList.Add( ( Change ) item );
			}
		}

		/// <summary>
		/// Update the currently active (latest) change
		/// </summary>
		/// <param name="updateInfo">Information about update to change</param>
		public void UpdateLatest ( string updateInfo )
		{
			if ( !ChangesList.Any( x => x.Name.Equals( TodaysDate ) ) )
			{
				Add( new Change( TodaysDate, "" ) );
			}
			Change change = ChangesList.First( x => x.Name.Equals( TodaysDate ) );
			change.Value += "\r\n" + updateInfo;
		}
	}
}
