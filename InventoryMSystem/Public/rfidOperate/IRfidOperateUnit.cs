using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryMSystem
{
    public interface IRfidOperateUnit
    {
        void registeCallback(deleRfidOperateCallback callback);
        void OperateStart();
    }
}
