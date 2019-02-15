using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static NaNiT.GlobalVariable;
using static NaNiT.LocalGlobals;

namespace NaNiT
{
    public class ClientObject : _ClientValues, IServer
    {
        internal ServerObject Server { get; set; }
        internal XmlSoft ClientXSofts { get; set; }
        public TcpClient ThisTcpClient { get; set; }
        protected internal NetworkStream Stream { get; private set; }


        protected internal int AwaitVarForCom;
        protected internal bool IsRegister, IsActive, StupidCheck, myMessageNotAwait;
        protected internal Thread ClientThreadThis;
        
        

        public ClientObject(TcpClient tcpClient, ServerObject serverObject) :base(tcpClient, serverObject)
        {
            ThisTcpClient = tcpClient;
            Server = serverObject;
            serverObject.AddConnection(this);
            UserIpAdress = Convert.ToString(((System.Net.IPEndPoint)ThisTcpClient.Client.RemoteEndPoint).Address);
        }

        public void Process()
        {
            try // в бесконечном цикле получаем сообщения от клиента
            {
                AwaitVarForCom = 0;
                Stream = ThisTcpClient.GetStream();
                while (ThisTcpClient != null && !gl_b_disconnectInProgress && IsActive)
                {
                    string MessageTestNull = GetMessage();
                    if (MessageTestNull == null)
                    {
                        Disconnect();
                        return;
                    }
                    Thread ClientThreadThis = Thread.CurrentThread;
                    if (ClientThreadThis.Name == null)
                        ClientThreadThis.Name = "Client " + UserHostName + "Proc";
                }
            }
            catch (Exception ex)
            {
                if (!gl_b_disconnectInProgress && IsActive)
                {
                    MessageBox.Show("ClientObject(proc) " + ex.Message);
                }
            }
            finally
            {
                if (!gl_b_disconnectInProgress && IsActive)
                {
                    Disconnect();
                }
            }
        }

        // чтение входящего сообщения и преобразование в строку
        private string GetMessage()
        {
            if (Stream.CanRead)
            {
                try
                {
                    byte[] data = new byte[64]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        if (gl_b_disconnectInProgress || !IsActive)
                        {
                            return null;
                        }
                        myMessageNotAwait = true;
                        bytes = Stream.Read(data, 0, data.Length);
                        if (bytes == 0)
                            return null;
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (Stream.DataAvailable && IsActive);
                    ServerCommands.CheckCommand(builder.ToString(), this, Server, Stream);
                    return builder.ToString();
                }
                catch
                {
                    if (!gl_b_disconnectInProgress && IsActive)
                    {
                        Disconnect();
                    }
                    return null;
                }
            }
            else
            {
                return null;
            }

        }


        // закрытие подключения
        public void Disconnect()
        {
            if (IsActive)
            {
                IsActive = false;
                try
                {
                    DateLastOnline = DateTime.Now.ToString();
                    if (Stream != null) { Stream.Close(); Stream = null; }
                    if (ThisTcpClient != null) { ThisTcpClient.Close(); ThisTcpClient = null; }
                    if (!gl_b_disconnectInProgress && !StupidCheck)
                    {
                        gl_sList_Messages.Add(UserHostName + " отключился [" + DateLastOnline + "]");
                        Server.RemoveConnection(this.Guid_id);
                    }
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                catch (Exception ex)
                {
                    if (!StupidCheck)
                        MessageBox.Show("ClientObject(close) " + ex.Message);
                }
            }
        }
    }
}