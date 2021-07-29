namespace _luoming
{
    partial class CreateDatabase
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
            this.txtDirect = new System.Windows.Forms.TextBox();
            this.txtDbName = new System.Windows.Forms.TextBox();
            this.btDirect = new System.Windows.Forms.Button();
            this.btCreate = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "工作空间目录：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "数据库名称：";
            // 
            // txtDirect
            // 
            this.txtDirect.Location = new System.Drawing.Point(123, 33);
            this.txtDirect.Name = "txtDirect";
            this.txtDirect.Size = new System.Drawing.Size(307, 21);
            this.txtDirect.TabIndex = 2;
            // 
            // txtDbName
            // 
            this.txtDbName.Location = new System.Drawing.Point(123, 92);
            this.txtDbName.Name = "txtDbName";
            this.txtDbName.Size = new System.Drawing.Size(88, 21);
            this.txtDbName.TabIndex = 3;
            // 
            // btDirect
            // 
            this.btDirect.Location = new System.Drawing.Point(466, 33);
            this.btDirect.Name = "btDirect";
            this.btDirect.Size = new System.Drawing.Size(75, 23);
            this.btDirect.TabIndex = 4;
            this.btDirect.Text = ".  .  .";
            this.btDirect.UseVisualStyleBackColor = true;
            this.btDirect.Click += new System.EventHandler(this.btDirect_Click);
            // 
            // btCreate
            // 
            this.btCreate.Location = new System.Drawing.Point(373, 151);
            this.btCreate.Name = "btCreate";
            this.btCreate.Size = new System.Drawing.Size(75, 23);
            this.btCreate.TabIndex = 5;
            this.btCreate.Text = "创建";
            this.btCreate.UseVisualStyleBackColor = true;
            this.btCreate.Click += new System.EventHandler(this.btCreate_Click);
            // 
            // btCancel
            // 
            this.btCancel.Location = new System.Drawing.Point(466, 151);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 6;
            this.btCancel.Text = "取消";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // CreateDatabase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 211);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btCreate);
            this.Controls.Add(this.btDirect);
            this.Controls.Add(this.txtDbName);
            this.Controls.Add(this.txtDirect);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "CreateDatabase";
            this.Text = "CreateDatabase";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDirect;
        private System.Windows.Forms.TextBox txtDbName;
        private System.Windows.Forms.Button btDirect;
        private System.Windows.Forms.Button btCreate;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}