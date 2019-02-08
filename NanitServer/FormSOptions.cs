using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;


namespace NaNiT
{
    public partial class FormSOptions : Form
    {
        bool tempSwitch = true;
        public static ServerObject server; // сервер
        public static Thread listenThread; // потока для прослушивания
        int state1 = 10;
        int state2 = 20;

        public FormSOptions()
        {
            try
            {
                InitializeComponent();
                Globals.form1 = this;
                ControlBoxPortServ.Text = Globals.servPort.ToString();
                backgroundWorker1.WorkerReportsProgress = true;
                backgroundWorker1.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("FormsOptions(init) " + ex.Message);
            }
        }

        public static void Start()
        {
            try
            {
                Globals.disconnectInProgress = false;
                server = new ServerObject();
                listenThread = new Thread(new ThreadStart(server.Listen));
                if (listenThread.Name == null)
                    listenThread.Name = "ServerListen (Start sub-main)";
                listenThread.Start(); //старт потока
                Globals.form1.ButStart.Text = "Остановить";
            }
            catch (Exception ex)
            {
                server.Disconnect();
                MessageBox.Show("FormsOptions(start) " + ex.Message);
            }
        }

        public void Stop()
        {
            server.Disconnect();
            ButStart.Text = "Запустить";
        }

        private void ButStart_Click(object sender, EventArgs e)
        {
            if (listenThread == null)
                Start();
            else
                Stop();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                listView1.Items.Add(new ListViewItem(Globals.MessageText));
                if (listView1.Items.Count > 1)
                    listView1.EnsureVisible(listView1.Items.Count - 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("FormsOptions(bwc_change) " + ex.Message);
            }
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (Globals.isOptOpenStatic)
            {
                if (Globals.MessageIn != Globals.MessageInOld)
                {
                    Globals.MessageInOld = Globals.MessageIn;
                    if (tempSwitch)
                        backgroundWorker1.ReportProgress(state1);
                    else
                        backgroundWorker1.ReportProgress(state2);
                    SFunctions.Revers(tempSwitch);
                }
                Thread BackWork = Thread.CurrentThread;
                if (BackWork.Name == null)
                    BackWork.Name = "BackWorker Options";
            }
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                MessageBox.Show("FormsOptions(bwc_canc)");
            }
            else if (e.Error != null)
            {
                MessageBox.Show("FormsOptions(bwc_comp) " + e.Error.Message);
            }
            else
            {
                //if (Globals.isOptOpen)
                MessageBox.Show("FormsOptions(bwc_??)");
            }
        }

        public void FormOptions_Close(object sender, EventArgs e)
        {
            Globals.isOptOpen = false;
            Globals.isAboutLoaded = false;
        }

        private void FormSOptions_Deactivate(object sender, EventArgs e)
        {
            TopMost = true;
        }

        private void ButOptSave_Click(object sender, EventArgs e)
        {
            try
            {
                Globals.servPort = Convert.ToInt32(ControlBoxPortServ.Text);
                RegistryKey localMachineKey = Registry.LocalMachine;
                RegistryKey localMachineSoftKey = localMachineKey.OpenSubKey("SOFTWARE", true);
                RegistryKey regNanit = localMachineSoftKey.CreateSubKey(@"N.A.N.I.T");
                RegistryKey servKey = regNanit.CreateSubKey("Update");
                servKey.SetValue("port_server", Globals.servPort);
                servKey.Close();
                servKey = null;
                regNanit.Close();
                regNanit = null;
                localMachineSoftKey.Close();
                localMachineSoftKey = null;
                localMachineKey.Close();
                localMachineKey = null;
                Globals.isOptOpen = false;
                Globals.isAboutLoaded = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("FormsOptions(save_reg) " + ex.Message);
            }
            Stop();
            Start();
        }

        private void ButOptClose_Click(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show("FormsOptions(close_click) " + ex.Message);
            }
        }


    }
}