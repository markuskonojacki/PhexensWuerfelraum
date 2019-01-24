using System.Windows;
using System.Windows.Controls;

namespace PhexensWuerfelraum.Ui.Desktop.CustomControls
{
    public class ChatMessagesListView : ListView
    {
        static ChatMessagesListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChatMessagesListView), new FrameworkPropertyMetadata(typeof(ChatMessagesListView)));
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return new Size(0, 0);
        }
    }
}