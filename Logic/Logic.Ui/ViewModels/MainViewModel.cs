using CommunityToolkit.Mvvm.DependencyInjection;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Extensions.DependencyInjection;

namespace PhexensWuerfelraum.Logic.Ui
{
    public class MainViewModel : BaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                .AddSingleton<NavigationViewModel>()
                .AddSingleton<SettingsViewModel>()
                .AddSingleton<ChatnRollViewModel>()
                .AddSingleton<CharacterViewModel>()
                .AddSingleton<DiceRoll>()
                .AddSingleton<AboutViewModel>()
                .AddSingleton<Chatroom>()
                .AddSingleton<DialogCoordinator>()
                .BuildServiceProvider());
        }

        public static AboutViewModel About => Ioc.Default.GetService<AboutViewModel>();
        public static CharacterViewModel Character => Ioc.Default.GetService<CharacterViewModel>();
        public static ChatnRollViewModel ChatnRoll => Ioc.Default.GetService<ChatnRollViewModel>();
        public static DialogCoordinator DialogCoordinator => Ioc.Default.GetService<DialogCoordinator>();
        public static DiceRoll Dice => Ioc.Default.GetService<DiceRoll>();
        public static MainViewModel Main => Ioc.Default.GetService<MainViewModel>();
        public static NavigationViewModel Navigation => Ioc.Default.GetService<NavigationViewModel>();
        public static SettingsViewModel Settings => Ioc.Default.GetService<SettingsViewModel>();
        public static Chatroom Chatroom => Ioc.Default.GetService<Chatroom>();
    }
}