using GalaSoft.MvvmLight.Ioc;
using MahApps.Metro.Controls.Dialogs;

namespace PhexensWuerfelraum.Logic.Ui
{
    public class MainViewModel : BaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            //if (BaseViewModel.IsInDesignModeStatic)
            //{
            //    // Create design time view services and models
            //}
            //else
            //{
            //    // Create run time view services and models
            //}

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<NavigationViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<ChatnRollViewModel>();
            SimpleIoc.Default.Register<CharacterViewModel>();
            SimpleIoc.Default.Register<DiceRoll>();
            SimpleIoc.Default.Register<AboutViewModel>();
            SimpleIoc.Default.Register<IDialogCoordinator, DialogCoordinator>();
        }

        public AboutViewModel About => SimpleIoc.Default.GetInstance<AboutViewModel>();
        public CharacterViewModel Character => SimpleIoc.Default.GetInstance<CharacterViewModel>();
        public ChatnRollViewModel ChatnRoll => SimpleIoc.Default.GetInstance<ChatnRollViewModel>();
        public DialogCoordinator DialogCoordinator => SimpleIoc.Default.GetInstance<DialogCoordinator>();
        public DiceRoll Dice => SimpleIoc.Default.GetInstance<DiceRoll>();
        public MainViewModel Main => SimpleIoc.Default.GetInstance<MainViewModel>();
        public NavigationViewModel Navigation => SimpleIoc.Default.GetInstance<NavigationViewModel>();
        public SettingsViewModel Settings => SimpleIoc.Default.GetInstance<SettingsViewModel>();
    }
}