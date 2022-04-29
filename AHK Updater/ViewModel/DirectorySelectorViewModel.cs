using AHKUpdater.Library;
using AHKUpdater.Model;
using AHKUpdater.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace AHKUpdater.ViewModel
{
    internal class DirectorySelectorViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<PathSuggestion> _suggestedPaths = new ObservableCollection<PathSuggestion>();
        private ICommand _cmdCancel;
        private ICommand _cmdRowClicked;
        private ICommand _cmdSelect;
        private PathSuggestion _selectedPath;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand CmdCancel => _cmdCancel ??= new RelayCommand<Window>( Cancel );

        public ICommand CmdRowClicked => _cmdRowClicked ??= new RelayCommand( RowClicked );

        public ICommand CmdSelect => _cmdSelect ??= new RelayCommand<Window>( Select, _ => { return SelectedPath != null && SelectedPath.Path.Exists; } );

        public string CurrentPath
        {
            get => SelectedPath == null ? "" : SelectedPath.Path.FullName;
            set => SelectedPath = new PathSuggestion( new DirectoryInfo( value ), true );
        }

        public PathSuggestion SelectedPath
        {
            get => _selectedPath;
            set
            {
                _selectedPath = value;
                OnPropertyChanged( "SelectedPath" );
                OnPropertyChanged( "SuggestedPaths" );
                OnPropertyChanged( "CurrentPath" );
            }
        }

        public ObservableCollection<PathSuggestion> SuggestedPaths
        {
            get
            {
                if ( SelectedPath != null && SelectedPath.Path.Exists )
                {
                    try
                    {
                        _suggestedPaths.Clear();
                        DirectoryInfo same = new DirectoryInfo( SelectedPath.Path.FullName );
                        try
                        {
                            DirectoryInfo up1 = same.Parent;
                            try
                            {
                                DirectoryInfo up2 = up1.Parent;
                                _suggestedPaths.NotNullAdd( new PathSuggestion( up2, false ) );
                            }
                            catch ( Exception ) { }
                            _suggestedPaths.NotNullAdd( new PathSuggestion( up1, false ) );
                        }
                        catch ( Exception ) { }
                        _suggestedPaths.NotNullAdd( new PathSuggestion( same, true ) );

                        foreach ( DirectoryInfo d in same.EnumerateDirectories() )
                        {
                            try { _suggestedPaths.Add( new PathSuggestion( d, false ) ); }
                            catch { }
                        }
                    }
                    catch ( Exception )
                    {
                        CustomMessageBox cmd = new CustomMessageBox( "You don't have permission to operate in this folder", "No permission", new string[] { "OK" } );
                        _ = cmd.ShowDialog();
                    }
                }
                return _suggestedPaths;
            }
        }

        private void Cancel ( Window o )
        {
            o.DialogResult = false;
            o.Close();
        }

        private void OnPropertyChanged ( string PropertyName )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( PropertyName ) );
        }

        private void RowClicked ( object obj )
        {
            if ( obj != null )
            {
                SelectedPath = (PathSuggestion) obj;
            }
        }

        private void Select ( Window o )
        {
            o.DialogResult = true;
            o.Close();
        }
    }
}
