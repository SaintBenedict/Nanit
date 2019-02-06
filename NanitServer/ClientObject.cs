using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace NaNiT
{
    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        protected internal int AwaitVarForCom;
        protected internal bool IsRegister;
        protected internal string userName;
        protected internal string dateOfRegister;
        protected internal string dateLastSeen;
        TcpClient client;
        ServerObject server; // объект сервера

        

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
            AwaitVarForCom = 0;
            IsRegister = false;
            dateOfRegister = null;
            dateLastSeen = null;
        }

        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                // получаем имя пользователя
                string message = GetMessage();
                userName = message;

                message = userName + " подключился";
                Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn, message);
                // посылаем сообщение о входе в чат всем подключенным пользователям
                if (!this.IsRegister)
                    server.BroadcastMessage("@HowdyHu%$-", this.Id, ServerObject.clients, this, 0);
                // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    message = GetMessage();
                    ServerCommands.CheckCommand(message, this, server, Stream);
                }
            }
            catch
            {
                Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn, userName + " отключён");
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(this.Id);
                Stream.Close();
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
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (Stream.DataAvailable);

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
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
}