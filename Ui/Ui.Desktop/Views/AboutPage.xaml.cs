using System.Diagnostics;
using System.IO;
using System.Reflection;
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

        private void LicenseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "LICENSE.txt");
            Process.Start(path);
        }

        private void ThirdPartyLicenseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start(Path.Combine(System.Environment.CurrentDirectory, "THIRD-PARTY-LICENSES.txt"));
        }
    }
}