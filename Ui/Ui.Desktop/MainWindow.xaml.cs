using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Navigation;
using CefSharp.Wpf;
using MahApps.Metro.Controls;
using PhexensWuerfelraum.Logic.Ui;
using MenuItem = PhexensWuerfelraum.Logic.Ui.MenuItem;
using Jot;
using Jot.Storage;

namespace PhexensWuerfelraum.Ui.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    partial class MainWindow
    {
        public StateTracker Tracker;

        public MainWindow()
        {
            #region setup CefSharp cache settings

            var cachePath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PhexensWuerfelraum"), "Cache");
            Directory.CreateDirectory(cachePath);

            var settings = new CefSettings
            {
                CachePath = cachePath
            };

            CefSharp.Cef.Initialize(settings);

            #endregion setup CefSharp cache settings

            InitializeComponent();

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            #region init navigation

            Navigation.Frame = new Frame() { NavigationUIVisibility = NavigationUIVisibility.Hidden };
            Navigation.Frame.Navigated += SplitViewFrame_OnNavigated;

            // Navigate to the home page.
            this.Loaded += (sender, args) => Navigation.Navigate(new Uri("Views/ChatnRollPage.xaml", UriKind.RelativeOrAbsolute));

            #endregion init navigation

            var charactersFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PhexensWuerfelraum");
            Directory.CreateDirectory(charactersFilePath);

            Tracker = new StateTracker() { StoreFactory = new JsonFileStoreFactory(charactersFilePath) };

            Tracker.Configure(this)
            .IdentifyAs("Main Window")
            .AddProperties<MetroWindow>(w => w.Height, w => w.Width, w => w.Top, w => w.Left/*, w => w.WindowState*/)
            .RegisterPersistTrigger(nameof(SizeChanged))
            .Apply();
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
            this.HamburgerMenuControl.Content = e.Content;
            this.HamburgerMenuControl.SelectedItem = e.ExtraData ?? ((NavigationViewModel)NavigationGrid.DataContext).GetItem(e.Uri);
            this.HamburgerMenuControl.SelectedOptionsItem = e.ExtraData ?? ((NavigationViewModel)NavigationGrid.DataContext).GetOptionsItem(e.Uri);
        }
    }
}