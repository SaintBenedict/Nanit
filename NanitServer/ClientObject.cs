using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace NaNiT
{
    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        protected internal int AwaitVarForCom;
        protected internal bool IsRegister, IsActive, StupidCheck, myMessageNotAwait, CloseMePliz;
        protected internal string cryptoLogin, userName;
        protected internal string dateOfRegister;
        protected internal string dateLastSeen;
        protected internal TcpClient client;
        ServerObject server;

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
            IsRegister = false;
            dateOfRegister = null;
            dateLastSeen = null;
            IsActive = true;
            StupidCheck = false;
        }

        public void Process()
        {
            try // в бесконечном цикле получаем сообщения от клиента
            {
                AwaitVarForCom = 0;
                Stream = client.GetStream();
                while (this.client != null)
                {
                    string MessageTestNull = GetMessage();
                    if (MessageTestNull == null)
                    {
                        Close();
                        return;
                    }
                    Thread ClientThread = Thread.CurrentThread;
                    if (ClientThread.Name == null)
                        ClientThread.Name = "Client " + userName + "Proc";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ClientObject(proc) " + ex.Message);
            }
            finally
            {
                Close();
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
                        bytes = Stream.Read(data, 0, data.Length);
                        if (CloseMePliz)
                        {
                            Close();
                            break;
                        }
                        myMessageNotAwait = true;
                        if (bytes == 0)
                            return null;
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (Stream.DataAvailable);
                    ServerCommands.CheckCommand(builder.ToString(), this, server, Stream);
                    return builder.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ClientObject(get_msg) " + ex.Message);
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
            try
            {
                if (!Globals.disconnectInProgress)
                {
                    dateLastSeen = DateTime.Now.ToString();
                    server.RemoveConnection(Id, cryptoLogin, Stream, client);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ClientObject(close) " + ex.Message);
            }
        }
    }
}