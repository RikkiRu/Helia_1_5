using System;
using System.Collections.Generic;
using System.Drawing;

namespace Helia_tcp_contract
{
    [Serializable]
    public class Player
    {
        public string name;
        public Color color;

        public Dictionary<string, Relations> relations;
        public List<Planet_government> governments;
        public List<Unit> units;
    }

    [Serializable]
    public enum Relations
    {
        none,
        enemy,
        friend
    }
}
