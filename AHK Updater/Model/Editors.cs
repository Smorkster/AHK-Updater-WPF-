using System.IO;

namespace AHKUpdater.Model
{
	public class Editor
	{
		public Editor () { }
		public Editor ( string Editor )
		{
			Path = new FileInfo( Editor );
		}

		public string Name => Path.Name;

		public FileInfo Path { get; set; }
	}
}
