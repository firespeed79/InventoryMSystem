using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using httpHelper;
using Inventory;
using System.Diagnostics;

namespace InventoryMSystem
{
    public partial class frmInventory : Form
    {
        InventoryCtl ctlInventory = new InventoryCtl();
        rfidOperateUnitStopInventoryTag operateUnitStopGetTag = new rfidOperateUnitStopInventoryTag();
        rfidOperateUnitInventoryTag operateUnitStartGetTag = new rfidOperateUnitInventoryTag();
        InvokeDic _UpdateList = new InvokeDic();

        bool bGettingTag = false;//是否正在获取标签
        Timer __timer;
        string __lastTagTimeStamp = string.Empty;


        public frmInventory()
        {
            InitializeComponent();
            this.lblEqual.Text = string.Empty;
            this.lblLess.Text = string.Empty;
            this.lblMore.Text = string.Empty;
            this.Shown += new EventHandler(frmInventory_Shown);
            this.FormClosing += new FormClosingEventHandler(frmInventory_FormClosing);
            this.operateUnitStartGetTag.registeCallback(new deleRfidOperateCallback(UpdateEpcList));


            __timer = new Timer();
            __timer.Interval = 1000;
            __timer.Tick += new EventHandler(__timer_Tick);

        }
        //定时获取扫描到的标签
        void __timer_Tick(object sender, EventArgs e)
        {
            scanTagPara para = new scanTagPara(InventoryCommand.盘点, this.__lastTagTimeStamp);
            string jsonString = fastJSON.JSON.Instance.ToJSON(para);
            HttpWebConnect helper = new HttpWebConnect();
            helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_getScanTags);
            string url = RestUrl.getScanedTags;
            helper.TryPostData(url, jsonString);
        }
        void helper_RequestCompleted_getScanTags(object o)
        {
            string strTags = (string)o;
            Debug.WriteLine(
                string.Format("frmInventory.helper_RequestCompleted_getScanTags  ->  = {0}"
                , strTags));
            object olist = fastJSON.JSON.Instance.ToObjectList(strTags, typeof(List<tagID>), typeof(tagID));
            deleControlInvoke dele = delegate(object ol)
            {
                List<tagID> tagList = (List<tagID>)ol;
                if (tagList.Count > 0)
                {
                    tagID t = tagList[tagList.Count - 1];
                    this.__lastTagTimeStamp = t.startTime;
                    for (int i = 0; i < tagList.Count; i++)
                    {
                        tagID temp = tagList[i];
                        LinkEPCToProduct(temp.tag);
                    }
                }
            };
            this.Invoke(dele, olist);
        }

        void frmInventory_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.beforeClosing();
        }
        void UpdateEpcList(object o)
        {
            operateMessage msg = (operateMessage)o;
            if (msg.status == "fail")
            {
                MessageBox.Show("出现错误：" + msg.message);
                this.bGettingTag = false;
                this.btnScan.Text = "开始盘点";
                return;
            }
            string value = msg.message;
            //把读取到的标签epc与产品的进行关联
            if (this.dgvProductInfo.InvokeRequired)
            {
                this.dgvProductInfo.Invoke(new deleUpdateContorl(LinkEPCToProduct), value);
            }
            else
                this.LinkEPCToProduct(value);
        }
        void LinkEPCToProduct(string epc)
        {
            if (!_UpdateList.CheckItem("LinkEPCToProduct"))
            {
                return;
            }
            _UpdateList.SetItem("LinkEPCToProduct", false);


            //根据传入的id按照以下思路处理
            // 首先在当前列表中查找是否含有该id，如果有，则账物相符加1，盘亏减1
            // 如果未在列表中找到该id，说明是盘盈，需要在列表中添加该产品信息，并且盘盈加1
            DataGridViewRowCollection rows = this.dgvProductInfo.Rows;
            bool bFind = false;
            foreach (DataGridViewRow vr in rows)
            {
                DataGridViewCell cepc = (DataGridViewCell)vr.Cells[1];
                if (((string)cepc.Value) == epc)
                {
                    DataGridViewCheckBoxCell cbc = (DataGridViewCheckBoxCell)vr.Cells[0];
                    if ((LightStatus)cbc.Value == LightStatus.TurnedOff)
                    {
                        cbc.Value = LightStatus.TurnedOn;
                        int ncrtEqul = int.Parse(this.lblEqual.Text);
                        int ncrtLess = int.Parse(this.lblLess.Text);
                        this.lblLess.Text = (ncrtLess - 1).ToString();
                        this.lblEqual.Text = (ncrtEqul + 1).ToString();
                    }
                    bFind = true;
                    break;
                }
            }
            if (bFind == false)
            {
                //获取具有该id的产品信息
                Product p1 = new Product(epc, string.Empty, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), string.Empty, string.Empty);
                string jsonString = fastJSON.JSON.Instance.ToJSON(p1);
                HttpWebConnect helper = new HttpWebConnect();
                helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_getProduct);
                string url = RestUrl.getProduct;
                helper.TryPostData(url, jsonString);
            }
            _UpdateList.SetItem("LinkEPCToProduct", true);

            //DataTable dt1 = ctlInventory.GetProductInfoTable(epc);
            //if (dt1.Rows.Count > 0)
            //{
            //    //将具体的产品信息添加到详细列表里面
            //    if (this.dgvProductInfo.DataSource != null)
            //    {
            //        DataTable dt = (DataTable)this.dgvProductInfo.DataSource;

            //        //首先检查该产品是否已经扫描过
            //        bool alreadyAdded = false;
            //        foreach (DataRow dr in dt.Rows)
            //        {
            //            if (dr.ItemArray[0].ToString() == epc)
            //            {
            //                alreadyAdded = true;
            //                break;
            //            }
            //        }
            //        //如果尚未扫描过
            //        if (!alreadyAdded)
            //        {
            //            DataRow dr = dt.NewRow();
            //            dr.ItemArray = dt1.Rows[0].ItemArray;
            //            dt.Rows.Add(dr);
            //        }
            //    }
            //}

        }
        void helper_RequestCompleted_getProduct(object o)
        {
            deleControlInvoke dele = delegate(object op)
            {
                string strProduct = (string)op;
                Product p = fastJSON.JSON.Instance.ToObject<Product>(strProduct);
                if (p.state == "ok")
                {
                    DataTable dt = null;
                    dt = (DataTable)dgvProductInfo.DataSource;
                    dt.Rows.Add(new object[]{
                        p.productID,
                        p.productName,
                        p.produceDate,
                        p.productCategory,
                        p.descript
                    });
                    foreach (DataGridViewRow vr in this.dgvProductInfo.Rows)
                    {
                        DataGridViewCell cepc = (DataGridViewCell)vr.Cells[1];
                        if (((string)cepc.Value) == p.productID)
                        {
                            DataGridViewCheckBoxCell cbc = (DataGridViewCheckBoxCell)vr.Cells[0];

                            cbc.Value = LightStatus.Unknown;
                            int ncrtMore = int.Parse(this.lblMore.Text);
                            this.lblMore.Text = (ncrtMore + 1).ToString();
                            break;
                        }
                    }
                }
                else
                {
                    Debug.WriteLine(
                        string.Format("frmInventory.helper_RequestCompleted_getProduct  ->  = {0}"
                        , "获取产品信息失败！"));
                }
            };
            this.Invoke(dele, o);
        }

        void frmInventory_Shown(object sender, EventArgs e)
        {
            this.refreshDGVProductDetail();
        }
        void refreshDGVProductDetail()
        {
            //DataTable dt = ctlInventory.GetProductInfoTop0Table();
            //this.dgvProductInfo.DataSource = dt;

            DataTable dt = null;
            if (this.dgvProductInfo.DataSource == null)
            {
                dt = new DataTable();
                dt.Columns.Add("产品编号", typeof(string));
                dt.Columns.Add("产品名称", typeof(string));
                dt.Columns.Add("生产日期", typeof(string));
                dt.Columns.Add("产品类别", typeof(string));
                dt.Columns.Add("产品备注信息", typeof(string));
            }
            else
            {
                dt = (DataTable)dgvProductInfo.DataSource;
            }
            dgvProductInfo.DataSource = dt;
            if (!this.dgvProductInfo.Columns.Contains("checkColumn"))
            {
                DataGridViewCheckBoxColumn cc = CreateCheckBoxColumn();
                dgvProductInfo.Columns.Insert(0, cc);
            }

            this.dgvProductInfo.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            int headerW = this.dgvProductInfo.RowHeadersWidth;
            int columnsW = 0;
            DataGridViewColumnCollection columns = this.dgvProductInfo.Columns;
            if (columns.Count > 0)
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    columnsW += columns[i].Width;
                }
                if (columnsW + headerW < this.dgvProductInfo.Width)
                {
                    int leftTotalWidht = this.dgvProductInfo.Width - columnsW - headerW;
                    int eachColumnAddedWidth = leftTotalWidht / (columns.Count);
                    for (int i = 0; i < columns.Count; i++)
                    {
                        columns[i].Width += eachColumnAddedWidth;
                    }
                }
            }

            HttpWebConnect helper = new HttpWebConnect();
            string url = RestUrl.getProductInfoForInventoryList;
            helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_getProductInfoForInventoryList);
            helper.TryRequest(url);
            return;
        }
        private DataGridViewCheckBoxColumn CreateCheckBoxColumn()
        {
            DataGridViewCheckBoxColumn cbc
                = new DataGridViewCheckBoxColumn();
            cbc.HeaderText = "";
            cbc.Name = "checkColumn";
            cbc.Width = 50;
            cbc.ThreeState = true;
            cbc.ValueType = typeof(LightStatus);
            cbc.TrueValue = LightStatus.TurnedOn;
            cbc.FalseValue = LightStatus.TurnedOff;
            cbc.IndeterminateValue = LightStatus.Unknown;
            return cbc;
        }

        void helper_RequestCompleted_getProductInfoForInventoryList(object o)
        {
            deleControlInvoke dele = delegate(object oProductList)
            {
                string strProduct = (string)oProductList;
                object olist = fastJSON.JSON.Instance.ToObjectList(strProduct, typeof(List<Product>), typeof(Product));

                DataTable dt = null;
                if (this.dgvProductInfo.DataSource == null)
                {
                    dt = new DataTable();
                    dt.Columns.Add("产品编号", typeof(string));
                    dt.Columns.Add("产品名称", typeof(string));
                    dt.Columns.Add("生产日期", typeof(string));
                    dt.Columns.Add("产品类别", typeof(string));
                    dt.Columns.Add("产品备注信息", typeof(string));
                }
                else
                {
                    dt = (DataTable)dgvProductInfo.DataSource;
                }
                dt.Rows.Clear();
                foreach (Product p in (List<Product>)olist)
                {
                    dt.Rows.Add(new object[]{
                        p.productID,
                        p.productName,
                        p.produceDate,
                        p.productCategory,
                        p.descript
                    });
                }
                dgvProductInfo.DataSource = dt;

                if (!this.dgvProductInfo.Columns.Contains("checkColumn"))
                {
                    DataGridViewCheckBoxColumn cbc = CreateCheckBoxColumn();

                    dgvProductInfo.Columns.Insert(0, cbc);
                }

                foreach (DataGridViewRow vr in this.dgvProductInfo.Rows)
                {
                    DataGridViewCheckBoxCell cbc = (DataGridViewCheckBoxCell)vr.Cells[0];

                    cbc.Value = LightStatus.TurnedOff;
                }


                this.dgvProductInfo.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                int headerW = this.dgvProductInfo.RowHeadersWidth;
                int columnsW = 0;
                DataGridViewColumnCollection columns = this.dgvProductInfo.Columns;
                columns[0].Width = 50;
                for (int i = 0; i < columns.Count; i++)
                {
                    columnsW += columns[i].Width;
                }
                if (columnsW + headerW < this.dgvProductInfo.Width)
                {
                    int leftTotalWidht = this.dgvProductInfo.Width - columnsW - headerW;
                    int eachColumnAddedWidth = leftTotalWidht / (columns.Count - 1);
                    for (int i = 1; i < columns.Count; i++)
                    {
                        columns[i].Width += eachColumnAddedWidth;
                    }
                }
            };
            this.Invoke(dele, o);
        }
        private void btnQuit_Click(object sender, EventArgs e)
        {
            this.beforeClosing();
            this.Close();
        }
        private void beforeClosing()
        {
            this.__timer.Enabled = false;
            return;

            this.operateUnitStartGetTag.closeSerialPort();
            bool bOk = _UpdateList.ChekcAllItem();
            // 如果没有全部完成，则要将消息处理让出，使Invoke有机会完成
            while (!bOk)
            {
                Application.DoEvents();
                bOk = _UpdateList.ChekcAllItem();
            }
            if (this.bGettingTag)
            {
                this.operateUnitStopGetTag.OperateStart(true);
            }
        }
        private void btnScan_Click(object sender, EventArgs e)
        {
            if (this.bGettingTag)
            {
                this.bGettingTag = false;
                this.btnScan.Text = "开始盘点";
                //停止通过网络获取标签
                this.__timer.Enabled = false;

                //this.operateUnitStartGetTag.closeSerialPort();
                //this.operateUnitStopGetTag.OperateStart(true);
            }
            else
            {
                this.bGettingTag = true;
                this.btnScan.Text = "停止";

                //初始化盘亏盘盈的数字统计信息
                this.lblEqual.Text = "0";
                this.lblLess.Text = this.dgvProductInfo.Rows.Count.ToString();
                this.lblMore.Text = "0";
                //开始通过网络获取标签
                __lastTagTimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                this.__timer.Enabled = true;
                //this.operateUnitStartGetTag.OperateStart();
            }
        }

        private void btnResult_Click(object sender, EventArgs e)
        {
            if (this.bGettingTag)
            {
                this.bGettingTag = false;
                this.btnScan.Text = "开始盘点";
                //停止通过网络获取标签
                this.__timer.Enabled = false;

                //this.operateUnitStartGetTag.closeSerialPort();
                //this.operateUnitStopGetTag.OperateStart(true);
            }
            List<Epc> list = new List<Epc>();
            //将具体的产品信息添加到详细列表里面
            if (this.dgvProductInfo.DataSource != null)
            {
                DataTable dt = (DataTable)this.dgvProductInfo.DataSource;

                foreach (DataRow dr in dt.Rows)
                {
                    Epc epc = new Epc(dr[0].ToString());
                    list.Add(epc);
                }
            }
            frmInventoryResult frm = new frmInventoryResult(list);
            frm.ShowDialog();

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.__timer.Enabled = false;
            this.bGettingTag = false;
            this.btnScan.Text = "开始盘点";
            this.lblEqual.Text = string.Empty;
            this.lblLess.Text = string.Empty;
            this.lblMore.Text = string.Empty;
            this.refreshDGVProductDetail();

        }
    }
    public enum LightStatus
    {
        Unknown,
        TurnedOn,
        TurnedOff
    };

}
