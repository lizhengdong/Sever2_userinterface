namespace sever2
{
    partial class LatestServerIP
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.CheckLatestServerIP = new System.Windows.Forms.GroupBox();
            this.CheckMailBoxPasswordLabel = new System.Windows.Forms.Label();
            this.CheckMailBoxAddLabel = new System.Windows.Forms.Label();
            this.ServerLatestIPTextBox = new System.Windows.Forms.TextBox();
            this.ServerLatestIPLabel = new System.Windows.Forms.Label();
            this.GiveUpGetIPButton = new System.Windows.Forms.Button();
            this.GetLatestServerIPButton = new System.Windows.Forms.Button();
            this.ChangeMailBoxInfoButton = new System.Windows.Forms.Button();
            this.SaveMailBoxInfoButton = new System.Windows.Forms.Button();
            this.ReceiveServerIPMailBoxPasswordTextBox = new System.Windows.Forms.TextBox();
            this.ReceiveIPMailBoxTextBox = new System.Windows.Forms.TextBox();
            this.ReceiveServerIPMailBoxPasswordLabel = new System.Windows.Forms.Label();
            this.ReceiveIPMailBoxLabel = new System.Windows.Forms.Label();
            this.EncryptServerIP = new System.Windows.Forms.GroupBox();
            this.EncryptedServerIPTextBox = new System.Windows.Forms.TextBox();
            this.EncryptedServerIPLabel = new System.Windows.Forms.Label();
            this.EncryptServerIPButton = new System.Windows.Forms.Button();
            this.CheckLatestServerIP.SuspendLayout();
            this.EncryptServerIP.SuspendLayout();
            this.SuspendLayout();
            // 
            // CheckLatestServerIP
            // 
            this.CheckLatestServerIP.Controls.Add(this.CheckMailBoxPasswordLabel);
            this.CheckLatestServerIP.Controls.Add(this.CheckMailBoxAddLabel);
            this.CheckLatestServerIP.Controls.Add(this.ServerLatestIPTextBox);
            this.CheckLatestServerIP.Controls.Add(this.ServerLatestIPLabel);
            this.CheckLatestServerIP.Controls.Add(this.GiveUpGetIPButton);
            this.CheckLatestServerIP.Controls.Add(this.GetLatestServerIPButton);
            this.CheckLatestServerIP.Controls.Add(this.ChangeMailBoxInfoButton);
            this.CheckLatestServerIP.Controls.Add(this.SaveMailBoxInfoButton);
            this.CheckLatestServerIP.Controls.Add(this.ReceiveServerIPMailBoxPasswordTextBox);
            this.CheckLatestServerIP.Controls.Add(this.ReceiveIPMailBoxTextBox);
            this.CheckLatestServerIP.Controls.Add(this.ReceiveServerIPMailBoxPasswordLabel);
            this.CheckLatestServerIP.Controls.Add(this.ReceiveIPMailBoxLabel);
            this.CheckLatestServerIP.Location = new System.Drawing.Point(61, 27);
            this.CheckLatestServerIP.Name = "CheckLatestServerIP";
            this.CheckLatestServerIP.Size = new System.Drawing.Size(470, 277);
            this.CheckLatestServerIP.TabIndex = 0;
            this.CheckLatestServerIP.TabStop = false;
            this.CheckLatestServerIP.Text = "查看最新的服务器IP";
            // 
            // CheckMailBoxPasswordLabel
            // 
            this.CheckMailBoxPasswordLabel.AutoSize = true;
            this.CheckMailBoxPasswordLabel.Location = new System.Drawing.Point(407, 66);
            this.CheckMailBoxPasswordLabel.Name = "CheckMailBoxPasswordLabel";
            this.CheckMailBoxPasswordLabel.Size = new System.Drawing.Size(0, 12);
            this.CheckMailBoxPasswordLabel.TabIndex = 11;
            // 
            // CheckMailBoxAddLabel
            // 
            this.CheckMailBoxAddLabel.AutoSize = true;
            this.CheckMailBoxAddLabel.Location = new System.Drawing.Point(405, 30);
            this.CheckMailBoxAddLabel.Name = "CheckMailBoxAddLabel";
            this.CheckMailBoxAddLabel.Size = new System.Drawing.Size(0, 12);
            this.CheckMailBoxAddLabel.TabIndex = 10;
            // 
            // ServerLatestIPTextBox
            // 
            this.ServerLatestIPTextBox.Location = new System.Drawing.Point(180, 223);
            this.ServerLatestIPTextBox.Name = "ServerLatestIPTextBox";
            this.ServerLatestIPTextBox.Size = new System.Drawing.Size(218, 21);
            this.ServerLatestIPTextBox.TabIndex = 9;
            // 
            // ServerLatestIPLabel
            // 
            this.ServerLatestIPLabel.AutoSize = true;
            this.ServerLatestIPLabel.Location = new System.Drawing.Point(28, 233);
            this.ServerLatestIPLabel.Name = "ServerLatestIPLabel";
            this.ServerLatestIPLabel.Size = new System.Drawing.Size(107, 12);
            this.ServerLatestIPLabel.TabIndex = 8;
            this.ServerLatestIPLabel.Text = "服务器最新IP地址:";
            // 
            // GiveUpGetIPButton
            // 
            this.GiveUpGetIPButton.Location = new System.Drawing.Point(305, 144);
            this.GiveUpGetIPButton.Name = "GiveUpGetIPButton";
            this.GiveUpGetIPButton.Size = new System.Drawing.Size(93, 30);
            this.GiveUpGetIPButton.TabIndex = 7;
            this.GiveUpGetIPButton.Text = "放弃";
            this.GiveUpGetIPButton.UseVisualStyleBackColor = true;
            this.GiveUpGetIPButton.Click += new System.EventHandler(this.GiveUpGetIPButton_Click);
            // 
            // GetLatestServerIPButton
            // 
            this.GetLatestServerIPButton.Location = new System.Drawing.Point(180, 144);
            this.GetLatestServerIPButton.Name = "GetLatestServerIPButton";
            this.GetLatestServerIPButton.Size = new System.Drawing.Size(99, 30);
            this.GetLatestServerIPButton.TabIndex = 6;
            this.GetLatestServerIPButton.Text = "获取最新IP";
            this.GetLatestServerIPButton.UseVisualStyleBackColor = true;
            this.GetLatestServerIPButton.Click += new System.EventHandler(this.GetLatestServerIPButton_Click);
            // 
            // ChangeMailBoxInfoButton
            // 
            this.ChangeMailBoxInfoButton.Location = new System.Drawing.Point(323, 84);
            this.ChangeMailBoxInfoButton.Name = "ChangeMailBoxInfoButton";
            this.ChangeMailBoxInfoButton.Size = new System.Drawing.Size(75, 23);
            this.ChangeMailBoxInfoButton.TabIndex = 5;
            this.ChangeMailBoxInfoButton.Text = "修改";
            this.ChangeMailBoxInfoButton.UseVisualStyleBackColor = true;
            this.ChangeMailBoxInfoButton.Click += new System.EventHandler(this.ChangeMailBoxInfoButton_Click);
            // 
            // SaveMailBoxInfoButton
            // 
            this.SaveMailBoxInfoButton.Location = new System.Drawing.Point(227, 84);
            this.SaveMailBoxInfoButton.Name = "SaveMailBoxInfoButton";
            this.SaveMailBoxInfoButton.Size = new System.Drawing.Size(75, 23);
            this.SaveMailBoxInfoButton.TabIndex = 4;
            this.SaveMailBoxInfoButton.Text = "保存";
            this.SaveMailBoxInfoButton.UseVisualStyleBackColor = true;
            this.SaveMailBoxInfoButton.Click += new System.EventHandler(this.SaveMailBoxInfoButton_Click);
            // 
            // ReceiveServerIPMailBoxPasswordTextBox
            // 
            this.ReceiveServerIPMailBoxPasswordTextBox.Location = new System.Drawing.Point(180, 57);
            this.ReceiveServerIPMailBoxPasswordTextBox.Name = "ReceiveServerIPMailBoxPasswordTextBox";
            this.ReceiveServerIPMailBoxPasswordTextBox.PasswordChar = '*';
            this.ReceiveServerIPMailBoxPasswordTextBox.Size = new System.Drawing.Size(218, 21);
            this.ReceiveServerIPMailBoxPasswordTextBox.TabIndex = 3;
            // 
            // ReceiveIPMailBoxTextBox
            // 
            this.ReceiveIPMailBoxTextBox.Location = new System.Drawing.Point(180, 22);
            this.ReceiveIPMailBoxTextBox.Name = "ReceiveIPMailBoxTextBox";
            this.ReceiveIPMailBoxTextBox.Size = new System.Drawing.Size(218, 21);
            this.ReceiveIPMailBoxTextBox.TabIndex = 2;
            // 
            // ReceiveServerIPMailBoxPasswordLabel
            // 
            this.ReceiveServerIPMailBoxPasswordLabel.AutoSize = true;
            this.ReceiveServerIPMailBoxPasswordLabel.Location = new System.Drawing.Point(26, 67);
            this.ReceiveServerIPMailBoxPasswordLabel.Name = "ReceiveServerIPMailBoxPasswordLabel";
            this.ReceiveServerIPMailBoxPasswordLabel.Size = new System.Drawing.Size(95, 12);
            this.ReceiveServerIPMailBoxPasswordLabel.TabIndex = 1;
            this.ReceiveServerIPMailBoxPasswordLabel.Text = "该邮箱登录密码:";
            // 
            // ReceiveIPMailBoxLabel
            // 
            this.ReceiveIPMailBoxLabel.AutoSize = true;
            this.ReceiveIPMailBoxLabel.Location = new System.Drawing.Point(24, 32);
            this.ReceiveIPMailBoxLabel.Name = "ReceiveIPMailBoxLabel";
            this.ReceiveIPMailBoxLabel.Size = new System.Drawing.Size(131, 12);
            this.ReceiveIPMailBoxLabel.TabIndex = 0;
            this.ReceiveIPMailBoxLabel.Text = "接收最新IP地址的邮箱:";
            // 
            // EncryptServerIP
            // 
            this.EncryptServerIP.Controls.Add(this.EncryptedServerIPTextBox);
            this.EncryptServerIP.Controls.Add(this.EncryptedServerIPLabel);
            this.EncryptServerIP.Controls.Add(this.EncryptServerIPButton);
            this.EncryptServerIP.Location = new System.Drawing.Point(61, 338);
            this.EncryptServerIP.Name = "EncryptServerIP";
            this.EncryptServerIP.Size = new System.Drawing.Size(470, 148);
            this.EncryptServerIP.TabIndex = 1;
            this.EncryptServerIP.TabStop = false;
            this.EncryptServerIP.Text = "对服务器IP地址加密";
            // 
            // EncryptedServerIPTextBox
            // 
            this.EncryptedServerIPTextBox.Location = new System.Drawing.Point(180, 88);
            this.EncryptedServerIPTextBox.Name = "EncryptedServerIPTextBox";
            this.EncryptedServerIPTextBox.Size = new System.Drawing.Size(218, 21);
            this.EncryptedServerIPTextBox.TabIndex = 2;
            // 
            // EncryptedServerIPLabel
            // 
            this.EncryptedServerIPLabel.AutoSize = true;
            this.EncryptedServerIPLabel.Location = new System.Drawing.Point(24, 97);
            this.EncryptedServerIPLabel.Name = "EncryptedServerIPLabel";
            this.EncryptedServerIPLabel.Size = new System.Drawing.Size(137, 12);
            this.EncryptedServerIPLabel.TabIndex = 1;
            this.EncryptedServerIPLabel.Text = "加密后的服务器IP地址：";
            // 
            // EncryptServerIPButton
            // 
            this.EncryptServerIPButton.Location = new System.Drawing.Point(180, 34);
            this.EncryptServerIPButton.Name = "EncryptServerIPButton";
            this.EncryptServerIPButton.Size = new System.Drawing.Size(218, 23);
            this.EncryptServerIPButton.TabIndex = 0;
            this.EncryptServerIPButton.Text = "加密";
            this.EncryptServerIPButton.UseVisualStyleBackColor = true;
            this.EncryptServerIPButton.Click += new System.EventHandler(this.EncryptServerIPButton_Click);
            // 
            // LatestServerIP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 516);
            this.Controls.Add(this.EncryptServerIP);
            this.Controls.Add(this.CheckLatestServerIP);
            this.Name = "LatestServerIP";
            this.Text = "第三方信息查询";
            this.CheckLatestServerIP.ResumeLayout(false);
            this.CheckLatestServerIP.PerformLayout();
            this.EncryptServerIP.ResumeLayout(false);
            this.EncryptServerIP.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox CheckLatestServerIP;
        private System.Windows.Forms.TextBox ServerLatestIPTextBox;
        private System.Windows.Forms.Label ServerLatestIPLabel;
        private System.Windows.Forms.Button GiveUpGetIPButton;
        private System.Windows.Forms.Button GetLatestServerIPButton;
        private System.Windows.Forms.Button ChangeMailBoxInfoButton;
        private System.Windows.Forms.Button SaveMailBoxInfoButton;
        private System.Windows.Forms.TextBox ReceiveServerIPMailBoxPasswordTextBox;
        private System.Windows.Forms.TextBox ReceiveIPMailBoxTextBox;
        private System.Windows.Forms.Label ReceiveServerIPMailBoxPasswordLabel;
        private System.Windows.Forms.Label ReceiveIPMailBoxLabel;
        private System.Windows.Forms.GroupBox EncryptServerIP;
        private System.Windows.Forms.Button EncryptServerIPButton;
        private System.Windows.Forms.TextBox EncryptedServerIPTextBox;
        private System.Windows.Forms.Label EncryptedServerIPLabel;
        private System.Windows.Forms.Label CheckMailBoxPasswordLabel;
        private System.Windows.Forms.Label CheckMailBoxAddLabel;
    }
}