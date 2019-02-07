using System;
using System.Net.Sockets;
using System.Text;

namespace NaNiT
{
    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        protected internal int AwaitVarForCom;
        protected internal bool IsRegister, IsActive, StupidCheck, myMessageNotAwait, CloseMePliz;
        protected internal string userName;
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
                }
            }
            catch
            {
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
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
                        myMessageNotAwait = true;
                        if (bytes == 0)
                            return null;
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (Stream.DataAvailable);
                    ServerCommands.CheckCommand(builder.ToString(), this, server, Stream);
                    return builder.ToString();
                }
                catch
                {
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
            if (!Globals.disconnectInProgress)
            {
                dateLastSeen = DateTime.Now.ToString();
                server.RemoveConnection(Id, userName, Stream, client);
            }
        }
    }
}