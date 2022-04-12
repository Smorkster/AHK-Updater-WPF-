using AHKUpdater.ViewModel;
using System;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace AHKUpdater
{
    public partial class App : Application
    {
        private readonly FileInfo _xmlFile = new FileInfo( $"{ Environment.GetEnvironmentVariable( "USERPROFILE" ) }\\AHKUpdaterData.xml" );

        protected override void OnStartup ( StartupEventArgs e )
        {
            base.OnStartup( e );
            DataViewModel dvm = Read();
            dvm.XmlFile = _xmlFile.FullName;
            MainWindow MainGUI = new MainWindow( dvm.SettingVM.GetSetting( "Application", "GlobalCulture" ).Value )
            {
                DataContext = dvm
            };

            MainGUI.Show();
        }

        private DataViewModel Read ()
        {
            using XmlReader stream = XmlReader.Create( new FileStream( _xmlFile.FullName, FileMode.Open ) );
            DataViewModel dvm;

            try
            {
                dvm = (DataViewModel) new XmlSerializer( typeof( DataViewModel ) ).Deserialize( stream );
            }
            catch ( InvalidOperationException )
            {
                dvm = new DataViewModel();
                dvm.SettingVM.ResetAllDefault();
            }

            if ( string.IsNullOrEmpty( dvm.SettingVM.GetSetting( "Files", "ScriptFileLocation" ).Value ) )
            {
                dvm.SettingVM.GetSetting( "Files", "ScriptFileLocation" ).Value = _xmlFile.Directory.FullName;
            }
            dvm.InitiateFull();

            return dvm;
        }
    }
}
