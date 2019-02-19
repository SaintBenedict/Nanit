using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace NaNiT
{
    class ContextMenus
    {
        public ContextMenuStrip Create()
        {
            ContextMenuStrip Menu = new ContextMenuStrip();
            ToolStripMenuItem item;
            ToolStripSeparator sep;

            /// Создание кнопок
            /// Они могут иметь иконки (item.Image = Resources.***;)
            item = new ToolStripMenuItem();
            item.Text = "Настройки";
            item.Click += new EventHandler(Options_Click);
            Menu.Items.Add(item);

            // Separator.
            sep = new ToolStripSeparator();
            Menu.Items.Add(sep);

            item = new ToolStripMenuItem();
            item.Text = "Exit";
            item.Click += new EventHandler(Exit_Click);
            Menu.Items.Add(item);

            return Menu;
        }

        void Options_Click(object sender, EventArgs e)
        {
            if (!MainProgram.ServerConfig.TrayMenuIsOpen && !MainProgram.ServerConfig.ServerFormIsOpen)
            {
                MainProgram.ServerForm.Show();
                MainProgram.ServerConfig.ServerFormIsOpen = true;
                MainProgram.ServerConfig.TrayMenuIsOpen = true;
            }
        }

        void Exit_Click(object sender, EventArgs e)
        {
            MainProgram.StopServer();
            MainProgram.TrayNotify.Icon.Dispose();
            MainProgram.TrayNotify.Dispose();
            MainProgram.ServerForm.Dispose();
            Application.Exit();
            Process.GetCurrentProcess().Kill();
        }
    }
}