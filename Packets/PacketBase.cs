﻿using NaNiT.Utils;
using System;

namespace NaNiT.Packets
{
    abstract class PacketBase
    {
        protected Object mStream;
        protected Connection mClient;
        protected Direction mDirection;

        public abstract Object OnReceive();
  
        public abstract void OnSend();
  
        public abstract int getPacketID();
    }
}
