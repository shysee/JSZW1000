namespace JSZW1000A
{
    public partial class DialogAsk : Form
    {
        string msgContent = "";
        string msgTitle = "";
        public DialogAsk(string msgTitle, string msgContent)
        {
            InitializeComponent();
            this.msgContent = msgContent;
            this.msgTitle = msgTitle;
        }
        private void setLang()
        {
            LocalizationManager.ApplyResources(this);
            if (MainFrm.Lang == 0)
            {
                btn确定.Font = btn取消.Font = new System.Drawing.Font("宋体", 10F);
                label1.Font = new System.Drawing.Font("微软雅黑", 27.75F);
                label2.Font = new System.Drawing.Font("微软雅黑", 13F);
            }
            else
            {
                btn确定.Font = btn取消.Font = new System.Drawing.Font("Calibri", 11.25F);
                label1.Font = new System.Drawing.Font("Calibri", 15F);
                label2.Font = new System.Drawing.Font("Calibri", 10F);
            }
            btn确定.Text = Strings.Get("Dialog.Confirm");
            btn取消.Text = Strings.Get("Dialog.Cancel");
        }
        private void btn确定_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
        private void btn取消_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
        private void DialogAsk_Load(object sender, EventArgs e)
        {
            setLang();
            this.label1.Text = this.msgContent;
            this.label2.Text = this.msgTitle;
        }
    }
}
