using NaNiT.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NaNiT.Packets
{
    class Packet5ConnectResponse : PacketBase
    {
        Dictionary<string, object> tmpArray = new Dictionary<string, object>();

        public Packet5ConnectResponse(Connection clientThread, Object stream, Direction direction)
        {
            mClient = clientThread;
            mStream = stream;
            mDirection = direction;
        }

        public override object OnReceive()
        {
            BinaryReader packetData = (BinaryReader)mStream;

            bool success = packetData.ReadBoolean();
            string crypto = packetData.ReadString();
            string rejectReason = packetData.ReadString();

            mClient.MyInfo.CryptoLogin = crypto;
            UserActiveInfo InfoStorage = mClient.MyInfo;
            
            
            return true;
        }

        public void Prepare(string rejectReason)
        {
            tmpArray.Add("rejectReason", rejectReason);
        }

        public override void OnSend()
        {
            MemoryStream packet = new MemoryStream();
            BinaryWriter packetWrite = new BinaryWriter(packet);

            packetWrite.Write(false);
            packetWrite.Write((short)0);
            byte[] buffer = Encoding.Unicode.GetBytes("rejectReason");
            packetWrite.Write((short)buffer.Length);
            packetWrite.Write(buffer);
            
        }

        public override int getPacketID()
        {
            return 2;
        }
    }
}
