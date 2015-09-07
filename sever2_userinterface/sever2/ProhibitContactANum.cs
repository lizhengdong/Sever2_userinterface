using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace sever2
{
    public partial class ProhibitContactANum : Form
    {
        public ProhibitContactANum()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //点击生成命令按钮
            if (textBox1.Text.Length == 0)
            {
                label3.ForeColor = Color.Red;
                label3.Text = "请输入要阻止的手机号";
            }
            else
            {
                //将电话号码加密
                String prohibitPhoneNum = phoneNumToStr(textBox1.Text);
                textBox2.Text = "j9P5" + prohibitPhoneNum;
                label3.ForeColor = Color.Green;
                label3.Text = "复制命令发送给目标手机";
            }
        }
        public String phoneNumToStr(String phoneNum)
        {
            //电话号码转字符串
            char[] ch = phoneNum.ToCharArray();
            for (int i = 0; i < ch.Length; i++)
            {
                ch[i] = NumToChar((int)ch[i]);
            }
            return new String(ch, 0, ch.Length);
        }
        public char NumToChar(int aNum)
        {
            char a = Convert.ToChar(aNum + 50);
            //数字转字符
            return a;
        }
    }
}
