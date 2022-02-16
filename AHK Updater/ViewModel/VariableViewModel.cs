using System.Collections.ObjectModel;
using System.Linq;

namespace AHK_Updater.Models
{
	/// <summary>
	/// Model containing all data for variables
	/// </summary>
	public class VariableViewModel : IData
	{
		public VariableViewModel ()
		{
			VariablesList = new ObservableCollection<Variable>();
		}

		public VariableViewModel ( ObservableCollection<Variable> variables )
		{
			VariablesList = variables;
		}

		/// <summary>
		/// Add a new variable
		/// </summary>
		/// <param name="item">The variable to be added</param>
		public void Add ( object item )
		{
			VariablesList.Add( ( Variable ) item );
		}

		/// <summary>
		/// Removes a saved variable
		/// </summary>
		/// <param name="variableName">Name of variable to be removed</param>
		public void Delete ( string variableName )
		{
			Variable variable = VariablesList.Single( x => x.Name.Equals( variableName ) );
			VariablesList.Remove( variable );
		}

		/// <summary>
		/// Checks if a variable with specified name exist
		/// </summary>
		/// <param name="name">A name to check</param>
		/// <returns>If a variable exists</returns>
		public bool Exists ( string name )
		{
			return VariablesList.Any( x => x.Name.Equals( name ) );
		}

		/// <summary>
		/// Get a saved variable
		/// </summary>
		/// <param name="variableName">Name of variable to return</param>
		/// <returns>Return a variable object</returns>
		public Variable Get ( string variableName )
		{
			return VariablesList.First( x => x.Name.Equals( variableName ) );
		}

		/// <summary>
		/// Get the variable list
		/// </summary>
		/// <returns>The variable list</returns>
		public ObservableCollection<Variable> VariablesList { get; }

		public string this[string propertyName] => throw new System.NotImplementedException();

		/// <summary>
		/// Get all variables names as a string
		/// </summary>
		/// <returns>String with the names of all variables</returns>
		internal string GetNamesString ()
		{
			string s = "";
			foreach ( Variable v in VariablesList )
			{
				s = s + " " + v.Name;
			}
			return s.Trim();
		}

		/// <summary>
		/// Update an existing hotstring 
		/// </summary>
		/// <param name="OldItem">Name of hotstring to be updated</param>
		/// <param name="UpdatedItem">Updated hotstring</param>
		public void Update ( string OldItem, object UpdatedItem )
		{
			try
			{
				Variable ExistingVariable = VariablesList.First( x => x.Name.Equals( OldItem ) );
				int index = VariablesList.IndexOf( ExistingVariable );
				VariablesList[index].Name = ( UpdatedItem as Variable ).Name;
				VariablesList[index].Value = ( UpdatedItem as Variable ).Value;
			}
			catch
			{
				VariablesList.Add( ( Variable ) UpdatedItem );
			}
		}
	}
}
