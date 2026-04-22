namespace JSZW1000A.SubWindows
{
    partial class SubOPSlitter
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            btn分条机归原位 = new Button();
            sw边角料选 = new Button();
            lb首先 = new Label();
            lb最后 = new Label();
            pnl边角料选后 = new Panel();
            panel2 = new Panel();
            lb边角料长度_后 = new Label();
            label3 = new Label();
            pnl边角料选先 = new Panel();
            panel4 = new Panel();
            lb边角料长度_先 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            txb单项宽 = new TextBox();
            label9 = new Label();
            txb总宽 = new TextBox();
            label10 = new Label();
            label11 = new Label();
            txb单项数量 = new TextBox();
            lb生产数量状态 = new Label();
            btn列表_下移 = new Button();
            btn列表_上移 = new Button();
            btn清除 = new Button();
            btn列表_之后插入 = new Button();
            btn列表_之前插入 = new Button();
            timer500ms = new System.Windows.Forms.Timer(components);
            pnlSlitter = new Panel();
            dataGridView1 = new DataGridView();
            任务号 = new DataGridViewTextBoxColumn();
            pictureBox1 = new PictureBox();
            btn装载材料 = new Button();
            timer200ms = new System.Windows.Forms.Timer(components);
            pnl边角料选后.SuspendLayout();
            pnl边角料选先.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // btn分条机归原位
            // 
            btn分条机归原位.BackgroundImage = Properties.Resources.bg_大按钮1;
            btn分条机归原位.FlatAppearance.BorderSize = 0;
            btn分条机归原位.FlatStyle = FlatStyle.Flat;
            btn分条机归原位.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn分条机归原位.ForeColor = Color.White;
            btn分条机归原位.Image = Properties.Resources.Park1;
            btn分条机归原位.Location = new Point(1165, 106);
            btn分条机归原位.Name = "btn分条机归原位";
            btn分条机归原位.Size = new Size(161, 55);
            btn分条机归原位.TabIndex = 8;
            btn分条机归原位.Text = "   分条机\r\n   归原位";
            btn分条机归原位.TextAlign = ContentAlignment.MiddleRight;
            btn分条机归原位.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn分条机归原位.UseVisualStyleBackColor = true;
            btn分条机归原位.Visible = false;
            btn分条机归原位.MouseDown += btn分条机归原位_MouseDown;
            btn分条机归原位.MouseUp += btn分条机归原位_MouseUp;
            // 
            // sw边角料选
            // 
            sw边角料选.BackgroundImage = Properties.Resources.sw_左右小开关0;
            sw边角料选.FlatAppearance.BorderSize = 0;
            sw边角料选.FlatStyle = FlatStyle.Flat;
            sw边角料选.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            sw边角料选.ForeColor = Color.White;
            sw边角料选.Location = new Point(943, 134);
            sw边角料选.Name = "sw边角料选";
            sw边角料选.Size = new Size(80, 40);
            sw边角料选.TabIndex = 26;
            sw边角料选.TextAlign = ContentAlignment.MiddleRight;
            sw边角料选.TextImageRelation = TextImageRelation.ImageBeforeText;
            sw边角料选.UseVisualStyleBackColor = true;
            sw边角料选.Click += btn边角料选_Click;
            // 
            // lb首先
            // 
            lb首先.AutoSize = true;
            lb首先.BackColor = Color.Transparent;
            lb首先.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            lb首先.ForeColor = Color.White;
            lb首先.Location = new Point(903, 147);
            lb首先.Name = "lb首先";
            lb首先.Size = new Size(37, 15);
            lb首先.TabIndex = 25;
            lb首先.Text = "首先";
            // 
            // lb最后
            // 
            lb最后.AutoSize = true;
            lb最后.BackColor = Color.Transparent;
            lb最后.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            lb最后.ForeColor = Color.White;
            lb最后.Location = new Point(1028, 147);
            lb最后.Name = "lb最后";
            lb最后.Size = new Size(37, 15);
            lb最后.TabIndex = 27;
            lb最后.Text = "最后";
            // 
            // pnl边角料选后
            // 
            pnl边角料选后.BackColor = Color.Transparent;
            pnl边角料选后.Controls.Add(panel2);
            pnl边角料选后.Controls.Add(lb边角料长度_后);
            pnl边角料选后.Controls.Add(label3);
            pnl边角料选后.Location = new Point(736, 94);
            pnl边角料选后.Name = "pnl边角料选后";
            pnl边角料选后.Size = new Size(155, 68);
            pnl边角料选后.TabIndex = 28;
            // 
            // panel2
            // 
            panel2.BackColor = Color.Transparent;
            panel2.BackgroundImage = Properties.Resources.OffcutRight;
            panel2.BackgroundImageLayout = ImageLayout.Zoom;
            panel2.Location = new Point(-5, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(67, 67);
            panel2.TabIndex = 31;
            // 
            // lb边角料长度_后
            // 
            lb边角料长度_后.AutoSize = true;
            lb边角料长度_后.BackColor = Color.Transparent;
            lb边角料长度_后.Font = new Font("Calibri", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lb边角料长度_后.ForeColor = Color.White;
            lb边角料长度_后.Location = new Point(65, 15);
            lb边角料长度_后.Name = "lb边角料长度_后";
            lb边角料长度_后.Size = new Size(45, 19);
            lb边角料长度_后.TabIndex = 30;
            lb边角料长度_后.Text = "0.0 in";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label3.ForeColor = Color.White;
            label3.Location = new Point(65, 0);
            label3.Name = "label3";
            label3.Size = new Size(52, 15);
            label3.TabIndex = 29;
            label3.Text = "边角料";
            // 
            // pnl边角料选先
            // 
            pnl边角料选先.BackColor = Color.Transparent;
            pnl边角料选先.Controls.Add(panel4);
            pnl边角料选先.Controls.Add(lb边角料长度_先);
            pnl边角料选先.Controls.Add(label6);
            pnl边角料选先.Location = new Point(103, 99);
            pnl边角料选先.Name = "pnl边角料选先";
            pnl边角料选先.Size = new Size(153, 74);
            pnl边角料选先.TabIndex = 29;
            // 
            // panel4
            // 
            panel4.BackColor = Color.Transparent;
            panel4.BackgroundImage = Properties.Resources.OffcutLeft;
            panel4.BackgroundImageLayout = ImageLayout.Zoom;
            panel4.Location = new Point(90, 7);
            panel4.Name = "panel4";
            panel4.Size = new Size(67, 67);
            panel4.TabIndex = 31;
            // 
            // lb边角料长度_先
            // 
            lb边角料长度_先.AutoSize = true;
            lb边角料长度_先.BackColor = Color.Transparent;
            lb边角料长度_先.Font = new Font("Calibri", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lb边角料长度_先.ForeColor = Color.White;
            lb边角料长度_先.Location = new Point(15, 16);
            lb边角料长度_先.Name = "lb边角料长度_先";
            lb边角料长度_先.Size = new Size(45, 19);
            lb边角料长度_先.TabIndex = 30;
            lb边角料长度_先.Text = "0.0 in";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.BackColor = Color.Transparent;
            label6.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label6.ForeColor = Color.White;
            label6.Location = new Point(20, 0);
            label6.Name = "label6";
            label6.Size = new Size(52, 15);
            label6.TabIndex = 29;
            label6.Text = "边角料";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.BackColor = Color.Transparent;
            label7.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label7.ForeColor = Color.White;
            label7.Location = new Point(956, 117);
            label7.Name = "label7";
            label7.Size = new Size(52, 15);
            label7.TabIndex = 30;
            label7.Text = "边角料";
            // 
            // label8
            // 
            label8.BackColor = Color.Transparent;
            label8.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label8.ForeColor = Color.White;
            label8.Location = new Point(211, 351);
            label8.Name = "label8";
            label8.Size = new Size(50, 20);
            label8.TabIndex = 32;
            label8.Text = "宽度";
            label8.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txb单项宽
            // 
            txb单项宽.Font = new Font("Microsoft YaHei UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            txb单项宽.Location = new Point(262, 348);
            txb单项宽.Name = "txb单项宽";
            txb单项宽.Size = new Size(56, 26);
            txb单项宽.TabIndex = 2;
            txb单项宽.Text = "0.0";
            txb单项宽.TextAlign = HorizontalAlignment.Right;
            txb单项宽.Click += txb单项宽_Click;
            txb单项宽.TextChanged += txb单项宽_TextChanged;
            txb单项宽.KeyDown += txb单项宽_KeyDown;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.BackColor = Color.Transparent;
            label9.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label9.ForeColor = Color.White;
            label9.Location = new Point(533, 278);
            label9.Name = "label9";
            label9.Size = new Size(23, 15);
            label9.TabIndex = 34;
            label9.Text = "in";
            // 
            // txb总宽
            // 
            txb总宽.Font = new Font("Microsoft YaHei UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            txb总宽.Location = new Point(466, 272);
            txb总宽.Name = "txb总宽";
            txb总宽.Size = new Size(61, 26);
            txb总宽.TabIndex = 1;
            txb总宽.Text = "0.0";
            txb总宽.Click += txb总宽_Click;
            txb总宽.TextChanged += txb单项宽_TextChanged;
            txb总宽.KeyDown += txb总宽_KeyDown;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.BackColor = Color.Transparent;
            label10.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label10.ForeColor = Color.White;
            label10.Location = new Point(318, 354);
            label10.Name = "label10";
            label10.Size = new Size(23, 15);
            label10.TabIndex = 35;
            label10.Text = "in";
            // 
            // label11
            // 
            label11.BackColor = Color.Transparent;
            label11.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label11.ForeColor = Color.White;
            label11.Location = new Point(419, 351);
            label11.Name = "label11";
            label11.Size = new Size(37, 20);
            label11.TabIndex = 37;
            label11.Text = "数量";
            label11.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txb单项数量
            // 
            txb单项数量.Font = new Font("Microsoft YaHei UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            txb单项数量.Location = new Point(462, 348);
            txb单项数量.Name = "txb单项数量";
            txb单项数量.Size = new Size(51, 26);
            txb单项数量.TabIndex = 3;
            txb单项数量.Text = "0";
            txb单项数量.TextAlign = HorizontalAlignment.Right;
            txb单项数量.Click += txb单项数量_Click;
            txb单项数量.TextChanged += txb单项宽_TextChanged;
            txb单项数量.KeyDown += txb单项数量_KeyDown;
            // 
            // lb生产数量状态
            // 
            lb生产数量状态.AutoSize = true;
            lb生产数量状态.BackColor = Color.Transparent;
            lb生产数量状态.Font = new Font("宋体", 15F, FontStyle.Regular, GraphicsUnit.Point);
            lb生产数量状态.ForeColor = Color.FromArgb(208, 208, 208);
            lb生产数量状态.Location = new Point(286, 690);
            lb生产数量状态.Name = "lb生产数量状态";
            lb生产数量状态.Size = new Size(179, 20);
            lb生产数量状态.TabIndex = 38;
            lb生产数量状态.Text = "实际 0/0 ; 总数 0";
            // 
            // btn列表_下移
            // 
            btn列表_下移.BackgroundImage = Properties.Resources.bg_大按钮1;
            btn列表_下移.FlatAppearance.BorderSize = 0;
            btn列表_下移.FlatStyle = FlatStyle.Flat;
            btn列表_下移.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn列表_下移.ForeColor = Color.White;
            btn列表_下移.Image = Properties.Resources.MoveDown1;
            btn列表_下移.Location = new Point(661, 632);
            btn列表_下移.Name = "btn列表_下移";
            btn列表_下移.Size = new Size(161, 55);
            btn列表_下移.TabIndex = 43;
            btn列表_下移.Text = "    下移  ";
            btn列表_下移.TextAlign = ContentAlignment.MiddleRight;
            btn列表_下移.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn列表_下移.UseVisualStyleBackColor = true;
            btn列表_下移.Click += btn列表_下移_Click;
            // 
            // btn列表_上移
            // 
            btn列表_上移.BackgroundImage = Properties.Resources.bg_大按钮1;
            btn列表_上移.FlatAppearance.BorderSize = 0;
            btn列表_上移.FlatStyle = FlatStyle.Flat;
            btn列表_上移.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn列表_上移.ForeColor = Color.White;
            btn列表_上移.Image = Properties.Resources.MoveUp1;
            btn列表_上移.Location = new Point(661, 569);
            btn列表_上移.Name = "btn列表_上移";
            btn列表_上移.Size = new Size(161, 55);
            btn列表_上移.TabIndex = 42;
            btn列表_上移.Text = "    上移  ";
            btn列表_上移.TextAlign = ContentAlignment.MiddleRight;
            btn列表_上移.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn列表_上移.UseVisualStyleBackColor = true;
            btn列表_上移.Click += btn列表_上移_Click;
            // 
            // btn清除
            // 
            btn清除.BackgroundImage = Properties.Resources.bg_大按钮1;
            btn清除.FlatAppearance.BorderSize = 0;
            btn清除.FlatStyle = FlatStyle.Flat;
            btn清除.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn清除.ForeColor = Color.White;
            btn清除.Image = Properties.Resources.Remove;
            btn清除.Location = new Point(661, 506);
            btn清除.Name = "btn清除";
            btn清除.Size = new Size(161, 55);
            btn清除.TabIndex = 39;
            btn清除.Text = "    清除  ";
            btn清除.TextAlign = ContentAlignment.MiddleRight;
            btn清除.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn清除.UseVisualStyleBackColor = true;
            btn清除.Click += btn清除_Click;
            // 
            // btn列表_之后插入
            // 
            btn列表_之后插入.BackgroundImage = Properties.Resources.bg_大按钮1;
            btn列表_之后插入.FlatAppearance.BorderSize = 0;
            btn列表_之后插入.FlatStyle = FlatStyle.Flat;
            btn列表_之后插入.Font = new Font("宋体", 10F, FontStyle.Regular, GraphicsUnit.Point);
            btn列表_之后插入.ForeColor = Color.White;
            btn列表_之后插入.Image = Properties.Resources.InsertAfter;
            btn列表_之后插入.Location = new Point(661, 443);
            btn列表_之后插入.Name = "btn列表_之后插入";
            btn列表_之后插入.Size = new Size(161, 55);
            btn列表_之后插入.TabIndex = 40;
            btn列表_之后插入.Text = " 在...之后插入";
            btn列表_之后插入.TextAlign = ContentAlignment.MiddleRight;
            btn列表_之后插入.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn列表_之后插入.UseVisualStyleBackColor = true;
            btn列表_之后插入.Click += btn列表_之后插入_Click;
            // 
            // btn列表_之前插入
            // 
            btn列表_之前插入.BackgroundImage = Properties.Resources.bg_大按钮1;
            btn列表_之前插入.FlatAppearance.BorderSize = 0;
            btn列表_之前插入.FlatStyle = FlatStyle.Flat;
            btn列表_之前插入.Font = new Font("宋体", 10F, FontStyle.Regular, GraphicsUnit.Point);
            btn列表_之前插入.ForeColor = Color.White;
            btn列表_之前插入.Image = Properties.Resources.InsertBefore;
            btn列表_之前插入.Location = new Point(661, 380);
            btn列表_之前插入.Name = "btn列表_之前插入";
            btn列表_之前插入.Size = new Size(161, 55);
            btn列表_之前插入.TabIndex = 41;
            btn列表_之前插入.Text = " 在...之前插入";
            btn列表_之前插入.TextAlign = ContentAlignment.MiddleRight;
            btn列表_之前插入.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn列表_之前插入.UseVisualStyleBackColor = true;
            btn列表_之前插入.Click += btn列表_之前插入_Click;
            // 
            // timer500ms
            // 
            timer500ms.Enabled = true;
            timer500ms.Interval = 500;
            timer500ms.Tick += timer500ms_Tick;
            // 
            // pnlSlitter
            // 
            pnlSlitter.BackColor = Color.Transparent;
            pnlSlitter.BackgroundImage = Properties.Resources.Slit6;
            pnlSlitter.BackgroundImageLayout = ImageLayout.Zoom;
            pnlSlitter.Location = new Point(44, 34);
            pnlSlitter.Name = "pnlSlitter";
            pnlSlitter.Size = new Size(50, 50);
            pnlSlitter.TabIndex = 44;
            pnlSlitter.Click += pnlSlitter_Click;
            pnlSlitter.MouseDown += pnlSlitter_MouseDown;
            pnlSlitter.MouseUp += pnlSlitter_MouseUp;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(137, 137, 137);
            dataGridViewCellStyle1.Font = new Font("Cambria", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = Color.Black;
            dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView1.BackgroundColor = Color.FromArgb(137, 137, 137);
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { 任务号 });
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(137, 137, 137);
            dataGridViewCellStyle2.Font = new Font("Cambria", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = Color.IndianRed;
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(83, 83, 83);
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            dataGridView1.Location = new Point(222, 389);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersVisible = false;
            dataGridViewCellStyle3.BackColor = Color.FromArgb(137, 137, 137);
            dataGridViewCellStyle3.Font = new Font("Cambria", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(83, 83, 83);
            dataGridViewCellStyle3.SelectionForeColor = Color.White;
            dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle3;
            dataGridView1.RowTemplate.Height = 200;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(340, 285);
            dataGridView1.TabIndex = 45;
            dataGridView1.TabStop = false;
            dataGridView1.CellClick += dataGridView1_CellClick;
            // 
            // 任务号
            // 
            任务号.HeaderText = "宽度0.0in；数量1";
            任务号.Name = "任务号";
            任务号.ReadOnly = true;
            任务号.Width = 330;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.InitialImage = Properties.Resources.bg_Slitter绘图2;
            pictureBox1.Location = new Point(197, 29);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(605, 241);
            pictureBox1.TabIndex = 46;
            pictureBox1.TabStop = false;
            // 
            // btn装载材料
            // 
            btn装载材料.BackgroundImage = Properties.Resources.bg_大按钮1;
            btn装载材料.FlatAppearance.BorderSize = 0;
            btn装载材料.FlatStyle = FlatStyle.Flat;
            btn装载材料.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn装载材料.ForeColor = Color.White;
            btn装载材料.Image = Properties.Resources.LoadMaterial;
            btn装载材料.Location = new Point(1165, 34);
            btn装载材料.Name = "btn装载材料";
            btn装载材料.Size = new Size(161, 55);
            btn装载材料.TabIndex = 8;
            btn装载材料.Text = "  装载材料";
            btn装载材料.TextAlign = ContentAlignment.MiddleRight;
            btn装载材料.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn装载材料.UseVisualStyleBackColor = true;
            btn装载材料.MouseDown += btn装载材料_MouseDown;
            btn装载材料.MouseUp += btn装载材料_MouseUp;
            // 
            // timer200ms
            // 
            timer200ms.Interval = 200;
            timer200ms.Tick += timer200ms_Tick;
            // 
            // SubOPSlitter
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackgroundImage = Properties.Resources.bg_Slitter_main;
            Controls.Add(dataGridView1);
            Controls.Add(pnlSlitter);
            Controls.Add(btn列表_下移);
            Controls.Add(btn列表_上移);
            Controls.Add(btn清除);
            Controls.Add(btn列表_之后插入);
            Controls.Add(btn列表_之前插入);
            Controls.Add(lb生产数量状态);
            Controls.Add(label11);
            Controls.Add(txb单项数量);
            Controls.Add(label10);
            Controls.Add(txb总宽);
            Controls.Add(label9);
            Controls.Add(label8);
            Controls.Add(txb单项宽);
            Controls.Add(label7);
            Controls.Add(pnl边角料选先);
            Controls.Add(pnl边角料选后);
            Controls.Add(lb最后);
            Controls.Add(sw边角料选);
            Controls.Add(lb首先);
            Controls.Add(btn装载材料);
            Controls.Add(btn分条机归原位);
            Controls.Add(pictureBox1);
            Name = "SubOPSlitter";
            Size = new Size(1516, 909);
            Load += SubOPSlitter_Load;
            VisibleChanged += SubOPSlitter_VisibleChanged;
            pnl边角料选后.ResumeLayout(false);
            pnl边角料选后.PerformLayout();
            pnl边角料选先.ResumeLayout(false);
            pnl边角料选先.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private Button btn分条机归原位;
        private Button btn边角料选;
        private Label lb首先;
        private Label lb最后;
        private Panel pnl边角料选后;
        private Label label3;
        private Label lb边角料长度_后;
        private Panel panel2;
        private Panel pnl边角料选先;
        private Panel panel4;
        private Label lb边角料长度_先;
        private Label label6;
        private Label label7;
        private Label label8;
        private TextBox txb单项宽;
        private Label label9;
        private TextBox txb总宽;
        private Label label10;
        private Label label11;
        private TextBox txb单项数量;
        private Label lb生产数量状态;
        private Button btn列表_下移;
        private Button btn列表_上移;
        private Button btn清除;
        private Button btn列表_之后插入;
        private Button btn列表_之前插入;
        private System.Windows.Forms.Timer timer500ms;
        private Panel pnlSlitter;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn 任务号;
        private Button sw边角料选;
        private PictureBox pictureBox1;
        private Button btn装载材料;
        private System.Windows.Forms.Timer timer200ms;
    }
}
