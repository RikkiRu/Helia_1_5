using Helia_tcp_contract;
using System;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Helia_1_5_client
{
    class ConnectionClient
    {
        Random rand = new Random();
        Socket ClientSocket;
        string username;
        Thread listen;
        int sizeOfMessage = 2048;
        BinaryFormatter binFormat = new BinaryFormatter();

        public void connect()
        {
            username = "";
            for (int i = 0; i < 5; i++)
            {
                username += (char)rand.Next(1, 127);
            }

            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ClientSocket.Connect("127.0.0.1", 8083);
            listen = new Thread(StartListen);
            listen.Start();
        }


        public void send(CommandServer x)
        {
             MemoryStream data = new MemoryStream();
             binFormat.Serialize(data, x);
             ClientSocket.Send(data.GetBuffer());
        }

        void StartListen()
        {
            while (true)
            {
                byte[] buffer = new byte[sizeOfMessage];
                ClientSocket.Receive(buffer);
                MemoryStream x = new MemoryStream(buffer);
                CommandClient data = (CommandClient)binFormat.Deserialize(x);
                switch(data.command)
                {
                    case typeOfCommandClient.AddPlanetNature:
                        Render.planets.Add(new dPlanet((Planet_nature)data.data));
                        break;
                }
            }
        }

        public void disconnect()
        {
            listen.Abort();
            ClientSocket.Close();
        }
    }
}
