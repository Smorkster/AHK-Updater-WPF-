using System;
using System.ComponentModel;

namespace AHK_Updater.Models
{
	public class MarkedItem
	{
		public delegate void UpdatedEventHandler ();
		public event UpdatedEventHandler DifferentChange;

		public MarkedItem () { }

		public string Name { get; set; }
		public string Text { get; set; }
		public string System { get; set; }
		public string DataType { get => Type; set => Type = value.ToLower(); }
		public string MenuTitle { get; set; }
		public bool IsEmpty { get { return Name.Equals( "" ); } }

		/// <summary>
		/// Control if data have been updated
		/// </summary>
		public bool DifferentFromOriginal
		{
			get { return DifferentFromOriginal1; }
			set
			{
				DifferentFromOriginal1 = value;
				if ( DifferentChange != null )
					DifferentChange();
			}
		}

		public string Type { get; set; }
		public bool DifferentFromOriginal1 { get; set; } = false;

		/// <summary>
		/// Clear object
		/// </summary>
		internal void Clear ()
		{
			Name = Text = System = Type = MenuTitle = "";
			DifferentFromOriginal = false;
		}

		/// <summary>
		/// Sets the parameters for the object
		/// </summary>
		/// <param name="name">Name of object</param>
		/// <param name="text">Text of object</param>
		/// <param name="system">System of object</param>
		/// <param name="type">Type of object</param>
		/// <param name="menutitle">Menutitle of object</param>
		internal void Set ( string name, string text, string system, string type, string menutitle )
		{
			Name = name;
			Text = text;
			System = system;
			Type = type;
			MenuTitle = menutitle;
			DifferentFromOriginal = false;
		}

		/// <summary>
		/// Sets the parameters for the object
		/// </summary>
		/// <param name="name">Name of object</param>
		/// <param name="text">Text of object</param>
		/// <param name="type">Type of object</param>
		internal void Set ( string name, string text, string type )
		{
			Name = name;
			Text = text;
			Type = type;
			System = "";
			MenuTitle = "";
			DifferentFromOriginal = false;
		}
	}
}
