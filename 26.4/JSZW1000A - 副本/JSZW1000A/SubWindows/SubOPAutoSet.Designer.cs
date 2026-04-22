namespace JSZW1000A.SubWindows
{
    partial class SubOPAutoSet
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
            panel10 = new Panel();
            btn装载材料 = new Button();
            textBox8 = new TextBox();
            txb计数 = new TextBox();
            btn重置计数 = new Button();
            sw分条开关 = new Button();
            txbSpringBtm = new TextBox();
            txbSpringTop = new TextBox();
            txb待分条板宽 = new TextBox();
            textBox1 = new TextBox();
            txb计算总宽 = new TextBox();
            lb分条关 = new Label();
            lb分条开 = new Label();
            label20 = new Label();
            label8 = new Label();
            label22 = new Label();
            label24 = new Label();
            label23 = new Label();
            label21 = new Label();
            label18 = new Label();
            label7 = new Label();
            pnlAuto = new Panel();
            lb颜色侧翻 = new Label();
            sw颜色面 = new Button();
            lb正逆序 = new Label();
            sw折弯顺序 = new Button();
            pnl左工具栏2 = new Panel();
            panel2 = new Panel();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label10 = new Label();
            button1 = new Button();
            pnl角度尺寸 = new Panel();
            lb工作单_比例 = new Label();
            label2 = new Label();
            lb工作单_角度 = new Label();
            lb工作单_尺寸 = new Label();
            lb工作单_角度尺寸 = new Label();
            btn角度尺寸 = new Button();
            btnMoveUp = new Button();
            lb颜色面 = new Label();
            lb折弯方向 = new Label();
            btnRstView = new Button();
            btnMoveDn = new Button();
            panel1 = new Panel();
            panel10.SuspendLayout();
            pnl左工具栏2.SuspendLayout();
            panel2.SuspendLayout();
            pnl角度尺寸.SuspendLayout();
            SuspendLayout();
            // 
            // panel10
            // 
            panel10.BackgroundImage = Properties.Resources.Auto1_Top;
            panel10.Controls.Add(btn装载材料);
            panel10.Controls.Add(textBox8);
            panel10.Controls.Add(txb计数);
            panel10.Controls.Add(btn重置计数);
            panel10.Controls.Add(sw分条开关);
            panel10.Controls.Add(txbSpringBtm);
            panel10.Controls.Add(txbSpringTop);
            panel10.Controls.Add(txb待分条板宽);
            panel10.Controls.Add(textBox1);
            panel10.Controls.Add(txb计算总宽);
            panel10.Controls.Add(lb分条关);
            panel10.Controls.Add(lb分条开);
            panel10.Controls.Add(label20);
            panel10.Controls.Add(label8);
            panel10.Controls.Add(label22);
            panel10.Controls.Add(label24);
            panel10.Controls.Add(label23);
            panel10.Controls.Add(label21);
            panel10.Controls.Add(label18);
            panel10.Controls.Add(label7);
            panel10.Controls.Add(pnlAuto);
            panel10.Location = new Point(2, 2);
            panel10.Name = "panel10";
            panel10.Size = new Size(1516, 100);
            panel10.TabIndex = 3;
            // 
            // btn装载材料
            // 
            btn装载材料.BackgroundImage = Properties.Resources.bg_中按钮1;
            btn装载材料.FlatAppearance.BorderSize = 0;
            btn装载材料.FlatStyle = FlatStyle.Flat;
            btn装载材料.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn装载材料.ForeColor = Color.White;
            btn装载材料.Image = Properties.Resources.LoadMaterial;
            btn装载材料.Location = new Point(132, 8);
            btn装载材料.Name = "btn装载材料";
            btn装载材料.Size = new Size(113, 54);
            btn装载材料.TabIndex = 51;
            btn装载材料.Text = " 装载\r\n 材料";
            btn装载材料.TextAlign = ContentAlignment.MiddleRight;
            btn装载材料.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn装载材料.UseVisualStyleBackColor = true;
            // 
            // textBox8
            // 
            textBox8.Location = new Point(98, 68);
            textBox8.Name = "textBox8";
            textBox8.Size = new Size(76, 23);
            textBox8.TabIndex = 50;
            textBox8.Visible = false;
            // 
            // txb计数
            // 
            txb计数.BackColor = Color.FromArgb(104, 110, 114);
            txb计数.BorderStyle = BorderStyle.FixedSingle;
            txb计数.Font = new Font("Calibri", 20F, FontStyle.Regular, GraphicsUnit.Point);
            txb计数.ForeColor = Color.White;
            txb计数.Location = new Point(1450, 25);
            txb计数.Name = "txb计数";
            txb计数.ReadOnly = true;
            txb计数.Size = new Size(48, 40);
            txb计数.TabIndex = 43;
            txb计数.Text = "6";
            // 
            // btn重置计数
            // 
            btn重置计数.BackgroundImage = Properties.Resources.bg_中按钮1;
            btn重置计数.FlatAppearance.BorderSize = 0;
            btn重置计数.FlatStyle = FlatStyle.Flat;
            btn重置计数.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn重置计数.ForeColor = Color.FromArgb(208, 208, 208);
            btn重置计数.Image = Properties.Resources.ResetCount;
            btn重置计数.Location = new Point(1331, 20);
            btn重置计数.Name = "btn重置计数";
            btn重置计数.Size = new Size(113, 54);
            btn重置计数.TabIndex = 41;
            btn重置计数.Text = " 重置\r\n 计数";
            btn重置计数.TextAlign = ContentAlignment.MiddleRight;
            btn重置计数.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn重置计数.UseVisualStyleBackColor = true;
            // 
            // sw分条开关
            // 
            sw分条开关.FlatAppearance.BorderColor = Color.FromArgb(67, 67, 67);
            sw分条开关.FlatAppearance.BorderSize = 0;
            sw分条开关.FlatAppearance.MouseDownBackColor = Color.FromArgb(67, 67, 67);
            sw分条开关.FlatAppearance.MouseOverBackColor = Color.FromArgb(67, 67, 67);
            sw分条开关.FlatStyle = FlatStyle.Flat;
            sw分条开关.Image = Properties.Resources.btm_分条开关1;
            sw分条开关.Location = new Point(728, 15);
            sw分条开关.Name = "sw分条开关";
            sw分条开关.Size = new Size(90, 66);
            sw分条开关.TabIndex = 40;
            sw分条开关.UseVisualStyleBackColor = true;
            // 
            // txbSpringBtm
            // 
            txbSpringBtm.Font = new Font("Calibri", 15F, FontStyle.Regular, GraphicsUnit.Point);
            txbSpringBtm.Location = new Point(332, 54);
            txbSpringBtm.Name = "txbSpringBtm";
            txbSpringBtm.Size = new Size(52, 32);
            txbSpringBtm.TabIndex = 48;
            txbSpringBtm.Text = "3.00";
            // 
            // txbSpringTop
            // 
            txbSpringTop.Font = new Font("Calibri", 15F, FontStyle.Regular, GraphicsUnit.Point);
            txbSpringTop.Location = new Point(332, 11);
            txbSpringTop.Name = "txbSpringTop";
            txbSpringTop.Size = new Size(52, 32);
            txbSpringTop.TabIndex = 48;
            txbSpringTop.Text = "2.00";
            // 
            // txb待分条板宽
            // 
            txb待分条板宽.Font = new Font("Calibri", 13F, FontStyle.Regular, GraphicsUnit.Point);
            txb待分条板宽.Location = new Point(581, 57);
            txb待分条板宽.Name = "txb待分条板宽";
            txb待分条板宽.Size = new Size(59, 29);
            txb待分条板宽.TabIndex = 48;
            txb待分条板宽.Text = "1200";
            // 
            // textBox1
            // 
            textBox1.Font = new Font("Calibri", 13F, FontStyle.Regular, GraphicsUnit.Point);
            textBox1.Location = new Point(414, 57);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(59, 29);
            textBox1.TabIndex = 48;
            textBox1.Text = "1";
            // 
            // txb计算总宽
            // 
            txb计算总宽.BackColor = Color.FromArgb(104, 110, 114);
            txb计算总宽.BorderStyle = BorderStyle.FixedSingle;
            txb计算总宽.Font = new Font("Calibri", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            txb计算总宽.ForeColor = Color.White;
            txb计算总宽.Location = new Point(844, 47);
            txb计算总宽.Name = "txb计算总宽";
            txb计算总宽.Size = new Size(59, 31);
            txb计算总宽.TabIndex = 48;
            txb计算总宽.Text = "1";
            // 
            // lb分条关
            // 
            lb分条关.AutoSize = true;
            lb分条关.BackColor = Color.Transparent;
            lb分条关.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            lb分条关.ForeColor = Color.FromArgb(208, 208, 208);
            lb分条关.Location = new Point(702, 71);
            lb分条关.Name = "lb分条关";
            lb分条关.Size = new Size(22, 15);
            lb分条关.TabIndex = 39;
            lb分条关.Text = "关";
            // 
            // lb分条开
            // 
            lb分条开.AutoSize = true;
            lb分条开.BackColor = Color.Transparent;
            lb分条开.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            lb分条开.ForeColor = Color.FromArgb(208, 208, 208);
            lb分条开.Location = new Point(687, 41);
            lb分条开.Name = "lb分条开";
            lb分条开.Size = new Size(37, 15);
            lb分条开.TabIndex = 38;
            lb分条开.Text = "分条";
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.BackColor = Color.Transparent;
            label20.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label20.ForeColor = Color.FromArgb(208, 208, 208);
            label20.Location = new Point(646, 68);
            label20.Name = "label20";
            label20.Size = new Size(37, 15);
            label20.TabIndex = 0;
            label20.Text = "毫米";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.BackColor = Color.Transparent;
            label8.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label8.ForeColor = Color.FromArgb(208, 208, 208);
            label8.Location = new Point(909, 54);
            label8.Name = "label8";
            label8.Size = new Size(37, 15);
            label8.TabIndex = 27;
            label8.Text = "毫米";
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.BackColor = Color.Transparent;
            label22.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label22.ForeColor = Color.FromArgb(208, 208, 208);
            label22.Location = new Point(289, 54);
            label22.Name = "label22";
            label22.Size = new Size(37, 30);
            label22.TabIndex = 26;
            label22.Text = "顶部\r\n回弹";
            // 
            // label24
            // 
            label24.AutoSize = true;
            label24.BackColor = Color.Transparent;
            label24.Font = new Font("宋体", 15F, FontStyle.Regular, GraphicsUnit.Point);
            label24.ForeColor = Color.FromArgb(208, 208, 208);
            label24.Location = new Point(384, 57);
            label24.Name = "label24";
            label24.Size = new Size(29, 20);
            label24.TabIndex = 26;
            label24.Text = "°";
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.BackColor = Color.Transparent;
            label23.Font = new Font("宋体", 15F, FontStyle.Regular, GraphicsUnit.Point);
            label23.ForeColor = Color.FromArgb(208, 208, 208);
            label23.Location = new Point(384, 9);
            label23.Name = "label23";
            label23.Size = new Size(29, 20);
            label23.TabIndex = 26;
            label23.Text = "°";
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.BackColor = Color.Transparent;
            label21.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label21.ForeColor = Color.FromArgb(208, 208, 208);
            label21.Location = new Point(289, 12);
            label21.Name = "label21";
            label21.Size = new Size(37, 30);
            label21.TabIndex = 26;
            label21.Text = "顶部\r\n回弹";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.BackColor = Color.Transparent;
            label18.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label18.ForeColor = Color.FromArgb(208, 208, 208);
            label18.Location = new Point(581, 15);
            label18.Name = "label18";
            label18.Size = new Size(75, 30);
            label18.TabIndex = 26;
            label18.Text = "待分条\r\n板材宽度:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.BackColor = Color.Transparent;
            label7.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label7.ForeColor = Color.FromArgb(208, 208, 208);
            label7.Location = new Point(835, 20);
            label7.Name = "label7";
            label7.Size = new Size(120, 15);
            label7.TabIndex = 26;
            label7.Text = "计算的板材宽度:";
            // 
            // pnlAuto
            // 
            pnlAuto.BackColor = Color.Transparent;
            pnlAuto.BackgroundImage = Properties.Resources.AutoOrig1_zh_CHS;
            pnlAuto.BackgroundImageLayout = ImageLayout.Zoom;
            pnlAuto.Location = new Point(22, 20);
            pnlAuto.Name = "pnlAuto";
            pnlAuto.Size = new Size(56, 56);
            pnlAuto.TabIndex = 4;
            // 
            // lb颜色侧翻
            // 
            lb颜色侧翻.AutoSize = true;
            lb颜色侧翻.BackColor = Color.Transparent;
            lb颜色侧翻.Font = new Font("微软雅黑", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            lb颜色侧翻.ForeColor = Color.White;
            lb颜色侧翻.Location = new Point(112, 124);
            lb颜色侧翻.Name = "lb颜色侧翻";
            lb颜色侧翻.Size = new Size(88, 25);
            lb颜色侧翻.TabIndex = 49;
            lb颜色侧翻.Text = "颜色侧翻";
            lb颜色侧翻.Click += lb颜色面_Click;
            // 
            // sw颜色面
            // 
            sw颜色面.BackgroundImage = Properties.Resources.sw_左右小开关0;
            sw颜色面.FlatAppearance.BorderSize = 0;
            sw颜色面.FlatStyle = FlatStyle.Flat;
            sw颜色面.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            sw颜色面.ForeColor = Color.White;
            sw颜色面.Location = new Point(114, 153);
            sw颜色面.Name = "sw颜色面";
            sw颜色面.Size = new Size(80, 40);
            sw颜色面.TabIndex = 50;
            sw颜色面.TextAlign = ContentAlignment.MiddleRight;
            sw颜色面.TextImageRelation = TextImageRelation.ImageBeforeText;
            sw颜色面.UseVisualStyleBackColor = true;
            sw颜色面.Click += sw颜色面_Click;
            // 
            // lb正逆序
            // 
            lb正逆序.AutoSize = true;
            lb正逆序.BackColor = Color.Transparent;
            lb正逆序.Font = new Font("微软雅黑", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            lb正逆序.ForeColor = Color.White;
            lb正逆序.Location = new Point(111, 27);
            lb正逆序.Name = "lb正逆序";
            lb正逆序.Size = new Size(88, 25);
            lb正逆序.TabIndex = 27;
            lb正逆序.Text = "折弯顺序";
            lb正逆序.Click += lb正逆序_Click;
            // 
            // sw折弯顺序
            // 
            sw折弯顺序.BackgroundImage = Properties.Resources.sw_左右小开关0;
            sw折弯顺序.FlatAppearance.BorderSize = 0;
            sw折弯顺序.FlatStyle = FlatStyle.Flat;
            sw折弯顺序.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            sw折弯顺序.ForeColor = Color.White;
            sw折弯顺序.Location = new Point(111, 56);
            sw折弯顺序.Name = "sw折弯顺序";
            sw折弯顺序.Size = new Size(80, 40);
            sw折弯顺序.TabIndex = 28;
            sw折弯顺序.TextAlign = ContentAlignment.MiddleRight;
            sw折弯顺序.TextImageRelation = TextImageRelation.ImageBeforeText;
            sw折弯顺序.UseVisualStyleBackColor = true;
            sw折弯顺序.Click += sw正逆序_Click;
            // 
            // pnl左工具栏2
            // 
            pnl左工具栏2.BackgroundImage = Properties.Resources.Auto1_Left;
            pnl左工具栏2.Controls.Add(panel2);
            pnl左工具栏2.Controls.Add(pnl角度尺寸);
            pnl左工具栏2.Controls.Add(btnMoveUp);
            pnl左工具栏2.Controls.Add(lb颜色侧翻);
            pnl左工具栏2.Controls.Add(sw颜色面);
            pnl左工具栏2.Controls.Add(lb颜色面);
            pnl左工具栏2.Controls.Add(lb折弯方向);
            pnl左工具栏2.Controls.Add(lb正逆序);
            pnl左工具栏2.Controls.Add(btnRstView);
            pnl左工具栏2.Controls.Add(btnMoveDn);
            pnl左工具栏2.Controls.Add(sw折弯顺序);
            pnl左工具栏2.Location = new Point(2, 101);
            pnl左工具栏2.Name = "pnl左工具栏2";
            pnl左工具栏2.Size = new Size(330, 809);
            pnl左工具栏2.TabIndex = 4;
            // 
            // panel2
            // 
            panel2.BackColor = Color.Transparent;
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(label3);
            panel2.Controls.Add(label4);
            panel2.Controls.Add(label5);
            panel2.Controls.Add(label6);
            panel2.Controls.Add(label10);
            panel2.Controls.Add(button1);
            panel2.ForeColor = Color.Black;
            panel2.Location = new Point(22, 559);
            panel2.Name = "panel2";
            panel2.Size = new Size(273, 129);
            panel2.TabIndex = 73;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("宋体", 12.75F, FontStyle.Regular, GraphicsUnit.Point);
            label3.ForeColor = Color.FromArgb(208, 208, 208);
            label3.Location = new Point(35, 6);
            label3.Name = "label3";
            label3.Size = new Size(76, 17);
            label3.TabIndex = 59;
            label3.Text = "超程抓取";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.Transparent;
            label4.Font = new Font("宋体", 12.75F, FontStyle.Regular, GraphicsUnit.Point);
            label4.ForeColor = Color.FromArgb(208, 208, 208);
            label4.Location = new Point(194, 48);
            label4.Name = "label4";
            label4.Size = new Size(42, 34);
            label4.TabIndex = 59;
            label4.Text = "抓取\r\n类型";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = Color.Transparent;
            label5.Font = new Font("宋体", 12.75F, FontStyle.Regular, GraphicsUnit.Point);
            label5.ForeColor = Color.FromArgb(208, 208, 208);
            label5.Location = new Point(43, 34);
            label5.Name = "label5";
            label5.Size = new Size(42, 17);
            label5.TabIndex = 59;
            label5.Text = "抓取";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.BackColor = Color.Transparent;
            label6.Font = new Font("宋体", 12.75F, FontStyle.Regular, GraphicsUnit.Point);
            label6.ForeColor = Color.FromArgb(208, 208, 208);
            label6.Location = new Point(37, 65);
            label6.Name = "label6";
            label6.Size = new Size(42, 17);
            label6.TabIndex = 59;
            label6.Text = "自动";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.BackColor = Color.Transparent;
            label10.Font = new Font("宋体", 12.75F, FontStyle.Regular, GraphicsUnit.Point);
            label10.ForeColor = Color.FromArgb(208, 208, 208);
            label10.Location = new Point(44, 94);
            label10.Name = "label10";
            label10.Size = new Size(42, 17);
            label10.TabIndex = 59;
            label10.Text = "推动";
            // 
            // button1
            // 
            button1.BackgroundImage = Properties.Resources.btm_4档开关彩1;
            button1.BackgroundImageLayout = ImageLayout.None;
            button1.FlatAppearance.BorderColor = Color.FromArgb(67, 67, 67);
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatAppearance.MouseDownBackColor = Color.FromArgb(67, 67, 67);
            button1.FlatAppearance.MouseOverBackColor = Color.FromArgb(67, 67, 67);
            button1.FlatStyle = FlatStyle.Flat;
            button1.Location = new Point(86, 19);
            button1.Name = "button1";
            button1.Size = new Size(89, 88);
            button1.TabIndex = 37;
            button1.UseVisualStyleBackColor = true;
            // 
            // pnl角度尺寸
            // 
            pnl角度尺寸.BackColor = Color.Transparent;
            pnl角度尺寸.BorderStyle = BorderStyle.FixedSingle;
            pnl角度尺寸.Controls.Add(lb工作单_比例);
            pnl角度尺寸.Controls.Add(label2);
            pnl角度尺寸.Controls.Add(lb工作单_角度);
            pnl角度尺寸.Controls.Add(lb工作单_尺寸);
            pnl角度尺寸.Controls.Add(lb工作单_角度尺寸);
            pnl角度尺寸.Controls.Add(btn角度尺寸);
            pnl角度尺寸.ForeColor = Color.Black;
            pnl角度尺寸.Location = new Point(22, 403);
            pnl角度尺寸.Name = "pnl角度尺寸";
            pnl角度尺寸.Size = new Size(273, 129);
            pnl角度尺寸.TabIndex = 73;
            // 
            // lb工作单_比例
            // 
            lb工作单_比例.AutoSize = true;
            lb工作单_比例.BackColor = Color.Transparent;
            lb工作单_比例.Font = new Font("宋体", 12.75F, FontStyle.Regular, GraphicsUnit.Point);
            lb工作单_比例.ForeColor = Color.FromArgb(208, 208, 208);
            lb工作单_比例.Location = new Point(65, 6);
            lb工作单_比例.Name = "lb工作单_比例";
            lb工作单_比例.Size = new Size(42, 17);
            lb工作单_比例.TabIndex = 59;
            lb工作单_比例.Text = "最大";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("宋体", 12.75F, FontStyle.Regular, GraphicsUnit.Point);
            label2.ForeColor = Color.FromArgb(208, 208, 208);
            label2.Location = new Point(194, 48);
            label2.Name = "label2";
            label2.Size = new Size(76, 34);
            label2.TabIndex = 59;
            label2.Text = "松开夹钳\r\n高度";
            // 
            // lb工作单_角度
            // 
            lb工作单_角度.AutoSize = true;
            lb工作单_角度.BackColor = Color.Transparent;
            lb工作单_角度.Font = new Font("宋体", 12.75F, FontStyle.Regular, GraphicsUnit.Point);
            lb工作单_角度.ForeColor = Color.FromArgb(208, 208, 208);
            lb工作单_角度.Location = new Point(61, 34);
            lb工作单_角度.Name = "lb工作单_角度";
            lb工作单_角度.Size = new Size(25, 17);
            lb工作单_角度.TabIndex = 59;
            lb工作单_角度.Text = "高";
            // 
            // lb工作单_尺寸
            // 
            lb工作单_尺寸.AutoSize = true;
            lb工作单_尺寸.BackColor = Color.Transparent;
            lb工作单_尺寸.Font = new Font("宋体", 12.75F, FontStyle.Regular, GraphicsUnit.Point);
            lb工作单_尺寸.ForeColor = Color.FromArgb(208, 208, 208);
            lb工作单_尺寸.Location = new Point(54, 62);
            lb工作单_尺寸.Name = "lb工作单_尺寸";
            lb工作单_尺寸.Size = new Size(25, 17);
            lb工作单_尺寸.TabIndex = 59;
            lb工作单_尺寸.Text = "中";
            // 
            // lb工作单_角度尺寸
            // 
            lb工作单_角度尺寸.AutoSize = true;
            lb工作单_角度尺寸.BackColor = Color.Transparent;
            lb工作单_角度尺寸.Font = new Font("宋体", 12.75F, FontStyle.Regular, GraphicsUnit.Point);
            lb工作单_角度尺寸.ForeColor = Color.FromArgb(208, 208, 208);
            lb工作单_角度尺寸.Location = new Point(60, 91);
            lb工作单_角度尺寸.Name = "lb工作单_角度尺寸";
            lb工作单_角度尺寸.Size = new Size(25, 17);
            lb工作单_角度尺寸.TabIndex = 59;
            lb工作单_角度尺寸.Text = "低";
            // 
            // btn角度尺寸
            // 
            btn角度尺寸.BackgroundImage = Properties.Resources.btm_4档开关1;
            btn角度尺寸.BackgroundImageLayout = ImageLayout.None;
            btn角度尺寸.FlatAppearance.BorderColor = Color.FromArgb(67, 67, 67);
            btn角度尺寸.FlatAppearance.BorderSize = 0;
            btn角度尺寸.FlatAppearance.MouseDownBackColor = Color.FromArgb(67, 67, 67);
            btn角度尺寸.FlatAppearance.MouseOverBackColor = Color.FromArgb(67, 67, 67);
            btn角度尺寸.FlatStyle = FlatStyle.Flat;
            btn角度尺寸.Location = new Point(86, 20);
            btn角度尺寸.Name = "btn角度尺寸";
            btn角度尺寸.Size = new Size(89, 88);
            btn角度尺寸.TabIndex = 37;
            btn角度尺寸.UseVisualStyleBackColor = true;
            // 
            // btnMoveUp
            // 
            btnMoveUp.BackgroundImage = Properties.Resources.bg_小按钮;
            btnMoveUp.FlatAppearance.BorderSize = 0;
            btnMoveUp.FlatStyle = FlatStyle.Flat;
            btnMoveUp.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btnMoveUp.ForeColor = Color.FromArgb(208, 208, 208);
            btnMoveUp.Image = Properties.Resources.MoveUp1;
            btnMoveUp.Location = new Point(224, 290);
            btnMoveUp.Name = "btnMoveUp";
            btnMoveUp.Size = new Size(101, 59);
            btnMoveUp.TabIndex = 72;
            btnMoveUp.Text = " 移动\r\n 向上";
            btnMoveUp.TextAlign = ContentAlignment.MiddleRight;
            btnMoveUp.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnMoveUp.UseVisualStyleBackColor = true;
            // 
            // lb颜色面
            // 
            lb颜色面.AutoSize = true;
            lb颜色面.BackColor = Color.Transparent;
            lb颜色面.Font = new Font("宋体", 13F, FontStyle.Regular, GraphicsUnit.Point);
            lb颜色面.ForeColor = Color.White;
            lb颜色面.Location = new Point(200, 167);
            lb颜色面.Name = "lb颜色面";
            lb颜色面.Size = new Size(44, 18);
            lb颜色面.TabIndex = 27;
            lb颜色面.Text = "向上";
            // 
            // lb折弯方向
            // 
            lb折弯方向.AutoSize = true;
            lb折弯方向.BackColor = Color.Transparent;
            lb折弯方向.Font = new Font("AcadEref", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            lb折弯方向.ForeColor = Color.White;
            lb折弯方向.Location = new Point(197, 63);
            lb折弯方向.Name = "lb折弯方向";
            lb折弯方向.Size = new Size(27, 22);
            lb折弯方向.TabIndex = 27;
            lb折弯方向.Text = "A";
            // 
            // btnRstView
            // 
            btnRstView.BackgroundImage = Properties.Resources.bg_小按钮;
            btnRstView.FlatAppearance.BorderSize = 0;
            btnRstView.FlatStyle = FlatStyle.Flat;
            btnRstView.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btnRstView.ForeColor = Color.FromArgb(208, 208, 208);
            btnRstView.Image = Properties.Resources.ResetView;
            btnRstView.Location = new Point(2, 290);
            btnRstView.Name = "btnRstView";
            btnRstView.Size = new Size(101, 59);
            btnRstView.TabIndex = 49;
            btnRstView.Text = " 重置\r\n 订单";
            btnRstView.TextAlign = ContentAlignment.MiddleRight;
            btnRstView.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnRstView.UseVisualStyleBackColor = true;
            // 
            // btnMoveDn
            // 
            btnMoveDn.BackgroundImage = Properties.Resources.bg_小按钮;
            btnMoveDn.FlatAppearance.BorderSize = 0;
            btnMoveDn.FlatStyle = FlatStyle.Flat;
            btnMoveDn.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btnMoveDn.ForeColor = Color.FromArgb(208, 208, 208);
            btnMoveDn.Image = Properties.Resources.MoveDown1;
            btnMoveDn.Location = new Point(111, 290);
            btnMoveDn.Name = "btnMoveDn";
            btnMoveDn.Size = new Size(101, 59);
            btnMoveDn.TabIndex = 49;
            btnMoveDn.Text = " 移动\r\n 向下";
            btnMoveDn.TextAlign = ContentAlignment.MiddleRight;
            btnMoveDn.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnMoveDn.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Location = new Point(334, 101);
            panel1.Name = "panel1";
            panel1.Size = new Size(1180, 805);
            panel1.TabIndex = 5;
            // 
            // SubOPAutoSet
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.Black;
            Controls.Add(panel1);
            Controls.Add(pnl左工具栏2);
            Controls.Add(panel10);
            Name = "SubOPAutoSet";
            Size = new Size(1516, 909);
            panel10.ResumeLayout(false);
            panel10.PerformLayout();
            pnl左工具栏2.ResumeLayout(false);
            pnl左工具栏2.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            pnl角度尺寸.ResumeLayout(false);
            pnl角度尺寸.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private Panel panel10;
        private Label lb颜色侧翻;
        private Button sw颜色面;
        private Button btn装载材料;
        private TextBox textBox8;
        private TextBox txb计数;
        private Label lb正逆序;
        private Button btn重置计数;
        private Button sw分条开关;
        private Button sw折弯顺序;
        private TextBox txbSpringBtm;
        private TextBox txbSpringTop;
        private TextBox txb待分条板宽;
        private TextBox textBox1;
        private TextBox txb计算总宽;
        private Label lb分条关;
        private Label lb分条开;
        private Label label20;
        private Label label8;
        private Label label22;
        private Label label24;
        private Label label23;
        private Label label21;
        private Label label18;
        private Label label7;
        private Panel pnlAuto;
        private Panel pnl左工具栏2;
        private Button btnMoveDn;
        private Panel panel1;
        private Button btnMoveUp;
        private Button btnRstView;
        private Panel panel2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label10;
        private Button button1;
        private Panel pnl角度尺寸;
        private Label lb工作单_比例;
        private Label label2;
        private Label lb工作单_角度;
        private Label lb工作单_尺寸;
        private Label lb工作单_角度尺寸;
        private Button btn角度尺寸;
        private Label lb颜色面;
        private Label lb折弯方向;
    }
}
