using System.Collections.Generic;

namespace AHK_Updater.Models
{
	public class SettingsViewModel
	{
		public SettingsViewModel ()
		{
			SettingsList = new List<Setting>();
		}

		public SettingsViewModel ( List<Setting> s )
		{
			SettingsList = s;
		}

		/// <summary>
		/// Add setting
		/// </summary>
		/// <param name="setting">Setting to be added</param>
		public void Add ( Setting setting )
		{
			SettingsList.Add( setting );
		}

		/// <summary>
		/// Get the setting with the given name
		/// </summary>
		/// <param name="name">Name of setting</param>
		/// <returns>Named setting</returns>
		public Setting Get ( string name )
		{
			return SettingsList.Find( x => x.Name.Equals( name ) );
		}

		/// <summary>
		/// Get the list of settings
		/// </summary>
		/// <returns>Current settings</returns>
		public List<Setting> SettingsList
		{
			get;
		}

		/// <summary>
		/// Update a setting
		/// </summary>
		/// <param name="settingName">Name of setting to update</param>
		/// <param name="newText">New setting value</param>
		public void Update ( string settingName, string newText )
		{
			int i = SettingsList.FindIndex( x => x.Name.Equals( settingName ) );
			SettingsList[i].Value = newText;
		}
	}
}
