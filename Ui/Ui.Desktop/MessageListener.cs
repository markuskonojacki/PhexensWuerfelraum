using System;
using System.Windows;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using PhexensWuerfelraum.Logic.Ui;

namespace PhexensWuerfelraum.Ui.Desktop
{
    public class MessageListener
    {
        #region properties

        public bool IsEnabled => true;
        private PickHeroDialog PickHeroDialog { get; set; }
        private CharacterViewModel CharacterViewModel { get; set; }
        private ChatnRollViewModel ChatnRollViewModel { get; set; }
        private MetroWindow metroWindow;

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
                async msg =>
                {
                    Log.Instance.Trace("OpenInfoMessage called");

                    metroWindow = Application.Current.MainWindow as MetroWindow;
                    await metroWindow.ShowMessageAsync(msg.InfoTitle, msg.InfoText);
                });

            Messenger.Default.Register<OpenHeroPickDialogMessage>(
                this,
                async msg =>
                {
                    Log.Instance.Trace("OpenHeroPickDialogMessage called");

                    if (CharacterViewModel.IsChildWindowOpenOrNotProperty == false)
                    {
                        PickHeroDialog = new PickHeroDialog() { };
                        PickHeroDialog.ClosingFinished += PickHeroDialog_ClosingFinished;
                        CharacterViewModel.IsChildWindowOpenOrNotProperty = true;

                        await ChildWindowManager.ShowChildWindowAsync(Application.Current.MainWindow, PickHeroDialog);

                        if (ChatnRollViewModel.ChatRoom.Connected)
                        {
                            metroWindow = Application.Current.MainWindow as MetroWindow;
                            metroWindow.Dispatcher.Invoke(new Action(() => { ChatnRollViewModel.Reconnect(); }));
                        }
                    }
                });
        }

        private void PickHeroDialog_ClosingFinished(object sender, RoutedEventArgs e)
        {
            PickHeroDialog = null;
            CharacterViewModel.IsChildWindowOpenOrNotProperty = false;
        }

        #endregion methods
    }
}