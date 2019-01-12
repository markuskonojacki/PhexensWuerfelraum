using GalaSoft.MvvmLight.Ioc;
using PhexensWuerfelraum.Logic.Ui;
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

            //SimpleIoc.Default.GetInstance<SettingsViewModel>().Tracker.Configure(this)
            //        .IdentifyAs("SettingsViewModel")
            //        .AddProperties<SettingsPage>(w => w.ServerAddress, w => w.ServerPort)
            //        .RegisterPersistTrigger("TextChanged")
            //        .Apply();
        }
    }
}