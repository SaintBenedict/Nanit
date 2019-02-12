using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static NaNiT.GlobalVariable;
using static NaNiT.LocalGlobals;

namespace NaNiT
{
    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        protected internal int AwaitVarForCom;
        protected internal bool IsRegister, IsActive, StupidCheck, myMessageNotAwait;
        protected internal string cryptoLogin, userName;
        protected internal string dateOfRegister;
        protected internal string dateLastSeen;
        protected internal string IP;
        protected internal TcpClient client;
        protected internal Thread ClientThreadThis;
        protected internal int idInDatabase;
        internal MyXml softXmlBase = null;
        ServerObject server;

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
            IP = Convert.ToString(((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Address);
        }

        public void Process()
        {
            try // в бесконечном цикле получаем сообщения от клиента
            {
                AwaitVarForCom = 0;
                Stream = client.GetStream();
                while (client != null && !gl_b_disconnectInProgress && IsActive)
                {
                    string MessageTestNull = GetMessage();
                    if (MessageTestNull == null)
                    {
                            Close();
                            return;
                    }
                    Thread ClientThreadThis = Thread.CurrentThread;
                    if (ClientThreadThis.Name == null)
                        ClientThreadThis.Name = "Client " + userName + "Proc";
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
                    Close();
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
                    ServerCommands.CheckCommand(builder.ToString(), this, server, Stream);
                    return builder.ToString();
                }
                catch (Exception ex)
                {
                    if (!gl_b_disconnectInProgress && IsActive)
                    {
                        Close();
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
        protected internal void Close()
        {
            if (IsActive)
            {
                IsActive = false;
                try
                {
                    dateLastSeen = DateTime.Now.ToString();
                    if (Stream != null) { Stream.Close(); Stream = null; }
                    if (client != null) { client.Close(); client = null; }
                    if (!gl_b_disconnectInProgress && !StupidCheck)
                    {
                        gl_sList_Messages.Add(userName + " отключился [" + dateLastSeen + "]");
                        server.RemoveConnection(this.Id);
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