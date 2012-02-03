using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Windows.Forms;
using System.Configuration;
using System.Diagnostics;

namespace InventoryMSystem
{
    public class SQLhelper
    {
        static SQLhelper SqlHelperInstance;
        static string connectString = string.Empty;
        public static SQLhelper GetInstance()
        {
            if (SqlHelperInstance == null)
            {
                SqlHelperInstance = new SQLhelper();
            }
            return SqlHelperInstance;
        }
        private SQLhelper()
        {
            sqlCon = new SqlConnection(rtConnection());
        }
        public static string rtConnection()
        {
            if (connectString == string.Empty)
            {
                appConfig appconfig = appConfig.getDefaultConfig();
                if (appconfig != null)
                {
                    SQLConnConfig config = null;
                    config = SQLConnConfig.getDefaultDBConfig(appconfig.dbType);
                    if (config != null)
                    {
                        connectString = config.connectString;
                    }
                }
            }
            return connectString;
        }
        public void ResetConnection()
        {
            if (SQLhelper.GetInstance().sqlCon != null)
            {
                SQLhelper.GetInstance().sqlCon.Close();
            }
            SQLhelper.GetInstance().sqlCon = new SqlConnection(rtConnection());
            //this.sqlCon = new SqlConnection(rtConnection());
        }
        private SqlConnection sqlCon;
        public DataSet ExecuteDatasetText(string connectionString, string commandText, object[] paramList)
        /// <summary>
        /// 返回结果集的方法，适用有条件和无条件的查询
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <param name="objDataTable">接受表信息</param>
        /// <param name="strErr">错误信息</param>
        /// <returns>执行成功返回１，否则返回０</returns>
        public int GetTable(string strSQL, out DataTable objDataTable, out string strErr)
        {
            objDataTable = new DataTable();
            int iReturn = 0;
            strErr = "";

#if DEBUG
            Debug.WriteLine(string.Format("!!! SQLhelper-> GetTable 1 SQL:\n{0}",
                                        strSQL));

#endif

            //声明对象
            SqlCommand sqlCom = new SqlCommand();

            sqlCom.CommandText = strSQL;
            sqlCom.Connection = sqlCon;
            SqlDataAdapter sqlDa = new SqlDataAdapter();
            sqlDa.SelectCommand = sqlCom;

            try
            {
                sqlDa.Fill(objDataTable);//数据集填充到objDataTable
                iReturn = 1;
            }
            catch (Exception ex)
            {
                strErr = ex.ToString();
                //MessageBox.Show(strErr);
#if DEBUG
                Debug.WriteLine(string.Format("!!!ERROR SQLhelper-> GetTable 1 exception:{0}",
                                            ex.Message));
#endif
                iReturn = 0;
            }
            finally
            {
                sqlDa.Dispose();
                sqlCom.Dispose();
            }
            return iReturn;
        }
        public static int getDT(string strSQL, out DataTable objDataTable, out string strErr)
        {
            objDataTable = new DataTable();
            int iReturn = 0;
            strErr = "";


#if DEBUG
            Debug.WriteLine(string.Format("!!! SQLhelper-> getDT 1 SQL:\n{0}",
                                        strSQL));
#endif

            //声明对象
            SqlCommand sqlCom = new SqlCommand();

            sqlCom.CommandText = strSQL;
            sqlCom.Connection = new SqlConnection(rtConnection());
            SqlDataAdapter sqlDa = new SqlDataAdapter();
            sqlDa.SelectCommand = sqlCom;

            try
            {
                sqlDa.Fill(objDataTable);//数据集填充到objDataTable
                iReturn = 1;
            }
            catch (Exception ex)
            {
                strErr = ex.ToString();
#if DEBUG
                Debug.WriteLine(string.Format("!!!ERROR SQLhelper-> getDT 1 exception:{0}",
                                                ex.Message));
#endif


                iReturn = 0;
            }
            finally
            {
                sqlDa.Dispose();
                sqlCom.Dispose();
            }
            return iReturn;
        }

        public int execProc(string procName, out DataTable objDataTable, out string strErr)
        {
            objDataTable = new DataTable();
            int iReturn = 0;
            strErr = "";

#if DEBUG
            Debug.WriteLine(string.Format("!!! SQLhelper-> execProc 1 SQL:\n{0}",
                                        procName));
#endif

            SqlCommand sqlCom = new SqlCommand();
            sqlCom.Connection = sqlCon;
            sqlCom.CommandType = CommandType.StoredProcedure;
            sqlCom.CommandText = procName;
            SqlDataAdapter sqlDa = new SqlDataAdapter();
            sqlDa.SelectCommand = sqlCom;
            try
            {
                sqlDa.Fill(objDataTable);
                iReturn = 1;
            }
            catch (Exception ex)
            {
                strErr = ex.Message.ToString();
#if DEBUG
                Debug.WriteLine(string.Format("!!!ERROR SQLhelper no para-> execProc 1 exception:\n{0}",
                                                ex.Message));
#endif
                iReturn = 0;
            }
            finally
            {
                sqlDa.Dispose();
                sqlCom.Dispose();
            }
            return iReturn;
        }

        public int execProc(string procName, SqlParameter[] parameters, out DataTable objDataTable, out string strErr)
        {
            objDataTable = new DataTable();
            int iReturn = 0;
            strErr = "";


#if DEBUG
            Debug.WriteLine(string.Format("!!! SQLhelper-> execProc 2 SQL:\n{0}",
                                        procName));
            foreach (SqlParameter param in parameters)
            {
                Debug.WriteLine(string.Format("\n name: {0} = {1}", param.ParameterName, param.Value));
            }
#endif

            SqlCommand sqlCom = new SqlCommand();
            sqlCom.Connection = sqlCon;
            sqlCom.CommandType = CommandType.StoredProcedure;
            sqlCom.CommandText = procName;
            foreach (SqlParameter sqlparam in parameters)
            {
                sqlCom.Parameters.Add(sqlparam);
            }
            SqlDataAdapter sqlDa = new SqlDataAdapter();
            sqlDa.SelectCommand = sqlCom;
            try
            {
                sqlDa.Fill(objDataTable);
                iReturn = 1;
            }
            catch (Exception ex)
            {
                strErr = ex.Message.ToString();
#if DEBUG
                Debug.WriteLine(string.Format("!!!ERROR SQLhelper with para-> execProc 2 exception:\n{0}",
                                                ex.Message));
#endif
                iReturn = 0;
            }
            finally
            {
                sqlDa.Dispose();
                sqlCom.Dispose();
            }
            return iReturn;

        }
        public int execProc(string procName)
        {
            int iReturn = 0;
            string strErr = "";
            SqlCommand sqlCom = new SqlCommand();
            sqlCom.Connection = sqlCon;
            sqlCom.CommandType = CommandType.StoredProcedure;
            sqlCom.CommandText = procName;


#if DEBUG
            Debug.WriteLine(string.Format("!!! SQLhelper-> execProc 3 SQL:\n{0}",
                                        procName));
#endif

            try
            {
                iReturn = sqlCom.ExecuteNonQuery();
                //sqlDa.Fill(objDataTable);
                //iReturn = 1;
            }
            catch (Exception ex)
            {
                strErr = ex.Message.ToString();
#if DEBUG
                Debug.WriteLine(string.Format("!!!ERROR SQLhelper -> execProc 3 exception:\n{0}",
                                                ex.Message));
#endif
                iReturn = 0;
            }
            finally
            {
                //sqlDa.Dispose();
                sqlCom.Dispose();
            }
            return iReturn;

        }
        /// <summary>
        /// 带参数的查询
        /// </summary>
        /// <param name="strSQL"></param>
        /// <param name="parameters"></param>
        /// <param name="objDataTable"></param>
        /// <param name="strErr"></param>
        /// <returns></returns>
        public int GetTable(string strSQL, SqlParameter[] parameters, out DataTable objDataTable, out string strErr)
        {
            objDataTable = new DataTable();
            int iReturn = 0;
            strErr = "";

#if DEBUG
            Debug.WriteLine(string.Format("!!! SQLhelper-> GetTable 2 SQL:\n{0}",
                                        strSQL));
            foreach (SqlParameter param in parameters)
            {
                Debug.WriteLine(string.Format("\n name: {0} = {1}", param.ParameterName, param.Value));
            }
#endif
            //声明对象
            SqlCommand sqlCom = new SqlCommand();
            sqlCom.CommandText = strSQL;
            sqlCom.Connection = sqlCon;
            foreach (SqlParameter param in parameters)
            {
                sqlCom.Parameters.Add(param);
            }
            SqlDataAdapter sqlDa = new SqlDataAdapter();
            sqlDa.SelectCommand = sqlCom;



            try
            {
                sqlDa.Fill(objDataTable);//数据集填充到objDataTable
                iReturn = 1;
            }
            catch (Exception ex)
            {
                strErr = ex.ToString();
#if DEBUG
                Debug.WriteLine(string.Format("!!!ERROR SQLhelper -> GetTable 2 exception:\n{0}",
                                                ex.Message));
#endif

                iReturn = 0;
            }
            finally
            {
                sqlDa.Dispose();
                sqlCom.Dispose();
            }
            return iReturn;
        }

        /// <summary>
        ///  无返回结果集的方法，增加，删除，修改方法调用
        /// </summary>
        /// <param name="strSQL">不同的查询语句，实现不同的方法</param>
        /// <param name="strErr">接收错误信息</param>
        /// <returns>执行成功返回１，否则返回０</returns>
        public int NoneReturn(string strSQL, out string strErr)
        {

            int iReturn = 0;
            strErr = "";


#if DEBUG
            Debug.WriteLine(string.Format("!!! SQLhelper-> NoneReturn 1 SQL:\n{0}",
                                        strSQL));
#endif

            //声明对象
            SqlCommand sqlCom = new SqlCommand();
            sqlCom.Connection = sqlCon;
            String strstring = strSQL;
            sqlCom.CommandText = strstring;

            try
            {
                sqlCon.Open();
                sqlCom.ExecuteNonQuery();//打开连接，执行方法
                iReturn = 1;
            }
            catch (Exception ex)
            {
                strErr = ex.ToString();
#if DEBUG
                Debug.WriteLine(string.Format("!!!ERROR SQLhelper-> NoneReturn 1 exception:\n{0}",
                                            ex.Message));
#endif
                iReturn = 0;
            }
            finally
            {
                sqlCon.Close();
                sqlCom.Dispose();//关闭连接，释放资源
            }
            return iReturn;

        }
        /// <summary>
        /// 有参数
        /// </summary>
        /// <param name="strSQL"></param>
        /// <param name="parameters"></param>
        /// <param name="strErr"></param>
        /// <returns></returns>
        public int NoneReturn(string strSQL, SqlParameter[] parameters, out string strErr)
        {

            int iReturn = 0;
            strErr = "";

#if DEBUG
            Debug.WriteLine(string.Format("!!! SQLhelper-> NoneReturn 2 SQL:\n{0}",
                                        strSQL));
            foreach (SqlParameter param in parameters)
            {
                Debug.WriteLine(string.Format("\n name: {0} = {1}", param.ParameterName, param.Value));
            }
#endif


            //声明对象
            SqlCommand sqlCom = new SqlCommand();
            sqlCom.Connection = sqlCon;
            String strstring = strSQL;
            sqlCom.CommandText = strstring;
            foreach (SqlParameter param in parameters)
            {
                sqlCom.Parameters.Add(param);
            }

            try
            {
                sqlCon.Open();
                sqlCom.ExecuteNonQuery();//打开连接，执行方法
                iReturn = 1;
            }
            catch (Exception ex)
            {
                strErr = ex.ToString();
#if DEBUG
                Debug.WriteLine(string.Format("!!!ERROR SQLhelper-> NoneReturn 2 exception:\n{0}",
                                            ex.Message));
#endif
                iReturn = 0;
            }
            finally
            {
                sqlCon.Close();
                sqlCom.Dispose();//关闭连接，释放资源
            }
            return iReturn;
        }
    }
}
