using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using PhexensWuerfelraum.Logic.Ui;

namespace PhexensWuerfelraum.Ui.Desktop
{
    /// <summary>
    /// Interaction logic for ChatnRollPage.xaml
    /// </summary>
    public partial class ChatnRoll
    {
        #region properties

        private readonly MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

        public CustomDialog _customCloseDialogTest;

        private List<string> messageHistory = new();
        private int currentMessageHistoryIndex = 0;

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

        private async void CloseCustomDialog(object sender, RoutedEventArgs e)
        {
            var dialog = CustomCloseDialogTest();

            await mainWindow.HideMetroDialogAsync(dialog);
            await mainWindow.ShowMessageAsync("Dialog gone", "The custom dialog has closed");
        }

        private void ChatInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var context = (ChatnRollViewModel)DataContext;

                messageHistory.Add(context.Message);

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

        private void SelectableMessageTextBlock_TextSelected(string SelectedText)
        {
            Clipboard.SetDataObject(SelectedText);
        }

        private void ChatInputTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (messageHistory.Count > 0)
            {
                var context = (ChatnRollViewModel)DataContext;

                if (e.Key == Key.Up)
                {
                    if (currentMessageHistoryIndex <= 0)
                        currentMessageHistoryIndex = messageHistory.Count;

                    context.Message = messageHistory[currentMessageHistoryIndex - 1];
                    currentMessageHistoryIndex--;
                }
                else if (e.Key == Key.Down)
                {
                    if (currentMessageHistoryIndex >= messageHistory.Count - 1)
                        currentMessageHistoryIndex = 0;

                    context.Message = messageHistory[currentMessageHistoryIndex + 1];
                    currentMessageHistoryIndex++;
                }
            }
        }

        private void StackPanel_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }
    }
}