using NaNiT.Functions;
using System;
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
            if (MainProgram.CurrentServerStatus != ServerState.Running)
            {
                MainProgram.StartServer();
                ButStart.Text = "Остановить";
            }
            else
            {
                MainProgram.StopServer();
                ButStart.Text = "Запустить";
            }
        }

        //private void ListWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
        //    try
        //    {
        //        if (ServerApplication.LogMessageList[0] != null)
        //        {
        //            listView1.Items.Add(new ListViewItem(ServerApplication.LogMessageList[0]));
        //            ServerApplication.LogMessageList.RemoveAt(0);
        //            if (listView1.Items.Count > 1)
        //                listView1.EnsureVisible(listView1.Items.Count - 1);
        //            if (ServerApplication.ListenThread.ThreadState != 0)
        //                ButStart.Text = "Запустить";
        //            else
        //                ButStart.Text = "Остановить";
        //        }
        //    }
        //    catch (Exception ex) { Error.Msg("EP1Fm0.1", ex.ToString()); }
        //}

        //private void ListWorker_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    bool SwitchWork = true;
        //    while (true)
        //    {
        //        if (ServerApplication.LogMessageList.Count > 0)
        //        {
        //            if (SwitchWork)
        //                ListWorker.ReportProgress(10);
        //            else
        //                ListWorker.ReportProgress(20);
        //            SwitchWork = Revers(SwitchWork);
        //            Thread.Sleep(200);
        //        }
        //        else
        //        {
        //            Thread.Sleep(1000);
        //        }
        //        if (!ServerApplication.ServerFormIsOpen)
        //            Thread.Sleep(10000);
        //    }

        //    bool Revers(bool first)
        //    {
        //        if (first == true)
        //            return false;
        //        else
        //            return true;
        //    }
        //}

        //private void ButOptSave_Click(object sender, EventArgs e)
        //{
        //    RegistryWork servReg = new RegistryWork("Server");
        //    ServerApplication.ServerConnectionPort = Convert.ToInt32(ControlBoxPortServ.Text);
        //    servReg.Write("port_server", ServerApplication.ServerConnectionPort);
        //    servReg.Exit();
        //    ServerApplication.RestartServer();
        //}

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

        //private void ListWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    Error.Msg("EP1Fm0.1", e.ToString());
        //}
    }
}