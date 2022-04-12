using AHKUpdater.Model;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

namespace AHKUpdater.Library
{
    internal static class Extensions
    {
        public static ObservableCollection<string> GetDisplayNameSettingGroups<T> ( this ObservableCollection<T> c )
        {
            Contract.Requires( c != null );
            ObservableCollection<string> collection = new ObservableCollection<string>();
            foreach ( T i in c )
            {
                collection.Add( Regex.Replace( i.ToString(), Extras.RegexToDisplayName, " $1" ).Trim() );
            }

            return collection;
        }

        public static void RemoveMessage ( this ObservableCollection<Message> c, string msg )
        {
            foreach ( Message f in c )
            {
                if ( f.Msg.Equals( msg, StringComparison.OrdinalIgnoreCase ) )
                {
                    _ = c.Remove( f );
                }
            }
        }

        public static string ToDisplayName ( this string t ) => Regex.Replace( t, Extras.RegexToDisplayName, " $1" ).Trim().FirstCharToUpper();

        public static string FirstCharToUpper ( this string input ) =>
        input switch
        {
            null => throw new ArgumentNullException( nameof( input ) ),
            "" => throw new ArgumentException( $"{nameof( input )} cannot be empty", nameof( input ) ),
            _ => string.Concat( input[ 0 ].ToString().ToUpper(), input.AsSpan( 1 ) )
        };

        internal static ObservableCollection<PathSuggestion> NotNullAdd<PathSuggestion> ( this ObservableCollection<PathSuggestion> c, PathSuggestion o )
        {
            if ( o != null )
            {
                c.Add( o );
            }

            return c;
        }
    }
}