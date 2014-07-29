using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helia_tcp_contract;
using System.Drawing;

namespace Helia_1_5_server
{
    static class manager
    {
        public static Random rand = new Random();

        public static Connection connection;

        public static List<Planet_nature> planets;
        public static List<Player> players;

        public static Color getRandColor()
        {
            Color res = Color.FromArgb(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256));
            return res;
        }

        public static void start()
        {
            planets = new List<Planet_nature>();
            players = new List<Player>();

            generate();

            connection = new Connection();
            connection.Start();
        }

        static void generate()
        {
            planets = mapGenerator.genNature();
        }

        public static void stop()
        {
            connection.close();
        }

        public static void makeUnit (Player who, UnitType type, float x, float y)
        {
            Unit nUn = new Unit();
            nUn.type = type;
            nUn.x = x;
            nUn.y = y;
            nUn.owner = who.name;

            who.units.Add(nUn);
            connection.sendToAll(new CommandClient(typeOfCommandClient.AddUnit, nUn));
        }
    }
}
