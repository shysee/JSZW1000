namespace JSZW1000A
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
            setLang();
        }

        private void btn上翻板归零_MouseDown(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Name == "btn上翻板归零") { MainFrm.Hmi_bArray[90] = true; mf.AdsWritePlc(); }
            else if (btn.Name == "btn上翻板折弯") { MainFrm.Hmi_bArray[91] = true; mf.AdsWritePlc(); }
            else if (btn.Name == "btn上翻板伸出") { MainFrm.Hmi_bArray[92] = true; mf.AdsWritePlc(); }
            else if (btn.Name == "btn上翻板缩回") { MainFrm.Hmi_bArray[93] = true; mf.AdsWritePlc(); }
            else if (btn.Name == "btn下翻板归零") { MainFrm.Hmi_bArray[94] = true; mf.AdsWritePlc(); }
            else if (btn.Name == "btn下翻板折弯") { MainFrm.Hmi_bArray[95] = true; mf.AdsWritePlc(); }
            else if (btn.Name == "btn下翻板伸出") { MainFrm.Hmi_bArray[96] = true; mf.AdsWritePlc(); }
            else if (btn.Name == "btn下翻板缩回") { MainFrm.Hmi_bArray[97] = true; mf.AdsWritePlc(); }
        }

        private void btn上翻板归零_MouseUp(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Name == "btn上翻板归零") { MainFrm.Hmi_bArray[90] = false; mf.AdsWritePlc(); }
            else if (btn.Name == "btn上翻板折弯") { MainFrm.Hmi_bArray[91] = false; mf.AdsWritePlc(); }
            else if (btn.Name == "btn上翻板伸出") { MainFrm.Hmi_bArray[92] = false; mf.AdsWritePlc(); }
            else if (btn.Name == "btn上翻板缩回") { MainFrm.Hmi_bArray[93] = false; mf.AdsWritePlc(); }
            else if (btn.Name == "btn下翻板归零") { MainFrm.Hmi_bArray[94] = false; mf.AdsWritePlc(); }
            else if (btn.Name == "btn下翻板折弯") { MainFrm.Hmi_bArray[95] = false; mf.AdsWritePlc(); }
            else if (btn.Name == "btn下翻板伸出") { MainFrm.Hmi_bArray[96] = false; mf.AdsWritePlc(); }
            else if (btn.Name == "btn下翻板缩回") { MainFrm.Hmi_bArray[97] = false; mf.AdsWritePlc(); }
        }

        public void setLang()
        {
            LocalizationManager.ApplyResources(this);
            if (MainFrm.Lang == 0)
            {
                btn上翻板缩回.Font = btn上翻板伸出.Font = btn上翻板归零.Font = btn上翻板折弯.Font =
                    btn下翻板归零.Font = btn下翻板折弯.Font = btn下翻板伸出.Font = btn下翻板缩回.Font = new System.Drawing.Font("微软雅黑", 13.25F);
            }
            else
            {
                btn上翻板缩回.Font = btn上翻板伸出.Font = btn上翻板归零.Font = btn上翻板折弯.Font =
                    btn下翻板归零.Font = btn下翻板折弯.Font = btn下翻板伸出.Font = btn下翻板缩回.Font = new System.Drawing.Font("Calibri", 10F);
            }
            btn上翻板缩回.Text = Strings.Get("ForceMan.TopSlideRetract");
            btn上翻板伸出.Text = Strings.Get("ForceMan.TopSlideStretch");
            btn上翻板归零.Text = Strings.Get("ForceMan.TopFoldZero");
            btn上翻板折弯.Text = Strings.Get("ForceMan.TopFold");
            btn下翻板缩回.Text = Strings.Get("ForceMan.BottomSlideRetract");
            btn下翻板伸出.Text = Strings.Get("ForceMan.BottomSlideStretch");
            btn下翻板归零.Text = Strings.Get("ForceMan.BottomFoldZero");
            btn下翻板折弯.Text = Strings.Get("ForceMan.BottomFold");
        }

    }
}
