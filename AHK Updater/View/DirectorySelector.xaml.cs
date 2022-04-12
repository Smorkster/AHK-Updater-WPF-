using AHKUpdater.ViewModel;
using System.IO;
using System.Windows;

namespace AHKUpdater.View
{
    public partial class DirectorySelector : Window
    {
        public DirectorySelector ()
        {
            InitializeComponent();
        }

        public DirectorySelector ( string startPath )
        {
            InitializeComponent();
            ( (DirectorySelectorViewModel) DataContext ).SelectedPath = new Model.PathSuggestion( new DirectoryInfo( startPath ), false );
        }
    }
}
