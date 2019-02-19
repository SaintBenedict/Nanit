using NaNiT.Permissions;
using NaNiT.Utils;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using static NaNiT.GlobalVariable;
using static NaNiT.Utils.Functions;

namespace NaNiT
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    class Connection
    {
        protected internal TcpClient Tcp;
        _ClientThread ClientThis;
        public UserActiveInfo MyInfo;
        protected internal NetworkStream StreamOfClient;
        public ClientState statusOfCurrentClient = ClientState.PendingConnect;
        private BinaryReader myStreamToRead;
        private BinaryWriter myStreamToWrite;
        public int kickTargetTimestamp = 0;

        public bool connectionAlive { get { if (Tcp.Connected && ClientProgram.CurrentClientStatus != _ClientState.Aborted) return true; else return false; } }

        public Connection(TcpClient NewTcp, _ClientThread ThisStarter, NetworkStream thisNewStream)
        {
            Tcp = NewTcp;
            ClientThis = ThisStarter;
            StreamOfClient = thisNewStream;
            gl_b_serverIsConnected = true;
            _ClientThread.connections.Add(this);
            MyInfo = new UserActiveInfo();
            MyInfo.CryptoLogin = gl_s_OSdateCrypt;
            MyInfo.HostShortName = gl_s_userName;
            
        }

        // Первое подключение к серверу
        public void Start()
        {
            try
            {
                myStreamToRead = new BinaryReader(Tcp.GetStream());
                myStreamToWrite = new BinaryWriter(Tcp.GetStream());
                ClientProgram.logInfo("Установлена связь с сервером.");
                new Thread(new ThreadStart(new RearwardThread(this, myStreamToRead, myStreamToWrite, Direction.Server).Run)).Start();
            }
            catch (Exception e)
            {
                if (gl_b_serverIsConnected)
                {
                    Disconnect();
                }
                ClientProgram.logException("ClientThread Exception: " + e.Message);
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
            if (ClientProgram.CurrentClientStatus != _ClientState.Aborted)
            {
                ClientProgram.CurrentClientStatus = _ClientState.Aborted;
                try
                {
                    Tcp.Close();
                }
                catch (Exception) { }
            }
        }
        
        public void ForceDisconnect(string reason)
        {
            if (ClientProgram.CurrentClientStatus != _ClientState.Aborted)
                ClientProgram.logError("[" + MyInfo.client + "] Dropped for " + reason);
            DoDisconnect(true);
        }

        public void ErrorDisconnect(Direction direction, string reason)
        {
            if (ClientProgram.CurrentClientStatus != _ClientState.Aborted)
                ClientProgram.logError("[" + MyInfo.client + "] Dropped by parent " + direction.ToString() + " for " + reason);
            DoDisconnect(true);
        }


        public void forceDisconnect()
        {
            DoDisconnect(false);
        }

                // отключение
        public static void Disconnect()
        {
            if (gl_b_serverIsConnected)
            {
                gl_b_serverIsConnected = false;
            }
        }
    }
}

