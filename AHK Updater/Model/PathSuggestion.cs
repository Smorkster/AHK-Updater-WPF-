using System.IO;

namespace AHKUpdater.Model
{
    public class PathSuggestion
    {
        private DirectoryInfo _path;
        private bool _sameAsCurrent;

        public PathSuggestion ()
        {
        }

        public PathSuggestion ( DirectoryInfo path, bool sameascurrent )
        {
            Path = path;
            SameAsCurrent = sameascurrent;
        }

        public DirectoryInfo Path
        {
            get { return _path; }
            set { _path = value; }
        }

        public bool SameAsCurrent
        {
            get { return _sameAsCurrent; }
            set { _sameAsCurrent = value; }
        }
    }
}
