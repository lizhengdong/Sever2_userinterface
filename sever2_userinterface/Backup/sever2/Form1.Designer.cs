namespace sever2
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "+.png");
            this.imageList1.Images.SetKeyName(1, "Comments.png");
            this.imageList1.Images.SetKeyName(2, "Help.png");
            this.imageList1.Images.SetKeyName(3, "Lock.png");
            this.imageList1.Images.SetKeyName(4, "Sound.png");
            this.imageList1.Images.SetKeyName(5, "Window Add.png");
            this.imageList1.Images.SetKeyName(6, "blue_speech_bubble_48.png");
            this.imageList1.Images.SetKeyName(7, "lock_48.png");
            this.imageList1.Images.SetKeyName(8, "Robo-Help.png");
            this.imageList1.Images.SetKeyName(9, "Blue.ico");
            this.imageList1.Images.SetKeyName(10, "Graphite.ico");
            this.imageList1.Images.SetKeyName(11, "Orange.ico");
            this.imageList1.Images.SetKeyName(12, "Furl_32x32.png");
            this.imageList1.Images.SetKeyName(13, "MySpace_32x32.png");
            this.imageList1.Images.SetKeyName(14, "160.ico");
            this.imageList1.Images.SetKeyName(15, "add_48.png");
            this.imageList1.Images.SetKeyName(16, "spanner_48.png");
            this.imageList1.Images.SetKeyName(17, "search_48.png");
            this.imageList1.Images.SetKeyName(18, "Tools.png");
            this.imageList1.Images.SetKeyName(19, "accepted_48.png");
            this.imageList1.Images.SetKeyName(20, "cancel_48.png");
            // 
            // treeView1
            // 
            this.treeView1.ImageIndex = 9;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(2, 74);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 9;
            this.treeView1.Size = new System.Drawing.Size(294, 327);
            this.treeView1.TabIndex = 2;
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // button2
            // 
            this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.ImageIndex = 18;
            this.button2.ImageList = this.imageList1;
            this.button2.Location = new System.Drawing.Point(109, 33);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(111, 35);
            this.button2.TabIndex = 3;
            this.button2.Text = "系统管理";
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.ImageIndex = 10;
            this.button1.ImageList = this.imageList1;
            this.button1.Location = new System.Drawing.Point(2, 33);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(110, 35);
            this.button1.TabIndex = 4;
            this.button1.Text = "终端查询";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button3.ImageIndex = 2;
            this.button3.ImageList = this.imageList1;
            this.button3.Location = new System.Drawing.Point(217, 33);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(79, 35);
            this.button3.TabIndex = 5;
            this.button3.Text = "帮助";
            this.button3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button3.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(296, 413);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.treeView1);
            this.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "手机终端采集 多线程测试";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
    }
}

