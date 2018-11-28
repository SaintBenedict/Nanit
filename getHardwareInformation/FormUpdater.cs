using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NaNiT
{
    public partial class FormUpdater : Form
    {
        int chInd = -1;
        int numInd = -1;
        public FormUpdater()
        {
            InitializeComponent();            
            ToolTip delB = new ToolTip();
            delB.SetToolTip(ButDel, "Удалить");
            ToolTip chgB = new ToolTip();
            chgB.SetToolTip(ButDel, "Редактировать");
            Enables();
        }

        private void Enables()
        {
            ///MessageBox.Show(ChList.CheckedIndices.Count.ToString());
            if (ChList.CheckedIndices.Count != 0)
            {
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
            Enables();
        }

        private void ButChg_Click(object sender, EventArgs e)
        {
            chInd = 1;
            foreach (int indexChecked in ChList.CheckedIndices)
                numInd =  indexChecked;
            foreach (object itemChecked in ChList.CheckedItems)
                textBox1.Text = itemChecked.ToString();
            ButDel.Enabled = false;
            ChList.Enabled = false;
            button1.Enabled = true;
        }

        private void ButDel_Click(object sender, EventArgs e)
        {
            var i = 0;
            int[] k = new int [15];
            foreach (int indexChecked in ChList.CheckedIndices)
            {
                k[i] = indexChecked;
                i++;
            }
            for (int j = 0; j < i; j++)
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

        private void FormUpdater_Close(object sender, EventArgs e)
        {
            Globals.isUpdOpen = false;
            Globals.form2.ButServiceChange.Enabled = true;
        }       

        /*public int IndOf(object value)
        {
            // получение индексов отмеченных элементов
            foreach (int indexChecked in ChList.CheckedIndices)
            {
                MessageBox.Show("Index#: " + indexChecked.ToString() + ", is checked. Checked state is:" +
                                ChList.GetItemCheckState(indexChecked).ToString() + ".");
            }
            // получение подписей (title) отмеченных элементов
            foreach (object itemChecked in ChList.CheckedItems)
            {
                MessageBox.Show("Item with title: \"" + itemChecked.ToString() +
                                "\", is checked. Checked state is: " +
                          ChList.GetItemCheckState(ChList.Items.IndexOf(itemChecked)).ToString() + ".");

            }
        }*/
    }
}
