using System;
using System.Diagnostics;
using System.Windows.Forms;
using static NaNiT.GlobalVariable;
using static NaNiT.GlobalFunctions;
using static NaNiT.LocalGlobals;

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

            if (gl_b_debug)
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
            if (!gl_b_isSoftOpen)
            {
                gl_f_soft = new FormSoft();
                gl_f_soft.Show();
                gl_b_isSoftOpen = true;
            }
        }

        void Options_Click(object sender, EventArgs e)
		{
            if (!gl_b_isAboutLoaded)
			{
                gl_b_isAboutLoaded = true;
                if (MD5Code("" + gl_s_OSdate) == gl_s_optionsPasswordReg)
                {
                    if (!gl_b_isOptOpen)
                    {
                        gl_f_options = new FormOptions();
                        gl_f_options.Text = (@"N.A.N.I.T (((ver." + gl_s_version + ")))");
                        gl_f_options.Show();
                        gl_b_isOptOpen = true;
                    }
                }
                else
                {
                    gl_f_login = new FormLogin();
                    gl_f_login.Show();
                }
            }
		}

		void Exit_Click(object sender, EventArgs e)
		{
            ClientProgram.TrayNotify.Dispose();
            Application.Exit();
            Process.GetCurrentProcess().Kill();
        }
	}
}