using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace AHKUpdater.ViewModel
{
    public class EditorViewModel : INotifyPropertyChanged
    {
        public EditorViewModel ()
        {
            EditorsList = new ObservableCollection<FileInfo>();
            GenerateEditorList();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<FileInfo> EditorsList { get; }

        public void AddEditor ( string EditorPath )
        {
            if ( File.Exists( EditorPath ) )
            {
                EditorsList.Add( new FileInfo( EditorPath ) );
                OnPropertyChanged( "EditorList" );
            }
        }

        public void GenerateEditorList ()
        {
            string[] SuggestedEditors = { @"C:\Program Files\Notepad++\notepad++.exe", @"C:\Program Files (x86)\Notepad++\notepad++.exe", @"C:\Windows\System32\notepad.exe" };
            foreach ( string editor in SuggestedEditors )
            { AddEditor( editor ); }
        }

        public bool ListContainsName ( string Text )
        {
            return EditorsList.Any( x => x.Name.Equals( Text, System.StringComparison.OrdinalIgnoreCase ) );
        }

        public bool ListContainsPath ( string Text )
        {
            return EditorsList.Any( x => x.FullName.Equals( Text, System.StringComparison.OrdinalIgnoreCase ) );
        }

        private void OnPropertyChanged ( string PropertyName ) { PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( PropertyName ) ); }
    }
}
