using AHK_Updater.Models;
using AHK_Updater.ViewModel;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace AHK_Updater
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public MainWindow MainGUI;

		protected override void OnStartup ( StartupEventArgs e )
		{
			base.OnStartup( e );
			var dvm = new DataViewModel();
			MainGUI = new MainWindow
			{
				DataContext = dvm
			};
			MainGUI.Show();

			SetupEditors();
			var a = Read();

			dvm.HotstringViewModel.Add( new Hotstring( "Test", "Test", "Sys", "ijdkqwopijfowklfålpoewijfkmewllåpfokijewkfwfeklew" ) );
			dvm.HotstringViewModel.Add( new Hotstring( "Test2", "Test", "Sys", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" ) );
			dvm.HotstringViewModel.Add( new Hotstring( "Test3", "Test", "TestSys2", "Testing menutext" ) );
			dvm.HotstringViewModel.Add( new Hotstring( "Test4", "Test", "TestSys2", "Test menu" ) );
		}

		private object Read ()
		{
			using ( FileStream stream = new FileStream( @"C:\Users\6g1w\Documents\t.xml", FileMode.Open ) )
			{
				return ( new XmlSerializer( typeof( DataViewModel ) ) ).Deserialize( stream );
			}
		}

		private void SetupEditors ()
		{
			EditorViewModel evm = new EditorViewModel();
			evm.GenerateEditorList();
			( ( ComboBox ) MainGUI.FindName( "CbEditorForOpeningFiles" ) ).DataContext = evm;
		}
	}
}
