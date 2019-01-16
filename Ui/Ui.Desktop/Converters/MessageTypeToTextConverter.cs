using PhexensWuerfelraum.Logic.ClientServer;
using System;
using System.Windows.Data;

namespace PhexensWuerfelraum.Ui.Desktop
{
    internal class MessageTypeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var ret = "";

            switch (value)
            {
                case MessageType.Action:
                    ret = " ";
                    break;

                case MessageType.Text:
                    ret = ": ";
                    break;

                case MessageType.Roll:
                    ret = " würfelt auf ";
                    break;
            }

            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}