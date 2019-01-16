using CommonServiceLocator;
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
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

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

        public AboutViewModel About => ServiceLocator.Current.GetInstance<AboutViewModel>();
        public CharacterViewModel Character => ServiceLocator.Current.GetInstance<CharacterViewModel>();
        public ChatnRollViewModel ChatnRoll => ServiceLocator.Current.GetInstance<ChatnRollViewModel>();
        public DialogCoordinator DialogCoordinator => ServiceLocator.Current.GetInstance<DialogCoordinator>();
        public DiceRoll Dice => ServiceLocator.Current.GetInstance<DiceRoll>();
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public NavigationViewModel Navigation => ServiceLocator.Current.GetInstance<NavigationViewModel>();
        public SettingsViewModel Settings => ServiceLocator.Current.GetInstance<SettingsViewModel>();
    }
}