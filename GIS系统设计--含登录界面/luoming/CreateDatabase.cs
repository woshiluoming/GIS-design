using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;

namespace _luoming
{
    public partial class CreateDatabase : Form
    {
        public CreateDatabase()
        {
            InitializeComponent();
        }
        string filePath;

        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }
        string fileName;

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }
        IWorkspace newWorkspace;

        // 新建工作空间
        public IWorkspace NewWorkspace
        {
            get { return newWorkspace; }
            set { newWorkspace = value; }
        }

        // 浏览工作空间目录
        private void btDirect_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.txtDirect.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        // 创建数据库
        private void btCreate_Click(object sender, EventArgs e)
        {
            filePath = this.txtDirect.Text.ToString().Trim();
            fileName = this.txtDbName.Text.ToString().Trim();
            if(filePath == "") return;
            IWorkspaceFactory workspaceFactory = new AccessWorkspaceFactoryClass();
            IWorkspaceName workspaceName = workspaceFactory.Create(filePath,fileName,null,0);
            IName name = workspaceName as IName;
            IWorkspace workspace = (IWorkspace) name.Open();
            newWorkspace = workspace;
            this.Close();
            this.Dispose();
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}
