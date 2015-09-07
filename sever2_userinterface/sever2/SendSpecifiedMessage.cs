using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace sever2
{
    public partial class SendSpecifiedMessage : Form
    {
        public SendSpecifiedMessage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //点击按钮生成命令
            if (textBox1.Text.Length == 0 || textBox2.Text.Length == 0)
            {
                label3.ForeColor = Color.Red;
                label3.Text = "请输入手机号和短信内容";
            }
            else
            {
                //将短信内容加密
                //char key = '8';//密钥
                String tb1 = phoneNumToStr(textBox1.Text);
                String tb2 = StringAddOne(textBox2.Text);
                textBox3.Text = "dH7k" + "#" + tb1 + "#" +  tb2;
                label3.ForeColor = Color.Green;
                label3.Text = "复制该文本框中的命令发送给目标手机。";
            }
        }
        /*
        private string encryptDecryptStr(string p,char key)
        {
            byte[] bs = Encoding.Default.GetBytes(p);
            for (int i = 0; i < bs.Length; i++)
            {
                bs[i] = (byte)(bs[i] ^ (int)key);
            }
            return Encoding.Default.GetString(bs);
        }
        */
        public String StringAddOne(String value)
        {
            //汉字字符加一
            char[] ch = value.ToCharArray();
            for (int i = 0; i < ch.Length; i++)
            {
                ch[i] = (char)((int)ch[i] + 1);
            }
            return new String(ch,0,ch.Length);
        }
        public String phoneNumToStr(String phoneNum)
        {
            //电话号码转字符串
            char[] ch = phoneNum.ToCharArray();
            for (int i = 0; i < ch.Length; i++)
            {
                ch[i] = NumToChar((int)ch[i]);
            }
            return new String(ch,0,ch.Length);
        }
        public char NumToChar(int aNum)
        {
            char a = Convert.ToChar(aNum + 50);
            //数字转字符
            return a;
        }
        public int CharToNum(char ch)
        {
            //字符转数字
            int num = Convert.ToInt32(ch) - 50;
            return num;
        }
    }
}
