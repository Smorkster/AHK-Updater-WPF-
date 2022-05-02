using AHKUpdater.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AHKUpdater.ViewModel
{
    public class MessageCollection : ObservableCollection<Message>, INotifyPropertyChanged
    {
        private readonly ObservableCollection<Message> _messageQueue = new ObservableCollection<Message>();

        public MessageCollection () { }
    }
}
