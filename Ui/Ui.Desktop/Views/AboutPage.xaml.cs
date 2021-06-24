using System.Diagnostics;
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
            var psi = new ProcessStartInfo
            {
                FileName = "https://github.com/markuskonojacki/PhexensWuerfelraum/blob/master/LICENSE.txt",
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void ThirdPartyLicenseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "https://github.com/markuskonojacki/PhexensWuerfelraum/blob/master/THIRD-PARTY-LICENSES.txt",
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void ChangelogButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "https://github.com/markuskonojacki/PhexensWuerfelraum/releases",
                UseShellExecute = true
            };
            Process.Start(psi);
        }
    }
}