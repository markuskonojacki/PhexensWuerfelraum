using MahApps.Metro.Controls.Dialogs;
using PhexensWuerfelraum.Logic.Ui;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PhexensWuerfelraum.Ui.Desktop
{
    /// <summary>
    /// Interaction logic for ChatnRollPage.xaml
    /// </summary>
    public partial class ChatnRoll
    {
        #region properties

        private MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

        public CustomDialog _customCloseDialogTest;

        public CustomDialog CustomCloseDialogTest()
        {
            CustomDialog dialog = new CustomDialog();

            if (_customCloseDialogTest == null)
            {
                ResourceDictionary resdict = (ResourceDictionary)Resources["ChatnRollResourceDictionary"];

                foreach (var item in resdict.Values)
                {
                    if (item.GetType() == typeof(CustomDialog))
                    {
                        CustomDialog custDiag = (CustomDialog)item;

                        if (custDiag.Name == "CustomCloseDialogTest")
                            dialog = (CustomDialog)item;
                    }
                }
            }
            else
            {
                dialog = _customCloseDialogTest;
            }

            return dialog;
        }

        #endregion properties

        public ChatnRoll()
        {
            InitializeComponent();
        }

        private async void OpenTrialModifierDialog()
        {
            EventHandler<DialogStateChangedEventArgs> dialogManagerOnDialogOpened = null;
            dialogManagerOnDialogOpened = (o, args) =>
            {
                DialogManager.DialogOpened -= dialogManagerOnDialogOpened;
                Console.WriteLine("Custom Dialog opened!");
            };
            DialogManager.DialogOpened += dialogManagerOnDialogOpened;

            EventHandler<DialogStateChangedEventArgs> dialogManagerOnDialogClosed = null;
            dialogManagerOnDialogClosed = (o, args) =>
            {
                DialogManager.DialogClosed -= dialogManagerOnDialogClosed;
                Console.WriteLine("Custom Dialog closed!");
            };
            DialogManager.DialogClosed += dialogManagerOnDialogClosed;

            var dialog = CustomCloseDialogTest();

            await mainWindow.ShowMetroDialogAsync(dialog);
            await dialog.WaitUntilUnloadedAsync();
        }

        private async void CloseCustomDialog(object sender, RoutedEventArgs e)
        {
            var dialog = CustomCloseDialogTest();

            await mainWindow.HideMetroDialogAsync(dialog);
            await mainWindow.ShowMessageAsync("Dialog gone", "The custom dialog has closed");
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var context = (ChatnRollViewModel)DataContext;
                context.SendTextCommand.Execute(null);
            }
        }

        private void ListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var ScrollViewer = Template.FindName("PageScrollViewer", this) as ScrollViewer;
            ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset - e.Delta / 3);
        }

        private void Trials_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            TrialScrollViewer.ScrollToVerticalOffset(TrialScrollViewer.VerticalOffset - e.Delta / 3);
        }
    }
}