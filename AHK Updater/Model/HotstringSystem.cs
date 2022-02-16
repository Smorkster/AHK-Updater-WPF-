using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AHK_Updater.Models
{
	public class HotstringSystem
	{
		string _SystemName;
		ObservableCollection<Hotstring> _HotstringList;
		public HotstringSystem () { }
		public HotstringSystem ( string SystemName )
		{
			_SystemName = SystemName;
			_HotstringList = new ObservableCollection<Hotstring>();
		}

		public HotstringSystem ( Hotstring hotstring )
		{
			_SystemName = hotstring.System;
			_HotstringList = new ObservableCollection<Hotstring>();
			_HotstringList.Add( hotstring );
		}

		public HotstringSystem ( string SystemName, ObservableCollection<Hotstring> hotstrings )
		{
			_SystemName = SystemName;
			_HotstringList = hotstrings;
		}

		public HotstringSystem ( Hotstring hotstring, ObservableCollection<Hotstring> hotstrings )
		{
			_SystemName = hotstring.System;
			_HotstringList = hotstrings;
		}

		public string SystemName { get { return _SystemName; } set { _SystemName = value; OnPropertyChanged( "SystemName" ); } }
		public ObservableCollection<Hotstring> HotstringList { get { return _HotstringList; } set { _HotstringList = value; } }

		public string this[string propertyName] => throw new NotImplementedException();

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged ( string PropertyName )
		{
			if ( PropertyChanged != null )
			{
				PropertyChanged( this, new PropertyChangedEventArgs( PropertyName ) );
			}
		}
	}
}
