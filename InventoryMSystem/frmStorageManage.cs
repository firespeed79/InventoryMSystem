using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Inventory;
using httpHelper;

namespace InventoryMSystem
{
    public partial class frmStorageManage : Form
    {
        rfidOperateUnitStopInventoryTag operateUnitStopGetTag = new rfidOperateUnitStopInventoryTag();
        rfidOperateUnitInventoryTag operateUnitStartGetTag = new rfidOperateUnitInventoryTag();
        StorageManageCtl ctl = new StorageManageCtl();
        bool bGettingTag = false;//是否正在获取标签
        InvokeDic _UpdateList = new InvokeDic();
        Timer __timer;
        public frmStorageManage()
        {
            InitializeComponent();
            this.Shown += new EventHandler(frmStorageManage_Shown);
            this.tabControl1.SelectedIndexChanged += new EventHandler(tabControl1_SelectedIndexChanged);
            this.dgvNotStoragedPInfo.CellContentClick += new DataGridViewCellEventHandler(dgvNotStoragedPInfo_CellContentClick);
            this.dgvStoragedP.CellContentClick += new DataGridViewCellEventHandler(dgvStoragedP_CellContentClick);
            this.operateUnitStartGetTag.registeCallback(new deleRfidOperateCallback(UpdateEpcList));

            __timer = new Timer();
            __timer.Interval = 1000;
            __timer.Tick += new EventHandler(__timer_Tick);

            this.FormClosing += new FormClosingEventHandler(frmStorageManage_FormClosing);
        }

        string __lastTagTimeStamp = string.Empty;
        //定时获取扫描到的标签
        void __timer_Tick(object sender, EventArgs e)
        {
            scanTagPara para = new scanTagPara(InventoryCommand.扫描入库, this.__lastTagTimeStamp);
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
                string.Format("frmStorageManage.helper_RequestCompleted_getScanTags  ->  = {0}"
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


        void frmStorageManage_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.closeSerialPort();
        }

        void dgvStoragedP_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                DataGridViewRowCollection rows = this.dgvStoragedP.Rows;
                DataGridViewRow row = rows[e.RowIndex];
                DataGridViewCheckBoxCell cbc = (DataGridViewCheckBoxCell)row.Cells[0];
                if ((string)cbc.Value == Boolean.FalseString)
                {
                    cbc.Value = Boolean.TrueString;
                }
                else
                {
                    cbc.Value = Boolean.FalseString;
                }
            }
        }

        void dgvNotStoragedPInfo_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                if (!_UpdateList.CheckItem("LinkEPCToProduct"))
                {
                    return;
                }
                _UpdateList.SetItem("LinkEPCToProduct", false);
                DataGridViewRowCollection rows = this.dgvNotStoragedPInfo.Rows;
                DataGridViewRow row = rows[e.RowIndex];
                DataGridViewCheckBoxCell cbc = (DataGridViewCheckBoxCell)row.Cells[0];
                if ((string)cbc.Value == Boolean.FalseString)
                {
                    cbc.Value = Boolean.TrueString;
                }
                else
                {
                    cbc.Value = Boolean.FalseString;
                }
                _UpdateList.SetItem("LinkEPCToProduct", true);
            }
        }
        void UpdateEpcList(object o)
        {
            operateMessage msg = (operateMessage)o;
            if (msg.status == "fail")
            {
                MessageBox.Show("出现错误：" + msg.message);
                this.bGettingTag = false;
                this.btnGetTag.Text = "扫描";
                return;
            }
            string value = msg.message;
            //把读取到的标签epc与产品的进行关联
            Debug.WriteLine(string.Format(
                        "UpdateEpcList -> epc = {0}"
                        , value));
            this.LinkEPCToProduct(value);
        }
        void LinkEPCToProduct(string epc)
        {
            Debug.WriteLine(
                string.Format("frmStorageManage.LinkEPCToProduct  -> epc = {0}"
                , epc));

            if (!_UpdateList.CheckItem("LinkEPCToProduct"))
            {
                return;
            }
            _UpdateList.SetItem("LinkEPCToProduct", false);
            DataGridViewRowCollection rows = this.dgvNotStoragedPInfo.Rows;
            foreach (DataGridViewRow vr in rows)
            {
                DataGridViewCell cepc = (DataGridViewCell)vr.Cells[1];
                if (((string)cepc.Value) == epc)
                {
                    DataGridViewCheckBoxCell cbc = (DataGridViewCheckBoxCell)vr.Cells[0];
                    cbc.Value = Boolean.TrueString;
                    break;
                }
            }
            _UpdateList.SetItem("LinkEPCToProduct", true);
        }
        void frmStorageManage_Shown(object sender, EventArgs e)
        {
            this.RefreshNotStoragedPInfo();
        }

        void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedIndex == 0)
            {
                this.RefreshNotStoragedPInfo();
            }
            else
            {
                RefreshStoragedPInfo();

            }
        }
        public void RefreshStoragedPInfo()
        {
            this.cbSelectAllStoragedP.Checked = false;

            DataTable table = null;
            table = ctl.GetInStorageProductInfoTable();
            this.dgvStoragedP.DataSource = table;
            if (!this.dgvStoragedP.Columns.Contains("checkColumn"))
            {
                DataGridViewCheckBoxColumn cc = new DataGridViewCheckBoxColumn();
                cc.HeaderText = "";
                cc.Name = "checkColumn";
                cc.Width = 30;
                dgvStoragedP.Columns.Insert(0, cc);
            }
            this.dgvStoragedP.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            int headerW = this.dgvStoragedP.RowHeadersWidth;
            int columnsW = 0;
            DataGridViewColumnCollection columns = this.dgvStoragedP.Columns;
            columns[0].Width = 50;
            for (int i = 0; i < columns.Count; i++)
            {
                columnsW += columns[i].Width;
            }
            if (columnsW + headerW < this.dgvStoragedP.Width)
            {
                int leftTotalWidht = this.dgvStoragedP.Width - columnsW - headerW;
                int eachColumnAddedWidth = leftTotalWidht / (columns.Count - 1);
                for (int i = 1; i < columns.Count; i++)
                {
                    columns[i].Width += eachColumnAddedWidth;
                }
            }

        }
        void helper_RequestCompleted_getPreProListToStorage(object o)
        {
            deleControlInvoke dele = delegate(object oProductList)
            {
                string strProduct = (string)oProductList;
                Debug.WriteLine(
                    string.Format("frmStorageManage.helper_RequestCompleted_disposeList  ->  = {0}"
                    , strProduct));
                object olist = fastJSON.JSON.Instance.ToObjectList(strProduct, typeof(List<Product>), typeof(Product));

                DataTable dt = null;
                if (this.dgvNotStoragedPInfo.DataSource == null)
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
                    dt = (DataTable)dgvNotStoragedPInfo.DataSource;
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
                dgvNotStoragedPInfo.DataSource = dt;

                if (!this.dgvNotStoragedPInfo.Columns.Contains("checkColumn"))
                {
                    DataGridViewCheckBoxColumn cc = new DataGridViewCheckBoxColumn();
                    cc.HeaderText = "";
                    cc.Name = "checkColumn";
                    cc.Width = 50;
                    dgvNotStoragedPInfo.Columns.Insert(0, cc);
                }

                this.dgvNotStoragedPInfo.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                int headerW = this.dgvNotStoragedPInfo.RowHeadersWidth;
                int columnsW = 0;
                DataGridViewColumnCollection columns = this.dgvNotStoragedPInfo.Columns;
                columns[0].Width = 50;
                for (int i = 0; i < columns.Count; i++)
                {
                    columnsW += columns[i].Width;
                }
                if (columnsW + headerW < this.dgvNotStoragedPInfo.Width)
                {
                    int leftTotalWidht = this.dgvNotStoragedPInfo.Width - columnsW - headerW;
                    int eachColumnAddedWidth = leftTotalWidht / (columns.Count - 1);
                    for (int i = 1; i < columns.Count; i++)
                    {
                        columns[i].Width += eachColumnAddedWidth;
                    }
                }
            };
            this.Invoke(dele, o);
        }
        public void RefreshNotStoragedPInfo()
        {
            DataTable dt = null;
            if (this.dgvNotStoragedPInfo.DataSource == null)
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
                dt = (DataTable)dgvNotStoragedPInfo.DataSource;
            }
            dgvNotStoragedPInfo.DataSource = dt;
            if (!this.dgvNotStoragedPInfo.Columns.Contains("checkColumn"))
            {
                DataGridViewCheckBoxColumn cc = new DataGridViewCheckBoxColumn();
                cc.HeaderText = "";
                cc.Name = "checkColumn";
                cc.Width = 50;
                dgvNotStoragedPInfo.Columns.Insert(0, cc);
            }

            this.dgvNotStoragedPInfo.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            int headerW = this.dgvNotStoragedPInfo.RowHeadersWidth;
            int columnsW = 0;
            DataGridViewColumnCollection columns = this.dgvNotStoragedPInfo.Columns;
            columns[0].Width = 50;
            for (int i = 0; i < columns.Count; i++)
            {
                columnsW += columns[i].Width;
            }
            if (columnsW + headerW < this.dgvNotStoragedPInfo.Width)
            {
                int leftTotalWidht = this.dgvNotStoragedPInfo.Width - columnsW - headerW;
                int eachColumnAddedWidth = leftTotalWidht / (columns.Count - 1);
                for (int i = 1; i < columns.Count; i++)
                {
                    columns[i].Width += eachColumnAddedWidth;
                }
            }

            HttpWebConnect helper = new HttpWebConnect();
            string url = RestUrl.getPreProListToStorage;
            helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_getPreProListToStorage);
            helper.TryRequest(url);
            return;

            this.cbSelectAllNotStoragedP.Checked = false;
            DataTable table = null;

            table = ctl.GetNotInStorageProductInfoTable();
            this.dgvNotStoragedPInfo.DataSource = table;

        }
        private void closeSerialPort()
        {
            return;
            //需要一种异常处理方案
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
        private void btnQuit_Click(object sender, EventArgs e)
        {
            this.closeSerialPort();
            this.Close();
        }

        private void btnGetTag_Click(object sender, EventArgs e)
        {
            if (this.bGettingTag)
            {
                this.bGettingTag = false;
                this.btnGetTag.Text = "扫描";
                //停止通过网络获取标签
                this.__timer.Enabled = false;

                //本地扫描标签，不通过网络
                //this.operateUnitStartGetTag.closeSerialPort();
                //this.operateUnitStopGetTag.OperateStart(true);
            }
            else
            {
                this.bGettingTag = true;
                this.btnGetTag.Text = "停止";
                //开始通过网络获取标签
                __lastTagTimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                this.__timer.Enabled = true;

                //本地扫描标签，不通过网络
                //this.operateUnitStartGetTag.OperateStart();
            }
        }
        private void btnStorageP_Click(object sender, EventArgs e)
        {
            List<Product> list = new List<Product>();
            DataGridViewRowCollection rows = this.dgvNotStoragedPInfo.Rows;
            foreach (DataGridViewRow vr in rows)
            {
                DataGridViewCheckBoxCell cbc = (DataGridViewCheckBoxCell)vr.Cells[0];
                if ((string)cbc.Value == Boolean.TrueString)
                {
                    DataGridViewCell cepc = (DataGridViewCell)vr.Cells[1];
                    string epc = (string)cepc.Value;
                    Product p = new Product(epc, string.Empty, string.Empty, string.Empty, string.Empty);
                    list.Add(p);
                    //ctl.SetProductInStorage(epc);
                }
            }
            string jsonString = fastJSON.JSON.Instance.ToJSON(list);
            Debug.WriteLine(
                string.Format("frmStorageManage.btnStorageP_Click  ->  = {0}"
                , jsonString));
            HttpWebConnect helper = new HttpWebConnect();
            helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_addProductToStorage);
            string url = RestUrl.addProductToStorage;
            helper.TryPostData(url, jsonString);

            return;

        }
        void helper_RequestCompleted_addProductToStorage(object o)
        {
            
            string strProducts = (string)o;
            Debug.WriteLine(
                string.Format("frmStorageManage.helper_RequestCompleted_addProductToStorage  ->  = {0}"
                , strProducts));
            object olist = fastJSON.JSON.Instance.ToObjectList(strProducts, typeof(List<Product>), typeof(Product));
            deleControlInvoke dele = delegate(object ol)
            {
                foreach (Product c in (List<Product>)olist)
                {
                    Debug.WriteLine(c.productID + "      " + c.productName + "     " + c.state);
                }
                this.RefreshNotStoragedPInfo();
            };
            this.Invoke(dele, olist);
        }

        private void btnDeleteStoragedP_Click(object sender, EventArgs e)
        {
            DataGridViewRowCollection rows = this.dgvStoragedP.Rows;
            foreach (DataGridViewRow vr in rows)
            {
                DataGridViewCheckBoxCell cbc = (DataGridViewCheckBoxCell)vr.Cells[0];

                if (cbc.Value != null && bool.Parse(cbc.Value.ToString()))
                {
                    DataGridViewCell cepc = (DataGridViewCell)vr.Cells[1];
                    string epc = (string)cepc.Value;
                    ctl.SetProductNotInStorage(epc);
                    /* 
                    
                    Debug.WriteLine(string.Format(
                                "btnDeleteStoragedP_Click -> Select Index = {0}"
                                , cbc.RowIndex.ToString()));
                    */
                }
            }
            this.RefreshStoragedPInfo();
        }

        private void cbSelectAllStoragedP_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbSelectAllStoragedP.Checked)
            {
                DataGridViewRowCollection rows = this.dgvStoragedP.Rows;
                foreach (DataGridViewRow vr in rows)
                {
                    DataGridViewCheckBoxCell cbc = (DataGridViewCheckBoxCell)vr.Cells[0];
                    cbc.Value = Boolean.TrueString;

                }
            }
            else
            {
                DataGridViewRowCollection rows = this.dgvStoragedP.Rows;
                foreach (DataGridViewRow vr in rows)
                {
                    DataGridViewCheckBoxCell cbc = (DataGridViewCheckBoxCell)vr.Cells[0];
                    cbc.Value = Boolean.FalseString;
                }
            }
        }

        private void cbSelectAllNotStoragedP_CheckedChanged(object sender, EventArgs e)
        {
            DataGridViewRowCollection rows = this.dgvNotStoragedPInfo.Rows;
            if (this.cbSelectAllNotStoragedP.Checked)
            {
                foreach (DataGridViewRow vr in rows)
                {
                    DataGridViewCheckBoxCell cbc = (DataGridViewCheckBoxCell)vr.Cells[0];
                    cbc.Value = Boolean.TrueString;

                }
            }
            else
            {
                foreach (DataGridViewRow vr in rows)
                {
                    DataGridViewCheckBoxCell cbc = (DataGridViewCheckBoxCell)vr.Cells[0];
                    cbc.Value = Boolean.FalseString;
                }
            }
        }

        private void btnRefreshNotStoragedP_Click(object sender, EventArgs e)
        {
            this.RefreshNotStoragedPInfo();
        }

        private void btnRefreshStoragedP_Click(object sender, EventArgs e)
        {
            this.RefreshStoragedPInfo();
        }

    }
}
