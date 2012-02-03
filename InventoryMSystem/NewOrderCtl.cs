using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace InventoryMSystem
{
    public class NewOrderCtl
    {
        string sqlSelectOrderItems =
            @"select productName as 产品名称,productQuantity as 订购数量 from tbOrder;";
        string sqlInsertAddOrderItem =
            @"insert into tbOrder(productName,productQuantity) values(@productName,@productQuantity);";
        string sqlDeleteDeleteOrderItem =
            @"delete from tbOrder where productName = @productName;";

        public bool DeleteOrderItem(string productName)
        {
            try
            {
                int result = int.Parse(DbOperate.ExecuteNonQuery(
                                             sqlDeleteDeleteOrderItem
                                             , new object[1]
                                                    {
                                                        productName
                                                    }).ToString());
                if (result > 0)
                {
                    return true;
                }
            }
            catch (System.Exception ex)
            {

                MessageBox.Show("删除订单项时出现错误：" + ex.Message);
            }
            return false;
        }

        public bool AddOrderItem(string productName, int productQuantity)
        {
            try
            {
                int result = int.Parse(DbOperate.ExecuteNonQuery(
                                             sqlInsertAddOrderItem
                                             , new object[2]
                                                    {
                                                        productName
                                                        ,productQuantity
                                                    }).ToString());
                if (result > 0)
                {
                    return true;
                }
            }
            catch (System.Exception ex)
            {

                MessageBox.Show("添加订单项时出现错误：" + ex.Message);
            }
            return false;
        }

        public DataTable GetOrderTable()
        {
            DataSet ds = null;
            try
            {
                ds = DbOperate.ExecuteDataSet(
                           sqlSelectOrderItems, null);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        return ds.Tables[0];
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("查询数据库时出现错误：" + ex.Message);
            }
            return null;
        }
    }
}
