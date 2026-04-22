using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JSZW400
{
    public partial class FrmForceMan : Form
    {
        MainFrm mf;
        public FrmForceMan(MainFrm mf1)
        {
            InitializeComponent();
            this.mf = mf1;
        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmForceMan_Load(object sender, EventArgs e)
        {

        }

        private void btn上翻板归零_MouseDown(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Name == "btn上翻板归零") { MainFrm.Hmi_bArray[90] = true; mf.AdsWritePlc(); }
            else if (btn.Name == "btn上翻板折弯") { MainFrm.Hmi_bArray[91] = true; mf.AdsWritePlc(); }
            else if (btn.Name == "btn上滑台伸出") { MainFrm.Hmi_bArray[92] = true; mf.AdsWritePlc(); }
            else if (btn.Name == "btn上滑台缩回") { MainFrm.Hmi_bArray[93] = true; mf.AdsWritePlc(); }
            else if (btn.Name == "btn下翻板归零") { MainFrm.Hmi_bArray[94] = true; mf.AdsWritePlc(); }
            else if (btn.Name == "btn下翻板折弯") { MainFrm.Hmi_bArray[95] = true; mf.AdsWritePlc(); }            
            else if (btn.Name == "btn下滑台伸出") { MainFrm.Hmi_bArray[96] = true; mf.AdsWritePlc(); }
            else if (btn.Name == "btn下滑台缩回") { MainFrm.Hmi_bArray[97] = true; mf.AdsWritePlc(); }
        }

        private void btn上翻板归零_MouseUp(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Name == "btn上翻板归零") { MainFrm.Hmi_bArray[90] = false; mf.AdsWritePlc(); }
            else if (btn.Name == "btn上翻板折弯") { MainFrm.Hmi_bArray[91] = false; mf.AdsWritePlc(); }
            else if (btn.Name == "btn上滑台伸出") { MainFrm.Hmi_bArray[92] = false; mf.AdsWritePlc(); }
            else if (btn.Name == "btn上滑台缩回") { MainFrm.Hmi_bArray[93] = false; mf.AdsWritePlc(); }
            else if (btn.Name == "btn下翻板归零") { MainFrm.Hmi_bArray[94] = false; mf.AdsWritePlc(); }
            else if (btn.Name == "btn下翻板折弯") { MainFrm.Hmi_bArray[95] = false; mf.AdsWritePlc(); }            
            else if (btn.Name == "btn下滑台伸出") { MainFrm.Hmi_bArray[96] = false; mf.AdsWritePlc(); }
            else if (btn.Name == "btn下滑台缩回") { MainFrm.Hmi_bArray[97] = false; mf.AdsWritePlc(); }
        }
    }
}
