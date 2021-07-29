namespace _luoming
{
    partial class importdata
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtImportData = new System.Windows.Forms.TextBox();
            this.btFName = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.txtNewName = new System.Windows.Forms.TextBox();
            this.btSave = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.cbWorkspace = new System.Windows.Forms.ComboBox();
            this.pbImport = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "导入数据集：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "工作空间：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(34, 149);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "数据集名：";
            // 
            // txtImportData
            // 
            this.txtImportData.Location = new System.Drawing.Point(117, 29);
            this.txtImportData.Name = "txtImportData";
            this.txtImportData.Size = new System.Drawing.Size(363, 21);
            this.txtImportData.TabIndex = 3;
            // 
            // btFName
            // 
            this.btFName.Location = new System.Drawing.Point(507, 29);
            this.btFName.Name = "btFName";
            this.btFName.Size = new System.Drawing.Size(75, 23);
            this.btFName.TabIndex = 4;
            this.btFName.Text = ".  .  .";
            this.btFName.UseVisualStyleBackColor = true;
            this.btFName.Click += new System.EventHandler(this.btFName_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // txtNewName
            // 
            this.txtNewName.Location = new System.Drawing.Point(117, 146);
            this.txtNewName.Name = "txtNewName";
            this.txtNewName.Size = new System.Drawing.Size(363, 21);
            this.txtNewName.TabIndex = 5;
            // 
            // btSave
            // 
            this.btSave.Location = new System.Drawing.Point(405, 234);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(75, 23);
            this.btSave.TabIndex = 6;
            this.btSave.Text = "导入";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // btCancel
            // 
            this.btCancel.Location = new System.Drawing.Point(507, 234);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 7;
            this.btCancel.Text = "取消";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // cbWorkspace
            // 
            this.cbWorkspace.FormattingEnabled = true;
            this.cbWorkspace.Location = new System.Drawing.Point(117, 85);
            this.cbWorkspace.Name = "cbWorkspace";
            this.cbWorkspace.Size = new System.Drawing.Size(363, 20);
            this.cbWorkspace.TabIndex = 8;
            // 
            // pbImport
            // 
            this.pbImport.Location = new System.Drawing.Point(25, 182);
            this.pbImport.Name = "pbImport";
            this.pbImport.Size = new System.Drawing.Size(536, 35);
            this.pbImport.TabIndex = 9;
            // 
            // importdata
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 286);
            this.Controls.Add(this.pbImport);
            this.Controls.Add(this.cbWorkspace);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.txtNewName);
            this.Controls.Add(this.btFName);
            this.Controls.Add(this.txtImportData);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "importdata";
            this.Text = "importdata";
            this.Load += new System.EventHandler(this.importdata_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtImportData;
        private System.Windows.Forms.Button btFName;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox txtNewName;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.ComboBox cbWorkspace;
        private System.Windows.Forms.ProgressBar pbImport;
    }
}