using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace PhexensWuerfelraum.Logic.ClientServer
{
    public delegate void PacketEventHandler(object sender, PacketEvents e);

    public delegate void PersonalPacketEventHandler(object sender, PersonalPacketEvents e);

    public class SimpleServer
    {
        public IPAddress Address { get; private set; }
        public int Port { get; private set; }

        public IPEndPoint EndPoint { get; private set; }
        public Socket Socket { get; private set; }

        public bool IsRunning { get; private set; }
        public List<SimpleClient> Connections { get; private set; }

        private Task _receivingTask;
        private Task _checkClientHeartbeatTask;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Wrong suggestion")]
        private Dictionary<Guid, DateTime> ClientLastMessage = new Dictionary<Guid, DateTime>();

        public event PacketEventHandler OnConnectionAccepted;

        public event PacketEventHandler OnConnectionRemoved;

        public event PacketEventHandler OnPacketReceived;

        public event PacketEventHandler OnPacketSent;

        public event PersonalPacketEventHandler OnPersonalPacketSent;

        public event PersonalPacketEventHandler OnPersonalPacketReceived;

        public SimpleServer(IPAddress address, int port)
        {
            Address = address;
            Port = port;

            EndPoint = new IPEndPoint(address, port);

            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                ReceiveTimeout = 10000
            };
            Connections = new List<SimpleClient>();
        }

        public bool Open()
        {
            Socket.Bind(EndPoint);
            Socket.Listen(10);
            return true;
        }

        public async Task<bool> Start()
        {
            _receivingTask = Task.Run(() => MonitorStreams());
            _checkClientHeartbeatTask = Task.Run(() => CheckClientHeartbeat());

            IsRunning = true;

            await Listen();
            await _checkClientHeartbeatTask;
            await _receivingTask;

            Socket.Close();

            return true;
        }

        public bool Close()
        {
            IsRunning = false;
            Connections.Clear();
            return true;
        }

        public async Task<bool> Listen()
        {
            while (IsRunning)
            {
                if (Socket.Poll(100000, SelectMode.SelectRead))
                {
                    var newConnection = Socket.Accept();
                    if (newConnection != null)
                    {
                        var client = new SimpleClient();
                        var newGuid = await client.CreateGuid(newConnection);

                        await client.SendMessage(newGuid);

                        Connections.Add(client);

                        ClientLastMessage.Add(client.ClientGuid, DateTime.Now);

                        var e = BuildEvent(client, null, string.Empty);
                        OnConnectionAccepted?.Invoke(this, e);
                    }
                }

                Thread.Sleep(5);
            }

            return true;
        }

        private void CheckClientHeartbeat()
        {
            int timeoutHours = 12;

            while (IsRunning)
            {
                foreach (var client in Connections.ToList())
                {
                    ClientLastMessage.TryGetValue(client.ClientGuid, out DateTime lastMessageDateTime);

                    TimeSpan timeSpan = DateTime.Now - lastMessageDateTime;

                    if (timeSpan.TotalHours >= timeoutHours)
                    {
                        Console.WriteLine($"{DateTime.Now} | Haven't heard from '{client.ClientGuid}' in over {timeoutHours} hours; client removed from Connections");

                        var e5 = BuildEvent(client, null, string.Empty);
                        Connections.Remove(client);
                        OnConnectionRemoved?.Invoke(this, e5);
                        continue;
                    }
                }

                Thread.Sleep((int)new TimeSpan(0, 10, 0).TotalMilliseconds);
            }
        }

        private void MonitorStreams()
        {
            while (IsRunning)
            {
                foreach (var client in Connections.ToList())
                {
                    if (client.Socket.IsConnected() == true)
                    {
                        var readObject = ReadObject(client.Socket);

                        if (readObject == null)
                        {
                            Console.WriteLine($"{DateTime.Now} | readObject is null; client { client.ClientGuid } removed from Connections");

                            var e5 = BuildEvent(client, null, string.Empty);
                            Connections.Remove(client);
                            OnConnectionRemoved?.Invoke(this, e5);
                            continue;
                        }

                        var e1 = BuildEvent(client, null, readObject);
                        OnPacketReceived?.Invoke(this, e1);

                        if (readObject is PingPacket ping)
                        {
                            client.SendObject(ping).Wait();
                            continue;
                        }

                        if (readObject is PersonalPacket pp)
                        {
                            var destination = Connections.FirstOrDefault(c => c.ClientGuid.ToString() == pp.GuidId);
                            var e4 = BuildEvent(client, destination, pp);
                            OnPersonalPacketReceived?.Invoke(this, e4);

                            if (destination != null)
                            {
                                destination.SendObject(pp).Wait();
                                var e2 = BuildEvent(client, destination, pp);
                                OnPersonalPacketSent?.Invoke(this, e2);
                            }
                        }

                        if (readObject is ChatPacket cp)
                        {
                            SendObjectToClients(cp);
                        }
                    }
                    else
                    {
                        var e5 = BuildEvent(client, null, string.Empty);
                        Connections.Remove(client);
                        OnConnectionRemoved?.Invoke(this, e5);
                        continue;
                    }
                }

                Thread.Sleep(5);
            }
        }

        public void SendObjectToClients(object package)
        {
            foreach (var c in Connections.ToList())
            {
                if (package is ChatPacket chatP)
                {
                    chatP.DateTime = DateTime.Now;

                    ClientLastMessage.Remove(chatP.SenderGuid);
                    ClientLastMessage.Add(chatP.SenderGuid, chatP.DateTime);

                    // only send ChatPacket if...
                    if (c.IsGameMaster == true ||  // ...the recipient is a game master
                        chatP.RecipientGuid == c.ClientGuid ||  // ...recipient is intended recipient
                        chatP.RecipientGuid == Guid.Empty ||  // ...the recipient is everyone
                        chatP.SenderGuid == c.ClientGuid)           // ...the recipient is yourself
                    {
                        c.SendObject(package).Wait();
                        var e3 = BuildEvent(c, c, package);
                        OnPacketSent?.Invoke(this, e3);
                    }
                }
                else
                {
                    c.SendObject(package).Wait();
                    var e3 = BuildEvent(c, c, package);
                    OnPacketSent?.Invoke(this, e3);
                }
            }
        }

        private object ReadObject(Socket clientSocket)
        {
            byte[] data = new byte[clientSocket.ReceiveBufferSize];

            try
            {
                using (Stream s = new NetworkStream(clientSocket))
                {
                    s.Read(data, 0, data.Length);
                    var memory = new MemoryStream(data)
                    {
                        Position = 0
                    };

                    var formatter = new BinaryFormatter();
                    var obj = formatter.Deserialize(memory);

                    return obj;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} | Exception caught: {ex.Message} ({ex.InnerException?.Message})");
            }

            return null;
        }

        private PacketEvents BuildEvent(SimpleClient sender, SimpleClient receiver, object package)
        {
            return new PacketEvents
            {
                Sender = sender,
                Receiver = receiver,
                Packet = package
            };
        }

        private PersonalPacketEvents BuildEvent(SimpleClient sender, SimpleClient receiver, PersonalPacket package)
        {
            return new PersonalPacketEvents
            {
                Sender = sender,
                Receiver = receiver,
                Packet = package
            };
        }
    }
}