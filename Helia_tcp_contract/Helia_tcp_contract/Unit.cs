using System;

namespace Helia_tcp_contract
{
    [Serializable]
    public class Unit
    {
        public float x;
        public float y;
        public UnitType type;
        public string owner;
    }

    [Serializable]
    public enum UnitType
    {
        newPlayer,
        colonizator,
    }
}
