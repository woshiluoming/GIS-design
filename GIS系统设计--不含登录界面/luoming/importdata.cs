using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.esriSystem;

namespace _luoming
{
    public partial class importdata : Form
    {
        ArrayList curWorkspaceList;

        // 当前工作空间列表
        public ArrayList CurWorkspaceList
        {
            get { return curWorkspaceList; }
            set { curWorkspaceList = value; }
        }

        public importdata()
        {
            InitializeComponent();
        }

        // 选择SHAPE数据文件
        private void btFName_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = "shp文件";
            this.openFileDialog1.Filter = "shp(*.shp)|*.shp";
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = this.openFileDialog1.FileName;
                this.txtImportData.Text = fileName;
                FileInfo fileInfo = new FileInfo(fileName);
                this.txtNewName.Text = fileInfo.Name.Substring(0, fileInfo.Name.Length - 4);
            }
        }

        // 初始化Combox空间，将工作空间列表中的工作空间名作为子项
        private void importdata_Load(object sender, EventArgs e)
        {
            IWorkspace workspace;
            for (int i = 0; i < curWorkspaceList.Count; i++)
            {
                workspace = curWorkspaceList[i] as IWorkspace;
                this.cbWorkspace.Items.Add(workspace.PathName);
            }
            this.cbWorkspace.Text = this.cbWorkspace.Items[0].ToString();
        }

        // 退出窗体
        private void btCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        // 导入空间数据
        private void btSave_Click(object sender, EventArgs e)
        {
            if (txtNewName.Text.Trim() == "")
                return;
            IWorkspace workspace;
            int index = -1;
            for (int i = 0; i < curWorkspaceList.Count; i++)
            {
                workspace = curWorkspaceList[i] as IWorkspace;
                if (workspace.PathName == this.cbWorkspace.Text.Trim())
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0)
            {
                workspace = curWorkspaceList[index] as IWorkspace;
                // shp
                IWorkspaceFactory workspaceFactory = new ShapefileWorkspaceFactoryClass();
                IWorkspace shpWorkspace;
                FileInfo fileInfo = new FileInfo(this.txtImportData.Text);
                shpWorkspace = workspaceFactory.OpenFromFile(fileInfo.DirectoryName, 0);
                IFeatureClass shpFeatureClass;
                IFeatureWorkspace shpFeatureWorkspace = shpWorkspace as IFeatureWorkspace;
                shpFeatureClass = shpFeatureWorkspace.OpenFeatureClass(fileInfo.Name);

                // workspace
                UID pUID = new UIDClass();
                IFeatureWorkspace featureworkspace = workspace as IFeatureWorkspace;

                switch (shpFeatureClass.FeatureType)
                {
                    case esriFeatureType.esriFTSimple:
                        pUID.Value = "esriGeoDatabase.Feature";
                        break;
                    case esriFeatureType.esriFTSimpleJunction:
                        pUID.Value = "esriGeoDatabase.SimpleJunctionFeature";
                        break;
                    case esriFeatureType.esriFTComplexJunction:
                        pUID.Value = "esriGeoDatabase.ComplexJunctionFeature";
                        break;
                    case esriFeatureType.esriFTSimpleEdge:
                        pUID.Value = "esriGeoDatabase.SimpleEdgeFeature";
                        break;
                    case esriFeatureType.esriFTComplexEdge:
                        pUID.Value = "esriGeoDatabase.ComplexEdgeFeature";
                        break;
                }
                IFeatureClass newFeatureClass;
                // 创建数据集
                newFeatureClass = featureworkspace.CreateFeatureClass(this.txtNewName.Text, shpFeatureClass.Fields, pUID, null, shpFeatureClass.FeatureType, "shape", "");

                int featureCount = shpFeatureClass.FeatureCount(null);

                this.pbImport.Minimum = 1;
                this.pbImport.Maximum = featureCount;

                IFeature feature;
                IWorkspaceEdit workspaceEdit;
                IFeatureCursor featureCursor,insertFeatureCursor;
                workspaceEdit = workspace as IWorkspaceEdit;
                // 开始编辑
                workspaceEdit.StartEditing(true);
                workspaceEdit.StartEditOperation();
                // 导入数据
                featureCursor = shpFeatureClass.Search(null, true);
                feature = featureCursor.NextFeature();
                insertFeatureCursor = newFeatureClass.Insert(false);
                int count = 0;
                while (feature != null)
                {
                    count++;
                    this.pbImport.Value = count;

                    insertFeatureCursor.InsertFeature(feature as IFeatureBuffer);
                    feature = featureCursor.NextFeature();
                }
                // 结束编辑
                workspaceEdit.StopEditOperation();
                workspaceEdit.StopEditing(true);
                this.Close();
                this.Dispose();
            }
        }

    }
}
