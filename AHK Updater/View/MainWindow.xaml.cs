using AHKUpdater.Library;
using AHKUpdater.Model;
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
        private void CbHotstringSystems_DropDownOpened ( object sender, EventArgs e )
        {
        }

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

            Localization.Localization.Culture = CultureInfo.GetCultureInfo( cultureToUse );
            Language = XmlLanguage.GetLanguage( cultureToUse );
            InitializeComponent();
            Title = $"Version { FileVersionInfo.GetVersionInfo( Assembly.GetExecutingAssembly().Location ).ProductVersion }";
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

        private void LvFunctions_SelectionChanged ( object sender, SelectionChangedEventArgs e )
        {
            if ( LvFunctions.SelectedItem != null )
            {
                AhkFunction f = ( (FunctionViewModel) TiFunctions.DataContext ).FunctionList.First( x => x.Id.Equals( ( (AhkFunction) LvFunctions.SelectedItem ).Id ) ).CopyThis();
                ( (FunctionViewModel) TiFunctions.DataContext ).CurrentlyActive = f;
            }
        }

        private void LvHotstrings_SelectionChanged ( object sender, SelectionChangedEventArgs e )
        {
            /*if ( LvHotstrings.SelectedItem != null )
            {
                AhkHotstring h = ( (HotstringViewModel) TiHotstrings.DataContext ).HotstringList.First( x => x.Id.Equals( ( (AhkHotstring) LvHotstrings.SelectedItem ).Id ) ).CopyThis();
                ( (HotstringViewModel) TiHotstrings.DataContext ).CurrentlyActive = h;
            }*/
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

        private bool SettingsViewSource_Filter ( object obj )
        {
            return LvSettingsType.SelectedValue != null && ( obj as Setting ).SettingGroup.Equals( Regex.Replace( LvSettingsType.SelectedValue.ToString(), "\\s", "" ), StringComparison.OrdinalIgnoreCase );
        }

        private void Window_ContentRendered ( object sender, EventArgs e )
        {
            CollectionView hotstringCV = (CollectionView) CollectionViewSource.GetDefaultView( LvHotstrings.ItemsSource );
            CollectionView settingsCV = (CollectionView) CollectionViewSource.GetDefaultView( IcSettings.ItemsSource );

            hotstringCV.Filter = HotstringViewSource_Filter;
            settingsCV.Filter = SettingsViewSource_Filter;

            CbHotstringSystems.SelectedIndex = -1;
            LvFunctions.SelectedIndex = -1;
            LvVariables.SelectedIndex = -1;

            LvHotstrings.SelectionChanged += LvHotstrings_SelectionChanged;
            LvFunctions.SelectionChanged += LvFunctions_SelectionChanged;
            LvVariables.SelectionChanged += LvVariables_SelectionChanged;
            LvSettingsType.SelectionChanged += LvSettingsType_SelectionChanged;

            ( (DataViewModel) DataContext ).SettingVM.SetSomeDefaults();
            if ( ( (DataViewModel) DataContext ).FunctionVM.FunctionList.Count == 0 )
            {
                AhkFunction printtext = new AhkFunction( "PrintText", "{\r\nClipboard =\r\nClipboard = % text %\r\nSleep 400\r\nSend ^ v\r\n}" );
                printtext.AddParameter( "text" );
                ( (DataViewModel) DataContext ).FunctionVM.Add( printtext );
            }
            Closing += ( (DataViewModel) DataContext ).MainWindow_Closing;
            Activate();
        }

        private void ListViewItem_PreviewMouseLeftButtonDown ( object sender, System.Windows.Input.MouseButtonEventArgs e )
        {
            AhkHotstring h = ( (AhkHotstring) ( (ListViewItem) sender ).DataContext ).CopyThis();
            ( (HotstringViewModel) TiHotstrings.DataContext ).CurrentlyActive = h;
        }
    }
}
