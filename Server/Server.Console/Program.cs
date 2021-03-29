using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using SimpleSockets;
using SimpleSockets.Messaging;
using SimpleSockets.Messaging.Metadata;
using SimpleSockets.Server;
using PhexensWuerfelraum.Logic.ClientServer;
using System.Reflection;

namespace PhexensWuerfelraum.Server.Console
{
    internal class Program
    {
        private static int Port;
        private static bool UseSSL;
        private static bool Compress;

        private static List<AuthPacket> AuthenticatedUsers = new List<AuthPacket>();
        private static SimpleSocketListener _listener;

        private static void Main(string[] args)
        {
            WriteLine("Starting the server");

            var config = new ConfigurationBuilder()
                .AddIniFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config"), "settings.ini"), optional: true, reloadOnChange: true)
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
                var privateKeyPath = Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config"), privKeyFileName);
                var publicKeyFileName = "PublicKey.pem";
                var publicKeyPath = Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config"), publicKeyFileName);

                if (!File.Exists(privateKeyPath))
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

                //var cert = new X509Certificate2(File.ReadAllBytes(Path.GetFullPath(@"C:\Users\" + Environment.UserName + @"\Desktop\test.pfx")), "Password"); // Generate: https://raw.githubusercontent.com/Cloet/SimpleSockets/master/Self-SignedCertificate%20Script.ps1

                _listener = new SimpleSocketTcpSslListener(cert);
            }
            else
            {
                _listener = new SimpleSocketTcpListener();
            }

            _listener.ObjectSerializer = new BinarySerializer();
            _listener.AllowReceivingFiles = true;

            BindEvents();
            _listener.StartListening(Port);

            while (true)
            {
                Options();

                System.Console.Read();
            }
        }

        private static void Options()
        {
            if (_listener.IsRunning)
            {
                System.Console.WriteLine("");
                WriteLine("Available options:");
                WriteLine("    - send a message   (M)");
                WriteLine("    - send a broadcast (BC)");

                var option = System.Console.ReadLine();

                if (option != null)
                    switch (option.ToUpper())
                    {
                        case "M":
                            SendMessage();
                            break;

                        case "BC":
                            BroadcastMessageAsync();
                            break;

                        default:
                            Options();
                            break;
                    }
                else
                {
                    Options();
                }
            }
        }

        private static async void BroadcastMessageAsync()
        {
            WriteLine("Enter your message you want to send to every client: ");
            var message = System.Console.ReadLine();
            var clients = _listener.GetConnectedClients();

            foreach (var client in clients)
            {
                await _listener.SendMessageAsync(client.Value.Id, message, Compress, false, false);
            }
        }

        private static int ShowClients()
        {
            var ids = new List<int>();
            var clients = _listener.GetConnectedClients();
            foreach (var client in clients)
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

        private static async void SendMessage()
        {
            System.Console.Clear();
            var id = ShowClients();

            WriteLine("Enter your message you want to send to the server: ");
            var message = System.Console.ReadLine();

            await _listener.SendMessageAsync(id, message, Compress, false, false);
        }

        //private static async void SendMessageContract()
        //{
        //    System.Console.Clear();
        //    var id = ShowClients();

        //    Write("Press enter to send a MessageContract...  ");
        //    System.Console.ReadLine();

        //    await _listener.SendMessageContractAsync(id, ChatPacketContract, _compress, _encrypt, false);
        //}

        //private static async void SendCustom()
        //{
        //    System.Console.Clear();
        //    var id = ShowClients();

        //    Write("Enter the header you want to use for the transmission...  ");
        //    var header = System.Console.ReadLine();

        //    Write("Enter the message you want to send...  ");
        //    var message = System.Console.ReadLine();

        //    await _listener.SendCustomHeaderAsync(id, message, header, _compress, _encrypt, false);
        //}

        //private static async void SendFile()
        //{
        //    System.Console.Clear();
        //    var id = ShowClients();

        //    Write("Enter the path to the file you want to send to the server... ");
        //    var path = System.Console.ReadLine();

        //    Write("Enter the path on the server where the file should be stored... ");
        //    var targetPath = System.Console.ReadLine();

        //    await _listener.SendFileAsync(id, path, targetPath, _compress, _encrypt, false);
        //}

        //private static async void SendFolder()
        //{
        //    System.Console.Clear();
        //    var id = ShowClients();

        //    Write("Enter the path to the folder you want to send to the server... ");
        //    var path = System.Console.ReadLine();

        //    Write("Enter the path on the server where the folder should be stored... ");
        //    var targetPath = System.Console.ReadLine();

        //    await _listener.SendFolderAsync(id, path, targetPath, true, false);
        //}

        //private static async void SendObject()
        //{
        //    System.Console.Clear();
        //    var id = ShowClients();

        //    var person = new Person("Test", "FirstName", "5th Avenue");

        //    WriteLine("Press enter to send an object.");
        //    System.Console.ReadLine();

        //    await _listener.SendObjectAsync(id, person);
        //}

        #region Events

        private static void BindEvents()
        {
            _listener.SslAuthStatus += _listener_SslAuthStatus;
            //_listener.FileReceiver += ListenerOnFileReceiver;
            _listener.MessageReceived += MessageReceived;
            _listener.MessageSubmitted += MessageSubmitted;
            _listener.MessageWithMetaDataReceived += CustomHeaderReceivedAsync;
            _listener.ClientDisconnected += ClientDisconnected;
            _listener.ClientConnected += ClientConnected;
            _listener.ServerHasStarted += ServerHasStarted;
            _listener.MessageFailed += MessageFailed;
            _listener.ServerErrorThrown += ErrorThrown;
            _listener.ObjectReceived += ListenerOnObjectReceived;
            _listener.MessageUpdateFileTransfer += ListenerOnMessageUpdateFileTransfer;
            _listener.ServerLogs += _listener_ServerLogs;
        }

        private static void _listener_SslAuthStatus(IClientInfo client, AuthStatus status)
        {
            if (status == AuthStatus.Failed)
                WriteLine("Server failed to authenticate certificate of client " + client.Id + " " + client.Guid + ".");
            if (status == AuthStatus.Success)
                WriteLine("Server authenticate certificate of client " + client.Id + " " + client.Guid + ".");
        }

        private static void _listener_ServerLogs(string log)
        {
            WriteLine(log);
        }

        //*****Begin Events************///

        private static void ListenerOnMessageUpdate(IClientInfo client, string msg, string header, MessageType msgType, MessageState state)
        {
            // WriteLine("Sending message to client: msg = " + msg + ", header = " + header);
        }

        private static void ListenerOnMessageUpdateFileTransfer(IClientInfo client, string origin, string loc, double percentageDone, MessageState state)
        {
            //WriteLine("Sending message to client: " + percentageDone);
        }

        private static void ListenerOnObjectReceived(IClientInfo client, object obj, Type objType)
        {
            WriteLine("Received an object of type = " + objType.FullName);

            if (obj.GetType() == typeof(AuthPacket))
            {
                var authPacket = (AuthPacket)obj;

                WriteLine($"User '{authPacket.UserModel.UserName}' has sent an authentication packet");

                authPacket.UserModel.Id = client.Id;
                AuthenticatedUsers.Add(authPacket);

                _listener.SendObject(client.Id, authPacket, Compress, false, false);

                List<UserModel> userModelList = new List<UserModel>();
                foreach (var user in AuthenticatedUsers)
                {
                    userModelList.Add(user.UserModel);
                }

                ChatPacket userJoinNotification = new ChatPacket(ChatMessageType.Text, $"Nutzer {authPacket.UserModel.UserName} ist dem Chat beigetreten", 0, "Server", 0, "", "Silver");

                foreach (var user in AuthenticatedUsers)
                {
                    _listener.SendObject(user.UserModel.Id, new ChatroomPacket(userModelList), Compress, false, false);
                    _listener.SendObject(user.UserModel.Id, userJoinNotification, Compress, false, false);
                }
            }
            else if (obj.GetType() == typeof(ChatPacket))
            {
                var chatPacket = (ChatPacket)obj;
                chatPacket.FromId = client.Id;

                if (chatPacket.ToId == 0) // to everyone
                {
                    foreach (var user in AuthenticatedUsers)
                    {
                        _listener.SendObject(user.UserModel.Id, chatPacket, Compress, false, false);
                    }
                }
                else // only to the recepient and the game master
                {
                    var recepients = AuthenticatedUsers.FindAll(a => a.UserModel.Id == chatPacket.ToId || a.UserModel.IsGameMaster == true);

                    foreach (var user in recepients)
                    {
                        _listener.SendObject(user.UserModel.Id, chatPacket, Compress, false, false);
                    }
                }
            }
        }

        //private static void ListenerOnFileReceiver(IClientInfo client, int currentPart, int totalParts, string loc, MessageState state)
        //{
        //    if (state == MessageState.ReceivingData)
        //    {
        //        //if (progress == null)
        //        //{
        //        //    progress = new ProgressBar();
        //        //    System.Console.Write("Receiving a File... ");
        //        //}

        //        var progressDouble = ((double)currentPart / totalParts);

        //        //progress.Report(progressDouble);

        //        if (progressDouble >= 1.00)
        //        {
        //            //progress.Dispose();
        //            //progress = null;
        //            Thread.Sleep(200);
        //            WriteLine("File Transfer Complete");
        //        }
        //    }

        //    if (state == MessageState.Decrypting)
        //        WriteLine("Decrypting File this might take a while.");
        //    if (state == MessageState.Decompressing)
        //        WriteLine("Decompressing the File this might take a while.");
        //    if (state == MessageState.DecompressingDone)
        //        WriteLine("Decompressing has finished.");
        //    if (state == MessageState.DecryptingDone)
        //        WriteLine("Decrypting has finished.");
        //    if (state == MessageState.Completed)
        //        WriteLine("File received and stored at location: " + loc);
        //}

        //private static async void CustomHeaderReceivedAsync(IClientInfo client, string msg, string header)
        private static async void CustomHeaderReceivedAsync(IClientInfo client, object msg, IDictionary<object, object> metadata, Type objectType)
        {
            //WriteLine($"The server received a message from the client with ID '{client.Id}' the header is '{header}' and the message is '{msg}'");

            foreach (KeyValuePair<object, object> entry in metadata)
            {
                WriteLine($"Key: {entry.Key} , Value: {entry.Value}");

                if ((string)entry.Key == "command")
                {
                    switch (entry.Value)
                    {
                        case "GetClientList":
                            WriteLine($"Sending List of connected clients to client with ID '{client.Id}'");

                            var clients = _listener.GetConnectedClients();

                            foreach (var user in clients)
                            {
                                WriteLine("Client ID: " + user.Value.Id + " with IPv4 : " + user.Value.RemoteIPv4);
                                await _listener.SendMessageAsync(client.Id, $"- ID '{user.Value.Id.ToString()}'; Name '{ AuthenticatedUsers.Find(a => a.UserModel.Id == user.Value.Id).UserModel.UserName}'", Compress, false, false);
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
            //totalmsg++;
            //WriteLine("The server has received a message from client " + client.Id + " with name : " + client.ClientName + " and guid : " + client.Guid);
            //WriteLine("The client is running on " + client.OsVersion + " and UserDomainName = " + client.UserDomainName);
            WriteLine("The server has received a message from client " + client.Id + " the message reads: " + msg);
        }

        private static void MessageSubmitted(IClientInfo client, bool close)
        {
            WriteLine("A message has been sent to client " + client.Id);
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
            WriteLine("The server has failed to deliver a message to client " + client.Id);
            WriteLine("Error message : " + exception.Message);
        }

        private static void ClientConnected(IClientInfo client)
        {
            WriteLine("Client " + client.Id + " with IPv4 " + client.RemoteIPv4 + " has connected to the server.");

            Task.Run(() =>
            {
                Thread.Sleep(5000);

                if (AuthenticatedUsers.Exists(a => a.UserModel.Id == client.Id) == false)
                {
                    WriteLine("Client has not authenticated in 5 seconds, disconnect");
                    _listener.Close(client.Id);
                }
            });
        }

        private static void ClientDisconnected(IClientInfo client, DisconnectReason reason)
        {
            var id = 0;
            if (client != null)
                id = client.Id;

            WriteLine("Client " + id + " has disconnected from the server.");

            if (AuthenticatedUsers.Exists(a => a.UserModel.Id == id))
            {
                ChatPacket userLeaveNotification = new ChatPacket(ChatMessageType.Text, $"Nutzer {AuthenticatedUsers.Find(a => a.UserModel.Id == id).UserModel.UserName} hat den Chat verlassen", 0, "Server", 0, "", "Silver");

                AuthenticatedUsers.Remove(AuthenticatedUsers.Find(a => a.UserModel.Id == id));

                foreach (var user in AuthenticatedUsers)
                {
                    _listener.SendObject(user.UserModel.Id, userLeaveNotification, Compress, false, false);
                }
            }
        }

        #endregion Events

        private static void Write(string text)
        {
            System.Console.Write($"{DateTime.Now} | {text}");
        }

        private static void WriteLine(string text)
        {
            System.Console.WriteLine($"{DateTime.Now} | {text}");
        }
    }
}