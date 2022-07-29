using GalaSoft.MvvmLight.Ioc;
using PhexensWuerfelraum.Logic.ClientServer;
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
        private ChatnRollViewModel ChatnRollViewModel { get; set; } = SimpleIoc.Default.GetInstance<ChatnRollViewModel>();

        public int PlayerNumber
        {
            get { return (int)GetValue(PlayerNumberProperty); }
            set { SetValue(PlayerNumberProperty, value); }
        }

        public static readonly DependencyProperty PlayerNumberProperty =
            DependencyProperty.Register("PlayerNumber", typeof(int), typeof(TrialRollControl), new PropertyMetadata(0));

        public TrialRollControl()
        {
            InitializeComponent();
        }

        private void RollButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Grid button = sender as Grid;

            if (ChatnRollViewModel.SendRollCommand.CanExecute(null) == true)
            {
                if (PlayerNumber == 0)
                {
                    if (button.DataContext is CharacterModel.Talent)
                    {
                        ChatnRollViewModel.Message = new DiceRoll().RollTrial((CharacterModel.Talent)button.DataContext, PlayerNumber);
                    }
                    else
                    {
                        ChatnRollViewModel.Message = new DiceRoll().RollTrial((CharacterModel.Zauber)button.DataContext, PlayerNumber);
                    }

                    ChatnRollViewModel.SendRollCommand.Execute(null);
                }
                else
                {
                    if (button.DataContext is CharacterModel.Talent)
                    {
                        ChatPacket chatPacket = new ChatPacket(ChatMessageType.Roll, $"{new DiceRoll().RollTrial((CharacterModel.Talent)button.DataContext, PlayerNumber)}", 0, "Der Meister", 0, "", "Purple");
                        ChatnRollViewModel.ChatRoom.Messages.Add(chatPacket);
                    }
                    else
                    {
                        ChatPacket chatPacket = new ChatPacket(ChatMessageType.Roll, $"{new DiceRoll().RollTrial((CharacterModel.Zauber)button.DataContext, PlayerNumber)}", 0, "Der Meister", 0, "", "Purple");
                        ChatnRollViewModel.ChatRoom.Messages.Add(chatPacket);
                    }
                }
            }
        }
    }
}