namespace sever2
{
    partial class UserInfo
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
            if (disposing && (components != null)) {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserInfo));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.PhoneNumText = new System.Windows.Forms.TextBox();
            this.PhoneIDText = new System.Windows.Forms.TextBox();
            this.SystemType = new System.Windows.Forms.TextBox();
            this.PhoneArray = new System.Windows.Forms.TextBox();
            this.UserName = new System.Windows.Forms.TextBox();
            this.UpdateButton = new System.Windows.Forms.Button();
            this.ExitButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "  IMSI号：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "  IMEI号：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 86);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "系统类型：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 122);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "手机号码：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(23, 152);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "用户姓名：";
            // 
            // PhoneNumText
            // 
            this.PhoneNumText.Location = new System.Drawing.Point(92, 18);
            this.PhoneNumText.Name = "PhoneNumText";
            this.PhoneNumText.ReadOnly = true;
            this.PhoneNumText.Size = new System.Drawing.Size(248, 21);
            this.PhoneNumText.TabIndex = 5;
            // 
            // PhoneIDText
            // 
            this.PhoneIDText.Location = new System.Drawing.Point(92, 50);
            this.PhoneIDText.Name = "PhoneIDText";
            this.PhoneIDText.ReadOnly = true;
            this.PhoneIDText.Size = new System.Drawing.Size(248, 21);
            this.PhoneIDText.TabIndex = 6;
            // 
            // SystemType
            // 
            this.SystemType.Location = new System.Drawing.Point(92, 83);
            this.SystemType.Name = "SystemType";
            this.SystemType.ReadOnly = true;
            this.SystemType.Size = new System.Drawing.Size(248, 21);
            this.SystemType.TabIndex = 7;
            // 
            // PhoneArray
            // 
            this.PhoneArray.Location = new System.Drawing.Point(92, 119);
            this.PhoneArray.MaxLength = 18;
            this.PhoneArray.Name = "PhoneArray";
            this.PhoneArray.Size = new System.Drawing.Size(248, 21);
            this.PhoneArray.TabIndex = 8;
            // 
            // UserName
            // 
            this.UserName.Location = new System.Drawing.Point(92, 149);
            this.UserName.MaxLength = 18;
            this.UserName.Name = "UserName";
            this.UserName.Size = new System.Drawing.Size(248, 21);
            this.UserName.TabIndex = 9;
            // 
            // UpdateButton
            // 
            this.UpdateButton.Location = new System.Drawing.Point(92, 188);
            this.UpdateButton.Name = "UpdateButton";
            this.UpdateButton.Size = new System.Drawing.Size(75, 29);
            this.UpdateButton.TabIndex = 11;
            this.UpdateButton.Text = "保存";
            this.UpdateButton.UseVisualStyleBackColor = true;
            this.UpdateButton.Click += new System.EventHandler(this.UpdateButton_Click);
            // 
            // ExitButton
            // 
            this.ExitButton.Location = new System.Drawing.Point(265, 188);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(75, 29);
            this.ExitButton.TabIndex = 12;
            this.ExitButton.Text = "退出";
            this.ExitButton.UseVisualStyleBackColor = true;
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // UserInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 233);
            this.Controls.Add(this.ExitButton);
            this.Controls.Add(this.UpdateButton);
            this.Controls.Add(this.UserName);
            this.Controls.Add(this.PhoneArray);
            this.Controls.Add(this.SystemType);
            this.Controls.Add(this.PhoneIDText);
            this.Controls.Add(this.PhoneNumText);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UserInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "修改用户信息";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox PhoneNumText;
        private System.Windows.Forms.TextBox PhoneIDText;
        private System.Windows.Forms.TextBox SystemType;
        private System.Windows.Forms.TextBox PhoneArray;
        private System.Windows.Forms.TextBox UserName;
        private System.Windows.Forms.Button UpdateButton;
        private System.Windows.Forms.Button ExitButton;
    }
}