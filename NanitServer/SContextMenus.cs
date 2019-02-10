using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using static NaNiT.GlobalVariable;

namespace NaNiT
{
	class SContextMenus
	{
        public ContextMenuStrip Create()
		{
			ContextMenuStrip menu = new ContextMenuStrip();
			ToolStripMenuItem item;
			ToolStripSeparator sep;
            Thread Tray = Thread.CurrentThread;
            if (Tray.Name == null)
                Tray.Name = "Tray Icon";

            /// Создание кнопок

            item = new ToolStripMenuItem();
			item.Text = "Настройки";
			item.Click += new EventHandler(Options_Click);
			///item.Image = Resources.About;
			menu.Items.Add(item);
			
            // Separator.
			sep = new ToolStripSeparator();
			menu.Items.Add(sep);

			item = new ToolStripMenuItem();
			item.Text = "Exit";
			item.Click += new System.EventHandler(Exit_Click);
			///item.Image = Resources.Exit;
			menu.Items.Add(item);

			return menu;
		}

        
        void Options_Click(object sender, EventArgs e)
		{
            if (!gl_b_isAboutLoaded)
			{
                    if (!gl_b_isOptOpen)
                    {
                        //gl_f_optionsServ = new FormSOptions();
                        gl_f_optionsServ.Text = (@"N.A.N.I.T Server");
                        gl_f_optionsServ.Show();
                        gl_b_isOptOpen = true;
                    }
            }
		}

		void Exit_Click(object sender, EventArgs e)
		{
            gl_f_optionsServ.Stop();
            gl_f_optionsServ.Dispose();
            SProgram.notifyIcon.Dispose();
            Application.Exit();
            Process.GetCurrentProcess().Kill();
        }
	}
}