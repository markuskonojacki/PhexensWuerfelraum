using PhexensWuerfelraum.Logic.ClientServer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhexensWuerfelraum.Server.Console
{
    public class ConsoleViewModel : BaseViewModel
    {
        private string _externalAddress;

        public string ExternalAddress
        {
            get { return _externalAddress; }
            set { OnPropertyChanged(ref _externalAddress, value); }
        }

        private string _port;

        public string Port
        {
            get { return _port; }
            set { OnPropertyChanged(ref _port, value); }
        }

        private string _status = "Idle";

        public string Status
        {
            get { return _status; }
            set { OnPropertyChanged(ref _status, value); }
        }

        private int _clientsConnected;

        public int ClientsConnected
        {
            get { return _clientsConnected; }
            set { OnPropertyChanged(ref _clientsConnected, value); }
        }

        public ObservableCollection<string> Outputs { get; set; }
        public List<UserModel> UserList = new List<UserModel>();

        public ICommand RunCommand { get; set; }
        public ICommand StopCommand { get; set; }

        private SimpleServer _server;
        private bool _isRunning;

        private Task _updateTask;
        private Task _listenTask;

        public ConsoleViewModel()
        {
            Outputs = new ObservableCollection<string>();

            RunCommand = new AsyncCommand(Run);
            StopCommand = new AsyncCommand(Stop);
        }

        private async Task Run()
        {
            Status = "Connecting...";
            await SetupServer();
            _server.Open();
            _listenTask = Task.Run(() => _server.Start());
            _updateTask = Task.Run(() => Update());
            _isRunning = true;

            WriteOutput(string.Format("Listening on {0}:{1}", ExternalAddress, Port.ToString()));
        }

        private async Task SetupServer()
        {
            Status = "Validating socket...";
            var isValidPort = int.TryParse(Port, out int socketPort);

            if (!isValidPort)
            {
                DisplayError("Port value is not valid.");
                return;
            }

            Status = "Obtaining IP...";
            await Task.Run(() => GetExternalIp());
            Status = "Setting up server...";
            _server = new SimpleServer(IPAddress.Any, socketPort);

            Status = "Setting up events...";
            _server.OnConnectionAccepted += Server_OnConnectionAccepted;
            _server.OnConnectionRemoved += Server_OnConnectionRemoved;
            _server.OnPacketSent += Server_OnPacketSent;
            _server.OnPersonalPacketSent += Server_OnPersonalPacketSent;
            _server.OnPersonalPacketReceived += Server_OnPersonalPacketReceived;
            _server.OnPacketReceived += Server_OnPacketReceived;
        }

        private void Update()
        {
            while (_isRunning)
            {
                Thread.Sleep(5);
                if (!_server.IsRunning)
                {
                    Task.Run(() => Stop());
                    return;
                }

                ClientsConnected = _server.Connections.Count;
                Status = "Running";
            }
        }

        private async Task Stop()
        {
            ExternalAddress = string.Empty;
            _isRunning = false;
            ClientsConnected = 0;
            _server.Close();

            await _listenTask;
            await _updateTask;
            Status = "Stopped";
        }

        private void GetExternalIp()
        {
            try
            {
                // https://stackoverflow.com/a/26486683
                Task<string>[] tasks = new[]
                {
                    Task<string>.Factory.StartNew( () => new WebClient().DownloadString(@"http://icanhazip.com").Trim() ),
                    Task<string>.Factory.StartNew( () => new WebClient().DownloadString(@"https://bot.whatismyipaddress.com").Trim() ),
                    Task<string>.Factory.StartNew( () => new WebClient().DownloadString(@"http://ipinfo.io/ip").Trim() ),
                    Task<string>.Factory.StartNew( () => new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}").Matches(new WebClient().DownloadString(@"http://checkip.dyndns.org").Trim())[0].ToString() )
                };

                int index = Task.WaitAny(tasks);
                ExternalAddress = tasks[index].Result;
            }
            catch { ExternalAddress = "Error receiving IP address."; }
        }

        private void DisplayError(string message)
        {
            System.Console.WriteLine("Error: " + message);
        }

        private void Server_OnPacketSent(object sender, PacketEvents e)
        {
        }

        private void Server_OnPacketReceived(object sender, PacketEvents e)
        {
        }

        private void Server_OnPersonalPacketSent(object sender, PersonalPacketEvents e)
        {
            WriteOutput("Personal Packet Sent");
        }

        private void Server_OnPersonalPacketReceived(object sender, PersonalPacketEvents e)
        {
            if (e.Packet.Package is UserConnectionPacket ucp)
            {
                var userType = "";
                switch (ucp.UserType)
                {
                    case UserType.Player:
                        userType = "Spieler";
                        break;
                    case UserType.GameMaster:
                        userType = "Meister";
                        break;
                }

                var notification = new ChatPacket
                {
                    Username = "Server",
                    Message = $"{ucp.Username} ist dem Chat als {userType} beigetreten"
                };

                if (UserList.Exists(u => u.UserGuid == ucp.UserGuid))
                {
                    UserList.Remove(UserList.Find(u => u.UserGuid == ucp.UserGuid));
                }
                else
                {
                    UserList.Add(new UserModel() {
                        UserGuid = ucp.UserGuid,
                        UserName = ucp.Username,
                        UserType = ucp.UserType                        
                    });
                }
                
                ucp.Users = UserList;

                Task.Run(() => _server.SendObjectToClients(ucp)).Wait();
                Thread.Sleep(500);
                Task.Run(() => _server.SendObjectToClients(notification)).Wait();

                WriteOutput($"Personal Packet Received: User '{ucp.Username}'; Guid '{ucp.UserGuid}'; Type '{ucp.UserType}'");
            }
        }

        private void Server_OnConnectionAccepted(object sender, PacketEvents e)
        {
            WriteOutput("Client Connected: " + e.Sender.Socket.RemoteEndPoint.ToString());
        }

        private void Server_OnConnectionRemoved(object sender, PacketEvents e)
        {
            if (!UserList.Exists(u => u.UserGuid == e.Sender.ClientId.ToString()))
            {
                return;
            }

            var notification = new ChatPacket
            {
                Username = "Server",
                Message = "Ein Mitspieler hat den Chat verlassen"
            };

            var userPacket = new UserConnectionPacket
            {
                UserGuid = e.Sender.ClientId.ToString(),
                Username = UserList.Find(u => u.UserGuid == e.Sender.ClientId.ToString()).UserName,
                UserType = UserList.Find(u => u.UserGuid == e.Sender.ClientId.ToString()).UserType,
                IsJoining = false
            };

            if (UserList.Exists(u => u.UserGuid == userPacket.UserGuid))
                UserList.Remove(UserList.Find(u => u.UserGuid == userPacket.UserGuid));

            userPacket.Users = UserList;

            if (_server.Connections.Count != 0)
            {
                Task.Run(() => _server.SendObjectToClients(userPacket)).Wait();
                Task.Run(() => _server.SendObjectToClients(notification)).Wait();
            }
            WriteOutput("Client Disconnected: " + e.Sender.Socket.RemoteEndPoint.ToString());
        }

        private void WriteOutput(string message)
        {
            System.Console.WriteLine(message);
        }
    }
}