using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.SQLite;
using Db4objects.Db4o;

namespace InventoryMSystem
{
    public class SQLConnConfig
    {
        public string configName = "SQLConnConfig";
        public DBType dbType = DBType.sqlite;
        public string connectString = string.Empty;

        public SQLConnConfig(DBType type, string connect)
        {
            this.dbType = type;
            this.connectString = connect;
        }
        static string configFilePath = staticClass.configFilePath;
        public SQLConnConfig() { }
        public static SQLConnConfig getDefaultDBConfig(DBType type)
        {
            SQLConnConfig config = null;
//            IObjectContainer db = Db4oFactory.OpenFile(SQLConnConfig.configFilePath);
            IObjectContainer db = staticClass.db;

            try
            {
                IList<SQLConnConfig> list = db.Query<SQLConnConfig>(delegate(SQLConnConfig cf)
                {
                    return cf.dbType == type;
                }
                                                          );
                if (list.Count > 0)
                {
                    config = list[0];
                }

            }
            finally
            {
                //db.Close();
            }
            return config;
        }

        public static void saveConfig(SQLConnConfig config)
        {
            //IObjectContainer db = Db4oFactory.OpenFile(SQLConnConfig.configFilePath); 
            IObjectContainer db = staticClass.db;

            try
            {
                IList<SQLConnConfig> list = db.Query<SQLConnConfig>(delegate(SQLConnConfig cf)
                {
                    return cf.dbType == config.dbType;
                }
                                                          );
                if (list.Count <= 0)
                {
                    db.Store(config);
                }
                else
                {
                    SQLConnConfig.copy(config, list[0]);
                    db.Store(list[0]);
                }

            }
            finally
            {
                //db.Close();
            }
        }
        public static void copy(SQLConnConfig source, SQLConnConfig dest)
        {
            dest.configName = source.configName;
            dest.dbType = source.dbType;
            dest.connectString = source.connectString;
        }
        public static bool testConnection(DBType dbType, string connectString)
        {
            bool bR = false;
            string connStr = connectString;
            switch ((int)dbType)
            {
                case (int)DBType.sqlite:
                    //connStr = ConfigManager.GetDBConnectString(DBType.sqlite);
                    if (connStr != string.Empty)
                    {
                        SQLiteConnection cn = new SQLiteConnection(connStr);
                        try
                        {
                            cn.Open();
                            bR = true;
                        }
                        catch
                        {
                            //return false;
                        }
                        finally
                        {
                            cn.Close();
                        }
                    }
                    break;
                case (int)DBType.sqlserver:
                    SqlConnection conn = new SqlConnection();
                    //connStr = ConfigManager.GetDBConnectString(DBType.sqlserver);
                    try
                    {
                        conn.ConnectionString = connStr;
                        conn.Open();
                        bR = true;
                    }
                    catch
                    {
                        //return false;
                    }
                    finally
                    {
                        conn.Close();
                    }
                    break;
            }
            return bR;
        }
    }
}
