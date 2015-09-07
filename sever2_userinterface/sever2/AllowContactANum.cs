using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace sever2
{
    public partial class AllowContactANum : Form
    {
        public AllowContactANum()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //生成命令
            if (textBox1.Text.Length == 0)
            {
                label3.ForeColor = Color.Red;
                label3.Text = "请输入目标号码";
            }
            else
            {
                //将电话号码加密
                String prohibitPhoneNum = phoneNumToStr(textBox1.Text);
                textBox2.Text = "UR4f" + prohibitPhoneNum;
                label3.ForeColor = Color.Green;
                label3.Text = "复制命令发送到目标手机";
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
