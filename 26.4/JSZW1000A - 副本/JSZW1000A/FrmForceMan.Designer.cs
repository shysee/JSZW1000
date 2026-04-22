namespace JSZW1000A
{
    partial class FrmForceMan
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmForceMan));
            btn上翻板伸出 = new Button();
            btn上翻板缩回 = new Button();
            btn下翻板缩回 = new Button();
            btn下翻板伸出 = new Button();
            btn上翻板折弯 = new Button();
            btn上翻板归零 = new Button();
            btn下翻板折弯 = new Button();
            btn下翻板归零 = new Button();
            pictureBox1 = new PictureBox();
            button2 = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // btn上翻板伸出
            // 
            btn上翻板伸出.BackgroundImage = Properties.Resources.bg_大按钮150;
            btn上翻板伸出.BackgroundImageLayout = ImageLayout.Stretch;
            btn上翻板伸出.FlatAppearance.BorderSize = 0;
            btn上翻板伸出.FlatStyle = FlatStyle.Flat;
            btn上翻板伸出.Font = new Font("微软雅黑", 13.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn上翻板伸出.ForeColor = Color.White;
            btn上翻板伸出.Image = (Image)resources.GetObject("btn上翻板伸出.Image");
            btn上翻板伸出.Location = new Point(509, 143);
            btn上翻板伸出.Name = "btn上翻板伸出";
            btn上翻板伸出.Size = new Size(150, 110);
            btn上翻板伸出.TabIndex = 42;
            btn上翻板伸出.Text = "  上滑台   伸出";
            btn上翻板伸出.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn上翻板伸出.UseVisualStyleBackColor = true;
            btn上翻板伸出.MouseDown += btn上翻板归零_MouseDown;
            btn上翻板伸出.MouseUp += btn上翻板归零_MouseUp;
            // 
            // btn上翻板缩回
            // 
            btn上翻板缩回.BackgroundImage = Properties.Resources.bg_大按钮150;
            btn上翻板缩回.BackgroundImageLayout = ImageLayout.Stretch;
            btn上翻板缩回.FlatAppearance.BorderSize = 0;
            btn上翻板缩回.FlatStyle = FlatStyle.Flat;
            btn上翻板缩回.Font = new Font("微软雅黑", 13.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn上翻板缩回.ForeColor = Color.White;
            btn上翻板缩回.Image = (Image)resources.GetObject("btn上翻板缩回.Image");
            btn上翻板缩回.Location = new Point(393, 12);
            btn上翻板缩回.Name = "btn上翻板缩回";
            btn上翻板缩回.Size = new Size(150, 110);
            btn上翻板缩回.TabIndex = 43;
            btn上翻板缩回.Text = "  上滑台\r\n  缩回";
            btn上翻板缩回.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn上翻板缩回.UseVisualStyleBackColor = true;
            btn上翻板缩回.MouseDown += btn上翻板归零_MouseDown;
            btn上翻板缩回.MouseUp += btn上翻板归零_MouseUp;
            // 
            // btn下翻板缩回
            // 
            btn下翻板缩回.BackgroundImage = Properties.Resources.bg_大按钮150;
            btn下翻板缩回.BackgroundImageLayout = ImageLayout.Stretch;
            btn下翻板缩回.FlatAppearance.BorderSize = 0;
            btn下翻板缩回.FlatStyle = FlatStyle.Flat;
            btn下翻板缩回.Font = new Font("微软雅黑", 13.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn下翻板缩回.ForeColor = Color.White;
            btn下翻板缩回.Image = Properties.Resources.BotApronSlide_Dn1;
            btn下翻板缩回.Location = new Point(393, 438);
            btn下翻板缩回.Name = "btn下翻板缩回";
            btn下翻板缩回.Size = new Size(150, 110);
            btn下翻板缩回.TabIndex = 45;
            btn下翻板缩回.Text = "  下滑台\r\n  缩回";
            btn下翻板缩回.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn下翻板缩回.UseVisualStyleBackColor = true;
            btn下翻板缩回.MouseDown += btn上翻板归零_MouseDown;
            btn下翻板缩回.MouseUp += btn上翻板归零_MouseUp;
            // 
            // btn下翻板伸出
            // 
            btn下翻板伸出.BackgroundImage = (Image)resources.GetObject("btn下翻板伸出.BackgroundImage");
            btn下翻板伸出.BackgroundImageLayout = ImageLayout.Stretch;
            btn下翻板伸出.FlatAppearance.BorderSize = 0;
            btn下翻板伸出.FlatStyle = FlatStyle.Flat;
            btn下翻板伸出.Font = new Font("微软雅黑", 13.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn下翻板伸出.ForeColor = Color.White;
            btn下翻板伸出.Image = Properties.Resources.BotApronSlide_Up1;
            btn下翻板伸出.Location = new Point(509, 303);
            btn下翻板伸出.Name = "btn下翻板伸出";
            btn下翻板伸出.Size = new Size(150, 110);
            btn下翻板伸出.TabIndex = 44;
            btn下翻板伸出.Text = "  下滑台   伸出";
            btn下翻板伸出.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn下翻板伸出.UseVisualStyleBackColor = true;
            btn下翻板伸出.MouseDown += btn上翻板归零_MouseDown;
            btn下翻板伸出.MouseUp += btn上翻板归零_MouseUp;
            // 
            // btn上翻板折弯
            // 
            btn上翻板折弯.BackgroundImage = Properties.Resources.bg_大按钮150;
            btn上翻板折弯.BackgroundImageLayout = ImageLayout.Stretch;
            btn上翻板折弯.FlatAppearance.BorderSize = 0;
            btn上翻板折弯.FlatStyle = FlatStyle.Flat;
            btn上翻板折弯.Font = new Font("微软雅黑", 13.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn上翻板折弯.ForeColor = Color.White;
            btn上翻板折弯.Image = Properties.Resources.TopApronFold_Fwd;
            btn上翻板折弯.Location = new Point(786, 143);
            btn上翻板折弯.Name = "btn上翻板折弯";
            btn上翻板折弯.Size = new Size(150, 110);
            btn上翻板折弯.TabIndex = 46;
            btn上翻板折弯.Text = "  上翻板\r\n  折弯";
            btn上翻板折弯.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn上翻板折弯.UseVisualStyleBackColor = true;
            btn上翻板折弯.MouseDown += btn上翻板归零_MouseDown;
            btn上翻板折弯.MouseUp += btn上翻板归零_MouseUp;
            // 
            // btn上翻板归零
            // 
            btn上翻板归零.BackgroundImage = Properties.Resources.bg_大按钮150;
            btn上翻板归零.BackgroundImageLayout = ImageLayout.Stretch;
            btn上翻板归零.FlatAppearance.BorderSize = 0;
            btn上翻板归零.FlatStyle = FlatStyle.Flat;
            btn上翻板归零.Font = new Font("微软雅黑", 13.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn上翻板归零.ForeColor = Color.White;
            btn上翻板归零.Image = Properties.Resources.TopApronFold_Bwd;
            btn上翻板归零.Location = new Point(716, 12);
            btn上翻板归零.Name = "btn上翻板归零";
            btn上翻板归零.Size = new Size(150, 110);
            btn上翻板归零.TabIndex = 46;
            btn上翻板归零.Text = "  上翻板\r\n  归零";
            btn上翻板归零.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn上翻板归零.UseVisualStyleBackColor = true;
            btn上翻板归零.MouseDown += btn上翻板归零_MouseDown;
            btn上翻板归零.MouseUp += btn上翻板归零_MouseUp;
            // 
            // btn下翻板折弯
            // 
            btn下翻板折弯.BackgroundImage = Properties.Resources.bg_大按钮150;
            btn下翻板折弯.BackgroundImageLayout = ImageLayout.Stretch;
            btn下翻板折弯.FlatAppearance.BorderSize = 0;
            btn下翻板折弯.FlatStyle = FlatStyle.Flat;
            btn下翻板折弯.Font = new Font("微软雅黑", 13.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn下翻板折弯.ForeColor = Color.White;
            btn下翻板折弯.Image = Properties.Resources.BtmApronFold_Up;
            btn下翻板折弯.Location = new Point(786, 303);
            btn下翻板折弯.Name = "btn下翻板折弯";
            btn下翻板折弯.Size = new Size(150, 110);
            btn下翻板折弯.TabIndex = 46;
            btn下翻板折弯.Text = "  下翻板\r\n  折弯";
            btn下翻板折弯.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn下翻板折弯.UseVisualStyleBackColor = true;
            btn下翻板折弯.MouseDown += btn上翻板归零_MouseDown;
            btn下翻板折弯.MouseUp += btn上翻板归零_MouseUp;
            // 
            // btn下翻板归零
            // 
            btn下翻板归零.BackgroundImage = Properties.Resources.bg_大按钮150;
            btn下翻板归零.BackgroundImageLayout = ImageLayout.Stretch;
            btn下翻板归零.FlatAppearance.BorderSize = 0;
            btn下翻板归零.FlatStyle = FlatStyle.Flat;
            btn下翻板归零.Font = new Font("微软雅黑", 13.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn下翻板归零.ForeColor = Color.White;
            btn下翻板归零.Image = Properties.Resources.ApronDown1;
            btn下翻板归零.Location = new Point(716, 438);
            btn下翻板归零.Name = "btn下翻板归零";
            btn下翻板归零.Size = new Size(150, 110);
            btn下翻板归零.TabIndex = 46;
            btn下翻板归零.Text = "  下翻板\r\n  归零";
            btn下翻板归零.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn下翻板归零.UseVisualStyleBackColor = true;
            btn下翻板归零.Click += button7_Click;
            btn下翻板归零.MouseDown += btn上翻板归零_MouseDown;
            btn下翻板归零.MouseUp += btn上翻板归零_MouseUp;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Image = Properties.Resources.FolderGeometry1;
            pictureBox1.Location = new Point(3, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(511, 552);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 48;
            pictureBox1.TabStop = false;
            // 
            // button2
            // 
            button2.BackColor = Color.Transparent;
            button2.BackgroundImageLayout = ImageLayout.Stretch;
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("微软雅黑", 13.25F, FontStyle.Regular, GraphicsUnit.Point);
            button2.ForeColor = Color.White;
            button2.Image = Properties.Resources.cancel40;
            button2.Location = new Point(898, 2);
            button2.Name = "button2";
            button2.Size = new Size(60, 64);
            button2.TabIndex = 49;
            button2.TextImageRelation = TextImageRelation.TextBeforeImage;
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // FrmForceMan
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(960, 576);
            Controls.Add(button2);
            Controls.Add(btn下翻板归零);
            Controls.Add(btn下翻板折弯);
            Controls.Add(btn上翻板归零);
            Controls.Add(btn上翻板折弯);
            Controls.Add(btn下翻板缩回);
            Controls.Add(btn下翻板伸出);
            Controls.Add(btn上翻板缩回);
            Controls.Add(btn上翻板伸出);
            Controls.Add(pictureBox1);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            Name = "FrmForceMan";
            Text = "手动强制";
            Load += FrmForceMan_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button btn上翻板伸出;
        private Button btn上翻板缩回;
        private Button btn下翻板缩回;
        private Button btn下翻板伸出;
        private Button btn上翻板折弯;
        private Button btn上翻板归零;
        private Button btn下翻板折弯;
        private Button btn下翻板归零;
        private PictureBox pictureBox1;
        private Button button2;
    }
}