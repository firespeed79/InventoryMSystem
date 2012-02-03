using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using httpHelper;
using System.Diagnostics;
using Inventory;

namespace InventoryMSystem
{
    public partial class frmProductManage : Form, IRefreshDGV
    {
        //RFIDHelper _RFIDHelper = new RFIDHelper();
        //SerialPort comport = new SerialPort();
        InvokeDic _UpdateList = new InvokeDic();
        rfidOperateUnitGetTagEPC operateUnit = new rfidOperateUnitGetTagEPC();
        string tagUII = string.Empty;
        productManageCtl ctl = new productManageCtl();

        ctlProductName ctlProductName = new ctlProductName();
        public frmProductManage()
        {
            InitializeComponent();
            //DataTable dt = ctlProductName.GetProductName();
            //if (dt != null && dt.Rows.Count > 0)
            //{
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        this.cmbName.Items.Add(dr[0].ToString());
            //    }
            //}


            this.operateUnit.registeCallback(new deleRfidOperateCallback(UpdateStatus));
            this.operateUnit.openSerialPort();

            this.dateTimePicker1.Value = DateTime.Now;
            this.FormClosing += new FormClosingEventHandler(frmProductManage_FormClosing);
            this.Shown += new EventHandler(frmProductManage_Shown);
            //comport.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            //使得Helper类可以向串口中写入数据
            //_RFIDHelper.evtWriteToSerialPort += new deleVoid_Byte_Func(RFIDHelper_evtWriteToSerialPort);
            // 处理当前操作的状态
            //_RFIDHelper.evtCardState += new deleVoid_RFIDEventType_Object_Func(_RFIDHelper_evtCardState);

        }

        void frmProductManage_Shown(object sender, EventArgs e)
        {

            this.refreshDGV();
            this.fillCategary();
        }

        private void fillCategary()
        {
            HttpWebConnect helper = new HttpWebConnect();
            string url = RestUrl.allProductName;
            helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_disposeProductNameList);
            helper.TryRequest(url);
        }
        void helper_RequestCompleted_disposeProductNameList(object o)
        {
            string strProduct = (string)o;
            Debug.WriteLine(
             string.Format("frmProductManage.helper_RequestCompleted_disposeProductNameList  -> response = {0}"
             , strProduct));
            object olist = fastJSON.JSON.Instance.ToObjectList(strProduct, typeof(List<ProductName>), typeof(ProductName));
            deleControlInvoke dele = delegate(object list)
            {
                foreach (ProductName c in (List<ProductName>)list)
                {
                    this.cmbName.Items.Add(c.name);
                    //Debug.WriteLine(c.name);
                }
            };
            this.Invoke(dele, olist);
        }
        void frmProductManage_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.closeSerialPort();
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            this.closeSerialPort();
            /* 
            
            bool bOk = false;
            if (null != comport)
            {
                if (comport.IsOpen)
                {
                    bOk = _UpdateList.ChekcAllItem();
                    // 如果没有全部完成，则要将消息处理让出，使Invoke有机会完成
                    while (!bOk)
                    {
                        Application.DoEvents();
                        bOk = _UpdateList.ChekcAllItem();
                    }
                    comport.Close();
                }
            }
            */
            this.Close();
        }

        private void btnGetPID_Click(object sender, EventArgs e)
        {
            //DateTime dt = DateTime.Now;
            //this.txtPID.Text = dt.ToString("yyyyMMddHHmmssff");
            this.operateUnit.OperateStart();
            //_RFIDHelper.StartCallback();
            //_RFIDHelper.SendCommand(RFIDHelper.RFIDCommand_RMU_GetStatus, RFIDEventType.RMU_CardIsReady);

        }
        void UpdateStatus(string value)
        {
            if (this.statusLabel.InvokeRequired)
            {
                this.statusLabel.Invoke(new deleUpdateContorl(UpdateStatusLable), value);
            }
            else
            {
                UpdateStatusLable(value);
            }
        }
        void UpdateStatus(object o)
        {
            operateMessage msg = (operateMessage)o;
            if (msg.status == "fail")
            {
                MessageBox.Show("出现错误：" + msg.message);
                return;
            }
            string value = msg.message;
            if (this.statusLabel.InvokeRequired)
            {
                this.statusLabel.Invoke(new deleUpdateContorl(UpdateStatusLable), value);
            }
            else
            {
                UpdateStatusLable(value);
            }
        }
        void UpdateStatusLable(string value)
        {
            this.txtPID.Text = value;
            //this.statusLabel.Text = value;
        }
        void UpdateEPCtxtBox(string value)
        {
            if (!_UpdateList.CheckItem("UpdateTipLable"))
            {
                return;
            }
            _UpdateList.SetItem("UpdateTipLable", false);

            this.txtPID.Text = value;

            _UpdateList.SetItem("UpdateTipLable", true);
        }

        private void btnSerialPortConf_Click(object sender, EventArgs e)
        {
            this.closeSerialPort();
            frmSerialPortConfig frm = new frmSerialPortConfig();
            frm.ShowDialog();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmProductManageAdd frmAdd = new frmProductManageAdd(this);
            frmAdd.ShowDialog();
        }
        private void closeSerialPort()
        {
            operateUnit.closeSerialPort();
            bool bOk = _UpdateList.ChekcAllItem();
            // 如果没有全部完成，则要将消息处理让出，使Invoke有机会完成
            while (!bOk)
            {
                Application.DoEvents();
                bOk = _UpdateList.ChekcAllItem();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
        void helper_RequestCompleted_disposeList(object o)
        {
            //if (this.InvokeRequired)
            //{
            //    this.Invoke(new deleControlInvoke(updateDatagrid), o);
            //}
            deleControlInvoke dele = delegate(object oProductList)
            {
                string strProduct = (string)oProductList;
                Debug.WriteLine(
                    string.Format("frmProductManage.helper_RequestCompleted_disposeList  -> response = {0}"
                    , strProduct));
                Product u2 = fastJSON.JSON.Instance.ToObject<Product>(strProduct);
                object olist = fastJSON.JSON.Instance.ToObjectList(strProduct, typeof(List<Product>), typeof(Product));
                DataTable dt = null;
                if (this.dataGridView1.DataSource == null)
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
                    dt = (DataTable)dataGridView1.DataSource;
                }
                dt.Rows.Clear();
                foreach (Product c in (List<Product>)olist)
                {
                    dt.Rows.Add(new object[] {
                    c.productID,c.productName,c.produceDate,c.productCategory,c.descript
                });
                    Debug.WriteLine(c.productID + "      " + c.produceDate);
                }
                this.dataGridView1.DataSource = dt;

                this.dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                int headerW = this.dataGridView1.RowHeadersWidth;
                int columnsW = 0;
                DataGridViewColumnCollection columns = this.dataGridView1.Columns;
                if (columns.Count > 0)
                {
                    for (int i = 0; i < columns.Count; i++)
                    {
                        columnsW += columns[i].Width;
                    }
                    if (columnsW + headerW < this.dataGridView1.Width)
                    {
                        int leftTotalWidht = this.dataGridView1.Width - columnsW - headerW;
                        int eachColumnAddedWidth = leftTotalWidht / (columns.Count);
                        for (int i = 0; i < columns.Count; i++)
                        {
                            columns[i].Width += eachColumnAddedWidth;
                        }
                    }
                }
            };
            this.Invoke(dele, o);

        }
        public void refreshDGV()
        {

            HttpWebConnect helper = new HttpWebConnect();
            string url = RestUrl.allProducts;
            helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_disposeList);
            helper.TryRequest(url);
            return;
            //this.dataGridView1.DataSource = ctl.GetProductInfoTable();

            //this.dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            //int headerW = this.dataGridView1.RowHeadersWidth;
            //int columnsW = 0;
            //DataGridViewColumnCollection columns = this.dataGridView1.Columns;
            //if (columns.Count > 0)
            //{
            //    for (int i = 0; i < columns.Count; i++)
            //    {
            //        columnsW += columns[i].Width;
            //    }
            //    if (columnsW + headerW < this.dataGridView1.Width)
            //    {
            //        int leftTotalWidht = this.dataGridView1.Width - columnsW - headerW;
            //        int eachColumnAddedWidth = leftTotalWidht / (columns.Count);
            //        for (int i = 0; i < columns.Count; i++)
            //        {
            //            columns[i].Width += eachColumnAddedWidth;
            //        }
            //    }
            //}
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.cmbName.SelectedIndex == -1)
            {
                MessageBox.Show("请选择产品名称！");
                return;
            }
            Product newPro = new Product(this.txtPID.Text, this.cmbName.Text,
                                    this.dateTimePicker1.Value.ToString("yyyy-MM-dd HH:mm:ss"), this.cmbFactory.Text,
                                    this.txtComment.Text);
            string jsonString = fastJSON.JSON.Instance.ToJSON(newPro);
            Debug.WriteLine(
                string.Format("frmProductManageAdd.btnSave_Click  -> json string = {0}"
                , jsonString));
            HttpWebConnect helper = new HttpWebConnect();
            string url = RestUrl.updateProduct;
            helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_update);
            helper.TryPostData(url, jsonString);
            return;

            if (ctl.UpdateProductItem(this.txtPID.Text, this.cmbName.Text,
                                    this.dateTimePicker1.Value.ToString("yyyy-MM-dd HH:mm:ss"), this.cmbFactory.Text,
                                    this.txtComment.Text))
            {
                MessageBox.Show("更新产品信息成功！");
                this.refreshDGV();
            }
            else
            {
                MessageBox.Show("更新产品信息出错！");
            }

        }
        void helper_RequestCompleted_update(object o)
        {
            string strProduct = (string)o;
            Debug.WriteLine(
                string.Format("frmProductManageAdd.helper_RequestCompleted  -> response = {0}"
                , strProduct));
            Product u2 = fastJSON.JSON.Instance.ToObject<Product>(strProduct);
            deleControlInvoke dele = delegate(object op)
            {
                Product p = (Product)op;
                if (p.state == "ok")
                {
                    MessageBox.Show("更新产品信息成功！");
                    this.refreshDGV();
                }
                else
                {
                    MessageBox.Show("更新产品信息出错！");
                }
            };
            this.Invoke(dele, u2);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.txtPID.Text == null || this.txtPID.Text.Length <= 0)
            {
                return;
            }
            Product newPro = new Product(this.txtPID.Text, this.cmbName.Text,
                        this.dateTimePicker1.Value.ToString("yyyy-MM-dd HH:mm:ss"), this.cmbFactory.Text,
                        this.txtComment.Text);
            string jsonString = fastJSON.JSON.Instance.ToJSON(newPro);
            Debug.WriteLine(
                string.Format("frmProductManageAdd.btnDelete_Click -> json string = {0}"
                , jsonString));
            HttpWebConnect helper = new HttpWebConnect();
            string url = RestUrl.deleteProduct;
            helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_delete);
            helper.TryPostData(url, jsonString);
            return;

            if (ctl.DeleteProductItem(this.txtPID.Text))
            {
                MessageBox.Show("删除产品信息成功！");
                this.refreshDGV();
            }
            else
            {
                MessageBox.Show("删除产品信息出错！");
            }
        }
        void helper_RequestCompleted_delete(object o)
        {
            string strProduct = (string)o;
            Debug.WriteLine(
                string.Format("frmProductManageAdd.helper_RequestCompleted_delete-> response = {0}"
                , strProduct));
            Product u2 = fastJSON.JSON.Instance.ToObject<Product>(strProduct);
            deleControlInvoke dele = delegate(object op)
            {
                Product p = (Product)op;
                if (p.state == "ok")
                {
                    MessageBox.Show("删除产品信息成功！");
                    this.refreshDGV();
                }
                else
                {
                    MessageBox.Show("删除产品信息出错！");
                }
            };
            this.Invoke(dele, u2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.refreshDGV();
        }

        void SetLabelContent()
        {
            DataTable tb = (DataTable)dataGridView1.DataSource;
            if (tb != null && tb.Rows.Count > 0)
            {
                txtPID.Text = tb.Rows[0][0].ToString();
                this.cmbName.Text = tb.Rows[0][1].ToString();
                try
                {
                    this.dateTimePicker1.Value = DateTime.Parse(tb.Rows[0][2].ToString());
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                this.cmbFactory.Text = tb.Rows[0][3].ToString();
                this.txtComment.Text = tb.Rows[0][4].ToString();
            }
            else
            {
                txtPID.Text = null;
                cmbName.Text = null;
                this.dateTimePicker1.Value = DateTime.Now;
                cmbFactory.Text = null;
                txtComment.Text = null;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataTable tb = (DataTable)dataGridView1.DataSource;
            if (e.RowIndex >= 0)
            {
                dataGridView1.Rows[e.RowIndex].Selected = true;
                txtPID.Text = tb.Rows[e.RowIndex][0].ToString();
                //this.cmbName.Text = tb.Rows[e.RowIndex][1].ToString();
                this.cmbName.SelectedIndex = this.cmbName.Items.IndexOf(tb.Rows[e.RowIndex][1].ToString());
                try
                {
                    this.dateTimePicker1.Value = DateTime.Parse(tb.Rows[e.RowIndex][2].ToString());
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                this.cmbFactory.Text = tb.Rows[e.RowIndex][3].ToString();
                this.txtComment.Text = tb.Rows[e.RowIndex][4].ToString();
            }
        }
    }
}
