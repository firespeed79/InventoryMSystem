using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using System.Diagnostics;

namespace InventoryMSystem
{
    public class rfidOperateUnitBase : IRfidOperateUnit
    {
        deleRfidOperateCallback callback = null;
        public List<operateAction> actionList = new List<operateAction>();
        RFIDHelper _RFIDHelper = new RFIDHelper();
        //public SerialPort comport = new SerialPort();
        SerialPort comport = StaticSerialPort.getStaticSerialPort();
        int ActionIndex = 0;//标记执行到的Action的索引,从0开始
        bool bAutoRemoveParser = false;

        public rfidOperateUnitBase()
        {
            //comport.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            //StaticSerialPort.AddParser(_RFIDHelper.Parse);
            //使得Helper类可以向串口中写入数据
            _RFIDHelper.evtWriteToSerialPort += new deleVoid_Byte_Func(RFIDHelper_evtWriteToSerialPort);
            // 处理当前操作的状态
            _RFIDHelper.evtCardState += new deleVoid_RFIDEventType_Object_Func(_RFIDHelper_evtCardState);

        }
        public void openSerialPort()
        {
            StaticSerialPort.AddParser(_RFIDHelper.Parse);
        }
        public void closeSerialPort()
        {
            StaticSerialPort.removeParser(_RFIDHelper.Parse);
        }
        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int n = comport.BytesToRead;//n为返回的字节数
                byte[] buf = new byte[n];//初始化buf 长度为n
                comport.Read(buf, 0, n);//读取返回数据并赋值到数组
                _RFIDHelper.Parse(buf);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void RFIDHelper_evtWriteToSerialPort(byte[] value)
        {
            if (comport == null)
            {
                return;
            }
            try
            {
                if (!comport.IsOpen)
                {
                    //ConfigManager.SetSerialPort(ref comport);
                    comport.Open();

                }
                comport.Write(value, 0, value.Length);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void _RFIDHelper_evtCardState(RFIDEventType eventType, object o)
        {
            if ((this.ActionIndex + 1) <= actionList.Count)
            {
                operateAction action = actionList[ActionIndex];
                if (!action.bLoop)
                {
                    ActionIndex++;
                }
                if (eventType == action.invokeEvent)
                {
                    string value = null;
                    value = action.getProcessedData(o);
                    if (value != null)
                    {
                        if (null != this.callback)
                        {
                            this.callback(new operateMessage("success", value));
                        }
                    }
                    if (action.nextCommandType != RFIDEventType.RMU_Unknown)
                    {
                        _RFIDHelper.SendCommand(action.nextCommand, action.nextCommandType, false);
                    }

                    //如果已经是最后一个action，选择是否自动关闭数据解析回调
                    if ((ActionIndex > (actionList.Count - 1)) && this.bAutoRemoveParser)
                    {
                        this.closeSerialPort();
                    }
                }
                else
                {
                    //异常出现
                    this.clearException(eventType, o);
                }
            }
        }

        public void registeCallback(deleRfidOperateCallback callback)
        {
            this.callback = callback;
        }
        public void OperateStart(bool autoClose)
        {
            this.bAutoRemoveParser = autoClose;
            this.OperateStart();
        }
        public void OperateStart()
        {
            this.openSerialPort();
            this.bInventoryStoped = false;
            ActionIndex = 0;
            if (actionList.Count > 0)
            {
                operateAction action = actionList[ActionIndex];

                if (!action.bLoop)
                {
                    ActionIndex++;
                }
                _RFIDHelper.SendCommand(action.nextCommand, action.nextCommandType);
            }
        }

        bool bInventoryStoped = false;
        /// <summary>
        /// 当读写器处于识别标签状态时，只能发送停止识别的命令才能被接收
        /// 因此如果接收到RMU_Exception事件，也可能是因为命令没有被接收
        /// 这里尝试发送 停止识别命令以确认这种情况
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="o"></param>
        public virtual void clearException(RFIDEventType eventType, object o)
        {
            if (eventType == RFIDEventType.RMU_Unknown)
            {
                return;
            }
            switch ((int)eventType)
            {
                case (int)RFIDEventType.RMU_Exception:
                    if (!bInventoryStoped)
                    {
                        bInventoryStoped = true;
                        _RFIDHelper.SendCommand(RFIDHelper.RFIDCommand_RMU_StopGet, RFIDEventType.RMU_StopGet);
                        break;
                    }
                    if (null != this.callback)
                    {
                        this.callback(new operateMessage("fail", "设备异常"));
                    }
                    break;
                case (int)RFIDEventType.RMU_Inventory:
                case (int)RFIDEventType.RMU_InventoryAnti:
                    _RFIDHelper.SendCommand(RFIDHelper.RFIDCommand_RMU_StopGet, RFIDEventType.RMU_StopGet);
                    this.OperateStart();
                    break;
                case (int)RFIDEventType.RMU_StopGet:
                    this.OperateStart();
                    break;
            }
        }
    }
}
