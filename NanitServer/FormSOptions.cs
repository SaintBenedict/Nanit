using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace NaNiT
{
    public partial class FormSOptions : Form
    {
        Socket listenSocket, handler, handler2;

        public FormSOptions()
        {
            InitializeComponent();
            Globals.form1 = this;
            Globals.isOptOpen = true;
            ControlBoxPortServ.Text = Globals.servPort.ToString();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.RunWorkerAsync();
        }

        private void ButStart_Click(object sender, EventArgs e)
        {
            // получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, Globals.servPort);
            // создаем сокет
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // связываем сокет с локальной точкой
            listenSocket.Bind(ipPoint);
            // длина очереди
            listenSocket.Listen(10);
            textBox1.Text += "Сервер создан. Ожидание подключений." + Environment.NewLine;

            handler = listenSocket.Accept();

            textBox1.Text += "Клиент 1 подключился" + Environment.NewLine;
            bool GetMessage = false;
            // получаем сообщение
            string s = "";
            int bytes = 0; // количество полученных байтов
            byte[] data = new byte[256]; // буфер для получаемых данных
            while (GetMessage == false)
            {
                if (handler.Available > 0)
                {
                    GetMessage = true;
                    while (handler.Available > 0)
                    {
                        bytes = handler.Receive(data);
                        s += Encoding.Unicode.GetString(data, 0, bytes);
                    }
                }
                Thread.Sleep(300);
            }
            textBox1.Text += "Сообщение от клиента 1: " + s + Environment.NewLine;
            // отправляем ответ, первому клиенту мы отправляем только его порт
            IPEndPoint ep = (IPEndPoint)handler.RemoteEndPoint;
            string message = ep.Port.ToString();
            string message_to_client2 = ep.Address.ToString() + ":" + ep.Port.ToString();
            data = Encoding.Unicode.GetBytes(message);
            handler.Send(data);


            handler2 = listenSocket.Accept();

            textBox1.Text += "Клиент 2 подключился" + Environment.NewLine;
            GetMessage = false;
            // получаем сообщение 2 клиента
            s = "";
            bytes = 0; // количество полученных байтов
            while (GetMessage == false)
            {
                if (handler2.Available > 0)
                {
                    GetMessage = true;
                    while (handler2.Available > 0)
                    {
                        bytes = handler2.Receive(data);
                        s += Encoding.Unicode.GetString(data, 0, bytes);
                    }
                }
                Thread.Sleep(300);
            }
            textBox1.Text += "Сообщение от клиента 2: " + s + Environment.NewLine;
            // отправляем ответ, второму клиенту мы отправляем адрес и порт первого клиента
            data = Encoding.Unicode.GetBytes(message_to_client2);
            handler2.Send(data);
        }

        public void FormOptions_Close(object sender, EventArgs e)
        {
            Globals.isOptOpen = false;
            Globals.isAboutLoaded = false;
        }

        private void ButOptSave_Click(object sender, EventArgs e)
        {
            Globals.servPort = Convert.ToInt32(ControlBoxPortServ.Text); ;

            RegistryKey localMachineKey = Registry.LocalMachine;
            RegistryKey localMachineSoftKey = localMachineKey.OpenSubKey("SOFTWARE", true);
            RegistryKey regNanit = localMachineSoftKey.CreateSubKey(@"N.A.N.I.T");
            RegistryKey servKey = regNanit.CreateSubKey("Update");
            servKey.SetValue("port_server", Globals.servPort);
            Globals.isOptOpen = false;
            Globals.isAboutLoaded = false;
        }

        private void ButOptClose_Click(object sender, EventArgs e)
        {
            if (Globals.servPort != Convert.ToInt32(ControlBoxPortServ.Text))
            {
                const string message = "Закрыть настройки? Все несохранённые изменения будут потеряны";
                const string caption = "";
                var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    this.Close();
                    Globals.isAboutLoaded = false;
                    Globals.isOptOpen = false;
                }
            }
            else
            {
                this.Close();
                Globals.isAboutLoaded = false;
                Globals.isOptOpen = false;
            }
        }

        
    }
}