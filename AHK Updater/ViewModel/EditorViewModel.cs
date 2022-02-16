using AHK_Updater.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace AHK_Updater.ViewModel
{
	public class EditorViewModel : INotifyPropertyChanged
	{
		public EditorViewModel ()
		{
			EditorsList = new ObservableCollection<FileInfo>();
		}

		public bool AddEditor ( string EditorPath )
		{
			if ( File.Exists( EditorPath ) )
			{
				//Editor NewEditor = new Editor( EditorPath );
				EditorsList.Add( new FileInfo( EditorPath ) );
				OnPropertyChanged( "EditorList" );
			}
			return false;
		}

		public void GenerateEditorList ()
		{
			string[] SuggestedEditors = { @"C:\Program Files\Notepad++\notepad++.exe", @"C:\Program Files (x86)\Notepad++\notepad++.exe", @"C:\Windows\System32\notepad.exe" };
			foreach ( string editor in SuggestedEditors )
			{ _ = AddEditor( editor ); }
		}

		public ObservableCollection<FileInfo> EditorsList { get; }

		public bool ListContainsName ( string Text )
		{
			return EditorsList.Any( x => x.Name.Equals( Text ) );
		}

		public bool ListContainsPath ( string Text )
		{
			return EditorsList.Any( x => x.FullName.Equals( Text ) );
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
