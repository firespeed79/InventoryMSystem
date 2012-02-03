using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InventoryMSystem
{
    public class DbOperate
    {
        public static bool checkDBSetting()
        {
            if (staticClass.currentDbType == DBType.None)
            {
                appConfig appconfig = appConfig.getDefaultConfig();
                if (appconfig != null)
                {
                    staticClass.currentDbType = appconfig.dbType;
                    SQLConnConfig config = SQLConnConfig.getDefaultDBConfig(staticClass.currentDbType);
                    if (config != null)
                    {
                        staticClass.currentDBConnectString = config.connectString;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        public static DataSet ExecuteDataSet(string commandText, params object[] paramList)
        {
            if (checkDBSetting() == false)
            {
                return null;
            }
            DataSet ds = null;

            int dbtype = (int)staticClass.currentDbType;
            string connectionString = staticClass.currentDBConnectString;
            switch (dbtype)
            {
                case (int)DBType.sqlite:
                    ds = SQLiteHelper.ExecuteDataSet(connectionString, commandText, paramList);
                    break;
                case (int)DBType.sqlserver:
                    ds = SqlHelper.ExecuteDatasetText(connectionString, commandText, paramList);
                    break;
            }

            return ds;
        }
        public static int ExecuteScalar(string commandText, params object[] paramList)
        {
            int result = -1;
            if (checkDBSetting() == false)
            {
                return result;
            }
            int dbtype = (int)staticClass.currentDbType;
            //int dbtype = (int)ConfigManager.GetCurrentDBType();
            string connectionString = staticClass.currentDBConnectString;
            //string connectionString = ConfigManager.GetDBConnectString(ConfigManager.GetCurrentDBType());
            switch (dbtype)
            {
                case (int)DBType.sqlite:
                    result = int.Parse(
                                SQLiteHelper.ExecuteScalar(
                                            connectionString,
                                            commandText,
                                            paramList).ToString());
                    break;
                case (int)DBType.sqlserver:
                    result = int.Parse(
                            SqlHelper.ExecuteScalar(connectionString, commandText, paramList).ToString());
                    break;
            }

            return result;
        }
        public static int ExecuteNonQuery(string commandText, params object[] paramList)
        {
            int result = -1;
            if (checkDBSetting() == false)
            {
                return result;
            }
            int dbtype = (int)staticClass.currentDbType;
            //int dbtype = (int)ConfigManager.GetCurrentDBType();
            string connectionString = staticClass.currentDBConnectString;
            //int dbtype = (int)ConfigManager.GetCurrentDBType();
            //string connectionString = ConfigManager.GetDBConnectString(ConfigManager.GetCurrentDBType());
            switch (dbtype)
            {
                case (int)DBType.sqlite:
                    result = int.Parse(
                                SQLiteHelper.ExecuteNonQuery(
                                            connectionString,
                                            commandText,
                                            paramList).ToString());
                    break;
                case (int)DBType.sqlserver:
                    result = int.Parse(
                            SqlHelper.ExecuteNonQuery(connectionString, commandText, paramList).ToString());
                    break;
            }

            return result;
        }

    }
}
