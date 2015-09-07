using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;//正则表达式
using System.IO;
namespace sever2
{
    public partial class LatestServerIP : Form
    {
        public LatestServerIP()
        {
            InitializeComponent();
            //初始化时检测有无最新保存的邮箱名密码
            //有则将其取出放到邮箱名和密码文本框中
            string mailAndPasswd = readFile();
            if (mailAndPasswd != null && mailAndPasswd.Length > 1)
            {
                //不为空
                string mailAdd = mailAndPasswd.Substring(mailAndPasswd.LastIndexOf("#") + 1, mailAndPasswd.LastIndexOf("$") - 1);
                string mailPas = mailAndPasswd.Substring(mailAndPasswd.LastIndexOf("$") + 1, mailAndPasswd.Length - mailAdd.Length - 2);
                ReceiveIPMailBoxTextBox.Text = mailAdd;
                ReceiveServerIPMailBoxPasswordTextBox.Text = mailPas;
                ReceiveIPMailBoxTextBox.ReadOnly = true;
                ReceiveServerIPMailBoxPasswordTextBox.ReadOnly = true;
            }
        }

        private void SaveMailBoxInfoButton_Click(object sender, EventArgs e)
        {
            //保存邮箱和密码
            string mailAdd = ReceiveIPMailBoxTextBox.Text;
            string mailPas = ReceiveServerIPMailBoxPasswordTextBox.Text;
            if (mailAdd.Length == 0)
            {
                CheckMailBoxAddLabel.ForeColor = Color.Red;
                CheckMailBoxAddLabel.Text = "*";
                return;
            }
            if (mailPas.Length == 0)
            {
                CheckMailBoxPasswordLabel.ForeColor = Color.Red;
                CheckMailBoxPasswordLabel.Text = "*";
                return;
            }
            if (!IsValidEmail(mailAdd))
            {
                CheckMailBoxAddLabel.ForeColor = Color.Red;
                CheckMailBoxAddLabel.Text = "格式错误";
                return;
            }
            writeFile(mailAdd, mailPas);//保存到文件
            ReceiveIPMailBoxTextBox.ReadOnly = true;
            ReceiveServerIPMailBoxPasswordTextBox.ReadOnly = true;
        }
        //判断邮件是否合法
        bool IsValidEmail(string strIn)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
        public static void writeFile(String aMailAdd, String aPassword)
        {
            //判断文件是否存在，如果不存在就创建该文件
            string aPATH = "userInfo";
            string aFILE = "userInfo/mailInfo.txt";
            if (!Directory.Exists(aPATH))
            {
                //不存在即创建
                Directory.CreateDirectory(aPATH);
            }
            if (!File.Exists(aFILE))
            {
                //不存在即创建
                File.Create(aFILE).Close();

            }

            //将用户名密码写入到文件
            string mailAndPasswd = "#" + aMailAdd + "$" + aPassword + "\n";
            FileStream fs = new FileStream(aFILE, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            sw.Write(mailAndPasswd);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();//关闭流
        }
        public static string readFile()
        {
            string aPATH = "userInfo";
            string aFILE = "userInfo/mailInfo.txt";
            DirectoryInfo TheFolder = new DirectoryInfo(aPATH);

            if (!Directory.Exists(aPATH))
            {
                //不存在即创建
                Directory.CreateDirectory(aPATH);

            }
            if (!File.Exists(aFILE))
            {
                //不存在即创建
                File.Create(aFILE).Close();
            }


            /*
            string[] srr = File.ReadAllLines(aFILE);
            if (srr.Length != 0)
            {
                return srr[srr.Length]; //返回最后一行
            }
            else
            {
                return null;
            }
            */

            //读文本文件最后一行
            FileStream aFile = new FileStream(aFILE, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader aStreamReader = new StreamReader(aFILE);
            string aLine = "";
            string bLine = "";
            while (aLine != null)
            {
                aLine = aStreamReader.ReadLine();
                if (aLine != null && !aLine.Equals(""))
                    bLine = aLine;
            }
            aStreamReader.Close();
            aFile.Close();
            return bLine;

        }

        private void ChangeMailBoxInfoButton_Click(object sender, EventArgs e)
        {
            //点击该按钮文本框的邮箱和密码变为可编辑模式
            ReceiveIPMailBoxTextBox.ReadOnly = false;
            ReceiveServerIPMailBoxPasswordTextBox.ReadOnly = false;
        }

        private void GetLatestServerIPButton_Click(object sender, EventArgs e)
        {
            //使用时先注册
            RegistDll aRegistDll = new RegistDll();
            aRegistDll.DoUpdate();

            string mailAdd = ReceiveIPMailBoxTextBox.Text;
            string mailPas = ReceiveServerIPMailBoxPasswordTextBox.Text;
            //点击获取最新的IP地址，从上一文本框中取出邮箱地址，分析邮件，找到最新的ip地址
            //将其存到文本文件里，并在下面的文本框中显示出来
            //如果在邮箱中查不到IP，则从文本文件里找到最新的IP地址放到显示IP的文本框中
            if (mailAdd.Length == 0)
            {
                CheckMailBoxAddLabel.ForeColor = Color.Red;
                CheckMailBoxAddLabel.Text = "*";
                return;
            }
            if (mailPas.Length == 0)
            {
                CheckMailBoxPasswordLabel.ForeColor = Color.Red;
                CheckMailBoxPasswordLabel.Text = "*";
                return;
            }
            if (!IsValidEmail(mailAdd))
            {
                CheckMailBoxAddLabel.ForeColor = Color.Red;
                CheckMailBoxAddLabel.Text = "格式错误";
                return;
            }

            //根据邮箱地址密码获取邮件
            //receiveMail aReceiveMail = new receiveMail();
            //string mailResult = aReceiveMail.ReceiveByJmail(mailAdd, mailPas);
            //ServerLatestIPTextBox.Text = mailResult;
        }

        private void GiveUpGetIPButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void EncryptServerIPButton_Click(object sender, EventArgs e)
        {
            //点击加密
            if (ServerLatestIPTextBox.Text.Length == 0)
            {
                //如果IP栏为空
                EncryptedServerIPTextBox.Text = "IP不能为空";
                return;
            }
            else if (!IsIP(ServerLatestIPTextBox.Text))
            {
                //如果填写的不是IP格式
                EncryptedServerIPTextBox.Text = "IP格式不对";
                return;
            }
            else
            {
                //加密
                EncryptIPAdd aEncryptIPAdd = new EncryptIPAdd();
                EncryptedServerIPTextBox.Text = "xS7j" + aEncryptIPAdd.encrypt(ServerLatestIPTextBox.Text);

            }
        }
        public bool IsIP(string ip)
        {
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
    }
}
