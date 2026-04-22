namespace JSZW400.SubWindows
{
    partial class SubOPAutoDraw
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubOPAutoDraw));
            label1 = new Label();
            panel1 = new Panel();
            label5 = new Label();
            label4 = new Label();
            lb分条开 = new Label();
            sw网格大小 = new Button();
            label6 = new Label();
            label3 = new Label();
            txb网格单元长 = new TextBox();
            label2 = new Label();
            lb网格状态 = new Label();
            sw对齐网格 = new Button();
            label15 = new Label();
            btn撤销 = new Button();
            btn清除 = new Button();
            panel2 = new Panel();
            timer1 = new System.Windows.Forms.Timer(components);
            panel3 = new Panel();
            btn确定 = new Button();
            tbw1 = new TextBox();
            tbh = new TextBox();
            label12 = new Label();
            tbw2 = new TextBox();
            label11 = new Label();
            tbl2 = new TextBox();
            label10 = new Label();
            tbl1 = new TextBox();
            label9 = new Label();
            label8 = new Label();
            tbn = new TextBox();
            label7 = new Label();
            btn_插入 = new Button();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft YaHei UI", 29F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(609, 321);
            label1.Name = "label1";
            label1.Size = new Size(136, 51);
            label1.TabIndex = 1;
            label1.Text = "Auto2";
            // 
            // panel1
            // 
            panel1.BackgroundImage = Properties.Resources.bg_Auto2_Top;
            panel1.Controls.Add(btn_插入);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(lb分条开);
            panel1.Controls.Add(sw网格大小);
            panel1.Controls.Add(label6);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(txb网格单元长);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(lb网格状态);
            panel1.Controls.Add(sw对齐网格);
            panel1.Controls.Add(label15);
            panel1.Controls.Add(btn撤销);
            panel1.Controls.Add(btn清除);
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1516, 100);
            panel1.TabIndex = 2;
            // 
            // label5
            // 
            label5.BackColor = Color.Transparent;
            label5.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label5.ForeColor = Color.White;
            label5.Location = new Point(1017, 72);
            label5.Name = "label5";
            label5.Size = new Size(60, 15);
            label5.TabIndex = 50;
            label5.Text = "粗糙";
            label5.TextAlign = ContentAlignment.TopRight;
            // 
            // label4
            // 
            label4.BackColor = Color.Transparent;
            label4.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label4.ForeColor = Color.White;
            label4.Location = new Point(1017, 13);
            label4.Name = "label4";
            label4.Size = new Size(60, 15);
            label4.TabIndex = 50;
            label4.Text = "精细";
            label4.TextAlign = ContentAlignment.TopRight;
            // 
            // lb分条开
            // 
            lb分条开.BackColor = Color.Transparent;
            lb分条开.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            lb分条开.ForeColor = Color.White;
            lb分条开.Location = new Point(1005, 44);
            lb分条开.Name = "lb分条开";
            lb分条开.Size = new Size(70, 15);
            lb分条开.TabIndex = 50;
            lb分条开.Text = "合适";
            lb分条开.TextAlign = ContentAlignment.TopRight;
            // 
            // sw网格大小
            // 
            sw网格大小.FlatAppearance.BorderColor = Color.FromArgb(67, 67, 67);
            sw网格大小.FlatAppearance.BorderSize = 0;
            sw网格大小.FlatAppearance.MouseDownBackColor = Color.FromArgb(67, 67, 67);
            sw网格大小.FlatAppearance.MouseOverBackColor = Color.FromArgb(67, 67, 67);
            sw网格大小.FlatStyle = FlatStyle.Flat;
            sw网格大小.Image = Properties.Resources.btm_3档开关2;
            sw网格大小.Location = new Point(1072, 15);
            sw网格大小.Name = "sw网格大小";
            sw网格大小.Size = new Size(98, 71);
            sw网格大小.TabIndex = 49;
            sw网格大小.UseVisualStyleBackColor = true;
            sw网格大小.Click += sw网格大小_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.BackColor = Color.Transparent;
            label6.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label6.ForeColor = Color.White;
            label6.Location = new Point(1179, 37);
            label6.Name = "label6";
            label6.Size = new Size(67, 15);
            label6.TabIndex = 48;
            label6.Text = "网格大小";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label3.ForeColor = Color.White;
            label3.Location = new Point(666, 41);
            label3.Name = "label3";
            label3.Size = new Size(37, 15);
            label3.TabIndex = 48;
            label3.Text = "毫米";
            // 
            // txb网格单元长
            // 
            txb网格单元长.Font = new Font("Calibri", 13F, FontStyle.Regular, GraphicsUnit.Point);
            txb网格单元长.Location = new Point(615, 35);
            txb网格单元长.Name = "txb网格单元长";
            txb网格单元长.Size = new Size(45, 29);
            txb网格单元长.TabIndex = 47;
            txb网格单元长.Text = "1";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label2.ForeColor = Color.White;
            label2.Location = new Point(542, 35);
            label2.Name = "label2";
            label2.Size = new Size(67, 30);
            label2.TabIndex = 46;
            label2.Text = "网格单元\r\n长度单位";
            // 
            // lb网格状态
            // 
            lb网格状态.AutoSize = true;
            lb网格状态.BackColor = Color.Transparent;
            lb网格状态.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            lb网格状态.ForeColor = Color.White;
            lb网格状态.Location = new Point(429, 50);
            lb网格状态.Name = "lb网格状态";
            lb网格状态.Size = new Size(22, 15);
            lb网格状态.TabIndex = 45;
            lb网格状态.Text = "关";
            // 
            // sw对齐网格
            // 
            sw对齐网格.BackgroundImage = Properties.Resources.sw_左右小开关0;
            sw对齐网格.FlatAppearance.BorderSize = 0;
            sw对齐网格.FlatStyle = FlatStyle.Flat;
            sw对齐网格.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            sw对齐网格.ForeColor = Color.White;
            sw对齐网格.Location = new Point(343, 37);
            sw对齐网格.Name = "sw对齐网格";
            sw对齐网格.Size = new Size(80, 40);
            sw对齐网格.TabIndex = 44;
            sw对齐网格.TextAlign = ContentAlignment.MiddleRight;
            sw对齐网格.TextImageRelation = TextImageRelation.ImageBeforeText;
            sw对齐网格.UseVisualStyleBackColor = true;
            sw对齐网格.Click += sw对齐网格_Click;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.BackColor = Color.Transparent;
            label15.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label15.ForeColor = Color.White;
            label15.Location = new Point(350, 19);
            label15.Name = "label15";
            label15.Size = new Size(67, 15);
            label15.TabIndex = 43;
            label15.Text = "对齐网格";
            // 
            // btn撤销
            // 
            btn撤销.BackgroundImage = (Image)resources.GetObject("btn撤销.BackgroundImage");
            btn撤销.FlatAppearance.BorderSize = 0;
            btn撤销.FlatStyle = FlatStyle.Flat;
            btn撤销.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn撤销.ForeColor = Color.White;
            btn撤销.Image = Properties.Resources.Undo1_40_40;
            btn撤销.Location = new Point(167, 23);
            btn撤销.Name = "btn撤销";
            btn撤销.Size = new Size(113, 54);
            btn撤销.TabIndex = 42;
            btn撤销.Text = " 撤销";
            btn撤销.TextAlign = ContentAlignment.MiddleRight;
            btn撤销.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn撤销.UseVisualStyleBackColor = true;
            btn撤销.Click += btn撤销_Click;
            // 
            // btn清除
            // 
            btn清除.BackgroundImage = Properties.Resources.bg_中按钮1;
            btn清除.FlatAppearance.BorderSize = 0;
            btn清除.FlatStyle = FlatStyle.Flat;
            btn清除.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn清除.ForeColor = Color.White;
            btn清除.Image = Properties.Resources.Remove;
            btn清除.Location = new Point(29, 23);
            btn清除.Name = "btn清除";
            btn清除.Size = new Size(113, 54);
            btn清除.TabIndex = 42;
            btn清除.Text = " 清除";
            btn清除.TextAlign = ContentAlignment.MiddleRight;
            btn清除.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn清除.UseVisualStyleBackColor = true;
            btn清除.Click += btn清除_Click;
            // 
            // panel2
            // 
            panel2.BackColor = Color.Black;
            panel2.Controls.Add(panel3);
            panel2.Location = new Point(10, 110);
            panel2.Name = "panel2";
            panel2.Size = new Size(1481, 781);
            panel2.TabIndex = 4;
            panel2.MouseDown += panel2_MouseDown;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            // 
            // panel3
            // 
            panel3.BackgroundImage = (Image)resources.GetObject("panel3.BackgroundImage");
            panel3.BackgroundImageLayout = ImageLayout.Zoom;
            panel3.Controls.Add(btn确定);
            panel3.Controls.Add(tbw1);
            panel3.Controls.Add(tbh);
            panel3.Controls.Add(label12);
            panel3.Controls.Add(tbw2);
            panel3.Controls.Add(label11);
            panel3.Controls.Add(tbl2);
            panel3.Controls.Add(label10);
            panel3.Controls.Add(tbl1);
            panel3.Controls.Add(label9);
            panel3.Controls.Add(label8);
            panel3.Controls.Add(tbn);
            panel3.Controls.Add(label7);
            panel3.Location = new Point(1158, 3);
            panel3.Name = "panel3";
            panel3.Size = new Size(309, 172);
            panel3.TabIndex = 52;
            // 
            // btn确定
            // 
            btn确定.BackColor = Color.Transparent;
            btn确定.BackgroundImage = Properties.Resources.bg_小按钮;
            btn确定.BackgroundImageLayout = ImageLayout.Stretch;
            btn确定.FlatAppearance.BorderSize = 0;
            btn确定.FlatStyle = FlatStyle.Flat;
            btn确定.Font = new Font("微软雅黑", 12F, FontStyle.Regular, GraphicsUnit.Point);
            btn确定.ForeColor = Color.White;
            btn确定.ImeMode = ImeMode.NoControl;
            btn确定.Location = new Point(224, 20);
            btn确定.Name = "btn确定";
            btn确定.Size = new Size(70, 35);
            btn确定.TabIndex = 77;
            btn确定.Text = "确定";
            btn确定.UseVisualStyleBackColor = false;
            // 
            // tbw1
            // 
            tbw1.Location = new Point(152, 33);
            tbw1.Name = "tbw1";
            tbw1.Size = new Size(31, 23);
            tbw1.TabIndex = 3;
            tbw1.Text = "20";
            // 
            // tbh
            // 
            tbh.Location = new Point(224, 71);
            tbh.Name = "tbh";
            tbh.Size = new Size(31, 23);
            tbh.TabIndex = 11;
            tbh.Text = "20";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.BackColor = Color.Transparent;
            label12.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label12.Location = new Point(190, 72);
            label12.Name = "label12";
            label12.Size = new Size(34, 21);
            label12.TabIndex = 10;
            label12.Text = "H=";
            // 
            // tbw2
            // 
            tbw2.Location = new Point(152, 136);
            tbw2.Name = "tbw2";
            tbw2.Size = new Size(31, 23);
            tbw2.TabIndex = 9;
            tbw2.Text = "20";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.BackColor = Color.Transparent;
            label11.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label11.Location = new Point(105, 137);
            label11.Name = "label11";
            label11.Size = new Size(47, 21);
            label11.TabIndex = 8;
            label11.Text = "W2=";
            // 
            // tbl2
            // 
            tbl2.Location = new Point(243, 100);
            tbl2.Name = "tbl2";
            tbl2.Size = new Size(31, 23);
            tbl2.TabIndex = 7;
            tbl2.Text = "20";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.BackColor = Color.Transparent;
            label10.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label10.Location = new Point(204, 101);
            label10.Name = "label10";
            label10.Size = new Size(39, 21);
            label10.TabIndex = 6;
            label10.Text = "L2=";
            // 
            // tbl1
            // 
            tbl1.Location = new Point(54, 102);
            tbl1.Name = "tbl1";
            tbl1.Size = new Size(31, 23);
            tbl1.TabIndex = 5;
            tbl1.Text = "20";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.BackColor = Color.Transparent;
            label9.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label9.Location = new Point(13, 103);
            label9.Name = "label9";
            label9.Size = new Size(39, 21);
            label9.TabIndex = 4;
            label9.Text = "L1=";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.BackColor = Color.Transparent;
            label8.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label8.Location = new Point(105, 34);
            label8.Name = "label8";
            label8.Size = new Size(47, 21);
            label8.TabIndex = 2;
            label8.Text = "W1=";
            // 
            // tbn
            // 
            tbn.Location = new Point(54, 35);
            tbn.Name = "tbn";
            tbn.Size = new Size(31, 23);
            tbn.TabIndex = 1;
            tbn.Text = "1";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.BackColor = Color.Transparent;
            label7.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label7.Location = new Point(19, 35);
            label7.Name = "label7";
            label7.Size = new Size(35, 21);
            label7.TabIndex = 0;
            label7.Text = "N=";
            // 
            // btn_插入
            // 
            btn_插入.BackgroundImage = (Image)resources.GetObject("btn_插入.BackgroundImage");
            btn_插入.FlatAppearance.BorderSize = 0;
            btn_插入.FlatStyle = FlatStyle.Flat;
            btn_插入.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn_插入.ForeColor = Color.White;
            btn_插入.Image = (Image)resources.GetObject("btn_插入.Image");
            btn_插入.Location = new Point(1358, 23);
            btn_插入.Name = "btn_插入";
            btn_插入.Size = new Size(113, 54);
            btn_插入.TabIndex = 53;
            btn_插入.Text = " 快速\r\n 插入";
            btn_插入.TextAlign = ContentAlignment.MiddleRight;
            btn_插入.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_插入.UseVisualStyleBackColor = true;
            // 
            // SubOPAutoDraw
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.Black;
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(label1);
            Name = "SubOPAutoDraw";
            Size = new Size(1516, 909);
            Load += SubOPAuto2_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private Label label1;
        private Panel panel1;
        private Button btn清除;
        private Button btn撤销;
        private Button sw对齐网格;
        private Label label15;
        private Label label2;
        private Label lb网格状态;
        private Label label3;
        private TextBox txb网格单元长;
        private Button sw网格大小;
        private Label label5;
        private Label label4;
        private Label lb分条开;
        private Label label6;
        private Panel panel2;
        private System.Windows.Forms.Timer timer1;
        private Button btn_插入;
        private Panel panel3;
        private Button btn确定;
        private TextBox tbw1;
        private TextBox tbh;
        private Label label12;
        private TextBox tbw2;
        private Label label11;
        private TextBox tbl2;
        private Label label10;
        private TextBox tbl1;
        private Label label9;
        private Label label8;
        private TextBox tbn;
        private Label label7;
    }
}
