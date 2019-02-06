using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace NaNiT
{
	class SContextMenus
	{       
        public ContextMenuStrip Create()
		{
			ContextMenuStrip menu = new ContextMenuStrip();
			ToolStripMenuItem item;
			ToolStripSeparator sep;

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
            if (!Globals.isAboutLoaded)
			{
                    if (!Globals.isOptOpen)
                    {
                        //Globals.form1 = new FormSOptions();
                        Globals.form1.Text = (@"N.A.N.I.T Server");
                        Globals.form1.Show();
                        Globals.isOptOpen = true;
                    }
            }
		}

		void Exit_Click(object sender, EventArgs e)
		{
            Globals.form1.Stop();
            Globals.form1.Dispose();
            SProgram.notifyIcon.Dispose();
            Application.Exit();
            Process.GetCurrentProcess().Kill();
        }
	}
}