using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace InventoryMSystem
{
    public class StorageManageCtl
    {

        string sqlSelectNotInStorage =
                @"select productID as 产品编号,productName as 产品名称,produceDate as 生产日期
                ,productCategory as 产品类别,descript 产品备注信息 from tbProduct where 
                productStatus = '0';";
        string sqlSelectInStorage =
        @"select productID as 产品编号,productName as 产品名称,produceDate as 生产日期
                ,productCategory as 产品类别,descript 产品备注信息 from tbProduct where 
                productStatus = '1';";
        string sqlUpdateChangeStorageStatusToInStorage =
            @"update tbProduct set productStatus = '1' where productID = @productID;";
        string sqlUpdateChangeStorageStatusToNotInStorage=
            @"update tbProduct set productStatus = '0' where productStatus = '1' and
                productID = @productID;";

        public bool SetProductNotInStorage(string productID)
        {
            try
            {
                int result = int.Parse(DbOperate.ExecuteNonQuery(
                                             sqlUpdateChangeStorageStatusToNotInStorage
                                             , new object[1]
                                                    {
                                                        productID
                                                    }).ToString());
                if (result > 0)
                {
                    return true;
                }
            }
            catch (System.Exception ex)
            {

                MessageBox.Show("取消入库时出现错误：" + ex.Message);
            }
            return false;
        }

        public bool SetProductInStorage(string productID)
        {
            try
            {
                int result = int.Parse(DbOperate.ExecuteNonQuery(
                                             sqlUpdateChangeStorageStatusToInStorage
                                             , new object[1]
                                                    {
                                                        productID
                                                    }).ToString());
                if (result > 0)
                {
                    return true;
                }
            }
            catch (System.Exception ex)
            {

                MessageBox.Show("入库时出现错误：" + ex.Message);
            }
            return false;
        }
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
        public DataTable GetNotInStorageProductInfoTable()
        {
            DataSet ds = null;
            try
            {
                ds = DbOperate.ExecuteDataSet(
                           sqlSelectNotInStorage, null);
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
