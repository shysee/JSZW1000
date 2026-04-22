using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace JSZW400
{
    public partial class FrmSaveAs : Form
    {
        public string filename="";
        public FrmSaveAs()
        {
            InitializeComponent();
        }

        private void btn保存_Click(object sender, EventArgs e)
        {
            this.filename = textBox1.Text;
            this.DialogResult = DialogResult.OK;
            
        }

        private void btn取消_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void FrmSaveAs_Load(object sender, EventArgs e)
        {
            
            System.DateTime ctm = new System.DateTime();
            ctm = System.DateTime.Now; // 取当前年月日时分秒
            string s = ctm.Year.ToString() + String.Format("{0:D2}", ctm.Month)
                + String.Format("{0:D2}", ctm.Day) +"_"+ String.Format("{0:D2}", ctm.Hour) +
                String.Format("{0:D2}", ctm.Minute);
            string s1=s.Substring(2, s.Length - 2);
            string s2 = "Fold_" + s1;
            textBox1.Text = s2;
        }
    }
}
