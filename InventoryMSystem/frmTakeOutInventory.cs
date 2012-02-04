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
    public partial class frmTakeOutInventory : Form
    {
        rfidOperateUnitStopInventoryTag operateUnitStopGetTag = new rfidOperateUnitStopInventoryTag();
        rfidOperateUnitInventoryTag operateUnitStartGetTag = new rfidOperateUnitInventoryTag();
        InvokeDic _UpdateList = new InvokeDic();
        TakeOutInventoryCtl ctlTakeOutInventory = new TakeOutInventoryCtl();
        string __lastTagTimeStamp = string.Empty;
        Timer __timer;

        bool bGettingTag = false;//是否正在获取标签

        public frmTakeOutInventory()
        {
            InitializeComponent();

            __timer = new Timer();
            __timer.Interval = 1000;
            __timer.Tick += new EventHandler(__timer_Tick);

            this.FormClosing += new FormClosingEventHandler(frmTakeOutInventory_FormClosing);
            this.Shown += new EventHandler(frmTakeOutInventory_Shown);
            this.operateUnitStartGetTag.registeCallback(new deleRfidOperateCallback(UpdateEpcList));

        }
        //定时获取扫描到的标签
        void __timer_Tick(object sender, EventArgs e)
        {
            Debug.WriteLine(
                string.Format("frmTakeOutInventory.__timer_Tick  ->  = {0}"
                , ""));
            scanTagPara para = new scanTagPara(InventoryCommand.扫描出库, this.__lastTagTimeStamp);
            string jsonString = fastJSON.JSON.Instance.ToJSON(para);
            HttpWebConnect helper = new HttpWebConnect();
            helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_getScanTags);
            string url = RestUrl.getScanedTags;
            helper.TryPostData(url, jsonString);
        }
        void helper_RequestCompleted_getScanTags(object o)
        {
            string strTags = (string)o;
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

        void frmTakeOutInventory_Shown(object sender, EventArgs e)
        {
            this.refreshDGVOrder();
            this.refreshDGVProductDetail();
        }
        void refreshDGVOrder()
        {
            DataTable dt = null;
            if (this.dgvTakenOutP.DataSource == null)
            {
                dt = new DataTable();
                dt.Columns.Add("产品名称", typeof(string));
                dt.Columns.Add("订购数量", typeof(string));
                dt.Columns.Add("已扫描数量", typeof(string));
                dgvTakenOutP.DataSource = dt;
            }
            else
            {
                dt = (DataTable)dgvTakenOutP.DataSource;
            }
            dt.Rows.Clear();
            //DataTable dt = ctlTakeOutInventory.GetOrderTable();
            //this.dgvTakenOutP.DataSource = dt;

            this.dgvTakenOutP.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            int headerW = this.dgvTakenOutP.RowHeadersWidth;
            int columnsW = 0;
            DataGridViewColumnCollection columns = this.dgvTakenOutP.Columns;
            if (columns.Count > 0)
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    columnsW += columns[i].Width;
                }
                if (columnsW + headerW < this.dgvTakenOutP.Width)
                {
                    int leftTotalWidht = this.dgvTakenOutP.Width - columnsW - headerW;
                    int eachColumnAddedWidth = leftTotalWidht / (columns.Count);
                    for (int i = 0; i < columns.Count; i++)
                    {
                        columns[i].Width += eachColumnAddedWidth;
                    }
                }
            }

            string jsonString = string.Empty;
            HttpWebConnect helper = new HttpWebConnect();
            helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_getProductList4deleteProductFromStorage);
            string url = RestUrl.getProductList4deleteProductFromStorage;
            helper.TryPostData(url, jsonString);
        }
        void helper_RequestCompleted_getProductList4deleteProductFromStorage(object o)
        {
            deleControlInvoke dele = delegate(object oOrders)
            {
                string strOrder = (string)oOrders;
                object olist = fastJSON.JSON.Instance.ToObjectList(strOrder, typeof(List<Order>), typeof(Order));
                List<Order> orderList = (List<Order>)olist;
                if (orderList.Count > 0)
                {
                    DataTable dt = null;
                    if (this.dgvTakenOutP.DataSource == null)
                    {
                        dt = new DataTable();
                        dt.Columns.Add("产品名称", typeof(string));
                        dt.Columns.Add("订购数量", typeof(string));
                        dt.Columns.Add("已扫描数量", typeof(string));
                    }
                    else
                    {
                        dt = (DataTable)dgvTakenOutP.DataSource;
                    }
                    dt.Rows.Clear();
                    foreach (Order item in orderList)
                    {
                        dt.Rows.Add(new object[]{
                            item.productName,
                            item.quantity,
                            "0"
                        });
                    }
                }

            };
            this.Invoke(dele, o);
        }
        void updateDGVProductDetail(Product _p)
        {
            if (_p != null)
            {
                DataTable dt = (DataTable)this.dgvDetailProductsInfo.DataSource;
                dt.Rows.Add(new object[] {
                    _p.productID,
                    _p.productName,
                    _p.produceDate,
                    _p.productCategory,
                    _p.descript
                });
            }
        }
        void refreshDGVProductDetail()
        {
            DataTable dtTemp = null;
            if (this.dgvDetailProductsInfo.DataSource == null)
            {
                dtTemp = new DataTable();
                dtTemp.Columns.Add("产品编号", typeof(string));
                dtTemp.Columns.Add("产品名称", typeof(string));
                dtTemp.Columns.Add("生产日期", typeof(string));
                dtTemp.Columns.Add("产品类别", typeof(string));
                dtTemp.Columns.Add("备注信息", typeof(string));
                this.dgvDetailProductsInfo.DataSource = dtTemp;
            }
            else
            {
                dtTemp = (DataTable)this.dgvDetailProductsInfo.DataSource;
            }
            dtTemp.Rows.Clear();

            this.dgvDetailProductsInfo.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            int headerW = this.dgvDetailProductsInfo.RowHeadersWidth;
            int columnsW = 0;
            DataGridViewColumnCollection columns = this.dgvDetailProductsInfo.Columns;
            if (columns.Count > 0)
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    columnsW += columns[i].Width;
                }
                if (columnsW + headerW < this.dgvDetailProductsInfo.Width)
                {
                    int leftTotalWidht = this.dgvDetailProductsInfo.Width - columnsW - headerW;
                    int eachColumnAddedWidth = leftTotalWidht / (columns.Count);
                    for (int i = 0; i < columns.Count; i++)
                    {
                        columns[i].Width += eachColumnAddedWidth;
                    }
                }
            }
            return;
            DataTable dt = ctlTakeOutInventory.GetProductInfoTop0Table();
            this.dgvDetailProductsInfo.DataSource = dt;


        }
        void frmTakeOutInventory_FormClosing(object sender, FormClosingEventArgs e)
        {
            //this.closeSerialPort();
        }
        void UpdateEpcList(object o)
        {
            operateMessage msg = (operateMessage)o;
            if (msg.status == "fail")
            {
                MessageBox.Show("出现错误：" + msg.message);
                this.bGettingTag = false;
                this.btnGetP.Text = "扫描";
                return;
            }
            string value = msg.message;
            //把读取到的标签epc与产品的进行关联
            if (this.dgvDetailProductsInfo.InvokeRequired)
            {
                this.dgvDetailProductsInfo.Invoke(new deleUpdateContorl(LinkEPCToProduct), value);
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
            _UpdateList.SetItem("LinkEPCToProduct", true);

            //根据编码获取产品具体信息
            Product p1 = new Product(epc, string.Empty, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), string.Empty, string.Empty);
            string jsonString = fastJSON.JSON.Instance.ToJSON(p1);
            HttpWebConnect helper = new HttpWebConnect();
            helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_getProduct);
            string url = RestUrl.getProduct;
            helper.TryPostData(url, jsonString);
            return;

            DataTable dt1 = ctlTakeOutInventory.GetProductInfoTable(epc);
            if (dt1.Rows.Count > 0)
            {
                //将具体的产品信息添加到详细列表里面
                if (this.dgvDetailProductsInfo.DataSource != null)
                {
                    DataTable dt = (DataTable)this.dgvDetailProductsInfo.DataSource;

                    //首先检查该产品是否已经扫描过
                    bool alreadyAdded = false;
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr.ItemArray[0].ToString() == epc)
                        {
                            alreadyAdded = true;
                            break;
                        }
                    }
                    //如果尚未扫描过
                    if (!alreadyAdded)
                    {
                        //检查该产品在出库单中是否已经足够，数量足够的话不需要再添加
                        bool bEnough = false;
                        string productName = dt1.Rows[0].ItemArray[1].ToString();
                        DataTable dtOrder = (DataTable)this.dgvTakenOutP.DataSource;
                        int quantityOrdered = -1;
                        int quantityNow = -1;
                        DataRow drOrderProduct = null;
                        if (dtOrder != null)
                        {
                            foreach (DataRow dr in dtOrder.Rows)
                            {
                                if (dr[0].ToString() == productName)
                                {
                                    drOrderProduct = dr;
                                    break;
                                }
                            }
                            try
                            {
                                quantityOrdered = int.Parse(drOrderProduct[1].ToString());
                                quantityNow = int.Parse(drOrderProduct[2].ToString());
                            }
                            catch (System.Exception ex)
                            {
                                MessageBox.Show("程序异常：" + ex.Message);
                                return;
                            }
                        }
                        if (quantityNow > -1 && quantityOrdered > -1)
                        {
                            if (quantityNow >= quantityOrdered)
                            {
                                bEnough = true;
                            }
                        }
                        if (!bEnough)
                        {
                            if (null != drOrderProduct)
                            {
                                drOrderProduct[2] = (++quantityNow).ToString();
                            }
                            DataRow dr = dt.NewRow();
                            dr.ItemArray = dt1.Rows[0].ItemArray;
                            dt.Rows.Add(dr);

                            if (this.CheckAllOrderEnough())
                            {
                                this.btnStartCheck.Enabled = true;
                            }
                        }

                    }
                }
            }
            //DataGridViewRowCollection rows = this.dgvNotStoragedPInfo.Rows;
            //foreach (DataGridViewRow vr in rows)
            //{
            //    DataGridViewCell cepc = (DataGridViewCell)vr.Cells[1];
            //    if (((string)cepc.Value) == epc)
            //    {
            //        DataGridViewCheckBoxCell cbc = (DataGridViewCheckBoxCell)vr.Cells[0];
            //        cbc.Value = Boolean.TrueString;
            //        break;
            //    }
            //}
        }
        void helper_RequestCompleted_getProduct(object o)
        {
            deleControlInvoke dele = delegate(object oProduct)
            {
                string strProduct = (string)oProduct;
                Product u2 = fastJSON.JSON.Instance.ToObject<Product>(strProduct);
                if (u2 != null && u2.state == "ok")
                {
                    Debug.WriteLine(
                        string.Format("frmTakeOutInventory.helper_RequestCompleted_getProduct  ->  = {0}"
                        , u2.toString()));
                    //将具体的产品信息添加到详细列表里面
                    if (this.dgvDetailProductsInfo.DataSource != null)
                    {
                        DataTable dt = (DataTable)this.dgvDetailProductsInfo.DataSource;

                        //首先检查该产品是否已经扫描过
                        bool alreadyAdded = false;
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (dr.ItemArray[0].ToString() == u2.productID)
                            {
                                alreadyAdded = true;
                                break;
                            }
                        }
                        //如果尚未扫描过
                        if (!alreadyAdded)
                        {
                            //检查该产品在出库单中是否已经足够，数量足够的话不需要再添加
                            bool bEnough = false;
                            string productName = u2.productName;
                            DataTable dtOrder = (DataTable)this.dgvTakenOutP.DataSource;
                            int quantityOrdered = -1;
                            int quantityNow = -1;
                            DataRow drOrderProduct = null;
                            if (dtOrder != null && dtOrder.Rows.Count > 0)
                            {
                                foreach (DataRow dr in dtOrder.Rows)
                                {
                                    if (dr[0].ToString() == productName)
                                    {
                                        drOrderProduct = dr;
                                        break;
                                    }
                                }
                                if (drOrderProduct != null)
                                {
                                    try
                                    {
                                        quantityOrdered = int.Parse(drOrderProduct[1].ToString());
                                        quantityNow = int.Parse(drOrderProduct[2].ToString());
                                    }
                                    catch (System.Exception ex)
                                    {
                                        MessageBox.Show("程序异常：" + ex.Message);
                                        return;
                                    }
                                    if (quantityNow > -1 && quantityOrdered > -1)
                                    {
                                        if (quantityNow >= quantityOrdered)
                                        {
                                            bEnough = true;
                                        }
                                    }
                                    if (!bEnough)
                                    {
                                        if (null != drOrderProduct)
                                        {
                                            drOrderProduct[2] = (++quantityNow).ToString();
                                        }
                                        dt.Rows.Add(new object[] {
                                    u2.productID,
                                    u2.productName,
                                    u2.produceDate,
                                    u2.productCategory,
                                    u2.descript
                                    });

                                        if (this.CheckAllOrderEnough())
                                        {
                                            this.btnStartCheck.Enabled = true;
                                        }
                                    }
                                }


                            }
                        }
                    }
                }
            };
            this.Invoke(dele, o);
        }
        bool CheckAllOrderEnough()
        {
            bool bEnough = true;
            DataTable dt = (DataTable)this.dgvTakenOutP.DataSource;
            foreach (DataRow dr in dt.Rows)
            {
                int quantityOrdered = -1;
                int quantityNow = -1;
                try
                {
                    quantityNow = int.Parse(dr[2].ToString());
                    quantityOrdered = int.Parse(dr[1].ToString());
                    if (quantityOrdered > quantityNow)
                    {
                        bEnough = false;
                        break;
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("程序异常：" + ex.Message);
                }
            }
            return bEnough;
        }
        private void closeSerialPort()
        {
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

        private void btnGetP_Click(object sender, EventArgs e)
        {
            if (this.bGettingTag)
            {
                this.bGettingTag = false;
                this.btnGetP.Text = "扫描";

                this.__timer.Enabled = false;

                //本地标签扫描
                //this.operateUnitStartGetTag.closeSerialPort();
                //this.operateUnitStopGetTag.OperateStart(true);
            }
            else
            {
                this.bGettingTag = true;
                this.btnGetP.Text = "停止";
                //通过网络获取标签
                __lastTagTimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                this.__timer.Enabled = true;

                //本地标签扫描
                //this.operateUnitStartGetTag.OperateStart();
            }
        }

        private void btnStartCheck_Click(object sender, EventArgs e)
        {
            this.__timer.Enabled = false;
            this.bGettingTag = false;
            this.btnGetP.Text = "扫描";
            this.btnStartCheck.Enabled = false;
            //this.closeSerialPort();
            List<Product> list = new List<Product>();

            DataTable dt = (DataTable)this.dgvDetailProductsInfo.DataSource;
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string productID = dr[0].ToString();
                    Product p1 = new Product(productID, string.Empty, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), string.Empty, string.Empty);
                    list.Add(p1);
                    //this.ctlTakeOutInventory.InsertProductInfoIntoOutInventory(productID, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    //this.ctlTakeOutInventory.ChangeProductStatusforOutInventory(productID);
                }
            }
            string jsonString = fastJSON.JSON.Instance.ToJSON(list);
            HttpWebConnect helper = new HttpWebConnect();
            helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_deleteProductToStorage);
            string url = RestUrl.deleteProductFromStorage;
            helper.TryPostData(url, jsonString);


            DataTable dtOrder = (DataTable)this.dgvTakenOutP.DataSource;
            List<Order> listOrder = new List<Order>();
            if (dtOrder != null)
            {
                foreach (DataRow dr in dtOrder.Rows)
                {
                    string Name = dr[0].ToString();
                    Order o = new Order(Name, 1);
                    listOrder.Add(o);
                }

                jsonString = fastJSON.JSON.Instance.ToJSON(listOrder);
                Debug.WriteLine(
                	string.Format("frmTakeOutInventory.btnStartCheck_Click  -> orders = {0}"
                	, jsonString));
                helper = new HttpWebConnect();
                helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_deleteOrders);
                url = RestUrl.deleteOrders;
                helper.TryPostData(url, jsonString);
            }


            //this.ctlTakeOutInventory.DeleteOrderInfo();
            //this.frmTakeOutInventory_Shown(null, null);
        }
        void helper_RequestCompleted_deleteOrders(object o)
        {
            string strOrders = (string)o;
            Debug.WriteLine(
            	string.Format("frmTakeOutInventory.helper_RequestCompleted_deleteOrders  ->  = {0}"
            	, strOrders));
            object olist = fastJSON.JSON.Instance.ToObjectList(strOrders, typeof(List<Order>), typeof(Order));
            foreach (Order c in (List<Order>)olist)
            {
                if (c.state != "ok")
                {
                    MessageBox.Show(string.Format("删除名称为 {0} 的订单出现异常！", c.productName),"提示");
                    return;
                }
            }
            deleControlInvoke dele = delegate(object oNull)
            {
                this.refreshDGVOrder();
                this.refreshDGVProductDetail();
            };
            this.Invoke(dele, o);
        }
        void helper_RequestCompleted_deleteProductToStorage(object o)
        {
            string strProducts = (string)o;
            Debug.WriteLine(
            	string.Format("frmTakeOutInventory.helper_RequestCompleted_deleteProductToStorage  ->  = {0}"
            	, strProducts));
            object olist = fastJSON.JSON.Instance.ToObjectList(strProducts, typeof(List<Product>), typeof(Product));
            foreach (Product c in (List<Product>)olist)
            {
                if (c.state != "ok")
                {
                    MessageBox.Show(string.Format("编号为 {0} 的产品出库出现异常！", c.productID), "提示");
                    return;
                }
                Debug.WriteLine(c.productID + "      " + c.productName + "     " + c.state);
            }
        }
    }
}
