using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;

namespace _luoming
{
    /// <summary>
    /// This class is used to synchronize a gven PageLayoutControl and a MapControl.
    /// When initialized, the user must pass the reference of these control to the class, bind
    /// the control together by calling 'BindControls' which in turn sets a joined Map referenced
    /// by both control; and set all the buddy controls joined between these two controls.
    /// When alternating between the MapControl and PageLayoutControl, you should activate the visible control
    /// and deactivate the other by calling ActivateXXX.
    /// This calss is limited to a situation where the controls are not simultaneously visible.
    /// </summary>
    public class ControlsSynchronizer
    {
        #region class members
        private IMapControl3 m_mapControl = null;
        private IPageLayoutControl2 m_pageLayoutControl = null;
        private ITool m_mapActiveTool = null;
        private ITool m_pageLayoutActiveTool = null;
        private bool m_IsMapCtrlactive = true;
        private ArrayList m_frameworkControls = null;
        #endregion
        #region constructor

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ControlsSynchronizer()
        {
            //初始化 ArrayList
            m_frameworkControls = new ArrayList();
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mapControl"></param>
        /// <param name="pageLayoutControl"></param>
        public ControlsSynchronizer(IMapControl3 mapControl, IPageLayoutControl2 pageLayoutControl)
            : this()
        {
            //为类成员赋值
            m_mapControl = mapControl;
            m_pageLayoutControl = pageLayoutControl;
        }
        #endregion
        #region properties
        /// <summary>
        /// 取得或设置 MapControl
        /// </summary>
        public IMapControl3 MapControl
        {
            get { return m_mapControl; }
            set { m_mapControl = value; }
        }
        /// <summary>
        /// 取得或设置 PageLayoutControl
        /// </summary>
        public IPageLayoutControl2 PageLayoutControl
        {
            get { return m_pageLayoutControl; }
            set { m_pageLayoutControl = value; }
        }
        /// <summary>
        /// 取得当前 ActiveView 的类型
        /// </summary>
        public string ActiveViewType
        {
            get
            {
                if (m_IsMapCtrlactive)
                    return "MapControl";
                else
                    return "PageLayoutControl";
            }
        }
        /// <summary>
        /// 取得当前活动的 Control
        /// </summary>
        public object ActiveControl
        {
            get
            {
                if (m_mapControl == null || m_pageLayoutControl == null)
                    throw new Exception("ControlsSynchronizer::ActiveControl:\r\nEither MapControl or PageLayoutControl are not initialized!");
                if (m_IsMapCtrlactive)
                    return m_mapControl.Object;
                else
                    return m_pageLayoutControl.Object;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 激活 MapControl 并解除 the PagleLayoutControl
        /// </summary>
        public void ActivateMap()
        {
            try
            {
                if (m_pageLayoutControl == null || m_mapControl == null)
                    throw new Exception("ControlsSynchronizer::ActivateMap:\r\nEither MapControl or PageLayoutControl are not initialized!");
                //缓存当前 PageLayout 的 CurrentTool 
                if (m_pageLayoutControl.CurrentTool != null) m_pageLayoutActiveTool = m_pageLayoutControl.CurrentTool;
                //解除 PagleLayout
                m_pageLayoutControl.ActiveView.Deactivate();
                //激活 MapControl
                m_mapControl.ActiveView.Activate(m_mapControl.hWnd);
                //将之前 MapControl 最后使用的 tool，作为活动的 tool，赋给 MapControl 的 CurrentTool
                if (m_mapActiveTool != null) m_mapControl.CurrentTool = m_mapActiveTool;
                m_IsMapCtrlactive = true;
                //为每一个的 framework controls,设置 Buddy control 为 MapControl
                this.SetBuddies(m_mapControl.Object);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("ControlsSynchronizer::ActivateMap:\r\n{0}", ex.Message));
            }
        }

        /// <summary>
        /// 激活 PagleLayoutControl 并减活 MapCotrol
        /// </summary>
        public void ActivatePageLayout()
        {
            try
            {
                if (m_pageLayoutControl == null || m_mapControl == null)
                    throw new Exception("ControlsSynchronizer::ActivatePageLayout:\r\nEither MapControl or PageLayoutControl are not initialized!");
                //缓存当前 MapControl 的 CurrentTool
                if (m_mapControl.CurrentTool != null) m_mapActiveTool = m_mapControl.CurrentTool;
                //解除 MapControl
                m_mapControl.ActiveView.Deactivate();
                //激活 PageLayoutControl
                m_pageLayoutControl.ActiveView.Activate(m_pageLayoutControl.hWnd);
                //将之前 PageLayoutControl 最后使用的 tool，作为活动的 tool，赋给 PageLayoutControl 的 CurrentTool
                if (m_pageLayoutActiveTool != null) m_pageLayoutControl.CurrentTool = m_pageLayoutActiveTool;
                m_IsMapCtrlactive = false;
                //为每一个的 framework controls,设置 Buddy control 为 PageLayoutControl
                this.SetBuddies(m_pageLayoutControl.Object);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("ControlsSynchronizer::ActivatePageLayout:\r\n{0}", ex.Message));
            }
        }

        /// <summary>
        /// 给予一个地图, 置换 PageLayoutControl 和 MapControl 的 focus map
        /// </summary>
        /// <param name="newMap"></param>
        public void ReplaceMap(IMap newMap)
        {
            if (newMap == null)
                throw new Exception("ControlsSynchronizer::ReplaceMap:\r\nNew map for replacement is not initialized!");
            if (m_pageLayoutControl == null || m_mapControl == null)
                throw new Exception("ControlsSynchronizer::ReplaceMap:\r\nEither MapControl or PageLayoutControl are not initialized!");
            //create a new instance of IMaps collection which is needed by the PageLayout
            //创建一个 PageLayout 需要用到的,新的 IMaps collection 的实例
            IMaps maps = new Maps();
            //add the new map to the Maps collection
            //把新的地图加到 Maps collection 里头去
            maps.Add(newMap);
            bool bIsMapActive = m_IsMapCtrlactive;
            //call replace map on the PageLayout in order to replace the focus map
            //we must call ActivatePageLayout, since it is the control we call 'ReplaceMaps'
            //调用 PageLayout 的 replace map 来置换 focus map
            //我们必须调用 ActivatePageLayout,因为它是那个我们可以调用"ReplaceMaps"的 Control
            this.ActivatePageLayout();
            m_pageLayoutControl.PageLayout.ReplaceMaps(maps);
            //assign the new map to the MapControl
            //把新的地图赋给 MapControl
            m_mapControl.Map = newMap;
            //reset the active tools
            //重设 active tools
            m_pageLayoutActiveTool = null;
            m_mapActiveTool = null;
            //make sure that the last active control is activated
            //确认之前活动的 control 被激活
            if (bIsMapActive)
            {
                this.ActivateMap();
                m_mapControl.ActiveView.Refresh();
            }
            else
            {
                this.ActivatePageLayout();
                m_pageLayoutControl.ActiveView.Refresh();
            }
        }
        /// <summary>
        /// bind the MapControl and PageLayoutControl together by assigning a new joint focus map
        /// 指定共同的 Map 来把 MapControl 和 PageLayoutControl 绑在一起
        /// </summary>
        /// <param name="mapControl"></param>
        /// <param name="pageLayoutControl"></param>
        /// <param name="activateMapFirst">true if the MapControl supposed to be activated first,如果 MapControl 被首先激活,则为 true</param>
        public void BindControls(IMapControl3 mapControl, IPageLayoutControl2 pageLayoutControl, bool activateMapFirst)
        {
            if (mapControl == null || pageLayoutControl == null)
                throw new Exception("ControlsSynchronizer::BindControls:\r\nEither MapControl or PageLayoutControl are not initialized!");
            m_mapControl = MapControl;
            m_pageLayoutControl = pageLayoutControl;
            this.BindControls(activateMapFirst);
        }
        /// <summary>
        /// bind the MapControl and PageLayoutControl together by assigning a new joint focus map
        /// 指定共同的 Map 来把 MapControl 和 PageLayoutControl 绑在一起
        /// </summary>
        /// <param name="activateMapFirst">true if the MapControl supposed to be activated first,如果 MapControl 被首先激活,则为 true</param>
        public void BindControls(bool activateMapFirst)
        {
            if (m_pageLayoutControl == null || m_mapControl == null)
                throw new Exception("ControlsSynchronizer::BindControls:\r\nEither MapControl or PageLayoutControl are not initialized!");
            //create a new instance of IMap
            //创造 IMap 的一个实例
            IMap newMap = new MapClass();
            newMap.Name = "Map";
            //create a new instance of IMaps collection which is needed by the PageLayout
            //创造一个新的 IMaps collection 的实例,这是 PageLayout 所需要的
            IMaps maps = new Maps();
            //add the new Map instance to the Maps collection
            //把新的 Map 实例赋给 Maps collection
            maps.Add(newMap);
            //call replace map on the PageLayout in order to replace the focus map
            //调用 PageLayout 的 replace map 来置换 focus map
            m_pageLayoutControl.PageLayout.ReplaceMaps(maps);
            //assign the new map to the MapControl
            //把新的 map 赋给 MapControl
            m_mapControl.Map = newMap;
            //reset the active tools
            //重设 active tools
            m_pageLayoutActiveTool = null;
            m_mapActiveTool = null;
            //make sure that the last active control is activated
            //确定最后活动的 control 被激活
            if (activateMapFirst)
                this.ActivateMap();
            else
                this.ActivatePageLayout();
        }
        /// <summary>
        ///by passing the application's toolbars and TOC to the synchronization class, it saves you the
        ///management of the buddy control each time the active control changes. This method ads the framework
        ///control to an array; once the active control changes, the class iterates through the array and
        ///calles SetBuddyControl on each of the stored framework control.
        /// </summary>
        /// <param name="control"></param>
        public void AddFrameworkControl(object control)
        {
            if (control == null)
                throw new Exception("ControlsSynchronizer::AddFrameworkControl:\r\nAdded control is not initialized!");
            m_frameworkControls.Add(control);
        }
        /// <summary>
        /// Remove a framework control from the managed list of controls
        /// </summary>
        /// <param name="control"></param>
        public void RemoveFrameworkControl(object control)
        {
            if (control == null)
                throw new Exception("ControlsSynchronizer::RemoveFrameworkControl:\r\nControl to be removed is not initialized!");
            m_frameworkControls.Remove(control);
        }
        /// <summary>
        /// Remove a framework control from the managed list of controls by specifying its index in the list
        /// </summary>
        /// <param name="index"></param>
        public void RemoveFrameworkControlAt(int index)
        {
            if (m_frameworkControls.Count < index)
                throw new Exception("ControlsSynchronizer::RemoveFrameworkControlAt:\r\nIndex is out of range!");
            m_frameworkControls.RemoveAt(index);
        }
        /// <summary>
        /// when the active control changes, the class iterates through the array of the framework controls
        /// and calles SetBuddyControl on each of the controls.
        /// </summary>
        /// <param name="buddy">the active control</param>
        private void SetBuddies(object buddy)
        {
            try
            {
                if (buddy == null)
                    throw new Exception("ControlsSynchronizer::SetBuddies:\r\nTarget Buddy Control is not initialized!");
                foreach (object obj in m_frameworkControls)
                {
                    if (obj is IToolbarControl)
                    {
                        ((IToolbarControl)obj).SetBuddyControl(buddy);
                    }
                    else if (obj is ITOCControl)
                    {
                        ((ITOCControl)obj).SetBuddyControl(buddy);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("ControlsSynchronizer::SetBuddies:\r\n{0}", ex.Message));
            }
        }
        #endregion
    }
}