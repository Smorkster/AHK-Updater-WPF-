using AHK_Updater.Library;
using AHK_Updater.Models;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;

namespace AHK_Updater.ViewModel
{
	[XmlRoot( "Ahk" )]
	public class DataViewModel
	{
		public DataViewModel () { InitializeViewModels(); }

		void InitializeViewModels()
		{
			HotstringViewModel = new HotstringSystemViewModel();
			VariableViewModel = new VariableViewModel();
			FunctionViewModel = new FunctionViewModel();
			SettingViewModel = new SettingsViewModel();
			ChangeViewModel = new ChangeViewModel();
		}

		[XmlElement( "Variables" )]
		public VariableViewModel VariableViewModel { get; set; }

		[XmlElement("Hotstrings")]
		public HotstringSystemViewModel HotstringViewModel { get; set; }

		[XmlElement( "Functions" )]
		public FunctionViewModel FunctionViewModel { get; set; }

		[XmlElement( "Settings" )]
		public SettingsViewModel SettingViewModel { get; set; }

		[XmlElement( "Changes" )]
		public ChangeViewModel ChangeViewModel { get; set; }

		public void Add ( Variable item )
		{
			VariableViewModel.Add( item );
		}

		public void Add ( Hotstring item )
		{
			HotstringViewModel.Add( item );
		}

		public void Add ( Function item )
		{
			FunctionViewModel.Add( item );
		}

		public void Add ( Setting item )
		{
			SettingViewModel.Add( item );
		}

		public void Add ( Change item )
		{
			ChangeViewModel.Add( item );
		}

		public void Remove ( Variable item )
		{
			//VariableViewModel.Remove( VariableViewModel.First( x => x.Name.Equals( item.Name ) ) );
		}

		/*public void Remove ( Hotstring item )
		{
			var HSToRemove = HotstringViewModel.HotstringsList.First( x => x.Name.Equals( item.Name ) );
			HotstringViewModel.HotstringsList.Remove( HSToRemove );
			if ( !HotstringViewModel.HotstringsList.Any( x => x.System.Equals( item.System ) ) )
			{
				HotstringViewModel.HotstringSystems.Remove( HotstringViewModel.HotstringSystems.First( x => x.System.Equals( item.System ) ) );
			}
		}*/

		public void Remove ( Function item )
		{
			//VariableViewModel.Remove( VariableViewModel.First( x => x.Name.Equals( item.Name ) ) );
		}

		public void Remove ( Setting item )
		{
			//VariableViewModel.Remove( VariableViewModel.First( x => x.Name.Equals( item.Name ) ) );
		}

		public void Remove ( Change item )
		{
			//VariableViewModel.Remove( VariableViewModel.First( x => x.Name.Equals( item.Name ) ) );
		}

		public void Update ( Variable item )
		{ }

		public void Update ( Hotstring item )
		{ }

		public void Update ( Function item )
		{ }

		public void Update ( Setting item )
		{ }

		public void Update ( Change item )
		{ }

		public Hotstring CopyHotstring ( Hotstring toCopy )
		{
			return new Hotstring( toCopy );
		}

		ICommand _cmdSaveFile;
		public ICommand CmdSaveFile { get { return _cmdSaveFile ?? ( _cmdSaveFile = new RelayCommand( x => { SaveFile(); }, predicate => CheckUpdated() ) ); } }

		private bool CheckUpdated ()
		{
			return HotstringViewModel.HotstringsUpdated;
		}

		private void SaveFile ()
		{
			using ( FileStream stream = new FileStream( @"C:\Users\6g1w\Documents\t.xml", FileMode.Create ) )
			{
				var xsz = new XmlSerializer( this.GetType() );
				xsz.Serialize( stream, this );
			}
		}
	}
}
