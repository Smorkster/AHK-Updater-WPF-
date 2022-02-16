using System.IO;

namespace AHK_Updater.Models
{
	public class Editor
	{
		public Editor () { }
		public Editor ( string Editor )
		{
			Path = new FileInfo( Editor );
		}

		public string Name
		{
			get
			{
				return Path.Name;
			}
		}

		public FileInfo Path
		{
			get
			{
				return Path;
			}
			set
			{
				Path = value;
			}
		}
	}
}
