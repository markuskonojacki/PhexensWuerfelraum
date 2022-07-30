using System.Windows.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using MahApps.Metro.Controls;
using PhexensWuerfelraum.Logic.Ui;

namespace PhexensWuerfelraum.Ui.Desktop
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();

            TextBoxHelper.SetWatermark(PublicKey, $"-----BEGIN CERTIFICATE----- \n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n -----END CERTIFICATE-----");
        }

        private void StaticUserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //CharacterViewModel CharacterViewModel = Ioc.Default.GetService<CharacterViewModel>();
            //CharacterViewModel.LoadCharacterList();
        }

        private void UninstallButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Ioc.Default.GetService<SettingsViewModel>().Uninstall();
        }
    }
}