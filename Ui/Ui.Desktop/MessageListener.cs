using CommunityToolkit.Mvvm.DependencyInjection;
using PhexensWuerfelraum.Logic.Ui;
using CommunityToolkit.Mvvm.Messaging;

namespace PhexensWuerfelraum.Ui.Desktop
{
    public class MessageListener
    {
        #region properties

        public bool IsEnabled => true;
        private CharacterViewModel CharacterViewModel { get; set; }
        private ChatnRollViewModel ChatnRollViewModel { get; set; }

        #endregion properties

        #region constructors and destructors

        public MessageListener()
        {
            InitMessenger();
        }

        #endregion constructors and destructors

        #region methods

        private void InitMessenger()
        {
            CharacterViewModel = Ioc.Default.GetService<CharacterViewModel>();
            ChatnRollViewModel = Ioc.Default.GetService<ChatnRollViewModel>();
        }

        #endregion methods
    }
}