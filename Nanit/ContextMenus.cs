using System;
using System.Diagnostics;
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

            if (!Globals.DEBUGMODE)
            {
                item = new ToolStripMenuItem();
                item.Text = "Просмотр программ";
                item.Click += new EventHandler(Software_Click);
                menu.Items.Add(item);
            }

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
            CatchInfo.InfoGet();
        }

        void Software_Click(object sender, EventArgs e)
        {
            Globals.form4 = new FormSoft();
            Globals.form4.Show();
        }

        void Options_Click(object sender, EventArgs e)
		{
            if (!Globals.isAboutLoaded)
			{
                Globals.isAboutLoaded = true;
                if (Functions.MD5Code("" + Globals.OSdate) == Globals.optionsPasswordReg)
                {
                    if (!Globals.isOptOpen)
                    {
                        Globals.form2 = new FormOptions();
                        Globals.form2.Text = (@"N.A.N.I.T (((ver." + Globals.version + ")))");
                        Globals.form2.Show();
                        Globals.isOptOpen = true;
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
            Process.GetCurrentProcess().Kill();
        }
	}
}