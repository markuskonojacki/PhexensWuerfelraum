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
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<NavigationViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<ChatnRollViewModel>();
            SimpleIoc.Default.Register<CharacterViewModel>();
            SimpleIoc.Default.Register<DiceRoll>();
            SimpleIoc.Default.Register<AboutViewModel>();
            SimpleIoc.Default.Register<Chatroom>();
            SimpleIoc.Default.Register<IDialogCoordinator, DialogCoordinator>();
        }

        public static AboutViewModel About => SimpleIoc.Default.GetInstance<AboutViewModel>();
        public static CharacterViewModel Character => SimpleIoc.Default.GetInstance<CharacterViewModel>();
        public static ChatnRollViewModel ChatnRoll => SimpleIoc.Default.GetInstance<ChatnRollViewModel>();
        public static DialogCoordinator DialogCoordinator => SimpleIoc.Default.GetInstance<DialogCoordinator>();
        public static DiceRoll Dice => SimpleIoc.Default.GetInstance<DiceRoll>();
        public static MainViewModel Main => SimpleIoc.Default.GetInstance<MainViewModel>();
        public static NavigationViewModel Navigation => SimpleIoc.Default.GetInstance<NavigationViewModel>();
        public static SettingsViewModel Settings => SimpleIoc.Default.GetInstance<SettingsViewModel>();
        public static Chatroom Chatroom => SimpleIoc.Default.GetInstance<Chatroom>();
    }
}