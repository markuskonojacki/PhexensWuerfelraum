using System.Windows.Controls;

namespace PhexensWuerfelraum.Ui.Desktop
{
    /// <summary>
    /// Interaction logic for BattleMapPage.xaml
    /// </summary>
    public partial class CharacterPage
    {
        public CharacterPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Pass the scroll event through to the defined ScrollViewer in Resources\MyStyle.xaml
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var ScrollViewer = Template.FindName("PageScrollViewer", this) as ScrollViewer;
            ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset - e.Delta / 3);
        }
    }
}