using System;
using System.Windows.Forms;

namespace NaNiT
{
	class ContextMenus
	{       
        public ContextMenuStrip Create()
		{
			ContextMenuStrip menu = new ContextMenuStrip();
			ToolStripMenuItem item;
			ToolStripSeparator sep;

            /// Создание кнопок

            item = new ToolStripMenuItem();
			item.Text = "Сбор данных в файл";
            item.Click += new EventHandler(DataHover_Click);
			menu.Items.Add(item);

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

		void DataHover_Click(object sender, EventArgs e)
		{
            Program.InfoGet();
        }

		void Options_Click(object sender, EventArgs e)
		{
            if (!Globals.isAboutLoaded)
			{
                Globals.isAboutLoaded = true;
                if (Program.MD5Code("") == Globals.optionsPasswordReg)
                {
                    if (!Globals.isOptOpen)
                    {
                        Globals.form2 = new FormOptions();
                        Globals.isOptOpen = true;
                        Globals.form2.Text = (@"N.A.N.I.T (((ver." + Globals.version + ")))");
                        Globals.form2.Show();
                    }
                }
                else
                {
                    Globals.form1 = new FormLogin();
                    Globals.form1.Show();
                }
            }
		}

		void Exit_Click(object sender, EventArgs e)
		{
            Program.notifyIcon.Dispose();
            Application.Exit();
		}
	}
}