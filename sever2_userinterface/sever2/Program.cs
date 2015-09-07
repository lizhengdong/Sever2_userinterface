using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using sever2;
using System.IO;
namespace sever2li
{
    static class Program
    {
        
        //检测是否有加密狗
        [DllImport("PhoneDevice.dll", EntryPoint = "TestDevice")]
        public static extern bool TestDevice();
        
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
       
        static void Main()
        
        {
            #region 判断是否有加密狗
            //bool device = TestDevice();
            //if (!device)
            //{
            //    MessageBox.Show("没有加密狗或加密狗错误！");
            //    //return;
            //}
            //else
            //{
            //    Application.EnableVisualStyles();
            //    Application.SetCompatibleTextRenderingDefault(false);
            //    //Application.Run(new Form1());
            //    Application.Run(new Phone());
            //}
            #endregion
            #region 日期比较
            try
            {
                if (!dateCompare.dateTimeCompare())
                {
                    //如果已经过期
                    MessageBox.Show("程序使用时间过期了！请联系技术支持！");
                    return;
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
            #endregion

            #region 日期限制判断
            //添加一个时间限制，直到10月1日前可用，10月1日后不能用

            string jiezhi = "2014-09-01";//结束的日期
            DateTime dtstart = System.DateTime.Now;//获得今天的日期
            DateTime dtend = Convert.ToDateTime(jiezhi);//把结束的日期类型转换为DateTime

            try
            {
                TimeSpan tsstart = new TimeSpan(dtstart.Ticks);//将日期转化为可以比较的类型  
                TimeSpan tsend = new TimeSpan(dtend.Ticks);
                TimeSpan ts = tsend.Subtract(tsstart).Duration();//结束日期减去当前日期  
                //dateTerm = ts.Days.ToString() + "天"  
                //        + ts.Hours.ToString() + "小时"  
                //        + ts.Minutes.ToString() + "分钟"  
                //        + ts.Seconds.ToString() + "秒";  

                string leftDays = ts.Days.ToString();
                //MessageBox.Show(leftDays);

            }
            catch
            {

            }

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
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                //Application.Run(new Form1());
                Application.Run(new Phone());
            }
            #endregion

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            ////Application.Run(new Form1());
            //Application.Run(new Phone());
        }
        
    }
    
}