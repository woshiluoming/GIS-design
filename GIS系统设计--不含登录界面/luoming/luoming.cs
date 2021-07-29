using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using _luoming;

namespace _luoming
{
    public partial class Form1 : Form
    {
        private ControlsSynchronizer m_controlsSynchronizer = null;
        private ESRI.ArcGIS.Controls.IMapControl3 m_mapControl = null;
        private ESRI.ArcGIS.Controls.IPageLayoutControl2 m_pageLayoutControl = null;
        // private IMapDocument pMapDocument;

        //TOCControl 控件变量
        private ITOCControl2 m_tocControl = null;
        //TOCControl 中 Map 菜单
        private IToolbarMenu m_menuMap = null;
        //TOCControl 中图层菜单
        private IToolbarMenu m_menuLayer = null;

        // 全局个人数据库目录里
        string m_FullPath;
        // 全局数据库名
        string m_FullDatasetName;
        // 当前工作控件
        IWorkspace m_Workspace;
        // 工作空间列表
        ArrayList m_WorkspaceList = new ArrayList();
        // 当前选择节点
        TreeNode m_CurrentNode;
        // 当前图层
        IFeatureLayer m_curFeatureLayer;

        public Form1()
        {
            InitializeComponent();
            m_Workspace = null;
            refreshTree();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            m_mapControl = (IMapControl3)this.axMapControl1.Object;
            m_pageLayoutControl = (IPageLayoutControl2)this.axPageLayoutControl1.Object;

            //初始化 controls synchronization calss
            m_controlsSynchronizer = new ControlsSynchronizer(m_mapControl, m_pageLayoutControl);
            //把 MapControl 和 PageLayoutControl 绑定起来(两个都指向同一个 Map),然后设置 MapControl 为活动的 Control
            m_controlsSynchronizer.BindControls(true);
            //为了在切换 MapControl 和 PageLayoutControl 视图同步，要添加 Framework Control
            m_controlsSynchronizer.AddFrameworkControl(axToolbarControl1.Object);
            m_controlsSynchronizer.AddFrameworkControl(this.axTOCControl1.Object);
            // 添加打开命令按钮到工具条
            OpenNewMapDocument openMapDoc = new OpenNewMapDocument(m_controlsSynchronizer);
            axToolbarControl1.AddItem(openMapDoc, -1, 0, false, -1, esriCommandStyles.esriCommandStyleIconOnly);

            m_tocControl = (ITOCControl2)this.axTOCControl1.Object;

            m_menuMap = new ToolbarMenuClass();
            m_menuLayer = new ToolbarMenuClass();

            //添加自定义菜单项到 TOCCOntrol 的 Map 菜单中
            //打开文档菜单
            m_menuMap.AddItem(new OpenNewMapDocument(m_controlsSynchronizer), -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            //添加数据菜单
            m_menuMap.AddItem(new ControlsAddDataCommandClass(), -1, 1, false, esriCommandStyles.esriCommandStyleIconAndText);
            //打开全部图层菜单
            m_menuMap.AddItem(new LayerVisibility(), 1, 2, false, esriCommandStyles.esriCommandStyleTextOnly);
            //关闭全部图层菜单
            m_menuMap.AddItem(new LayerVisibility(), 2, 3, false, esriCommandStyles.esriCommandStyleTextOnly);
            //以二级菜单的形式添加内置的“选择”菜单
            m_menuMap.AddSubMenu("esriControls.ControlsFeatureSelectionMenu", 4, true);
            //以二级菜单的形式添加内置的“地图浏览”菜单
            m_menuMap.AddSubMenu("esriControls.ControlsMapViewMenu", 5, true);
            //添加自定义菜单项到 TOCCOntrol 的图层菜单中
            m_menuLayer = new ToolbarMenuClass();
            //添加“移除图层”菜单项
            m_menuLayer.AddItem(new RemoveLayer(), -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);
            //添加“放大到整个图层”菜单项
            m_menuLayer.AddItem(new ZoomToLayer(), -1, 1, true, esriCommandStyles.esriCommandStyleTextOnly);
            //设置菜单的 Hook
            m_menuLayer.SetHook(m_mapControl);
            m_menuMap.SetHook(m_mapControl);

        }

        // 新建地图命令 
        private void New_Click(object sender, EventArgs e)
        {
            //询问是否保存当前地图
            DialogResult res = MessageBox.Show(" 是否保存当前地图 ?", " 提 示 ", MessageBoxButtons.YesNo,
           MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                //如果要保存，调用另存为对话框
                ICommand command = new ControlsSaveAsDocCommandClass();
                if (m_mapControl != null)
                    command.OnCreate(m_controlsSynchronizer.MapControl.Object);
                else
                    command.OnCreate(m_controlsSynchronizer.PageLayoutControl.Object);
                command.OnClick();
            }
            //创建新的地图实例
            IMap map = new MapClass();
            map.Name = "Map";
            m_controlsSynchronizer.MapControl.DocumentFilename = string.Empty;
            //更新新建地图实例的共享地图文档
            m_controlsSynchronizer.ReplaceMap(map);
        }

        // 打开地图文档 Mxd 命令
        private void Open_Click(object sender, EventArgs e)
        {
            if (this.axMapControl1.LayerCount > 0)
            {
                DialogResult result = MessageBox.Show("是否保存当前地图？", "警告", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Cancel) return;
                if (result == DialogResult.Yes) this.Save_Click(null, null);
            }
            OpenNewMapDocument openMapDoc = new OpenNewMapDocument(m_controlsSynchronizer);
            openMapDoc.OnCreate(m_controlsSynchronizer.MapControl.Object);
            openMapDoc.OnClick();
        }

        /// 添加数据命令 
        private void AddData_Click(object sender, EventArgs e)
        {
            int currentLayerCount = this.axMapControl1.LayerCount;
            ICommand pCommand = new ControlsAddDataCommandClass();
            pCommand.OnCreate(this.axMapControl1.Object);
            pCommand.OnClick();

            IMap pMap = this.axMapControl1.Map;
            this.m_controlsSynchronizer.ReplaceMap(pMap);
        }

        // 保存地图文档命令
        private void Save_Click(object sender, EventArgs e)
        {
            // 首先确认当前地图文档是否有效
            if (null != m_pageLayoutControl.DocumentFilename && m_mapControl.CheckMxFile(m_pageLayoutControl.DocumentFilename))
            {
                // 创建一个新的地图文档实例
                IMapDocument mapDoc = new MapDocumentClass();
                // 打开当前地图文档
                mapDoc.Open(m_pageLayoutControl.DocumentFilename, string.Empty);
                // 用 PageLayout 中的文档替换当前文档中的 PageLayout 部分
                mapDoc.ReplaceContents((IMxdContents)m_pageLayoutControl.PageLayout);
                // 保存地图文档
                mapDoc.Save(mapDoc.UsesRelativePaths, false);
                mapDoc.Close();
            }
        }

        // 另存为地图文档命令 
        private void SaveAs_Click(object sender, EventArgs e)
        {
            //如果当前视图为 MapControl 时，提示用户另存为操作将丢失 PageLayoutControl 中的设置
            if (m_controlsSynchronizer.ActiveControl is IMapControl3)
            {
                if (MessageBox.Show(" 另存为地图文档将丢失制版视图的设置 \r\n 您要继续吗 ?", " 提 示 ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }
            //调用另存为命令
            ICommand command = new ControlsSaveAsDocCommandClass();
            command.OnCreate(m_controlsSynchronizer.ActiveControl);
            command.OnClick();
        }

        // 退出程序 
        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControl2.SelectedIndex == 0)
            {
                //激活 MapControl
                m_controlsSynchronizer.ActivateMap();
            }
            else
            {
                //激活 PageLayoutControl
                m_controlsSynchronizer.ActivatePageLayout();
            }
        }

        private void axToolbarControl1_OnMouseMove(object sender, IToolbarControlEvents_OnMouseMoveEvent e)
        {
            // 取得鼠标所在工具的索引号
            int index = axToolbarControl1.HitTest(e.x, e.y, false);
            if (index != -1)
            {
                // 取得鼠标所在工具的 ToolbarItem
                IToolbarItem toolbarItem = axToolbarControl1.GetItem(index);
                // 设置状态栏信息
                MessageLabel.Text = toolbarItem.Command.Message;
            }
            else
            {
                MessageLabel.Text = " 就绪 ";
            }
        }

        private void axMapControl1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            // 显示当前比例尺
            CoordinateLabel.Text = " 当前坐标 X = " + Math.Round(e.mapX, 6).ToString() + " Y = " + Math.Round(e.mapY, 6).ToString() + " " + this.axMapControl1.MapUnits.ToString().Substring(4);
        }

        private void axMapControl1_OnMapReplaced(object sender, IMapControlEvents2_OnMapReplacedEvent e)
        {
            // 当主地图显示控件的地图更换时，鹰眼中的地图也跟随更换
            this.axMapControl2.Map = new MapClass();
            // 添加主地图控件中的所有图层到鹰眼控件中
            for (int i = 1; i <= this.axMapControl1.LayerCount; i++)
            {
                this.axMapControl2.AddLayer(this.axMapControl1.get_Layer(this.axMapControl1.LayerCount - i));
            }
            // 设置 MapControl 显示范围至数据的全局范围
            this.axMapControl2.Extent = this.axMapControl1.FullExtent;
            // 刷新鹰眼控件地图
            this.axMapControl2.Refresh();

        }

        private void axMapControl1_OnExtentUpdated(object sender, IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            // 得到新范围
            IEnvelope pEnv = (IEnvelope)e.newEnvelope;
            IGraphicsContainer pGra = axMapControl2.Map as IGraphicsContainer;
            IActiveView pAv = pGra as IActiveView;
            // 在绘制前，清除 axMapControl2 中的任何图形元素
            pGra.DeleteAllElements();
            IRectangleElement pRectangleEle = new RectangleElementClass();
            IElement pEle = pRectangleEle as IElement;
            pEle.Geometry = pEnv;
            // 设置鹰眼图中的红线框
            IRgbColor pColor = new RgbColorClass();
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
            pColor.Transparency = 255;
            // 产生一个线符号对象
            ILineSymbol pOutline = new SimpleLineSymbolClass();
            pOutline.Width = 2;
            pOutline.Color = pColor;
            // 设置颜色属性
            pColor = new RgbColorClass();
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
            pColor.Transparency = 0;
            // 设置填充符号的属性
            IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            pFillSymbol.Color = pColor;
            pFillSymbol.Outline = pOutline;
            IFillShapeElement pFillShapeEle = pEle as IFillShapeElement;
            pFillShapeEle.Symbol = pFillSymbol;
            pGra.AddElement((IElement)pFillShapeEle, 0);
            // 刷新
            pAv.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        private void axMapControl2_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (this.axMapControl2.Map.LayerCount != 0)
            {
                // 按下鼠标左键移动矩形框
                if (e.button == 1)
                {
                    IPoint pPoint = new PointClass();
                    pPoint.PutCoords(e.mapX, e.mapY);
                    IEnvelope pEnvelope = this.axMapControl1.Extent;
                    pEnvelope.CenterAt(pPoint);
                    this.axMapControl1.Extent = pEnvelope;
                    this.axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                }
                // 按下鼠标右键绘制矩形框
                else if (e.button == 2)
                {
                    IEnvelope pEnvelop = this.axMapControl2.TrackRectangle();
                    this.axMapControl1.Extent = pEnvelop;
                    this.axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                }
            }
        }

        private void axMapControl2_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            // 如果不是左键按下就直接返回
            if (e.button != 1) return;
            IPoint pPoint = new PointClass();
            pPoint.PutCoords(e.mapX, e.mapY);
            this.axMapControl1.CenterAt(pPoint);
            this.axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
        }

        private void axTOCControl1_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {
            //如果不是右键按下直接返回
            if (e.button != 2) return;
            esriTOCControlItem item = esriTOCControlItem.esriTOCControlItemNone;
            IBasicMap map = null;
            ILayer layer = null;
            object other = null;
            object index = null;
            //判断所选菜单的类型
            m_tocControl.HitTest(e.x, e.y, ref item, ref map, ref layer, ref other, ref index);
            //确定选定的菜单类型，Map 或是图层菜单
            if (item == esriTOCControlItem.esriTOCControlItemMap)
                m_tocControl.SelectItem(map, null);
            else
                m_tocControl.SelectItem(layer, null);
            //设置 CustomProperty 为 layer (用于自定义的 Layer 命令)
            m_mapControl.CustomProperty = layer;
            //弹出右键菜单
            if (item == esriTOCControlItem.esriTOCControlItemMap)
                m_menuMap.PopupMenu(e.x, e.y, m_tocControl.hWnd);
            if (item == esriTOCControlItem.esriTOCControlItemLayer)
                m_menuLayer.PopupMenu(e.x, e.y, m_tocControl.hWnd);

            //弹出右键菜单
            if (item == esriTOCControlItem.esriTOCControlItemMap)
                m_menuMap.PopupMenu(e.x, e.y, m_tocControl.hWnd);
            if (item == esriTOCControlItem.esriTOCControlItemLayer)
            {
                //动态添加 OpenAttributeTable 菜单项
                m_menuLayer.AddItem(new OpenAttributeTable(layer), -1, 2, true, esriCommandStyles.esriCommandStyleTextOnly);
                m_menuLayer.PopupMenu(e.x, e.y, m_tocControl.hWnd);
                //移除 OpenAttributeTable 菜单项，以防止重复添加
                m_menuLayer.Remove(2);
            }
        }

        /// <summary>
        /// 主地图控件的右键响应函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (e.button == 2)
            {
                //弹出右键菜单
                m_menuMap.PopupMenu(e.x, e.y, m_mapControl.hWnd);
            }
        }

        /// <summary> 
        /// 双击 TOCControl 控件时触发的事件
        /// </summary> 
        /// <param name="sender"></param> 
        /// <param name="e"></param> 
        private void axTOCControl1_OnDoubleClick(object sender, ITOCControlEvents_OnDoubleClickEvent e)
        {
            esriTOCControlItem itemType = esriTOCControlItem.esriTOCControlItemNone;
            IBasicMap basicMap = null;
            ILayer layer = null;
            object unk = null;
            object data = null;
            axTOCControl1.HitTest(e.x, e.y, ref itemType, ref basicMap, ref layer, ref unk, ref data);
            if (e.button == 1)
            {
                if (itemType == esriTOCControlItem.esriTOCControlItemLegendClass)
                {
                    //取得图例
                    ILegendClass pLegendClass = ((ILegendGroup)unk).get_Class((int)data);
                    //创建符号选择器 SymbolSelector 实例
                    SymbolSelectorFrm SymbolSelectorFrm = new SymbolSelectorFrm(pLegendClass, layer);
                    if (SymbolSelectorFrm.ShowDialog() == DialogResult.OK)
                    {
                        //局部更新主 Map 控件
                        m_mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                        //设置新的符号
                        pLegendClass.Symbol = SymbolSelectorFrm.pSymbol;
                        //更新主 Map 控件和图层控件
                        this.axMapControl1.ActiveView.Refresh();
                        this.axTOCControl1.Refresh();
                    }
                }
            }
        }

        private void 打开数据库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = "个人空间数据库";
            this.openFileDialog1.Filter = "geodatabase(*.mdb)|*.mdb";
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                String fileName = this.openFileDialog1.FileName;
                FileInfo fileInfo = new FileInfo(fileName);
                m_FullPath = fileInfo.Name;
                m_FullDatasetName = fileInfo.Name;
                IWorkspaceFactory workspaceFactory = new AccessWorkspaceFactoryClass();
                IWorkspace workspace = workspaceFactory.OpenFromFile(fileName, 0);
                if (workspace != null)
                {
                    m_Workspace = workspace;
                    m_WorkspaceList.Add(workspace);
                }
            }
            refreshTree();
        }

        private void 创建数据库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateDatabase createDatabase = new CreateDatabase();
            createDatabase.ShowDialog();
            m_FullDatasetName = createDatabase.FileName;
            m_FullPath = createDatabase.FilePath;
            m_Workspace = createDatabase.NewWorkspace;
            m_WorkspaceList.Add(m_Workspace);
            refreshTree();
        }

        // 刷新工作空间树
        private void refreshTree()
        {
            this.treeView1.Nodes.Clear();
            TreeNode rootNode;
            for (int i = 0; i < m_WorkspaceList.Count; i++)
            {
                IWorkspace workspace = m_WorkspaceList[i] as IWorkspace;
                FileInfo fileInfo = new FileInfo(workspace.PathName);
                string fileName = fileInfo.Name;
                rootNode = new TreeNode();
                rootNode.Tag = workspace.PathName;
                rootNode.Name = fileName.Substring(0, fileName.LastIndexOf('.'));
                rootNode.Text = fileName.Substring(0, fileName.LastIndexOf('.'));
                IEnumDatasetName enumDatasetName = workspace.get_DatasetNames(esriDatasetType.esriDTFeatureClass);
                enumDatasetName.Reset();
                IDatasetName datasetName = enumDatasetName.Next();
                while (datasetName != null)
                {
                    TreeNode childNode = new TreeNode();
                    childNode.Text = datasetName.Name;
                    rootNode.Nodes.Add(childNode);
                    datasetName = enumDatasetName.Next();
                }
                this.treeView1.Nodes.Add(rootNode);
                this.treeView1.ExpandAll();
            }
        }

        // 选择树控件节点后，在地图控件中显示当前的图层
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode tempNode = new TreeNode();
            IWorkspace workspace = null;
            IEnumDataset enumDataset = null;
            IDataset dataset = null;
            IFeatureLayer featureLayer = new FeatureLayerClass();
            int i;
            m_CurrentNode = e.Node;
            if (e.Node.Parent != null)
            {
                for (i = 0; i < m_WorkspaceList.Count; i++)
                {
                    workspace = m_WorkspaceList[i] as IWorkspace;
                    if (workspace.PathName.ToString() == e.Node.Parent.Tag.ToString())
                    {
                        break;
                    }
                }
                if (i < m_WorkspaceList.Count)
                {
                    enumDataset = workspace.get_Datasets(esriDatasetType.esriDTFeatureClass);
                    enumDataset.Reset();
                    dataset = enumDataset.Next();
                    while (dataset != null)
                    {
                        if (dataset.Name == e.Node.Text)
                        {
                            featureLayer.FeatureClass = dataset as IFeatureClass;
                            m_curFeatureLayer = featureLayer;
                            break;
                        }
                        dataset = enumDataset.Next();
                    }
                }
            }
            this.axMapControl1.ClearLayers();
            this.axMapControl1.AddLayer(featureLayer as ILayer);
            this.axMapControl1.Extent = this.axMapControl1.FullExtent;
        }

        private void 连接数据库ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //定义工作空间，工作空间的数据源来自SDE，IWorkspaceFactory是Geodatabase的入口
            Type factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
            IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(factoryType);

            //通过IPropertySet设置通过SDE连接数据库的各种参数
            IPropertySet propertySet = new PropertySet();
            propertySet.SetProperty("SERVER", "DESKTOP-NOHMKV3");
            propertySet.SetProperty("INSTANCE", "sde:sqlserver:DESKTOP-NOHMKV3");
            propertySet.SetProperty("DATABASE", "luomingdatabase");
            propertySet.SetProperty("USER", "sa");
            propertySet.SetProperty("PASSWORD", "123456");
            propertySet.SetProperty("VERSION", "SDE.DEFAULT");
            // propertySet.SetProperty("AUTHENTICATION_MODE", "DBMS");

            IWorkspace workspace = null;
            //通过以上设置的参数将数据库的数据通过SDE读入工作空间
            try
            {
                workspace = workspaceFactory.Open(propertySet, 0);
                MessageBox.Show("数据库连接成功！");
            }
            catch
            {
                MessageBox.Show("数据库创建失败，请重新创建");
            }

            //将工作空间中的所有数据存入Enum数据集，这些数据中可能有FeatureDataSet、FeatureClass等多种数据类型
            IEnumDataset enumDataSet = workspace.get_Datasets(esriDatasetType.esriDTAny);
            enumDataSet.Reset();

            //再把Enum数据集中的数据一个个读到DataSet中
            IDataset dataSet;
            dataSet = enumDataSet.Next();

            //判断数据集是否有数据
            while (dataSet != null)
            {
                //判断数据集中的数据是什么类型
                if (dataSet is IFeatureDataset)
                {
                    //如果是FeatureDataSet做以下处理
                    IFeatureWorkspace featureWorkspace = workspace as IFeatureWorkspace;
                    IFeatureDataset featureDataset = featureWorkspace.OpenFeatureDataset(dataSet.Name);
                    IEnumDataset pEnumDataset1 = featureDataset.Subsets;
                    pEnumDataset1.Reset();
                    IDataset pDataset1 = pEnumDataset1.Next();

                    if (pDataset1 is IFeatureClass)
                    {
                        IFeatureLayer pFeatureLayer = new FeatureLayerClass();
                        pFeatureLayer.FeatureClass = featureWorkspace.OpenFeatureClass(pDataset1.Name);
                        pFeatureLayer.Name = pFeatureLayer.FeatureClass.AliasName;
                        axMapControl1.Map.AddLayer(pFeatureLayer);
                        axMapControl1.ActiveView.Refresh();
                    }
                }
                else if (dataSet is IFeatureClass)
                {
                    //如果是FeatureClass做以下处理
                    IFeatureWorkspace featureWorkspace = workspace as IFeatureWorkspace;
                    IFeatureClass feature = featureWorkspace.OpenFeatureClass(dataSet.Name);
                    IFeatureLayer layer = new FeatureLayerClass();
                    layer.FeatureClass = feature;
                    layer.Name = feature.AliasName;
                    axMapControl1.AddLayer(layer);
                }
                dataSet = enumDataSet.Next();
            }

            axMapControl1.Refresh();
            MessageBox.Show("数据读取完毕！");
        }

        private void 数据入库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importdata fImportdata = new importdata();
            fImportdata.CurWorkspaceList = m_WorkspaceList;
            fImportdata.ShowDialog();
            refreshTree();
        }

        private void 导出地图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //参数定义      
            IHookHelper layout_hookHelper = new HookHelperClass();
            //参数赋值  
            layout_hookHelper.Hook = this.axPageLayoutControl1.Object;

            frmResource EMFrm = new frmResource(layout_hookHelper);
            EMFrm.ShowDialog(); 
        }
    }
}