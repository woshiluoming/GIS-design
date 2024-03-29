﻿using System;
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
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
namespace _luoming
{
    public partial class AttributeTableFrm : Form
    {
        public DataTable attributeTable;

        public AttributeTableFrm()
        {
            InitializeComponent();
        }

        private void AttributeTableFrm_Load(object sender, EventArgs e)
        {

        }

        /// <summary> 
        /// 根据图层字段创建一个只含字段的空 DataTable 
        /// </summary> 
        /// <param name="pLayer"></param> 
        /// <param name="tableName"></param> 
        /// <returns></returns> 
        private static DataTable CreateDataTableByLayer(ILayer pLayer, string tableName)
        {
            //创建一个 DataTable 表
            DataTable pDataTable = new DataTable(tableName);
            //取得 ITable 接口
            ITable pTable = pLayer as ITable;
            IField pField = null;
            DataColumn pDataColumn;
            //根据每个字段的属性建立 DataColumn 对象
            for (int i = 0; i < pTable.Fields.FieldCount; i++)
            {
                pField = pTable.Fields.get_Field(i);
                //新建一个 DataColumn 并设置其属性
                pDataColumn = new DataColumn(pField.Name);
                if (pField.Name == pTable.OIDFieldName)
                {
                    pDataColumn.Unique = true;//字段值是否唯一
                }
                //字段值是否允许为空
                pDataColumn.AllowDBNull = pField.IsNullable;
                //字段别名
                pDataColumn.Caption = pField.AliasName;
                //字段数据类型
                pDataColumn.DataType = System.Type.GetType(ParseFieldType(pField.Type));
                //字段默认值
                pDataColumn.DefaultValue = pField.DefaultValue;
                //当字段为 String 类型是设置字段长度
                if (pField.VarType == 8)
                {
                    pDataColumn.MaxLength = pField.Length;
                }
                //字段添加到表中
                pDataTable.Columns.Add(pDataColumn);
                pField = null;
                pDataColumn = null;
            }
            return pDataTable;
        }

        /// <summary> 
        /// 将 GeoDatabase 字段类型转换成.Net 3
        /// </summary> 
        /// <param name="fieldType">字段类型</param> 
        /// <returns></returns> 
        public static string ParseFieldType(esriFieldType fieldType)
        {
            switch (fieldType)
            {
                case esriFieldType.esriFieldTypeBlob:
                    return "System.String";
                case esriFieldType.esriFieldTypeDate:
                    return "System.DateTime";
                case esriFieldType.esriFieldTypeDouble:
                    return "System.Double";
                case esriFieldType.esriFieldTypeGeometry:
                    return "System.String";
                case esriFieldType.esriFieldTypeGlobalID:
                    return "System.String";
                case esriFieldType.esriFieldTypeGUID:
                    return "System.String";
                case esriFieldType.esriFieldTypeInteger:
                    return "System.Int32";
                case esriFieldType.esriFieldTypeOID:
                    return "System.String";
                case esriFieldType.esriFieldTypeRaster:
                    return "System.String";
                case esriFieldType.esriFieldTypeSingle:
                    return "System.Single";
                case esriFieldType.esriFieldTypeSmallInteger:
                    return "System.Int32";
                case esriFieldType.esriFieldTypeString:
                    return "System.String";
                default:
                    return "System.String";
            }
        }

        /// <summary> 
        /// 填充 DataTable 中的数据
        /// </summary> 
        /// <param name="pLayer"></param> 
        /// <param name="tableName"></param> 
        /// <returns></returns> 
        public static DataTable CreateDataTable(ILayer pLayer, string tableName)
        {
            //创建空 DataTable 
            DataTable pDataTable = CreateDataTableByLayer(pLayer, tableName);
            //取得图层类型
            string shapeType = getShapeType(pLayer);
            //创建 DataTable 的行对象
            DataRow pDataRow = null;
            //从 ILayer 查询到 ITable 
            ITable pTable = pLayer as ITable;
            ICursor pCursor = pTable.Search(null, false);
            //取得 ITable 中的行信息
            IRow pRow = pCursor.NextRow();
            int n = 0;
            while (pRow != null)
            {
                //新建 DataTable 的行对象
                pDataRow = pDataTable.NewRow();
                for (int i = 0; i < pRow.Fields.FieldCount; i++)
                {
                    //如果字段类型为 esriFieldTypeGeometry，则根据图层类型设置字段值
                    if (pRow.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                    {
                        pDataRow[i] = shapeType;
                    }
                    //当图层类型为 Anotation 时，要素类中会有 esriFieldTypeBlob 类型的数据，
                    //其存储的是标注内容，如此情况需将对应的字段值设置为 Element 
                    else if (pRow.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeBlob)
                    {
                        pDataRow[i] = "Element";
                    }
                    else
                    {
                        pDataRow[i] = pRow.get_Value(i);
                    }
                }
                //添加 DataRow 到 DataTable 
                pDataTable.Rows.Add(pDataRow);
                pDataRow = null;
                n++;
                //为保证效率，一次只装载最多条记录
                if (n == 2000)
                {
                    pRow = null;
                }
                else
                {
                    pRow = pCursor.NextRow();
                }
            }
            return pDataTable;
        }

        /// <summary> 
        /// 获得图层的 Shape 类型
        /// </summary> 
        /// <param name="pLayer">图层</param> 
        /// <returns></returns> 
        public static string getShapeType(ILayer pLayer)
        {
            IFeatureLayer pFeatLyr = (IFeatureLayer)pLayer;
            switch (pFeatLyr.FeatureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPoint:
                    return "Point";
                case esriGeometryType.esriGeometryPolyline:
                    return "Polyline";
                case esriGeometryType.esriGeometryPolygon:
                    return "Polygon";
                default:
                    return "";
            }
        }

        /// <summary> 
        /// 绑定 DataTable 到 DataGridView 
        /// </summary> 
        /// <param name="player"></param> 
        public void CreateAttributeTable(ILayer player)
        {
            string tableName;
            tableName = getValidFeatureClassName(player.Name);
            attributeTable = CreateDataTable(player, tableName);
            this.dataGridView1.DataSource = attributeTable;
            this.Text = "属性表[" + tableName + "] " + "记录数：" + attributeTable.Rows.Count.ToString();
        }

        /// <summary> 
        /// 替换数据表名中的点
        /// </summary> 
        /// <param name="FCname"></param> 
        /// <returns></returns> 
        public static string getValidFeatureClassName(string FCname)
        {
            int dot = FCname.IndexOf(".");
            if (dot != -1)
            {
                return FCname.Replace(".", "_");
            }
            return FCname;
        }
    }
}
