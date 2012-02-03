using System;
using System.Collections.Generic;
using System.Text;
using Db4objects.Db4o;
using InventoryMSystem;

namespace Config
{
    public class serialPortConfig
    {
        static string configFilePath = staticClass.configFilePath;
        public string configName = string.Empty;
        public string portName = string.Empty;
        public string baudRate = string.Empty;
        public string parity = string.Empty;
        public string dataBits = string.Empty;
        public string stopBits = string.Empty;

        public serialPortConfig(string configName, string port, string rate, string parity, string dataBits, string stopBits)
        {
            this.configName = configName;
            this.portName = port;
            this.baudRate = rate;
            this.parity = parity;
            this.dataBits = dataBits;
            this.stopBits = stopBits;
        }
        public static void copy(serialPortConfig source, serialPortConfig dest)
        {
            dest.configName = source.configName;
            dest.portName = source.portName;
            dest.baudRate = source.baudRate;
            dest.parity = source.parity;
            dest.dataBits = source.dataBits;
            dest.stopBits = source.stopBits;
        }
        public static serialPortConfig getDefaultConfig()
        {
            serialPortConfig config = null;
            //IObjectContainer db = Db4oFactory.OpenFile(serialPortConfig.configFilePath);
            IObjectContainer db = staticClass.db;

            try
            {
                IList<serialPortConfig> list = db.Query<serialPortConfig>(delegate(serialPortConfig cf)
                {
                    return cf.configName == "default";
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
        public static void saveConfig(serialPortConfig config)
        {
            // accessDb4o
            //IObjectContainer db = Db4oFactory.OpenFile(serialPortConfig.configFilePath);
            IObjectContainer db = staticClass.db;

            try
            {
                IList<serialPortConfig> list = db.Query<serialPortConfig>(delegate(serialPortConfig cf)
                {
                    return cf.configName == config.configName;
                }
                                                          );
                if (list.Count <= 0)
                {
                    db.Store(config);
                }
                else
                {
                    serialPortConfig.copy(config, list[0]);
                    db.Store(list[0]);
                }

            }
            finally
            {
                //db.Close();
            }
        }
    }
}
