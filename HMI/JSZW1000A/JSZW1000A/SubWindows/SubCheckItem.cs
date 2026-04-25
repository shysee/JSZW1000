namespace JSZW1000A.SubWindows
{
    public partial class SubCheckItem : UserControl
    {
        public MainFrm mf;

        public SubCheckItem(MainFrm mf1)
        {
            this.mf = mf1;
            InitializeComponent();
            setLang();
        }

        private void setLang()
        {
            LocalizationManager.ApplyResources(this);
            HideLegacyLanguageSelector();
            if (MainFrm.Lang == 0)
            {
                radioBtn_CN.Checked = true;
                this.label5.Font = label3.Font = label9.Font = new System.Drawing.Font("微软雅黑", 18F);
                this.label1.Font = label2.Font = label4.Font = label6.Font = label7.Font = label10.Font = label8.Font = new System.Drawing.Font("微软雅黑", 18F);

            }
            else
            {
                radioBtn_EN.Checked = true;
                this.label5.Font = label3.Font = label9.Font = new System.Drawing.Font("Calibri", 18F);
                this.label1.Font = label2.Font = label4.Font = label6.Font = label7.Font = label10.Font = label8.Font = new System.Drawing.Font("Calibri", 18F);
            }
            label5.Text = Strings.Get("CheckItem.Title");
            label3.Text = Strings.Get("CheckItem.Subtitle");
            label9.Text = Strings.Get("CheckItem.Note");
            label1.Text = Strings.Get("CheckItem.Item1");
            label2.Text = string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                Strings.Get("CheckItem.Item2"),
                MainFrm.FormatDisplayLengthWithUnit(60, 2),
                MainFrm.FormatDisplayLengthWithUnit(80, 2));
            label4.Text = string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                Strings.Get("CheckItem.Item3"),
                MainFrm.FormatDisplayLengthWithUnit(25, 3));
            label6.Text = Strings.Get("CheckItem.Item4");
            label7.Text = Strings.Get("CheckItem.Item5");
            label10.Text = string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                Strings.Get("CheckItem.Item5Detail"),
                MainFrm.FormatDisplayLengthWithUnit(1.6, 3),
                MainFrm.FormatDisplayLengthWithUnit(2.0, 3),
                MainFrm.FormatDisplayLengthWithUnit(1.2, 3));
            label8.Text = Strings.Get("CheckItem.Item6");
        }
        private void radioBtn_CN_Click(object sender, EventArgs e)
        {
            if (!radioBtn_CN.Visible)
                return;

            LocalizationManager.UseSessionLanguage(AppLanguage.ZhCn);
            setLang();
            mf.setLang();
        }
        private void radioBtn_EN_Click(object sender, EventArgs e)
        {
            if (!radioBtn_EN.Visible)
                return;

            LocalizationManager.UseSessionLanguage(AppLanguage.EnUs);
            setLang();
            mf.setLang();
        }
        private void SubCheckItem_Load(object sender, EventArgs e)
        {
            radioBtn_CN.Checked = LocalizationManager.CurrentLanguage == AppLanguage.ZhCn;
            HideLegacyLanguageSelector();
        }

        private void HideLegacyLanguageSelector()
        {
            radioBtn_CN.Visible = false;
            radioBtn_EN.Visible = false;
            radioBtn_CN.Enabled = false;
            radioBtn_EN.Enabled = false;
        }

        int iLogo = 0;
        bool bAdd = true;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (bAdd)
            {
                iLogo++;
                if (iLogo == 10) { bAdd = false; }
            }
            else
            {
                iLogo--;
                if (iLogo == 0)
                {
                    bAdd = true;

                }

            }

            switch (iLogo)
            {
                case 0:
                    pnlLOGO.BackgroundImage = global::JSZW1000A.Properties.Resources.JG_LOGO_0;
                    break;
                case 1:
                    pnlLOGO.BackgroundImage = global::JSZW1000A.Properties.Resources.JG_LOGO_10;
                    break;
                case 2:
                    pnlLOGO.BackgroundImage = global::JSZW1000A.Properties.Resources.JG_LOGO_20;
                    break;
                case 3:
                    pnlLOGO.BackgroundImage = global::JSZW1000A.Properties.Resources.JG_LOGO_30;
                    break;
                case 4:
                    pnlLOGO.BackgroundImage = global::JSZW1000A.Properties.Resources.JG_LOGO_40;
                    break;
                case 5:
                    pnlLOGO.BackgroundImage = global::JSZW1000A.Properties.Resources.JG_LOGO_50;
                    break;
                case 6:
                    pnlLOGO.BackgroundImage = global::JSZW1000A.Properties.Resources.JG_LOGO_60;
                    break;
                case 7:
                    pnlLOGO.BackgroundImage = global::JSZW1000A.Properties.Resources.JG_LOGO_70;
                    break;
                case 8:
                    pnlLOGO.BackgroundImage = global::JSZW1000A.Properties.Resources.JG_LOGO_80;
                    break;
                case 9:
                    pnlLOGO.BackgroundImage = global::JSZW1000A.Properties.Resources.JG_LOGO_90;
                    break;
                case 10:
                    pnlLOGO.BackgroundImage = global::JSZW1000A.Properties.Resources.JG_LOGO_100;
                    break;
                default:
                    pnlLOGO.BackgroundImage = global::JSZW1000A.Properties.Resources.JG_LOGO_100;
                    break;
            }


            sw接受1.BackgroundImage = b接受1 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            sw接受2.BackgroundImage = b接受2 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            sw接受3.BackgroundImage = b接受3 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            sw接受4.BackgroundImage = b接受4 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            sw接受5.BackgroundImage = b接受5 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            sw接受6.BackgroundImage = b接受6 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;

            lb接受1.Text = LocalizationText.YesNo(b接受1);
            lb接受2.Text = LocalizationText.YesNo(b接受2);
            lb接受3.Text = LocalizationText.YesNo(b接受3);
            lb接受4.Text = LocalizationText.YesNo(b接受4);
            lb接受5.Text = LocalizationText.YesNo(b接受5);
            lb接受6.Text = LocalizationText.YesNo(b接受6);

            lbPLC连接.Text = LocalizationText.PlcConnection(MainFrm.AdsConn);
            lbPLC连接.ForeColor = (MainFrm.AdsConn) ? System.Drawing.Color.Green : System.Drawing.Color.Red;
            pnlPLC连接.BackgroundImage = (MainFrm.AdsConn) ? global::JSZW1000A.Properties.Resources.Warning1 : global::JSZW1000A.Properties.Resources.Warning0;

            lb接受1.ForeColor = b接受1 ? Color.FromArgb(96, 176, 255) : Color.White;
            lb接受2.ForeColor = b接受2 ? Color.FromArgb(96, 176, 255) : Color.White;
            lb接受3.ForeColor = b接受3 ? Color.FromArgb(96, 176, 255) : Color.White;
            lb接受4.ForeColor = b接受4 ? Color.FromArgb(96, 176, 255) : Color.White;
            lb接受5.ForeColor = b接受5 ? Color.FromArgb(96, 176, 255) : Color.White;
            lb接受6.ForeColor = b接受6 ? Color.FromArgb(96, 176, 255) : Color.White;
            if (b接受1 && b接受2 && b接受3 && b接受4 && b接受5 && b接受6)
                mf.b条款确认 = true;
            else
                mf.b条款确认 = false;

        }

        bool b接受1 = false, b接受2 = false, b接受3 = false, b接受4 = false, b接受5 = false, b接受6 = false;

        private void sw接受5_Click(object sender, EventArgs e)
        {
            b接受5 = !b接受5;
            sw接受5.BackgroundImage = b接受5 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            lb接受5.Text = LocalizationText.YesNo(b接受5);
            lb接受5.ForeColor = b接受5 ? Color.FromArgb(96, 176, 255) : Color.White;
        }



        private void sw接受6_Click(object sender, EventArgs e)
        {
            b接受6 = !b接受6;
            sw接受6.BackgroundImage = b接受6 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            lb接受6.Text = LocalizationText.YesNo(b接受6);
            lb接受6.ForeColor = b接受6 ? Color.FromArgb(96, 176, 255) : Color.White;
        }

        private void sw接受4_Click(object sender, EventArgs e)
        {
            b接受4 = !b接受4;
            sw接受4.BackgroundImage = b接受4 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            lb接受4.Text = LocalizationText.YesNo(b接受4);
            lb接受4.ForeColor = b接受4 ? Color.FromArgb(96, 176, 255) : Color.White;
        }

        private void sw接受3_Click(object sender, EventArgs e)
        {
            b接受3 = !b接受3;
            sw接受3.BackgroundImage = b接受3 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            lb接受3.Text = LocalizationText.YesNo(b接受3);
            lb接受3.ForeColor = b接受3 ? Color.FromArgb(96, 176, 255) : Color.White;
        }

        private void sw接受2_Click(object sender, EventArgs e)
        {
            b接受2 = !b接受2;
            sw接受2.BackgroundImage = b接受2 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            lb接受2.Text = LocalizationText.YesNo(b接受2);
            lb接受2.ForeColor = b接受2 ? Color.FromArgb(96, 176, 255) : Color.White;
        }

        private void sw接受1_Click(object sender, EventArgs e)
        {
            b接受1 = !b接受1;
            sw接受1.BackgroundImage = b接受1 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            lb接受1.Text = LocalizationText.YesNo(b接受1);
            lb接受1.ForeColor = b接受1 ? Color.FromArgb(96, 176, 255) : Color.White;
        }




    }
}
