using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace NaNiT
{
    public partial class FormUpdater : Form
    {
        sbyte chInd = -1;
        sbyte numInd = -1;
        public FormUpdater()
        {
            InitializeComponent();            
            ToolTip delB = new ToolTip();
            delB.SetToolTip(ButDel, "Удалить");
            ToolTip chgB = new ToolTip();
            chgB.SetToolTip(ButDel, "Редактировать");
            ChList.Items.Clear();

            for (byte j = 0; j < 11; j++)
            {
                if (Globals.pathUpdate[j] != null)
                {
                    ChList.Items.Add(Globals.pathUpdate[j]);
                }
            }            
            Enables();
        }

        private void Enables()
        {
            if (ChList.CheckedIndices.Count != 0)
            {
                ChList.SetItemCheckState(0, CheckState.Indeterminate);
                if (ChList.CheckedIndices.Count == 1)
                    ButChg.Enabled = true;
                else
                    ButChg.Enabled = false;

                if (ChList.CheckedIndices.Count > 0)
                    ButDel.Enabled = true;
            }
            else
            {
                ButDel.Enabled = false;
                ButChg.Enabled = false;
            }
            if (ChList.Items.Count > 10)
                button1.Enabled = false;
            else
                button1.Enabled = true;
        }

        private void ChList_ItemCheck(Object sender, EventArgs e)
        {
            foreach (sbyte indexChecked in ChList.CheckedIndices)
            {
                if (indexChecked == 0)
                    ChList.SetItemCheckState(0, CheckState.Unchecked);
            }
            Enables();
        }

        private void ButChg_Click(object sender, EventArgs e)
        {
            chInd = 1;
            foreach (sbyte indexChecked in ChList.CheckedIndices)
                numInd =  indexChecked;
            foreach (object itemChecked in ChList.CheckedItems)
                textBox1.Text = itemChecked.ToString();
            ButDel.Enabled = false;
            ChList.Enabled = false;
            button1.Enabled = true;
        }

        private void ButDel_Click(object sender, EventArgs e)
        {
            sbyte i = 0;
            short[] k = new short[15];
            foreach (short indexChecked in ChList.CheckedIndices)
            {
                k[i] = indexChecked;
                i++;
            }
            for (byte j = 0; j < i; j++)
            {
                ChList.Items.RemoveAt(k[j]-j);
            }
            i = -1; k = null;
            Enables();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (chInd == -1)
            {
                    ChList.Items.Add(textBox1.Text);
                    textBox1.Text = "";
            }
            else
            {
                ChList.Items.Insert(numInd, textBox1.Text);
                ChList.Items.RemoveAt(numInd + 1);
                textBox1.Text = "";
                chInd = -1;
                ChList.Enabled = true;
            }
            Enables();
        }

        public void FormUpdater_Close(object sender, EventArgs e)
        {
            RegistryKey localMachineKey = Registry.LocalMachine;
            RegistryKey localMachineSoftKey = localMachineKey.OpenSubKey("SOFTWARE", true);
            RegistryKey regNanit = localMachineSoftKey.CreateSubKey(@"N.A.N.I.T");
            RegistryKey updateKey = regNanit.CreateSubKey("Update");
            for (byte j = 0; j < ChList.Items.Count; j++)
            {
                Globals.pathUpdate[j] = ChList.Items[j].ToString();
                updateKey.SetValue("path_update_" + j.ToString(), Globals.pathUpdate[j]);
            }
            for (byte j = Convert.ToByte(ChList.Items.Count); j < 11; j++)
            {
                Globals.pathUpdate[j] = null;
                updateKey.SetValue("path_update_" + j.ToString(), "NULL");
            }
            regNanit.Close();
            updateKey.Close();
            Globals.form2.ServiceInit();
            Globals.isUpdOpen = false;
            Globals.form2.ButServiceChange.Enabled = true;
        }
    }
}

