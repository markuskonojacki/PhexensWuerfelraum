using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Ioc;
using PhexensWuerfelraum.Logic.Ui;

namespace PhexensWuerfelraum.Ui.Desktop.UserControls
{
    /// <summary>
    /// Interaction logic for AttributeRollControl.xaml
    /// </summary>
    public partial class AttributeRollControl : UserControl
    {
        private ChatnRollViewModel ChatnRollViewModel { get; set; } = SimpleIoc.Default.GetInstance<ChatnRollViewModel>();

        public AttributeRollControl()
        {
            InitializeComponent();
        }

        private void RollButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Grid button = sender as Grid;

            if (ChatnRollViewModel.SendRollCommand.CanExecute(null) == true)
            {
                if (button.DataContext is CharacterModel.Attribut)
                {
                    CharacterModel.Attribut attribut = button.DataContext as CharacterModel.Attribut;
                    ChatnRollViewModel.Message = new DiceRoll().RollDice($"attribut{ CharacterModel.MapAttributeTypeToStringShort(attribut.Type) }", 0);
                }

                ChatnRollViewModel.SendRollCommand.Execute(null);
            }
        }
    }
}