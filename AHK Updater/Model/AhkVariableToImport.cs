using System.ComponentModel;

namespace AHKUpdater.Model
{
    public class AhkVariableToImport : AhkVariable, INotifyPropertyChanged
    {
        private bool _importThis = true;

        public AhkVariableToImport () => ImportThis = true;

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
