using Helia_tcp_contract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
                lock (connections) connections.Add(connection);
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
            //Console.WriteLine("что-то произошло");

            try
            {
                connection.Socket.EndReceive(result);
                MemoryStream x = new MemoryStream(connection.Buffer);
                CommandServer data = (CommandServer)binFormat.Deserialize(x);

                switch(data.command)
                {
                    case typeOfCommandServer.getAll:
                        connection.name = data.data.ToString();
                        connections.Add(connection);
                        sendAll(connection.Socket); 
                        break;

                    case typeOfCommandServer.KillMe:
                        connections.Remove(connection);
                        break;
                }
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







        //все данные игроку
        void sendAll(Socket connection)
        {
            Console.WriteLine("get all command");

            for (int i = 0; i < manager.planets.Count; i++)
            {
                MemoryStream x = new MemoryStream();
                binFormat.Serialize(x, new CommandClient(typeOfCommandClient.AddPlanetNature, manager.planets[i]));
                connection.Send(x.GetBuffer());
            }

            MemoryStream end = new MemoryStream();
            binFormat.Serialize(end, new CommandClient(typeOfCommandClient.thatsAll, 0));
            connection.Send(end.GetBuffer());
        }
    }
}
