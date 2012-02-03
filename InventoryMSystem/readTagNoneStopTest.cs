using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace InventoryMSystem
{
    public partial class readTagNoneStopTest : Form
    {
        rfidOperateUnitStopInventoryTag operateUnitStopGetTag = new rfidOperateUnitStopInventoryTag();
        rfidOperateUnitInventoryTag operateUnitStartGetTag = new rfidOperateUnitInventoryTag();
        bool bGettingTag = false;//是否正在获取标签

        public readTagNoneStopTest()
        {
            InitializeComponent();

            this.FormClosing += new FormClosingEventHandler(frmInventory_FormClosing);
            this.operateUnitStartGetTag.registeCallback(new deleRfidOperateCallback(UpdateEpcList));
        }
        void UpdateEpcList(object o)
        {
            operateMessage msg = (operateMessage)o;
            if (msg.status == "fail")
            {
                MessageBox.Show("出现错误：" + msg.message);
                this.bGettingTag = false;
                return;
            }
            string value = msg.message;
            //把读取到的标签epc与产品的进行关联
            if (this.textBox1.InvokeRequired)
            {
                this.textBox1.Invoke(new deleUpdateContorl(LinkEPCToProduct), value);
            }
            else
                this.LinkEPCToProduct(value);
        }
        void LinkEPCToProduct(string epc)
        {
            this.textBox1.Text = epc;
        }
        void frmInventory_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.closeSerialPort();
        }
        private void closeSerialPort()
        {
            this.operateUnitStartGetTag.closeSerialPort();
            if (this.bGettingTag)
            {
                this.operateUnitStopGetTag.OperateStart(true);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.bGettingTag)
            {
                this.bGettingTag = false;
                this.btnScan.Text = "扫描";
                this.operateUnitStartGetTag.closeSerialPort();
                this.operateUnitStopGetTag.OperateStart(true);
            }
            else
            {
                this.bGettingTag = true;
                this.btnScan.Text = "停止";
                this.operateUnitStartGetTag.OperateStart();
            }
        }
    }
}
