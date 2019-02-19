using System;
using System.IO;
using NaNiT.Functions;

namespace NaNiT.Packets
{
    class Packet5ChatReceive : PacketBase
    {
        public Packet5ChatReceive(ClientThread clientThread, Object stream, Direction direction)
        {
            mClient = clientThread;
            mStream = stream;
            mDirection = direction;
        }

        public override Object OnReceive()
        {
            BinaryReader packetData = (BinaryReader)mStream;
            
            string name = packetData.ReadString();
            string message = packetData.ReadString();

            return null;
        }

        public override void OnSend()
        {
            //Should this even happen either?
        }

        public override int getPacketID()
        {
            return 5;
        }
    }
}
