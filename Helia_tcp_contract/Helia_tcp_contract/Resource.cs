using System;

namespace Helia_tcp_contract
{
    [Serializable]
    public class Resource
    {
        public ResouresType type;
        public int Count;
        public int Delta;
    }

    [Serializable]
    public enum ResouresType
    {
        garbage,
        air,
        happy,
        people,
        eat,
        technic,
        material,
        knowleges,
        water,
        waterDrink,
    }
}
