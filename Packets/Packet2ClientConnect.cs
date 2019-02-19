using NaNiT.Utils;
using System;
using System.IO;
using static NaNiT.Utils.Functions;

namespace NaNiT.Packets
{
    class Packet2ClientConnect : PacketBase
    {
        public Packet2ClientConnect(Connection clientThread, Object stream, Direction direction)
        {
            mClient = clientThread;
            mStream = stream;
            mDirection = direction;
        }

        public override Object OnReceive()
        {
            BinaryReader packetData = (BinaryReader)mStream;
            string[] datas = new string[2];
            datas = Function.ReadStrings(packetData, 2);

            string _cryptologin = datas[0];
            string _userhost = datas[1];

            // Identify player to server
            mClient.MyInfo.CryptoLogin = _cryptologin;
            mClient.MyInfo.HostShortName = _userhost;
            
            ClientProgram.logDebug("AssetDigest", "[" + mClient.MyInfo.client + "] ");
            
            return null;
        }

        public override void OnSend()
        {
            //This should never happen! We don't NEED to send it!
        }

        public override int getPacketID()
        {
            return 5;
        }
    }
}
