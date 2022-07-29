using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PhexensWuerfelraum.Logic.ClientServer;
using SimpleSockets;
using SimpleSockets.Messaging.Metadata;
using SimpleSockets.Server;

namespace PhexensWuerfelraum.Server.Console
{
    internal class Program
    {
        private static int Port;
        private static bool UseSSL;
        private static bool Compress;

        private static readonly List<AuthPacket> AuthenticatedUsers = new();
        private static SimpleSocketListener _listener;

        private static readonly List<HeartbeatPacket> heartbeats = new();
        private static Dictionary<int, int> d20statistic = new();

        #region version

        private static Version AssemblyVersion()
        {
            return Assembly.GetEntryAssembly().GetName().Version;
        }

        public static string Version { get => $"v{AssemblyVersion().Major}.{AssemblyVersion().Minor}.{AssemblyVersion().Build}"; }

        #endregion version

        private static void Main(string[] args)
        {
            WriteLine($"Starting the server {Version}");

            string configFileIniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "settings.ini");
            WriteLine($"Reading config file: { configFileIniPath }");

            if (File.Exists(configFileIniPath))
            {
                var config = new ConfigurationBuilder()
                    .AddIniFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "settings.ini"), optional: true, reloadOnChange: true)
                    .AddCommandLine(args)
                    .Build();

                WriteLine("Configuration:");

                Port = int.Parse(config["port"]);
                WriteLine($"Port is {Port}");

                UseSSL = bool.Parse(config["ssl"]);
                WriteLine($"SSL is {(UseSSL ? "ON" : "OFF")}");

                Compress = bool.Parse(config["compress"]);
                WriteLine($"Compression is {(Compress ? "ON" : "OFF")}");

                if (UseSSL)
                {
                    var privKeyFileName = "PrivateKey.pfx";
                    var privateKeyPath = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", privKeyFileName));
                    var publicKeyFileName = "PublicKey.pem";
                    var publicKeyPath = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", publicKeyFileName));

                    if (File.Exists(privateKeyPath))
                    {
                        WriteLine("Using existing private key");
                    }
                    else
                    {
                        X509Certificate2 generatedCert = Certificates.GenerateCertificate("PhexensWuerfelraumServer");

                        string publicKey = Certificates.ExportToPEM(generatedCert);

                        File.WriteAllBytes(privateKeyPath, generatedCert.Export(X509ContentType.Pfx));
                        File.WriteAllText(publicKeyPath, publicKey);

                        System.Console.WriteLine("");
                        WriteLine($"No certificate found. Generated a new private/public key pair. Give the following string to your users. It is also saved as config\\{publicKeyFileName}");
                        System.Console.WriteLine("");
                        System.Console.Write(publicKey);
                        System.Console.WriteLine("");
                        System.Console.WriteLine($"NEVER share the {privKeyFileName}!");
                        System.Console.WriteLine("");
                    }

                    var cert = new X509Certificate2(File.ReadAllBytes(privateKeyPath));

                    _listener = new SimpleSocketTcpSslListener(cert);
                }
                else
                {
                    _listener = new SimpleSocketTcpListener();
                }

                _listener.ObjectSerializer = new JsonSerialization();
                _listener.AllowReceivingFiles = true;

                BindEvents();
                _listener.StartListening(Port);

                for (int i = 1; i <= 20; i++)
                {
                    d20statistic[i] = 0;
                }

                while (true)
                {
                    System.Console.Read();
                }
            }
            else
            {
                WriteLine($"Config file { configFileIniPath } does not exist. Did you rename the settings.example.ini to settings.ini?");
                System.Console.Read();
            }
        }

        private static int ShowClients()
        {
            var ids = new List<int>();

            foreach (var client in _listener.GetConnectedClients())
            {
                ids.Add(client.Value.Id);
                WriteLine("Client ID: " + client.Value.Id + " with IPv4 : " + client.Value.RemoteIPv4);
            }

            WriteLine("Enter the id of the client; (E) to exit... ");
            var chosen = System.Console.ReadLine();

            if (chosen != null && chosen.ToUpper() == "E")
                return 0;

            if (!ids.Contains(int.Parse(chosen)))
            {
                chosen = ShowClients().ToString();
            }

            return int.Parse(chosen);
        }

        #region Events

        private static void BindEvents()
        {
            _listener.SslAuthStatus += Listener_SslAuthStatus;
            _listener.MessageReceived += MessageReceived;
            _listener.MessageSubmitted += MessageSubmitted;
            _listener.MessageWithMetaDataReceived += CustomHeaderReceivedAsync;
            _listener.ClientDisconnected += ClientDisconnected;
            _listener.ClientConnected += ClientConnected;
            _listener.ServerHasStarted += ServerHasStarted;
            _listener.MessageFailed += MessageFailed;
            _listener.ServerErrorThrown += ErrorThrown;
            _listener.ObjectReceived += ListenerOnObjectReceived;
        }

        private static void Listener_SslAuthStatus(IClientInfo client, AuthStatus status)
        {
            if (status == AuthStatus.Failed)
                WriteLine($"Server failed to authenticate certificate of client {client.Id}");
            if (status == AuthStatus.Success)
                WriteLine($"Server authenticated certificate of client {client.Id}");
        }

        private static void ListenerOnObjectReceived(IClientInfo client, object obj, Type objType)
        {
            if (obj.GetType() != typeof(HeartbeatPacket))
                WriteLine($"Received an object of type = '{objType.FullName}'");

            if (obj.GetType() == typeof(AuthPacket))
            {
                var authPacket = (AuthPacket)obj;

                WriteLine($"User '{authPacket.UserModel.UserName}' ({authPacket.UserModel.UserIdentifier}) has sent an authentication packet");

                authPacket.UserModel.Id = client.Id;

                if (AuthenticatedUsers.Exists(a => a.UserModel.UserIdentifier == authPacket.UserModel.UserIdentifier))
                {
                    var authPackDoublicateUser = AuthenticatedUsers.Find(a => a.UserModel.UserIdentifier == authPacket.UserModel.UserIdentifier);
                    AuthenticatedUsers.Remove(authPackDoublicateUser);
                    WriteLine($"Removed duplicate connected user with user identifier {authPacket.UserModel.UserIdentifier}");
                }

                AuthenticatedUsers.Add(authPacket);

                _listener.SendObjectAsync(client.Id, authPacket, Compress, false, false);

                List<UserModel> userModelList = new();
                foreach (var user in AuthenticatedUsers.ToArray())
                {
                    userModelList.Add(user.UserModel);
                }

                SendUserListToUsers();

                ChatPacket userJoinNotification = new(ChatMessageType.Text, $"Nutzer '{authPacket.UserModel.UserName}' ist dem Chat beigetreten", 0, "Server", 0, "", "Silver");

                foreach (var user in AuthenticatedUsers.ToArray())
                {
                    if (_listener.IsConnected(user.UserModel.Id))
                        _listener.SendObjectAsync(user.UserModel.Id, userJoinNotification, Compress, false, false);
                }
            }
            else if (obj.GetType() == typeof(ChatPacket))
            {
                var chatPacket = (ChatPacket)obj;
                chatPacket.FromId = client.Id;

                if (chatPacket.MessageType == ChatMessageType.Roll || chatPacket.MessageType == ChatMessageType.RollWhisper)
                {
                    string pat = @"【(\d{1,2})】, 【(\d{1,2})】, 【(\d{1,2})】";
                    Regex r = new(pat, RegexOptions.IgnoreCase);
                    Match m = r.Match(chatPacket.Message);

                    if (m.Success)
                    {
                        d20statistic[int.Parse(m.Groups[1].Value)]++;
                        d20statistic[int.Parse(m.Groups[2].Value)]++;
                        d20statistic[int.Parse(m.Groups[3].Value)]++;

                        int sumRolls = 0;

                        for (int i = 1; i <= 20; i++)
                        {
                            sumRolls += d20statistic[i];
                        }

                        for (int i = 1; i <= 20; i++)
                        {
                            WriteLine($"{i}: {d20statistic[i]} ({Math.Round((double)d20statistic[i] / (double)sumRolls * 100, 2)} %)");
                        }
                        WriteLine($"sum: {sumRolls} d20 rolls");
                    }
                }

                if (chatPacket.ToId == 0) // to everyone
                {
                    foreach (var user in AuthenticatedUsers.ToArray())
                    {
                        _listener.SendObjectAsync(user.UserModel.Id, chatPacket, Compress, false, false);
                    }
                }
                else // only to the recepient and the game master
                {
                    var recepients = AuthenticatedUsers.FindAll(a => a.UserModel.Id == chatPacket.ToId || a.UserModel.Id == chatPacket.FromId || a.UserModel.IsGameMaster == true);

                    foreach (var user in recepients.ToArray())
                    {
                        _listener.SendObjectAsync(user.UserModel.Id, chatPacket, Compress, false, false);
                    }
                }
            }
            else if (obj.GetType() == typeof(CharacterDataPacket))
            {
                var characterDataPacket = (CharacterDataPacket)obj;
                var recepients = AuthenticatedUsers.FindAll(a => a.UserModel.IsGameMaster == true);

                foreach (var user in recepients.ToArray())
                {
                    _listener.SendObjectAsync(user.UserModel.Id, characterDataPacket, Compress, false, false);
                }
            }
            else if (obj.GetType() == typeof(CharacterRequestPacket))
            {
                var characterRequestPacket = (CharacterRequestPacket)obj;
                var recepients = AuthenticatedUsers.FindAll(a => a.UserModel.IsGameMaster == false);

                foreach (var user in recepients.ToArray())
                {
                    _listener.SendObjectAsync(user.UserModel.Id, characterRequestPacket, Compress, false, false);
                }
            }
            else if (obj.GetType() == typeof(HeartbeatPacket))
            {
                var heartbeatPacket = (HeartbeatPacket)obj;
                //WriteLine($"Heartbeat received from client '{client.Id}'; first beat '{heartbeatPacket.FirstBeat}'; current beat '{heartbeatPacket.LastBeat}'");
                heartbeats.Add(heartbeatPacket);
            }
        }

        private static async void CustomHeaderReceivedAsync(IClientInfo client, object msg, IDictionary<object, object> metadata, Type objectType)
        {
            WriteLine($"The server received a message from the client with ID '{client.Id}' the message is '{msg}'");

            foreach (KeyValuePair<object, object> entry in metadata)
            {
                WriteLine($"Key: {entry.Key} , Value: {entry.Value}");

                if ((string)entry.Key == "command")
                {
                    switch (entry.Value)
                    {
                        case "GetClientList":
                            WriteLine($"Sending List of connected clients to client with ID '{client.Id}'");

                            foreach (var user in _listener.GetConnectedClients())
                            {
                                string username = "";

                                if (AuthenticatedUsers.Exists(a => a.UserModel.Id == user.Value.Id))
                                    username = AuthenticatedUsers.Find(a => a.UserModel.Id == user.Value.Id).UserModel.UserName;

                                WriteLine("Client ID: " + user.Value.Id + " with IPv4 : " + user.Value.RemoteIPv4);
                                await _listener.SendMessageAsync(client.Id, $"- ID '{user.Value.Id}'; Name '{username}'", Compress, false, false);
                            }
                            break;

                        case "SendAuthenticate":

                            break;

                        default:
                            break;
                    }
                }
            }
        }

        private static void MessageReceived(IClientInfo client, string msg)
        {
            string username = "";

            if (AuthenticatedUsers.Exists(a => a.UserModel.Id == client.Id))
                username = AuthenticatedUsers.Find(a => a.UserModel.Id == client.Id).UserModel.UserName;

            WriteLine($"The server has received a message from client {client.Id} ({username}) the message reads: {msg}");
        }

        private static void MessageSubmitted(IClientInfo client, bool close)
        {
            string username = "";

            if (AuthenticatedUsers.Exists(a => a.UserModel.Id == client.Id))
                username = AuthenticatedUsers.Find(a => a.UserModel.Id == client.Id).UserModel.UserName;

            WriteLine($"A message has been sent to client {client.Id} ({username})");
        }

        private static void ServerHasStarted()
        {
            WriteLine("The server has started");
        }

        private static void ErrorThrown(Exception exception)
        {
            WriteLine("The server has thrown an error. Message: " + exception.Message);
            WriteLine("Stacktrace: " + exception.StackTrace);
        }

        private static void MessageFailed(IClientInfo client, byte[] messageData, Exception exception)
        {
            string username = "";

            if (AuthenticatedUsers.Exists(a => a.UserModel.Id == client.Id))
                username = AuthenticatedUsers.Find(a => a.UserModel.Id == client.Id).UserModel.UserName;

            WriteLine($"The server has failed to deliver a message to client {client.Id} ({username})");
            WriteLine("Error message : " + exception.Message);
        }

        private static void ClientConnected(IClientInfo client)
        {
            WriteLine($"Client {client.Id} with IP {client.RemoteIPv4} has connected to the server");

            Task.Run(() =>
            {
                Thread.Sleep(5000);

                if (AuthenticatedUsers.Exists(a => a.UserModel.Id == client.Id) == false)
                {
                    WriteLine("Client has not authenticated in 5 seconds, disconnect");
                    if (_listener.IsConnected(client.Id))
                    {
                        _listener.Close(client.Id);
                    }
                    else
                    {
                        WriteLine("Client has self-disconnected");
                    }
                }
                else
                {
                    StartClientConnectionMonitoring(client);
                }
            });
        }

        private static void ClientDisconnected(IClientInfo client, DisconnectReason reason)
        {
            if (client != null)
            {
                var id = client.Id;

                if (AuthenticatedUsers.Exists(a => a.UserModel.Id == id))
                {
                    ChatPacket userLeaveNotification = new(ChatMessageType.Text, $"Nutzer '{AuthenticatedUsers.Find(a => a.UserModel.Id == id).UserModel.UserName}' hat den Chat verlassen", 0, "Server", 0, "", "Silver");
                    WriteLine($"Client {id} ({AuthenticatedUsers.Find(a => a.UserModel.Id == id).UserModel.UserName}) has disconnected from the server");

                    AuthenticatedUsers.Remove(AuthenticatedUsers.Find(a => a.UserModel.Id == id));

                    foreach (var user in AuthenticatedUsers.ToArray())
                    {
                        if (_listener.IsConnected(user.UserModel.Id))
                            _listener.SendObjectAsync(user.UserModel.Id, userLeaveNotification, Compress, false, false);
                    }
                }
            }

            SendUserListToUsers();
        }

        #endregion Events

        public static void SendUserListToUsers()
        {
            List<UserModel> userModelList = new();
            foreach (var user in AuthenticatedUsers.ToArray())
            {
                if (_listener.IsConnected(user.UserModel.Id))
                {
                    userModelList.Add(user.UserModel);
                }
                else
                {
                    WriteLine($"User {user.UserModel.Id} ({user.UserModel.UserName}) was removed from the list of authenticated users");
                    AuthenticatedUsers.Remove(user);
                }
            }

            foreach (var user in AuthenticatedUsers.ToArray())
            {
                try
                {
                    if (_listener.IsConnected(user.UserModel.Id))
                        _listener.SendObjectAsync(user.UserModel.Id, new ChatroomPacket(userModelList), Compress, false, false);
                }
                catch (Exception)
                {
                    AuthenticatedUsers.Remove(user);
                }
            }
        }

        private static void StartClientConnectionMonitoring(IClientInfo client)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(10000);

                    if (_listener.IsConnected(client.Id) == false && AuthenticatedUsers.Exists(a => a.UserModel.Id == client.Id))
                    {
                        AuthenticatedUsers.Remove(AuthenticatedUsers.Find(a => a.UserModel.Id == client.Id));
                        _listener.Close(client.Id);
                        break;
                    }

                    if (AuthenticatedUsers.Exists(a => a.UserModel.Id == client.Id))
                    {
                        var authpacket = AuthenticatedUsers.Find(a => a.UserModel.Id == client.Id);

                        if (heartbeats.Exists(h => h.Guid == authpacket.UserModel.UserIdentifier))
                        {
                            var lastbeat = heartbeats.FindLast(h => h.Guid == authpacket.UserModel.UserIdentifier);

                            if ((DateTime.UtcNow - lastbeat.LastBeat).TotalSeconds >= 30)
                            {
                                WriteLine($"User with id {client.Id} hasn't sent a heartbeat in over 30 seconds. Forcefully closing the connection now.");
                                AuthenticatedUsers.Remove(AuthenticatedUsers.Find(a => a.UserModel.Id == client.Id));
                                _listener.Close(client.Id);
                                break;
                            }
                        }
                    }
                }
            });
        }

        private static void WriteLine(string text)
        {
            System.Console.WriteLine($"{DateTime.Now} | {text}");
        }
    }
}