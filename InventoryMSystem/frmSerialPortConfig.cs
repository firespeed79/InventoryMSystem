using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using Config;

namespace InventoryMSystem
{
    public partial class frmSerialPortConfig : Form
    {
        public frmSerialPortConfig()
        {
            InitializeComponent();
        }

        private void SerialPortConfig_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            cmbPortName.Items.AddRange(ports);

            LoadConfig();

        }
        void LoadConfig()
        {
            try
            {
                serialPortConfig config = serialPortConfig.getDefaultConfig();
                if (config == null)
                {
                    return;
                }
                string portname = config.portName;
                if (string.Empty == portname)
                {
                    cmbPortName.SelectedIndex = -1;
                }
                else
                {

                    cmbPortName.SelectedIndex = cmbPortName.Items.IndexOf(portname);
                }
                string baudRate = config.baudRate;
                if (string.Empty != baudRate)
                {
                    cmbBaudRate.SelectedIndex = cmbBaudRate.Items.IndexOf(baudRate);
                }
                else
                {
                    cmbBaudRate.SelectedIndex = -1;
                }
                string parity = config.parity;
                if (string.Empty != parity)
                {
                    cmbParity.SelectedIndex = cmbParity.Items.IndexOf(parity);
                }
                else
                {
                    cmbParity.SelectedIndex = -1;
                }
                string stopbites = config.stopBits;
                if (string.Empty != stopbites)
                {
                    cmbStopBits.SelectedIndex = cmbStopBits.Items.IndexOf(stopbites);
                }
                else
                {
                    cmbStopBits.SelectedIndex = -1;
                }
                string databits = config.dataBits;
                if (string.Empty != databits)
                {
                    cmbDataBits.SelectedIndex = cmbDataBits.Items.IndexOf(databits);
                }
                else
                {
                    cmbDataBits.SelectedIndex = -1;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSaveConfig_Click(object sender, EventArgs e)
        {
            //ConfigManager.SaveSerialPortConfigurnation(cmbPortName.Text,
            //                                 cmbBaudRate.Text,
            //                                 cmbParity.Text,
            //                                 cmbDataBits.Text,
            //                                 cmbStopBits.Text);


            serialPortConfig config = new serialPortConfig("default", cmbPortName.Text,
                                  cmbBaudRate.Text,
                                  cmbParity.Text,
                                  cmbDataBits.Text,
                                  cmbStopBits.Text);
            serialPortConfig.saveConfig(config);
            //MessageBox.Show("保存完成！", "串口设置");
            StaticSerialPort.resetStaticSerialPort();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

}
