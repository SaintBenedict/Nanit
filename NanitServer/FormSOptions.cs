﻿using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;


namespace NaNiT
{
    public partial class FormSOptions : Form
    {
        bool tempSwitch = true;
        static ServerObject server; // сервер
        static Thread listenThread; // потока для прослушивания
        int state1 = 10;
        int state2 = 20;

        public FormSOptions()
        {
            InitializeComponent();
            Globals.form1 = this;
            ControlBoxPortServ.Text = Globals.servPort.ToString();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.RunWorkerAsync();
            Start();
            if (listenThread.ThreadState == 0)
                ButStart.Text = "Остановить";
            else
            {
                ButStart.Text = "Запустить";
                Start();
            }
        }

        private void Start()
        {
            try
            {
                Globals.disconnectInProgress = false;
                server = new ServerObject();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Name = "ServerListen (Start sub-main)";
                listenThread.Start(); //старт потока
            }
            catch (Exception ex)
            {
                server.Disconnect();
                MessageBox.Show(ex.Message);
            }
        }
        public void Stop()
        {
            server.Disconnect();
        }

        private void ButStart_Click(object sender, EventArgs e)
        {
            if (listenThread.ThreadState == 0)
            {
                Stop();
                ButStart.Text = "Запустить";
            }
            else
            {
                Start();
                ButStart.Text = "Остановить";
            }
        }

        private void RevealMoreItems(object sender, DragEventArgs e)
        {
            var listView = (ListView)sender;

            var point = listView.PointToClient(new Point(e.X, e.Y));
            var item = listView.GetItemAt(point.X, point.Y);
            if (item == null)
                return;

            var index = item.Index;
            var maxIndex = listView.Items.Count;
            var scrollZoneHeight = listView.Font.Height;

            if (index > 0 && point.Y < scrollZoneHeight)
            {
                listView.Items[index - 1].EnsureVisible();
            }
            else if (index < maxIndex && point.Y > listView.Height - scrollZoneHeight)
            {
                listView.Items[index + 1].EnsureVisible();
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            listView1.Items.Add(new ListViewItem(Globals.MessageText));
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (Globals.isOptOpenStatic)
            {
                if (Globals.MessageIn != Globals.MessageInOld)
                {
                    if (tempSwitch)
                        backgroundWorker1.ReportProgress(state1);
                    else
                        backgroundWorker1.ReportProgress(state2);
                    Globals.MessageInOld = Globals.MessageIn;
                    SFunctions.Revers(tempSwitch);
                }
            }
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                MessageBox.Show("Canceled!");
            }
            else if (e.Error != null)
            {
                MessageBox.Show("Error: " + e.Error.Message);
            }
            else
            {
                //if (Globals.isOptOpen)
                MessageBox.Show("Что-то пошло не так. Я закончил работать");
            }
        }

        public void FormOptions_Close(object sender, EventArgs e)
        {
            Globals.isOptOpen = false;
            Globals.isAboutLoaded = false;
        }

        private void FormSOptions_Deactivate(object sender, EventArgs e)
        {
            this.TopMost = true;
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
                    this.Hide();
                    Globals.isAboutLoaded = false;
                    Globals.isOptOpen = false;
                }
            }
            else
            {
                this.Hide();
                Globals.isAboutLoaded = false;
                Globals.isOptOpen = false;
            }
        }


    }
}