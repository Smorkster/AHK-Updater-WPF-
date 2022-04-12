using AHKUpdater.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace AHKUpdater.Validations
{
    public static class ValidateCulture
    {
        private static readonly HashSet<string> CultureNames = CreateCultureNames();

        internal static bool Exists ( string name ) { return CultureNames.Contains( name, StringComparer.OrdinalIgnoreCase ); }

        private static HashSet<string> CreateCultureNames ()
        {
            CultureInfo[] cultureInfos = CultureInfo.GetCultures( CultureTypes.AllCultures )
                                          .Where( x => !string.IsNullOrEmpty( x.Name ) )
                                          .ToArray();
            HashSet<string> allNames = new HashSet<string>( StringComparer.OrdinalIgnoreCase );
            allNames.UnionWith( cultureInfos.Select( x => x.TwoLetterISOLanguageName ) );
            allNames.UnionWith( cultureInfos.Select( x => x.Name ) );
            return allNames;
        }
    }

    public class ValidateDirectoryPath : ValidationRule
    {
        public override ValidationResult Validate ( object value, CultureInfo cultureInfo )
        {
            string suggestedPath = ( (Setting) ( (BindingExpression) value ).ResolvedSource ).Value;
            return Directory.Exists( suggestedPath ) ? ValidationResult.ValidResult : new ValidationResult( false, "" );
        }
    }

    public class ValidateSettingValue : ValidationRule
    {
        public override ValidationResult Validate ( object value, CultureInfo cultureInfo )
        {
            string text = ( (Setting) ( (BindingExpression) value ).ResolvedSource ).Value;
            return string.IsNullOrEmpty( text ) ? new ValidationResult( false, "" ) : ValidationResult.ValidResult;
        }
    }
    public class ValidateSettingFileName : ValidationRule
    {
        public override ValidationResult Validate ( object value, CultureInfo cultureInfo )
        {
            //string text = ( (Setting) ( (BindingExpression) value ).ResolvedSource ).Value;
            //return string.IsNullOrEmpty( text ) ? new ValidationResult( false, "" ) : ValidationResult.ValidResult;
            return ValidationResult.ValidResult;
        }
    }
}
