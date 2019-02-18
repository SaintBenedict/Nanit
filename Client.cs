using System;

namespace NaNiT
{
    public class UserActiveInfo
    {
        public string name;
        public string account;
        public string ip;
        public uint id;
        public string uuid;
        
        public int lastOnline = 0;
        
        public string client { get { if (String.IsNullOrEmpty(name)) return ip; else return name; } }
        
    }
}
