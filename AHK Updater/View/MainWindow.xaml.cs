using AHK_Updater.Models;
using AHK_Updater.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace AHK_Updater
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow ()
		{
			InitializeComponent();
		}

		private void LvHotstrings_SelectionChanged ( object sender, SelectionChangedEventArgs e )
		{
			Hotstring h = ( ( DataViewModel ) DataContext ).CopyHotstring( ( Hotstring ) LvHotstrings.SelectedItem );
			gridHotstring.DataContext = h;
			( ( HotstringSystemViewModel ) TiHotstrings.DataContext ).ActiveHotstring = h;
		}
	}
}
