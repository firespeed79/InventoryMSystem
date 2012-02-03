using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace InventoryMSystem
{
    public class InventoryResultCtl
    {
        string sqlSelectInStorage =
            @"select productID as 产品编号,productName as 产品名称,produceDate as 生产日期
                ,productCategory as 产品类别,descript 产品备注信息 from tbProduct where 
                productStatus = '1';";
        string sqlSelectGetProductByID =
    @"select productID as 产品编号,productName as 产品名称,produceDate as 生产日期
                ,productCategory as 产品类别,descript 产品备注信息 from tbProduct where 
                  productID in(@productIDs);";
        string sqlSelectGetProductByIDFormat =
    @"select productID as 产品编号,productName as 产品名称,produceDate as 生产日期
                ,productCategory as 产品类别,descript 产品备注信息 from tbProduct where 
                  productID in({0});";

        public DataTable GetInStorageProductInfoTable()
        {
            DataSet ds = null;
            try
            {
                ds = DbOperate.ExecuteDataSet(
                           sqlSelectInStorage, null);
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
        public DataTable GetInStorageProductInventoryByIDs(string productIDs)
        {
            DataSet ds = null;
            string cmd = string.Format(sqlSelectGetProductByIDFormat, productIDs);
            try
            {
                ds = DbOperate.ExecuteDataSet(
                           cmd, null);
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
        public DataTable GetInStorageProductInventoryMore(string productIDs)
        {
            DataSet ds = null;
            try
            {
                ds = DbOperate.ExecuteDataSet(
                           sqlSelectGetProductByID,new object[1]{productIDs});
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
