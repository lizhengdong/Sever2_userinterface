using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace sever2
{
    class RegistDll
    {
        public void DoUpdate()
        {
            //在注册路径前后加上双引号，以免注册时路径出现空格，发生崩溃
            string strPath = "\"" + Application.StartupPath;
            //IniFile ini = new IniFile(strPath + @"/sys.ini" + "\"");
            //注册Jmail
            /*
            if (ini.IniReadValue("System", "AndroidTalkComReg").ToString() != "1")
            {
                if (DllReg(strPath))
                {
                    ini.IniWriteValue("System", "AndroidTalkComReg", "1");
                }
                else
                {
                    MessageBox.Show("没有自动注册AndroidTalkCom.dll，请手动注册");
                }
            }
             * */
            DllReg(strPath);
        }
        public static Boolean DllReg(string targetdir)
        {
            try
            {
                try
                {
                    string registJmail = "regsvr32 /s " + targetdir + "\\" + "jmail.dll" + "\"";
                    Process p = new Process();
                    p.StartInfo.FileName = @"C:\WINDOWS\system32\cmd.exe ";
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardInput = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    p.StandardInput.WriteLine(@registJmail);　　//执行注册AndroidTalkCom.dll的命令
                    p.StandardInput.WriteLine("exit");
                    p.WaitForExit();
                    p.Close();
                    p.Dispose();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("没有自动注册Jmail，请手动注册");
                return false;

            }
        }
    }
}
