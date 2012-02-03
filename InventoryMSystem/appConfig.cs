using System;
using System.Collections.Generic;
using System.Text;
using Db4objects.Db4o;

namespace InventoryMSystem
{
    public class appConfig
    {
        public DBType dbType = DBType.sqlite;
        public appConfig() { }
        public appConfig(DBType type)
        {
            this.dbType = type;
        }
        static string configFilePath = staticClass.configFilePath;
        public static appConfig getDefaultConfig()
        {
            appConfig config = null;
            //IObjectContainer db = Db4oFactory.OpenFile(appConfig.configFilePath);
            IObjectContainer db = staticClass.db;

            try
            {
                IList<appConfig> list = db.Query<appConfig>(typeof(appConfig));

                if (list.Count > 0)
                {
                    config = list[0];
                }

            }
            finally
            {
               // db.Close();
            }
            return config;
        }

        public static void saveConfig(appConfig config)
        {
            //IObjectContainer db = Db4oFactory.OpenFile(appConfig.configFilePath);
            IObjectContainer db = staticClass.db;

            try
            {
                IList<appConfig> list = db.Query<appConfig>(typeof(appConfig));
                if (list.Count <= 0)
                {
                    db.Store(config);
                }
                else
                {
                    appConfig.copy(config, list[0]);
                    db.Store(list[0]);
                }

            }
            finally
            {
                //db.Close();
            }
        }
        public static void copy(appConfig source, appConfig dest)
        {
            dest.dbType = source.dbType;
        }

    }

}
