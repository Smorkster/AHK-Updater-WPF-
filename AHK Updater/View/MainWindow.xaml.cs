using AHKUpdater.Library;
using AHKUpdater.Model;
using AHKUpdater.View;
using AHKUpdater.ViewModel;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace AHKUpdater
{
    /// <summary> Interaction logic for MainWindow.xaml </summary>
    public partial class MainWindow : Window
    {
        public MainWindow ( string cultureName )
        {
            string cultureToUse = "en";
            if ( string.IsNullOrEmpty( cultureName ) )
            {
                _ = MessageBox.Show( Localization.Localization.ValidationNoCultureSpecified );
            }
            else if ( !Extras.GetAvailableCultures().Contains( cultureName, StringComparer.OrdinalIgnoreCase ) )
            {
                _ = MessageBox.Show( $"{ Localization.Localization.ValidationInvalidCulture }\n{ Localization.Localization.ValidationInvalidCultureInfoUsed } '{ cultureName }'" );
            }
            else
            {
                cultureToUse = cultureName;
            }

            ContentRendered += MainWindow_ContentRendered;
            Localization.Localization.Culture = CultureInfo.GetCultureInfo( cultureToUse );
            Language = XmlLanguage.GetLanguage( cultureToUse );
            Title = $"Version { FileVersionInfo.GetVersionInfo( Assembly.GetExecutingAssembly().Location ).ProductVersion }";
            InitializeComponent();
        }

        private void CbHotstringSystems_DropDownOpened ( object sender, EventArgs e )
        {
        }

        private void CbHotstringSystems_SelectionChanged ( object sender, SelectionChangedEventArgs e )
        {
            if ( LvHotstrings.ItemsSource != null )
            {
                CollectionViewSource.GetDefaultView( LvHotstrings.ItemsSource ).Refresh();
            }
        }

        private bool HotstringViewSource_Filter ( object e )
        {
            return CbHotstringSystems.SelectedValue != null && ( e as AhkHotstring ).System.Equals( CbHotstringSystems.SelectedValue.ToString(), StringComparison.OrdinalIgnoreCase );
        }

        private void ListViewItem_PreviewMouseLeftButtonDown ( object sender, System.Windows.Input.MouseButtonEventArgs e )
        {
            AhkHotstring h = ( (AhkHotstring) ( (ListViewItem) sender ).DataContext ).CopyThis();
            ( (HotstringViewModel) TiHotstrings.DataContext ).CurrentlyActive = h;
        }

        private void LvFunctions_SelectionChanged ( object sender, SelectionChangedEventArgs e )
        {
            if ( LvFunctions.SelectedItem != null )
            {
                AhkFunction f = ( (FunctionViewModel) TiFunctions.DataContext ).FunctionList.First( x => x.Id.Equals( ( (AhkFunction) LvFunctions.SelectedItem ).Id ) ).CopyThis();
                ( (FunctionViewModel) TiFunctions.DataContext ).CurrentlyActive = f;
            }
        }

        private void LvSettingsType_SelectionChanged ( object sender, SelectionChangedEventArgs e )
        {
            if ( LvSettingsType.ItemsSource != null )
            {
                CollectionViewSource.GetDefaultView( IcSettings.ItemsSource ).Refresh();
            }
        }

        private void LvVariables_SelectionChanged ( object sender, SelectionChangedEventArgs e )
        {
            if ( LvVariables.SelectedItem != null )
            {
                AhkVariable v = ( (AhkVariable) LvVariables.SelectedItem ).CopyThis();
                ( (VariableViewModel) TiVariables.DataContext ).CurrentlyActive = v;
            }
        }

        private void MainWindow_ContentRendered ( object sender, EventArgs e )
        {
            CollectionView hotstringCV = (CollectionView) CollectionViewSource.GetDefaultView( LvHotstrings.ItemsSource );
            CollectionView settingsCV = (CollectionView) CollectionViewSource.GetDefaultView( IcSettings.ItemsSource );

            CbHotstringSystems.SelectedIndex = -1;
            LvFunctions.SelectedIndex = -1;
            LvVariables.SelectedIndex = -1;

            ( (DataViewModel) DataContext ).SettingVM.SetSomeDefaults();
            if ( ( (DataViewModel) DataContext ).FunctionVM.FunctionList.Count == 0 || !( (DataViewModel) DataContext ).FunctionVM.NameExistsAll( "PrintText" ) )
            {
                AhkFunction printtext = new AhkFunction( "PrintText", "{\r\nClipboard =\r\nClipboard = % text %\r\nSleep 400\r\nSend ^ v\r\n}" );
                printtext.AddParameter( "text" );
                ( (DataViewModel) DataContext ).FunctionVM.Add( printtext );
                ( (DataViewModel) DataContext ).FunctionVM.FunctionsUpdated = true;
            }
            Closing += ( (DataViewModel) DataContext ).MainWindow_Closing;
            if ( ( (DataViewModel) DataContext ).XmlError > 0 )
            {
                string message = "";

                switch ( ( (DataViewModel) DataContext ).XmlError )
                {
                    case 1:
                        message = Localization.Localization.MsgNoXmlFileFound;
                        break;
                    case 2:
                        message = Localization.Localization.MsgErrorReadingFile;
                        break;
                }
                CustomMessageBox cmd = new CustomMessageBox( message, "", new string[] { Localization.Localization.MsgErrorReadingFileButton } );
                _ = cmd.ShowDialog();
            }


            hotstringCV.Filter = HotstringViewSource_Filter;
            settingsCV.Filter = SettingsViewSource_Filter;

            LvFunctions.SelectionChanged += LvFunctions_SelectionChanged;
            LvVariables.SelectionChanged += LvVariables_SelectionChanged;
            LvSettingsType.SelectionChanged += LvSettingsType_SelectionChanged;
            ( (DataViewModel) DataContext ).HotstringVM.HotstringList.CollectionChanged += ( (DataViewModel) DataContext ).HotstringVM.HotstringList_CollectionChanged;
            ( (DataViewModel) DataContext ).VariableVM.VariableList.CollectionChanged += ( (DataViewModel) DataContext ).VariableVM.VariableList_CollectionChanged;
            ( (DataViewModel) DataContext ).FunctionVM.FunctionList.CollectionChanged += ( (DataViewModel) DataContext ).FunctionVM.FunctionList_CollectionChanged;

            Activate();
        }

        private bool SettingsViewSource_Filter ( object obj )
        {
            return LvSettingsType.SelectedValue != null && ( obj as Setting ).SettingGroup.Equals( Regex.Replace( LvSettingsType.SelectedValue.ToString(), "\\s", "" ), StringComparison.OrdinalIgnoreCase );
        }
    }
}
