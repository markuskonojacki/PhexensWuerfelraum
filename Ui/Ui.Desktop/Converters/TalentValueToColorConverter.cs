using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PhexensWuerfelraum.Ui.Desktop.Converters
{
    class TalentValueToColorConverter : IValueConverter
    {
        /// <summary>
        /// color coded skill levels based on WdS S. 12
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                SolidColorBrush color;
                int taw = (int)value;

                if (taw < 0)
                {
                    color = (SolidColorBrush)new BrushConverter().ConvertFrom("#707070"); // light gray
                }
                else if (taw <= 7)
                {
                    color = (SolidColorBrush)new BrushConverter().ConvertFrom("#222222"); // dark gray
                }
                else if (taw <= 10)
                {
                    color = (SolidColorBrush)new BrushConverter().ConvertFrom("#2DA52F"); // green
                }
                else if (taw <= 15)
                {
                    color = (SolidColorBrush)new BrushConverter().ConvertFrom("#3E89CE"); // blue
                }
                else if (taw <= 18)
                {
                    color = (SolidColorBrush)new BrushConverter().ConvertFrom("#8237C8"); // purple
                }
                else if (taw <= 20)
                {
                    color = (SolidColorBrush)new BrushConverter().ConvertFrom("#D5B500"); // gold
                }
                else
                {
                    color = (SolidColorBrush)new BrushConverter().ConvertFrom("#EA4E25"); // orange
                }

                return color;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
