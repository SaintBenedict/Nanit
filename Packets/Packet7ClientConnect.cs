using NaNiT.Functions;
using System;
using System.IO;
using NaNiT.Permissions;
using System.Text;

namespace NaNiT.Packets
{
    class Packet7ClientConnect : PacketBase
    {
        public Packet7ClientConnect(ClientThread clientThread, Object stream, Direction direction)
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
            mClient.myInfo.CryptoLogin = _cryptologin;
            mClient.myInfo.HostShortName = _userhost;
            
            MainProgram.logDebug("AssetDigest", "[" + mClient.myInfo.client + "] ");

            User userPData = new User(mClient.myInfo.CryptoLogin, DateTime.Now.ToString(), DateTime.Now.ToString(), mClient.myInfo.HostShortName, mClient.myInfo.UserIpAdress);
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
