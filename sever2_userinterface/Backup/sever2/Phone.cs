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
            toolStripStatusLabel1.Text = "׼������";
            lis = true;        
         //   jiazai();  //�ĵ��������ļ�Ŀ¼���ļ���
            GetTree(); //���ؿͻ�����Ϣ             
            login();   //֪ͨ�����������ƶ�����
            TOrder();  //���������߳�
            Recevice();//�����ļ�����     
            yuyinS();  //����Symbian��������          

        }

        #region ȫ�ֱ���
        Socket client;
        FileStream MyFileStream;
        TcpListener lisner;
        TcpListener lisner1;
        bool lis;
        static string P2P = "202.004.155.187";
        //static string P2P = "123.118.173.192";
      
        IPEndPoint TranTo = new IPEndPoint(IPAddress.Parse(P2P), 4000);
        string zaixian = null;
        Socket SocketS = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//�ļ������� 
        string FileNow;           //��¼��ǰ����д����ļ���
      
        #endregion

        #region  ���ؿͻ�����Ϣ

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

                PhoneNum = datareader["PhoneNum"].ToString();         //IMSI��
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
                    //����Ƿ���� ������ʮ�� ����ΪOff
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
                        if (PhoneNum.CompareTo(SelectBefore) == 0) //Ϊˢ��ǰѡ�еĿͻ���
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

        #region  ����ע��

        //ÿ��5�뷢��һ�� UDPע��
        private void login1()
        {
            IPEndPoint stemp1 = new IPEndPoint(IPAddress.Any, int.Parse("8000"));
            IPEndPoint sever = new IPEndPoint(IPAddress.Parse(P2P), int.Parse("6000"));

            UdpClient tl = new UdpClient(stemp1);
            Byte[] send = Encoding.ASCII.GetBytes("Moniter");
            tl.Send(send, send.Length, sever);
            tl.Close();

        }
        //TCPע��
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
                        //���ܿ��ƶ˵���Ϣ


                        byte[] tempKZ = TransferFiles.ReceiveVarData(Log);
                        if (tempKZ.Length == 0)

                            toolStripStatusLabel4.Text = "������δ����";
                        else
                            toolStripStatusLabel4.Text = "��¼�������ɹ�";
                        break;


                    }

                }


                catch
                {
                    
                    toolStripStatusLabel4.Text = "������δ����";

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

                //�����ֻ��˴������ֻ����ӣ����
                while (true)
                {
                    byte[] temp = TransferFiles.ReceiveVarData(SocketS);

                    string temp2 = System.Text.Encoding.ASCII.GetString(temp);
                    //     MessageBox.Show(temp2 + "�ֻ�����A");

                    if (temp.Length == 0)
                        //if (SocketS.Available == 0)  
                        break;
                    else
                    {
                        string[] pinfor = temp2.Split('@');   //����1031
                        // textBox2.Text = pinfor[0];
                        //  textBox1.Text = pinfor[1];
                        string OS = pinfor[0];
                        string PhoneNum = pinfor[1];  //IMSI��
                        string PhoneID = pinfor[2];   //���к�
                       
                        ClientOnline(PhoneNum, PhoneID,OS);      //�ڽ�������ʾ�ֻ�����״̬
                        string[] a = new string[] { };

                        a = GetCommand(PhoneID);              //ÿ���յ��ֻ�ע��ͻ�ȡ�Ƿ��д��ֻ�������

                        if (a != null)                        //�д��ֻ�������ʱ
                        {
                            i = 0;
                            while (a[i] != null)
                            {
                                if (a[i].CompareTo("Voice") != 0)
                                {
                                    byte[] msg = Encoding.UTF8.GetBytes(a[i]);
                                    MessageBox.Show("���ֻ���" + PhoneID + "��������" + a[i]);
                                    TransferFiles.SendVarData(SocketS, msg);
                                    i++;
                                }
                                else                                                          //����������������ʱ
                                {

                                    byte[] msg = Encoding.UTF8.GetBytes(a[i]);
                                    MessageBox.Show("���ֻ���" + PhoneID + "��������" + a[i]);
                                    TransferFiles.SendVarData(SocketS, msg);

                                    //������ͬϵͳ���д���
                                    FileVoiceCreate(PhoneID, PhoneNum,OS);
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
                                    }
                                    if (OS.CompareTo("Mobile") == 0)
                                    {                                       
                                       Interface b = new Interface();                           //Windows Mobile����������  
                                        b.Ini(FileNow);
                                    }
                                    if (OS.CompareTo("Android")==0)                              //Android��������
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

        #region ���ܺ��� ��ʼ�����ļ�
        //����߳̽����ļ�
        private void StartReceive()
        {
            Form.CheckForIllegalCrossThreadCalls = false;
            //����һ������˵�
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, int.Parse("2005"));
            //�����������
            lisner = new TcpListener(ipep);
            //���Գ���
            lisner.Start();
            try
            {
                while (lis)
                {
                    ////ȷ������
                    if (!lisner.Pending())
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    //  �����ֻ��˴������ļ���Ϣ
                    client = lisner.AcceptSocket();
                    //ÿ����һ���ļ����ӣ��ʹ���һ����Ӧ���̴߳����ļ��Ľ���
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
                    toolStripStatusLabel1.Text = "�Ѿ�����";

                    //���[�ļ���]                    
                    string SendFileName = System.Text.Encoding.ASCII.GetString(TransferFiles.ReceiveVarData(client));
                    if (String.Compare(SendFileName, String.Empty) == 0)
                        break;
                    MessageBox.Show(SendFileName);
                    //���[�ֻ���]
                    string info = System.Text.Encoding.ASCII.GetString(TransferFiles.ReceiveVarData(client));
                    string[] pinfor = info.Split('@');   //����1031                   
                    string PhoneNum = pinfor[0];
                    string PhoneID = pinfor[1];
                    //����һ�����ļ�
                    string path = FileCreateSub(PhoneID);
                    string RTime = System.DateTime.Now.ToShortDateString();
                    SendFileName = PhoneNum + "@" + RTime + SendFileName;
                    string fileFullName = path + "\\" + SendFileName;

                    //  FileStream
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

                    //��ӵ�dgv��
                    // �ֻ��ţ��ļ������ļ���С�����ʱ��                
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



        #region   ����Windows��Ϣ,�رմ���ʱִ��
        protected override void WndProc(ref   Message m)
        {

            const int WM_SYSCOMMAND = 0x0112;
            const int SC_CLOSE = 0xF060;
            if (m.Msg == WM_SYSCOMMAND && (int)m.WParam == SC_CLOSE)
            {//��׽�رմ�����Ϣ   
                //   User   clicked   close   button   
                //this.WindowState = FormWindowState.Minimized;//�����ϽǺ��رհ�ť����С��

                ServiceStop();
            }
            base.WndProc(ref   m);
        }
        #endregion


        #region ֹͣ����

        //ֹͣ����
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

            toolStripStatusLabel1.Text = "���ӹر�";
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

        #region ���ݿ�Ĳ���

        //   E:\����\ѧϰ\�о���\��Ŀ\������\����\����\sever2\sever2\sever2\server.mdb
        //�����ͻ��ˣ������ݲ���client����
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
            // toolStripStatusLabel4.Text = "������";
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
            // toolStripStatusLabel4.Text = "������";
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
                        MessageBox.Show("ֻ�ɼ���һ���ն�");
                }


                if (Content != null)
                {
                    string sql = "insert into Command(PhoneIMSI,PhoneID,Content,ComTime,Flag) values('" + PhoneNum + "','" + PhoneID + "','" + Content + "','" + ComTime + "','" + Flag + "')";
                    string sql1 = "insert into ALLCommand(PhoneIMSI,PhoneID,Content,ComTime,Flag) values('" + PhoneNum + "','" + PhoneID + "','" + Content + "','" + ComTime + "','" + Flag + "')";
                    //  string sql = "insert into Command(PhoneNum,Content,ComTime,Flag) values('" + PhoneNum + "','" + Content + "','" + ComTime + "','" + Flag + "')";
                    // string sql1 = "insert into ALLCommand(PhoneNum,Content,ComTime,Flag) values('" + PhoneNum + "','" + Content + "','" + ComTime + "','" + Flag + "')";

                    IntoDatabase1(PhoneNum, PhoneID, Content, ComTime, Flag, sql);//�������񵽡�ִ���С��б���
                    IntoDatabase1(PhoneNum, PhoneID, Content, ComTime, Flag, sql1);//�������񵽡�ȫ�������б���
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



      
        
        #region Symbian�����Ĳ���
        string MyFileName = AppDomain.CurrentDomain.BaseDirectory + "1.amr";
        string MyFileName2 = AppDomain.CurrentDomain.BaseDirectory + "2.amr";
        string MyFileName11 = AppDomain.CurrentDomain.BaseDirectory + "1.wav";
        string MyFileName21 = AppDomain.CurrentDomain.BaseDirectory + "2.wav";
        string MyFileTest = AppDomain.CurrentDomain.BaseDirectory + "test.amr";

        
        IPEndPoint nv = new IPEndPoint(IPAddress.Any, 2007);
        int sign = 1;//�����ļ���־λ
        int play = 0;//�����ļ���־λ
        int i1, j = 0;        
        string sTemp;
        FileStream fs1;
        BinaryWriter wb1;
        FileStream fs2;
        BinaryWriter wb2;
     //   UdpClient receive = new UdpClient(new IPEndPoint(IPAddress.Any, 2007));
        IPEndPoint RemoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);//Զ�̷��ͽڵ�    
        MusicPlayer music = new MusicPlayer();   //0220
        public int temp = 0;//0220
        bool OneTime = true;//��ʼ�ļ�1.amr����Ϊ����Ϊ5K
        int flag = 1;
       // IPEndPoint  new IPEndPoint(IPAddress.Any, 2007)



        private void yuyinS()
        {

           /* Thread TempThreadS = new Thread(new ThreadStart(this.ReceiveFile));//���������ļ��߳�
            TempThreadS.IsBackground = true;
            TempThreadS.Start();*/
            Thread TempThreadS = new Thread(new ThreadStart(this.ReceiveFile1));//���������ļ��߳�
            TempThreadS.IsBackground = true;
            TempThreadS.Start();
            Thread playVoice = new Thread(new ThreadStart(this.playvoice1));//���������ļ��߳�
            playVoice.IsBackground = true;
            playVoice.Start();      
        }
        //UDP����Զ�̴������������������AMR��ʽ���ļ�
        public void ReceiveFile1()  //20110108ԭʼ����
        {
            IPEndPoint symTemp = new IPEndPoint(IPAddress.Any, 2007);
            Socket symbiany = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            symbiany.Bind(symTemp);
            try
            {

                while (true)
                {

                    //Byte[] receiveBytes = receive.Receive(ref RemoteIPEndPoint);//����Զ�̽ڵ�ķ�������
                    Byte[] receiveBytes = new Byte[2100];
                    symbiany.Receive(receiveBytes, receiveBytes.Length, 0);


                    toolStripStatusLabel1.Text = "Symbian��������";
                    if (FileNow == null)
                        break;                  
                    RecordS(receiveBytes);//������¼����                  
                    if (sign == 1)  //��1.amr�������ļ�
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
                toolStripStatusLabel1.Text = "Symbian����ֹͣ";
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

                        if (OneTime == true)  //��һ��
                        {
                          //  Byte[] receiveBytes = receive.Receive(ref RemoteIPEndPoint);//����Զ�̽ڵ�ķ�������
                            Byte[] receiveBytes = new Byte[1400];
                            symbiany.Receive(receiveBytes, receiveBytes.Length, 0);

                            if (FileNow == null)
                                break;
                            toolStripStatusLabel1.Text = "Symbian��������";
                            RecordS(receiveBytes);//������¼����             

                            //ʵʱ���� 
                            
                            if (!File.Exists(MyFileName))
                                CreatFileHead(MyFileName);
                            FileInfo f1 = new FileInfo(MyFileName);
                            i1 = Int32.Parse(f1.Length.ToString());
                            if (i1 < 5 * 1024)  //��ʼ���ñ���·��Ϊ1.amr,�ļ���СΪ5K������
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
                                //��1.amrת����1.wav
                                string path = AppDomain.CurrentDomain.BaseDirectory + "test1.exe";
                                System.Diagnostics.Process.Start(path);
                                Thread.Sleep(4000);  //0221    
                                //play = 1;//���ű�־Ϊ1 ���������Բ���1.amr                                
                                //sign = 2;
                                //MessageBox.Show(MyFileName11);
                                System.Diagnostics.Process.Start("WAVPlayer.exe", MyFileName11);
                                OneTime = false;
                            }
                           

                        }//end of onetime
                        else
                        {
                         //   Byte[] receiveBytes = receive.Receive(ref RemoteIPEndPoint);//����Զ�̽ڵ�ķ�������
                            Byte[] receiveBytes = new Byte[1400];
                            symbiany.Receive(receiveBytes, receiveBytes.Length, 0);

                            if (FileNow == null)
                                break;
                            toolStripStatusLabel1.Text = "Symbian��������";
                            RecordS(receiveBytes);//������¼����

                            if (flag == 1)
                            {//д2 
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
                            {//д1
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

                                if (!File.Exists(MyFileName11))//1�����ڣ���ʼ��2
                                {
                                    File.Delete(MyFileName);//ɾ��1.amr
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

                                if (!File.Exists(MyFileName21))   //2�����ڣ���ʼ��1                
                                {

                                    File.Delete(MyFileName2);//ɾ��2.amr

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
                    toolStripStatusLabel1.Text = "Symbian����ֹͣ";
                }
            }//end of while     
        }//end of func   
        string path;
        int TempName = 0;
        
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
       // Symbian��������
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
        
        
       
        //Symbian¼������ĺ���
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

        public void CreatFileHeadM(string MyFileName)  //����һ���ļ�����Ԥ��д��MAV�Ķ����Ƶ�ͷ
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

        public void login() //���ƶ����ߣ�֪ͨ������
        {
            sendMy = new Thread(new ThreadStart(loginT));
            sendMy.IsBackground = true;
            sendMy.Start();
        }
        private void TOrder()  //����ķ���
        {
            ThreadTrany = new Thread(new ThreadStart(Order));
            ThreadTrany.IsBackground = true;
            ThreadTrany.Start();
            if (this.IsDisposed)
                return;

        }
        private void Recevice()  //�ļ��Ľ���
        {
            lis = true;
            TempThread = new Thread(new ThreadStart(this.StartReceive));
            TempThread.IsBackground = true;
            TempThread.Start();
            toolStripStatusLabel1.Text = "���ڼ���������";



        }

        public string FileCreateSub(string file)
        {
            //����ÿ���ֻ���Ӧ���ļ���
            string path = AppDomain.CurrentDomain.BaseDirectory + "ReceviceFile//" + file;
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "ReceviceFile//" + file))
            {
                //  MessageBox.Show("directory exists");  //C#�����ļ���  
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

            if (hv == false)  //û�м�¼ʱ
            {

                string sqlI = "insert into client (PhoneNum,PhoneID,SendTime,Flag,OS) values('" + PhoneNum + "','" + PhoneID + "','" + ComTime + "','" + Flag + "','" + OS + "')";
                IntoDatabase1(PhoneNum, PhoneID, ComTime, Flag,OS, sqlI);
            }

        }


        private void timer2_Tick(object sender, EventArgs e)
        {
            GetTreeTest();

        }

        #region �����ĵ��������״�ṹ
        private void jiazai()
        {
            /*string[] drives = Directory.GetLogicalDrives();//�õ������ϵ�������
            foreach (string drive in drives)//ѭ��
            {
                MyNode mn = new MyNode(drive, true);
                this.treeView9.Nodes.Add(mn);//�������������TREEVIEW�ؼ���
            }*/

            //�õ������ļ���Ŀ¼
            string ReceviceFile = AppDomain.CurrentDomain.BaseDirectory + "ReceviceFile";
            
            MyNode mn = new MyNode(ReceviceFile, true);
            this.treeView9.Nodes.Clear();//0222
            this.listView4.Clear();//0222
            this.treeView9.Nodes.Add(mn);


            this.listView4.Columns.Add("����", this.listView4.Width / 3, HorizontalAlignment.Center);
            this.listView4.Columns.Add("��С", this.listView4.Width / 4, HorizontalAlignment.Center);
            this.listView4.Columns.Add("����", this.listView4.Width / 6, HorizontalAlignment.Center);
            this.listView4.Columns.Add("�޸�ʱ��", this.listView4.Width / 6, HorizontalAlignment.Center);
            this.listView4.Columns.Add("����λ��", this.listView4.Width / 6, HorizontalAlignment.Center);
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

                            t = temp2.ToString() + "MB";

                        }
                        else if (temp1 < 1)
                        {
                            t = "1KB";
                        }
                        else t = temp1.ToString() + "KB";


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
            //   TreeNode tnx=e.Node;//��ȡѡ�е����ڵ�
            //   foreach(TreeNode t in tnx.Nodes)//ѭ������ڵ��µ��ӽڵ�
            //   {
            //    t.Checked=e.Node.Checked;//ѡ�����е��ӽڵ�
            //   }

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

        
       
    }
}

        
       

       
      
