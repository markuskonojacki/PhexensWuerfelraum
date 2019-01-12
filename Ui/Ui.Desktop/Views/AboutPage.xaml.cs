using System.Windows.Controls;

namespace PhexensWuerfelraum.Ui.Desktop.Views
{
    /// <summary>
    /// Interaction logic for AboutPage.xaml
    /// </summary>
    public partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        public string Version
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
        }
    }
}