using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace sever2
{
    public partial class ServerIP : Form
    {
        String configFile = AppDomain.CurrentDomain.BaseDirectory + "ServerIP.config";

        public ServerIP()
        {
            InitializeComponent();
        }

        private void 修改服务器IP地址_Load(object sender, EventArgs e)
        {
            //读取IP地址，并显示       
            String address = "";
           
            StreamReader fileStream = null;
            try
            {
                fileStream = new StreamReader(configFile, System.Text.Encoding.Default);
                while (!fileStream.EndOfStream)
                {
                    address += fileStream.ReadLine();
                }
                fileStream.Close();
                fileStream.Dispose();
            }
            catch
            {
                fileStream.Close();
                fileStream.Dispose();
                MessageBox.Show("加载IP失败");
            }

            ServerIPAddr.Text = address;
        }

        private void serverQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void serverSave_Click(object sender, EventArgs e)
        {
            if (!checkIP())
            {
                return;
            }
            //处理修改服务器IP程序
            String address = ServerIPAddr.Text; 
            //写到文件中
            FileStream stream = null;
            bool change = false;
            try
            {
                stream = new FileStream(configFile, FileMode.Truncate);
                byte[] content = System.Text.Encoding.Default.GetBytes(address);
                stream.Write(content, 0, content.Length);
                //stream.SetLength(0);
                stream.Flush();
                stream.Close();
                change = true;
            }
            catch
            {
                stream.Close();
                MessageBox.Show("修改IP失败！");
            }
            if (change)
            {
                Application.Restart();
                this.Close();
            }
        }

        public bool checkIP()
        {

            String ipnew = ServerIPAddr.Text;
           

            //检查IP合法性
            string[] ip = ipnew.Split('.');
            if (ip.Length != 4)
            {
                MessageBox.Show("IP输入错误，请重新输入");
                return false;
            }
            for (int i = 0; i < 4; ++i)
            {
                ip[i] = ip[i].TrimStart('0');
                if (ip[i] == "") ip[i] = "0";
                try
                {
                    int value = int.Parse(ip[i]);
                    if (value < 0 || value > 255)
                    {
                        MessageBox.Show("IP输入错误，请重新输入");
                        return false;
                    }
                } catch (Exception e) {
                    return false;
                }
                   
            }
            return true;
        }

    }
}
