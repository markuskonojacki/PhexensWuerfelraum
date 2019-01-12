using MahApps.Metro.IconPacks;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace PhexensWuerfelraum.Logic.Ui
{
    public class NavigationViewModel : BaseViewModel
    {
        private static readonly ObservableCollection<MenuItem> AppMenu = new ObservableCollection<MenuItem>();
        private static readonly ObservableCollection<MenuItem> AppOptionsMenu = new ObservableCollection<MenuItem>();

        public NavigationViewModel()
        {
            this.Menu.Add(new MenuItem() { Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.CommentTextMultiple }, IsEnabled = true, Text = "Chat'n'Roll", NavigationDestination = new Uri("Views/ChatnRollPage.xaml", UriKind.RelativeOrAbsolute) });
            this.Menu.Add(new MenuItem() { Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.Brush }, IsEnabled = true, Text = "Whiteboard", NavigationDestination = new Uri("Views/WhiteboardPage.xaml", UriKind.RelativeOrAbsolute) });
            this.Menu.Add(new MenuItem() { Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.SwordCross }, IsEnabled = false, Text = "BattleMap", NavigationDestination = new Uri("Views/BattleMapPage.xaml", UriKind.RelativeOrAbsolute) });
            this.Menu.Add(new MenuItem() { Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.AccountEdit }, IsEnabled = true, Text = "Charakter", NavigationDestination = new Uri("Views/CharacterPage.xaml", UriKind.RelativeOrAbsolute) });
            this.Menu.Add(new MenuItem() { Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.HeartBroken }, IsEnabled = true, Text = "Patzertabelle", NavigationDestination = new Uri("Views/PatzerTabellePage.xaml", UriKind.RelativeOrAbsolute) });
            this.Menu.Add(new MenuItem() { Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.Wikipedia }, IsEnabled = true, Text = "Wiki", NavigationDestination = new Uri("Views/WikiPage.xaml", UriKind.RelativeOrAbsolute) });

            this.OptionsMenu.Add(new MenuItem() { Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.Cogs }, IsEnabled = true, Text = "Einstellungen", NavigationDestination = new Uri("Views/SettingsPage.xaml", UriKind.RelativeOrAbsolute) });
            this.OptionsMenu.Add(new MenuItem() { Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.Information }, IsEnabled = true, Text = "Über", NavigationDestination = new Uri("Views/AboutPage.xaml", UriKind.RelativeOrAbsolute) });

            Log.Instance.Trace("Navigation initialized");
        }

        public ObservableCollection<MenuItem> Menu => AppMenu;

        public ObservableCollection<MenuItem> OptionsMenu => AppOptionsMenu;

        public object GetItem(object uri)
        {
            return null == uri ? null : this.Menu.FirstOrDefault(m => m.NavigationDestination.Equals(uri));
        }

        public object GetOptionsItem(object uri)
        {
            return null == uri ? null : this.OptionsMenu.FirstOrDefault(m => m.NavigationDestination.Equals(uri));
        }
    }
}