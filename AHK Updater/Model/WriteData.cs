using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AHK_Updater.Models
{
	public class WriteData
	{
		ObservableCollection<Change> ChangesToWrite;
		ObservableCollection<Function> FunctionsToWrite;
		ObservableCollection<Hotstring> HotstringsToWrite;
		ObservableCollection<Variable> VariablesToWrite;
		List<Setting> Settings;

		public bool FunctionsToWriteIsEmpty { get { return FunctionsToWrite.Count == 0; } }
		public bool VariablesToWriteIsEmpty { get { return VariablesToWrite.Count == 0; } }

		public WriteData ()
		{
			ChangesToWrite = new ObservableCollection<Change>();
			FunctionsToWrite = new ObservableCollection<Function>();
			HotstringsToWrite = new ObservableCollection<Hotstring>();
			VariablesToWrite = new ObservableCollection<Variable>();
			Settings = new List<Setting>();
		}

		public WriteData ( ObservableCollection<Change> c,
						ObservableCollection<Function> f,
						ObservableCollection<Hotstring> h,
						ObservableCollection<Variable> v,
						List<Setting> s )
		{
			ChangesToWrite = c;
			FunctionsToWrite = f;
			HotstringsToWrite = h;
			VariablesToWrite = v;
			Settings = s;
		}

		/// <summary>
		/// Add a hotstring to be written to file.
		/// </summary>
		/// <param name="item">Item to be written</param>
		public bool AddItem ( object item )
		{
			bool newExtraction = false;
			switch ( item.GetType().Name )
			{
				case "Change":
					if ( !ChangesToWrite.Contains( ( Change ) item ) )
					{
						ChangesToWrite.Add( ( Change ) item );
						newExtraction = true;
					}
					break;
				case "Function":
					if ( !FunctionsToWrite.Contains( ( Function ) item ) )
					{
						FunctionsToWrite.Add( ( Function ) item );
						newExtraction = true;
					}
					break;
				case "Hotstring":
					if ( !HotstringsToWrite.Contains( ( Hotstring ) item ) )
					{
						HotstringsToWrite.Add( ( Hotstring ) item );
						newExtraction = true;
					}
					break;
				case "Variable":
					if ( !VariablesToWrite.Contains( ( Variable ) item ) )
					{
						VariablesToWrite.Add( ( Variable ) item );
						newExtraction = true;
					}
					break;
			}

			return newExtraction;
		}

		/// <summary>
		/// Clear this lists to cancel writing to file
		/// </summary>
		public void Clear ()
		{
			HotstringsToWrite.Clear();
			VariablesToWrite.Clear();
			FunctionsToWrite.Clear();
		}

		/// <summary>
		/// Return list of the functions to write
		/// </summary>
		/// <returns>List of functions</returns>
		public ObservableCollection<Change> GetChanges ()
		{
			return ChangesToWrite;
		}

		/// <summary>
		/// Return list of the functions to write
		/// </summary>
		/// <returns>List of functions</returns>
		public ObservableCollection<Function> GetFunctions ()
		{
			return FunctionsToWrite;
		}

		/// <summary>
		/// Return list of the hotstrings to write
		/// </summary>
		/// <returns>List of hotstrings</returns>
		public ObservableCollection<Hotstring> GetHotstrings ()
		{
			return HotstringsToWrite;
		}

		/// <summary>
		/// Return list of the variables to write
		/// </summary>
		/// <returns>List of variables</returns>
		public ObservableCollection<Variable> GetVariables ()
		{
			return VariablesToWrite;
		}

		/// <summary>
		/// Return current settings
		/// </summary>
		/// <returns>Current settings</returns>
		public List<Setting> GetSettings ()
		{
			return Settings;
		}

		/// <summary>
		/// Removes the hotstring from extractionlist
		/// </summary>
		/// <param name="removeExtractedHotstring">Hotstring to remove</param>
		public void RemoveItem ( object item )
		{
			switch ( item.GetType().Name )
			{
				case "Change":
					ChangesToWrite.Remove( ( Change ) item );
					break;
				case "Function":
					FunctionsToWrite.Remove( ( Function ) item );
					break;
				case "Hotstring":
					HotstringsToWrite.Remove( ( Hotstring ) item );
					break;
				case "Variable":
					VariablesToWrite.Remove( ( Variable ) item );
					break;
			}
		}
	}
}
