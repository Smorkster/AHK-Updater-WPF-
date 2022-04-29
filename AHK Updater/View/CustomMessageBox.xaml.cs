using AHKUpdater.ViewModel;
using System.Windows;

namespace AHKUpdater.View
{
    /// <summary> Interaction logic for CustomMessageBox.xaml </summary>
    public partial class CustomMessageBox : Window
    {
        public CustomMessageBox ( string message, string title, string[] buttons )
        {
            InitializeComponent();
            ( (CustomMessageBoxViewModel) DataContext ).Message = message;
            ( (CustomMessageBoxViewModel) DataContext ).Title = title;
            ( (CustomMessageBoxViewModel) DataContext ).Buttons = buttons;
        }

        public int Answer ()
        {
            return ( (CustomMessageBoxViewModel) DataContext ).Answer;
        }

        private void CenterWindow ( object sender, System.EventArgs e )
        {
            Left = ( SystemParameters.PrimaryScreenWidth / 2 ) - ( Width / 2 );
            Top = ( SystemParameters.PrimaryScreenHeight / 2 ) - ( Height / 2 );
        }
    }
}
