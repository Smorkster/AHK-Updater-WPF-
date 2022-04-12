using AHKUpdater.ViewModel;
using System.Windows;

namespace AHKUpdater.View
{
    /// <summary> Interaction logic for CustomMessageBox.xaml </summary>
    public partial class CustomMessageBox : Window
    {
        private readonly string[] _buttons;
        private readonly string _message;
        private readonly string _title;

        public CustomMessageBox ( string message, string title, string[] buttons )
        {
            _buttons = buttons;
            _message = message;
            _title = title;
            InitializeComponent();
            ContentRendered += CustomMessageBox_ContentRendered;
        }

        public int Answer ()
        {
            return ( (CustomMessageBoxViewModel) DataContext ).Answer;
        }

        private void CustomMessageBox_ContentRendered ( object sender, System.EventArgs e )
        {
            ( (CustomMessageBoxViewModel) DataContext ).Message = _message;
            ( (CustomMessageBoxViewModel) DataContext ).Title = _title;
            ( (CustomMessageBoxViewModel) DataContext ).Buttons = _buttons;
        }
    }
}
