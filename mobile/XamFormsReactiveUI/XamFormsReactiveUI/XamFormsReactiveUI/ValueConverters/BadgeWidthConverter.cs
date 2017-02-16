using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamFormsReactiveUI.ValueConverters
{
    /// <summary>
    /// Badge width converter.
    /// </summary>
    class BadgeWidthConverter : IValueConverter
    {
        /// <summary>
        /// The width of the base.
        /// </summary>
        readonly double baseWidth;

        /// <summary>
        /// The width ratio.
        /// </summary>
        const double widthRatio = 0.33;

        public BadgeWidthConverter(double baseWidth)
        {
            this.baseWidth = baseWidth;
        }

        #region IValueConverter implementation

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var text = value as string;
            if ((text != null) && (text.Length > 1))
            {
                // We won't measure text length exactly here!
                // May be we should, but it's too tricky. So,
                // we'll just approximate new badge width as
                // linear function from text legth.

                return baseWidth * (1 + widthRatio * (text.Length - 1));
            }
            return baseWidth;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
