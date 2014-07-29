using System;
using System.Collections.Generic;
using System.Drawing;

namespace Helia_tcp_contract
{
    [Serializable]
    public class Player
    {
        public string name;
        public Color color = Color.White;
        public PlayerState playerState;

        public List<Planet_government> governments = new List<Planet_government>();
        public List<Unit> units = new List<Unit>();
    }


    [Serializable]
    public enum PlayerState
    {
        online, offline
    }
}
