using Helia_tcp_contract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;


namespace Helia_1_5_server
{
    class Connection
    {
        int port;
        Socket serverSocket;
        int sizeOfMessage = 2048;

        List<ConnectionInfo> connections = new List<ConnectionInfo>();

        class ConnectionInfo
        {
            public Socket Socket;
            public byte[] Buffer;
            public string name;
           
        }

        public void Start()
        {
            port = 8083;
            IPHostEntry localInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPEndPoint myEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            serverSocket = new Socket(myEP.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(myEP);
            serverSocket.Listen((int)SocketOptionName.MaxConnections);
            serverSocket.BeginAccept(AcceptCallback, serverSocket);
            Console.WriteLine("Запуск сервера произведен");
        }

        void AcceptCallback(IAsyncResult result)
        {
            try
            {
                Socket s = (Socket)result.AsyncState;
                ConnectionInfo connection = new ConnectionInfo();
                connection.name = "init";
                connection.Socket = s.EndAccept(result);
                connection.Buffer = new byte[sizeOfMessage];
                connection.Socket.BeginReceive(connection.Buffer, 0, sizeOfMessage, SocketFlags.None, new AsyncCallback(RecieveCallback), connection);
                serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), result.AsyncState);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        BinaryFormatter binFormat = new BinaryFormatter();

        void RecieveCallback(IAsyncResult result)
        {
            ConnectionInfo connection = (ConnectionInfo)result.AsyncState;

            try
            {
                connection.Socket.EndReceive(result);
                MemoryStream x = new MemoryStream(connection.Buffer);
                CommandServer data = (CommandServer)binFormat.Deserialize(x);

                switch(data.command)
                {
                    case typeOfCommandServer.getAll:
                        try
                        {
                            connection.name = data.data.ToString();

                            if (connections.Where(c => c.name == data.data.ToString()).FirstOrDefault() != null) 
                                throw new Exception("Такое подключение уже есть! ");
                            if (manager.players.Where(c => (c.name == data.data.ToString() && c.playerState == PlayerState.online)).FirstOrDefault() != null)
                                throw new Exception("Такой игрок есть онлайн! ");

                            Console.WriteLine("Тук-тук! У нас новое подключение! " + connection.name);
                            connections.Add(connection);

                            sendAll(connection.Socket);

                            Player searchPl = manager.players.Where(c => c.name == connection.name).FirstOrDefault();
                            if (searchPl == null)
                            {
                                createNewPlayer(connection.name);
                            }
                            else
                            {
                               // searchPl.playerState = PlayerState.online; //ОБЯЗАТЕЛЬНО ВРУБИТЬ ПРИ РЕЛИЗЕ
                            }
                        }
                        catch(Exception ex)
                        {
                            string message = ex.Message + connection.name;
                            Console.WriteLine(message);
                            send(connection.Socket, new CommandClient(typeOfCommandClient.Exception, message));
                        }
                        break;

                    case typeOfCommandServer.KillMe:
                        Player pForOff = manager.players.Where(c => c.name == connection.name).FirstOrDefault();
                        if( pForOff!=null)
                        {
                            pForOff.playerState = PlayerState.offline;
                            Console.WriteLine("Отключен " + pForOff.name);
                        }
                        connections.Remove(connection);
                        break;

                    case typeOfCommandServer.ping:
                        send(connection.Socket, new CommandClient(typeOfCommandClient.Exception, "Сервер доступен"));
                        break;
                }

                connection.Socket.BeginReceive(connection.Buffer, 0, sizeOfMessage, SocketFlags.None, new AsyncCallback(RecieveCallback), connection);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                connections.Remove(connection);
            }
        }

        public void close()
        {
            try
            {
                serverSocket.Shutdown(SocketShutdown.Both);
            }
            catch
            {
                serverSocket.Close();
            }
            Console.WriteLine("Сервер был остановлен");
        }


        void send(Socket s, CommandClient command)
        {
            MemoryStream x = new MemoryStream();
            binFormat.Serialize(x, command);
            s.Send(x.GetBuffer());
        }

        //все данные игроку
        void sendAll(Socket connection)
        {
            lock (connection)
            {
                for (int i = 0; i < manager.players.Count; i++)
                {
                    sendPlayer(connection, manager.players[i].name);
                }

                for (int i = 0; i < manager.planets.Count; i++)
                {
                    MemoryStream x = new MemoryStream();
                    binFormat.Serialize(x, new CommandClient(typeOfCommandClient.AddPlanetNature, manager.planets[i]));
                    connection.Send(x.GetBuffer());
                }
                //
                //

                send(connection, new CommandClient(typeOfCommandClient.thatsAll, 0));
            }
        }

        void sendPlayer(Socket s, string name)
        {
            Player p = manager.players.Where(c => c.name == name).FirstOrDefault();
            if (p == null) return;
            send(s, new CommandClient(typeOfCommandClient.AddPlayer, p));
        }

        void createNewPlayer(string username)
        {
            Console.WriteLine("Создаем новго игрока " + username);
            Player user = new Player();
            user.name = username;
            //user.playerState = PlayerState.online; ВКЛЮЧИТЬ НА РЕЛИЗЕ
            user.color = Color.Red;
            manager.players.Add(user);
            sendToAll(new CommandClient(typeOfCommandClient.AddPlayer, user));

            manager.makeUnit(user, UnitType.newPlayer, 0, 0);
        }

        public void sendToAll(CommandClient command)
        {
            lock (connections)
            {
                for (int i = 0; i < connections.Count; i++)
                {
                    send(connections[i].Socket, command);
                }
            }
        }
    }
}
