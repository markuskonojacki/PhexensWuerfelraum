using System;
using System.Windows.Data;
using System.Globalization;
using PhexensWuerfelraum.Logic.ClientServer;
using System.Diagnostics;
using System.Windows.Media;

namespace PhexensWuerfelraum.Ui.Desktop
{
    internal class MessageToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ChatMessageType messageType = (ChatMessageType)values[0];
            string message = values[1].ToString();

            if (messageType == ChatMessageType.Roll || messageType == ChatMessageType.RollWhisper)
            {
                if (message.Contains("Misslungen") || message.Contains("Doppel 20") || message.Contains("Dreifach 20"))
                    return Brushes.DarkRed;
                else if (message.Contains("Erfolg") || message.Contains("Doppel 1") || message.Contains("Dreifach 1"))
                    return Brushes.DarkGreen;
                else
                    return Brushes.Black;
            }
            else
            {
                return Brushes.Black;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}