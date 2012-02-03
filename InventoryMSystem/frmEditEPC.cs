using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace InventoryMSystem
{
    public partial class frmEditEPC : Form
    {
        InvokeDic _UpdateList = new InvokeDic();
        RFIDHelper _RFIDHelper = new RFIDHelper();
        //SerialPort comport = new SerialPort();
        rfidOperateUnitGetTagEPC operateUnitGetTagEpc = new rfidOperateUnitGetTagEPC();
        rfidOperateUnitWirteEPC operateUnitWriteEpc = new rfidOperateUnitWirteEPC();

        EditEPCCtl ctl = new EditEPCCtl();
        //bool bEPCOK = false;
        // 记录当前返回写入数据成功提示时的操作意图，区分写入EPC还是密码成功
        // 0 为epc，1 为密码
        int nSingleWriteDataState = 0;
        string tagUII = string.Empty;

        public frmEditEPC()
        {
            InitializeComponent();
            this.lblSecretConfirm.Text = "";
            this.statusLabel.Text = "";
            //string secret = ConfigManager.GetLockMemSecret();
            string secret = null;
            if (null == secret)
            {
                secret = "12345678";
            }
            this.txtSecret.Text = secret;
            this.txtSecretAgain.Text = secret;
            this.operateUnitGetTagEpc.registeCallback(new deleRfidOperateCallback(UpdateEPCtxtBox));
            //this.operateUnitGetTagEpc.openSerialPort();

            this.FormClosing += new FormClosingEventHandler(frmEditEPC_FormClosing);
            //comport.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            //使得Helper类可以向串口中写入数据
            //_RFIDHelper.evtWriteToSerialPort += new deleVoid_Byte_Func(RFIDHelper_evtWriteToSerialPort);
            // 处理当前操作的状态
            //_RFIDHelper.evtCardState += new deleVoid_RFIDEventType_Object_Func(_RFIDHelper_evtCardState);

            this.lblTip.Text = "";
        }

        void frmEditEPC_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.closeSerialPort();
        }
        void RFIDHelper_evtWriteToSerialPort(byte[] value)
        {
            //if (comport == null)
            //{
            //    return;
            //}
            //try
            //{
            //    if (!comport.IsOpen)
            //    {
            //        ConfigManager.SetSerialPort(ref comport);
            //        comport.Open();

            //    }
            //    comport.Write(value, 0, value.Length);
            //}
            //catch (System.Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }
        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //try
            //{
            //    int n = comport.BytesToRead;//n为返回的字节数
            //    byte[] buf = new byte[n];//初始化buf 长度为n
            //    comport.Read(buf, 0, n);//读取返回数据并赋值到数组
            //    _RFIDHelper.Parse(buf);
            //}
            //catch (System.Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string Value = string.Empty;
            string txtContent = textBox1.Text;
            lblTip.ForeColor = Color.Red;
            this.button1.Enabled = false;
            if (txtContent == null)
            {
                Value = "未输入EPC";

                goto SetTipValue;
            }
            if (txtContent.Length >= 0 && txtContent.Length < 24)
            {
                Value = "当前EPC长度为 " + txtContent.Length.ToString() + ",长度不符合要求";
                goto SetTipValue;

            }
            if (txtContent.Length > 24)
            {
                Value = "当前EPC长度为 " + txtContent.Length.ToString() + ",长度不符合要求";
                goto SetTipValue;
            }
            if (txtContent.Length == 24)
            {
                //Value = "当前EPC长度为 " + txtContent.Length.ToString();
                if (!Regex.IsMatch(txtContent, "[0-9a-fA-F]{24}"))
                {
                    Value = "EPC中含有不符合要求的符号";
                    goto SetTipValue;
                }

                //if (this.ctl.CheckEpcExist(txtContent))
                //{
                //    Value = "当前EPC已经使用过，请重新制定";
                //    goto SetTipValue;
                //}


                Value = "当前EPC符合要求";
                lblTip.ForeColor = Color.Black;
                this.button1.Enabled = true;
            }

        SetTipValue: this.lblTip.Text = Value;
        }
        void _RFIDHelper_evtCardState(RFIDEventType eventType, object o)
        {
            string value = "";
            switch ((int)eventType)
            {
                case (int)RFIDEventType.RMU_Exception:
                    if (null != o)
                    {

                    }
                    value = "设备尚未准备就绪！";
                    //MessageBox.Show("设备尚未准备就绪！");
                    UpdateStatus(value);
                    break;

                case (int)RFIDEventType.RMU_CardIsReady:
                    _RFIDHelper.SendCommand(RFIDHelper.RFIDCommand_RMU_InventoryAnti3, RFIDEventType.RMU_InventoryAnti);
                    break;

                case (int)RFIDEventType.RMU_InventoryAnti:
                    if (o != null && (string)o != "ok")
                    {
                        value = RFIDHelper.GetEPCFormUII((string)o);

                        _RFIDHelper.StopCallback();
                        _RFIDHelper.SendCommand(RFIDHelper.RFIDCommand_RMU_StopGet, RFIDEventType.RMU_StopGet);
                    }
                    if (this.textBox1.InvokeRequired)
                    {
                        this.textBox1.Invoke(new deleUpdateContorl(UpdateEPCtxtBox), value);
                    }
                    else
                    {
                        UpdateEPCtxtBox(value);
                    }
                    break;
                case (int)RFIDEventType.RMU_SingleWriteData:
                    if (nSingleWriteDataState == 0)
                    {
                        if (o == null)
                        {
                            //MessageBox.Show("更改标签EPC失败");
                            value = "更改标签EPC失败";
                        }
                        else
                        {
                            value = "更改标签EPC成功";
                            //MessageBox.Show("更改标签EPC成功");
                        }
                    }
                    if (nSingleWriteDataState == 1)
                    {
                        if (o == null)
                        {
                            value = "标签锁定失败";
                            //MessageBox.Show("标签锁定失败");
                        }
                        else
                        {
                            value = "标签( " + (string)o + " )锁定成功 ";

                            //MessageBox.Show(value);
                        }
                    }
                    UpdateStatus(value);
                    break;
                case (int)RFIDEventType.RMU_Inventory:
                    if (null == o)
                    {
                        value = "正在检测周围标签...";
                    }
                    else
                    {
                        _RFIDHelper.SendCommand(RFIDHelper.RFIDCommand_RMU_StopGet, RFIDEventType.RMU_StopGet);
                        tagUII = (string)o;
                        value = "检测到标签：" + tagUII;
                    }
                    UpdateStatus(value);
                    break;
                case (int)RFIDEventType.RMU_StopGet:
                    _RFIDHelper.RmuLockTagReserverdEpcTid("00000000", tagUII);

                    break;
                case (int)RFIDEventType.RMU_LockMem:
                    {
                        if (null == o)
                        {
                            value = "标签锁定失败 ";
                            //MessageBox.Show("标签锁定失败 ");
                        }
                        else if ((string)o == "ok")//写入密码
                        {
                            value = "写入密码...";
                            Debug.WriteLine(string.Format(" 写入密码->RMU_LockMem {0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.Millisecond.ToString()));
                            string _initialPwd = RFIDHelper.PwdCheck(null);
                            List<string> commands = new List<string>();
                            string strPwdT = RFIDHelper.PwdCheck(txtSecret.Text);
                            string pwdTH4 = strPwdT.Substring(0, 4);//前四位 
                            string pwdTT4 = strPwdT.Substring(4, 4);//后四位 
                            List<string> commandSetSecret1 = RFIDHelper.RmuWriteDataCommandCompose(RMU_CommandType.RMU_SingleWriteData, _initialPwd, 0, 2, pwdTH4, null);
                            List<string> commandSetSecret2 = RFIDHelper.RmuWriteDataCommandCompose(RMU_CommandType.RMU_SingleWriteData, pwdTH4 + _initialPwd, 0, 3, pwdTT4, null);
                            commands.AddRange(commandSetSecret1);
                            commands.AddRange(commandSetSecret2);
                            this.nSingleWriteDataState = 1;
                            _RFIDHelper.SendCommand(commands, RFIDEventType.RMU_SingleWriteData, false);
                        }
                    }
                    UpdateStatus(value);
                    break;
            }
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
        void UpdateEPCtxtBox(string value)
        {
            this.textBox1.Text = value;

        }
        void UpdateEPCtxtBox(object o)
        {
            operateMessage msg = (operateMessage)o;
            if (msg.status == "fail")
            {
                MessageBox.Show("出现错误：" + msg.message);
                return;
            }
            string value = msg.message;
            if (!_UpdateList.CheckItem("UpdateTipLable"))
            {
                return;
            }
            _UpdateList.SetItem("UpdateTipLable", false);

            if (this.textBox1.InvokeRequired)
            {
                this.textBox1.Invoke(new deleUpdateContorl(UpdateEPCtxtBox), value);
            }
            else
                UpdateEPCtxtBox(value);

            _UpdateList.SetItem("UpdateTipLable", true);

            this.closeSerialPort();
        }

        // read tag epc
        private void button2_Click(object sender, EventArgs e)
        {
            //_RFIDHelper.StartCallback();
            //_RFIDHelper.SendCommand(RFIDHelper.RFIDCommand_RMU_GetStatus, RFIDEventType.RMU_CardIsReady);
            this.operateUnitGetTagEpc.openSerialPort();
            this.operateUnitGetTagEpc.OperateStart();

        }
        // write EPC to Tag
        private void button1_Click(object sender, EventArgs e)
        {
            string strID = textBox1.Text;
            if (strID.Length < 0 || strID.Length > 24 || !Regex.IsMatch(strID, "[0-9a-fA-F]{24}"))
            {
                MessageBox.Show("EPC长度应为24位，且不能包含除 a-f 和 A-F之外的字母！");
                return;
            }
            this.operateUnitWriteEpc.setEPC(strID);
            //this.operateUnitWriteEpc.openSerialPort();
            this.operateUnitWriteEpc.OperateStart(true);
            /* 
            
            List<string> CommandWriteData =
                RFIDHelper.RmuWriteDataCommandCompose(RMU_CommandType.RMU_SingleWriteData,
                                                                    null,
                                                                    1,
                                                                    2,
                                                                    strID,
                                                                    null);
            this.nSingleWriteDataState = 0;
            _RFIDHelper.StartCallback();
            _RFIDHelper.SendCommand(CommandWriteData, RFIDEventType.RMU_SingleWriteData, false);
            */
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //bool bOk = false;
            //if (null != comport)
            //{
            //    if (comport.IsOpen)
            //    {
            //        bOk = _UpdateList.ChekcAllItem();
            //        // 如果没有全部完成，则要将消息处理让出，使Invoke有机会完成
            //        while (!bOk)
            //        {
            //            Application.DoEvents();
            //            bOk = _UpdateList.ChekcAllItem();
            //        }
            //        comport.Close();
            //    }
            //}
            this.closeSerialPort();
            this.Close();
        }

        private void txtSecretAgain_TextChanged(object sender, EventArgs e)
        {
            if (this.txtSecret.Text == this.txtSecretAgain.Text)
            {
                this.lblSecretConfirm.Text = "密码一致";
                this.lblSecretConfirm.ForeColor = Color.Black;
                btnLockTag.Enabled = true;
            }
            else
            {
                this.lblSecretConfirm.Text = "密码不一致";
                this.lblSecretConfirm.ForeColor = Color.Red;
                btnLockTag.Enabled = false;
            }
        }

        private void btnLockTag_Click(object sender, EventArgs e)
        {
            _RFIDHelper.SendCommand(RFIDHelper.RFIDCommand_RMU_Inventory,
                                    RFIDEventType.RMU_Inventory);
        }
        void UpdateStatusLable(string value)
        {
            this.statusLabel.Text = value;
        }

        private void btnSerialPortConfig_Click(object sender, EventArgs e)
        {
            this.closeSerialPort();
            frmSerialPortConfig frm = new frmSerialPortConfig();
            frm.ShowDialog();
        }
        private void closeSerialPort()
        {
            bool bOk = _UpdateList.ChekcAllItem();
            // 如果没有全部完成，则要将消息处理让出，使Invoke有机会完成
            while (!bOk)
            {
                Application.DoEvents();
                bOk = _UpdateList.ChekcAllItem();
            }
            operateUnitGetTagEpc.closeSerialPort();
        }
        private void btnSaveSecret_Click(object sender, EventArgs e)
        {
            if (this.txtSecret.Text != this.txtSecretAgain.Text)
            {
                MessageBox.Show("两次输入的密码不一致！");
                return;
            }
            string secret = "12345678";
            if (null != this.txtSecret)
            {
                secret = this.txtSecret.Text;
                secret += "00000000";
                secret = secret.Substring(0, 8);
            }
            //ConfigManager.SaveConfigItem("secret", secret);
            if (ConfigManager.SaveLockMemSecret(secret))
            {
                MessageBox.Show("密码保存成功！");
            }
            else
            {
                MessageBox.Show("密码保存失败！");
            }
        }

        private void txtSecret_TextChanged(object sender, EventArgs e)
        {
            if (this.txtSecret.Text == this.txtSecretAgain.Text)
            {
                this.lblSecretConfirm.Text = "密码一致";
                this.lblSecretConfirm.ForeColor = Color.Black;
                this.btnLockTag.Enabled = true;
            }
            else
            {
                this.lblSecretConfirm.Text = "密码不一致";
                this.lblSecretConfirm.ForeColor = Color.Red;
                btnLockTag.Enabled = false;
            }
        }
    }
}
