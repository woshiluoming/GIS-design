using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using System.Text.RegularExpressions;
using ESRI.ArcGIS.Controls;

namespace _luoming
{
    public partial class frmResource : Form
    {
        private double pWidth, pHeight;
        private IActiveView pActiveView = null;
        public frmResource()
        {
            InitializeComponent();
        }
        public frmResource(IHookHelper hookHelper)
        {
            InitializeComponent();
            pActiveView = hookHelper.ActiveView;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.saveMapFileDialog.ShowDialog();
            this.txtFileName.Text = saveMapFileDialog.FileName;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (!IsNumbericA(txtWidth.Text))
            {
                MessageBox.Show("请输入数字！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtWidth.Focus();
                return;
            }
            if (!IsNumbericA(txtHeight.Text))
            {
                MessageBox.Show("请输入数字！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtHeight.Focus();
                return;
            }
            if (txtResolution.Value > 1)
            {
                ExportTool();
            }
            this.Cursor = Cursors.Default;
        }

        private void ExportTool()
        {
            IExport pExport = null;
            //           IExportJPEG pExportFormat;  
            IWorldFileSettings pWorldFile = null;
            IExportImage pExportType;
            IEnvelope pDriverBounds = null;
            //         int lScreenResolution ;  
            ESRI.ArcGIS.esriSystem.tagRECT userRECT = new ESRI.ArcGIS.esriSystem.tagRECT();
            IEnvelope pEnv = new EnvelopeClass();
            //          double dWidth;  
            //          double dHeight;  
            int lResolution;
            lResolution = Convert.ToInt32(this.txtResolution.Value);
            switch (this.saveMapFileDialog.Filter.ToString().Trim().Substring(0,3))
            {
                case "jpg":
                    pExport = new ExportJPEGClass();
                    break;
                case "bmp":
                    pExport = new ExportBMPClass();
                    break;
                case "gif":
                    pExport = new ExportGIFClass();
                    break;
                case "tif":
                    pExport = new ExportTIFFClass();
                    break;
                case "png":
                    pExport = new ExportPNGClass();
                    break;
                case "emf":
                    pExport = new ExportEMFClass();
                    break;
                case "pdf":
                    pExport = new ExportPDFClass();
                    break;
                case ".ai":
                    pExport = new ExportAIClass();
                    break;
                case "svg":
                    pExport = new ExportSVGClass();
                    break;
                default:
                    pExport = new ExportJPEGClass();
                    break;
            }

            if (this.txtFileName.Text.ToString().Trim() != "")
            {
                if (System.IO.File.Exists(this.txtFileName.Text.ToString()) == true)
                {
                    MessageBox.Show("该文件已经存在，请重新命名！");
                    this.txtFileName.Text = "";
                    this.txtFileName.Focus();
                }
                else
                {
                    pExport.ExportFileName = this.txtFileName.Text;
                    pExport.Resolution = lResolution;
                    pExportType = pExport as IExportImage;
                    pExportType.ImageType = esriExportImageType.esriExportImageTypeTrueColor;
                    pEnv = pActiveView.Extent;
                    pWorldFile = (IWorldFileSettings)pExport;
                    pWorldFile.MapExtent = pEnv;
                    pWorldFile.OutputWorldFile = false;
                    userRECT.top = 0;
                    userRECT.left = 0;
                    userRECT.right = Convert.ToInt32(pWidth);
                    userRECT.bottom = Convert.ToInt32(pHeight);
                    pDriverBounds = new EnvelopeClass();
                    pDriverBounds.PutCoords(userRECT.top, userRECT.bottom, userRECT.right, userRECT.top);
                    pExport.PixelBounds = pDriverBounds;

                    ITrackCancel pTrackCancel = new TrackCancelClass();
                    pActiveView.Output(pExport.StartExporting(), lResolution, ref userRECT, pActiveView.Extent, pTrackCancel);
                    pExport.FinishExporting();
                    MessageBox.Show("打印图片保存成功!", "保存", MessageBoxButtons.OK);
                    this.Close();

                }

            }
            else
            {
                MessageBox.Show("请保存文件!");
            }

        }

        public IActiveView ResActiveView
        {
            get
            {
                return pActiveView;
            }
            set
            {
                pActiveView = value;
            }
        }

        private void txtResolution_ValueChanged(object sender, EventArgs e)
        {
            cmbPageSize_SelectedValueChanged(null, null);
            if (this.radioButton3.Checked == true)
            {
                this.txtWidth.Text = pWidth.ToString(".00");
                this.txtHeight.Text = pHeight.ToString(".00");
            }

        }

        private void cmbPageSize_SelectedValueChanged(object sender, EventArgs e)
        {
            switch (cmbPageSize.Text)
            {
                case "自定义大小":
                    txtWidth.Focus();
                    break;
                case "A4":
                    showWH(21, 29.7);
                    break;
                case "A3":
                    showWH(29.7, 42);
                    break;
                case "A2":
                    showWH(42, 59.4);
                    break;
                case "A1":
                    showWH(59.4, 84.1);
                    break;
                case "A0":
                    showWH(84.1, 118.9);
                    break;
            }
        }

        //显示宽度和高度  
        //传入的参数必须是厘米  
        private void showWH(double pW, double pH)
        {
            pWidth = Convert.ToDouble((pW / 2.54) * Convert.ToDouble(txtResolution.Value));
            pHeight = Convert.ToDouble((pH / 2.54) * Convert.ToDouble(txtResolution.Value));
            if (this.radioButton1.Checked == true)
            {
                this.txtWidth.Text = Convert.ToDouble(pW / 2.54).ToString(".00");
                this.txtHeight.Text = Convert.ToDouble(pH / 2.54).ToString(".00");
            }
            else if (this.radioButton2.Checked == true)
            {
                this.txtWidth.Text = pW.ToString(".00");
                this.txtHeight.Text = pH.ToString(".00");
            }
            else
            {
                this.txtWidth.Text = pW.ToString(".00");
                this.txtHeight.Text = pH.ToString(".00");
            }
        }

        private void txtWidth_TextChanged(object sender, EventArgs e)
        {
            //if (!IsNumbericA(txtWidth.Text))  
            //{  
            //    MessageBox.Show("请输入数字！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);  
            //    return;  
            //}  
            if (this.radioButton1.Checked == true)
            {
                pWidth = Convert.ToDouble(txtWidth.Text) * Convert.ToDouble(txtResolution.Value);

            }
            else if (this.radioButton2.Checked == true)
            {
                pWidth = Convert.ToDouble(Convert.ToDouble(txtWidth.Text) / 2.54) * Convert.ToDouble(txtResolution.Value);

            }
            else
            {
                pWidth = Convert.ToDouble(txtWidth.Text);
            }
        }

        private void txtHeight_TextChanged(object sender, EventArgs e)
        {
            //if (!IsNumbericA(txtHeight.Text))  
            //{  
            //    MessageBox.Show("请输入数字！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);  
            //    return;  
            //}  

            if (this.radioButton1.Checked == true)
            {
                pHeight = Convert.ToDouble(txtHeight.Text) * Convert.ToDouble(txtResolution.Value);

            }
            else if (this.radioButton2.Checked == true)
            {
                pHeight = Convert.ToDouble(Convert.ToDouble(txtHeight.Text) / 2.54) * Convert.ToDouble(txtResolution.Value);

            }
            else
            {
                pHeight = Convert.ToDouble(txtHeight.Text);
            }
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            this.txtWidth.Text = Convert.ToDouble(pWidth / Convert.ToDouble(txtResolution.Value)).ToString(".00");
            this.txtHeight.Text = Convert.ToDouble(pHeight / Convert.ToDouble(txtResolution.Value)).ToString(".00");
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            this.txtWidth.Text = Convert.ToDouble(2.54 * pWidth / Convert.ToDouble(txtResolution.Value)).ToString(".00");
            this.txtHeight.Text = Convert.ToDouble(2.54 * pHeight / Convert.ToDouble(txtResolution.Value)).ToString(".00");
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            this.txtWidth.Text = pWidth.ToString(".00");
            this.txtHeight.Text = pHeight.ToString(".00");
        }

        #region 通用函数
        /// <summary>  
        /// 是否大于０的数字  
        /// </summary>  
        /// <param name="v"></param>  
        /// <returns></returns>  
        private bool IsNumbericA(string v)
        {
            return ((this.IsIntegerA(v)) || (this.IsFloatA(v)));
        }
        /// <summary>  
        /// 是否正浮点数  
        /// </summary>  
        /// <param name="v"></param>  
        /// <returns></returns>  
        private bool IsFloatA(string v)
        {
            string pattern = @"^[1-9]\d*\.\d*|0\.\d*[1-9]\d*$";
            Regex reg = new Regex(pattern);
            return reg.IsMatch(v);
        }
        /// <summary>  
        /// 是否正整数  


        /// </summary>  
        /// <param name="v"></param>  
        /// <returns></returns>  
        private bool IsIntegerA(string v)
        {
            string pattern = @"^[0-9]*[1-9][0-9]*$";
            Regex reg = new Regex(pattern);
            return reg.IsMatch(v);
        }
        #endregion

        private void frmResource_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
    }
}
