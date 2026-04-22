namespace JSZW1000A
{
    partial class FrmSaveAs
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn保存 = new System.Windows.Forms.Button();
            this.btn取消 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft YaHei UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBox1.Location = new System.Drawing.Point(150, 113);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(150, 26);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "wang0829";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(79, 113);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "文件名";
            // 
            // btn保存
            // 
            this.btn保存.Location = new System.Drawing.Point(69, 260);
            this.btn保存.Name = "btn保存";
            this.btn保存.Size = new System.Drawing.Size(84, 42);
            this.btn保存.TabIndex = 2;
            this.btn保存.Text = "保存";
            this.btn保存.UseVisualStyleBackColor = true;
            this.btn保存.Click += new System.EventHandler(this.btn保存_Click);
            // 
            // btn取消
            // 
            this.btn取消.Location = new System.Drawing.Point(208, 260);
            this.btn取消.Name = "btn取消";
            this.btn取消.Size = new System.Drawing.Size(84, 42);
            this.btn取消.TabIndex = 2;
            this.btn取消.Text = "取消";
            this.btn取消.UseVisualStyleBackColor = true;
            this.btn取消.Click += new System.EventHandler(this.btn取消_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(124, 189);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "该文件名已存在！";
            this.label2.Visible = false;
            // 
            // FrmSaveAs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 401);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btn取消);
            this.Controls.Add(this.btn保存);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FrmSaveAs";
            this.Text = "FrmSaveAs";
            this.Load += new System.EventHandler(this.FrmSaveAs_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox textBox1;
        private Label label1;
        private Button btn保存;
        private Button btn取消;
        private Label label2;
    }
}