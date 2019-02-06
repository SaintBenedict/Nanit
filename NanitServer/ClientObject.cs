using System;
using System.Net.Sockets;
using System.Text;

namespace NaNiT
{
    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        string userName;
        TcpClient client;
        ServerObject server; // объект сервера

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
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
                Globals.MessageText = message;
                Globals.MessageIn = 2;
                Globals.ClientId = this.Id;
                // посылаем сообщение о входе в чат всем подключенным пользователям
                server.BroadcastMessage(message, this.Id);
                // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    message = GetMessage();
                    if (String.Format("{0}: {1}", userName, message) != String.Format("{0}: {1}", userName, null))
                    {
                        
                        message = String.Format("{0}: {1}", userName, message);
                        Globals.MessageText = message;
                        Globals.MessageIn++;
                        server.BroadcastMessage(message, this.Id);
                    }
                    else 
                    {
                        Stream.Close();
                        message = String.Format("{0}: покинул чат", userName);
                        Globals.MessageText = message;
                        Globals.MessageIn++;
                        message = null;
                        if (message != null)
                        server.BroadcastMessage(message, this.Id);
                        else
                        break;

                    }
                }
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