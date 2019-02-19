using NaNiT.Functions;
using NaNiT.Utils;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;


namespace NaNiT
{
    public partial class FormSOptions : Form
    {
        public FormSOptions()
        {
            InitializeComponent();
            this.ControlBoxPortServ.Text = MainProgram.ServerConfig.serverPort.ToString();
            MainProgram.ServerForm = this;
        }

        private void ButStart_Click(object sender, EventArgs e)
        {
            if (MainProgram.CurrentServerStatus == ServerState.Running)
            {
                MainProgram.StopServer();
                ButStart.Text = "Запустить";
            }
            else
            {
                MainProgram.StartServer();
                ButStart.Text = "Остановить";
            }
        }

        private void ListWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (MainProgram.LogMessageList[0] != null)
                {
                    listView1.Items.Add(new ListViewItem(MainProgram.LogMessageList[0]));
                    MainProgram.LogMessageList.RemoveAt(0);
                    if (listView1.Items.Count > 1)
                        listView1.EnsureVisible(listView1.Items.Count - 1);
                    if (MainProgram.CurrentServerStatus == ServerState.Running)
                        ButStart.Text = "Остановить";
                    else
                        ButStart.Text = "Запустить";
                }
            }
            catch (Exception ex) { Error.Msg("EP1Fm0.1", ex.ToString()); }
        }

        private void ListWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            bool SwitchWork = true;
            while (true)
            {
                if (MainProgram.LogMessageList.Count > 0)
                {
                    if (SwitchWork)
                        ListWorker.ReportProgress(10);
                    else
                        ListWorker.ReportProgress(20);
                    SwitchWork = Revers(SwitchWork);
                    Thread.Sleep(200);
                }
                else
                {
                    Thread.Sleep(1000);
                }
                if (!MainProgram.ServerConfig.ServerFormIsOpen)
                    Thread.Sleep(10000);
            }

            bool Revers(bool first)
            {
                if (first == true)
                    return false;
                else
                    return true;
            }
        }

        private void ButOptSave_Click(object sender, EventArgs e)
        {
            RegistryWork servReg = new RegistryWork("Server");
            MainProgram.ServerConfig.serverPort = Convert.ToInt32(ControlBoxPortServ.Text);
            servReg.Write("port_server", MainProgram.ServerConfig.serverPort);
            servReg.Exit();
            MainProgram.RestartingServer();
        }

        private void ButOptClose_Click(object sender, EventArgs e)
        {
            if (MainProgram.ServerConfig.serverPort != Convert.ToInt32(ControlBoxPortServ.Text))
            {
                const string message = "Закрыть настройки? Все несохранённые изменения будут потеряны";
                const string caption = "";
                var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Yes)
                    CloseForm();
            }
            else
                CloseForm();
        }

        public void CloseForm(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void CloseForm()
        {
            Hide();
            MainProgram.ServerConfig.TrayMenuIsOpen = false;
            MainProgram.ServerConfig.ServerFormIsOpen = false;
        }

        private void ListWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MainProgram.logException("Фоновый работник формы внезапно прекратил работу: " + e.ToString());
        }
    }
}