using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using FunSumC2XML;

namespace FunSumGUI
{
    public partial class Form1 : Form
    {
        private string[] fileNames;
        private string dirPath;
        private Summarization sum;

        public Form1()
        {
            InitializeComponent();
        }

        private void btn_select_Click(object sender, EventArgs e)
        {
            // 显示文件夹选择对话框
            DialogResult result = this.folderDlg_1.ShowDialog();

            // 获取文件信息
            if (result == DialogResult.OK)
            {
                if (this.lst_files.Items.Count != 0)
                {
                    this.lst_files.Items.Clear();
                }
                this.dirPath = this.folderDlg_1.SelectedPath;
                DirectoryInfo dirInfo = new DirectoryInfo(this.dirPath);
                FileInfo[] files = dirInfo.GetFiles("*.c");
                foreach (FileInfo file in files)
                {
                    this.lst_files.Items.Add(file.Name);
                }
            }
        }

        private void btn_analysis_Click(object sender, EventArgs e)
        {
            if (this.lst_files.Items == null || this.lst_files.Items.Count == 0)
            {
                MessageBox.Show("请先选择C源代码文件所在的文件夹", "提示");
                return;
            }

            this.fileNames = new string[this.lst_files.Items.Count];
            for (int i = 0; i < this.lst_files.Items.Count; i++)
            {
                this.fileNames[i] = this.dirPath + "\\" + this.lst_files.Items[i].ToString();
            }

            CStandard standard = CStandard.ANSI_C;
            if (this.comboBox_standard.SelectedIndex == 1)
                standard = CStandard.Keil_C_51;

            this.sum = new Summarization(this.fileNames, standard);
            this.sum.Summary();
            this.sum.PostProcess();
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            if (this.sum == null)
            {
                MessageBox.Show("请先抽取设计要素", "提示");
                return;
            }
            DialogResult result = this.saveFileDlg_1.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string filename = this.saveFileDlg_1.FileName;
                if (this.saveFileDlg_1.FilterIndex == 1)
                {
                    sum.ToWordFile(filename);
                }
                else if (this.saveFileDlg_1.FilterIndex == 2)
                {
                    sum.ToExcelFile(filename);
                }
            }
            this.sum = null;
        }

        private void btn_remove_Click(object sender, EventArgs e)
        {
            if (this.lst_files.SelectedItem != null)
            {
                this.lst_files.Items.Remove(this.lst_files.SelectedItem);
            }
            else
            {
                MessageBox.Show("请先选中要移除的文件", "提示");
            }
        }

        private void saveFileDlg_1_FileOk(object sender, CancelEventArgs e)
        {

        }
    }
}
