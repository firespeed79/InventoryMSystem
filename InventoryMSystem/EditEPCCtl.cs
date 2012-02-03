using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace InventoryMSystem
{
    public class EditEPCCtl
    {
        string sqlSelectGetEpc =
            @"select productID from tbProduct  where productID = @productID;";

        public bool CheckEpcExist(string epc)
        {
            try
            {
                int result = int.Parse(DbOperate.ExecuteNonQuery(
                                             sqlSelectGetEpc
                                             , new object[1]
                                                    {
                                                        epc
                                                    }).ToString());
                if (result > 0)
                {
                    return true;
                }
            }
            catch (System.Exception ex)
            {

                MessageBox.Show("查询数据时出现错误：" + ex.Message);
            }
            return false;
        }
    }
}
