using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Data.OleDb;

namespace sever2
{
    public partial class UserInfo : Form
    {
        private string phoneNum;
        private string phoneID;
        private string os;
        private string name;
        private string phoneArray;

        public UserInfo()
        {
            InitializeComponent();
        }

        public UserInfo(string _PhonenNum, string _PhoneID, string _OS)
        {
            phoneNum = _PhonenNum;
            phoneID = _PhoneID;
            os = _OS;

            //连结数据库找其它两项信息
            name = "";
            phoneArray = "";
            InitializeComponent();

            OleDbConnectionStringBuilder connectStringBuilder = new OleDbConnectionStringBuilder();
            connectStringBuilder.DataSource = AppDomain.CurrentDomain.BaseDirectory + "server.mdb";
            connectStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";
            DataTable dt = new DataTable();
            OleDbConnection conn = new OleDbConnection(connectStringBuilder.ConnectionString);
            conn.Open();
            string sql = "select * from Client where PhoneNum = '" + phoneNum + "' and PhoneID = '" + 
                phoneID + "' and OS = '" + os + "'";
            OleDbCommand myCmd = new OleDbCommand(sql, conn);
            OleDbDataReader datareader = myCmd.ExecuteReader();

            if(datareader.Read()) {
                phoneArray = datareader["PhoneNumber"].ToString();
                name = datareader["UserName"].ToString();

            }
            conn.Close();

            this.PhoneNumText.Text = phoneNum;
            this.PhoneIDText.Text = phoneID;
            this.SystemType.Text = os;
            this.PhoneArray.Text = phoneArray;
            this.UserName.Text = name;
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            OleDbConnectionStringBuilder connectStringBuilder = new OleDbConnectionStringBuilder();
            connectStringBuilder.DataSource = AppDomain.CurrentDomain.BaseDirectory + "server.mdb";
            connectStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";

            phoneArray = this.PhoneArray.Text;
            name = this.UserName.Text;

            DataTable dt = new DataTable();
            OleDbConnection conn = new OleDbConnection(connectStringBuilder.ConnectionString);
            conn.Open();
            string sql = "update Client set PhoneNumber = '" + phoneArray + "', UserName = '" + 
                name + "' where PhoneNum = '" + phoneNum + "' and PhoneID = '" + phoneID + "' and OS = '" + os + "'";
            OleDbCommand myCmd = new OleDbCommand(sql, conn);
            myCmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}
