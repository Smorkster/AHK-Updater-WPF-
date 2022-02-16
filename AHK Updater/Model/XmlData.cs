using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace AHK_Updater.Models
{
	/*	/// <summary>
		/// Handling data from the XML-file
		/// Example XML:
		/// <ahk>
		/// <variables>
		/// <variable variableName=""></variable>
		/// </variables>
		/// <hotstrings>
		/// <hotstring hotstringName="" hotstringSystem=""></hotstring>
		/// </hotstrings>
		/// <functions>
		/// <function functionName=""></function>
		/// </functions>
		/// <changelog>
		/// <change version=""></change>
		/// </changelog>
		/// </ahk>
		/// </summary>
		internal class XmlData
		{
			XElement XmlDoc;
			readonly bool OldFormat;
			WriteData WriteData;

			public XmlData ( WriteData w )
			{
				WriteData = w;
			}

			public XmlData ( XElement d )
			{
				XmlDoc = d;
				if ( XmlDoc.FirstNode.ToString().Contains( "ahkcommand" ) )
				{
					OldFormat = true;
				}
				else
				{
					OldFormat = false;
				}
			}

			/// <summary>
			/// Get all hotstrings from XML and create a list
			/// </summary>
			public ObservableCollection<Hotstring> GetHotstrings ()
			{
				ObservableCollection<Hotstring> hotstringData = new ObservableCollection<Hotstring>();
				if ( OldFormat )
				{
					IEnumerable<XElement> items = XmlDoc.Elements( "ahkcommand" ).Where( x => x.Descendants().First().ToString().Contains( "command" ) );
					foreach ( XElement item in items )
					{
						if ( !item.Descendants( "system" ).First().Value.Contains( "Variables" ) )
						{
							hotstringData.Add( new Hotstring( item.Descendants( "command" ).First().Value,
															item.Descendants( "text" ).First().Value,
															item.Descendants( "system" ).First().Value,
															$"MenuItem for { item.Descendants( "command" ).First().Value }" ) );
						}
					}
				}
				else
				{
					IEnumerable<XElement> hotstrings = XmlDoc.Descendants( "hotstrings" );
					if ( hotstrings.Any() )
					{
						foreach ( XElement item in hotstrings.Descendants( "hotstring" ) )
						{
							string title = "";
							try
							{
								if ( !item.Attribute( "hotstringMenuTitle" ).Value.Equals( "" ) )
									title = item.Attribute( "hotstringMenuTitle" ).Value;
							}
							catch { title = $"Menuitem for {item.Attribute( "hotstringName" ).Value }"; }
							hotstringData.Add( new Hotstring( item.Attribute( "hotstringName" ).Value,
															item.Value.Replace( "\n", "\r\n" ),
															item.Attribute( "hotstringSystem" ).Value,
															title ) );
						}
					}
				}

				return hotstringData;
			}

			/// <summary>
			/// Get all functions from XML and create a list
			/// </summary>
			public ObservableCollection<Function> GetFunctions ()
			{
				ObservableCollection<Function> functionData = new ObservableCollection<Function>();
				if ( OldFormat )
				{
					IEnumerable<XElement> items = XmlDoc.Elements( "ahkcommand" ).Where( x => x.Descendants().First().ToString().Contains( "functionname" ) );
					foreach ( XElement item in items )
					{
						functionData.Add( new Function( item.Descendants( "functionname" ).First().Value,
													item.Descendants( "functiontext" ).First().Value ) );
					}
				}
				else
				{
					IEnumerable<XElement> functions = XmlDoc.Descendants( "functions" );
					if ( XmlDoc.Descendants( "functions" ).Any() )
					{
						foreach ( XElement item in functions.Descendants( "function" ) )
						{
							functionData.Add( new Function( item.Attribute( "functionName" ).Value,
													item.Value.Replace( "\n", "\r\n" ) ) );
						}
					}
				}

				return functionData;
			}

			/// <summary>
			/// Get all changes from XML and create a list
			/// </summary>
			public ObservableCollection<Change> GetChangelog ()
			{
				ObservableCollection<Change> changeData = new ObservableCollection<Change>();
				if ( OldFormat )
				{
					IEnumerable<XElement> items = XmlDoc.Elements( "ahkcommand" ).Where( x => x.Descendants().First().ToString().Contains( "version" ) );
					foreach ( XElement item in items )
					{
						changeData.Add( new Change( item.Descendants( "version" ).First().Value,
													item.Descendants( "entry" ).First().Value ) );
					}
				}
				else
				{
					IEnumerable<XElement> changelog = XmlDoc.Descendants( "changelog" );
					if ( changelog.Any() )
					{
						if ( changelog.Any() )
						{
							foreach ( XElement item in changelog.Descendants( "change" ) )
							{
								changeData.Add( new Change( item.Attribute( "version" ).Value,
															item.Value ) );
							}
						}
					}
				}

				return changeData;
			}

			/// <summary>
			/// Get all variables from XML and create a list
			/// </summary>
			public ObservableCollection<Variable> GetVariables ()
			{
				ObservableCollection<Variable> variableData = new ObservableCollection<Variable>();
				if ( OldFormat )
				{
					IEnumerable<XElement> items = XmlDoc.Elements( "ahkcommand" ).Elements( "system" ).Where( x => x.Value.Equals( "Variables" ) );
					foreach ( XElement item in items )
					{
						var nodes = item.Parent.Descendants().ToArray();
						variableData.Add( new Variable( nodes[0].Value.ToString(),
													nodes[1].Value.ToString() ) );
					}
				}
				else
				{
					IEnumerable<XElement> variables = XmlDoc.Descendants( "variables" );
					if ( variables.Any() )
					{
						foreach ( XElement item in variables.Descendants( "variable" ) )
						{
							variableData.Add( new Variable( item.Attribute( "variableName" ).Value,
															item.Value ) );
						}
					}
				}

				return variableData;
			}

			/// <summary>
			/// Get all settings from XML and create an settingsobject
			/// </summary>
			public SettingViewModel GetSettings ()
			{
				if ( !OldFormat )
				{
					SettingViewModel settingData = new SettingViewModel();

					IEnumerable<XElement> settings = XmlDoc.Descendants( "settings" );
					if ( settings.Any() )
					{
						foreach ( XElement item in settings.Descendants( "setting" ) )
						{
							settingData.Add( new Setting( item.Attribute( "settingName" ).Value,
														item.Value,
														item.Attribute( "defaultValue" ).Value ) );
						}
					}
					return settingData;
				}
				return null;
			}

			/// <summary>
			/// Create a XDocument to be written to file
			/// </summary>
			/// <returns>XDocument for writing to file</returns>
			public XElement CreateXMLDocument ()
			{
				XmlDoc = new XElement(
						new XElement( "ahk",
							new XElement( "settings",
								WriteData.GetSettings().Select( s => new XElement( "setting",
																							 new XAttribute( "settingName",
																											 s.Name ),
																							 new XAttribute( "defaultValue",
																											 s.DefaultValue ),
																							 s.Value ) )
							),
							new XElement( "variables",
								WriteData.GetVariables().Select( v => new XElement( "variable", new XAttribute( "variableName", v.Name ),
																					 v.Value ) )
							),
							new XElement( "hotstrings",
								WriteData.GetHotstrings().Select( h => new XElement( "hotstring",
																					 new XAttribute( "hotstringName",
																									 h.Name ),
																					 new XAttribute( "hotstringSystem",
																									 h.System ),
																					 new XAttribute( "hotstringMenuTitle",
																									 h.MenuTitle ),
																					 h.Value ) )
							),
							new XElement( "functions",
								WriteData.GetFunctions().Select( f => new XElement( "function",
																					 new XAttribute( "functionName",
																									 f.Name ),
																					 f.Value ) )
							),
							new XElement( "changelog",
								WriteData.GetChanges().Select( c => new XElement( "change",
																					 new XAttribute( "version", c.Name ),
																					 c.Value ) )
							)
						)
					);

				return XmlDoc;
			}
		}
	}*/
}