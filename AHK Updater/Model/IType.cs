using System.ComponentModel;

namespace AHK_Updater.Models
{
	public interface IType
	{
		string this[string propertyName] { get; }

		string Name { get; set; }
		string Value { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
