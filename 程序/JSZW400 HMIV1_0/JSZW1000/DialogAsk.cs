using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace JSZW400
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
            btn确定.Text = (MainFrm.Lang == 0) ? " 确定" : "OK";
            btn取消.Text = (MainFrm.Lang == 0) ? " 取消" : "Cancle";
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
