

using System;
using Xamarin.Forms;
using XamFormsReactiveUI.ValueConverters;

namespace XamFormsReactiveUI.Views
{
    public class Badge : AbsoluteLayout
    {
        /// <summary>
        /// The text property.
        /// </summary>
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create("Text", typeof(String), typeof(Badge), "");

        /// <summary>
        /// The box color property.
        /// </summary>
        public static readonly BindableProperty BoxColorProperty =
            BindableProperty.Create("BoxColor", typeof(Color), typeof(Badge), Color.Default);

        /// <summary>
        /// The text.
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the box.
        /// </summary>
        public Color BoxColor
        {
            get { return (Color)GetValue(BoxColorProperty); }
            set { SetValue(BoxColorProperty, value); }
        }

        /// <summary>
        /// The box.
        /// </summary>
        protected RoundedBox Box;

        /// <summary>
        /// The label.
        /// </summary>
        protected Label Label;

        /// <summary>
        /// Initializes a new instance of the <see cref="Badge"/> class.
        /// </summary>
        public Badge(double size, double fontSize)
        {
            HeightRequest = size;
            WidthRequest = HeightRequest;

            // Box
            Box = new RoundedBox
            {
                CornerRadius = HeightRequest / 2
            };
            Box.SetBinding(BackgroundColorProperty, new Binding("BoxColor", source: this));
            Children.Add(Box, new Rectangle(0, 0, 1.0, 1.0), AbsoluteLayoutFlags.All);

            // Label
            Label = new Label
            {
                TextColor = Color.White,
                FontSize = fontSize,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
            Label.SetBinding(Label.TextProperty, new Binding("Text",
                BindingMode.OneWay, source: this));
            Children.Add(Label, new Rectangle(0, 0, 1.0, 1.0), AbsoluteLayoutFlags.All);

            // Auto-width
            SetBinding(WidthRequestProperty, new Binding("Text", BindingMode.OneWay,
                new BadgeWidthConverter(WidthRequest), source: this));

            // Hide if no value
            SetBinding(IsVisibleProperty, new Binding("Text", BindingMode.OneWay,
                new BadgeVisibleValueConverter(), source: this));

            // Default color
            BoxColor = Color.Red;
        }
    }
}
