using GalaSoft.MvvmLight.Ioc;
using PhexensWuerfelraum.Logic.ClientServer;
using PhexensWuerfelraum.Logic.Ui;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PhexensWuerfelraum.Ui.Desktop
{
    public class MultiValueMessageFromConverter : IMultiValueConverter
    {
        public CharacterModel Character { get; set; } = SimpleIoc.Default.GetInstance<CharacterViewModel>().Character;
        public SettingsModel Settings { get; set; } = SimpleIoc.Default.GetInstance<SettingsViewModel>().Setting;

        /// <param name="values">
        ///     <Binding Path="MessageType" />
        ///     <Binding Path="FromUsername" />
        ///     <Binding Path="ToUsername" />
        /// </param>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string ret = "";
            ChatMessageType messageType = (ChatMessageType)values[0];
            string fromUsername = values[1].ToString();
            string toUsername = values[2]?.ToString();
            string ownName = "";

            if (Settings.StaticUserName != "")
            {
                ownName = Settings.StaticUserName;
            }
            else
            {
                ownName = Character.Name;
            }

            switch (messageType)
            {
                case ChatMessageType.Action:
                    ret = $"{fromUsername} ";
                    break;

                case ChatMessageType.Text:
                    ret = $"{fromUsername}: ";
                    break;

                case ChatMessageType.Roll:
                    ret = $"{fromUsername} würfelt ";
                    break;

                case ChatMessageType.Whisper:
                    if (fromUsername == ownName)
                    {
                        ret = $"Du flüsterst {toUsername}: ";
                    }
                    else if (toUsername == ownName)
                    {
                        ret = $"{fromUsername} flüstert dir: ";
                    }
                    else
                    {
                        ret = $"{fromUsername} flüstert an {toUsername}: ";
                    }
                    break;

                case ChatMessageType.RollWhisper:
                    if (fromUsername == ownName)
                    {
                        ret = $"Du würfelst privat an {toUsername} ";
                    }
                    else if (toUsername == ownName)
                    {
                        ret = $"{fromUsername} würfelt dir privat ";
                    }
                    else
                    {
                        ret = $"{fromUsername} würfelt privat an {toUsername} ";
                    }
                    break;
            }

            return ret;
        }

        public object[] ConvertBack(object values, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}