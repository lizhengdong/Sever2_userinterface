namespace sever2
{
    partial class ServerIP
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
            this.label1 = new System.Windows.Forms.Label();
            this.ServerIPAddr = new System.Windows.Forms.TextBox();
            this.serverSave = new System.Windows.Forms.Button();
            this.serverQuit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "服务器IP地址：";
            // 
            // ServerIPAddr
            // 
            this.ServerIPAddr.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ServerIPAddr.Location = new System.Drawing.Point(123, 6);
            this.ServerIPAddr.Name = "ServerIPAddr";
            this.ServerIPAddr.Size = new System.Drawing.Size(234, 21);
            this.ServerIPAddr.TabIndex = 1;
            // 
            // serverSave
            // 
            this.serverSave.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.serverSave.Location = new System.Drawing.Point(123, 59);
            this.serverSave.Name = "serverSave";
            this.serverSave.Size = new System.Drawing.Size(99, 30);
            this.serverSave.TabIndex = 2;
            this.serverSave.Text = "保存并重启程序";
            this.serverSave.UseVisualStyleBackColor = true;
            this.serverSave.Click += new System.EventHandler(this.serverSave_Click);
            // 
            // serverQuit
            // 
            this.serverQuit.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.serverQuit.Location = new System.Drawing.Point(256, 59);
            this.serverQuit.Name = "serverQuit";
            this.serverQuit.Size = new System.Drawing.Size(101, 30);
            this.serverQuit.TabIndex = 3;
            this.serverQuit.Text = "放弃";
            this.serverQuit.UseVisualStyleBackColor = true;
            this.serverQuit.Click += new System.EventHandler(this.serverQuit_Click);
            // 
            // ServerIP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(383, 110);
            this.Controls.Add(this.serverQuit);
            this.Controls.Add(this.serverSave);
            this.Controls.Add(this.ServerIPAddr);
            this.Controls.Add(this.label1);
            this.Name = "ServerIP";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "修改服务器IP地址";
            this.Load += new System.EventHandler(this.修改服务器IP地址_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ServerIPAddr;
        private System.Windows.Forms.Button serverSave;
        private System.Windows.Forms.Button serverQuit;
    }
}