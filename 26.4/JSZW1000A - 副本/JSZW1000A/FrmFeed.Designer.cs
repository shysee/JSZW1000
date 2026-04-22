namespace JSZW1000A
{
    partial class FrmFeed
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
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmFeed));
            dataGridView1 = new DataGridView();
            Index = new DataGridViewTextBoxColumn();
            Length = new DataGridViewTextBoxColumn();
            Width = new DataGridViewTextBoxColumn();
            Thickness = new DataGridViewTextBoxColumn();
            Quantity = new DataGridViewTextBoxColumn();
            GroupsNeeded = new DataGridViewTextBoxColumn();
            Batch = new DataGridViewTextBoxColumn();
            Total = new DataGridViewTextBoxColumn();
            btn列表插入 = new Button();
            pos2 = new TextBox();
            label117 = new Label();
            pos1 = new TextBox();
            label119 = new Label();
            btn料架后退 = new Button();
            btn料架前进 = new Button();
            txb进料定位 = new TextBox();
            label9 = new Label();
            btn移动装料架 = new Button();
            sw吸盘2 = new Button();
            label2 = new Label();
            sw吸盘1 = new Button();
            label1 = new Label();
            btn确认 = new Button();
            btn列表_清除 = new Button();
            button1 = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            txb进料位置 = new TextBox();
            label6 = new Label();
            sw吸盘4 = new Button();
            label4 = new Label();
            sw吸盘3 = new Button();
            label3 = new Label();
            pos4 = new TextBox();
            label5 = new Label();
            pos3 = new TextBox();
            label7 = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(137, 137, 137);
            dataGridViewCellStyle1.Font = new Font("Cambria", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = Color.Black;
            dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView1.BackgroundColor = Color.FromArgb(137, 137, 137);
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { Index, Length, Width, Thickness, Quantity, GroupsNeeded, Batch, Total });
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = Color.FromArgb(137, 137, 137);
            dataGridViewCellStyle5.Font = new Font("Cambria", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle5.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = Color.FromArgb(83, 83, 83);
            dataGridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = DataGridViewTriState.False;
            dataGridView1.DefaultCellStyle = dataGridViewCellStyle5;
            dataGridView1.Location = new Point(42, 23);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersVisible = false;
            dataGridViewCellStyle6.BackColor = Color.FromArgb(137, 137, 137);
            dataGridViewCellStyle6.Font = new Font("Cambria", 12F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle6.ForeColor = Color.Black;
            dataGridViewCellStyle6.SelectionBackColor = Color.FromArgb(83, 83, 83);
            dataGridViewCellStyle6.SelectionForeColor = Color.White;
            dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle6;
            dataGridView1.RowTemplate.Height = 40;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(596, 312);
            dataGridView1.TabIndex = 45;
            dataGridView1.TabStop = false;
            // 
            // Index
            // 
            Index.HeaderText = "序号";
            Index.Name = "Index";
            Index.ReadOnly = true;
            Index.Width = 40;
            // 
            // Length
            // 
            dataGridViewCellStyle2.Font = new Font("宋体", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = Color.White;
            Length.DefaultCellStyle = dataGridViewCellStyle2;
            Length.HeaderText = "长度";
            Length.Name = "Length";
            Length.Width = 80;
            // 
            // Width
            // 
            dataGridViewCellStyle3.BackColor = Color.FromArgb(137, 137, 137);
            dataGridViewCellStyle3.Font = new Font("宋体", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = Color.White;
            Width.DefaultCellStyle = dataGridViewCellStyle3;
            Width.HeaderText = "宽度";
            Width.Name = "Width";
            Width.Resizable = DataGridViewTriState.True;
            Width.SortMode = DataGridViewColumnSortMode.NotSortable;
            Width.Width = 80;
            // 
            // Thickness
            // 
            dataGridViewCellStyle4.Font = new Font("宋体", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle4.ForeColor = Color.White;
            Thickness.DefaultCellStyle = dataGridViewCellStyle4;
            Thickness.HeaderText = "厚度";
            Thickness.Name = "Thickness";
            Thickness.Resizable = DataGridViewTriState.True;
            Thickness.SortMode = DataGridViewColumnSortMode.NotSortable;
            Thickness.Width = 80;
            // 
            // Quantity
            // 
            Quantity.HeaderText = "块数";
            Quantity.Name = "Quantity";
            Quantity.Width = 80;
            // 
            // GroupsNeeded
            // 
            GroupsNeeded.HeaderText = "组数";
            GroupsNeeded.MaxInputLength = 2;
            GroupsNeeded.Name = "GroupsNeeded";
            GroupsNeeded.Width = 80;
            // 
            // Batch
            // 
            Batch.HeaderText = "批次";
            Batch.Name = "Batch";
            Batch.Width = 80;
            // 
            // Total
            // 
            Total.HeaderText = "总数";
            Total.Name = "Total";
            Total.ReadOnly = true;
            Total.Width = 70;
            // 
            // btn列表插入
            // 
            btn列表插入.BackgroundImage = Properties.Resources.bg_大按钮1;
            btn列表插入.BackgroundImageLayout = ImageLayout.Stretch;
            btn列表插入.FlatAppearance.BorderSize = 0;
            btn列表插入.FlatStyle = FlatStyle.Flat;
            btn列表插入.Font = new Font("宋体", 10F, FontStyle.Regular, GraphicsUnit.Point);
            btn列表插入.ForeColor = Color.White;
            btn列表插入.Image = Properties.Resources.InsertBefore;
            btn列表插入.Location = new Point(700, 23);
            btn列表插入.Name = "btn列表插入";
            btn列表插入.Size = new Size(161, 55);
            btn列表插入.TabIndex = 48;
            btn列表插入.Text = "     插入   ";
            btn列表插入.TextAlign = ContentAlignment.MiddleRight;
            btn列表插入.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn列表插入.UseVisualStyleBackColor = true;
            btn列表插入.Click += btn列表插入_Click;
            // 
            // pos2
            // 
            pos2.Font = new Font("Microsoft YaHei UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            pos2.Location = new Point(869, 401);
            pos2.Name = "pos2";
            pos2.Size = new Size(60, 27);
            pos2.TabIndex = 147;
            pos2.Text = "0";
            pos2.TextAlign = HorizontalAlignment.Right;
            // 
            // label117
            // 
            label117.AutoSize = true;
            label117.BackColor = Color.Transparent;
            label117.Font = new Font("微软雅黑", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label117.ForeColor = Color.FromArgb(208, 208, 208);
            label117.Location = new Point(816, 405);
            label117.Name = "label117";
            label117.Size = new Size(51, 21);
            label117.TabIndex = 148;
            label117.Text = "位置2";
            // 
            // pos1
            // 
            pos1.Font = new Font("Microsoft YaHei UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            pos1.Location = new Point(869, 366);
            pos1.Name = "pos1";
            pos1.Size = new Size(60, 27);
            pos1.TabIndex = 144;
            pos1.Text = "0";
            pos1.TextAlign = HorizontalAlignment.Right;
            // 
            // label119
            // 
            label119.AutoSize = true;
            label119.BackColor = Color.Transparent;
            label119.Font = new Font("微软雅黑", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label119.ForeColor = Color.FromArgb(208, 208, 208);
            label119.Location = new Point(816, 370);
            label119.Name = "label119";
            label119.Size = new Size(51, 21);
            label119.TabIndex = 145;
            label119.Text = "位置1";
            // 
            // btn料架后退
            // 
            btn料架后退.BackgroundImage = Properties.Resources.bg_大按钮1;
            btn料架后退.BackgroundImageLayout = ImageLayout.Stretch;
            btn料架后退.FlatAppearance.BorderSize = 0;
            btn料架后退.FlatStyle = FlatStyle.Flat;
            btn料架后退.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn料架后退.ForeColor = Color.White;
            btn料架后退.Image = (Image)resources.GetObject("btn料架后退.Image");
            btn料架后退.Location = new Point(229, 446);
            btn料架后退.Name = "btn料架后退";
            btn料架后退.Size = new Size(161, 55);
            btn料架后退.TabIndex = 158;
            btn料架后退.Text = "    后退  ";
            btn料架后退.TextAlign = ContentAlignment.MiddleRight;
            btn料架后退.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn料架后退.UseVisualStyleBackColor = true;
            btn料架后退.MouseDown += btn_MouseDown;
            btn料架后退.MouseUp += btn_MouseUp;
            // 
            // btn料架前进
            // 
            btn料架前进.BackgroundImage = Properties.Resources.bg_大按钮1;
            btn料架前进.BackgroundImageLayout = ImageLayout.Stretch;
            btn料架前进.FlatAppearance.BorderSize = 0;
            btn料架前进.FlatStyle = FlatStyle.Flat;
            btn料架前进.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn料架前进.ForeColor = Color.White;
            btn料架前进.Image = Properties.Resources.R;
            btn料架前进.Location = new Point(229, 365);
            btn料架前进.Name = "btn料架前进";
            btn料架前进.Size = new Size(161, 55);
            btn料架前进.TabIndex = 157;
            btn料架前进.Text = "    前进  ";
            btn料架前进.TextAlign = ContentAlignment.MiddleRight;
            btn料架前进.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn料架前进.UseVisualStyleBackColor = true;
            btn料架前进.MouseDown += btn_MouseDown;
            btn料架前进.MouseUp += btn_MouseUp;
            // 
            // txb进料定位
            // 
            txb进料定位.Font = new Font("Microsoft YaHei UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            txb进料定位.Location = new Point(60, 369);
            txb进料定位.Name = "txb进料定位";
            txb进料定位.Size = new Size(79, 26);
            txb进料定位.TabIndex = 156;
            txb进料定位.Text = "500.0";
            txb进料定位.KeyDown += txb进料定位_KeyDown;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.BackColor = Color.Transparent;
            label9.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label9.ForeColor = Color.FromArgb(208, 208, 208);
            label9.Location = new Point(145, 375);
            label9.Name = "label9";
            label9.Size = new Size(37, 15);
            label9.TabIndex = 155;
            label9.Text = "毫米";
            // 
            // btn移动装料架
            // 
            btn移动装料架.BackgroundImage = Properties.Resources.bg_大按钮1;
            btn移动装料架.BackgroundImageLayout = ImageLayout.Stretch;
            btn移动装料架.FlatAppearance.BorderSize = 0;
            btn移动装料架.FlatStyle = FlatStyle.Flat;
            btn移动装料架.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn移动装料架.ForeColor = Color.White;
            btn移动装料架.Image = Properties.Resources.Z;
            btn移动装料架.Location = new Point(42, 446);
            btn移动装料架.Name = "btn移动装料架";
            btn移动装料架.Size = new Size(161, 55);
            btn移动装料架.TabIndex = 154;
            btn移动装料架.Text = "    移动 \r\n   装料架";
            btn移动装料架.TextAlign = ContentAlignment.MiddleRight;
            btn移动装料架.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn移动装料架.UseVisualStyleBackColor = true;
            btn移动装料架.MouseDown += btn_MouseDown;
            btn移动装料架.MouseUp += btn_MouseUp;
            // 
            // sw吸盘2
            // 
            sw吸盘2.BackgroundImage = Properties.Resources.sw_左右小开关0;
            sw吸盘2.BackgroundImageLayout = ImageLayout.Stretch;
            sw吸盘2.FlatAppearance.BorderSize = 0;
            sw吸盘2.FlatStyle = FlatStyle.Flat;
            sw吸盘2.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            sw吸盘2.ForeColor = Color.White;
            sw吸盘2.Location = new Point(509, 452);
            sw吸盘2.Name = "sw吸盘2";
            sw吸盘2.Size = new Size(80, 40);
            sw吸盘2.TabIndex = 152;
            sw吸盘2.TextAlign = ContentAlignment.MiddleRight;
            sw吸盘2.TextImageRelation = TextImageRelation.ImageBeforeText;
            sw吸盘2.UseVisualStyleBackColor = true;
            sw吸盘2.Click += sw吸盘2_Click;
            // 
            // label2
            // 
            label2.BackColor = Color.Gray;
            label2.BorderStyle = BorderStyle.FixedSingle;
            label2.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            label2.ForeColor = SystemColors.Control;
            label2.Image = Properties.Resources.bg_main_big;
            label2.Location = new Point(415, 446);
            label2.Name = "label2";
            label2.Size = new Size(185, 53);
            label2.TabIndex = 153;
            label2.Text = "吸盘组2";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // sw吸盘1
            // 
            sw吸盘1.BackgroundImage = Properties.Resources.sw_左右小开关0;
            sw吸盘1.BackgroundImageLayout = ImageLayout.Stretch;
            sw吸盘1.FlatAppearance.BorderSize = 0;
            sw吸盘1.FlatStyle = FlatStyle.Flat;
            sw吸盘1.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            sw吸盘1.ForeColor = Color.White;
            sw吸盘1.Location = new Point(509, 373);
            sw吸盘1.Name = "sw吸盘1";
            sw吸盘1.Size = new Size(80, 40);
            sw吸盘1.TabIndex = 150;
            sw吸盘1.TextAlign = ContentAlignment.MiddleRight;
            sw吸盘1.TextImageRelation = TextImageRelation.ImageBeforeText;
            sw吸盘1.UseVisualStyleBackColor = true;
            sw吸盘1.Click += sw吸盘1_Click;
            // 
            // label1
            // 
            label1.BackColor = Color.Gray;
            label1.BorderStyle = BorderStyle.FixedSingle;
            label1.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            label1.ForeColor = SystemColors.Control;
            label1.Image = Properties.Resources.bg_main_big;
            label1.Location = new Point(415, 367);
            label1.Name = "label1";
            label1.Size = new Size(185, 53);
            label1.TabIndex = 151;
            label1.Text = "吸盘组1";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btn确认
            // 
            btn确认.BackgroundImage = Properties.Resources.bg_大按钮1;
            btn确认.BackgroundImageLayout = ImageLayout.Stretch;
            btn确认.FlatAppearance.BorderSize = 0;
            btn确认.FlatStyle = FlatStyle.Flat;
            btn确认.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn确认.ForeColor = Color.White;
            btn确认.Image = (Image)resources.GetObject("btn确认.Image");
            btn确认.Location = new Point(700, 196);
            btn确认.Name = "btn确认";
            btn确认.Size = new Size(161, 55);
            btn确认.TabIndex = 159;
            btn确认.Text = "    确认  ";
            btn确认.TextAlign = ContentAlignment.MiddleRight;
            btn确认.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn确认.UseVisualStyleBackColor = true;
            btn确认.Click += btn确认_Click;
            // 
            // btn列表_清除
            // 
            btn列表_清除.BackgroundImage = Properties.Resources.bg_大按钮1;
            btn列表_清除.BackgroundImageLayout = ImageLayout.Stretch;
            btn列表_清除.FlatAppearance.BorderSize = 0;
            btn列表_清除.FlatStyle = FlatStyle.Flat;
            btn列表_清除.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn列表_清除.ForeColor = Color.White;
            btn列表_清除.Image = Properties.Resources.Remove;
            btn列表_清除.Location = new Point(700, 106);
            btn列表_清除.Name = "btn列表_清除";
            btn列表_清除.Size = new Size(161, 55);
            btn列表_清除.TabIndex = 160;
            btn列表_清除.Text = "    删除  ";
            btn列表_清除.TextAlign = ContentAlignment.MiddleRight;
            btn列表_清除.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn列表_清除.UseVisualStyleBackColor = true;
            btn列表_清除.Click += btn列表_清除_Click;
            // 
            // button1
            // 
            button1.BackgroundImage = Properties.Resources.bg_大按钮1;
            button1.BackgroundImageLayout = ImageLayout.Stretch;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            button1.ForeColor = Color.White;
            button1.Image = Properties.Resources.Undo;
            button1.Location = new Point(700, 280);
            button1.Name = "button1";
            button1.Size = new Size(161, 55);
            button1.TabIndex = 163;
            button1.Text = "    返回  ";
            button1.TextAlign = ContentAlignment.MiddleRight;
            button1.TextImageRelation = TextImageRelation.ImageBeforeText;
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 500;
            timer1.Tick += timer1_Tick;
            // 
            // txb进料位置
            // 
            txb进料位置.BackColor = Color.FromArgb(104, 110, 114);
            txb进料位置.BorderStyle = BorderStyle.FixedSingle;
            txb进料位置.Font = new Font("Calibri", 12.75F, FontStyle.Bold, GraphicsUnit.Point);
            txb进料位置.ForeColor = Color.White;
            txb进料位置.Location = new Point(60, 401);
            txb进料位置.Name = "txb进料位置";
            txb进料位置.ReadOnly = true;
            txb进料位置.Size = new Size(79, 28);
            txb进料位置.TabIndex = 164;
            txb进料位置.Text = "210.0";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.BackColor = Color.Transparent;
            label6.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label6.ForeColor = Color.FromArgb(208, 208, 208);
            label6.Location = new Point(145, 406);
            label6.Name = "label6";
            label6.Size = new Size(37, 15);
            label6.TabIndex = 165;
            label6.Text = "毫米";
            // 
            // sw吸盘4
            // 
            sw吸盘4.BackgroundImage = Properties.Resources.sw_左右小开关0;
            sw吸盘4.BackgroundImageLayout = ImageLayout.Stretch;
            sw吸盘4.FlatAppearance.BorderSize = 0;
            sw吸盘4.FlatStyle = FlatStyle.Flat;
            sw吸盘4.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            sw吸盘4.ForeColor = Color.White;
            sw吸盘4.Location = new Point(721, 454);
            sw吸盘4.Name = "sw吸盘4";
            sw吸盘4.Size = new Size(80, 40);
            sw吸盘4.TabIndex = 168;
            sw吸盘4.TextAlign = ContentAlignment.MiddleRight;
            sw吸盘4.TextImageRelation = TextImageRelation.ImageBeforeText;
            sw吸盘4.UseVisualStyleBackColor = true;
            sw吸盘4.Visible = false;
            //sw吸盘4.Click += sw吸盘4_Click;
            // 
            // label4
            // 
            label4.BackColor = Color.Gray;
            label4.BorderStyle = BorderStyle.FixedSingle;
            label4.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            label4.ForeColor = SystemColors.Control;
            label4.Image = Properties.Resources.bg_main_big;
            label4.Location = new Point(627, 448);
            label4.Name = "label4";
            label4.Size = new Size(185, 53);
            label4.TabIndex = 169;
            label4.Text = "吸盘组4";
            label4.TextAlign = ContentAlignment.MiddleLeft;
            label4.Visible = false;
            // 
            // sw吸盘3
            // 
            sw吸盘3.BackgroundImage = Properties.Resources.sw_左右小开关0;
            sw吸盘3.BackgroundImageLayout = ImageLayout.Stretch;
            sw吸盘3.FlatAppearance.BorderSize = 0;
            sw吸盘3.FlatStyle = FlatStyle.Flat;
            sw吸盘3.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            sw吸盘3.ForeColor = Color.White;
            sw吸盘3.Location = new Point(721, 375);
            sw吸盘3.Name = "sw吸盘3";
            sw吸盘3.Size = new Size(80, 40);
            sw吸盘3.TabIndex = 166;
            sw吸盘3.TextImageRelation = TextImageRelation.ImageBeforeText;
            sw吸盘3.UseVisualStyleBackColor = true;
            sw吸盘3.Visible = false;
            //sw吸盘3.Click += sw吸盘3_Click;
            // 
            // label3
            // 
            label3.BackColor = Color.Gray;
            label3.BorderStyle = BorderStyle.FixedSingle;
            label3.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            label3.ForeColor = SystemColors.Control;
            label3.Image = Properties.Resources.bg_main_big;
            label3.Location = new Point(627, 369);
            label3.Name = "label3";
            label3.Size = new Size(185, 53);
            label3.TabIndex = 167;
            label3.Text = "吸盘组3";
            label3.TextAlign = ContentAlignment.MiddleLeft;
            label3.Visible = false;
            // 
            // pos4
            // 
            pos4.Font = new Font("Microsoft YaHei UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            pos4.Location = new Point(869, 471);
            pos4.Name = "pos4";
            pos4.Size = new Size(60, 27);
            pos4.TabIndex = 172;
            pos4.Text = "0";
            pos4.TextAlign = HorizontalAlignment.Right;
            pos4.Visible = false;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = Color.Transparent;
            label5.Font = new Font("微软雅黑", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label5.ForeColor = Color.FromArgb(208, 208, 208);
            label5.Location = new Point(816, 475);
            label5.Name = "label5";
            label5.Size = new Size(51, 21);
            label5.TabIndex = 173;
            label5.Text = "位置4";
            label5.Visible = false;
            // 
            // pos3
            // 
            pos3.Font = new Font("Microsoft YaHei UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            pos3.Location = new Point(869, 436);
            pos3.Name = "pos3";
            pos3.Size = new Size(60, 27);
            pos3.TabIndex = 170;
            pos3.Text = "0";
            pos3.TextAlign = HorizontalAlignment.Right;
            pos3.Visible = false;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.BackColor = Color.Transparent;
            label7.Font = new Font("微软雅黑", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label7.ForeColor = Color.FromArgb(208, 208, 208);
            label7.Location = new Point(816, 440);
            label7.Name = "label7";
            label7.Size = new Size(51, 21);
            label7.TabIndex = 171;
            label7.Text = "位置3";
            label7.Visible = false;
            // 
            // FrmFeed
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.bg_main_big;
            ClientSize = new Size(944, 537);
            Controls.Add(pos4);
            Controls.Add(label5);
            Controls.Add(pos3);
            Controls.Add(label7);
            Controls.Add(sw吸盘4);
            Controls.Add(label4);
            Controls.Add(sw吸盘3);
            Controls.Add(label3);
            Controls.Add(label6);
            Controls.Add(txb进料位置);
            Controls.Add(button1);
            Controls.Add(btn列表_清除);
            Controls.Add(btn确认);
            Controls.Add(btn料架后退);
            Controls.Add(btn料架前进);
            Controls.Add(txb进料定位);
            Controls.Add(label9);
            Controls.Add(btn移动装料架);
            Controls.Add(sw吸盘2);
            Controls.Add(label2);
            Controls.Add(sw吸盘1);
            Controls.Add(label1);
            Controls.Add(pos2);
            Controls.Add(label117);
            Controls.Add(pos1);
            Controls.Add(label119);
            Controls.Add(btn列表插入);
            Controls.Add(dataGridView1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FrmFeed";
            Text = "FrmFeed";
            Load += FrmFeed_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView1;
        private Button btn列表插入;
        private Label label116;
        private TextBox pos2;
        private Label label117;
        private Label label118;
        private TextBox pos1;
        private Label label119;
        private Button btn料架后退;
        private Button btn料架前进;
        private TextBox txb进料定位;
        private Label label9;
        private Button btn移动装料架;
        private Button sw吸盘2;
        private Label label2;
        private Button sw吸盘1;
        private Label label1;
        private Button btn确认;
        private Button btn列表_清除;
        private Button button1;
        private System.Windows.Forms.Timer timer1;
        private TextBox txb进料位置;
        private Label label6;
        private Button sw吸盘4;
        private Label label4;
        private Button sw吸盘3;
        private Label label3;
        private TextBox pos4;
        private Label label5;
        private TextBox pos3;
        private Label label7;
        private DataGridViewTextBoxColumn Index;
        private DataGridViewTextBoxColumn Length;
        private DataGridViewTextBoxColumn Width;
        private DataGridViewTextBoxColumn Thickness;
        private DataGridViewTextBoxColumn Quantity;
        private DataGridViewTextBoxColumn GroupsNeeded;
        private DataGridViewTextBoxColumn Batch;
        private DataGridViewTextBoxColumn Total;
    }
}