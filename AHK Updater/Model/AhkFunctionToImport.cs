using System.ComponentModel;

namespace AHKUpdater.Model
{
    public class AhkFunctionToImport : AhkFunction, INotifyPropertyChanged
    {
        private bool _importThis = true;

        public AhkFunctionToImport ()
        {
            ParameterList = new System.Collections.ObjectModel.ObservableCollection<Parameter>();
            ImportThis = true;
        }

        public bool ImportThis
        {
            get { return _importThis; }
            set
            {
                _importThis = value;
                OnPropertyChanged( "ImportThis" );
            }
        }
    }
}
