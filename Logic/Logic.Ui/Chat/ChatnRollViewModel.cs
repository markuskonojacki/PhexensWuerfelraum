using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using PhexensWuerfelraum.Logic.ClientServer;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace PhexensWuerfelraum.Logic.Ui
{
    public class ChatnRollViewModel : BaseViewModel
    {
        #region properties

        public CharacterModel Character { get; set; } = SimpleIoc.Default.GetInstance<CharacterViewModel>().Character;
        private DiceRoll DiceRoll { get; set; } = SimpleIoc.Default.GetInstance<DiceRoll>();

        public string Address { get; set; }
        public Chatroom ChatRoom { get; set; } = SimpleIoc.Default.GetInstance<Chatroom>();
        public string ColorCode { get; set; }
        public string Message { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public UserType UserType { get; set; }
        public bool BlockConnectionCommands { get; set; }
        public int DiceAmount { get; set; } = 1;

        private readonly SettingsViewModel settingsViewModel = SimpleIoc.Default.GetInstance<SettingsViewModel>();

        #endregion properties

        #region Commands

        public RelayCommand ConnectCommand { get; set; }
        public RelayCommand DisconnectCommand { get; set; }
        public RelayCommand OpenTrialModifierCommand { get; private set; }
        public RelayCommand SendTextCommand { get; set; }
        public RelayCommand SendRollCommand { get; set; }
        public RelayCommand SendActionCommand { get; set; }
        public RelayCommand<string> RollDiceCommand { get; set; }

        public RelayCommand OpenUpdateInfoCommand { get; private set; }

        #endregion Commands

        #region constructors

        public ChatnRollViewModel()
        {
            ConnectCommand = new RelayCommand(async () => await Connect(), !ChatRoom.Connected && !BlockConnectionCommands);
            DisconnectCommand = new RelayCommand(() => Disconnect(), ChatRoom.Connected && !BlockConnectionCommands);
            SendTextCommand = new RelayCommand(async () => await SendText(), true);
            SendRollCommand = new RelayCommand(async () => await SendRoll(), true);
            SendActionCommand = new RelayCommand(async () => await SendAction(), true);
            RollDiceCommand = new RelayCommand<string>(async (parm) => await SendDice(parm), true);
            OpenUpdateInfoCommand = new RelayCommand(() => OpenUpdateInfo());
        }

        #endregion constructors

        #region methods

        /// <summary>
        /// establish a connection to the chat server
        /// </summary>
        /// <returns></returns>
        private async Task Connect()
        {
            ToggleBlockConnectionCommands(false);

            var character = SimpleIoc.Default.GetInstance<CharacterViewModel>().Character;
            var settings = settingsViewModel.Setting;

            Username = SimpleIoc.Default.GetInstance<CharacterViewModel>().SelectedCharacter.Name;

            if (string.IsNullOrWhiteSpace(Username))
                Username = character.Name != "" ? character.Name : "User" + new Random().Next(1000, 9999);

            UserType = (SimpleIoc.Default.GetInstance<SettingsViewModel>().Setting.GameMasterMode) ? UserType.GameMaster : UserType.Player;
            Address = SimpleIoc.Default.GetInstance<SettingsViewModel>().Setting.ServerAddress;
            Port = SimpleIoc.Default.GetInstance<SettingsViewModel>().Setting.ServerPort;
            ColorCode = (SimpleIoc.Default.GetInstance<SettingsViewModel>().Setting.GameMasterMode) ? "Red" : "Black";

            var validIp = IPAddress.TryParse(Address, out IPAddress ipAddress);

            if (!validIp)
            {
                try
                {
                    ipAddress = Dns.GetHostAddresses(Address)[0];
                }
                catch (Exception ex)
                {
                    DisplayError("Server Adresse leer oder ungültig, bitte in den Einstellungen überprüfen.");
                    return;
                }
            }

            UserModel user = new UserModel(Username, UserType);

            var validPort = int.TryParse(Port, out int socketPort);

            if (!validPort)
            {
                DisplayError("Gibt bitte einen gültigen Server Port an");
                return;
            }

            if (string.IsNullOrWhiteSpace(Address))
            {
                DisplayError("Gib bitte eine gültige Server Adresse an");
                return;
            }

            if (string.IsNullOrWhiteSpace(Username))
            {
                DisplayError("Gib bitte einen Namen an");
                return;
            }

            ChatRoom.Clear();

            await Task.Run(() => ChatRoom.Connect(Address, socketPort));

            settingsViewModel.AllowEdit = false;
            ToggleBlockConnectionCommands(true);
        }

        /// <summary>
        /// disconnect from the chat server
        /// </summary>
        /// <returns></returns>
        private void Disconnect()
        {
            ToggleBlockConnectionCommands(false);

            if (ChatRoom == null)
                DisplayError("Du bist mit keinem Server verbunden");

            ChatRoom.Disconnect();

            settingsViewModel.AllowEdit = true;
            ToggleBlockConnectionCommands(true);
        }

        /// <summary>
        /// disconnects from and reconnects to the chat server
        /// </summary>
        public async Task ReconnectAsync()
        {
            Disconnect();
            await Connect();
        }

        /// <summary>
        /// toggles between the states of enable/disable of the connect/disconnect command buttons, with a slight delay to prevent connection spamming
        /// </summary>
        /// <param name="delay"></param>
        private async void ToggleBlockConnectionCommands(bool delay)
        {
            if (delay)
            {
                await Task.Factory.StartNew(() => Thread.Sleep(1000));
            }

            BlockConnectionCommands = !BlockConnectionCommands;

            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
            SendTextCommand.RaiseCanExecuteChanged();
            RollDiceCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Send a message to the screen via dialog
        /// </summary>
        /// <param name="message">message to be displayed</param>
        private void DisplayError(string message)
        {
            MessengerInstance.Send(new OpenInfoMessage("Chat", message));
        }

        /// <summary>
        /// inform user of available update
        /// </summary>
        private void OpenUpdateInfo()
        {
            MessengerInstance.Send(new OpenInfoMessage("Aktualisierung vorhanden", "Eine neue Version wurde gefunden. Die neue Version wird jetzt im Hintergrund heruntergeladen und installiert. Anschließend startet das Programm von alleine neu."));
        }

        /// <summary>
        /// send message to chat
        /// </summary>
        private async Task SendText()
        {
            if (ChatRoom == null)
                DisplayError("Du bist mit keinem Server verbunden");

            if (ChatRoom.Connected)
            {
                await ChatRoom.Send(Username, Message, ColorCode, ChatRoom.Recipient, ChatRoom.SelectedUser?.UserName, ChatMessageType.Text);
            }
            Message = string.Empty;
        }

        /// <summary>
        /// send message to chat
        /// </summary>
        private async Task SendRoll()
        {
            if (ChatRoom == null)
                DisplayError("Du bist mit keinem Server verbunden");

            if (ChatRoom.Connected)
            {
                await ChatRoom.Send(Username, Message, ColorCode, ChatRoom.Recipient, ChatRoom.SelectedUser?.UserName, ChatMessageType.Roll);
            }
            Message = string.Empty;
        }

        /// <summary>
        /// send message to chat
        /// </summary>
        private async Task SendAction()
        {
            if (ChatRoom == null)
                DisplayError("Du bist mit keinem Server verbunden");

            if (ChatRoom.Connected)
            {
                await ChatRoom.Send(Username, Message, ColorCode, ChatRoom.Recipient, ChatRoom.SelectedUser?.UserName, ChatMessageType.Action);
            }
            Message = string.Empty;
        }

        /// <summary>
        /// send one of the predefined dice rolls to chat
        /// </summary>
        private async Task SendDice(string commandParameter)
        {
            if (ChatRoom.Connected)
            {
                Message = DiceRoll.RollDice(commandParameter, DiceAmount);
                await ChatRoom.Send(Username, Message, ColorCode, ChatRoom.Recipient, ChatRoom.SelectedUser?.UserName, ChatMessageType.Roll);
            }
            Message = string.Empty;
        }

        #endregion methods
    }
}