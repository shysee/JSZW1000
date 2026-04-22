namespace JSZW400
{
    partial class DialogAsk
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogAsk));
            pictureBox1 = new PictureBox();
            label2 = new Label();
            label1 = new Label();
            btn确定 = new Button();
            btn取消 = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox1.Location = new Point(33, 41);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(95, 96);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 38;
            pictureBox1.TabStop = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Microsoft YaHei UI", 13F, FontStyle.Regular, GraphicsUnit.Point);
            label2.ForeColor = Color.WhiteSmoke;
            label2.Location = new Point(129, 9);
            label2.Name = "label2";
            label2.Size = new Size(172, 24);
            label2.TabIndex = 37;
            label2.Text = "详细操作提示。。。";
            label2.Visible = false;
            // 
            // label1
            // 
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("微软雅黑", 27.75F, FontStyle.Regular, GraphicsUnit.Point);
            label1.ForeColor = Color.WhiteSmoke;
            label1.Location = new Point(147, 62);
            label1.Name = "label1";
            label1.Size = new Size(508, 52);
            label1.TabIndex = 36;
            label1.Text = "询问内容";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btn确定
            // 
            btn确定.BackColor = SystemColors.Control;
            btn确定.BackgroundImage = Properties.Resources.bg_中按钮;
            btn确定.FlatAppearance.BorderSize = 0;
            btn确定.FlatStyle = FlatStyle.Flat;
            btn确定.Font = new Font("Calibri", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn确定.ForeColor = Color.White;
            btn确定.Image = Properties.Resources.tick1;
            btn确定.Location = new Point(209, 218);
            btn确定.Name = "btn确定";
            btn确定.Size = new Size(113, 54);
            btn确定.TabIndex = 54;
            btn确定.Text = "  确定";
            btn确定.TextAlign = ContentAlignment.MiddleRight;
            btn确定.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn确定.UseVisualStyleBackColor = false;
            btn确定.Click += btn确定_Click;
            // 
            // btn取消
            // 
            btn取消.BackgroundImage = Properties.Resources.bg_中按钮;
            btn取消.FlatAppearance.BorderSize = 0;
            btn取消.FlatStyle = FlatStyle.Flat;
            btn取消.Font = new Font("宋体", 10F, FontStyle.Regular, GraphicsUnit.Point);
            btn取消.ForeColor = Color.White;
            btn取消.Image = Properties.Resources.cancel40;
            btn取消.Location = new Point(489, 218);
            btn取消.Name = "btn取消";
            btn取消.Size = new Size(113, 54);
            btn取消.TabIndex = 55;
            btn取消.Text = "  取消";
            btn取消.TextAlign = ContentAlignment.MiddleRight;
            btn取消.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn取消.UseVisualStyleBackColor = true;
            btn取消.Click += btn取消_Click;
            // 
            // DialogAsk
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Zoom;
            ClientSize = new Size(741, 301);
            Controls.Add(btn取消);
            Controls.Add(btn确定);
            Controls.Add(pictureBox1);
            Controls.Add(label2);
            Controls.Add(label1);
            DoubleBuffered = true;
            ForeColor = SystemColors.ControlText;
            FormBorderStyle = FormBorderStyle.None;
            Name = "DialogAsk";
            Text = "DialogAsk";
            TransparencyKey = Color.Transparent;
            Load += DialogAsk_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Label label2;
        private Label label1;
        private Button btn确定;
        private Button btn取消;
    }
}