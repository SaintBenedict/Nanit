using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class СlientForm : Form
    {
        public СlientForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
  

       

        private void buttonSend_Click(object sender, EventArgs e)
        {
            Program.SendMessage();
            Globals.form1.textSend.Text = "";
        }

        private void buttoncon_Click(object sender, EventArgs e)
        {
            Program.Chat();
        }

        private void buttondis_Click(object sender, EventArgs e)
        {
            Program.Disconnect();
        }
    }
}

