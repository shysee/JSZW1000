namespace JSZW1000A.SubWindows
{
    public partial class SubCheckItem : UserControl
    {
        public MainFrm mf;
        private int Lang = 0;

        public SubCheckItem(MainFrm mf1)
        {
            this.mf = mf1;
            InitializeComponent();
            setLang();
        }

        private void setLang()
        {
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

            label5.Text = (MainFrm.Lang == 0) ? "在操作此双边折弯机之前，您必须同意" : "Before Operating this Bi-directional Folder, You Must Agree that";
            label3.Text = (MainFrm.Lang == 0) ? "以下所有检查均已完成：" : "All the following checks have been completed:";
            label9.Text = (MainFrm.Lang == 0) ? "这些检查必须在操作开始前完成，并被接受。" : "These checks must be completed and agreed before operation can begin.";
            label1.Text = (MainFrm.Lang == 0) ? "1.激光器的安装位置按照说明书设置；" : "1.The Laser Position should be set according to the instructions;";
            label2.Text = (MainFrm.Lang == 0) ? "2.挡指高度位置设置正确，约6-8CM；" : "2.The Height Position of the Mute Signal is set correctly, about 6-8CM;";
            label4.Text = (MainFrm.Lang == 0) ? "3.半夹钳位置设置为25.0mm附近；" : "3.Set the Half Clamping Position to around 25.0mm;";
            label6.Text = (MainFrm.Lang == 0) ? "4.紧急拉绳未被触发，且急停按钮已弹出；" : "4.Kick Pole isn't obstructed and Emergency-Stop button Popped up;";
            label7.Text = (MainFrm.Lang == 0) ? "5.被折弯材料厚度：" : "5.Thickness of the bent material:";
            label10.Text = (MainFrm.Lang == 0) ? "钢板≤1.6mm,铝板≤2.0mm,不锈钢板≤1.2mm；" : "Steel ≤ 1.6mm, Aluminum ≤ 2.0mm, Stainless Steel ≤ 1.2mm;";
            label8.Text = (MainFrm.Lang == 0) ? "6.已经阅读操作手册，并受过培训；" : "6. Have read the operation manual and received training;";



        }
        private void radioBtn_CN_Click(object sender, EventArgs e)
        {
            MainFrm.Lang = 0;
            setLang();
            mf.setLang();
        }
        private void radioBtn_EN_Click(object sender, EventArgs e)
        {
            MainFrm.Lang = 1;
            setLang();
            mf.setLang();
        }
        private void SubCheckItem_Load(object sender, EventArgs e)
        {
            if (MainFrm.Lang == 0)
            {
                radioBtn_CN.Checked = true;
            }
            else
                radioBtn_CN.Checked = false;
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

            lb接受1.Text = MainFrm.Lang == 0 ? (b接受1 ? "是" : "否") : (b接受1 ? "Y" : "N");
            lb接受2.Text = MainFrm.Lang == 0 ? (b接受2 ? "是" : "否") : (b接受2 ? "Y" : "N");
            lb接受3.Text = MainFrm.Lang == 0 ? (b接受3 ? "是" : "否") : (b接受3 ? "Y" : "N");
            lb接受4.Text = MainFrm.Lang == 0 ? (b接受4 ? "是" : "否") : (b接受4 ? "Y" : "N");
            lb接受5.Text = MainFrm.Lang == 0 ? (b接受5 ? "是" : "否") : (b接受5 ? "Y" : "N");
            lb接受6.Text = MainFrm.Lang == 0 ? (b接受6 ? "是" : "否") : (b接受6 ? "Y" : "N");

            lbPLC连接.Text = (MainFrm.AdsConn) ? (MainFrm.Lang == 0 ? "PLC连接成功！" : "PLC Connection Successful.") : (MainFrm.Lang == 0 ? "PLC连接失败！" : "PLC Connection Failed.");
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
            lb接受5.Text = MainFrm.Lang == 0 ? (b接受5 ? "是" : "否") : (b接受5 ? "Y" : "N");
            lb接受5.ForeColor = b接受5 ? Color.FromArgb(96, 176, 255) : Color.White;
        }



        private void sw接受6_Click(object sender, EventArgs e)
        {
            b接受6 = !b接受6;
            sw接受6.BackgroundImage = b接受6 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            lb接受6.Text = MainFrm.Lang == 0 ? (b接受6 ? "是" : "否") : (b接受6 ? "Y" : "N");
            lb接受6.ForeColor = b接受6 ? Color.FromArgb(96, 176, 255) : Color.White;
        }

        private void sw接受4_Click(object sender, EventArgs e)
        {
            b接受4 = !b接受4;
            sw接受4.BackgroundImage = b接受4 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            lb接受4.Text = MainFrm.Lang == 0 ? (b接受4 ? "是" : "否") : (b接受4 ? "Y" : "N");
            lb接受4.ForeColor = b接受4 ? Color.FromArgb(96, 176, 255) : Color.White;
        }

        private void sw接受3_Click(object sender, EventArgs e)
        {
            b接受3 = !b接受3;
            sw接受3.BackgroundImage = b接受3 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            lb接受3.Text = MainFrm.Lang == 0 ? (b接受3 ? "是" : "否") : (b接受3 ? "Y" : "N");
            lb接受3.ForeColor = b接受3 ? Color.FromArgb(96, 176, 255) : Color.White;
        }

        private void sw接受2_Click(object sender, EventArgs e)
        {
            b接受2 = !b接受2;
            sw接受2.BackgroundImage = b接受2 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            lb接受2.Text = MainFrm.Lang == 0 ? (b接受2 ? "是" : "否") : (b接受2 ? "Y" : "N");
            lb接受2.ForeColor = b接受2 ? Color.FromArgb(96, 176, 255) : Color.White;
        }

        private void sw接受1_Click(object sender, EventArgs e)
        {
            b接受1 = !b接受1;
            sw接受1.BackgroundImage = b接受1 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            lb接受1.Text = MainFrm.Lang == 0 ? (b接受1 ? "是" : "否") : (b接受1 ? "Y" : "N");
            lb接受1.ForeColor = b接受1 ? Color.FromArgb(96, 176, 255) : Color.White;
        }




    }
}
