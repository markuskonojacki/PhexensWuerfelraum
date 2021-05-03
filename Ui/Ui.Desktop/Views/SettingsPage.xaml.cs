using MahApps.Metro.Controls;
using System.Windows.Controls;

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
            //CharacterViewModel CharacterViewModel = SimpleIoc.Default.GetInstance<CharacterViewModel>();
            //CharacterViewModel.LoadCharacterList();
        }
    }
}