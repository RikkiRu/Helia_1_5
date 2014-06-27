using System;

namespace Helia_tcp_contract
{
    [Serializable]
    public enum buildingsEnum
    {
       grass,
       city,
       forest
    }

    [Serializable]
    public class Building
    {
        public buildingsEnum type; 

        public Building(buildingsEnum type)
        {
            this.type = type;
        }
    }
}
