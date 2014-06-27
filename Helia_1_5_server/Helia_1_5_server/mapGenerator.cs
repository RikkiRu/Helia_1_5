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
            res.Add(getNewPlanet(PlanetType.flat, 30, 30, 10));
            res.Add(getNewPlanet(PlanetType.flat, 80, 30, 5));
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
            natPlanet.radius = manager.rand.Next(10, 20);
            natPlanet.type = type;

            switch(type)
            {
                case PlanetType.flat:
                    for (int i = 0; i < sectorsCount; i++ )
                    {
                        natPlanet.sectors[i] = new sector();
                        if (manager.rand.Next(0, 2) == 1)
                        {
                            natPlanet.sectors[i].natureBuilding = new Building(buildingsEnum.grass);
                        }
                        else
                        {
                            natPlanet.sectors[i].natureBuilding = new Building(buildingsEnum.forest);
                        }
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
