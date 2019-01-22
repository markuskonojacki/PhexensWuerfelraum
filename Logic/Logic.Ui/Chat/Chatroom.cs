using GalaSoft.MvvmLight.Ioc;
using PhexensWuerfelraum.Logic.ClientServer;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PhexensWuerfelraum.Logic.Ui
{
    public class Chatroom : BaseViewModel
    {
        private SimpleClient _client;
        private Task _connectionTask;
        private Task<bool> _listenTask;
        private bool _pinged = false;
        private DateTime _pingLastSent;
        private DateTime _pingSent;
        private Task _updateTask;
        private UserModel _ownUser;
        private UserModel _selectedUser;
        private readonly SettingsViewModel SettingsViewModel = SimpleIoc.Default.GetInstance<SettingsViewModel>();
        private MediaPlayer mediaPlayer1;
        private MediaPlayer mediaPlayer2;
        private MediaPlayer mediaPlayer3;
        private MediaPlayer mediaPlayer4;
        private MediaPlayer mediaPlayer5;

        public Chatroom()
        {
            Messages = new ObservableCollection<ChatPacket>();
            Users = new ObservableCollection<UserModel>();
            Status = "Verbinden";
        }

        public bool IsRunning { get; set; }
        public bool Connected { get; set; }
        public ObservableCollection<ChatPacket> Messages { get; set; }
        public string Status { get; set; }
        public ObservableCollection<UserModel> Users { get; set; }
        public Guid Recipient { get; set; } = Guid.Empty;

        public UserModel SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;

                if (_selectedUser == null)
                {
                    Recipient = Guid.Empty;
                }
                else
                {
                    Recipient = new Guid(_selectedUser.UserGuid);
                }
            }
        }

        public void Clear()
        {
            Log.Instance.Trace("ChatRoom.Clear");
            Messages.Clear();
            Users.Clear();
        }

        public async Task Connect(UserModel user, string address, int port)
        {
            Log.Instance.Debug("Connect");
            Status = "Verbinde...";

            if (SetupClient(user.UserName, address, port))
            {
                try
                {
                    var packet = await GetNewConnectionPacket(user);
                    await InitializeConnection(packet);
                    _ownUser = user;
                }
                catch (Exception ex)
                {
                    Log.Instance.Error($"{ex.Message} ({ex.InnerException})");
                }
            }
        }

        public async Task Disconnect()
        {
            Log.Instance.Debug("Disconnect");
            Status = "Trenne...";

            if (IsRunning)
            {
                IsRunning = false;
                await _connectionTask;
                await _updateTask;

                _client.Disconnect();

                Connected = false;
            }

            Status = "Verbinden";

            Messages.Add(new ChatPacket
            {
                Username = string.Empty,
                Message = "Du hast die Verbindung zum Server getrennt.",
                UserColor = "black"
            });

            Users.Clear();
        }

        public async Task Send(string username, string message, string colorCode, Guid recipient, MessageType messageType = MessageType.Text)
        {
            Log.Instance.Trace("ChatRoom.Send");

            if (recipient != Guid.Empty)
            {
                messageType = MessageType.Whisper;
            }

            ChatPacket cap = new ChatPacket
            {
                Username = username,
                Message = message,
                UserColor = colorCode,
                RecipientGuid = recipient,
                SenderGuid = _client.ClientGuid,
                MessageType = messageType
            };

            await _client.SendObject(cap);
        }

        private async Task<PersonalPacket> GetNewConnectionPacket(UserModel user)
        {
            Log.Instance.Trace("ChatRoom.GetNewConnectionPacket");
            _listenTask = Task.Run(() => _client.Connect());

            IsRunning = await _listenTask;
            Connected = IsRunning;

            var notifyServer = new UserConnectionPacket
            {
                Username = user.UserName,
                UserType = user.UserType,
                IsJoining = true,
                UserGuid = _client.ClientGuid.ToString()
            };

            var personalPacket = new PersonalPacket
            {
                GuidId = _client.ClientGuid.ToString(),
                Package = notifyServer
            };

            return personalPacket;
        }

        private async Task InitializeConnection(PersonalPacket connectionPacket)
        {
            Log.Instance.Trace("ChatRoom.InitializeConnection");
            _pinged = false;

            if (IsRunning)
            {
                _updateTask = Task.Run(() => Update());
                await _client.SendObject(connectionPacket);
                _connectionTask = Task.Run(() => MonitorConnection());
                Status = "Verbunden";
                Connected = true;
            }
            else
            {
                Status = "Fehler";
                await Disconnect();
                Connected = false;
            }
        }

        private bool ManagePacket(object packet)
        {
            if (packet != null)
            {
                if (packet is ChatPacket chatP)
                {
                    Messages.Add(chatP);

                    if (SettingsViewModel.Setting.SoundEffectsEnabled)
                    {
                        if (chatP.Username != _ownUser.UserName)
                        {
                            mediaPlayer1 = new MediaPlayer();
                            mediaPlayer1.Open(new Uri(Directory.GetParent(Assembly.GetExecutingAssembly().Location) + "/Resources/Sounds/Notification.wav"));
                            mediaPlayer1.Play();
                        }

                        if (chatP.MessageType == MessageType.Roll)
                        {
                            if (chatP.Message.Contains("Doppel 1!"))
                            {
                                mediaPlayer2 = new MediaPlayer();
                                mediaPlayer2.Open(new Uri(Directory.GetParent(Assembly.GetExecutingAssembly().Location) + "/Resources/Sounds/roll-1-1.wav"));
                                mediaPlayer2.Play();
                            }
                            else if (chatP.Message.Contains("dreifach 1!"))
                            {
                                mediaPlayer3 = new MediaPlayer();
                                mediaPlayer3.Open(new Uri(Directory.GetParent(Assembly.GetExecutingAssembly().Location) + "/Resources/Sounds/roll-1-1-1.wav"));
                                mediaPlayer3.Play();
                            }
                            else if (chatP.Message.Contains("doppel 20;"))
                            {
                                mediaPlayer4 = new MediaPlayer();
                                mediaPlayer4.Open(new Uri(Directory.GetParent(Assembly.GetExecutingAssembly().Location) + "/Resources/Sounds/roll-20-20.wav"));
                                mediaPlayer4.Play();
                            }
                            else if (chatP.Message.Contains("dreifach 20;"))
                            {
                                mediaPlayer5 = new MediaPlayer();
                                mediaPlayer5.Open(new Uri(Directory.GetParent(Assembly.GetExecutingAssembly().Location) + "/Resources/Sounds/roll-20-20-20.wav"));
                                mediaPlayer5.Play();
                            }
                        }
                    }
                }

                if (packet is UserConnectionPacket connectionP)
                {
                    Users.Clear();
                    foreach (var user in connectionP.Users)
                    {
                        Users.Add(user);
                    }
                }

                if (packet is PingPacket pingP)
                {
                    _pingLastSent = DateTime.Now;
                    _pingSent = _pingLastSent;
                    _pinged = false;
                }

                return true;
            }

            return false;
        }

        private async Task MonitorConnection()
        {
            _pingSent = DateTime.Now;
            _pingLastSent = DateTime.Now;

            while (IsRunning)
            {
                Thread.Sleep(1);
                var timePassed = (_pingSent.TimeOfDay - _pingLastSent.TimeOfDay);
                if (timePassed > TimeSpan.FromSeconds(5))
                {
                    if (!_pinged)
                    {
                        var result = await _client.PingConnection();
                        _pinged = true;

                        Thread.Sleep(5000);

                        if (_pinged)
                            await Task.Run(() => Disconnect());
                    }
                }
                else
                {
                    _pingSent = DateTime.Now;
                }
            }
        }

        private async Task<bool> MonitorData()
        {
            var newObject = await _client.RecieveObject();

            Application.Current.Dispatcher.Invoke(delegate
            {
                return ManagePacket(newObject);
            });

            return false;
        }

        private bool SetupClient(string username, string address, int port)
        {
            Log.Instance.Trace("ChatRoom.SetupClient");
            _client = new SimpleClient(address, port)
            {
                ClientUserName = username,
                IsGameMaster = SettingsViewModel.Setting.GameMasterMode,
            };

            return true;
        }

        private async Task Update()
        {
            Log.Instance.Trace("ChatRoom.Update");
            while (IsRunning)
            {
                Thread.Sleep(1);
                var recieved = await MonitorData();

                if (recieved)
                    Trace.WriteLine(recieved);
            }
        }
    }
}