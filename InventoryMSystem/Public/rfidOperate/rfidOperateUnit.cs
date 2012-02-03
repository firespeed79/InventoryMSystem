using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryMSystem
{
    public class rfidOperateUnitStopInventoryTag : rfidOperateUnitBase
    {
        public rfidOperateUnitStopInventoryTag()
        {
            operateActionStopGet op = new operateActionStopGet();
            
            this.actionList.Add(op);
        }
    }
    public class rfidOperateUnitInventoryTag : rfidOperateUnitBase
    {
        public rfidOperateUnitInventoryTag()
        {
            this.actionList.Add(new operateActionCheckReady());
            this.actionList.Add( new operateActionInventoryAnti());
            operateActionInventoryAntiNoStopGet op = new operateActionInventoryAntiNoStopGet();
            op.bLoop = true;
            this.actionList.Add(op);
             
        }
    }
    public class rfidOperateUnitWirteEPC : rfidOperateUnitBase
    {
        public void setEPC(string epc)
        {
            operateActionInventoryWriteEpc op = (operateActionInventoryWriteEpc)this.actionList[1];
            op.setEPC(epc);
        }
        public rfidOperateUnitWirteEPC()
        {
            this.actionList.Add(new operateActionCheckReady());
            this.actionList.Add(new operateActionInventoryWriteEpc());
        }
        public rfidOperateUnitWirteEPC(string epc)
        {
            this.actionList.Add(new operateActionCheckReady());
            this.actionList.Add(new operateActionInventoryWriteEpc(epc));
        }
    }


    public class rfidOperateUnitGetTagEPC : rfidOperateUnitBase
    {
        
        public rfidOperateUnitGetTagEPC()
        {
            this.actionList.Add(new operateActionCheckReady());
            this.actionList.Add(new operateActionInventoryAnti());
            this.actionList.Add(new operateActionInventoryAntiNoNextCommand());
            this.actionList.Add(new operateActionStopGet());
        }
    }
}
