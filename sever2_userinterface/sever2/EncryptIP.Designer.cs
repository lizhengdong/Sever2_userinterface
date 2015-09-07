namespace sever2
{
    partial class EncryptIP
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
            this.NewServerIPLabel = new System.Windows.Forms.Label();
            this.NewServerIPTextBox = new System.Windows.Forms.TextBox();
            this.EncryptButton = new System.Windows.Forms.Button();
            this.GiveUpButton = new System.Windows.Forms.Button();
            this.EncryptedServerIPButton = new System.Windows.Forms.Label();
            this.EncryptedServerIPTextBox = new System.Windows.Forms.TextBox();
            this.CheckIPLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // NewServerIPLabel
            // 
            this.NewServerIPLabel.AutoSize = true;
            this.NewServerIPLabel.Location = new System.Drawing.Point(26, 23);
            this.NewServerIPLabel.Name = "NewServerIPLabel";
            this.NewServerIPLabel.Size = new System.Drawing.Size(107, 12);
            this.NewServerIPLabel.TabIndex = 0;
            this.NewServerIPLabel.Text = "新的第三方IP地址:";
            // 
            // NewServerIPTextBox
            // 
            this.NewServerIPTextBox.Location = new System.Drawing.Point(188, 20);
            this.NewServerIPTextBox.Name = "NewServerIPTextBox";
            this.NewServerIPTextBox.Size = new System.Drawing.Size(206, 21);
            this.NewServerIPTextBox.TabIndex = 1;
            // 
            // EncryptButton
            // 
            this.EncryptButton.Location = new System.Drawing.Point(188, 60);
            this.EncryptButton.Name = "EncryptButton";
            this.EncryptButton.Size = new System.Drawing.Size(75, 23);
            this.EncryptButton.TabIndex = 2;
            this.EncryptButton.Text = "加密";
            this.EncryptButton.UseVisualStyleBackColor = true;
            this.EncryptButton.Click += new System.EventHandler(this.EncryptButton_Click);
            // 
            // GiveUpButton
            // 
            this.GiveUpButton.Location = new System.Drawing.Point(319, 60);
            this.GiveUpButton.Name = "GiveUpButton";
            this.GiveUpButton.Size = new System.Drawing.Size(75, 23);
            this.GiveUpButton.TabIndex = 3;
            this.GiveUpButton.Text = "放弃";
            this.GiveUpButton.UseVisualStyleBackColor = true;
            this.GiveUpButton.Click += new System.EventHandler(this.GiveUpButton_Click);
            // 
            // EncryptedServerIPButton
            // 
            this.EncryptedServerIPButton.AutoSize = true;
            this.EncryptedServerIPButton.Location = new System.Drawing.Point(28, 101);
            this.EncryptedServerIPButton.Name = "EncryptedServerIPButton";
            this.EncryptedServerIPButton.Size = new System.Drawing.Size(131, 12);
            this.EncryptedServerIPButton.TabIndex = 4;
            this.EncryptedServerIPButton.Text = "加密后的第三方IP地址:";
            // 
            // EncryptedServerIPTextBox
            // 
            this.EncryptedServerIPTextBox.Location = new System.Drawing.Point(188, 101);
            this.EncryptedServerIPTextBox.Name = "EncryptedServerIPTextBox";
            this.EncryptedServerIPTextBox.Size = new System.Drawing.Size(206, 21);
            this.EncryptedServerIPTextBox.TabIndex = 5;
            // 
            // CheckIPLabel
            // 
            this.CheckIPLabel.AutoSize = true;
            this.CheckIPLabel.Location = new System.Drawing.Point(400, 23);
            this.CheckIPLabel.Name = "CheckIPLabel";
            this.CheckIPLabel.Size = new System.Drawing.Size(0, 12);
            this.CheckIPLabel.TabIndex = 6;
            // 
            // EncryptIP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 182);
            this.Controls.Add(this.CheckIPLabel);
            this.Controls.Add(this.EncryptedServerIPTextBox);
            this.Controls.Add(this.EncryptedServerIPButton);
            this.Controls.Add(this.GiveUpButton);
            this.Controls.Add(this.EncryptButton);
            this.Controls.Add(this.NewServerIPTextBox);
            this.Controls.Add(this.NewServerIPLabel);
            this.Name = "EncryptIP";
            this.Text = "手机端配置";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label NewServerIPLabel;
        private System.Windows.Forms.TextBox NewServerIPTextBox;
        private System.Windows.Forms.Button EncryptButton;
        private System.Windows.Forms.Button GiveUpButton;
        private System.Windows.Forms.Label EncryptedServerIPButton;
        private System.Windows.Forms.TextBox EncryptedServerIPTextBox;
        private System.Windows.Forms.Label CheckIPLabel;
    }
}