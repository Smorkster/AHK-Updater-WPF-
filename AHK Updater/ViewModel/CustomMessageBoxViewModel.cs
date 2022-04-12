using AHKUpdater.Library;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace AHKUpdater.ViewModel
{
    internal class CustomMessageBoxViewModel : INotifyPropertyChanged
    {
        private int _answer = 0;
        private string[] _buttons = null;
        private ICommand _cmdButtonClicked;
        private string _message = "";
        private string _title = "";

        public CustomMessageBoxViewModel ()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int Answer { get { return _answer; } }

        public string[] Buttons { get { return _buttons; } set { _buttons = value; OnPropertyChanged( "Buttons" ); } }

        public ICommand CmdButtonClicked => _cmdButtonClicked ??= new RelayCommand<string>( ButtonClicked );

        public string Message { get { return _message; } set { _message = value; OnPropertyChanged( "Message" ); } }

        public string Title { get { return _title; } set { _title = value; OnPropertyChanged( "Title" ); } }

        private void ButtonClicked ( string obj )
        {
            _answer = Array.IndexOf( _buttons, obj );
            foreach ( Window w in Application.Current.Windows )
            {
                if ( w.GetType().Name.Equals( "CustomMessageBox" ) )
                {
                    w.Close();
                }
            }
        }

        private void OnPropertyChanged ( string PropertyName ) { PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( PropertyName ) ); }
    }
}
