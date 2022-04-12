using System.Collections.ObjectModel;
using System.Globalization;
using System.Resources;

namespace AHKUpdater.Library
{
    public static class Extras
    {
        public static readonly string RegexToDisplayName = "((?<=\\p{Ll})\\p{Lu}|\\p{Lu}(?=\\p{Ll}))";
        public static ObservableCollection<string> GetAvailableCultures ()
        {
            ObservableCollection<string> result = new ObservableCollection<string>();

            foreach ( CultureInfo culture in CultureInfo.GetCultures( CultureTypes.AllCultures ) )
            {
                try
                {
                    if ( culture.Equals( CultureInfo.InvariantCulture ) )
                    {
                        continue;
                    }

                    if ( new ResourceManager( typeof( Localization.Localization ) ).GetResourceSet( culture, true, false ) != null )
                    {
                        result.Add( culture.Name );
                    }
                }
                catch ( CultureNotFoundException ) { }
            }
            return result;
        }
    }
}
