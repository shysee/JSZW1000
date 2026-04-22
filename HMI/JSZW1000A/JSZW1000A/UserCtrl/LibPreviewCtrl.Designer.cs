namespace JSZW1000A.UserCtrl
{
    partial class LibPreviewCtrl
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(188, 188);
            this.panel1.TabIndex = 0;
            this.panel1.Click += new System.EventHandler(this.panel1_Click);
            // 
            // lbName
            // 
            this.lbName.Font = new System.Drawing.Font("Microsoft YaHei UI", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lbName.Location = new System.Drawing.Point(3, 191);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(182, 29);
            this.lbName.TabIndex = 1;
            this.lbName.Text = "屋面檐口";
            // 
            // LibPreviewCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbName);
            this.Controls.Add(this.panel1);
            this.Name = "LibPreviewCtrl";
            this.Size = new System.Drawing.Size(188, 220);
            this.Load += new System.EventHandler(this.LibPreviewCtrl_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.LibPreviewCtrl_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private Label lbName;
    }
}
