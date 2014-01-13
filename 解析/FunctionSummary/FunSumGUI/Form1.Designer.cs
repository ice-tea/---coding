namespace FunSumGUI
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lst_files = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_select = new System.Windows.Forms.Button();
            this.folderDlg_1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btn_analysis = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.saveFileDlg_1 = new System.Windows.Forms.SaveFileDialog();
            this.lbl_result_filename = new System.Windows.Forms.Label();
            this.btn_save = new System.Windows.Forms.Button();
            this.btn_remove = new System.Windows.Forms.Button();
            this.comboBox_standard = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // lst_files
            // 
            this.lst_files.FormattingEnabled = true;
            this.lst_files.ItemHeight = 12;
            this.lst_files.Location = new System.Drawing.Point(33, 33);
            this.lst_files.Name = "lst_files";
            this.lst_files.Size = new System.Drawing.Size(207, 148);
            this.lst_files.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "待分析的C文件";
            // 
            // btn_select
            // 
            this.btn_select.Location = new System.Drawing.Point(165, 4);
            this.btn_select.Name = "btn_select";
            this.btn_select.Size = new System.Drawing.Size(75, 23);
            this.btn_select.TabIndex = 2;
            this.btn_select.Text = "选择";
            this.btn_select.UseVisualStyleBackColor = true;
            this.btn_select.Click += new System.EventHandler(this.btn_select_Click);
            // 
            // btn_analysis
            // 
            this.btn_analysis.Location = new System.Drawing.Point(165, 226);
            this.btn_analysis.Name = "btn_analysis";
            this.btn_analysis.Size = new System.Drawing.Size(75, 23);
            this.btn_analysis.TabIndex = 3;
            this.btn_analysis.Text = "抽取";
            this.btn_analysis.UseVisualStyleBackColor = true;
            this.btn_analysis.Click += new System.EventHandler(this.btn_analysis_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 266);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "选择要素保存文件";
            // 
            // saveFileDlg_1
            // 
            this.saveFileDlg_1.Filter = "Word文件 | *.docx|Excel文件 | *.xlsx";
            this.saveFileDlg_1.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDlg_1_FileOk);
            // 
            // lbl_result_filename
            // 
            this.lbl_result_filename.AutoSize = true;
            this.lbl_result_filename.Location = new System.Drawing.Point(129, 188);
            this.lbl_result_filename.Name = "lbl_result_filename";
            this.lbl_result_filename.Size = new System.Drawing.Size(0, 12);
            this.lbl_result_filename.TabIndex = 5;
            // 
            // btn_save
            // 
            this.btn_save.Location = new System.Drawing.Point(165, 266);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(75, 23);
            this.btn_save.TabIndex = 6;
            this.btn_save.Text = "保存报告";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // btn_remove
            // 
            this.btn_remove.Location = new System.Drawing.Point(101, 187);
            this.btn_remove.Name = "btn_remove";
            this.btn_remove.Size = new System.Drawing.Size(75, 23);
            this.btn_remove.TabIndex = 7;
            this.btn_remove.Text = "移除";
            this.btn_remove.UseVisualStyleBackColor = true;
            this.btn_remove.Click += new System.EventHandler(this.btn_remove_Click);
            // 
            // comboBox_standard
            // 
            this.comboBox_standard.FormattingEnabled = true;
            this.comboBox_standard.Items.AddRange(new object[] {
            "ANSI-C",
            "Keil-C 51"});
            this.comboBox_standard.SelectedIndex = 1;
            this.comboBox_standard.Location = new System.Drawing.Point(33, 228);
            this.comboBox_standard.Name = "comboBox_standard";
            this.comboBox_standard.Size = new System.Drawing.Size(99, 20);
            this.comboBox_standard.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 301);
            this.Controls.Add(this.comboBox_standard);
            this.Controls.Add(this.btn_remove);
            this.Controls.Add(this.btn_save);
            this.Controls.Add(this.lbl_result_filename);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btn_analysis);
            this.Controls.Add(this.btn_select);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lst_files);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "设计文档要素提取";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lst_files;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_select;
        private System.Windows.Forms.FolderBrowserDialog folderDlg_1;
        private System.Windows.Forms.Button btn_analysis;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.SaveFileDialog saveFileDlg_1;
        private System.Windows.Forms.Label lbl_result_filename;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.Button btn_remove;
        private System.Windows.Forms.ComboBox comboBox_standard;
    }
}

