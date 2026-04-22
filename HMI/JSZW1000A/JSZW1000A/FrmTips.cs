namespace JSZW1000A
{
    public partial class FrmTips : Form
    {
        int delay = 0;
        MainFrm mf;

        public FrmTips(MainFrm fm1, string s)
        {
            InitializeComponent();
            this.mf = fm1;
            if (s.Length > 8)
                this.label1.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            else
                this.label1.Font = new System.Drawing.Font("微软雅黑", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Text = s;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            delay++;
            if (delay > 3 || !mf.bTipFlag)
            {
                delay = 0;
                this.Dispose();
                mf.bTipFlag = false;
            }
            label2.Text = (3 - delay).ToString();
        }

        private void FrmTips_Load(object sender, EventArgs e)
        {
            delay = 0;
        }
    }
}
