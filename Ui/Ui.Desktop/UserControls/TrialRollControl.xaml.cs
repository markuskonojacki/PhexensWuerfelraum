using GalaSoft.MvvmLight.Ioc;
using PhexensWuerfelraum.Logic.Ui;
using System.Windows.Controls;
using System.Windows.Input;

namespace PhexensWuerfelraum.Ui.Desktop.UserControls
{
    /// <summary>
    /// Interaction logic for TrialRollControl.xaml
    /// </summary>
    public partial class TrialRollControl : UserControl
    {
        private ChatnRollViewModel ChatnRollViewModel { get; set; } = SimpleIoc.Default.GetInstance<ChatnRollViewModel>();

        public TrialRollControl()
        {
            InitializeComponent();
        }

        private void RollButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Grid button = sender as Grid;

            if (ChatnRollViewModel.SendRollCommand.CanExecute(null) == true)
            {
                if (button.DataContext is CharacterModel.Talent)
                {
                    ChatnRollViewModel.Message = new DiceRoll().RollTrial((CharacterModel.Talent)button.DataContext);
                }
                else
                {
                    ChatnRollViewModel.Message = new DiceRoll().RollTrial((CharacterModel.Zauber)button.DataContext);
                }

                ChatnRollViewModel.SendRollCommand.Execute(null);
            }
        }
    }
}