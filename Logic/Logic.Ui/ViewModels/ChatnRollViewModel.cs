﻿using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.DependencyInjection;
using PhexensWuerfelraum.Logic.ClientServer;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;

namespace PhexensWuerfelraum.Logic.Ui
{
    public class ChatnRollViewModel : BaseViewModel
    {
        #region properties

        public CharacterModel Character { get; set; } = Ioc.Default.GetService<CharacterViewModel>().Character;
        private DiceRoll DiceRoll { get; set; } = Ioc.Default.GetService<DiceRoll>();

        public string Address { get; set; }
        public Chatroom ChatRoom { get; set; } = Ioc.Default.GetService<Chatroom>();
        public string ColorCode { get; set; }
        public string Message { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public UserType UserType { get; set; }
        public bool BlockConnectionCommands { get; set; }
        public int DiceAmount { get; set; } = 1;
        public bool DetailsFlyOutIsOopen { get; set; }

        private readonly SettingsViewModel settingsViewModel = Ioc.Default.GetService<SettingsViewModel>();

        #endregion properties

        #region Commands

        public RelayCommand ConnectCommand { get; set; }
        public RelayCommand DisconnectCommand { get; set; }
        public RelayCommand OpenTrialModifierCommand { get; private set; }
        public RelayCommand SendTextCommand { get; set; }
        public RelayCommand SendRollCommand { get; set; }
        public RelayCommand SendActionCommand { get; set; }
        public RelayCommand<string> RollDiceCommand { get; set; }
        public RelayCommand ToggleRollModeCommand { get; set; }
        public RelayCommand OpenUpdateInfoCommand { get; private set; }
        public RelayCommand ToggleDetailsFlyOutCommand { get; set; }
        public RelayCommand RequestCharacterDataCommand { get; set; }

        #endregion Commands

        #region constructors

        public ChatnRollViewModel()
        {
            ConnectCommand = new RelayCommand(async () => await Connect(), () => !ChatRoom.Connected && !BlockConnectionCommands);
            DisconnectCommand = new RelayCommand(() => Disconnect(), () => ChatRoom.Connected && !BlockConnectionCommands);
            SendTextCommand = new RelayCommand(async () => await SendText());
            SendRollCommand = new RelayCommand(async () => await SendRoll());
            SendActionCommand = new RelayCommand(async () => await SendAction());
            RollDiceCommand = new RelayCommand<string>(async (parm) => await SendDice(parm));
            ToggleRollModeCommand = new RelayCommand(() => ToggleRollMode());
            OpenUpdateInfoCommand = new RelayCommand(() => OpenUpdateInfo());
            ToggleDetailsFlyOutCommand = new RelayCommand(() => ToggleDetailsFlyOut());
            RequestCharacterDataCommand = new RelayCommand(async () => await Chatroom.RequestCharacterData());
        }

        private void ToggleDetailsFlyOut()
        {
            DetailsFlyOutIsOopen = !DetailsFlyOutIsOopen;
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

            var character = Ioc.Default.GetService<CharacterViewModel>().Character;
            var settings = settingsViewModel.Setting;

            Username = Ioc.Default.GetService<CharacterViewModel>().SelectedCharacter.Name;

            if (string.IsNullOrWhiteSpace(Username))
                Username = character.Name != "" ? character.Name : "User" + new Random().Next(1000, 9999);

            UserType = (Ioc.Default.GetService<SettingsViewModel>().Setting.GameMasterMode) ? UserType.GameMaster : UserType.Player;
            Address = Ioc.Default.GetService<SettingsViewModel>().Setting.ServerAddress;
            Port = Ioc.Default.GetService<SettingsViewModel>().Setting.ServerPort;
            ColorCode = (Ioc.Default.GetService<SettingsViewModel>().Setting.GameMasterMode) ? "Red" : "Black";

            var validIp = IPAddress.TryParse(Address, out IPAddress ipAddress);

            if (!validIp)
            {
                try
                {
                    ipAddress = Dns.GetHostAddresses(Address)[0];
                }
                catch
                {
                    DisplayError("Server Adresse leer oder ungültig, bitte in den Einstellungen überprüfen.");
                    return;
                }
            }

            UserModel user = new(Username, UserType, settings.UserIdentifier);

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

            Chatroom.Disconnect();

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

            ConnectCommand.NotifyCanExecuteChanged();
            DisconnectCommand.NotifyCanExecuteChanged();
            SendTextCommand.NotifyCanExecuteChanged();
            RollDiceCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Send a message to the screen via dialog
        /// </summary>
        /// <param name="message">message to be displayed</param>
        private void DisplayError(string message)
        {
            Messenger.Send(new OpenInfoMessage("Chat", message));
        }

        /// <summary>
        /// inform user of available update
        /// </summary>
        private void OpenUpdateInfo()
        {
            //Messenger.Send(new OpenInfoMessage("Aktualisierung vorhanden", "Eine neue Version wurde gefunden. Die neue Version wird jetzt im Hintergrund heruntergeladen und installiert. Anschließend startet das Programm von alleine neu."));
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
                await Chatroom.Send(Username, Message, ColorCode, ChatRoom.Recipient, ChatRoom.SelectedUser?.UserName, ChatMessageType.Text);
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
                await Chatroom.Send(Username, Message, ColorCode, ChatRoom.Recipient, ChatRoom.SelectedUser?.UserName, ChatMessageType.Roll);
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
                await Chatroom.Send(Username, Message, ColorCode, ChatRoom.Recipient, ChatRoom.SelectedUser?.UserName, ChatMessageType.Action);
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
                await Chatroom.Send(Username, Message, ColorCode, ChatRoom.Recipient, ChatRoom.SelectedUser?.UserName, ChatMessageType.Roll);
            }
            Message = string.Empty;
        }

        private void ToggleRollMode()
        {
            Character.RollModeOpen = !Character.RollModeOpen;

            if (ChatRoom.Connected && ChatRoom.Users.Count > 0 && ChatRoom.Users.Any(u => u.IsGameMaster == true))
            {
                if (Character.RollModeOpen == false && settingsViewModel.Setting.GameMasterMode == false)
                    ChatRoom.SelectedUser = ChatRoom.Users.Where(u => u.IsGameMaster == true).First();
            }
        }

        #endregion methods
    }
}