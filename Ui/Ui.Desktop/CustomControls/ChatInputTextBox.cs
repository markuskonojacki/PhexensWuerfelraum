using System.Windows;
using System.Windows.Controls;

namespace PhexensWuerfelraum.Ui.Desktop.CustomControls
{
    // https://stackoverflow.com/a/42370393/7557790
    public class ChatInputTextBox : TextBox
    {
        static ChatInputTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChatInputTextBox), new FrameworkPropertyMetadata(typeof(ChatInputTextBox)));
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return new Size(0, 0);
        }
    }
}