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
            Process.Start("https://github.com/Derevar/PhexensWuerfelraum/blob/master/LICENSE.txt");
        }

        private void ThirdPartyLicenseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start("https://github.com/Derevar/PhexensWuerfelraum/blob/master/THIRD-PARTY-LICENSES.txt");
        }

        private void ChangelogButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start("https://github.com/Derevar/PhexensWuerfelraum/releases");
        }
    }
}