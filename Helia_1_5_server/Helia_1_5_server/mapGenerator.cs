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
        static int planetGeneratedInRing = 0;
        static int planetsToRing = 0;
        static int ringN = 0;
        static float distanceRings = 150;

        public static List<Planet_nature> genNature()
        {
            List<Planet_nature> res = new List<Planet_nature>();

            float[] zero = {0,0};
            res.Add(getNewPlanet(PlanetType.sun, zero));

            for (int i = 0; i < 20; i++ )
                res.Add(getNewPlanet(PlanetType.flat, coordPlanRing()));

            return res;
        }

        public static float[] coordPlanRing()
        {
            float[] xy = new float[2];

            planetGeneratedInRing++;


            if (planetGeneratedInRing > planetsToRing)
            {
                planetGeneratedInRing = 1;
                planetsToRing = 8 + ringN * 4;
                ringN++;
            }

            float angle = 360 / planetsToRing * planetGeneratedInRing / 57.0f;
            float radius = distanceRings * ringN;

            xy[0] = (float)Math.Sin((double)angle) * radius;
            xy[1] = (float)Math.Cos((double)angle) * radius;

            return xy;
        }

        static Planet_nature getNewPlanet(PlanetType type, float[] xy)
        {
            Planet_nature natPlanet = new Planet_nature();
            natPlanet.x=xy[0];
            natPlanet.y=xy[1];

            natPlanet.name=getPlanetName();

            natPlanet.resources = new Resource[3];
            for (int i = 0; i < natPlanet.resources.Length; i++ )
            {
                natPlanet.resources[i] = new Resource();
            }

            natPlanet.resources[0].type = ResouresType.garbage;
            natPlanet.resources[1].type=ResouresType.air;
            natPlanet.resources[2].type=ResouresType.water;

            int sectorsCount = 10;
            if (type == PlanetType.sun) sectorsCount = 1;

            natPlanet.sectors = new sector[sectorsCount];
            
            natPlanet.type = type;

            switch(type)
            {
                case PlanetType.flat:
                    natPlanet.radius = manager.rand.Next(10, 20);
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

                case PlanetType.sun:
                    natPlanet.name = "Helios";
                    natPlanet.radius = 50;
                    for (int i = 0; i < sectorsCount; i++)
                    {
                        natPlanet.sectors[i] = new sector();
                    }
                    break;
            }

            return natPlanet;
        }

        static string getPlanetName()
        {
            return "Planet #" + planetGeneratedInRing.ToString();
        }
    }
}
