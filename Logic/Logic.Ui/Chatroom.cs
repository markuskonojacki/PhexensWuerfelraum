using CommunityToolkit.Mvvm.DependencyInjection;
using Newtonsoft.Json;
using PhexensWuerfelraum.Logic.ClientServer;
using SimpleSockets;
using SimpleSockets.Client;
using SimpleSockets.Messaging;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.Versioning;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static PhexensWuerfelraum.Logic.Ui.CharacterModel;

namespace PhexensWuerfelraum.Logic.Ui
{
    public class Chatroom : BaseViewModel
    {
        #region properties

        private static readonly Chatroom ChatRoom = Ioc.Default.GetService<Chatroom>();
        private static readonly SettingsModel Settings = Ioc.Default.GetService<SettingsViewModel>().Setting;

        private static bool UseSSL;
        private static readonly string Password = "Password";
        private static SimpleSocketClient _client;
        private static AuthPacket ClientAuthInfo;
        private static int OwnId { get => ClientAuthInfo.UserModel.Id; }

        private UserModel _selectedUser;
        private readonly SettingsViewModel SettingsViewModel = Ioc.Default.GetService<SettingsViewModel>();

        private SoundPlayer soundPlayer1;
        private SoundPlayer soundPlayer2;
        private SoundPlayer soundPlayer3;
        private SoundPlayer soundPlayer4;
        private SoundPlayer soundPlayer5;

        public bool IsRunning { get; set; }
        public bool Connected { get; set; }
        public ObservableCollection<ChatPacket> Messages { get; set; } = new ObservableCollection<ChatPacket>();
        public string Status { get; set; } = "Verbinden";
        public ObservableCollection<UserModel> Users { get; set; } = new ObservableCollection<UserModel>();
        public int Recipient { get; set; } = 0;
        public static DateTime FirstBeat { get; set; }

        public UserModel SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;

                if (_selectedUser == null)
                {
                    Recipient = 0;
                }
                else
                {
                    Recipient = _selectedUser.Id;
                }
            }
        }

        #endregion properties

        public Chatroom()
        {
        }

        public void Clear()
        {
            Messages.Clear();
            Users.Clear();
        }

        public void Connect(string address, int port)
        {
            Status = "Verbinde...";
            UseSSL = SettingsViewModel.Setting.EnableSSL;

            if (UseSSL)
            {
                var cert = new X509Certificate2(new System.Text.ASCIIEncoding().GetBytes(SettingsViewModel.Setting.PublicKey));
                _client = new SimpleSocketTcpSslClient(cert);
            }
            else
            {
                _client = new SimpleSocketTcpClient();
            }

            _client.EnableExtendedAuth = false; // When the client sets 'EnableExtendedAuth' to true it will send a GUID, OSVersion, PC Name and DomainName to the server
            _client.ObjectSerializer = new JsonSerialization();
            _client.AllowReceivingFiles = true;

            BindEvents();

            _client.StartClient(address, port);
        }

        private static void ConnectedToServer(SimpleSocket a)
        {
            var character = Ioc.Default.GetService<CharacterViewModel>().Character;
            string username = Ioc.Default.GetService<CharacterViewModel>().SelectedCharacter.Name;

            if (string.IsNullOrEmpty(username))
            {
                username = string.IsNullOrEmpty(character.Name) ? "User" + new Random().Next(1000, 9999) : character.Name;
            }

            if (string.IsNullOrEmpty(username))
            {
                username = Settings.StaticUserName;
            }

            UserType userType = (Settings.GameMasterMode) ? UserType.GameMaster : UserType.Player;

            var userModel = new UserModel(username, userType, Settings.UserIdentifier);
            var authPacket = new AuthPacket(userModel, Password, "test"); // ToDo: channel
            _client.SendObject(authPacket);

            ChatRoom.Status = "Verbunden";
            ChatRoom.Connected = true;

            ChatRoom.StartHeartbeat();
        }

        public static void Disconnect()
        {
            Disconnected(_client);
        }

        private static void Disconnected(SimpleSocket a)
        {
            FirstBeat = DateTime.MinValue;

            Application.Current.Dispatcher.Invoke(delegate
            {
                ChatRoom.Status = "Verbinden";
                ChatRoom.Connected = false;
                ChatRoom.Users.Clear();
                ChatRoom.Messages.Add(new ChatPacket(ChatMessageType.Text, "Die Verbindung zum Server wurde getrennt oder konnte nicht aufgebaut werden.", 0, "", 0, ""));
            });

            UnbindEvents();

            _client.Close();

            //_client.Dispose();
        }

        public static async Task Send(string username, string message, string colorCode, int toId, string toName, ChatMessageType messageType = ChatMessageType.Text)
        {
            if (toId != 0)
            {
                switch (messageType)
                {
                    case ChatMessageType.Text:
                        messageType = ChatMessageType.Whisper;
                        break;

                    case ChatMessageType.Roll:
                        messageType = ChatMessageType.RollWhisper;
                        break;
                }
            }

            if (!string.IsNullOrEmpty(message) && !string.IsNullOrWhiteSpace(message))
            {
                message = message.Replace('"', '\'');

                ChatPacket chatPacket = new(messageType, message, 0, username, toId, toName, colorCode);
                await _client.SendObjectAsync(chatPacket);
            }
        }

        public static async Task RequestCharacterData()
        {
            if (_client != null)
            {
                CharacterModel character = Ioc.Default.GetService<CharacterViewModel>().Character;

                character.CharacterPlayer1 = new CharacterModel();
                character.CharacterPlayer2 = new CharacterModel();
                character.CharacterPlayer3 = new CharacterModel();
                character.CharacterPlayer4 = new CharacterModel();
                character.CharacterPlayer5 = new CharacterModel();
                character.CharacterPlayer6 = new CharacterModel();
                character.CharacterPlayer7 = new CharacterModel();

                CharacterRequestPacket characterRequestPacket = new();
                await _client.SendObjectAsync(characterRequestPacket);
            }
        }

        #region Events

        private static void BindEvents()
        {
            _client.SslAuthStatus += ClientOnSslAuthStatus;
            _client.DisconnectedFromServer += Disconnected;
            _client.ConnectedToServer += ConnectedToServer;
            _client.ClientErrorThrown += ErrorThrown;
            _client.ObjectReceived += ClientOnObjectReceived;
        }

        private static void UnbindEvents()
        {
            _client.SslAuthStatus -= ClientOnSslAuthStatus;
            _client.DisconnectedFromServer -= Disconnected;
            _client.ConnectedToServer -= ConnectedToServer;
            _client.ClientErrorThrown -= ErrorThrown;
            _client.ObjectReceived -= ClientOnObjectReceived;
        }

        private static void ClientOnSslAuthStatus(AuthStatus status)
        {
            if (status == AuthStatus.Failed)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    ChatRoom.Messages.Add(new ChatPacket(ChatMessageType.Text, "Probleme bei der Authentifizierung. Ist dass SSL Zertifikat korrekt?", 0, "", 0, ""));
                });
            }

            //if (status == AuthStatus.Success)
            //    WriteLine("Authenticated with success.");
        }

        private static void ClientOnObjectReceived(SimpleSocketClient simpleSocketClient, object obj, Type objType)
        {
            if (obj.GetType() == typeof(ChatPacket))
            {
                ChatRoom.ManageChatPacket((ChatPacket)obj);
            }
            else if (obj.GetType() == typeof(ChatroomPacket))
            {
                var c = (ChatroomPacket)obj;

                Application.Current.Dispatcher.Invoke(delegate
                {
                    var selectedUserBeforeReload = ChatRoom.SelectedUser;

                    ChatRoom.Users.Clear();

                    foreach (var user in c.Users)
                    {
                        ChatRoom.Users.Add(user);
                    }

                    if (selectedUserBeforeReload != null)
                    {
                        foreach (var user in ChatRoom.Users)
                        {
                            if (user.Id == selectedUserBeforeReload.Id)
                                ChatRoom.SelectedUser = user;
                        }
                    }
                });
            }
            else if (obj.GetType() == typeof(AuthPacket))
            {
                var a = (AuthPacket)obj;
                ClientAuthInfo = a;
            }
            else if (obj.GetType() == typeof(CharacterRequestPacket))
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    ManageCharacterRequestPacket();
                });
            }
            else if (obj.GetType() == typeof(CharacterDataPacket))
            {
                CharacterViewModel characterViewModel = Ioc.Default.GetService<CharacterViewModel>();
                CharacterDataPacket characterDataPacket = (CharacterDataPacket)obj;
                byte[] data = Convert.FromBase64String(characterDataPacket.Data);
                string decodedString = Encoding.UTF8.GetString(data);
                CharacterModel characterModel = JsonConvert.DeserializeObject<CharacterModel>(decodedString);

                // remove empty attributes
                IEnumerable attributes = characterModel.Attribute.Where(a => a.Value == 0).ToList();
                foreach (Attribut attribute in attributes)
                {
                    characterModel.Attribute.Remove(attribute);
                }

                Application.Current.Dispatcher.Invoke(delegate
                {
                    // check existing
                    if (characterViewModel.Character?.CharacterPlayer1?.Name == characterModel.Name)
                    {
                        characterViewModel.Character.CharacterPlayer1 = characterModel;
                    }
                    else if (characterViewModel.Character?.CharacterPlayer2?.Name == characterModel.Name)
                    {
                        characterViewModel.Character.CharacterPlayer2 = characterModel;
                    }
                    else if (characterViewModel.Character?.CharacterPlayer3?.Name == characterModel.Name)
                    {
                        characterViewModel.Character.CharacterPlayer3 = characterModel;
                    }
                    else if (characterViewModel.Character?.CharacterPlayer4?.Name == characterModel.Name)
                    {
                        characterViewModel.Character.CharacterPlayer4 = characterModel;
                    }
                    else if (characterViewModel.Character?.CharacterPlayer5?.Name == characterModel.Name)
                    {
                        characterViewModel.Character.CharacterPlayer5 = characterModel;
                    }
                    else if (characterViewModel.Character?.CharacterPlayer6?.Name == characterModel.Name)
                    {
                        characterViewModel.Character.CharacterPlayer6 = characterModel;
                    }
                    else if (characterViewModel.Character?.CharacterPlayer7?.Name == characterModel.Name)
                    {
                        characterViewModel.Character.CharacterPlayer7 = characterModel;
                    }
                    // else fill up
                    else if (string.IsNullOrEmpty(characterViewModel.Character?.CharacterPlayer1?.Name))
                    {
                        characterViewModel.Character.CharacterPlayer1 = characterModel;
                    }
                    else if (string.IsNullOrEmpty(characterViewModel.Character?.CharacterPlayer2?.Name))
                    {
                        characterViewModel.Character.CharacterPlayer2 = characterModel;
                    }
                    else if (string.IsNullOrEmpty(characterViewModel.Character?.CharacterPlayer3?.Name))
                    {
                        characterViewModel.Character.CharacterPlayer3 = characterModel;
                    }
                    else if (string.IsNullOrEmpty(characterViewModel.Character?.CharacterPlayer4?.Name))
                    {
                        characterViewModel.Character.CharacterPlayer4 = characterModel;
                    }
                    else if (string.IsNullOrEmpty(characterViewModel.Character?.CharacterPlayer5?.Name))
                    {
                        characterViewModel.Character.CharacterPlayer5 = characterModel;
                    }
                    else if (string.IsNullOrEmpty(characterViewModel.Character?.CharacterPlayer6?.Name))
                    {
                        characterViewModel.Character.CharacterPlayer6 = characterModel;
                    }
                    else if (string.IsNullOrEmpty(characterViewModel.Character?.CharacterPlayer7?.Name))
                    {
                        characterViewModel.Character.CharacterPlayer7 = characterModel;
                    }
                });
            }
        }

        public static async Task ManageCharacterRequestPacket()
        {
            CharacterModel character = Ioc.Default.GetService<CharacterViewModel>().Character;
            string encodedCharacter = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(character)));
            
            CharacterDataPacket characterDataPacket = new(encodedCharacter);

            await _client.SendObjectAsync(characterDataPacket);
        }

        private static void ErrorThrown(SimpleSocket socketClient, Exception error)
        {
            if (error.GetType() == typeof(Exception) ||
                error.GetType() == typeof(IOException) ||
                error.GetType() == typeof(ObjectDisposedException))
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    ChatRoom.Messages.Add(new ChatPacket(ChatMessageType.Text, "Ein Fehler ist aufgetreten. Die Verbindung zum Server wurde unerwartet geschlossen.", 0, "", 0, ""));
                });
            }
            else
            {
                throw error;
            }
        }

        private void StartHeartbeat()
        {
            Task.Run(() =>
            {
                while (Connected)
                {
                    if (FirstBeat == DateTime.MinValue)
                        FirstBeat = DateTime.UtcNow;

                    var heartbeat = new HeartbeatPacket(Settings.UserIdentifier, FirstBeat, DateTime.UtcNow);
                    _client.SendObjectAsync(heartbeat);

                    Thread.Sleep(3000);
                }
            });
        }

        #endregion Events

        private void ManageChatPacket(ChatPacket chatPacket)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                if ((chatPacket.MessageType == ChatMessageType.Roll || chatPacket.MessageType == ChatMessageType.RollWhisper) && chatPacket.FromId == OwnId && chatPacket.Message.Contains("(blind)"))
                {
                    chatPacket.Message = chatPacket.Message.Split(':')[0];
                }

                ChatRoom.Messages.Add(chatPacket);

                if (SettingsViewModel.Setting.SoundEffectsEnabled && OperatingSystem.IsWindows())
                {
                    PlaySoundOnChatMessage(chatPacket);
                }
            });
        }

        [SupportedOSPlatform("windows")]
        private static void PlaySoundOnChatMessage(ChatPacket chatPacket)
        {
            if (chatPacket.FromId != OwnId) // only play notification sound for messages from other users
            {
                var sri = Application.GetResourceStream(new Uri(@"pack://application:,,,/Resources/Sounds/Notification.wav"));

                if (sri != null)
                {
                    ChatRoom.soundPlayer1 = new(sri.Stream);
                    ChatRoom.soundPlayer1.Load();
                    ChatRoom.soundPlayer1.Play();
                }
            }

            if (chatPacket.MessageType == ChatMessageType.Roll || chatPacket.MessageType == ChatMessageType.RollWhisper)
            {
                if (chatPacket.Message.Contains("Doppel 1"))
                {
                    var sri = Application.GetResourceStream(new Uri(@"pack://application:,,,/Resources/Sounds/roll-1-1.wav"));

                    if (sri != null)
                    {
                        ChatRoom.soundPlayer2 = new(sri.Stream);
                        ChatRoom.soundPlayer2.Load();
                        ChatRoom.soundPlayer2.Play();
                    }
                }
                else if (chatPacket.Message.Contains("Dreifach 1"))
                {
                    var sri = Application.GetResourceStream(new Uri(@"pack://application:,,,/Resources/Sounds/roll-1-1-1.wav"));

                    if (sri != null)
                    {
                        ChatRoom.soundPlayer3 = new(sri.Stream);
                        ChatRoom.soundPlayer3.Load();
                        ChatRoom.soundPlayer3.Play();
                    }
                }
                else if (chatPacket.Message.Contains("Doppel 20"))
                {
                    var sri = Application.GetResourceStream(new Uri(@"pack://application:,,,/Resources/Sounds/roll-20-20.wav"));

                    if (sri != null)
                    {
                        ChatRoom.soundPlayer4 = new(sri.Stream);
                        ChatRoom.soundPlayer4.Load();
                        ChatRoom.soundPlayer4.Play();
                    }
                }
                else if (chatPacket.Message.Contains("Dreifach 20"))
                {
                    var sri = Application.GetResourceStream(new Uri(@"pack://application:,,,/Resources/Sounds/roll-20-20-20.wav"));

                    if (sri != null)
                    {
                        ChatRoom.soundPlayer5 = new(sri.Stream);
                        ChatRoom.soundPlayer5.Load();
                        ChatRoom.soundPlayer5.Play();
                    }
                }
            }
        }
    }
}