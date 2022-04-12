using System.ComponentModel;
using System.Windows.Input;

namespace AHKUpdater.ViewModel
{
    public interface IViewModel
    {
        event PropertyChangedEventHandler PropertyChanged;

        ICommand CmdRemove { get; }

        bool Unchanged { get; }

        bool AnySelected ();

        public bool NameExists ( string name );

        void Remove ();

        public void SaveCurrentlyActive ();

        public bool VerifyValid ();
    }
}