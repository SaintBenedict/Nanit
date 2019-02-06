using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NaNiT
{
    class CFunc
    {
        static string userName = Globals.userName;
        static private string host = Globals.servIP;
        static private int port = Globals.servPort;

        //
        //Подключение
        public static void Chat()
        {
            Program.client = new TcpClient();
            try
            {
                Program.client.Connect(host, port); //подключение клиента
                Program.stream = Program.client.GetStream(); // получаем поток
                string message = userName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                Program.stream.Write(data, 0, data.Length);
                // запускаем новый поток для получения данных
                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start(); //старт потока
                Globals.serverIsConnected = true;
                Globals.TimConnLock = 0;
            }
            catch
            {
                Program.notifyIcon.BalloonTipText = "Сервер недоступен";                // пришедшее сообщение от сервера
                Program.notifyIcon.ShowBalloonTip(6);                                  // отображаем подсказку 12 секунд
                Globals.serverIsConnected = false;
                Globals.TimConnLock++;
                Program.TempServConnect(Globals.TimConnLock);
                return;
            }
        }

        //
        // отправка сообщений
        public static void SendMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            Program.stream.Write(data, 0, data.Length);

        }

        //
        // получение сообщений
        public static void ReceiveMessage()
        {
            while (Globals.serverIsConnected)
            {
                if (Program.stream.CanRead)
                {
                    try
                    {
                        byte[] data = new byte[64]; // буфер для получаемых данных
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;
                        do
                        {

                            bytes = Program.stream.Read(data, 0, data.Length);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));

                        }
                        while (Program.stream.DataAvailable);
                        string message = builder.ToString();
                        Program.notifyIcon.BalloonTipText = message;
                        Program.notifyIcon.ShowBalloonTip(6);
                    }
                    catch
                    {
                        Program.notifyIcon.BalloonTipText = "Сервер стал недоступен";
                        Program.notifyIcon.ShowBalloonTip(6);
                        Globals.serverIsConnected = false;
                        Disconnect();
                        Globals.TimConnLock++;
                        Program.TempServConnect(Globals.TimConnLock);
                    }
                }
                else
                {
                    Program.notifyIcon.BalloonTipText = "Сервер стал недоступен";
                    Program.notifyIcon.ShowBalloonTip(6);
                    Globals.serverIsConnected = false;
                    Disconnect();
                    Globals.TimConnLock++;
                    Program.TempServConnect(Globals.TimConnLock);
                }
            }
        }

        //
        // отключение
        public static void Disconnect()
        {
            if (Program.stream != null)
                Program.stream.Close(); //отключение потока
            if (Program.client != null)
                Program.client.Close(); //отключение клиента
            Globals.serverIsConnected = false;
        }
    }
}

