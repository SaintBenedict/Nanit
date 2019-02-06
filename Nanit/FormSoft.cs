using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NaNiT
{
    public partial class FormSoft : Form
    {
        public FormSoft()
        {
            InitializeComponent();
            GetInstalledSoftware();
            Globals.form4 = this;
        }
        public void FormSoft_Close(object sender, EventArgs e)
        {
            Globals.isSoftOpen = false;
        }


        private void GetInstalledSoftware()
        {
            List<string> items = new List<string>();
            string SoftwareKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(SoftwareKey))
            {
                foreach (string skName in rk.GetSubKeyNames())
                {
                    using (RegistryKey sk = rk.OpenSubKey(skName))
                    {
                        try
                        {
                            if (sk.GetValue("DisplayName") != null)
                            {
                                items.Add(sk.GetValue("DisplayName").ToString());
                                items.Add(CheckValue(sk.GetValue("DisplayVersion")));
                                items.Add(CheckValue(sk.GetValue("Publisher")));
                                listView1.Items.Add(new ListViewItem(items.ToArray()));
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    items.Clear();
                }
            }
            SoftwareKey = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
            using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(SoftwareKey))
            {
                foreach (string skName in rk.GetSubKeyNames())
                {
                    using (RegistryKey sk = rk.OpenSubKey(skName))
                    {
                        try
                        {
                            if (sk.GetValue("DisplayName") != null)
                            {
                                items.Add(sk.GetValue("DisplayName").ToString());
                                items.Add(CheckValue(sk.GetValue("DisplayVersion")));
                                items.Add(CheckValue(sk.GetValue("Publisher")));
                                listView1.Items.Add(new ListViewItem(items.ToArray()));
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    items.Clear();
                }
            }
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            listView1.ListViewItemSorter = new ListViewColumnComparer(0);
            int progs = listView1.Items.Count;
            Globals.programs = new string[progs + 1, 3];
            for (int i = 0; i < progs; i++)
            {
                Globals.programs[i, 0] = listView1.Items[i].SubItems[0].Text;
                Globals.programs[i, 1] = listView1.Items[i].SubItems[1].Text;
                Globals.programs[i, 2] = listView1.Items[i].SubItems[2].Text;
            }
        }

        private string CheckValue(object input)
        {
            if (input != null)
                return input.ToString();
            else
                return string.Empty;
        }
    }

    class ListViewColumnComparer : IComparer
    {
        public int ColumnIndex { get; set; }

        public ListViewColumnComparer(int columnIndex)
        {
            ColumnIndex = columnIndex;
        }

        public int Compare(object x, object y)
        {
            try
            {
                return String.Compare(
                ((ListViewItem)x).SubItems[ColumnIndex].Text,
                ((ListViewItem)y).SubItems[ColumnIndex].Text);
            }
            catch (Exception) // если вдруг столбец пустой (или что-то пошло не так)
            {
                return 0;
            }
        }
    }
}
