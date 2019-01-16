using PhexensWuerfelraum.Logic.Ui;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Ioc;

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
            var talent = button.DataContext as CharacterModel.Talent;

            if (ChatnRollViewModel.SendRollCommand.CanExecute(null) == true)
            {
                ChatnRollViewModel.Message = new DiceRoll().RollTrial(talent);
                ChatnRollViewModel.SendRollCommand.Execute(null);
            }
        }
    }
}