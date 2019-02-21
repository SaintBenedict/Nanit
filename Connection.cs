using NaNiT.Permissions;
using NaNiT.Utils;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static NaNiT.GlobalVariable;
using static NaNiT.Utils.Functions;

namespace NaNiT
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    class Connection
    {
        protected internal TcpClient Tcp;
        AttemptingServer ClientThis;
        public UserActiveInfo MyInfo;
        protected internal NetworkStream StreamOfClient;
        public ClientState MyStateOnServer = ClientState.PendingConnect;
        public _ClientState UserState = MainClient.StateMyActtivity;
        public ServChecker ServState = MainClient.ServerStatus;
        private BinaryReader myStreamToRead;
        private BinaryWriter myStreamToWrite;
        public int kickTargetTimestamp = 0;

        public bool connectionAlive { get { if (Tcp.Connected && UserState != _ClientState.Connected && ServState != ServChecker.IsConnecting) return true; else return false; } }

        public Connection(TcpClient NewTcp, AttemptingServer ThisStarter, NetworkStream thisNewStream)
        {
            Tcp = NewTcp;
            ClientThis = ThisStarter;
            StreamOfClient = thisNewStream;
            UserState = _ClientState.Connected;
            ServState = ServChecker.IsConnecting;
            MyInfo = new UserActiveInfo(gl_s_OSdateCrypt, gl_s_userName);
            ThreadName.Current("Поток первого подключения");

        }

        // Первое подключение к серверу
        public void Start()
        {
            try
            {
                myStreamToRead = new BinaryReader(Tcp.GetStream());
                myStreamToWrite = new BinaryWriter(Tcp.GetStream());
                MainClient.logInfo("Установлена связь с сервером.");
                string message = "h@@lLloui-" + gl_s_userName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                StreamOfClient.Write(data, 0, data.Length);
                new Thread(new ThreadStart(new RearwardThread(this, myStreamToRead, myStreamToWrite, Direction.Server).Run)).Start();
            }
            catch (Exception e)
            {
                if (ServState == ServChecker.IsConnecting)
                {
                    Disconnect();
                    ServState = ServChecker.DisconnectingMe;
                    UserState = _ClientState.Disconnecting;
                    DoDisconnect(true);
                }
                MainClient.logException("ClientThread Exception: " + e.Message);
            }  
            finally
            {
                if (ServState == ServChecker.IsConnecting)
                {
                    Disconnect();
                    ServState = ServChecker.DisconnectingMe;
                    UserState = _ClientState.Disconnecting;
                    DoDisconnect(true);
                }
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
            if (UserState != _ClientState.Disconnecting)
            {
                UserState = _ClientState.Disconnecting;
                try
                {
                    Tcp.Close();
                }
                catch (Exception) { }
            }
        }
        
        public void ForceDisconnect(string reason)
        {
            if (UserState != _ClientState.Disconnecting)
                MainClient.logError("[" + MyInfo.client + "] Dropped for " + reason);
            DoDisconnect(true);
        }

        public void ErrorDisconnect(Direction direction, string reason)
        {
            if (UserState != _ClientState.Crashing)
                MainClient.logError("[" + MyInfo.client + "] Dropped by parent " + direction.ToString() + " for " + reason);
            DoDisconnect(true);
        }


        public void forceDisconnect()
        {
            DoDisconnect(false);
        }

                // отключение
        public static void Disconnect()
        {
            if (MainClient.ServerStatus == ServChecker.IsConnecting)
            {
                MainClient.ServerStatus = ServChecker.DisconnectingMe;
            }
        }
    }
}

