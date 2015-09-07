using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Web;
using System.Data.OleDb;



namespace sever2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ADDuser m = new ADDuser();
            m.Show();

        }

      /*  private string form2zhi = null;
        public string Form2ChuanZhi
        {
            get
            {
                return form2zhi;
            }
        }*/


        #region treeView 的双击事件
        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            Phone m = new Phone();
     //       form2zhi = treeView1.SelectedNode.Text;
      //      m.Form3ChuanZhi = form2zhi;
            m.Show();


        }

        #endregion    

        
        public void GetTree()
        {
            
            string PhoneNum, PhoneID,Flag;
           
            OleDbConnectionStringBuilder connectStringBuilder = new OleDbConnectionStringBuilder();
            connectStringBuilder.DataSource = AppDomain.CurrentDomain.BaseDirectory + "server.mdb";
            connectStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";
            DataTable dt = new DataTable();
            OleDbConnection conn = new OleDbConnection(connectStringBuilder.ConnectionString);
            conn.Open();
            string sql = "select * from Client ";
            OleDbCommand myCmd = new OleDbCommand(sql, conn);
            OleDbDataReader datareader = myCmd.ExecuteReader();
            while (datareader.Read())
            {
               
                PhoneNum = datareader["PhoneNum"].ToString();
                PhoneID = datareader["PhoneID"].ToString();
                Flag = datareader["Flag"].ToString();
                if (Flag.CompareTo("On") != 0)
                  treeView1.Nodes.Add(PhoneNum, PhoneNum, 20);
                else
                    treeView1.Nodes.Add(PhoneNum, PhoneNum, 19);                           
             
               
            }

            conn.Close();

          
        
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            treeView1.SelectedImageIndex = treeView1.SelectedNode.ImageIndex;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GetTree();
        }
    }
}