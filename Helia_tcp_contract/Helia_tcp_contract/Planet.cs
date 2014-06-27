using System;
using System.Collections.Generic;

namespace Helia_tcp_contract
{
    [Serializable]
    public enum PlanetType
    {
        flat
    }

    [Serializable]
    public class Planet_nature
    {
        public float x;
        public float y;
        public float radius;

        public string name;
        public sector[] sectors;
        public Resource[] resources;
        public PlanetType type;
    }

    [Serializable]
    public class Planet_government
    {
        public Resource[] resources;
        public string naturePlanetName;
    }

    [Serializable]
    public class sector
    {
        public string owner;
        public Building building;
        public Building natureBuilding;
    }
}
