using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace sever2
{
    public partial class EncryptIP : Form
    {

        public EncryptIP()
        {
            InitializeComponent();
        }

        private void EncryptButton_Click(object sender, EventArgs e)
        {
            /*
            string jiezhi = "2013-10-01";//结束的日期
            DateTime dtstart = System.DateTime.Now;//获得今天的日期
            DateTime dtend = Convert.ToDateTime(jiezhi);//把结束的日期类型转换为DateTime
            if (DateTime.Compare(dtstart, dtend) > 0)
            {
                //DateTime.Compare(t1,t2),方法获取一个数字,如果值小于0,则t1<t2,大于0,则t1>t2, 等于0,则t1=t2
                //过期了
                MessageBox.Show("程序使用时间过期了！请联系技术支持！");
                return;
            }
            else
            {
                //还没有过期
                //加密按钮
                if (NewServerIPTextBox.Text.Length == 0)
                {
                    //如果IP栏为空
                    CheckIPLabel.ForeColor = Color.Red;
                    CheckIPLabel.Text = "*";
                }
                else if (!IsIP(NewServerIPTextBox.Text))
                {
                    //如果填写的不是IP格式
                    CheckIPLabel.ForeColor = Color.Red;
                    CheckIPLabel.Text = "无效IP";
                }
                else
                {
                    //加密
                    EncryptIPAdd aEncryptIPAdd = new EncryptIPAdd();
                    EncryptedServerIPTextBox.Text = "xS7j" + aEncryptIPAdd.encrypt(NewServerIPTextBox.Text);

                }
            }
             * */
            //加密按钮
            if (NewServerIPTextBox.Text.Length == 0)
            {
                //如果IP栏为空
                CheckIPLabel.ForeColor = Color.Red;
                CheckIPLabel.Text = "*";
            }
            else if (!IsIP(NewServerIPTextBox.Text))
            {
                //如果填写的不是IP格式
                CheckIPLabel.ForeColor = Color.Red;
                CheckIPLabel.Text = "无效IP";
            }
            else
            {
                //加密
                EncryptIPAdd aEncryptIPAdd = new EncryptIPAdd();
                EncryptedServerIPTextBox.Text = "xS7j" + aEncryptIPAdd.encrypt(NewServerIPTextBox.Text);

            }
           
        }
        public bool IsIP(string ip)
        {
            //判断是否是正确格式的IP地址
            bool b = true;
            string[] lines = new string[4];
            string s = ".";


            lines = ip.Split(s.ToCharArray(), 4);//分隔字符串 

            for (int i = 0; i < 4; i++)
            {
                try
                {
                    //int ipNum = Convert.ToInt32(lines[i]);
                    int ipNum = -1;
                    if (int.TryParse(lines[i], out ipNum) == false)
                    {
                        //判断是否可以转换为整型
                        b = false;
                        return b;
                    }
                    if (ipNum >= 255 || ipNum < 0)
                    {
                        b = false;
                        return b;
                    }
                }
                catch
                {
                    //如果转换出错，说明不是数字
                    b = false;
                    return b;
                }

            }
            return b;
        }

        private void GiveUpButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
