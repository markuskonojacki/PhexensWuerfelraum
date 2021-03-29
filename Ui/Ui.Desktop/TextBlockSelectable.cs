using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace PhexensWuerfelraum.Ui.Desktop
{
    // https://stackoverflow.com/a/32870521
    public partial class TextBlockSelectable : TextBlock
    {
        private TextPointer StartSelectPosition;
        private TextPointer EndSelectPosition;
        public String SelectedText = "";

        public delegate void TextSelectedHandler(string SelectedText);

        public event TextSelectedHandler TextSelected;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            Point mouseDownPoint = e.GetPosition(this);
            StartSelectPosition = this.GetPositionFromPoint(mouseDownPoint, true);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            Point mouseUpPoint = e.GetPosition(this);
            EndSelectPosition = this.GetPositionFromPoint(mouseUpPoint, true);

            TextRange otr = new TextRange(this.ContentStart, this.ContentEnd);
            otr.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.Transparent));

            if (StartSelectPosition != null && EndSelectPosition != null)
            {
                TextRange ntr = new TextRange(StartSelectPosition, EndSelectPosition);
                ntr.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.Gray));

                SelectedText = ntr.Text;
                TextSelected?.Invoke(SelectedText);
            }
        }
    }
}