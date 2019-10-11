﻿using CefSharp.Wpf;
using Jot;
using Jot.Storage;
using MahApps.Metro.Controls;
using PhexensWuerfelraum.Logic.Ui;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Navigation;
using MenuItem = PhexensWuerfelraum.Logic.Ui.MenuItem;

namespace PhexensWuerfelraum.Ui.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    partial class MainWindow
    {
        public Tracker Tracker;

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
            HamburgerMenuControl.Content = Navigation.Frame;

            // Navigate to the home page.
            Loaded += (sender, args) => Navigation.Navigate(new Uri("Views/ChatnRollPage.xaml", UriKind.RelativeOrAbsolute));

            #endregion init navigation

            var charactersFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PhexensWuerfelraum");
            Directory.CreateDirectory(charactersFilePath);

            Tracker = new Tracker() { Store = new JsonFileStore(charactersFilePath) };

            Tracker.Configure<MetroWindow>()
               .Id(mw => "Main Window")
               .Properties(mw => new { mw.Height, mw.Width, mw.Top, mw.Left })
               .PersistOn(nameof(SizeChanged));
            Tracker.Track(this);
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
    }
}