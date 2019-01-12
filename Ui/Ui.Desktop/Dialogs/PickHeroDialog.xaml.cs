using System.Windows.Controls;
using GalaSoft.MvvmLight.Ioc;
using PhexensWuerfelraum.Logic.Ui;

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

            //CharacterListView.SelectedItem = null;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (CharacterListView.SelectedItem != null)
            {
                Visibility = System.Windows.Visibility.Hidden;

                // SelectedItem="{Binding SelectedCharacter, Mode=OneWayToSource}"
                var characterViewModel = SimpleIoc.Default.GetInstance<CharacterViewModel>();
                characterViewModel.SelectedCharacter = (CharacterModel)CharacterListView.SelectedItem;

                Close();
            }
        }
    }
}