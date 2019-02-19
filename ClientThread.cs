using System;
using NaNiT.Functions;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Threading;
using NaNiT.Packets;

namespace NaNiT
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    class ClientThread
    {
        public UserActiveInfo myInfo = new UserActiveInfo();
        public ClientState statusOfCurrentClient = ClientState.PendingConnect;

        private TcpClient ConnectingSocket;
        private BinaryReader myStreamToRead;
        private BinaryWriter myStreamToWrite;
        
        public int kickTargetTimestamp = 0;
        public bool connectionAlive { get { if (ConnectingSocket.Connected && statusOfCurrentClient != ClientState.Disposing) return true; else return false; } }

        public ClientThread(TcpClient newConnection)
        {
            ConnectingSocket = newConnection;
        }

        public void run()
        {
            try
            {
                myStreamToRead = new BinaryReader(ConnectingSocket.GetStream());
                myStreamToWrite = new BinaryWriter(ConnectingSocket.GetStream());

                IPEndPoint ipep = (IPEndPoint)ConnectingSocket.Client.RemoteEndPoint;
                IPAddress ipa = ipep.Address;
                myInfo.UserIpAdress = ipep.Address.ToString();

                Thread ThisThread = Thread.CurrentThread;
                if (ThisThread.Name == null)
                    ThisThread.Name = "ClientThread " + ipa.ToString(); 

                MainProgram.logInfo("[" + myInfo.client + "] Accepting new connection.");

                // Forwarding for data from CLIENT (cIn) to SERVER (sOut)
                new Thread(new ThreadStart(new ForwardThread(this, myStreamToRead, myStreamToWrite, Direction.Client).Run)).Start();
            }
            catch (Exception e)
            {
                RejectPreConnected("Ошибка при взимодействии с сокетом сервера");
                MainProgram.logException("ClientThread Exception: " + e.Message);
            }
        }

        public void SendClientPacket(Packet packetID, byte[] packetData)
        {
            if (kickTargetTimestamp != 0) return;
            try
            {
                myStreamToWrite.Write((short)packetID);
                myStreamToWrite.Write(packetData.Length);
                myStreamToWrite.Write(packetData);
                myStreamToWrite.Flush();
            }
            catch (Exception e)
            {
                ErrorDisconnect(Direction.Client, "Неудача при отправке пакета: " + e.Message);
            }
        }

        public void SendServerPacket(Packet packetID, byte[] packetData)
        {
            try
            {
                myStreamToWrite.Write((short)packetID);
                myStreamToWrite.Write(packetData.Length);
                myStreamToWrite.Write(packetData);
                myStreamToWrite.Flush();
            }
            catch (Exception e)
            {
                ErrorDisconnect(Direction.Server, "Неудача при отправке пакета: " + e.Message);
            }
        }
        
        public void KickClient(string reason)
        {
            SendServerPacket(Packet.ClientDisconnect, new byte[1]);             //This causes the server to gracefully save and remove the player, and close its connection, even if the client ignores ServerDisconnect.
            kickTargetTimestamp = Function.getTimestamp() + 7;
        }

        private void DoDisconnect(bool log)
        {
            if (statusOfCurrentClient != ClientState.Disposing)
            {
                statusOfCurrentClient = ClientState.Disposing;
                try
                {
                    ConnectingSocket.Close();
                }
                catch (Exception) { }
            }
        }

        public void RejectPreConnected(string reason)
        {
            Packet5ConnectResponse packet = new Packet5ConnectResponse(this, false, Direction.Client);
            packet.Prepare(reason);
            packet.OnSend();
            ForceDisconnect(reason);
        }

        public void ForceDisconnect(string reason)
        {
            if (statusOfCurrentClient != ClientState.Disposing)
                MainProgram.logError("[" + myInfo.client + "] Dropped for " + reason);
            DoDisconnect(true);
        }

        public void ErrorDisconnect(Direction direction, string reason)
        {
            if (statusOfCurrentClient != ClientState.Disposing)
                MainProgram.logError("[" + myInfo.client + "] Dropped by parent " + direction.ToString() + " for " + reason);
            DoDisconnect(true);
        }


        public void forceDisconnect()
        {
            DoDisconnect(false);
        }
    }
}
