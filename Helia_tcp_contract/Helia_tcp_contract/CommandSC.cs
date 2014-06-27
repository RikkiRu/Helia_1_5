using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Helia_tcp_contract
{
    [Serializable]
    public class CommandServer
    {
        public typeOfCommandServer command;
        public object data;

        public CommandServer(typeOfCommandServer x, object data)
        {
            this.command = x;
            this.data = data;
        }
    }

    [Serializable]
    public enum typeOfCommandServer
    {
        getAll,
        KillMe,
    }

    //==================================================================================

    [Serializable]
    public class CommandClient
    {
        public typeOfCommandClient command;
        public object data;

        public CommandClient(typeOfCommandClient x, object data)
        {
            this.command = x;
            this.data = data;
        }
    }

    [Serializable]
    public enum typeOfCommandClient
    {
        thatsAll,
        AddPlanetNature,
    }
}
