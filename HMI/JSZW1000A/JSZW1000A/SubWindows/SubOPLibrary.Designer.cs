namespace JSZW1000A.SubWindows
{
    partial class SubOPLibrary
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
            btn重新读取 = new Button();
            lb名称 = new Label();
            lbCurtPath = new Label();
            lb客户 = new Label();
            lb长度 = new Label();
            lb材料 = new Label();
            lb厚度 = new Label();
            lb备注 = new Label();
            btn清除 = new Button();
            flowLayoutPanel1 = new FlowLayoutPanel();
            btn选择目录 = new Button();
            panel2 = new Panel();
            btn导入DXF = new Button();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // btn重新读取
            // 
            btn重新读取.BackgroundImage = Properties.Resources.bg_中按钮1;
            btn重新读取.FlatAppearance.BorderSize = 0;
            btn重新读取.FlatStyle = FlatStyle.Flat;
            btn重新读取.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn重新读取.ForeColor = Color.FromArgb(208, 208, 208);
            btn重新读取.Image = Properties.Resources.ResetCount;
            btn重新读取.Location = new Point(1151, 21);
            btn重新读取.Name = "btn重新读取";
            btn重新读取.Size = new Size(113, 54);
            btn重新读取.TabIndex = 42;
            btn重新读取.Text = " 重新\r\n 读取";
            btn重新读取.TextAlign = ContentAlignment.MiddleRight;
            btn重新读取.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn重新读取.UseVisualStyleBackColor = true;
            btn重新读取.Click += btn重新读取_Click;
            // 
            // lb名称
            // 
            lb名称.AutoSize = true;
            lb名称.BackColor = Color.Transparent;
            lb名称.Font = new Font("Microsoft YaHei UI", 13F, FontStyle.Regular, GraphicsUnit.Point);
            lb名称.ForeColor = Color.FromArgb(208, 208, 208);
            lb名称.Location = new Point(151, 3);
            lb名称.Name = "lb名称";
            lb名称.Size = new Size(64, 24);
            lb名称.TabIndex = 43;
            lb名称.Text = "名称：";
            // 
            // lbCurtPath
            // 
            lbCurtPath.AutoSize = true;
            lbCurtPath.BackColor = Color.Transparent;
            lbCurtPath.Font = new Font("Microsoft YaHei UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            lbCurtPath.ForeColor = Color.FromArgb(208, 208, 208);
            lbCurtPath.Location = new Point(135, 45);
            lbCurtPath.Name = "lbCurtPath";
            lbCurtPath.Size = new Size(116, 30);
            lbCurtPath.TabIndex = 43;
            lbCurtPath.Text = "当前路径...";
            lbCurtPath.Click += lbCurtPath_Click;
            // 
            // lb客户
            // 
            lb客户.AutoSize = true;
            lb客户.BackColor = Color.Transparent;
            lb客户.Font = new Font("Microsoft YaHei UI", 13F, FontStyle.Regular, GraphicsUnit.Point);
            lb客户.ForeColor = Color.FromArgb(208, 208, 208);
            lb客户.Location = new Point(151, 30);
            lb客户.Name = "lb客户";
            lb客户.Size = new Size(64, 24);
            lb客户.TabIndex = 43;
            lb客户.Text = "客户：";
            // 
            // lb长度
            // 
            lb长度.AutoSize = true;
            lb长度.BackColor = Color.Transparent;
            lb长度.Font = new Font("Microsoft YaHei UI", 13F, FontStyle.Regular, GraphicsUnit.Point);
            lb长度.ForeColor = Color.FromArgb(208, 208, 208);
            lb长度.Location = new Point(7, 3);
            lb长度.Name = "lb长度";
            lb长度.Size = new Size(64, 24);
            lb长度.TabIndex = 43;
            lb长度.Text = "长度：";
            // 
            // lb材料
            // 
            lb材料.AutoSize = true;
            lb材料.BackColor = Color.Transparent;
            lb材料.Font = new Font("Microsoft YaHei UI", 13F, FontStyle.Regular, GraphicsUnit.Point);
            lb材料.ForeColor = Color.FromArgb(208, 208, 208);
            lb材料.Location = new Point(7, 57);
            lb材料.Name = "lb材料";
            lb材料.Size = new Size(64, 24);
            lb材料.TabIndex = 43;
            lb材料.Text = "材料：";
            // 
            // lb厚度
            // 
            lb厚度.AutoSize = true;
            lb厚度.BackColor = Color.Transparent;
            lb厚度.Font = new Font("Microsoft YaHei UI", 13F, FontStyle.Regular, GraphicsUnit.Point);
            lb厚度.ForeColor = Color.FromArgb(208, 208, 208);
            lb厚度.Location = new Point(7, 31);
            lb厚度.Name = "lb厚度";
            lb厚度.Size = new Size(64, 24);
            lb厚度.TabIndex = 43;
            lb厚度.Text = "厚度：";
            // 
            // lb备注
            // 
            lb备注.AutoSize = true;
            lb备注.BackColor = Color.Transparent;
            lb备注.Font = new Font("Microsoft YaHei UI", 13F, FontStyle.Regular, GraphicsUnit.Point);
            lb备注.ForeColor = Color.FromArgb(208, 208, 208);
            lb备注.Location = new Point(151, 56);
            lb备注.Name = "lb备注";
            lb备注.Size = new Size(64, 24);
            lb备注.TabIndex = 43;
            lb备注.Text = "备注：";
            // 
            // btn清除
            // 
            btn清除.BackgroundImage = Properties.Resources.bg_中按钮1;
            btn清除.FlatAppearance.BorderSize = 0;
            btn清除.FlatStyle = FlatStyle.Flat;
            btn清除.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn清除.ForeColor = Color.White;
            btn清除.Image = Properties.Resources.Remove;
            btn清除.Location = new Point(1270, 21);
            btn清除.Name = "btn清除";
            btn清除.Size = new Size(113, 54);
            btn清除.TabIndex = 44;
            btn清除.Text = " 清除";
            btn清除.TextAlign = ContentAlignment.MiddleRight;
            btn清除.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn清除.UseVisualStyleBackColor = true;
            btn清除.Click += btn清除_Click;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.BackColor = Color.Black;
            flowLayoutPanel1.Location = new Point(16, 88);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(1486, 798);
            flowLayoutPanel1.TabIndex = 46;
            // 
            // btn选择目录
            // 
            btn选择目录.BackgroundImage = Properties.Resources.bg_中按钮1;
            btn选择目录.FlatAppearance.BorderSize = 0;
            btn选择目录.FlatStyle = FlatStyle.Flat;
            btn选择目录.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn选择目录.ForeColor = Color.FromArgb(208, 208, 208);
            btn选择目录.Image = Properties.Resources.查阅文件夹0;
            btn选择目录.Location = new Point(16, 21);
            btn选择目录.Name = "btn选择目录";
            btn选择目录.Size = new Size(113, 54);
            btn选择目录.TabIndex = 42;
            btn选择目录.Text = "选择\r\n 目录";
            btn选择目录.TextAlign = ContentAlignment.MiddleRight;
            btn选择目录.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn选择目录.UseVisualStyleBackColor = true;
            btn选择目录.Click += btn选择目录_Click;
            // 
            // panel2
            // 
            panel2.BackColor = Color.Transparent;
            panel2.Controls.Add(lb客户);
            panel2.Controls.Add(lb名称);
            panel2.Controls.Add(lb长度);
            panel2.Controls.Add(lb材料);
            panel2.Controls.Add(lb厚度);
            panel2.Controls.Add(lb备注);
            panel2.Location = new Point(556, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(465, 85);
            panel2.TabIndex = 50;
            // 
            // btn导入DXF
            // 
            btn导入DXF.BackgroundImage = Properties.Resources.bg_中按钮1;
            btn导入DXF.FlatAppearance.BorderSize = 0;
            btn导入DXF.FlatStyle = FlatStyle.Flat;
            btn导入DXF.Font = new Font("宋体", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn导入DXF.ForeColor = Color.White;
            btn导入DXF.Image = Properties.Resources.Download;
            btn导入DXF.Location = new Point(1389, 21);
            btn导入DXF.Name = "btn导入DXF";
            btn导入DXF.Size = new Size(113, 54);
            btn导入DXF.TabIndex = 44;
            btn导入DXF.Text = " 导入\r\nCAD";
            btn导入DXF.TextAlign = ContentAlignment.MiddleRight;
            btn导入DXF.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn导入DXF.UseVisualStyleBackColor = true;
            btn导入DXF.Click += btn导入DXF_Click;
            // 
            // SubOPLibrary
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackgroundImage = Properties.Resources.bg_main_big;
            Controls.Add(panel2);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(btn导入DXF);
            Controls.Add(btn清除);
            Controls.Add(lbCurtPath);
            Controls.Add(btn选择目录);
            Controls.Add(btn重新读取);
            Name = "SubOPLibrary";
            Size = new Size(1516, 909);
            Load += SubOPLibrary_Load;
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btn重新读取;
        private Label lb名称;
        private Label lbCurtPath;
        private Label lb客户;
        private Label lb长度;
        private Label lb材料;
        private Label lb厚度;
        private Label lb备注;
        private Button btn清除;
        private FlowLayoutPanel flowLayoutPanel1;
        private TextBox txb总宽;
        private Button btn选择目录;
        private Label lb总宽度;
        private Panel panel2;
        private Label label1;
        private Button btn导入DXF;
    }
}
