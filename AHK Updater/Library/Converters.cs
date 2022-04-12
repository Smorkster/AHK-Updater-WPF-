using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace AHKUpdater.Converters
{
    public class IsHidden : IValueConverter
    {
        public object Convert ( object value, Type targetType, object parameter, CultureInfo culture )
        {
            return ( (FileAttributes) value ).HasFlag( FileAttributes.Hidden );
        }

        public object ConvertBack ( object value, Type targetType, object parameter, CultureInfo culture ) => throw new NotImplementedException();
    }
    public class IsSystem : IValueConverter
    {
        public object Convert ( object value, Type targetType, object parameter, CultureInfo culture )
        {
            return ( (FileAttributes) value ).HasFlag( FileAttributes.System );
        }

        public object ConvertBack ( object value, Type targetType, object parameter, CultureInfo culture ) => throw new NotImplementedException();
    }
}
