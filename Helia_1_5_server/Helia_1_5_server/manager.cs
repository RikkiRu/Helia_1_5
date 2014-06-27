using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helia_tcp_contract;

namespace Helia_1_5_server
{
    static class manager
    {
        public static Random rand = new Random();

        public static Connection connection;

        public static List<Planet_nature> planets;
        public static List<Player> players;
        

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
    }
}
