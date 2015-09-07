using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
namespace sever2
{
    class dateCompare
    {
        public static Boolean dateTimeCompare()
        {
            //比较程序第一次距离现在的时间，如果低于3个月则允许使用

            string path = System.Environment.CurrentDirectory;
            string filePath = path + "\\dt.dat";//1文件，存放程序第一次使用的时间
            string yesterdayTimeFilePath = path + "\\td.dat";//2文件，存放上次启动程序的时间
            //MessageBox.Show(filePath);
            if (File.Exists(filePath))//如果1文件存在
            {
                //先对2文件，也就是上次程序启动的时间与当前时间进行比对
                if (File.Exists(yesterdayTimeFilePath))//如果2文件存在
                {
                    //如果存在则读取2文件中的时间与当前时间进行比对
                    FileStream readYesFs = new FileStream(yesterdayTimeFilePath, FileMode.Open);
                    StreamReader readYesSr = new StreamReader(readYesFs);
                    string xorReadYesTime = readYesSr.ReadLine();
                    readYesSr.Close();
                    readYesFs.Close();
                    string readYesTime = xorEncryptDecrypt(xorReadYesTime,"6");
                    //MessageBox.Show("readYesTime的值为" + readYesTime);
                    DateTime YesDateTime = Convert.ToDateTime(readYesTime);
                    DateTime nowDateTime = System.DateTime.Now;
                    //MessageBox.Show("之前的时间为" + YesDateTime + "\n当前时间为" + nowDateTime);
                    if (DateTime.Compare(YesDateTime, nowDateTime) > 0)//当前时间与2文件时间进行比对
                    {
                        //MessageBox.Show("私自改了系统时间，将系统时间调前了");
                        //如果保存的上次程序的启动时间比现在的当前时间还要晚
                        //说明用户私自改了当前系统的时间，即将系统时间向前调了
                        return false;
                    }
                    else
                    {
                        
                        //将当前的时间写入到2文件中
                        FileStream yesFs = new FileStream(yesterdayTimeFilePath, FileMode.Create);
                        StreamWriter yesSw = new StreamWriter(yesFs);
                        string yesTime = System.DateTime.Now.ToString();
                        string xorYesTime = xorEncryptDecrypt(yesTime, "6");
                        //MessageBox.Show("将当前的时间" + yesTime + "写入到文件2中");
                        yesSw.WriteLine(xorYesTime);
                        yesSw.Flush();
                        yesSw.Close();
                        yesFs.Close();
                    }
                }
                else
                {

                    //如果没有保存上次程序启动时间的文件，则创建2文件并把目前的时间写入
                    FileStream yesFs = new FileStream(yesterdayTimeFilePath,FileMode.Create);
                    StreamWriter yesSw = new StreamWriter(yesFs);
                    string yesTime = System.DateTime.Now.ToString();
                    //MessageBox.Show("创建并把目前的时间写入2文件," + yesTime);
                    string xorYesTime = xorEncryptDecrypt(yesTime,"6");
                    yesSw.WriteLine(xorYesTime);
                    yesSw.Flush();
                    yesSw.Close();
                    yesFs.Close();
                }
                
                //对1文件，也就是系统第一次启动所文件则获取文件里的日期与当前日期进行比对
                FileStream myFs = new FileStream(filePath, FileMode.Open);
                StreamReader mySr = new StreamReader(myFs);
                string firstTimeXor = mySr.ReadLine();
                mySr.Close();
                myFs.Close();
                string firstTime = xorEncryptDecrypt(firstTimeXor, "6");
                DateTime firstDateTime = Convert.ToDateTime(firstTime);//转化为datetime类型
                //MessageBox.Show(firstTime.ToString());
                DateTime currentDate = System.DateTime.Now;
                //MessageBox.Show(currentDate.ToString());
                //如果系统第一次启动时的时间比当前时间还要晚说明，用户更改了系统时间
                if (DateTime.Compare(firstDateTime, currentDate) > 0)
                {
                    //说明用户私自改了当前系统的时间，即将系统时间向前调了
                    //MessageBox.Show("说明用户私自改了当前系统的时间，即将系统时间向前调了" + firstDateTime + "\n" + currentDate);
                    return false;
                }

                TimeSpan firstDateTimeSpan = new TimeSpan(firstDateTime.Ticks);
                TimeSpan currentDateTimeSpan = new TimeSpan(currentDate.Ticks);//返回从0001-12-31-00-00到现在的100纳秒的间隔数
                TimeSpan nowFromFirstTimeSpan = currentDateTimeSpan.Subtract(firstDateTimeSpan).Duration();//当前时间减去开始事件，返回差的绝对值
                int pastDays = nowFromFirstTimeSpan.Days;
                //MessageBox.Show(pastDays.ToString());
                if (pastDays > 60)
                {
                    //如果超过了2个月的时间
                    //MessageBox.Show("超过了2个月的时间" + pastDays);
                    return false;//返回否
                }
                else
                {
                    //MessageBox.Show("没超过2个月的时间" + pastDays);
                    return true;//返回是
                }
            }
      
            else
            {
                //如果不存在就创建1文件并把当前日期写入文件
                DateTime nowDate = System.DateTime.Now;
                //MessageBox.Show(nowDate.ToString());
                FileStream myFs = new FileStream(filePath, FileMode.Create);
                StreamWriter mySw = new StreamWriter(myFs);
                //将nowDate异或加密
                string nowDateXor = xorEncryptDecrypt(nowDate.ToString(),"6");
                //MessageBox.Show(nowDateXor);
                mySw.WriteLine(nowDateXor);
                mySw.Flush();
                mySw.Close();
                myFs.Close();
                return true;
            }
        }
        public static string xorEncryptDecrypt(string sourceStr,string key)
        {
            string targetStr = "";
            byte[] souStrByte = Encoding.Default.GetBytes(sourceStr);
            byte[] keyByte = Encoding.Default.GetBytes(key);
            for (int i = 0; i < souStrByte.Length; i++)
            {
                souStrByte[i] = (byte)(souStrByte[i] ^ keyByte[0]);
            }
            targetStr = Encoding.Default.GetString(souStrByte);
            return targetStr;
        }
    }
}
