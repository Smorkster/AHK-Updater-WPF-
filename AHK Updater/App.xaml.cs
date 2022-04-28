using AHKUpdater.ViewModel;
using System;
using System.IO;
using System.Windows;

namespace AHKUpdater
{
    public partial class App : Application
    {
        protected override void OnStartup ( StartupEventArgs e )
        {
            base.OnStartup( e );
            FileInfo _xmlFile = new FileInfo( $"{ Environment.GetEnvironmentVariable( "USERPROFILE" ) }\\AHKUpdaterData.xml" );
            DataViewModel dvm = FileHandler.Read( _xmlFile );
            dvm.XmlFile = _xmlFile.FullName;
            MainWindow MainGUI = new MainWindow( dvm.SettingVM.GetSetting( "Application", "GlobalCulture" ).Value )
            {
                DataContext = dvm
            };

            MainGUI.Show();
        }
    }
}
