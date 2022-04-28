using System.ComponentModel;

namespace AHKUpdater.Model
{
    public class AhkHotstringToImport : AhkHotstring, INotifyPropertyChanged
    {
        private bool _importThis = true;

        public AhkHotstringToImport () => ImportThis = true;

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
