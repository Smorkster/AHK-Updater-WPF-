using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace AHK_Updater.Library
{
	public class ValidateHotstringCode : ValidationRule
	{
		public ValidateHotstringCode () { }

		public override ValidationResult Validate ( object value, CultureInfo cultureInfo )
		{
			if ( ( ( string ) value ).Length > 0 )
			{
				return ValidationResult.ValidResult;
			}
			else
			{
				return new ValidationResult( false, "Code must contain something" );
			}
		}
	}
}
