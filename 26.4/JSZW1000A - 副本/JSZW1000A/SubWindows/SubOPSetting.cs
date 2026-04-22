using Excel = Microsoft.Office.Interop.Excel;
using TextBox = System.Windows.Forms.TextBox;
//using Microsoft.Office.Interop.Excel;

namespace JSZW1000A.SubWindows
{
    public partial class SubOPSetting : UserControl
    {
        public MainFrm mf;
        public SubOPSetting()
        {
            InitializeComponent();
            setLang();
        }
        bool sw_角度映射 = false;

        private void SubOPSetting_Load(object sender, EventArgs e)
        {
            sw_角度映射 = false;
            LoadInit();

            lb后挡参数14.Visible = lb后挡参数15.Visible = lb后挡参数16.Visible = lb后挡参数17.Visible =
            lb后挡参数mm14.Visible = lb后挡参数mm15.Visible = lb后挡参数mm16.Visible = lb后挡参数mm17.Visible =
            txb后挡参数14.Visible = txb后挡参数15.Visible = txb后挡参数16.Visible = txb后挡参数17.Visible = (MainFrm.ConfigData[MainFrm.L1_GlobalSwitch + 9] > 400);
            txb后挡参数18.Visible = lb后挡参数18.Visible = lb后挡参数mm18.Visible = (MainFrm.ConfigData[MainFrm.L1_GlobalSwitch + 9] > 800);

            groupBox6.Visible = (MainFrm.ConfigData[MainFrm.L1_GlobalSwitch + 10] > 0);
        }

        private void setLang()
        {
            if (MainFrm.Lang == 0)
            {
                label5.Font = label1.Font = label2.Font = label3.Font = label4.Font = label6.Font = label9.Font = label11.Font = label13.Font =
                label16.Font = label19.Font = label20.Font = label18.Font = label15.Font = label21.Font = label22.Font = label23.Font =
                label24.Font = label25.Font = label26.Font = label27.Font = label28.Font = label73.Font = label71.Font = label72.Font = label70.Font = label51.Font =
                label50.Font = label69.Font = label53.Font = label52.Font = new System.Drawing.Font("微软雅黑", 12F);
                label74.Font = label75.Font = label76.Font = label77.Font = label78.Font =
                label7.Font = label8.Font = label10.Font = label12.Font = label14.Font = label17.Font = label33.Font = label34.Font =
                label35.Font = label36.Font = label37.Font = label38.Font = label39.Font = label40.Font = label41.Font =
                label48.Font = label49.Font = label97.Font = label98.Font = label99.Font = label100.Font = label101.Font = label102.Font = label103.Font =
                label47.Font = label81.Font = label82.Font = label83.Font = label84.Font = label85.Font = label86.Font = label87.Font = label88.Font = new System.Drawing.Font("微软雅黑", 10F);
                groupBox1.Font = groupBox2.Font = groupBox3.Font = groupBox4.Font = gbx角度映射.Font = new System.Drawing.Font("微软雅黑", 15.75F);
                btn解耦归零.Font = btn导入角度映像.Font = btn导入进阶参数.Font = btn角度映射保存.Font = btn角度映射下载.Font = new System.Drawing.Font("宋体", 10F);
            }
            else
            {
                label5.Font = label1.Font = label2.Font = label3.Font = label4.Font = label6.Font = label9.Font = label11.Font = label13.Font =
                label16.Font = label19.Font = label20.Font = label18.Font = label15.Font = label21.Font = label22.Font = label23.Font =
                label24.Font = label25.Font = label26.Font = label27.Font = label28.Font = label73.Font = label71.Font = label72.Font = label70.Font = label51.Font =
                label50.Font = label69.Font = label53.Font = label52.Font = new System.Drawing.Font("Calibri", 12F);
                label74.Font = label75.Font = label76.Font = label77.Font = label78.Font =
                label7.Font = label8.Font = label10.Font = label12.Font = label14.Font = label17.Font = label33.Font = label34.Font =
                label35.Font = label36.Font = label37.Font = label38.Font = label39.Font = label40.Font = label41.Font =
                label48.Font = label49.Font = label97.Font = label98.Font = label99.Font = label100.Font = label101.Font = label102.Font = label103.Font =
                label47.Font = label81.Font = label82.Font = label83.Font = label84.Font = label85.Font = label86.Font = label87.Font = label88.Font = new System.Drawing.Font("Calibri", 10F);
                groupBox1.Font = groupBox2.Font = groupBox3.Font = groupBox4.Font = gbx角度映射.Font = new System.Drawing.Font("Calibri", 15.75F);
                btn解耦归零.Font = btn导入角度映像.Font = btn导入进阶参数.Font = btn角度映射保存.Font = btn角度映射下载.Font = new System.Drawing.Font("Calibri", 10F);
            }
            label5.Text = (MainFrm.Lang == 0) ? "分条机速度" : "Slitter Speed";
            label1.Text = (MainFrm.Lang == 0) ? "加载桌板" : "Loading Tables";
            label2.Text = (MainFrm.Lang == 0) ? "角度显示" : "Angle Display";
            label3.Text = (MainFrm.Lang == 0) ? "松开方式" : "Unclamp Mode";
            label4.Text = (MainFrm.Lang == 0) ? "自动翻转" : "Auto Flip";
            label6.Text = (MainFrm.Lang == 0) ? "低" : "Low";
            label9.Text = (MainFrm.Lang == 0) ? "中" : "Medium";
            label11.Text = (MainFrm.Lang == 0) ? "高" : "High";
            label13.Text = (MainFrm.Lang == 0) ? "最大值" : "Maximum";
            label16.Text = (MainFrm.Lang == 0) ? "半夹钳" : "Half Clamp";
            label19.Text = (MainFrm.Lang == 0) ? "挤压折弯\r\n压钳高度" : "Squash Fold\r\nClamp Height";
            label20.Text = (MainFrm.Lang == 0) ? "最后一次折弯\r\n抬钳高度" : "Last Fold\r\nUnclamp Height";
            label18.Text = (MainFrm.Lang == 0) ? "后挡块最大位置 " : "Backgauge Max Pos";
            label15.Text = (MainFrm.Lang == 0) ? "后挡块最小位置 " : "Backgauge Min Pos";
            label21.Text = (MainFrm.Lang == 0) ? "分切偏移 " : "Slit Offset";
            label22.Text = (MainFrm.Lang == 0) ? "上翻板零位 " : "Top Apron Zero Position";
            label23.Text = (MainFrm.Lang == 0) ? "下翻板零位 " : "Bottom Apron Zero Position";
            label24.Text = (MainFrm.Lang == 0) ? "后挡停止位\r\n - 上挤压(仅自动)" : "Backstop\r\n - Upside Squash (AUTO)";
            label25.Text = (MainFrm.Lang == 0) ? "后挡停止位\r\n - 下挤压(仅自动)" : "Backstop\r\n - Dnside Squash (AUTO)";
            label26.Text = (MainFrm.Lang == 0) ? "后挡停止位\r\n - 外向挤压(仅自动)" : "Backstop\r\n - Outside Squash (AUTO)";
            label27.Text = (MainFrm.Lang == 0) ? "后挡停止位\r\n - 开放式上挤压(仅自动)" : "Backstop\r\n - Open Upside Squash (AUTO)";
            label28.Text = (MainFrm.Lang == 0) ? "后挡停止位 \r\n- 开放式下挤压(仅自动)" : "Backstop\r\n - Open Dnside Squash (AUTO)";
            label73.Text = (MainFrm.Lang == 0) ? "角度映射" : "Angle Mapping";
            label72.Text = (MainFrm.Lang == 0) ? "表格名称" : "Table Name";
            label70.Text = (MainFrm.Lang == 0) ? "材料" : "Material";
            label71.Text = (MainFrm.Lang == 0) ? "强度 " : "Strength";
            label51.Text = (MainFrm.Lang == 0) ? "厚度" : "Thickness";
            label50.Text = (MainFrm.Lang == 0) ? "机器测量" : "Machine Gauge";
            label69.Text = (MainFrm.Lang == 0) ? "翻板角度范围\r\n  (成型角度)" : "Fold Angle Range\r\n  (Forming Angle)";
            label53.Text = (MainFrm.Lang == 0) ? "下翻板补偿量" : "Bottom Offset";
            label52.Text = (MainFrm.Lang == 0) ? "上翻板补偿量" : "Top Offset";
            label74.Text = (MainFrm.Lang == 0) ? "一半" : "Half";
            label120.Text = (MainFrm.Lang == 0) ? "进料零点到设备的偏移" : "Deviation from zero feeding point to equipment";
            label121.Text = (MainFrm.Lang == 0) ? "板头到取料位置的偏移" : "Deviation from the board head to the material retrieval position";
            label122.Text = (MainFrm.Lang == 0) ? "钳口取料高度" : "Jaw height for material retrieval";
            label123.Text = (MainFrm.Lang == 0) ? "进料位置" : "Feed Position";

            label7.Text = label8.Text = label10.Text = label12.Text = label14.Text = label17.Text = label33.Text = label34.Text =
            label35.Text = label36.Text = label37.Text = label38.Text = label39.Text = label40.Text = label41.Text =
            label48.Text = label49.Text = label97.Text = label98.Text = label99.Text = label100.Text = label101.Text = label102.Text = label103.Text =
            label124.Text = label125.Text = label126.Text = label127.Text = (MainFrm.Lang == 0) ? "毫米" : "mm";
            label47.Text = label81.Text = label82.Text = label83.Text = label84.Text = label85.Text = label86.Text = label87.Text = label88.Text = (MainFrm.Lang == 0) ? "度" : "deg";

            groupBox1.Text = (MainFrm.Lang == 0) ? "夹钳设置 " : "Clamp Setting";
            groupBox2.Text = (MainFrm.Lang == 0) ? "后挡块 / 翻板设定 " : "Backgauge / Apron Setting";
            groupBox3.Text = (MainFrm.Lang == 0) ? "预设角度 " : "Preset Angles";
            groupBox4.Text = (MainFrm.Lang == 0) ? "预设长度 " : "Preset Lengths";
            groupBox5.Text = (MainFrm.Lang == 0) ? "后夹送原位设定 " : "Backgauge Home Setting";
            groupBox6.Text = (MainFrm.Lang == 0) ? "进料参数 " : "Feed Parameters";
            gbx角度映射.Text = (MainFrm.Lang == 0) ? "角度映射" : "Angle Map";
            btn解耦归零.Text = (MainFrm.Lang == 0) ? "伺服解耦\r\n 归零   " : "Decouple\r\n Servo Zero";
            btn导入角度映像.Text = (MainFrm.Lang == 0) ? " 导入  \r\n映像文件 " : "Load \r\nImage File";
            btn导入进阶参数.Text = (MainFrm.Lang == 0) ? " 导入  \r\n进阶参数 " : "Load \r\nAdv. Para";
            btn角度映射保存.Text = (MainFrm.Lang == 0) ? "保存" : "Save";
            btn角度映射下载.Text = (MainFrm.Lang == 0) ? "下载" : "Download";
            label114.Text = (MainFrm.Lang == 0) ? "成型角度不到时，增加补偿值;\r\n成型角度过头时，减小补偿值" : "Forming Angle is Insufficient, Add the Offset Value; \r\nForming Angle is Exceed, Reduce the Offset Value";

        }

        void textBox_MouseUp(object sender, MouseEventArgs e)
        {
            TextBox txb = (TextBox)sender;
            if (e.Button == MouseButtons.Left && (bool)txb.Tag == true)
                txb.SelectAll();
            txb.Tag = false;
        }

        void textBox_GotFocus(object sender, EventArgs e)
        {
            TextBox txb = (TextBox)sender;
            txb.Tag = true;    //设置标记             
            txb.SelectAll();   //注意1
        }

        private bool lastAdsConn = false;
        private int Delay0 = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            gbx角度映射.Visible = sw_角度映射;
            sw角度映射.BackgroundImage = sw_角度映射 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            sw分条机速度.BackgroundImage = MainFrm.Hmi_bArray[2] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            sw加载桌板.BackgroundImage = MainFrm.Hmi_bArray[3] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            sw角度显示.BackgroundImage = MainFrm.Hmi_bArray[4] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            sw松开方式1.BackgroundImage = MainFrm.Hmi_bArray[5] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            sw自动翻转.BackgroundImage = MainFrm.Hmi_bArray[6] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            sw自动进料.BackgroundImage = MainFrm.Hmi_bArray[7] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            sw演示.BackgroundImage = MainFrm.Hmi_bArray[8] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            //PLC连接跳变时,刷新一次
            switch (Delay0)
            {
                case 0:
                    if (MainFrm.AdsConn && !lastAdsConn)
                        lastAdsConn = MainFrm.AdsConn;
                    if (MainFrm.AdsConn && lastAdsConn)
                        Delay0 = 1;
                    break;
                case 1:
                    Delay0++; break;
                case 2:
                    Delay0++;
                    break;
                case 3:
                    LoadInit(); Delay0++;
                    break;

            }

        }

        private void sw角度映射_Click(object sender, EventArgs e)
        {
            sw_角度映射 = !sw_角度映射;
            sw角度映射.BackgroundImage = sw_角度映射 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            gbx角度映射.Visible = sw_角度映射;
            if (sw_角度映射) { Load角度映射(); }

        }

        void LoadInit()
        {
            txb夹钳_低.Text = MainFrm.Hmi_rArray[100].ToString();
            txb夹钳_中.Text = MainFrm.Hmi_rArray[101].ToString();
            txb夹钳_高.Text = MainFrm.Hmi_rArray[102].ToString();
            txb夹钳_最大.Text = MainFrm.Hmi_rArray[103].ToString();
            txb夹钳_半夹钳.Text = MainFrm.Hmi_rArray[104].ToString();
            txb夹钳_挤压折弯.Text = MainFrm.Hmi_rArray[105].ToString();
            txb夹钳_低.GotFocus += new EventHandler(textBox_GotFocus); txb夹钳_低.MouseUp += new MouseEventHandler(textBox_MouseUp);
            txb夹钳_中.GotFocus += new EventHandler(textBox_GotFocus); txb夹钳_中.MouseUp += new MouseEventHandler(textBox_MouseUp);
            txb夹钳_高.GotFocus += new EventHandler(textBox_GotFocus); txb夹钳_高.MouseUp += new MouseEventHandler(textBox_MouseUp);
            txb夹钳_最大.GotFocus += new EventHandler(textBox_GotFocus); txb夹钳_最大.MouseUp += new MouseEventHandler(textBox_MouseUp);
            txb夹钳_半夹钳.GotFocus += new EventHandler(textBox_GotFocus); txb夹钳_半夹钳.MouseUp += new MouseEventHandler(textBox_MouseUp);
            txb夹钳_挤压折弯.GotFocus += new EventHandler(textBox_GotFocus); txb夹钳_挤压折弯.MouseUp += new MouseEventHandler(textBox_MouseUp);
            //cbx挤压压钳高.SelectedIndex = Convert.ToInt32(MainFrm.Hmi_rArray[105]);
            cbx末次抬钳高.SelectedIndex = Convert.ToInt32(MainFrm.Hmi_rArray[106]);

            txb进料参数00.Text = MainFrm.Hmi_rArray[107].ToString();
            txb进料参数03.Text = MainFrm.Hmi_rArray[108].ToString();
            txb进料参数01.Text = MainFrm.Hmi_rArray[109].ToString();
            txb进料参数02.Text = MainFrm.Hmi_rArray[94].ToString();

            for (int i = 0; i < MainFrm.L3_BackgaugeApron - 2; i++)
            {
                System.Windows.Forms.TextBox txb = ((System.Windows.Forms.TextBox)this.Controls.Find("txb后挡参数" + string.Format("{0:D2}", i), true)[0]);
                txb.Text = string.Format("{0:F2}", MainFrm.Hmi_rArray[110 + i]);
                txb.GotFocus += new EventHandler(textBox_GotFocus);
                txb.MouseUp += new MouseEventHandler(textBox_MouseUp);
            }

            int idx = 0;
            cbx角度补偿0.Items.Clear();
            while (idx < mf.cbx材料选择.Items.Count)
            {
                cbx角度补偿0.Items.Add(mf.cbx材料选择.Items[idx]);
                idx++;
            }
            cbx角度补偿0.SelectedIndex = mf.cbx材料选择.SelectedIndex;
            //cbx角度补偿0.SelectedIndex = Convert.ToInt32(MainFrm.Hmi_rArray[56]);
            //Load角度映射();
        }
        private void Load角度映射()    //flag:选项从文件读取或手动选择
        {
            int id = 0;
            id = cbx角度补偿0.SelectedIndex;


            txb角度补偿2.Text = MainFrm.angleAddit[id].Material;
            txb角度补偿3.Text = MainFrm.angleAddit[id].Strength;
            txb角度补偿4.Text = MainFrm.angleAddit[id].Thickness;
            txb角度补偿5.Text = MainFrm.angleAddit[id].MachingGauging;
            for (int i = 0; i < 15; i++)
            {
                ((System.Windows.Forms.TextBox)this.Controls.Find("txb底部补偿" + string.Format("{0:D2}", i), true)[0]).Text = string.Format("{0:F2}", MainFrm.angleAddit[id].AngleRange[i]);
                ((System.Windows.Forms.TextBox)this.Controls.Find("txb顶部补偿" + string.Format("{0:D2}", i), true)[0]).Text = string.Format("{0:F2}", MainFrm.angleAddit[id].AngleRange[i + 15]);
            }
        }

        private void cbx角度补偿0_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainFrm.Hmi_rArray[56] = Convert.ToInt16(cbx角度补偿0.SelectedIndex);
            MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 9] = MainFrm.Hmi_rArray[56];
            mf.AdsWritePlc1float(56, MainFrm.Hmi_rArray[56]);
            mf.wrtConfigFile("[ManualOldSelect]", 9);
            Load角度映射();
        }
        private void sw分条机速度_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[2] = !MainFrm.Hmi_bArray[2];
            sw分条机速度.BackgroundImage = MainFrm.Hmi_bArray[2] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            mf.AdsWritePlc1Bit(2, MainFrm.Hmi_bArray[2]);
            MainFrm.ConfigData[MainFrm.L1_GlobalSwitch + 3] = Convert.ToSingle(MainFrm.Hmi_bArray[2]);
            mf.wrtConfigFile("[GlobalSwitch]", 3);
        }

        private void sw加载桌板_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[3] = !MainFrm.Hmi_bArray[3];
            sw加载桌板.BackgroundImage = MainFrm.Hmi_bArray[3] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            label75.Text = MainFrm.Hmi_bArray[3] ? (MainFrm.Lang == 0 ? "已启用" : "Enabled") : (MainFrm.Lang == 0 ? "已禁用" : "Disabled");
            mf.AdsWritePlc1Bit(3, MainFrm.Hmi_bArray[3]);
            MainFrm.ConfigData[MainFrm.L1_GlobalSwitch + 4] = Convert.ToSingle(MainFrm.Hmi_bArray[3]);
            mf.wrtConfigFile("[GlobalSwitch]", 4);
        }

        private void sw角度显示_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[4] = !MainFrm.Hmi_bArray[4];
            sw角度显示.BackgroundImage = MainFrm.Hmi_bArray[4] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            mf.AdsWritePlc1Bit(4, MainFrm.Hmi_bArray[4]);
            MainFrm.ConfigData[MainFrm.L1_GlobalSwitch + 5] = Convert.ToSingle(MainFrm.Hmi_bArray[4]);
            mf.wrtConfigFile("[GlobalSwitch]", 5);
        }

        private void sw松开方式1_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[5] = !MainFrm.Hmi_bArray[5];
            sw松开方式1.BackgroundImage = MainFrm.Hmi_bArray[5] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            label77.Text = MainFrm.Hmi_bArray[5] ? (MainFrm.Lang == 0 ? "自动" : "Auto") : (MainFrm.Lang == 0 ? "手动" : "Manual");
            mf.AdsWritePlc1Bit(5, MainFrm.Hmi_bArray[5]);
            MainFrm.ConfigData[MainFrm.L1_GlobalSwitch + 6] = Convert.ToSingle(MainFrm.Hmi_bArray[5]);
            mf.wrtConfigFile("[GlobalSwitch]", 6);
        }

        private void sw自动翻转_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[6] = !MainFrm.Hmi_bArray[6];
            sw自动翻转.BackgroundImage = MainFrm.Hmi_bArray[6] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            label78.Text = MainFrm.Hmi_bArray[6] ? (MainFrm.Lang == 0 ? "已启用" : "Enabled") : (MainFrm.Lang == 0 ? "已禁用" : "Disabled");
            mf.AdsWritePlc1Bit(6, MainFrm.Hmi_bArray[6]);
            MainFrm.ConfigData[MainFrm.L1_GlobalSwitch + 7] = Convert.ToSingle(MainFrm.Hmi_bArray[6]);
            mf.wrtConfigFile("[GlobalSwitch]", 7);
        }

        private void sw自动进料_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[7] = !MainFrm.Hmi_bArray[7];
            sw自动进料.BackgroundImage = MainFrm.Hmi_bArray[7] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            label29.Text = MainFrm.Hmi_bArray[7] ? (MainFrm.Lang == 0 ? "已启用" : "Enabled") : (MainFrm.Lang == 0 ? "已禁用" : "Disabled");
            mf.AdsWritePlc1Bit(7, MainFrm.Hmi_bArray[7]);
            MainFrm.ConfigData[MainFrm.L1_GlobalSwitch + 10] = Convert.ToSingle(MainFrm.Hmi_bArray[7]);
            mf.wrtConfigFile("[GlobalSwitch]", 10);

            groupBox6.Visible = (MainFrm.ConfigData[MainFrm.L1_GlobalSwitch + 10] > 0);
        }

        //后挡参数列中,有任何一个敲回车,下载该列数据到PLC并存入config.ini
        private void txb后挡参数0_KeyDown(object sender, KeyEventArgs e)
        {
            int idx = 0;
            // 判断：如果输入的是回车键
            if (e.KeyCode == Keys.Enter)
            {
                while (MainFrm.ConfigData[MainFrm.L3_BackgaugeApron + idx + 1] < 9990)
                {
                    MainFrm.Hmi_rArray[110 + idx] = Convert.ToSingle(((System.Windows.Forms.TextBox)this.Controls.Find("txb后挡参数" + string.Format("{0:D2}", idx), true)[0]).Text);
                    MainFrm.ConfigData[MainFrm.L3_BackgaugeApron + idx + 1] = MainFrm.Hmi_rArray[110 + idx];
                    idx++;
                }
                mf.AdsWritePlcFloat();
                mf.wrtConfigFile("[BackgaugeApron]", 0);

            }
        }

        private void txb夹钳_低_KeyDown(object sender, KeyEventArgs e)
        {
            // 判断：如果输入的是回车键
            if (e.KeyCode == Keys.Enter)
            {
                MainFrm.Hmi_rArray[100] = Convert.ToSingle(txb夹钳_低.Text);
                MainFrm.Hmi_rArray[101] = Convert.ToSingle(txb夹钳_中.Text);
                MainFrm.Hmi_rArray[102] = Convert.ToSingle(txb夹钳_高.Text);
                MainFrm.Hmi_rArray[103] = Convert.ToSingle(txb夹钳_最大.Text);
                MainFrm.Hmi_rArray[104] = Convert.ToSingle(txb夹钳_半夹钳.Text);
                MainFrm.Hmi_rArray[105] = Convert.ToSingle(txb夹钳_挤压折弯.Text);

                MainFrm.ConfigData[MainFrm.L2_ClampHeight + 1] = MainFrm.Hmi_rArray[100];
                MainFrm.ConfigData[MainFrm.L2_ClampHeight + 2] = MainFrm.Hmi_rArray[101];
                MainFrm.ConfigData[MainFrm.L2_ClampHeight + 3] = MainFrm.Hmi_rArray[102];
                MainFrm.ConfigData[MainFrm.L2_ClampHeight + 4] = MainFrm.Hmi_rArray[103];
                MainFrm.ConfigData[MainFrm.L2_ClampHeight + 5] = MainFrm.Hmi_rArray[104];
                MainFrm.ConfigData[MainFrm.L2_ClampHeight + 6] = MainFrm.Hmi_rArray[105];

                mf.AdsWritePlcFloat();
                mf.wrtConfigFile("[ClampHeight]", 1);
                mf.wrtConfigFile("[ClampHeight]", 2);
                mf.wrtConfigFile("[ClampHeight]", 3);
                mf.wrtConfigFile("[ClampHeight]", 4);
                mf.wrtConfigFile("[ClampHeight]", 5);
                mf.wrtConfigFile("[ClampHeight]", 6);
            }
        }

        private void cbx末次抬钳高_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainFrm.Hmi_rArray[106] = Convert.ToSingle(cbx末次抬钳高.SelectedIndex);
            MainFrm.ConfigData[MainFrm.L2_ClampHeight + 7] = MainFrm.Hmi_rArray[106];
            mf.AdsWritePlc1float(106, MainFrm.Hmi_rArray[106]);
            mf.wrtConfigFile("[ClampHeight]", 7);
        }

        private void btn角度映射保存_Click(object sender, EventArgs e)
        {
            int id = cbx角度补偿0.SelectedIndex;
            gets(id);
            mf.wrtAngleAdditFile(cbx角度补偿0.Text);
        }

        private void btn角度映射下载_Click(object sender, EventArgs e)
        {
            int id = cbx角度补偿0.SelectedIndex;
            gets(id);
            mf.AdsWritePlcFloat();

            MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 9] = id;
            mf.wrtConfigFile("[ManualOldSelect]", 9);
            mf.cbx材料选择.SelectedIndex = id;
        }

        void gets(int id)
        {
            MainFrm.angleAddit[id].Type = cbx角度补偿0.Text;
            MainFrm.angleAddit[id].Material = txb角度补偿2.Text;
            MainFrm.angleAddit[id].Strength = txb角度补偿3.Text;
            MainFrm.angleAddit[id].Thickness = txb角度补偿4.Text;
            MainFrm.angleAddit[id].MachingGauging = txb角度补偿5.Text;
            for (int i = 0; i < 15; i++)
            {
                MainFrm.Hmi_rArray[150 + i] = Convert.ToSingle(((System.Windows.Forms.TextBox)this.Controls.Find("txb底部补偿" + string.Format("{0:D2}", i), true)[0]).Text);
                MainFrm.Hmi_rArray[170 + i] = Convert.ToSingle(((System.Windows.Forms.TextBox)this.Controls.Find("txb顶部补偿" + string.Format("{0:D2}", i), true)[0]).Text);
                MainFrm.angleAddit[id].AngleRange[i] = MainFrm.Hmi_rArray[150 + i];
                MainFrm.angleAddit[id].AngleRange[i + 15] = MainFrm.Hmi_rArray[170 + i];
            }
        }



        string filename = System.Windows.Forms.Application.StartupPath + @"AngleMap.xlsx";
        private void btn导入角度映像_Click(object sender, EventArgs e)
        {
            Excel.Application app = new Excel.Application();
            Excel.Workbook wbk = app.Workbooks.Add(filename);
            int shCount = wbk.Worksheets.Count;
            Excel.Worksheet sh = wbk.Worksheets[1];
            sh.Activate();
            string s1 = "", s2 = "";
            for (int i = 0; i <= 378; i++)
            {
                if (i <= 155)
                {
                    s1 = sh.Cells[i + 2, 2].Text;
                    s2 = sh.Cells[i + 2, 3].Text;

                    MainFrm.Hmi_iAngleMapTop[i] = Convert.ToInt16(sh.Cells[i + 2, 2].Text);
                    MainFrm.Hmi_iAngleMapBtm[i] = Convert.ToInt16(sh.Cells[i + 2, 3].Text);
                }
                MainFrm.Hmi_iHeightMap[i] = Convert.ToInt16(sh.Cells[i + 2, 4].Text);
            }
            MainFrm.Hmi_rArray[1] = Convert.ToInt16(sh.Cells[2, 5].Text);
            MainFrm.Hmi_rArray[2] = Convert.ToInt16(sh.Cells[222, 5].Text);
            MainFrm.Hmi_rArray[3] = Convert.ToInt16(sh.Cells[2, 6].Text);
            MainFrm.Hmi_rArray[4] = Convert.ToInt16(sh.Cells[222, 6].Text);
            MainFrm.Hmi_rArray[5] = Convert.ToInt16(sh.Cells[2, 7].Text);
            MainFrm.Hmi_rArray[6] = Convert.ToInt16(sh.Cells[222, 7].Text);
            wbk.Close();
            app.Quit();
            mf.AdsWritePlc_AngleMap();
            mf.AdsWritePlc1float(1, MainFrm.Hmi_rArray[1]);
            mf.AdsWritePlc1float(2, MainFrm.Hmi_rArray[2]);
            mf.AdsWritePlc1float(3, MainFrm.Hmi_rArray[3]);
            mf.AdsWritePlc1float(4, MainFrm.Hmi_rArray[4]);
            mf.AdsWritePlc1float(5, MainFrm.Hmi_rArray[5]);
            mf.AdsWritePlc1float(6, MainFrm.Hmi_rArray[6]);

            if (MainFrm.Lang == 0)
                MessageBox.Show("映像数据更新及PLC写入完成!");
            else
                MessageBox.Show("Image data update and PLC writing completed!");
        }

        string filename2 = System.Windows.Forms.Application.StartupPath + @"Folder data values.xlsx";
        private void btn导入进阶参数_Click(object sender, EventArgs e)
        {
            string s0 = (MainFrm.Lang == 0) ? "确定导入进阶参数并更新到PLC吗？" : "Import Advanced Parameters and Update to the PLC?";
            DialogAsk dlgTips = new DialogAsk("", s0);
            //dlgTips.StartPosition = FormStartPosition.Manual;
            //dlgTips.Location = new Point(500, 200);           

            if (dlgTips.ShowDialog() == DialogResult.OK)
            {
                Excel.Application app = new Excel.Application();
                Excel.Workbook wbk = app.Workbooks.Add(filename2);
                int shCount = wbk.Worksheets.Count;
                Excel.Worksheet sh = wbk.Worksheets[1];
                sh.Activate();
                string s1 = "", s2 = "";
                for (int i = 0; i < 100; i++)
                {
                    s1 = sh.Cells[i + 2, 3].Text;
                    if (s1 != "")
                        MainFrm.Hmi_rAdvPara[i] = Convert.ToSingle(sh.Cells[i + 2, 3].Text);
                    else
                        MainFrm.Hmi_rAdvPara[i] = 0;
                }

                wbk.Close();
                app.Quit();
                mf.AdsWritePlc_AdvPara();
                if (MainFrm.Lang == 0)
                    MessageBox.Show("进阶参数更新及PLC写入完成!");
                else
                    MessageBox.Show("Advanced parameter update and PLC writing completed!");

            }
            else
            {
                ;
            }



        }

        private void btn解耦归零_MouseDown(object sender, MouseEventArgs e)
        {
            MainFrm.Hmi_bArray[11] = true;
            mf.AdsWritePlc();
        }

        private void btn解耦归零_MouseUp(object sender, MouseEventArgs e)
        {
            MainFrm.Hmi_bArray[11] = true;
            mf.AdsWritePlc();
        }



        private void txb进料参数0_KeyDown(object sender, KeyEventArgs e)
        {

            // 判断：如果输入的是回车键
            if (e.KeyCode == Keys.Enter)
            {
                MainFrm.Hmi_rArray[107] = Convert.ToSingle(txb进料参数00.Text);
                MainFrm.Hmi_rArray[109] = Convert.ToSingle(txb进料参数01.Text);
                MainFrm.Hmi_rArray[94] = Convert.ToSingle(txb进料参数02.Text);
                MainFrm.Hmi_rArray[108] = Convert.ToSingle(txb进料参数03.Text);

                MainFrm.ConfigData[MainFrm.L8_AutoFeedPara + 1] = MainFrm.Hmi_rArray[107];
                MainFrm.ConfigData[MainFrm.L8_AutoFeedPara + 2] = MainFrm.Hmi_rArray[109];
                MainFrm.ConfigData[MainFrm.L8_AutoFeedPara + 3] = MainFrm.Hmi_rArray[94];
                MainFrm.ConfigData[MainFrm.L8_AutoFeedPara + 4] = MainFrm.Hmi_rArray[108];

                mf.AdsWritePlcFloat();
                mf.wrtConfigFile("[AutoFeedPara]", 1);
                mf.wrtConfigFile("[AutoFeedPara]", 2);
                mf.wrtConfigFile("[AutoFeedPara]", 3);
                mf.wrtConfigFile("[AutoFeedPara]", 4);
            }
        }

        private void sw演示_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[8] = !MainFrm.Hmi_bArray[8];
            sw演示.BackgroundImage = MainFrm.Hmi_bArray[8] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            mf.AdsWritePlc1Bit(8, MainFrm.Hmi_bArray[8]);
        }
    }
}
