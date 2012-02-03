using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryMSystem
{
    public class operateAction
    {
        //当接收到此事件时触发动作
        public RFIDEventType invokeEvent = RFIDEventType.RMU_Unknown;
        public List<string> nextCommand = new List<string>();
        public RFIDEventType nextCommandType = RFIDEventType.RMU_Unknown;
        public string exceptionMsg = string.Empty;
        public bool bLoop = false;//标识是否执行到该action时是循环执行，例如循环读取标签直至停止
        public operateAction()
        {

        }
        /* 
        
        public operateAction(RFIDEventType invokeEvent, string nextCommand, RFIDEventType nextCommandType, string exMsg)
        {
            this.invokeEvent = invokeEvent;
            this.nextCommand = nextCommand;
            this.nextCommandType = nextCommandType;
            this.exceptionMsg = exMsg;
        }
        */
        public virtual string getProcessedData(object inData)
        {
            //子类应重写此方法
            string value = null;
            return value;
        }
        public override string ToString()
        {
            return invokeEvent.ToString() + " " + nextCommandType.ToString() + " " + nextCommand;
        }
    }
    public class operateActionCheckReady : operateAction
    {
        public operateActionCheckReady ()
        {
            this.nextCommand.Add(RFIDHelper.RFIDCommand_RMU_GetStatus);
            //this.nextCommand = RFIDHelper.RFIDCommand_RMU_GetStatus;
            this.nextCommandType = RFIDEventType.RMU_CardIsReady;
            this.exceptionMsg = "设备尚未准备就绪";
        }
    }
    public class operateActionInventoryAnti : operateAction
    {
        public operateActionInventoryAnti()
        {
            this.invokeEvent = RFIDEventType.RMU_CardIsReady;
            this.nextCommand.Add(RFIDHelper.RFIDCommand_RMU_InventoryAnti3);
            //this.nextCommand = RFIDHelper.RFIDCommand_RMU_InventoryAnti3;
            this.nextCommandType = RFIDEventType.RMU_InventoryAnti;
            this.exceptionMsg = "读取标签异常";
        }
    }
    public class operateActionInventoryAntiNoNextCommand : operateAction
    {
        public operateActionInventoryAntiNoNextCommand()
        {
            this.invokeEvent = RFIDEventType.RMU_InventoryAnti;
        }
    }
    public class operateActionInventoryAntiNoStopGet : operateAction
    {
        public operateActionInventoryAntiNoStopGet()
        {
            this.invokeEvent = RFIDEventType.RMU_InventoryAnti;
        }
        public override string getProcessedData(object inData)
        {
            string value = null;
            if (inData != null && (string)inData != "ok")
            {
                value = RFIDHelper.GetEPCFormUII((string)inData);
            }
            return value;
        }
    }
    public class operateActionJustStopGet : operateAction
    {
        public operateActionJustStopGet()
        {
            this.invokeEvent = RFIDEventType.RMU_InventoryAnti;
            this.nextCommand.Add(RFIDHelper.RFIDCommand_RMU_StopGet);
            //this.nextCommand = RFIDHelper.RFIDCommand_RMU_StopGet;
            this.nextCommandType = RFIDEventType.RMU_StopGet;
            this.exceptionMsg = "读取标签异常";
        }
    }
    public class operateActionStopGet : operateAction
    {
        public operateActionStopGet()
        {
            this.invokeEvent = RFIDEventType.RMU_InventoryAnti;
            this.nextCommand.Add(RFIDHelper.RFIDCommand_RMU_StopGet);
            //this.nextCommand = RFIDHelper.RFIDCommand_RMU_StopGet;
            this.nextCommandType = RFIDEventType.RMU_StopGet;
            this.exceptionMsg = "读取标签异常";
        }
        public override string getProcessedData(object inData)
        {
            string value = null;
            if (inData != null && (string)inData != "ok")
            {
                value = RFIDHelper.GetEPCFormUII((string)inData);
            }
            return value;
        }
    }
    public class operateActionInventoryWriteEpc : operateAction
    {
        string epc = string.Empty;
        public void setEPC(string epc)
        {
            this.epc = epc;
            this.nextCommand = 
                RFIDHelper.RmuWriteDataCommandCompose(RMU_CommandType.RMU_SingleWriteData,
                                                                                null,
                                                                                1,
                                                                                2,
                                                                                this.epc,
                                                                                null);
        }
        public operateActionInventoryWriteEpc():this(string.Empty)
        {

        }
        public operateActionInventoryWriteEpc(string epc)
        {
            this.epc = epc;
            this.invokeEvent = RFIDEventType.RMU_CardIsReady;
            this.nextCommand = 
                RFIDHelper.RmuWriteDataCommandCompose(RMU_CommandType.RMU_SingleWriteData,
                                                                                null,
                                                                                1,
                                                                                2,
                                                                                this.epc,
                                                                                null);
            this.nextCommandType = RFIDEventType.RMU_SingleWriteData;
            this.exceptionMsg = "写入标签异常";
        }
    }
}
