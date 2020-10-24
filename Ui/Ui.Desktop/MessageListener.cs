using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PhexensWuerfelraum.Logic.Ui;
using System.Windows;

namespace PhexensWuerfelraum.Ui.Desktop
{
    public class MessageListener
    {
        #region properties

        public bool IsEnabled => true;
        private CharacterViewModel CharacterViewModel { get; set; }
        private ChatnRollViewModel ChatnRollViewModel { get; set; }

        #endregion properties

        #region constructors and destructors

        public MessageListener()
        {
            InitMessenger();
        }

        #endregion constructors and destructors

        #region methods

        private void InitMessenger()
        {
            CharacterViewModel = SimpleIoc.Default.GetInstance<CharacterViewModel>();
            ChatnRollViewModel = SimpleIoc.Default.GetInstance<ChatnRollViewModel>();

            Messenger.Default.Register<OpenInfoMessage>(
                this,
                msg =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var mySettings = new MetroDialogSettings()
                        {
                            AffirmativeButtonText = "Ok",
                            DialogButtonFontSize = 20D
                        };

                        (Application.Current.MainWindow as MetroWindow).ShowMessageAsync(msg.InfoTitle, msg.InfoText, MessageDialogStyle.Affirmative, mySettings);
                    });
                });
        }

        #endregion methods
    }
}