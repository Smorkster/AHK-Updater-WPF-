using AHKUpdater.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace AHKUpdater.ViewModel
{
    internal class FileHandler
    {
        static DataViewModel modeltowrite;

        internal static string WriteScript ( DataViewModel extractionDvm )
        {
            modeltowrite = extractionDvm;
            using StreamWriter writer = new StreamWriter( $"{ extractionDvm.SettingVM.ScriptLocation }.ahk" );
            writer.WriteLine( CreateTitle( $"{ Localization.Localization.ScriptFileCreatedAt } { DateTime.UtcNow.ToShortDateString() }" ) );
            writer.WriteLine( "SetTimer,UPDATEDSCRIPT,1000" );
            writer.WriteLine( "UPDATEDSCRIPT:\r\nFileGetAttrib,attribs,%A_ScriptFullPath%\r\nIfInString,attribs,A\r\n{\r\nFileSetAttrib,-A,%A_ScriptFullPath%\r\nSplashTextOn,,,Updated script,\r\nSleep,500\r\nReload\r\n}\r\n" );
            if ( extractionDvm.SettingVM.IncludeMenu )
            {
                writer.WriteLine( CreateTitle( extractionDvm.SettingVM.GetSetting( "ScriptSettings", "TitleForMenu-section" ).Value ) );
                writer.WriteLine( CreateAHKMenu() );
                writer.WriteLine( CreateTitle( extractionDvm.SettingVM.GetSetting( "ScriptSettings", "TitleForMenuTriggers-section" ).Value ) );
                writer.WriteLine( CreateAHKMenuTriggers() );
            }

            if ( extractionDvm.VariableVM.VariableList.Count > 0 )
            {
                writer.WriteLine( CreateTitle( extractionDvm.SettingVM.GetSetting( "ScriptSettings", "TitleForVariables-section" ).Value ) );
                writer.WriteLine( FetchItemsForScript( typeof( AhkVariable ), extractionDvm.SettingVM.IncludeMenu ) );
            }
            if ( extractionDvm.FunctionVM.FunctionList.Count > 0 )
            {
                writer.WriteLine( CreateTitle( extractionDvm.SettingVM.GetSetting( "ScriptSettings", "TitleForFunctions-section" ).Value ) );
                writer.WriteLine( FetchItemsForScript( typeof( AhkFunction ), extractionDvm.SettingVM.IncludeMenu ) );
            }
            if ( extractionDvm.HotstringVM.HotstringList.Count > 0 )
            {
                writer.WriteLine( CreateTitle( extractionDvm.SettingVM.GetSetting( "ScriptSettings", "TitleForHotstrings-section" ).Value ) );
                writer.WriteLine( FetchItemsForScript( typeof( AhkHotstring ), extractionDvm.SettingVM.IncludeMenu ) );
            }
            writer.WriteLine( "ExitApp" );

            writer.Close();
            return $"{ extractionDvm.SettingVM.ScriptLocation }.ahk";
        }

        internal static void WriteXml ( DataViewModel dvm )
        {
            using FileStream stream = new FileStream( $"{ Environment.GetEnvironmentVariable( "USERPROFILE" ) }\\AHKUpdaterData.xml", FileMode.Create );
            XmlSerializer xsz = new XmlSerializer( dvm.GetType() );

            xsz.Serialize( stream, dvm );
        }

        internal static void WriteExtractedXml ( DataViewModel dvm )
        {
            using FileStream stream = new FileStream( $"{ dvm.SettingVM.GetSetting( "Files", "XmlFileLocationForExtraction" ).Value }\\{ dvm.SettingVM.GetSetting( "Files", "FileName" ).Value }.xml", FileMode.Create );
            XmlSerializer xsz = new XmlSerializer( dvm.GetType() );

            xsz.Serialize( stream, dvm );
        }

        private static ReadOnlySpan<char> CreateAHKMenu ()
        {
            string menu = "";
            IOrderedEnumerable<AhkHotstring> list = modeltowrite.HotstringVM.HotstringList.OrderBy( x => x.System ).ThenBy( y => y.MenuTitle );
            foreach ( AhkHotstring h in list )
            {
                // How to create menu in AHK:
                // menu, SystemMenu, add, MenuTitle, commandname
                string menuName = $"{ h.System.Replace( " ", "", StringComparison.OrdinalIgnoreCase ) }Menu";
                menu += $"menu, { menuName }, add, { h.MenuTitle }, { h.Name }\r\n";
            }

            menu += "Return\r\n";

            return menu;
        }

        internal static object ImportFile ( string fileToImportPath )
        {
            using XmlReader stream = XmlReader.Create( new FileStream( fileToImportPath, FileMode.Open ) );
            try
            {
                return new XmlSerializer( typeof( ahk ) ).Deserialize( stream );
            }
            catch
            {
                return new XmlSerializer( typeof( DataViewModel ) ).Deserialize( stream );
            }
        }

        private static ReadOnlySpan<char> CreateAHKMenuTriggers ()
        {
            string menutriggers = "";
            IEnumerable<string> systems = modeltowrite.HotstringVM.HotstringList.Select( x => x.System ).Distinct();

            foreach ( string system in systems )
            {
                menutriggers += $"::{ modeltowrite.SettingVM.GetSetting( "ScriptOperations", "MenuShowTrigger" ).Value} {system }::\r\nmenu, { system }Menu, show, %A_CaretX%, %A_CaretY%\r\nReturn\r\n\r\n";
            }

            return menutriggers;
        }

        private static string CreateTitle ( string TitleSection )
        {
            string titleDivider = modeltowrite.SettingVM.GetSetting( "ScriptSettings", "TitleDividerCharacter" ).Value;
            for ( int i = 0; i < TitleSection.Length + 2; i++ )
            {
                titleDivider += modeltowrite.SettingVM.GetSetting( "ScriptSettings", "TitleDividerCharacter" ).Value;
            }
            return $"; { titleDivider }\r\n; { TitleSection }\r\n; { titleDivider }\r\n";
        }

        internal static DataViewModel Read ( FileInfo _xmlFile )
        {
            DataViewModel dvm;

            try
            {
                using XmlReader stream = XmlReader.Create( new FileStream( _xmlFile.FullName, FileMode.Open ) );
                dvm = (DataViewModel) new XmlSerializer( typeof( DataViewModel ) ).Deserialize( stream );
            }
            catch ( FileNotFoundException )
            {
                dvm = new DataViewModel
                {
                    XmlError = 1
                };
            }
            catch ( InvalidOperationException )
            {
                dvm = new DataViewModel
                {
                    XmlError = 2
                };
            }

            if ( dvm.SettingVM.SettingList.Count == 0 )
            {
                dvm.SettingVM.ResetAllDefault();
                dvm.SettingVM.GetSetting( "Files", "ScriptFileLocation" ).Value = _xmlFile.Directory.FullName;
                dvm.SettingVM.GetSetting( "Files", "ScriptFileLocation" ).DefaultValue = _xmlFile.Directory.FullName;
            }
            else if ( string.IsNullOrEmpty( dvm.SettingVM.GetSetting( "Files", "ScriptFileLocation" ).Value ) )
            {
                dvm.SettingVM.GetSetting( "Files", "ScriptFileLocation" ).Value = _xmlFile.Directory.FullName;
                dvm.SettingVM.GetSetting( "Files", "ScriptFileLocation" ).DefaultValue = _xmlFile.Directory.FullName;
            }
            dvm.InitiateFull();

            return dvm;
        }

        private static ReadOnlySpan<char> FetchItemsForScript ( Type type, bool includeMenu )
        {
            string data = "";
            if ( type == typeof( AhkVariable ) )
            {
                foreach ( AhkVariable v in modeltowrite.VariableVM.VariableList )
                {
                    data += $"{ v.Name } = { v.Value }\r\n";
                }
            }
            else if ( type == typeof( AhkHotstring ) )
            {
                foreach ( AhkHotstring h in modeltowrite.HotstringVM.HotstringList )
                {
                    if ( h.HsTypeIsAdvanced == false )
                    {
                        data += $"::{ h.Name }::\r\n{ ( includeMenu ? $"{ h.Name }:\r\n" : string.Empty ) }text=\r\n(\r\n{ h.Value }\r\n)\r\nPrintText(text)\r\nReturn\r\n\r\n";
                    }
                    else
                    {
                        data += $"::{ h.Name }::\r\n{ ( includeMenu ? $"{ h.Name }:\r\n" : string.Empty ) }\r\n{ h.Value }\r\n";
                    }
                }
            }
            else if ( type == typeof( AhkFunction ) )
            {
                foreach ( AhkFunction f in modeltowrite.FunctionVM.FunctionList )
                {
                    if ( f.ParameterList.Count > 0 )
                    {
                        string parameters = "";
                        foreach ( Parameter p in f.ParameterList )
                        {
                            parameters = string.IsNullOrEmpty( parameters )
                                  ? $"{ p.Name }"
                                  : $"{ parameters }, { p.Name }";
                        }
                        data += $"{ f.Name } ({ parameters })\r\n{ f.Value }\r\n\r\n";
                    }
                    else
                    {
                        data += $"{ f.Name } ()\r\n{ f.Value }\r\n\r\n";
                    }
                }
            }
            return data;
        }
    }
}
