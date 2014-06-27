using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helia_tcp_contract;

namespace Helia_1_5_server
{
    class mapGenerator
    {
        static int planetGenerated = 0;

        public static List<Planet_nature> genNature()
        {
            List<Planet_nature> res = new List<Planet_nature>();
            res.Add(getNewPlanet(PlanetType.flat, 50, 50, 10));
            return res;
        }

        static Planet_nature getNewPlanet(PlanetType type, float x, float y, int sectorsCount)
        {
            Planet_nature natPlanet = new Planet_nature();
            natPlanet.x=x;
            natPlanet.y=y;
            natPlanet.name=getPlanetName();

            natPlanet.resources = new Resource[3];
            for (int i = 0; i < natPlanet.resources.Length; i++ )
            {
                natPlanet.resources[i] = new Resource();
            }

            natPlanet.resources[0].type = ResouresType.garbage;
            natPlanet.resources[1].type=ResouresType.air;
            natPlanet.resources[2].type=ResouresType.water;
            natPlanet.sectors = new sector[sectorsCount];
            natPlanet.radius = manager.rand.Next(20, 40);
            natPlanet.type = type;

            switch(type)
            {
                case PlanetType.flat:
                    for (int i = 0; i < sectorsCount; i++ )
                    {
                        natPlanet.sectors[i] = new sector();
                        natPlanet.sectors[i].natureBuilding = new Building(buildingsEnum.grass);
                    }
                    break;
            }

            return natPlanet;
        }

        static string getPlanetName()
        {
            planetGenerated++;
            return "Planet #" + planetGenerated.ToString();
        }
    }
}
