using PhexensWuerfelraum.Logic.Ui;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PhexensWuerfelraum.Ui.Desktop.UserControls
{
    /// <summary>
    /// Interaction logic for TrialRollControl.xaml
    /// </summary>
    public partial class TrialRollControl : UserControl
    {
        public TrialRollControl()
        {
            InitializeComponent();
        }

        private void RollButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Grid button = sender as Grid;
            var talent = button.DataContext as CharacterModel.Talent;
            MessageBox.Show(talent.Value.ToString());
        }
    }
}