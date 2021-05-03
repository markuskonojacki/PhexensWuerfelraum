using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Ioc;
using Jot;
using Jot.Storage;
using MahApps.Metro.Controls;
using Onova;
using Onova.Models;
using Onova.Services;
using PhexensWuerfelraum.Logic.Ui;
using MenuItem = PhexensWuerfelraum.Logic.Ui.MenuItem;

namespace PhexensWuerfelraum.Ui.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    partial class MainWindow
    {
        public Tracker Tracker;
        private ChatnRollViewModel ChatnRollViewModel { get; set; } = SimpleIoc.Default.GetInstance<ChatnRollViewModel>();
        private SettingsViewModel SettingsViewModel { get; set; } = SimpleIoc.Default.GetInstance<SettingsViewModel>();

        public MainWindow()
        {
            InitializeComponent();

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            #region init navigation

            Navigation.Frame = new Frame() { NavigationUIVisibility = NavigationUIVisibility.Hidden };
            Navigation.Frame.Navigated += SplitViewFrame_OnNavigated;
            HamburgerMenuControl.Content = Navigation.Frame;

            // Navigate to the home page.
            Loaded += (sender, args) => Navigation.Navigate(new Uri("Views/ChatnRollPage.xaml", UriKind.RelativeOrAbsolute));

            #endregion init navigation

            Directory.CreateDirectory(SettingsViewModel.DataPath);

            Tracker = new Tracker(new JsonFileStore(SettingsViewModel.DataPath));

            Tracker.Configure<MetroWindow>()
                .Id(mw => "Main Window")
                .Properties(mw => new { mw.Height, mw.Width, mw.Top, mw.Left })
                .PersistOn(nameof(SizeChanged));
            Tracker.Track(this);

#if !DEBUG
            if (SettingsModel.AutoUpdate)
            {
                Task task = Task.Run(() => UpdateAsync());
            }
#endif
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Only unused in DEBUG")]
        private async Task UpdateAsync()
        {
            var manager = new UpdateManager(
                AssemblyMetadata.FromAssembly(
                    Assembly.GetEntryAssembly(),
                    System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName),
                new GithubPackageResolver("Derevar", "PhexensWuerfelraum", "PhexensWuerfelraum-*.zip"),
                new ZipPackageExtractor());
            {
                var resultCheckForUpdatesAsync = await manager.CheckForUpdatesAsync();
                if (resultCheckForUpdatesAsync.CanUpdate)
                {
                    ChatnRollViewModel.OpenUpdateInfoCommand.Execute(null);

                    // Prepare an update by downloading and extracting the package
                    // (supports progress reporting and cancellation)
                    await manager.PrepareUpdateAsync(resultCheckForUpdatesAsync.LastVersion);

                    if (manager.IsUpdatePrepared(resultCheckForUpdatesAsync.LastVersion))
                    {
                        // Launch an executable that will apply the update
                        manager.LaunchUpdater(resultCheckForUpdatesAsync.LastVersion, true);

                        // Terminate the running application so that the updater can overwrite files
                        Environment.Exit(0);
                    }
                }
            }
        }

        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.InvokedItem;
            if (menuItem != null && menuItem.IsNavigation)
            {
                Navigation.Navigate(menuItem.NavigationDestination, menuItem);
            }
        }

        private void SplitViewFrame_OnNavigated(object sender, NavigationEventArgs e)
        {
            HamburgerMenuControl.SelectedItem = e.ExtraData ?? ((NavigationViewModel)NavigationGrid.DataContext).GetItem(e.Uri);
            HamburgerMenuControl.SelectedOptionsItem = e.ExtraData ?? ((NavigationViewModel)NavigationGrid.DataContext).GetOptionsItem(e.Uri);
        }

        private void CharacterDropDownSelector_Click(object sender, RoutedEventArgs e)
        {
            Navigation.Navigate(new Uri("Views/SettingsPage.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}