using GalaSoft.MvvmLight.Ioc;
using PhexensWuerfelraum.Logic.Ui;
using System.Windows.Controls;

namespace PhexensWuerfelraum.Ui.Desktop
{
    /// <summary>
    /// Interaction logic for PickHeroDialog.xaml
    /// </summary>
    public partial class PickHeroDialog
    {
        public PickHeroDialog()
        {
            InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Visibility = System.Windows.Visibility.Hidden;

            SimpleIoc.Default.GetInstance<CharacterViewModel>().SelectedCharacter = (CharacterModel)CharacterListView.SelectedItem;

            Close();
        }
    }
}