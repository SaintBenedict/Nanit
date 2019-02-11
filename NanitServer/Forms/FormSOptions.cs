using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using static NaNiT.GlobalVariable;
using static NaNiT.GlobalFunctions;


namespace NaNiT
{
    public partial class FormSOptions : Form
    {
        public static ServerObject server; // сервер
        public static Thread listenThread; // потока для прослушивания
        int state1 = 10;
        int state2 = 20;

        public FormSOptions()
        {
            try
            {
                InitializeComponent();
                gl_f_optionsServ = this;
                ControlBoxPortServ.Text = gl_i_servPort.ToString();
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
                gl_b_disconnectInProgress = false;
                server = null;
                server = new ServerObject();
                listenThread = new Thread(new ThreadStart(server.Listen));
                if (listenThread.Name == null)
                    listenThread.Name = "ServerListen (Start sub-main)";
                listenThread.Start(); //старт потока
                gl_f_optionsServ.ButStart.Text = "Остановить";
            }
            catch (Exception ex)
            {
                if (!gl_b_disconnectInProgress)
                    server.Disconnect();
                MessageBox.Show("FormsOptions(start) " + ex.Message);
            }
        }

        public void Stop()
        {
            if (!gl_b_disconnectInProgress)
                server.Disconnect();
            ButStart.Text = "Запустить";
        }

        private void ButStart_Click(object sender, EventArgs e)
        {
            if (listenThread.ThreadState != 0)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                listView1.Items.Add(new ListViewItem(gl_sList_Messages[0]));
                gl_sList_Messages.RemoveAt(0);
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
            while (gl_b_isOptOpenStatic)
            {
                if (gl_i_MessageIn != gl_i_MessageInOld)
                {
                    gl_i_MessageInOld = gl_i_MessageIn;
                    if (gl_b_tempSwitch)
                        backgroundWorker1.ReportProgress(state1);
                    else
                        backgroundWorker1.ReportProgress(state2);
                    gl_b_tempSwitch = Revers(gl_b_tempSwitch);
                    Thread BackWork = Thread.CurrentThread;
                    if (BackWork.Name == null)
                        BackWork.Name = "BackWorker Options";
                }
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
                //if (gl_b_isOptOpen)
                MessageBox.Show("FormsOptions(bwc_??)");
            }
        }

        public void FormOptions_Close(object sender, EventArgs e)
        {
            gl_b_isOptOpen = false;
            gl_b_isAboutLoaded = false;
        }

        private void FormSOptions_Deactivate(object sender, EventArgs e)
        {
            TopMost = true;
        }

        private void ButOptSave_Click(object sender, EventArgs e)
        {
            try
            {
                gl_i_servPort = Convert.ToInt32(ControlBoxPortServ.Text);
                RegistryKey localMachineKey = Registry.LocalMachine;
                RegistryKey localMachineSoftKey = localMachineKey.OpenSubKey("SOFTWARE", true);
                RegistryKey regNanit = localMachineSoftKey.CreateSubKey(@"N.A.N.I.T");
                RegistryKey servKey = regNanit.CreateSubKey("Update");
                servKey.SetValue("port_server", gl_i_servPort);
                servKey.Close();
                servKey = null;
                regNanit.Close();
                regNanit = null;
                localMachineSoftKey.Close();
                localMachineSoftKey = null;
                localMachineKey.Close();
                localMachineKey = null;
                gl_b_isOptOpen = false;
                gl_b_isAboutLoaded = false;
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
                if (gl_i_servPort != Convert.ToInt32(ControlBoxPortServ.Text))
                {
                    const string message = "Закрыть настройки? Все несохранённые изменения будут потеряны";
                    const string caption = "";
                    var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                    if (result == DialogResult.Yes)
                    {
                        this.Hide();
                        gl_b_isAboutLoaded = false;
                        gl_b_isOptOpen = false;
                    }
                }
                else
                {
                    this.Hide();
                    gl_b_isAboutLoaded = false;
                    gl_b_isOptOpen = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FormsOptions(close_click) " + ex.Message);
            }
        }


    }
}