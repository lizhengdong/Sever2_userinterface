using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Data.OleDb;
using System.Diagnostics;

namespace sever2
{
    public partial class ADDuser : Form
    {
        private string PhonenNum;
        private string PhoneID;
        private string ComTime;
        private string Flag;

        public ADDuser()
        {
            
        }
        public ADDuser(string _PhonenNum, string _PhoneID,  string _ComTime, string _Flag)
        {
            PhonenNum = _PhonenNum;
            PhoneID = _PhoneID;
            ComTime = _ComTime;
            Flag = _Flag;
            InitializeComponent();

        }
        
        public string SetCom;
      

        private void button1_Click(object sender, EventArgs e)
        {  
            try
            {
                if (SetInterval.Text != null)
                {
                    int set = Int32.Parse(SetInterval.Text);
                    SetCom = "SetInterval" + SetInterval.Text;                 

                    Interval(PhonenNum, PhoneID, SetCom, ComTime, Flag);
                    
                }
                else
                    MessageBox.Show("��������������ֵ��");
            }
            catch 
            {
                MessageBox.Show("��������������ֵ��");
               
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
          this.Close();
        }
       public void Interval(string PhoneNum,string PhoneID,string Content,string ComTime,string Flag)
       {
         string sql = "insert into Command(PhoneIMSI,PhoneID,Content,ComTime,Flag) values('" + PhoneNum + "','" + PhoneID + "','" + Content + "','" + ComTime + "','" + Flag + "')";
         string sql1 = "insert into ALLCommand(PhoneIMSI,PhoneID,Content,ComTime,Flag) values('" + PhoneNum + "','" + PhoneID + "','" + Content + "','" + ComTime + "','" + Flag + "')";
                
         IntoDatabase1(PhoneNum, PhoneID, Content, ComTime, Flag, sql);//�������񵽡�ִ���С��б���
         IntoDatabase1(PhoneNum, PhoneID, Content, ComTime, Flag, sql1);//�������񵽡�ȫ�������б���

         MessageBoxTimeOut temp1 = new MessageBoxTimeOut();
         temp1.Show("�������", "������ʾ", 1500);
         this.Close();
       }

      
       private void IntoDatabase1(string t1, string t2, string t3, string t4, string t5, string sql)
       {
           try
           {
               OleDbConnectionStringBuilder connectStringBuilder = new OleDbConnectionStringBuilder();
               connectStringBuilder.DataSource = AppDomain.CurrentDomain.BaseDirectory + "server.mdb";
               connectStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";
               OleDbConnection conn = new OleDbConnection(connectStringBuilder.ConnectionString);
               conn.Open();
           
               OleDbCommand cmd = new OleDbCommand(sql, conn);
               cmd.ExecuteNonQuery();
       
               conn.Close();
           }
           catch (System.Data.OleDb.OleDbException)
           {
           }
       }
    }
}