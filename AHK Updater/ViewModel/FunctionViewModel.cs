using System.Collections.ObjectModel;
using System.Linq;

namespace AHK_Updater.Models
{
	/// <summary>
	/// Model containing all data for functions
	/// </summary>
	public class FunctionViewModel : IData
	{
		public FunctionViewModel () { FunctionsList = new ObservableCollection<Function>(); }

		public FunctionViewModel ( ObservableCollection<Function> functions ) { FunctionsList = functions; }

		/// <summary>
		/// Adds a new function
		/// </summary>
		/// <param name="item">A new function</param>
		public void Add ( object item )
		{
			FunctionsList.Add( ( Function ) item );
		}

		/// <summary>
		/// Deletes a function from the list
		/// </summary>
		/// <param name="name">Name of function to be removed</param>
		public void Delete ( string name )
		{
			Function item = FunctionsList.Single( x => x.Name.Equals( name ) );
			FunctionsList.Remove( item );
		}

		/// <summary>
		/// Checks if there is already a function existing with given name
		/// </summary>
		/// <param name="name">Name of eventual new function to check with</param>
		/// <returns>True if there is a function existing with given name. Otherwise false</returns>
		public bool Exists ( string name )
		{
			foreach ( Function item in FunctionsList )
				if ( item.Name.Equals( name ) )
					return true;
			return false;
		}

		/// <summary>
		/// Get a function-object based on its name
		/// </summary>
		/// <param name="name">Name of function to search for</param>
		/// <returns>A function-object related to the name given on calling</returns>
		public Function Get ( string name )
		{
			return FunctionsList.Single( x => x.Name.Equals( name ) );
		}

		/// <summary>
		/// Returns all saved functions as a list
		/// </summary>
		/// <returns>Functions as a list</returns>
		public ObservableCollection<Function> FunctionsList
		{
			get;
		}

		public string this[string propertyName] => throw new System.NotImplementedException();

		/// <summary>
		/// Collect the names of all functions
		/// </summary>
		/// <returns>String-array of all functionnames</returns>
		public string GetNamesString ()
		{
			string s = "";
			foreach ( Function f in FunctionsList )
				s = s + " " + f.Name;

			return s.Trim();
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
				Function h = FunctionsList.First( x => x.Name.Equals( old ) );
				int index = FunctionsList.IndexOf( h );
				FunctionsList[index].Name = ( item as Function ).Name;
				FunctionsList[index].Value = ( item as Function ).Value;
			}
			catch
			{
				FunctionsList.Add( ( Function ) item );
			}
		}
	}
}
