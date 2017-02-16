

using System;
using Xamarin.Forms;

namespace XamFormsReactiveUI.ValueConverters
{
    internal class CheckVisibleValueConverter : IValueConverter
    {
        #region IValueConverter implementation

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var isAnswered = (bool)value;
            return isAnswered;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
