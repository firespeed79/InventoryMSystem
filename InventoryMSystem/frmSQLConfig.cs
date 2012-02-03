using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace InventoryMSystem
{
    public partial class frmSQLConfig : Form
    {
        //private SQLConnConfig mySQLConnectionTest;
        DBType type = DBType.sqlserver;
        string connectString = string.Empty;

        public frmSQLConfig()
        {
            InitializeComponent();
            //mySQLConnectionTest = new SQLConnConfig();

            this.comboBox1.Items.Add("Microsoft SQLServer");
            this.comboBox1.Items.Add("Sqlite");

        }
        bool checkValidation()
        {
            bool bR = true;
            switch (this.comboBox1.SelectedIndex)
            {
                case 0:
                    type = DBType.sqlserver;
                    break;
                case 1:
                    type = DBType.sqlite;
                    break;
            }
            this.connectString = this.txtConnString.Text;
            if (this.connectString == null || connectString.Length <= 0)
            {
                MessageBox.Show("请填写有效的数据库连接字符串", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                bR = false;
            }
            return bR;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.checkValidation() == false)
            {
                return;
            }
            if (SQLConnConfig.testConnection(type, this.txtConnString.Text))
            {
                MessageBox.Show("连接测试成功！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("连接测试失败！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void initialForm()
        {
            appConfig appconfig = appConfig.getDefaultConfig();
            if (appconfig != null)
            {
                this.type = appconfig.dbType;
            }
            //DBType type = ConfigManager.GetCurrentDBType();
            SQLConnConfig config = SQLConnConfig.getDefaultDBConfig(type);
            //type = config.dbType;
            if (type == DBType.sqlserver)
            {
                this.comboBox1.SelectedIndex = 0;
            }
            else if (type == DBType.sqlite)
            {
                this.comboBox1.SelectedIndex = 1;
            }

            //string connStr = ConfigManager.GetDBConnectString(type);
            if (config != null)
            {
                this.connectString = config.connectString;
            }
            this.txtConnString.Text = connectString;
        }

        private void SQLConfig_Load(object sender, EventArgs e)
        {
            initialForm();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.checkValidation() == false)
            {
                return;
            }
            appConfig appConfig = new appConfig(this.type);
            appConfig.saveConfig(appConfig);
            SQLConnConfig config = new SQLConnConfig(this.type, this.connectString);
            SQLConnConfig.saveConfig(config);

            staticClass.currentDbType = this.type;
            staticClass.currentDBConnectString = this.connectString;

            this.Close();
            //initialForm();
            //switch (this.comboBox1.SelectedIndex)
            //{
            //    case 0:
            //        if (ConfigManager.SaveDBConnectString(DBType.sqlserver, this.txtConnString.Text))
            //        {
            //            MessageBox.Show("保存成功！");
            //        }
            //        else
            //        {
            //            MessageBox.Show("保存失败！");
            //        }
            //        break;
            //    case 1:
            //        if (ConfigManager.SaveDBConnectString(DBType.sqlite, this.txtConnString.Text))
            //        {
            //            MessageBox.Show("保存成功！");
            //        }
            //        else
            //        {
            //            MessageBox.Show("保存失败！");
            //        }
            //        break;
            //}
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SQLConnConfig config = null;
            switch (this.comboBox1.SelectedIndex)
            {
                case 0:
                    //ConfigManager.SaveCurrentDBType(DBType.sqlserver);
                    config = SQLConnConfig.getDefaultDBConfig(DBType.sqlserver);
                    if (config != null)
                    {
                        this.txtConnString.Text = config.connectString;
                    }
                    else
                    {
                        this.txtConnString.Text = string.Empty;
                    }
                    //this.txtConnString.Text = ConfigManager.GetDBConnectString(DBType.sqlserver);
                    break;
                case 1:
                    config = SQLConnConfig.getDefaultDBConfig(DBType.sqlite);
                    if (config!=null)
                    {
                        this.txtConnString.Text = config.connectString;
                    }
                    else
                    {
                        this.txtConnString.Text = string.Empty;
                    }
                    //ConfigManager.SaveCurrentDBType(DBType.sqlite);
                    //this.txtConnString.Text = ConfigManager.GetDBConnectString(DBType.sqlite);
                    break;
            }

        }


    }
}