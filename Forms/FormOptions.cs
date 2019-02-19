using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using static NaNiT.GlobalVariable;
using static NaNiT.GlobalFunctions;
using static NaNiT.LocalGlobals;


namespace NaNiT
{
    public partial class FormOptions : Form
    {
        static object locker = new object();
        static bool workerByte = true;

        public FormOptions()
        {
            InitializeComponent();
            gl_f_options = this;
            gl_b_isOptOpen = true;
            ControlBoxIpServ.Text = gl_s_servIP;
            ControlBoxPortServ.Text = gl_i_servPort.ToString();
            CheckRoleAdmin.Checked = gl_b_roleAdmin;
            CheckRoleOperate.Checked = gl_b_roleOperate;
            CheckRoleMessager.Checked = gl_b_roleMessager;
            CheckRoleSecurity.Checked = gl_b_roleSecurity;
            CheckRoleAgent.Checked = gl_b_roleAgent;
            LabelServiceInstall.Text = "Загрузка...";
            LabelServiceInstall.ForeColor = System.Drawing.Color.Black;
            LabelServiceStart.Text = "Загрузка...";
            LabelServiceStart.ForeColor = System.Drawing.Color.Black;
            ButServiceInstall.Enabled = false;
            button1.Visible = false;
            button2.Visible = false;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.RunWorkerAsync();
            this.ClientSize = new System.Drawing.Size(485, 287);
            if (gl_b_debug)
            {
                button1.Visible = true;
                button2.Visible = true;
                this.ClientSize = new System.Drawing.Size(485, 319);
            }
            ServiceInit();
            ServiceWork.ServiceInit();
        }

        public void FormOptions_Close(object sender, EventArgs e)
        {
            gl_b_isOptOpen = false;
            gl_b_isAboutLoaded = false;
        }
        
        private void FormOptions_Deactivate(object sender, EventArgs e)
        {
            if (!gl_b_isUpdOpen)
                this.TopMost = true;
            else
                this.TopMost = false;
        }

        private void ButOptSave_Click(object sender, EventArgs e)
        {
            if (gl_s_servIP != ControlBoxIpServ.Text)
            {
                gl_s_servIP = ControlBoxIpServ.Text;
                gl_s_serverStatus = "Сервер стал недоступен";
                ClientProgram.TrayNotify.Icon = Resources.net2;
                gl_b_serverIsConnected = false;
            }
            if (gl_i_servPort != Convert.ToInt32(ControlBoxPortServ.Text))
            {
                gl_i_servPort = Convert.ToInt32(ControlBoxPortServ.Text);
            }
            
            gl_b_roleAdmin = CheckRoleAdmin.Checked;
            gl_b_roleOperate = CheckRoleOperate.Checked;
            gl_b_roleMessager = CheckRoleMessager.Checked;
            gl_b_roleSecurity = CheckRoleSecurity.Checked;
            gl_b_roleAgent = CheckRoleAgent.Checked;
            gl_s_md5PortIp = MD5Code(gl_i_servPort.ToString() + gl_s_servIP + gl_s_OSdate);
            gl_s_md5Clients = MD5Code(gl_s_OSdate + gl_b_roleSecurity.ToString().ToLower() + gl_b_roleMessager.ToString().ToLower() + gl_b_roleOperate.ToString().ToLower() + gl_b_roleAdmin.ToString().ToLower() + gl_b_roleAgent.ToString().ToLower());

            RegistryKey localMachineKey = Registry.LocalMachine;
            RegistryKey localMachineSoftKey = localMachineKey.OpenSubKey("SOFTWARE", true);
            RegistryKey regNanit = localMachineSoftKey.CreateSubKey(@"N.A.N.I.T");
            regNanit.SetValue("ip_server", gl_s_servIP);
            regNanit.SetValue("port_server", gl_i_servPort.ToString());
            regNanit.SetValue("validate_ip_port", gl_s_md5PortIp);
            regNanit.SetValue("RoleSecurity", gl_b_roleSecurity.ToString().ToLower());
            regNanit.SetValue("RoleMessager", gl_b_roleMessager.ToString().ToLower());
            regNanit.SetValue("RoleOperate", gl_b_roleOperate.ToString().ToLower());
            regNanit.SetValue("RoleAdmin", gl_b_roleAdmin.ToString().ToLower());
            regNanit.SetValue("RoleAgent", gl_b_roleAgent.ToString().ToLower());
            regNanit.SetValue("validate_clients", gl_s_md5Clients);
            regNanit.Close();

            this.Close();
            gl_b_isOptOpen = false;
            gl_b_isAboutLoaded = false;
        }

        private void ButOptClose_Click(object sender, EventArgs e)
        {
            if (gl_s_servIP != ControlBoxIpServ.Text)
            {
                const string message = "Закрыть настройки? Все несохранённые изменения будут потеряны";
                const string caption = "";
                var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    this.Close();
                    gl_b_isAboutLoaded = false;
                    gl_b_isOptOpen = false;
                }
            }
            else
            {
                this.Close();
                gl_b_isAboutLoaded = false;
                gl_b_isOptOpen = false;
            }
        }

        private void ButChangePass_Click(object sender, EventArgs e)
        {
            string tryPass = MD5Code(BoxPassOld.Text + gl_s_OSdate);
            if (tryPass == gl_s_optionsPasswordReg)
            {
                if (BoxPassNew.Text == BoxPassNew2.Text)
                {
                    gl_s_optionsPasswordReg = MD5Code(BoxPassNew.Text + gl_s_OSdate);
                    RegistryKey localMachineKey = Registry.LocalMachine;
                    RegistryKey localMachineSoftKey = localMachineKey.OpenSubKey("SOFTWARE", true);
                    RegistryKey regNanit = localMachineSoftKey.CreateSubKey(@"N.A.N.I.T");
                    regNanit.SetValue("password", gl_s_optionsPasswordReg);
                    regNanit.Close();
                    const string message = "Пароль успешно изменён";
                    const string caption = "";
                    var result = MessageBox.Show(message, caption, MessageBoxButtons.OK);
                    BoxPassNew.Text = "";
                    BoxPassNew2.Text = "";
                    BoxPassOld.Text = "";
                }
                else
                {
                    const string message = "Введённые пароли не совпадают";
                    const string caption = "";
                    var result = MessageBox.Show(message, caption, MessageBoxButtons.OK);
                    BoxPassNew.Text = "";
                    BoxPassNew2.Text = "";
                    BoxPassOld.Text = "";
                }
            }
            else
            {
                const string message = "Текущий пароль введён неверно";
                const string caption = "";
                var result = MessageBox.Show(message, caption, MessageBoxButtons.OK);
                BoxPassNew.Text = "";
                BoxPassNew2.Text = "";
                BoxPassOld.Text = "";
            }
        }

        private void ButServiceChange_Click(object sender, EventArgs e)
        {
            if (!gl_b_isUpdOpen)
            {
                gl_f_updater = new FormUpdater();
                gl_f_updater.Show();
                gl_b_isUpdOpen = true;
                ButServiceChange.Enabled = false;
            }
        }

        private void ButServiceInstall_Click(object sender, EventArgs e)
        {
            Thread InstallService = new Thread(new ThreadStart(ServiceWork.InstallService));
            InstallService.Name = "Install or Update on click";
            InstallService.Start();
        }


        private void ButServiceDel_Click(object sender, EventArgs e)
        {
            Thread DeleteService = new Thread(new ThreadStart(ServiceWork.DeleteService));
            DeleteService.Name = "Delete on click";
            DeleteService.Start();
        }

        public void ServiceInit()
        {
            lock (locker)
            {
                switch (gl_by_serviceStatus)
                //0 не установлена и не запущена. 1 установлена и запущена. 2 установлена не запущена. 3 обновление запущена 4 обновление не запущена
                {
                    case 0:
                        gl_f_options.ButServiceDel.Enabled = false;
                        gl_f_options.ButServiceInstall.Text = "Установить";
                        gl_f_options.LabelServiceStart.Text = "Не запущена";
                        gl_f_options.LabelServiceStart.ForeColor = System.Drawing.Color.Red;
                        gl_f_options.LabelServiceInstall.Text = "Не установлена";
                        gl_f_options.LabelServiceInstall.ForeColor = System.Drawing.Color.Red;
                        if (gl_i_adrUpdNum == -1)
                        {
                            gl_f_options.groupBox2.Text = "Служба Windows (для обновлений)";
                            gl_f_options.ButServiceInstall.Enabled = false;
                        }
                        else
                        {
                            gl_f_options.groupBox2.Text = "Установить службу обновлений версии " + gl_s_updVerAvi;
                            gl_f_options.ButServiceInstall.Enabled = true;
                        }
                        break;
                    case 1:
                        gl_f_options.ButServiceDel.Enabled = true;
                        gl_f_options.ButServiceInstall.Text = "ОК";
                        gl_f_options.ButServiceInstall.Enabled = false;
                        gl_f_options.LabelServiceStart.Text = "Запущена";
                        gl_f_options.LabelServiceStart.ForeColor = System.Drawing.Color.Green;
                        gl_f_options.LabelServiceInstall.Text = "Установлена (" + gl_s_nanitSvcVer + ")";
                        gl_f_options.LabelServiceInstall.ForeColor = System.Drawing.Color.Green;
                        if (gl_i_adrUpdNum == -1)
                            gl_f_options.groupBox2.Text = "Служба Windows (для обновлений)";
                        else
                            gl_f_options.groupBox2.Text = "Доступная версия службы обновлений " + gl_s_nanitSvcVer;
                        break;
                    case 2:
                        gl_f_options.ButServiceDel.Enabled = true;
                        gl_f_options.ButServiceInstall.Text = "Запустить";
                        gl_f_options.ButServiceInstall.Enabled = true;
                        gl_f_options.LabelServiceStart.Text = "Не запущена";
                        gl_f_options.LabelServiceStart.ForeColor = System.Drawing.Color.Red;
                        gl_f_options.LabelServiceInstall.Text = "Установлена (" + gl_s_nanitSvcVer + ")";
                        gl_f_options.LabelServiceInstall.ForeColor = System.Drawing.Color.Green;
                        if (gl_i_adrUpdNum == -1)
                            gl_f_options.groupBox2.Text = "Служба Windows (для обновлений)";
                        else
                            gl_f_options.groupBox2.Text = "Доступная версия службы обновлений " + gl_s_nanitSvcVer;
                        break;
                    case 3:
                        gl_f_options.ButServiceDel.Enabled = true;
                        gl_f_options.ButServiceInstall.Text = "Обновить";
                        gl_f_options.LabelServiceStart.Text = "Запущена";
                        gl_f_options.LabelServiceStart.ForeColor = System.Drawing.Color.Green;
                        gl_f_options.LabelServiceInstall.Text = "Установлена (" + gl_s_nanitSvcVer + ")";
                        gl_f_options.LabelServiceInstall.ForeColor = System.Drawing.Color.Green;
                        if (gl_i_adrUpdNum == -1)
                        {
                            gl_f_options.groupBox2.Text = "Служба Windows (для обновлений)";
                            gl_f_options.ButServiceInstall.Enabled = false;
                        }
                        else
                        {
                            gl_f_options.groupBox2.Text = "Обновить службу обновлений до версии " + gl_s_updVerAvi;
                            gl_f_options.ButServiceInstall.Enabled = true;
                        }
                        break;
                    case 4:
                        gl_f_options.ButServiceDel.Visible = false;
                        gl_f_options.LabelServiceStart.Text = "Не запущена";
                        gl_f_options.LabelServiceStart.ForeColor = System.Drawing.Color.Red;
                        gl_f_options.LabelServiceInstall.Text = "Установлена (" + gl_s_nanitSvcVer + ")";
                        gl_f_options.LabelServiceInstall.ForeColor = System.Drawing.Color.Green;
                        if (gl_i_adrUpdNum == -1)
                        {
                            gl_f_options.ButServiceInstall.Text = "Запустить";
                            gl_f_options.groupBox2.Text = "Служба Windows (для обновлений)";
                            gl_f_options.ButServiceInstall.Enabled = true;
                        }
                        else
                        {
                            gl_f_options.ButServiceInstall.Text = "Обновить";
                            gl_f_options.groupBox2.Text = "Обновить службу обновлений до версии " + gl_s_updVerAvi;
                            gl_f_options.ButServiceInstall.Enabled = true;
                        }
                        break;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)                  // Кнопка для тестирования всякой фигни
        {
            //gl_c_current.SendMessage(textBox1.Text);
            textBox1.Text = "";
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ServiceInit();
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (gl_b_isOptOpen)
            {
                if (workerByte)
                {
                    if (!gl_b_workLock)
                    {
                        backgroundWorker1.ReportProgress(30);
                        workerByte = Revers(workerByte);
                    }
                    else
                        Thread.Sleep(200);
                }
                else
                {
                    if (gl_b_workLock)
                    {
                        backgroundWorker1.ReportProgress(60);
                        workerByte = Revers(workerByte);
                    }
                    else
                        Thread.Sleep(200);
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
                if (gl_b_isOptOpen)
                    MessageBox.Show("Что-то пошло не так. Я закончил работать");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread DeleteService = new Thread(new ThreadStart(ServiceWork.DeleteService));
            DeleteService.Name = "Delete on click";
            DeleteService.Start();
        }
    }
}