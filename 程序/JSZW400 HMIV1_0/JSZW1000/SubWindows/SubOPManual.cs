//using static System.Windows.Forms.VisualStyles.VisualStyleElement;

using static JSZW400.MainFrm;

namespace JSZW400.SubWindows
{
    public partial class SubOPManual : UserControl
    {

        MainFrm mf;
        public SubOPManual(MainFrm fm1)
        {
            InitializeComponent(); setLang();
            this.mf = fm1;
        }

        bool b1;
        bool dgvCellValEn = false;
        private void SubOPManual_Load(object sender, EventArgs e)
        {

            dgvCellValEn = false;

            dataGridView1.ColumnHeadersVisible = false;
            txb手动折弯角.Text = Convert.ToString(MainFrm.Hmi_rArray[50]);
            txb后挡块定位.Text = Convert.ToString(MainFrm.Hmi_rArray[51]);
            txb顶部翻板定位.Text = Convert.ToString(MainFrm.Hmi_rArray[52]);
            txb底部翻板定位.Text = Convert.ToString(MainFrm.Hmi_rArray[53]);
            txb桌板定位.Text = Convert.ToString(MainFrm.Hmi_rArray[54]);
            if (MainFrm.Hmi_iArray[60] < 1)
                txb开始序.Text = Convert.ToString(1);
            else
                txb开始序.Text = Convert.ToString(MainFrm.Hmi_iArray[60]);


            txb手动折弯角.Tag = false;
            txb手动折弯角.GotFocus += new EventHandler(textBox_GotFocus);
            txb手动折弯角.MouseUp += new MouseEventHandler(textBox_MouseUp);
            txb后挡块定位.Tag = false;
            txb后挡块定位.GotFocus += new EventHandler(textBox_GotFocus);
            txb后挡块定位.MouseUp += new MouseEventHandler(textBox_MouseUp);
            txb顶部翻板定位.Tag = false;
            txb顶部翻板定位.GotFocus += new EventHandler(textBox_GotFocus);
            txb顶部翻板定位.MouseUp += new MouseEventHandler(textBox_MouseUp);
            txb底部翻板定位.Tag = false;
            txb底部翻板定位.GotFocus += new EventHandler(textBox_GotFocus);
            txb底部翻板定位.MouseUp += new MouseEventHandler(textBox_MouseUp);
            txb桌板定位.Tag = false;
            txb桌板定位.GotFocus += new EventHandler(textBox_GotFocus);
            txb桌板定位.MouseUp += new MouseEventHandler(textBox_MouseUp);
            txb开始序.Tag = false;
            txb开始序.GotFocus += new EventHandler(textBox_GotFocus);
            txb开始序.MouseUp += new MouseEventHandler(textBox_MouseUp);

            if (MainFrm.CurtOrder.lstSemiAuto.Count > 0)
                FillDataGrid(MainFrm.CurtOrder.lstSemiAuto);

            dgvCellValEn = true;
            timer500ms.Enabled = true;

        }

        public void setLang()
        {
            if (MainFrm.Lang == 0)
            {
                lb折弯模式点动.Font = lb折弯模式自动.Font = new System.Drawing.Font("宋体", 8F);
                label11.Font = label10.Font = label14.Font = label15.Font = lb连续.Font = lb步骤.Font = lb分条开.Font = lb分条关.Font = lb桌板打开.Font =
                    lb桌板关闭.Font = label20.Font = label21.Font = label22.Font = label23.Font = label24.Font = label25.Font = label26.Font =
                    label27.Font = label28.Font = label29.Font = label1.Font = label5.Font = label9.Font =
                    label4.Font = label6.Font = label7.Font = label8.Font = label3.Font = label2.Font = new System.Drawing.Font("宋体", 11.25F);
                lb连续.Font = lb步骤.Font = lb分条开.Font = lb分条关.Font = lb桌板打开.Font = lb桌板关闭.Font = lb分条点.Font = lb折弯点.Font =
                    lb连续.Font = lb连续.Font = lb连续.Font = lb连续.Font = lb连续.Font = lb连续.Font = new System.Drawing.Font("宋体", 11.25F);
                btn装载材料.Font = btn分条.Font = btn挤压.Font = btn开口挤压.Font = btn重置计数.Font = btn整机初始化.Font = btn后挡归原位.Font =
                    btn翻板向下.Font = btn分条机归原位.Font = btn移动后挡块.Font = btn滑动顶部翻板.Font = btn滑动底部翻板.Font = btn滑动双向翻板.Font = btn加载桌板.Font = new System.Drawing.Font("宋体", 11.25F);
                btn列表_之前插入.Font = btn列表_之后插入.Font = btn列表_清除.Font =
                   btn列表_上移.Font = btn列表_下移.Font = btn列表_复制.Font = btn列表_切换重新抓取.Font = new System.Drawing.Font("宋体", 10F);
                label20.Location = new Point(label20.Location.X, 330); label21.Location = new Point(label21.Location.X, 330); label22.Location = new Point(label22.Location.X, 330);
                label23.Location = new Point(label23.Location.X, 330); label24.Location = new Point(label24.Location.X, 330); label26.Location = new Point(label26.Location.X, 330);
            }
            else
            {
                lb折弯模式点动.Font = lb折弯模式自动.Font = new System.Drawing.Font("Calibri", 8F);
                label11.Font = label10.Font = label14.Font = label15.Font = lb连续.Font = lb步骤.Font = lb分条开.Font = lb分条关.Font = lb桌板打开.Font =
                    lb桌板关闭.Font = label20.Font = label21.Font = label22.Font = label23.Font = label24.Font = label25.Font = label26.Font =
                    label27.Font = label28.Font = label29.Font = label1.Font = label5.Font = label9.Font =
                    label4.Font = label6.Font = label7.Font = label8.Font = label3.Font = label2.Font = new System.Drawing.Font("Calibri", 10F);
                lb连续.Font = lb步骤.Font = lb分条开.Font = lb分条关.Font = lb桌板打开.Font = lb桌板关闭.Font = lb分条点.Font = lb折弯点.Font =
                    lb连续.Font = lb连续.Font = lb连续.Font = lb连续.Font = lb连续.Font = lb连续.Font = new System.Drawing.Font("Calibri", 10F);
                btn装载材料.Font = btn分条.Font = btn挤压.Font = btn开口挤压.Font = btn重置计数.Font = btn整机初始化.Font = btn后挡归原位.Font =
                    btn翻板向下.Font = btn分条机归原位.Font = btn移动后挡块.Font = btn滑动顶部翻板.Font = btn滑动底部翻板.Font = btn滑动双向翻板.Font = btn加载桌板.Font = new System.Drawing.Font("Calibri", 11F);
                btn列表_之前插入.Font = btn列表_之后插入.Font = btn列表_清除.Font =
                    btn列表_上移.Font = btn列表_下移.Font = btn列表_复制.Font = btn列表_切换重新抓取.Font = new System.Drawing.Font("Calibri", 10F);
                label20.Location = new Point(label20.Location.X, 315); label21.Location = new Point(label21.Location.X, 315); label22.Location = new Point(label22.Location.X, 315);
                label23.Location = new Point(label23.Location.X, 315); label24.Location = new Point(label24.Location.X, 315); label26.Location = new Point(label26.Location.X, 315);

            }
            label11.Text = (MainFrm.Lang == 0) ? "开始\r\n半自动" : "Start\r\nSemi-Auto";
            label14.Text = (MainFrm.Lang == 0) ? "后夹送开关" : "BACKGAUGE";
            label15.Text = (MainFrm.Lang == 0) ? "推动后单步" : "PUSH-Step";
            lb连续.Text = (MainFrm.Lang == 0) ? "连续" : "CONTINUE";
            lb步骤.Text = (MainFrm.Lang == 0) ? "步骤" : "STEP";
            lb分条开.Text = (MainFrm.Lang == 0) ? "分条" : "SLIT";
            lb分条关.Text = (MainFrm.Lang == 0) ? "关" : "OFF";
            lb桌板打开.Text = (MainFrm.Lang == 0) ? "桌板打开" : "TABLE ON";
            lb桌板关闭.Text = (MainFrm.Lang == 0) ? "桌板关闭" : "TABLE OFF";
            label21.Text = (MainFrm.Lang == 0) ? "行动类型" : "ACTION\r\nTYPE";
            label20.Text = (MainFrm.Lang == 0) ? "折弯角度" : "FLOD\r\nANGLE";
            label22.Text = (MainFrm.Lang == 0) ? "材料回弹" : "MATERIAL\r\nSPRINGBACK";
            label23.Text = (MainFrm.Lang == 0) ? "后夹送位置" : "BACKGAUGE\r\nPOSITION";
            label24.Text = (MainFrm.Lang == 0) ? "抓取类型" : "GRIP\r\nTYPE";
            label25.Text = (MainFrm.Lang == 0) ? "松开夹钳\r\n  高度" : "UNCLAMP\r\nHEIGHT";
            label26.Text = (MainFrm.Lang == 0) ? "翻板收缩" : "APRON\r\nRETRACT";
            label27.Text = (MainFrm.Lang == 0) ? "开始" : "START";
            label28.Text = (MainFrm.Lang == 0) ? "下一步" : "NEXT";
            label29.Text = (MainFrm.Lang == 0) ? "计数" : "COUNT";
            label1.Text = (MainFrm.Lang == 0) ? "折弯模式" : "FOLDMODE";
            lb分条点.Text = (MainFrm.Lang == 0) ? "分条点" : "SLIT";
            lb折弯点.Text = (MainFrm.Lang == 0) ? "折弯点" : "FOLD";
            label10.Text = (MainFrm.Lang == 0) ? "开始\r\n手动" : "Start\r\nManual";
            lb折弯模式点动.Text = (MainFrm.Lang == 0) ? "点动" : "JOG";
            lb折弯模式自动.Text = (MainFrm.Lang == 0) ? "自动" : "AUTO";
            label5.Text = (MainFrm.Lang == 0) ? "角度" : "COUNT";
            label9.Text = label4.Text = label6.Text = label7.Text = label8.Text = label3.Text = label2.Text = (MainFrm.Lang == 0) ? "毫米" : "mm";


            btn装载材料.Text = (MainFrm.Lang == 0) ? " 装载\r\n 材料" : "Load  \r\nMaterial";
            btn分条.Text = (MainFrm.Lang == 0) ? " 分条" : "Slit";
            btn挤压.Text = (MainFrm.Lang == 0) ? " 挤压" : " Squash";
            btn开口挤压.Text = (MainFrm.Lang == 0) ? " 开口\r\n 挤压" : " Open\r\n Squash";
            btn下载到PLC.Text = (MainFrm.Lang == 0) ? "下载半自动\r\n到PLC" : "Download\r\n to PLC";
            btn列表_之前插入.Text = (MainFrm.Lang == 0) ? " 在...之前插入" : "   Insert Before";
            btn列表_之后插入.Text = (MainFrm.Lang == 0) ? " 在...之后插入" : "   Insert After";
            btn列表_清除.Text = (MainFrm.Lang == 0) ? "    删除  " : "   Delete";
            btn列表_上移.Text = (MainFrm.Lang == 0) ? "    上移  " : "   Move Up";
            btn列表_下移.Text = (MainFrm.Lang == 0) ? "    下移  " : "   Move Down";
            btn列表_复制.Text = (MainFrm.Lang == 0) ? "    复制  " : "   Copy";
            btn列表_切换重新抓取.Text = (MainFrm.Lang == 0) ? "   切换  \r\n  重新抓取" : "  Toggle\r\n  Regrip";
            btn重置计数.Text = (MainFrm.Lang == 0) ? " 重置\r\n 计数" : " Reset\r\n Count";
            btn整机初始化.Text = (MainFrm.Lang == 0) ? " 整机复位" : "Machine Reset";
            btn后挡归原位.Text = (MainFrm.Lang == 0) ? " 后挡归原位" : "Backgauge Home";
            btn翻板向下.Text = (MainFrm.Lang == 0) ? " 翻板归零  " : "  Apron Down";
            btn分条机归原位.Text = (MainFrm.Lang == 0) ? "   分条机\r\n   归原位" : "  Slitter Home";
            btn移动后挡块.Text = (MainFrm.Lang == 0) ? "    移动 \r\n   后挡块" : "Move    \r\n  Backgauge";
            btn滑动顶部翻板.Text = (MainFrm.Lang == 0) ? "    滑动  \r\n  顶部翻板" : "Slide    \r\n  Top Apron";
            btn滑动底部翻板.Text = (MainFrm.Lang == 0) ? "  滑动  \r\n  底部翻板" : "Slide       \r\n Bottom Apron";
            btn滑动双向翻板.Text = (MainFrm.Lang == 0) ? "  滑动  \r\n  双向翻板" : "Slide      \r\n Both Aprons";
            btn加载桌板.Text = (MainFrm.Lang == 0) ? "  移动  \r\n  加载桌板" : "Move       \r\nLoading Tables";





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
            txb.Tag = true;
            //txb.SelectAll();
        }

        List<SemiAutoType> lstSemiAuto2 = new List<SemiAutoType>();

        /* 挤压不作为单独步骤的函数
        void FillDataGrid2(List<SemiAutoType> odr)
        {
            //int i = 0;
            //while (odr[i].折弯角度 != 0)
            dataGridView1.Rows.Clear();
            for (int i = 0; i < odr.Count; i++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView1);
                ((DataGridViewComboBoxCell)row.Cells[7]).Items.Add("推动");
                ((DataGridViewComboBoxCell)row.Cells[7]).Items.Add("抓取");
                ((DataGridViewComboBoxCell)row.Cells[7]).Items.Add("超程抓取");
                ((DataGridViewComboBoxCell)row.Cells[8]).Items.Add("低");
                ((DataGridViewComboBoxCell)row.Cells[8]).Items.Add("中");
                ((DataGridViewComboBoxCell)row.Cells[8]).Items.Add("高");
                ((DataGridViewComboBoxCell)row.Cells[8]).Items.Add("最大");
                ((DataGridViewComboBoxCell)row.Cells[9]).Items.Add("1");
                ((DataGridViewComboBoxCell)row.Cells[9]).Items.Add("2");
                ((DataGridViewComboBoxCell)row.Cells[9]).Items.Add("3");
                ((DataGridViewComboBoxCell)row.Cells[9]).Items.Add("4");

                row.Cells[0].Value = odr[i].折弯序号 + ".";

                if (odr[i].行动类型 == 3)
                {
                    row.Cells[1].Tag = 4;
                    row.Cells[1].Value = global::JSZW400.Properties.Resources.Slit7;
                    row.Cells[2].Value = "分切";
                    row.Cells[3].Value = global::JSZW400.Properties.Resources.null0;
                    row.Cells[4].Value = "";
                }
                else if (odr[i].行动类型 == 0 && odr[i].折弯方向 == 0)  //向上折弯
                {
                    row.Cells[1].Tag = 1;
                    row.Cells[1].Value = global::JSZW400.Properties.Resources.BottomApron4;
                    if (odr[i].折弯角度 < 30)
                        row.Cells[2].Value = 30.0;
                    else
                        row.Cells[2].Value = Convert.ToString(odr[i].折弯角度);
                    row.Cells[3].Value = global::JSZW400.Properties.Resources.SB;
                    row.Cells[4].Value = Convert.ToString(odr[i].回弹值);
                }
                else if (odr[i].行动类型 == 0 && odr[i].折弯方向 == 1)  //向下折弯
                {
                    row.Cells[1].Tag = 2;
                    row.Cells[1].Value = global::JSZW400.Properties.Resources.TopApron4;
                    if (odr[i].折弯角度 < 30)
                        row.Cells[2].Value = 30.0;
                    else
                        row.Cells[2].Value = Convert.ToString(odr[i].折弯角度);
                    row.Cells[3].Value = global::JSZW400.Properties.Resources.SB1;
                    row.Cells[4].Value = Convert.ToString(odr[i].回弹值);
                }
                else if (odr[i].行动类型 == 1)
                {
                    //row.Cells[1].Tag = 3;
                    //row.Cells[1].Value = global::JSZW400.Properties.Resources.Squash;
                    //row.Cells[2].Value = "挤压";
                    //row.Cells[3].Value = global::JSZW400.Properties.Resources.null0;
                    //row.Cells[4].Value = "";
                }
                row.Cells[5].Value = Convert.ToString(odr[i].后挡位置);
                row.Cells[10].Value = (odr[i].折弯方向 == 1) ? global::JSZW400.Properties.Resources.RetractBA
                                                                          : global::JSZW400.Properties.Resources.RetractTA;

                switch (odr[i].抓取类型)
                {
                    case 0:
                        row.Cells[6].Value = global::JSZW400.Properties.Resources.Gripper1;
                        break;
                    case 1:
                        row.Cells[6].Value = global::JSZW400.Properties.Resources.Gripper2;
                        break;
                    case 2:
                        row.Cells[6].Value = global::JSZW400.Properties.Resources.Gripper3;
                        break;
                    default:
                        break;
                }

                ((DataGridViewComboBoxCell)row.Cells[7]).Value = ((DataGridViewComboBoxCell)row.Cells[7]).Items[odr[i].抓取类型];
                ((DataGridViewComboBoxCell)row.Cells[8]).Value = ((DataGridViewComboBoxCell)row.Cells[8]).Items[odr[i].松开高度];
                ((DataGridViewComboBoxCell)row.Cells[9]).Value = ((DataGridViewComboBoxCell)row.Cells[9]).Items[odr[i].翻板收缩值];

                if (odr[i].重新抓取 == 0)
                {
                    row.DefaultCellStyle.ForeColor = Color.Black;
                    row.DefaultCellStyle.SelectionForeColor = Color.White;
                }
                else
                {
                    row.DefaultCellStyle.ForeColor = Color.Yellow;
                    row.DefaultCellStyle.SelectionForeColor = Color.Yellow;
                }

                //添加的行作为第一行
                dataGridView1.Rows.Add(row);

                //本行为挤压前折弯,需要增加以下挤压条目
                DataGridViewRow row_sq = new DataGridViewRow();
                row_sq.CreateCells(dataGridView1);
                ((DataGridViewComboBoxCell)row_sq.Cells[7]).Items.Add("推动");
                ((DataGridViewComboBoxCell)row_sq.Cells[7]).Items.Add("抓取");
                ((DataGridViewComboBoxCell)row_sq.Cells[7]).Items.Add("超程抓取");
                ((DataGridViewComboBoxCell)row_sq.Cells[8]).Items.Add("低");
                ((DataGridViewComboBoxCell)row_sq.Cells[8]).Items.Add("中");
                ((DataGridViewComboBoxCell)row_sq.Cells[8]).Items.Add("高");
                ((DataGridViewComboBoxCell)row_sq.Cells[8]).Items.Add("最大");
                ((DataGridViewComboBoxCell)row_sq.Cells[9]).Items.Add("1");
                ((DataGridViewComboBoxCell)row_sq.Cells[9]).Items.Add("2");
                ((DataGridViewComboBoxCell)row_sq.Cells[9]).Items.Add("3");
                ((DataGridViewComboBoxCell)row_sq.Cells[9]).Items.Add("4");
                if (odr[i].折弯角度 == 3.001)  //异常折弯角度表示挤压
                {
                    row_sq.Cells[1].Tag = 3;
                    row_sq.Cells[1].Value = global::JSZW400.Properties.Resources.Squash;
                    row_sq.Cells[2].Value = "挤压";
                    row_sq.Cells[3].Value = global::JSZW400.Properties.Resources.null0;
                    row_sq.Cells[4].Value = "";

                    double d1 = (odr[i].折弯方向 == 0) ? Hmi_rArray[115] : Hmi_rArray[116];
                    row_sq.Cells[5].Value = Convert.ToString(odr[i].后挡位置 + (-1) * d1);
                    row_sq.Cells[6].Value = row.Cells[6].Value;
                    ((DataGridViewComboBoxCell)row_sq.Cells[7]).Value = ((DataGridViewComboBoxCell)row_sq.Cells[7]).Items[odr[i].抓取类型];
                    int ix = (odr[i].操作提示 == 1) ? 2 : odr[i].松开高度;
                    ((DataGridViewComboBoxCell)row_sq.Cells[8]).Value = ((DataGridViewComboBoxCell)row_sq.Cells[8]).Items[ix];
                    ((DataGridViewComboBoxCell)row_sq.Cells[9]).Value = ((DataGridViewComboBoxCell)row_sq.Cells[9]).Items[odr[i].翻板收缩值];
                    row_sq.Cells[10].Value = row.Cells[10].Value;
                    dataGridView1.Rows.Add(row_sq);
                }
                else if (odr[i].折弯角度 == 3.99)
                {
                    row_sq.Cells[1].Tag = 3;
                    row_sq.Cells[1].Value = global::JSZW400.Properties.Resources.Squash;
                    row_sq.Cells[2].Value = "挤压";
                    row_sq.Cells[3].Value = global::JSZW400.Properties.Resources.null0;
                    row_sq.Cells[4].Value = "";

                    double d1 = (odr[i].折弯方向 == 0) ? Hmi_rArray[115] : Hmi_rArray[116];
                    row_sq.Cells[5].Value = Convert.ToString(odr[i].后挡位置 + (-1) * d1);
                    row_sq.Cells[6].Value = row.Cells[6].Value;
                    ((DataGridViewComboBoxCell)row_sq.Cells[7]).Value = ((DataGridViewComboBoxCell)row_sq.Cells[7]).Items[odr[i].抓取类型];
                    int ix = (odr[i].操作提示 == 1) ? 2 : odr[i].松开高度;
                    ((DataGridViewComboBoxCell)row_sq.Cells[8]).Value = ((DataGridViewComboBoxCell)row_sq.Cells[8]).Items[ix];
                    ((DataGridViewComboBoxCell)row_sq.Cells[9]).Value = ((DataGridViewComboBoxCell)row_sq.Cells[9]).Items[odr[i].翻板收缩值];
                    row_sq.Cells[10].Value = row.Cells[10].Value;
                    dataGridView1.Rows.Add(row_sq);
                }
            }
        }
        */

        void FillDataGrid(List<SemiAutoType> odr)
        {
            //int i = 0;
            //while (odr[i].折弯角度 != 0)
            dataGridView1.Rows.Clear();
            for (int i = 0; i < odr.Count; i++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView1);
                ((DataGridViewComboBoxCell)row.Cells[7]).Items.Add(MainFrm.Lang == 0 ? "推动" : "PUSH");
                ((DataGridViewComboBoxCell)row.Cells[7]).Items.Add(MainFrm.Lang == 0 ? "抓取" : "GRIP");
                ((DataGridViewComboBoxCell)row.Cells[7]).Items.Add(MainFrm.Lang == 0 ? "超程抓取" : "OVERGRIP");
                ((DataGridViewComboBoxCell)row.Cells[8]).Items.Add(MainFrm.Lang == 0 ? "低" : "LOW");
                ((DataGridViewComboBoxCell)row.Cells[8]).Items.Add(MainFrm.Lang == 0 ? "中" : "Medium");
                ((DataGridViewComboBoxCell)row.Cells[8]).Items.Add(MainFrm.Lang == 0 ? "高" : "HIGH");
                ((DataGridViewComboBoxCell)row.Cells[8]).Items.Add(MainFrm.Lang == 0 ? "最大" : "MAX");
                ((DataGridViewComboBoxCell)row.Cells[9]).Items.Add("1");
                ((DataGridViewComboBoxCell)row.Cells[9]).Items.Add("2");
                ((DataGridViewComboBoxCell)row.Cells[9]).Items.Add("3");
                ((DataGridViewComboBoxCell)row.Cells[9]).Items.Add("4");

                row.Cells[0].Value = odr[i].折弯序号 + ".";

                if (odr[i].行动类型 == 3)
                {
                    row.Cells[1].Tag = 5;
                    row.Cells[1].Value = global::JSZW400.Properties.Resources.Slit7;
                    row.Cells[2].Value = MainFrm.Lang == 0 ? "分切" : "SLIT";
                    row.Cells[3].Value = global::JSZW400.Properties.Resources.null0;
                    row.Cells[4].Value = "";
                }
                else if (odr[i].行动类型 == 0 && odr[i].折弯方向 == 0)  //向上折弯
                {
                    row.Cells[1].Tag = 1;
                    row.Cells[1].Value = global::JSZW400.Properties.Resources.BottomApron4;
                    if (odr[i].折弯角度 < 30)
                        row.Cells[2].Value = 30.0;
                    else
                        row.Cells[2].Value = Convert.ToString(odr[i].折弯角度);
                    row.Cells[3].Value = global::JSZW400.Properties.Resources.SB;
                    row.Cells[4].Value = Convert.ToString(odr[i].回弹值);
                }
                else if (odr[i].行动类型 == 0 && odr[i].折弯方向 == 1)  //向下折弯
                {
                    row.Cells[1].Tag = 2;
                    row.Cells[1].Value = global::JSZW400.Properties.Resources.TopApron4;
                    if (odr[i].折弯角度 < 30)
                        row.Cells[2].Value = 30.0;
                    else
                        row.Cells[2].Value = Convert.ToString(odr[i].折弯角度);
                    row.Cells[3].Value = global::JSZW400.Properties.Resources.SB1;
                    row.Cells[4].Value = Convert.ToString(odr[i].回弹值);
                }
                else if (odr[i].行动类型 == 1)
                {
                    row.Cells[1].Tag = 3;
                    row.Cells[1].Value = global::JSZW400.Properties.Resources.Squash;
                    row.Cells[2].Value = MainFrm.Lang == 0 ? "挤压" : "SQUASH";
                    row.Cells[3].Value = global::JSZW400.Properties.Resources.null0;
                    row.Cells[4].Value = "";
                }
                else if (odr[i].行动类型 == 2)      //open squash
                {
                    row.Cells[1].Tag = 4;
                    row.Cells[1].Value = global::JSZW400.Properties.Resources.SquashOpen;
                    row.Cells[2].Value = MainFrm.Lang == 0 ? "开口挤压" : "OPEN SQUASH";
                    row.Cells[3].Value = global::JSZW400.Properties.Resources.null0;
                    row.Cells[4].Value = "";
                }
                else if (odr[i].行动类型 == 8)      //flip
                {
                    row.Cells[1].Tag = 8;
                    row.Cells[1].Value = global::JSZW400.Properties.Resources.Flip;
                    row.Cells[2].Value = MainFrm.Lang == 0 ? "翻面" : "FLIP";
                    row.Cells[3].Value = global::JSZW400.Properties.Resources.null0;
                    row.Cells[4].Value = "";
                }
                row.Cells[5].Value = Convert.ToString(odr[i].后挡位置);
                row.Cells[5].Tag = odr[i].锥度斜率;
                row.Cells[10].Value = (odr[i].折弯方向 == 1) ? global::JSZW400.Properties.Resources.RetractBA
                                                                          : global::JSZW400.Properties.Resources.RetractTA;

                switch (odr[i].抓取类型)
                {
                    case 0:
                        row.Cells[6].Value = global::JSZW400.Properties.Resources.Gripper1;
                        break;
                    case 1:
                        row.Cells[6].Value = global::JSZW400.Properties.Resources.Gripper2;
                        break;
                    case 2:
                        row.Cells[6].Value = global::JSZW400.Properties.Resources.Gripper3;
                        break;
                    default:
                        break;
                }

                ((DataGridViewComboBoxCell)row.Cells[7]).Value = ((DataGridViewComboBoxCell)row.Cells[7]).Items[odr[i].抓取类型];
                ((DataGridViewComboBoxCell)row.Cells[8]).Value = ((DataGridViewComboBoxCell)row.Cells[8]).Items[odr[i].松开高度];
                ((DataGridViewComboBoxCell)row.Cells[9]).Value = ((DataGridViewComboBoxCell)row.Cells[9]).Items[odr[i].翻板收缩值];

                if (odr[i].重新抓取 == 0)
                {
                    row.DefaultCellStyle.ForeColor = Color.Black;
                    row.DefaultCellStyle.SelectionForeColor = Color.White;
                }
                else
                {
                    row.DefaultCellStyle.ForeColor = Color.Yellow;
                    row.DefaultCellStyle.SelectionForeColor = Color.Yellow;
                }

                //添加的行作为第一行
                dataGridView1.Rows.Add(row);

            }
        }



        void InsertRow(int i)
        {
            //dataGridView1.Rows.Clear();
            DataGridViewRow dr = new DataGridViewRow();
            dr.CreateCells(dataGridView1);
            ((DataGridViewComboBoxCell)dr.Cells[7]).Items.Add(MainFrm.Lang == 0 ? "推动" : "PUSH");
            ((DataGridViewComboBoxCell)dr.Cells[7]).Items.Add(MainFrm.Lang == 0 ? "抓取" : "GRIP");
            ((DataGridViewComboBoxCell)dr.Cells[7]).Items.Add(MainFrm.Lang == 0 ? "超程抓取" : "OVERGRIP");
            ((DataGridViewComboBoxCell)dr.Cells[8]).Items.Add(MainFrm.Lang == 0 ? "低" : "LOW");
            ((DataGridViewComboBoxCell)dr.Cells[8]).Items.Add(MainFrm.Lang == 0 ? "中" : "Medium");
            ((DataGridViewComboBoxCell)dr.Cells[8]).Items.Add(MainFrm.Lang == 0 ? "高" : "HIGH");
            ((DataGridViewComboBoxCell)dr.Cells[8]).Items.Add(MainFrm.Lang == 0 ? "最大" : "MAX");
            ((DataGridViewComboBoxCell)dr.Cells[9]).Items.Add("1");
            ((DataGridViewComboBoxCell)dr.Cells[9]).Items.Add("2");
            ((DataGridViewComboBoxCell)dr.Cells[9]).Items.Add("3");
            ((DataGridViewComboBoxCell)dr.Cells[9]).Items.Add("4");


            dr.Cells[0].Value = (i + 1).ToString() + ".";
            dr.Cells[1].Value = global::JSZW400.Properties.Resources.BottomApron4;
            dr.Cells[1].Tag = 1;
            dr.Cells[2].Value = "179";       //"179.0°"
            dr.Cells[3].Value = global::JSZW400.Properties.Resources.SB;
            dr.Cells[4].Value = "3";
            dr.Cells[5].Value = "500";
            dr.Cells[6].Value = global::JSZW400.Properties.Resources.Gripper2;
            ((DataGridViewComboBoxCell)dr.Cells[7]).Value = ((DataGridViewComboBoxCell)dr.Cells[7]).Items[1];
            ((DataGridViewComboBoxCell)dr.Cells[8]).Value = ((DataGridViewComboBoxCell)dr.Cells[8]).Items[0];
            ((DataGridViewComboBoxCell)dr.Cells[9]).Value = ((DataGridViewComboBoxCell)dr.Cells[9]).Items[3];
            dr.Cells[10].Value = global::JSZW400.Properties.Resources.RetractTA;

            dr.Cells[3].Tag = 1;
            dr.Cells[6].Tag = 1;
            dr.Cells[10].Tag = 1;

            //添加的行作为第一行
            dataGridView1.Rows.Insert(i, dr);
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //int k = ((DataGridViewComboBoxCell)this.dataGridView1.Rows[0].Cells[4]).RowIndex;
            if (dgvCellValEn)
            {
                reGeSemiData();
                cancelMode();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            keystep = 0;
            pnlQuickInput.Visible = false;
            int i = e.RowIndex;

            if (e.ColumnIndex == 1)// "行动类型"
            {
                if (Convert.ToInt16(this.dataGridView1.Rows[i].Cells[1].Tag) == 1)
                {
                    this.dataGridView1.Rows[i].Cells[1].Value = global::JSZW400.Properties.Resources.TopApron4;
                    this.dataGridView1.Rows[i].Cells[1].Tag = 2;
                    this.dataGridView1.Rows[i].Cells[3].Value = global::JSZW400.Properties.Resources.SB1;
                    this.dataGridView1.Rows[i].Cells[4].Value = MainFrm.SpringTop;
                    this.dataGridView1.Rows[i].Cells[10].Value = global::JSZW400.Properties.Resources.RetractBA;
                }
                else
                {
                    this.dataGridView1.Rows[i].Cells[1].Value = global::JSZW400.Properties.Resources.BottomApron4;
                    this.dataGridView1.Rows[i].Cells[1].Tag = 1;
                    this.dataGridView1.Rows[i].Cells[3].Value = global::JSZW400.Properties.Resources.SB;
                    this.dataGridView1.Rows[i].Cells[4].Value = MainFrm.SpringBtm;
                    this.dataGridView1.Rows[i].Cells[10].Value = global::JSZW400.Properties.Resources.RetractTA;
                }
            }

            //if (Convert.ToString(dataGridView1.Rows[i + 1].Cells[0].Value) == "" )
            //    dataGridView1.Rows[i + 1].Selected = true;
            //else if (Convert.ToString(dataGridView1.Rows[i].Cells[0].Value) == "")
            //    dataGridView1.Rows[i - 1].Selected = true;


        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int row = dataGridView1.CurrentCell.RowIndex;
            //判断相应的列
            if (dataGridView1.CurrentCell == dataGridView1[7, row] && dataGridView1.CurrentCell.RowIndex != -1)
            {
                //给这个DataGridViewComboBoxCell加上下拉事件
                (e.Control as System.Windows.Forms.ComboBox).SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);
            }
            if (dataGridView1.CurrentCell == dataGridView1[8, 0] && dataGridView1.CurrentCell.RowIndex != -1)
            {
                //给这个DataGridViewComboBoxCell加上下拉事件
                (e.Control as System.Windows.Forms.ComboBox).SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);
            }
            if (dataGridView1.CurrentCell == dataGridView1[9, 0] && dataGridView1.CurrentCell.RowIndex != -1)
            {
                //给这个DataGridViewComboBoxCell加上下拉事件
                (e.Control as System.Windows.Forms.ComboBox).SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);
            }
        }

        public void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.ComboBox combox = sender as System.Windows.Forms.ComboBox;
            int row = dataGridView1.CurrentCell.RowIndex;
            //这里比较重要
            combox.Leave += new EventHandler(combox_Leave);
            try
            {
                string s = Convert.ToString(combox.SelectedItem);
                if (dataGridView1.CurrentCell == dataGridView1[7, row] && dataGridView1.CurrentCell.RowIndex != -1)   //抓取类型
                {
                    if (combox.SelectedIndex == 2)
                        this.dataGridView1.Rows[row].Cells[6].Value = global::JSZW400.Properties.Resources.Gripper3;
                    else if (combox.SelectedIndex == 1)
                        this.dataGridView1.Rows[row].Cells[6].Value = global::JSZW400.Properties.Resources.Gripper2;
                    else
                        this.dataGridView1.Rows[row].Cells[6].Value = global::JSZW400.Properties.Resources.Gripper1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //（这一步比较重要，如果不加，会导致selectedchanged事件一直触发）
        /// <summary>
        /// 离开combox时，把事件删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void combox_Leave(object sender, EventArgs e)
        {
            System.Windows.Forms.ComboBox combox = sender as System.Windows.Forms.ComboBox;
            //做完处理，须撤销动态事件
            combox.SelectedIndexChanged -= new EventHandler(ComboBox_SelectedIndexChanged);
        }





        private void btnManFoldMode_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[45] = !MainFrm.Hmi_bArray[45];
            mf.AdsWritePlc1Bit(45, MainFrm.Hmi_bArray[45]);
            MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 1] = Convert.ToSingle(MainFrm.Hmi_bArray[45]);
            mf.wrtConfigFile("[ManualOldSelect]", 1);
        }

        private void btn基准分条折弯_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[46] = !MainFrm.Hmi_bArray[46];
            btn基准分条折弯.Image = MainFrm.Hmi_bArray[46] ? global::JSZW400.Properties.Resources.btm_2档开关1 : global::JSZW400.Properties.Resources.btm_2档开关0;
            mf.AdsWritePlc1Bit(46, MainFrm.Hmi_bArray[46]);
            MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 2] = Convert.ToSingle(MainFrm.Hmi_bArray[46]);
            mf.wrtConfigFile("[ManualOldSelect]", 2);
        }

        private void btn整机初始化_MouseDown(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Name == "btn整机初始化") { MainFrm.Hmi_bArray[41] = true; mf.AdsWritePlc(); }
            else if (btn.Name == "btn后挡归原位") { MainFrm.Hmi_bArray[42] = true; mf.AdsWritePlc(); }
            else if (btn.Name == "btn翻板向下") { MainFrm.Hmi_bArray[43] = true; mf.AdsWritePlc(); }
            else if (btn.Name == "btn分条机归原位") { MainFrm.Hmi_bArray[44] = true; mf.AdsWritePlc(); }
            else if (btn.Name == "btn移动后挡块") { MainFrm.Hmi_bArray[47] = true; mf.AdsWritePlc(); }
            else if (btn.Name == "btn滑动顶部翻板") { MainFrm.Hmi_bArray[48] = true; mf.AdsWritePlc(); }
            else if (btn.Name == "btn滑动底部翻板") { MainFrm.Hmi_bArray[49] = true; mf.AdsWritePlc(); }
            else if (btn.Name == "btn滑动双向翻板") { MainFrm.Hmi_bArray[50] = true; mf.AdsWritePlc(); }
            else if (btn.Name == "btn加载桌板") { MainFrm.Hmi_bArray[51] = true; mf.AdsWritePlc(); }
        }

        private void btn整机初始化_MouseUp(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Name == "btn整机初始化") { MainFrm.Hmi_bArray[41] = false; mf.AdsWritePlc(); }
            else if (btn.Name == "btn后挡归原位") { MainFrm.Hmi_bArray[42] = false; mf.AdsWritePlc(); }
            else if (btn.Name == "btn翻板向下") { MainFrm.Hmi_bArray[43] = false; mf.AdsWritePlc(); }
            else if (btn.Name == "btn分条机归原位") { MainFrm.Hmi_bArray[44] = false; mf.AdsWritePlc(); }
            else if (btn.Name == "btn移动后挡块") { MainFrm.Hmi_bArray[47] = false; mf.AdsWritePlc(); }
            else if (btn.Name == "btn滑动顶部翻板") { MainFrm.Hmi_bArray[48] = false; mf.AdsWritePlc(); }
            else if (btn.Name == "btn滑动底部翻板") { MainFrm.Hmi_bArray[49] = false; mf.AdsWritePlc(); }
            else if (btn.Name == "btn滑动双向翻板") { MainFrm.Hmi_bArray[50] = false; mf.AdsWritePlc(); }
            else if (btn.Name == "btn加载桌板") { MainFrm.Hmi_bArray[51] = false; mf.AdsWritePlc(); }
        }

        private void btn下载到PLC_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < MainFrm.Hmi_iSemiAuto.Length; i++)
            {
                MainFrm.Hmi_iSemiAuto[i] = 0;
            }

            下载存生产数据();
            mf.AdsWritePlc_SemiAuto();
        }
        private void 下载存生产数据()
        {
            int idx = 0;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (Convert.ToInt16(dataGridView1[1, i].Tag) == 1)
                {
                    MainFrm.Hmi_iSemiAuto[i * 10 + 0] = 0;
                    MainFrm.Hmi_iSemiAuto[i * 10 + 1] = 0;
                }
                else if (Convert.ToInt16(dataGridView1[1, i].Tag) == 2)
                {
                    MainFrm.Hmi_iSemiAuto[i * 10 + 0] = 0;
                    MainFrm.Hmi_iSemiAuto[i * 10 + 1] = 1;
                }
                else if (Convert.ToInt16(dataGridView1[1, i].Tag) == 3)     //挤压
                {
                    MainFrm.Hmi_iSemiAuto[i * 10 + 0] = 1;
                    MainFrm.Hmi_iSemiAuto[i * 10 + 1] = MainFrm.Hmi_iSemiAuto[(i - 1) * 10 + 1];
                    /*
                    if (i > 0)
                        MainFrm.Hmi_iSemiAuto[i * 10 + 1] = MainFrm.Hmi_iSemiAuto[(i - 1) * 10 + 1];  //为前一个,暂先如此
                    else
                        MainFrm.Hmi_iSemiAuto[i * 10 + 1] = 1;   //如为第一个,暂设为1
                    */
                }
                else if (Convert.ToInt16(dataGridView1[1, i].Tag) == 4)     //开口挤压
                {
                    MainFrm.Hmi_iSemiAuto[i * 10 + 0] = 2;
                    MainFrm.Hmi_iSemiAuto[i * 10 + 1] = MainFrm.Hmi_iSemiAuto[(i - 1) * 10 + 1];
                }
                else if (Convert.ToInt16(dataGridView1[1, i].Tag) == 5)
                {
                    MainFrm.Hmi_iSemiAuto[i * 10 + 0] = 3;
                    MainFrm.Hmi_iSemiAuto[i * 10 + 1] = 0;
                }
                else if (Convert.ToInt16(dataGridView1[1, i].Tag) == 8)     //翻面
                {
                    MainFrm.Hmi_iSemiAuto[i * 10 + 0] = 8;
                    MainFrm.Hmi_iSemiAuto[i * 10 + 1] = 1;
                }

                string str = Convert.ToString(dataGridView1[2, i].Value).Replace("°", "").Trim();
                string str1 = Convert.ToString(dataGridView1[4, i].Value).Replace("°", "").Trim();
                if (str == "分切" || str == "SLIT")
                {
                    MainFrm.Hmi_iSemiAuto[i * 10 + 2] = 8880;
                    MainFrm.Hmi_iSemiAuto[i * 10 + 3] = 0;
                }
                else if (str == "挤压" || str == "SQUASH")
                {
                    MainFrm.Hmi_iSemiAuto[i * 10 + 2] = 10;
                    MainFrm.Hmi_iSemiAuto[i * 10 + 3] = 0;
                }
                else if (str == "开口挤压" || str == "OPEN SQUASH")
                {
                    MainFrm.Hmi_iSemiAuto[i * 10 + 2] = 10;
                    MainFrm.Hmi_iSemiAuto[i * 10 + 3] = 0;
                }
                else if (str == "翻面" || str == "FLIP")
                {
                    MainFrm.Hmi_iSemiAuto[i * 10 + 2] = 80;
                    MainFrm.Hmi_iSemiAuto[i * 10 + 3] = 0;
                }
                else
                {
                    MainFrm.Hmi_iSemiAuto[i * 10 + 2] = Convert.ToInt16(Convert.ToDouble(str) * 10);
                    MainFrm.Hmi_iSemiAuto[i * 10 + 3] = Convert.ToInt16(Convert.ToDouble(str1) * 10);
                }
                if (idx < MainFrm.CurtOrder.lstSemiAuto.Count)
                    MainFrm.Hmi_iSemiAuto[i * 10 + 9] = Convert.ToInt16(MainFrm.CurtOrder.lstSemiAuto[idx].锥度斜率);
                else
                    MainFrm.Hmi_iSemiAuto[i * 10 + 9] = 0;
                idx++;
                string str2 = Convert.ToString(dataGridView1[5, i].Value).Replace("mm", "").Trim();
                MainFrm.Hmi_iSemiAuto[i * 10 + 4] = Convert.ToInt16(Convert.ToDouble(dataGridView1[5, i].Value) * 10);

                string s = ((DataGridViewComboBoxCell)this.dataGridView1.Rows[i].Cells[7]).Value.ToString();
                if (s == "推动" || s == "PUSH") MainFrm.Hmi_iSemiAuto[i * 10 + 5] = 0;
                else if (s == "抓取" || s == "GRIP") MainFrm.Hmi_iSemiAuto[i * 10 + 5] = 1;
                else if (s == "超程抓取" || s == "OVERGRIP") MainFrm.Hmi_iSemiAuto[i * 10 + 5] = 2;

                string s2 = ((DataGridViewComboBoxCell)this.dataGridView1.Rows[i].Cells[8]).Value.ToString();
                if (s2 == "低" || s2 == "LOW") MainFrm.Hmi_iSemiAuto[i * 10 + 6] = 0;
                else if (s2 == "中" || s2 == "Medium") MainFrm.Hmi_iSemiAuto[i * 10 + 6] = 1;
                else if (s2 == "高" || s2 == "HIGH") MainFrm.Hmi_iSemiAuto[i * 10 + 6] = 2;
                else if (s2 == "最大" || s2 == "MAX") MainFrm.Hmi_iSemiAuto[i * 10 + 6] = 3;

                MainFrm.Hmi_iSemiAuto[i * 10 + 7] = Convert.ToInt16(Convert.ToInt16(((DataGridViewComboBoxCell)this.dataGridView1.Rows[i].Cells[9]).Value) - 1);

                if (dataGridView1.Rows[i].DefaultCellStyle.ForeColor == Color.Yellow)
                    MainFrm.Hmi_iSemiAuto[i * 10 + 8] = 1;
                else
                    MainFrm.Hmi_iSemiAuto[i * 10 + 8] = 0;
            }
        }

        string txtSemiData = "";
        //for (int i = 0; i<CurtOrder.lstSemiAuto.Count; i++)
        //    {
        //        str += CurtOrder.lstSemiAuto[i].行动类型.ToString() + "/" + CurtOrder.lstSemiAuto[i].折弯方向.ToString() + "/" + CurtOrder.lstSemiAuto[i].折弯角度.ToString() + "/";
        //        str += CurtOrder.lstSemiAuto[i].回弹值.ToString() + "/" + CurtOrder.lstSemiAuto[i].后挡位置.ToString() + "/" + CurtOrder.lstSemiAuto[i].抓取类型.ToString() + "/";
        //        str += CurtOrder.lstSemiAuto[i].松开高度.ToString() + "/" + CurtOrder.lstSemiAuto[i].翻板收缩值.ToString() + ",";
        //    }
        public void reGeSemiData()
        {
            CurtOrder.lstSemiAuto.Clear();
            SemiAutoType odrSemi = new SemiAutoType();

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                odrSemi.折弯序号 = (i + 1);
                if (Convert.ToInt16(dataGridView1[1, i].Tag) == 1)
                {
                    odrSemi.行动类型 = 0;
                    odrSemi.折弯方向 = 0;
                }
                else if (Convert.ToInt16(dataGridView1[1, i].Tag) == 2)
                {
                    odrSemi.行动类型 = 0;
                    odrSemi.折弯方向 = 1;
                }
                else if (Convert.ToInt16(dataGridView1[1, i].Tag) == 3)     //挤压
                {
                    odrSemi.行动类型 = 1;
                    if (i > 0)
                        odrSemi.折弯方向 = CurtOrder.lstSemiAuto[i - 1].折弯方向;     //为前一个,暂先如此
                    else
                        odrSemi.折弯方向 = 1;   //如为第一个,暂设为1
                }
                else if (Convert.ToInt16(dataGridView1[1, i].Tag) == 4)     //开口挤压
                {
                    odrSemi.行动类型 = 2;
                    if (i > 0)
                        odrSemi.折弯方向 = CurtOrder.lstSemiAuto[i - 1].折弯方向;     //为前一个,暂先如此
                    else
                        odrSemi.折弯方向 = 1;   //如为第一个,暂设为1
                }
                else if (Convert.ToInt16(dataGridView1[1, i].Tag) == 5)     //分条
                {
                    odrSemi.行动类型 = 3;
                    odrSemi.折弯方向 = 0;
                }
                else if (Convert.ToInt16(dataGridView1[1, i].Tag) == 8)     //翻面
                {
                    odrSemi.行动类型 = 8;
                    odrSemi.折弯方向 = 1;
                }

                string str = Convert.ToString(dataGridView1[2, i].Value).Replace("°", "").Trim();
                string str1 = Convert.ToString(dataGridView1[4, i].Value).Replace("°", "").Trim();
                if (str == "分切" || str == "SLIT" || str == "翻面" || str == "FLIP")
                {
                    odrSemi.折弯角度 = 888;
                    odrSemi.回弹值 = 0;
                }
                else if (str == "挤压" || str == "开口挤压" || str == "SQUASH" || str == "OPEN SQUASH" )
                {
                    odrSemi.折弯角度 = 1.0;
                    odrSemi.回弹值 = 0;
                }
                else
                {
                    odrSemi.折弯角度 = Convert.ToDouble(str);
                    odrSemi.回弹值 = Convert.ToDouble(str1);
                }
                string str2 = Convert.ToString(dataGridView1[5, i].Value).Replace("mm", "").Trim();
                odrSemi.后挡位置 = Convert.ToDouble(str2);
                odrSemi.锥度斜率 = Convert.ToDouble(dataGridView1[5, i].Tag);

                string s = ((DataGridViewComboBoxCell)this.dataGridView1.Rows[i].Cells[7]).Value.ToString();
                if (s == "推动" || s == "PUSH") odrSemi.抓取类型 = 0;
                else if (s == "抓取" || s == "GRIP") odrSemi.抓取类型 = 1;
                else if (s == "超程抓取" || s == "OVERGRIP") odrSemi.抓取类型 = 2;

                string s2 = ((DataGridViewComboBoxCell)this.dataGridView1.Rows[i].Cells[8]).Value.ToString();
                if (s2 == "低" || s2 == "LOW") odrSemi.松开高度 = 0;
                else if (s2 == "中" || s2 == "Medium") odrSemi.松开高度 = 1;
                else if (s2 == "高" || s2 == "HIGH") odrSemi.松开高度 = 2;
                else if (s2 == "最大" || s2 == "MAX") odrSemi.松开高度 = 3;


                odrSemi.翻板收缩值 = Convert.ToInt16(((DataGridViewComboBoxCell)this.dataGridView1.Rows[i].Cells[9]).Value) - 1;

                if (dataGridView1.Rows[i].DefaultCellStyle.ForeColor == Color.Yellow)
                    odrSemi.重新抓取 = 1;
                else
                    odrSemi.重新抓取 = 0;

                CurtOrder.lstSemiAuto.Add(odrSemi);
            }
        }


        private void sw继续步骤_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[61] = !MainFrm.Hmi_bArray[61];
            sw继续步骤.Image = MainFrm.Hmi_bArray[61] ? global::JSZW400.Properties.Resources.btm_2档开关1 : global::JSZW400.Properties.Resources.btm_2档开关0;
            mf.AdsWritePlc1Bit(61, MainFrm.Hmi_bArray[61]);
        }

        private void sw分条开关_Click(object sender, EventArgs e)
        {
            string s0 = "";
            if (!MainFrm.Hmi_bArray[62])
                s0 = (MainFrm.Lang == 0) ? "确定需要启用纵切吗？" : "You Want to Enable Slitter?";

            DialogAsk dlgTips = new DialogAsk("", s0);
            dlgTips.StartPosition = FormStartPosition.Manual;
            dlgTips.Location = new Point(500, 200);
            if ((!MainFrm.Hmi_bArray[62] && dlgTips.ShowDialog() == DialogResult.OK) || MainFrm.Hmi_bArray[62])
            {
                MainFrm.Hmi_bArray[62] = !MainFrm.Hmi_bArray[62];
                sw分条开关.Image = MainFrm.Hmi_bArray[62] ? global::JSZW400.Properties.Resources.btm_分条开关1 : global::JSZW400.Properties.Resources.btm_分条开关0;
                mf.AdsWritePlc1Bit(62, MainFrm.Hmi_bArray[62]);
            }
        }

        private void sw后档块开关_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[63] = !MainFrm.Hmi_bArray[63];
            sw后档块开关.BackgroundImage = MainFrm.Hmi_bArray[63] ? global::JSZW400.Properties.Resources.sw_左右小开关1 : global::JSZW400.Properties.Resources.sw_左右小开关0;
            mf.AdsWritePlc1Bit(63, MainFrm.Hmi_bArray[63]);
        }

        private void sw推动连续_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[64] = !MainFrm.Hmi_bArray[64];
            sw推动连续.BackgroundImage = MainFrm.Hmi_bArray[64] ? global::JSZW400.Properties.Resources.sw_左右小开关1 : global::JSZW400.Properties.Resources.sw_左右小开关0;
            mf.AdsWritePlc1Bit(64, MainFrm.Hmi_bArray[64]);
        }

        void 列表_更改序号及选中行(int SNstart, int selrow)
        {
            //更改序号并重新选择
            for (int j = SNstart; j < dataGridView1.RowCount; j++)
            {
                dataGridView1[0, j].Value = (j + 1).ToString() + ".";
            }
            dataGridView1.ClearSelection();
            dataGridView1.Rows[selrow].Selected = true;
            dataGridView1.CurrentCell = dataGridView1.Rows[selrow].Cells[0];
        }

        private void btn列表_之前插入_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                InsertRow(dataGridView1.CurrentRow.Index);
                列表_更改序号及选中行(dataGridView1.CurrentRow.Index, dataGridView1.CurrentRow.Index - 1);
            }
            else
            {
                InsertRow(0);
            }
            cancelMode();
        }

        private void btn列表_之后插入_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                InsertRow(dataGridView1.CurrentRow.Index + 1);
                列表_更改序号及选中行(dataGridView1.CurrentRow.Index, dataGridView1.CurrentRow.Index + 1);
            }
            else
            {
                InsertRow(0);
            }
            cancelMode();
        }

        private void btn列表_清除_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null || dataGridView1.CurrentRow.Index < 0)
                return;

            dataGridView1.Rows.Remove(dataGridView1.CurrentRow);
            if (dataGridView1.CurrentRow == null)       //删除之后为空,跳出
                return;
            列表_更改序号及选中行(dataGridView1.CurrentRow.Index, dataGridView1.CurrentRow.Index);
            cancelMode();
        }

        private void btn列表_上移_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridViewSelectedRowCollection dgvsrc = dataGridView1.SelectedRows;//获取选中行的集合
                if (dgvsrc.Count > 0)
                {
                    int index = dataGridView1.SelectedRows[0].Index;//获取当前选中行的索引
                    if (index > 0)//如果该行不是第一行
                    {
                        DataGridViewRow dgvr = dataGridView1.Rows[index - dgvsrc.Count];//获取选中行的上一行
                        dataGridView1.Rows.RemoveAt(index - dgvsrc.Count);//删除原选中行的上一行
                        dataGridView1.Rows.Insert((index), dgvr);//将选中行的上一行插入到选中行的后面
                        for (int i = 0; i < dgvsrc.Count; i++)//选中移动后的行
                        {
                            dataGridView1.Rows[index - i - 1].Selected = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            列表_更改序号及选中行(dataGridView1.CurrentRow.Index, dataGridView1.CurrentRow.Index);
            cancelMode();
        }

        private void btn列表_下移_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridViewSelectedRowCollection dgvsrc = dataGridView1.SelectedRows;//获取选中行的集合
                if (dgvsrc.Count > 0)
                {
                    int index = dataGridView1.SelectedRows[0].Index;//获取当前选中行的索引
                    if (index >= 0 & (dataGridView1.RowCount - 1) != index)//如果该行不是最后一行
                    {
                        DataGridViewRow dgvr = dataGridView1.Rows[index + 1];//获取选中行的下一行
                        dataGridView1.Rows.RemoveAt(index + 1);//删除原选中行的上一行
                        dataGridView1.Rows.Insert((index + 1 - dgvsrc.Count), dgvr);//将选中行的上一行插入到选中行的后面
                        for (int i = 0; i < dgvsrc.Count; i++)//选中移动后的行
                        {
                            dataGridView1.Rows[index + 1 - i].Selected = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            列表_更改序号及选中行(dataGridView1.CurrentRow.Index - 1, dataGridView1.CurrentRow.Index);
            cancelMode();
        }

        private void btn列表_复制_Click(object sender, EventArgs e)
        {
            DataGridViewRow dr = new DataGridViewRow();
            dr.CreateCells(dataGridView1);
            ((DataGridViewComboBoxCell)dr.Cells[7]).Items.Add(MainFrm.Lang == 0 ? "推动" : "PUSH");
            ((DataGridViewComboBoxCell)dr.Cells[7]).Items.Add(MainFrm.Lang == 0 ? "抓取" : "GRIP");
            ((DataGridViewComboBoxCell)dr.Cells[7]).Items.Add(MainFrm.Lang == 0 ? "超程抓取" : "OVERGRIP");
            ((DataGridViewComboBoxCell)dr.Cells[8]).Items.Add(MainFrm.Lang == 0 ? "低" : "LOW");
            ((DataGridViewComboBoxCell)dr.Cells[8]).Items.Add(MainFrm.Lang == 0 ? "中" : "Medium");
            ((DataGridViewComboBoxCell)dr.Cells[8]).Items.Add(MainFrm.Lang == 0 ? "高" : "HIGH");
            ((DataGridViewComboBoxCell)dr.Cells[8]).Items.Add(MainFrm.Lang == 0 ? "最大" : "MAX");
            ((DataGridViewComboBoxCell)dr.Cells[9]).Items.Add("1");
            ((DataGridViewComboBoxCell)dr.Cells[9]).Items.Add("2");
            ((DataGridViewComboBoxCell)dr.Cells[9]).Items.Add("3");
            ((DataGridViewComboBoxCell)dr.Cells[9]).Items.Add("4");

            int i = dataGridView1.SelectedCells[0].RowIndex;
            dr.Cells[0].Value = (dataGridView1.RowCount + 1).ToString();
            dr.Cells[1].Value = dataGridView1.Rows[i].Cells[1].Value;
            dr.Cells[2].Value = dataGridView1.Rows[i].Cells[2].Value;
            dr.Cells[3].Value = dataGridView1.Rows[i].Cells[3].Value;
            dr.Cells[4].Value = dataGridView1.Rows[i].Cells[4].Value;
            dr.Cells[5].Value = dataGridView1.Rows[i].Cells[5].Value;
            dr.Cells[6].Value = dataGridView1.Rows[i].Cells[6].Value;
            ((DataGridViewComboBoxCell)dr.Cells[7]).Value = ((DataGridViewComboBoxCell)dataGridView1.Rows[i].Cells[7]).Value;
            ((DataGridViewComboBoxCell)dr.Cells[8]).Value = ((DataGridViewComboBoxCell)dataGridView1.Rows[i].Cells[8]).Value;
            ((DataGridViewComboBoxCell)dr.Cells[9]).Value = ((DataGridViewComboBoxCell)dataGridView1.Rows[i].Cells[9]).Value;
            dr.Cells[10].Value = dataGridView1.Rows[i].Cells[10].Value;
            dr.DefaultCellStyle.ForeColor = dataGridView1.Rows[i].DefaultCellStyle.ForeColor;
            dr.DefaultCellStyle.SelectionForeColor = dataGridView1.Rows[i].DefaultCellStyle.SelectionForeColor;

            //添加的行作为第一行
            dataGridView1.Rows.Add(dr);
            cancelMode();
        }

        private void txb手动折弯角_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                MainFrm.Hmi_rArray[50] = Convert.ToSingle(txb手动折弯角.Text);
                mf.AdsWritePlc1float(50, MainFrm.Hmi_rArray[50]);
                MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 3] = MainFrm.Hmi_rArray[50];
                mf.wrtConfigFile("[ManualOldSelect]", 3);

            }
        }

        private void txb后挡块定位_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                MainFrm.Hmi_rArray[51] = Convert.ToSingle(txb后挡块定位.Text);
                mf.AdsWritePlc1float(51, MainFrm.Hmi_rArray[51]);
                MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 4] = MainFrm.Hmi_rArray[51];
                mf.wrtConfigFile("[ManualOldSelect]", 4);
            }
        }

        private void txb顶部翻板定位_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                MainFrm.Hmi_rArray[52] = Convert.ToSingle(txb顶部翻板定位.Text);
                mf.AdsWritePlc1float(52, MainFrm.Hmi_rArray[52]);
                MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 5] = MainFrm.Hmi_rArray[52];
                mf.wrtConfigFile("[ManualOldSelect]", 5);
            }
        }

        private void txb底部翻板定位_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                MainFrm.Hmi_rArray[53] = Convert.ToSingle(txb底部翻板定位.Text);
                mf.AdsWritePlc1float(53, MainFrm.Hmi_rArray[53]);
                MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 6] = MainFrm.Hmi_rArray[53];
                mf.wrtConfigFile("[ManualOldSelect]", 6);
            }
        }

        private void txb桌板定位_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                MainFrm.Hmi_rArray[54] = Convert.ToSingle(txb桌板定位.Text);
                mf.AdsWritePlc1float(54, MainFrm.Hmi_rArray[54]);
                MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 7] = MainFrm.Hmi_rArray[54];
                mf.wrtConfigFile("[ManualOldSelect]", 7);
            }
        }

        private void SubOPManual_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {

                txb手动折弯角.Text = MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 3].ToString();
                txb底部翻板定位.Text = MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 4].ToString();
                txb顶部翻板定位.Text = MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 5].ToString();
                txb桌板定位.Text = MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 5].ToString();
            }
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            ;
        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            ;
        }

        bool bflg = false;

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                pnlQuickInput.Visible = true;
            }
            if (e.ColumnIndex == 5)
            {
                bflg = true;
                txbActiveTxb = (DataGridViewTextBoxCell)dataGridView1.Rows[e.RowIndex].Cells[5];
                PopCal();
            }
        }



        private void btn挤压_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
                return;
            this.dataGridView1.CurrentRow.Cells[1].Tag = 3;
            this.dataGridView1.CurrentRow.Cells[1].Value = global::JSZW400.Properties.Resources.Squash;
            this.dataGridView1.CurrentRow.Cells[2].Value = MainFrm.Lang == 0 ? "挤压" : "SQUASH";
            this.dataGridView1.CurrentRow.Cells[3].Value = global::JSZW400.Properties.Resources.null0;
            this.dataGridView1.CurrentRow.Cells[4].Value = "";
            cancelMode();
        }
        private void btn开口挤压_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
                return;
            this.dataGridView1.CurrentRow.Cells[1].Tag = 4;
            this.dataGridView1.CurrentRow.Cells[1].Value = global::JSZW400.Properties.Resources.SquashOpen;
            this.dataGridView1.CurrentRow.Cells[2].Value = MainFrm.Lang == 0 ? "开口挤压" : "OPEN SQUASH";
            this.dataGridView1.CurrentRow.Cells[3].Value = global::JSZW400.Properties.Resources.null0;
            this.dataGridView1.CurrentRow.Cells[4].Value = "";
            cancelMode();
        }

        private void btn分条_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
                return;
            this.dataGridView1.CurrentRow.Cells[1].Tag = 5;
            this.dataGridView1.CurrentRow.Cells[1].Value = global::JSZW400.Properties.Resources.Slit7;
            this.dataGridView1.CurrentRow.Cells[2].Value = MainFrm.Lang == 0 ? "分切" : "SLIT";
            this.dataGridView1.CurrentRow.Cells[3].Value = global::JSZW400.Properties.Resources.null0;
            this.dataGridView1.CurrentRow.Cells[4].Value = "";
            this.dataGridView1.CurrentRow.Cells[5].Value = MainFrm.CurtOrder.Width.ToString();


            this.dataGridView1.CurrentRow.Cells[6].Value = dataGridView1.Rows[dataGridView1.CurrentRow.Index + 1].Cells[6].Value;
            ((DataGridViewComboBoxCell)dataGridView1.CurrentRow.Cells[7]).Value = ((DataGridViewComboBoxCell)dataGridView1.Rows[dataGridView1.CurrentRow.Index + 1].Cells[7]).Value;
            cancelMode();
        }

        private void btn翻面_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
                return;
            this.dataGridView1.CurrentRow.Cells[1].Tag = 8;
            this.dataGridView1.CurrentRow.Cells[1].Value = global::JSZW400.Properties.Resources.Flip;
            this.dataGridView1.CurrentRow.Cells[2].Value = MainFrm.Lang == 0 ? "翻面" : "FLIP";
            this.dataGridView1.CurrentRow.Cells[3].Value = global::JSZW400.Properties.Resources.null0;
            this.dataGridView1.CurrentRow.Cells[4].Value = "";
            this.dataGridView1.CurrentRow.Cells[5].Value = dataGridView1.Rows[dataGridView1.CurrentRow.Index - 1].Cells[5].Value;


            this.dataGridView1.CurrentRow.Cells[6].Value = dataGridView1.Rows[dataGridView1.CurrentRow.Index - 1].Cells[6].Value;
            ((DataGridViewComboBoxCell)dataGridView1.CurrentRow.Cells[7]).Value = ((DataGridViewComboBoxCell)dataGridView1.Rows[dataGridView1.CurrentRow.Index - 1].Cells[7]).Value;
            cancelMode();
        }

        int old步序 = 0;
        private void timer500ms_Tick(object sender, EventArgs e)
        {
            //if (dataGridView1.Rows.Count >0)
            //    label30.Text = Convert.ToString(dataGridView1.CurrentRow.Index);
            txb顶部翻板val.Text = MainFrm.Hmi_rArray[27].ToString("f1");
            txb底部翻板val.Text = MainFrm.Hmi_rArray[29].ToString("f1");
            txb桌板定位val.Text = MainFrm.Hmi_rArray[26].ToString("f1");
            txb计数.Text = Hmi_iArray[62].ToString();
            txb下一步序.Text = (MainFrm.Hmi_iArray[61] + 2).ToString();

            //this.Invoke(new Action(() =>
            //{
            //    if (b1 && dataGridView1.Rows.Count > 0 && (MainFrm.Hmi_iArray[61]) < dataGridView1.Rows.Count)
            //    {
            //        dataGridView1.ClearSelection();
            //        dataGridView1.Rows[MainFrm.Hmi_iArray[61]].Selected = true;
            //        b1 = false;
            //    }
            //}));


            if (MainFrm.Hmi_iArray[61] != old步序 && dataGridView1.Rows.Count > 0 && (MainFrm.Hmi_iArray[61]) < dataGridView1.Rows.Count)
            {
                //dataGridView1.CurrentCell = null;
                dataGridView1.ClearSelection();
                dataGridView1.Rows[MainFrm.Hmi_iArray[61]].Selected = true;
                old步序 = MainFrm.Hmi_iArray[61];
            }

            pnlManual.BackgroundImage = MainFrm.Hmi_iArray[20] == 3 ? global::JSZW400.Properties.Resources.Manual1 : global::JSZW400.Properties.Resources.Manual;
            txb手动折弯角.Visible = MainFrm.Hmi_bArray[45];
            label5.Visible = MainFrm.Hmi_bArray[45];

            btnManFoldMode.BackgroundImage = MainFrm.Hmi_bArray[45] ? global::JSZW400.Properties.Resources.sw_左右小开关1 : global::JSZW400.Properties.Resources.sw_左右小开关0;
            lb折弯模式自动.ForeColor = MainFrm.Hmi_bArray[45] ? Color.FromArgb(96, 176, 255) : Color.White;

            btn基准分条折弯.Image = MainFrm.Hmi_bArray[46] ? global::JSZW400.Properties.Resources.btm_2档开关1 : global::JSZW400.Properties.Resources.btm_2档开关0;
            lb分条点.ForeColor = MainFrm.Hmi_bArray[46] ? Color.FromArgb(96, 176, 255) : Color.White;
            lb折弯点.ForeColor = !MainFrm.Hmi_bArray[46] ? Color.FromArgb(96, 176, 255) : Color.White;

            pnl半自动开关.BackgroundImage = MainFrm.Hmi_iArray[20] == 4 ? global::JSZW400.Properties.Resources.Bike1 : global::JSZW400.Properties.Resources.Bike;

            sw后档块开关.BackgroundImage = MainFrm.Hmi_bArray[63] ? global::JSZW400.Properties.Resources.sw_左右小开关1 : global::JSZW400.Properties.Resources.sw_左右小开关0;
            lb后挡块开关.Text = MainFrm.Lang == 0 ? (MainFrm.Hmi_bArray[63] ? "开" : "关") : (MainFrm.Hmi_bArray[63] ? "ON" : "OFF");
            lb后挡块开关.ForeColor = MainFrm.Hmi_bArray[63] ? Color.FromArgb(96, 176, 255) : Color.White;

            sw推动连续.BackgroundImage = MainFrm.Hmi_bArray[64] ? global::JSZW400.Properties.Resources.sw_左右小开关1 : global::JSZW400.Properties.Resources.sw_左右小开关0;
            lb重复开关.Text = MainFrm.Lang == 0 ? (MainFrm.Hmi_bArray[64] ? "开" : "关") : (MainFrm.Hmi_bArray[64] ? "ON" : "OFF");
            lb重复开关.ForeColor = MainFrm.Hmi_bArray[64] ? Color.FromArgb(96, 176, 255) : Color.White;

            sw继续步骤.Image = MainFrm.Hmi_bArray[61] ? global::JSZW400.Properties.Resources.btm_2档开关1 : global::JSZW400.Properties.Resources.btm_2档开关0;
            lb连续.ForeColor = MainFrm.Hmi_bArray[61] ? Color.FromArgb(96, 176, 255) : Color.White;
            lb步骤.ForeColor = !MainFrm.Hmi_bArray[61] ? Color.FromArgb(96, 176, 255) : Color.White;

            sw分条开关.Image = MainFrm.Hmi_bArray[62] ? global::JSZW400.Properties.Resources.btm_分条开关1 : global::JSZW400.Properties.Resources.btm_分条开关0;
            lb分条开.ForeColor = MainFrm.Hmi_bArray[62] ? Color.FromArgb(96, 176, 255) : Color.White;
            lb分条关.ForeColor = !MainFrm.Hmi_bArray[62] ? Color.FromArgb(96, 176, 255) : Color.White;

            sw桌板开关.Image = MainFrm.Hmi_bArray[66] ? global::JSZW400.Properties.Resources.btm_2档开关1 : global::JSZW400.Properties.Resources.btm_2档开关0;
            lb桌板打开.ForeColor = MainFrm.Hmi_bArray[66] ? Color.FromArgb(96, 176, 255) : Color.White;
            lb桌板关闭.ForeColor = !MainFrm.Hmi_bArray[66] ? Color.FromArgb(96, 176, 255) : Color.White;

            if (MainFrm.CurtOrder.Name == "")
                label12.Text = "";
            else
                label12.Text = "Name: " + MainFrm.CurtOrder.Name + "  Length: " + MainFrm.CurtOrder.Width;


        }


        private void pnlManual_MouseDown(object sender, MouseEventArgs e)
        {
            MainFrm.Hmi_bArray[40] = true;
            mf.AdsWritePlc1Bit(40, MainFrm.Hmi_bArray[40]);
        }

        private void pnlManual_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_iArray[0] = 3;
            //MainFrm.Hmi_bArray[40] = true;
            //MainFrm.Hmi_bArray[60] = false;
            mf.AdsWritePlc1Int(0, MainFrm.Hmi_iArray[0]);
        }

        private void pnlManual_MouseUp(object sender, MouseEventArgs e)
        {
            MainFrm.Hmi_bArray[40] = false;
            mf.AdsWritePlc1Bit(40, MainFrm.Hmi_bArray[40]);
        }

        private void pnl半自动开关_MouseDown(object sender, MouseEventArgs e)
        {
            if (!SlitterValid())
                return;

            MainFrm.Hmi_bArray[61] = false;
            mf.AdsWritePlc1Bit(40, MainFrm.Hmi_bArray[40]);
            MainFrm.Hmi_iArray[0] = 4;
            mf.AdsWritePlc1Int(0, MainFrm.Hmi_iArray[0]);

        }

        private void pnl半自动开关_MouseUp(object sender, MouseEventArgs e)
        {
            if (!SlitterValid())
                return;

            MainFrm.Hmi_bArray[60] = true;
            mf.AdsWritePlc1Bit(60, MainFrm.Hmi_bArray[60]);
        }

        private void pnl半自动开关_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < MainFrm.Hmi_iSemiAuto.Length; i++)
            {
                MainFrm.Hmi_iSemiAuto[i] = 0;
            }

            下载存生产数据();

            if (!SlitterValid())
                return;

            mf.AdsWritePlc_SemiAuto();

            //MainFrm.Hmi_bArray[60] = false;
            //mf.AdsWritePlc1Bit(60, MainFrm.Hmi_bArray[60]);
            //MainFrm.Hmi_iArray[0] = 4;
            ////MainFrm.Hmi_bArray[40] = false;
            ////MainFrm.Hmi_bArray[60] = true;
            //mf.AdsWritePlc1Int(0, MainFrm.Hmi_iArray[0]);
        }

        private bool SlitterValid()
        {
            下载存生产数据();
            if ((Convert.ToInt16(dataGridView1[1, 0].Tag) == 5 && MainFrm.Hmi_bArray[62])
                    || (MainFrm.Hmi_iSemiAuto[0] != 3 && !MainFrm.Hmi_bArray[62]))
                return true;
            else
            {
                mf.ShowTips(1, 0);
                return false;

            }


        }

        private void sw桌板开关_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[66] = !MainFrm.Hmi_bArray[66];
            sw桌板开关.Image = MainFrm.Hmi_bArray[66] ? global::JSZW400.Properties.Resources.btm_2档开关1 : global::JSZW400.Properties.Resources.btm_2档开关0;
            mf.AdsWritePlc1Bit(66, MainFrm.Hmi_bArray[66]);
        }
        private FrmCalculator dlgCal = null;
        private void txb后挡块定位_Click(object sender, EventArgs e)
        {
            //FrmCalculator dlgParaMachine = new FrmCalculator(this);
            //dlgParaMachine.StartPosition = FormStartPosition.Manual;
            //dlgParaMachine.Location = new Point(945, 30);
            //dlgParaMachine.TopMost= true;
            //dlgParaMachine.Show();
            ////float f = Convert.ToSingle(PopCal(945, 30, txb后挡块定位.Text));
            ////txb后挡块定位.Text = f.ToString("F2");
            //txbActiveTxb = txb后挡块定位;
            //txb后挡块定位.Focus();
            txbActiveTxb = txb后挡块定位;
            PopCal();
        }

        public string InCal = "";
        private string PopCal(int cx, int cy, string val)
        {
            string str1 = "";
            FrmCalculator dlgParaMachine = new FrmCalculator(this);
            dlgParaMachine.StartPosition = FormStartPosition.Manual;
            dlgParaMachine.Location = new Point(cx, cy);

            if (dlgParaMachine.ShowDialog() == DialogResult.OK)
            {
                if (dlgParaMachine.getval() == "")
                    str1 = val;
                else
                    str1 = dlgParaMachine.getval();
            }
            else
            {
                str1 = val;
            }
            dlgParaMachine.Dispose();
            return str1;
        }

        private Object txbActiveTxb;
        int keystep = 0;
        public void sendkey(string InCal0)
        {
            if (txbActiveTxb is TextBox TB)
            {
                TB.Focus();
                if (InCal0 == "CLR")
                {
                    TB.Text = "0";
                }
                else if (InCal0 == "ENTER")
                {
                    SendKeys.Send("{" + InCal0 + "}");
                }
                else
                {
                    SendKeys.Send("{" + InCal0 + "}");
                }

            }

            if (txbActiveTxb is DataGridViewTextBoxCell DTB)
            {
                //dataGridView1.CurrentCell = DTB;
                //dataGridView1.BeginEdit(false);
                string s = DTB.Value.ToString();
                if (InCal0 == "CLR")
                {
                    DTB.Value = "0";
                }
                else if (InCal0 == "ENTER")
                {
                    keystep = 0;
                    ;
                }
                else if (InCal0 == "BACKSPACE")
                {
                    DTB.Value = s.Substring(0, s.Length - 1);
                }
                else if (InCal0 == "-" && s.Substring(0, 1) == "-")
                {
                    DTB.Value = s.Substring(1);
                }
                else if (InCal0 == "-" && s.Substring(0, 1) != "-")
                {
                    DTB.Value = "-" + DTB.Value;
                }
                else if (s.Substring(0, 1) == "0")
                {
                    DTB.Value = s.Substring(1) + InCal0;       //去掉第一个0
                }
                else
                //DTB.Value += InCal0;
                {
                    string s1 = "";

                    dataGridView1.EndEdit();

                    if (keystep == 0)
                        s1 = "";
                    else
                        s1 = DTB.Value.ToString();
                    //MessageBox.Show(s1);
                    keystep++;
                    var arr = s1.ToArray();
                    foreach (var c in arr)
                    {
                        SendKeys.Send("{" + c + "}");
                    }

                    SendKeys.Send("{" + InCal0 + "}");
                    //timer1.Enabled = true;

                }

                //DTB.Value = "9";
                dataGridView1.EndEdit();
            }


        }

        private void txb顶部翻板定位_Click(object sender, EventArgs e)
        {
            txbActiveTxb = txb顶部翻板定位;
            //txbActiveTxb = (DataGridViewTextBoxCell)dataGridView1.Rows[1].Cells[7];
            PopCal();
        }

        private void PopCal()
        {
            if (dlgCal == null || dlgCal.IsDisposed)
            {
                dlgCal = new FrmCalculator(this);
                dlgCal.StartPosition = FormStartPosition.Manual;
                dlgCal.Location = new Point(945, 30);
                dlgCal.TopMost = true;
                dlgCal.Show();
            }
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            dlgCal.Dispose();
        }

        private void button12_MouseDown(object sender, MouseEventArgs e)
        {
            Hmi_bArray[52] = true;
            mf.AdsWritePlc();
        }

        private void button12_MouseUp(object sender, MouseEventArgs e)
        {
            Hmi_bArray[52] = false;
            mf.AdsWritePlc();
        }

        private void button13_MouseDown(object sender, MouseEventArgs e)
        {
            Hmi_bArray[53] = true;
            mf.AdsWritePlc();
        }

        private void button13_MouseUp(object sender, MouseEventArgs e)
        {
            Hmi_bArray[53] = false;
            mf.AdsWritePlc();
        }

        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void button15_MouseDown(object sender, MouseEventArgs e)
        {
            Hmi_bArray[101] = true;
            mf.AdsWritePlc();
        }

        private void button15_MouseUp(object sender, MouseEventArgs e)
        {
            Hmi_bArray[101] = false;
            mf.AdsWritePlc();
        }

        private void button14_MouseDown(object sender, MouseEventArgs e)
        {
            Hmi_bArray[102] = true;
            mf.AdsWritePlc();
        }

        private void button14_MouseUp(object sender, MouseEventArgs e)
        {
            Hmi_bArray[102] = false;
            mf.AdsWritePlc();
        }

        private void button16_MouseDown(object sender, MouseEventArgs e)
        {
            Hmi_bArray[103] = true;
            mf.AdsWritePlc();
        }

        private void button16_MouseUp(object sender, MouseEventArgs e)
        {
            Hmi_bArray[103] = false;
            mf.AdsWritePlc();
        }

        private void button17_MouseDown(object sender, MouseEventArgs e)
        {
            Hmi_bArray[104] = true;
            mf.AdsWritePlc();
        }

        private void button17_MouseUp(object sender, MouseEventArgs e)
        {
            Hmi_bArray[104] = false;
            mf.AdsWritePlc();
        }

        private void button18_MouseDown(object sender, MouseEventArgs e)
        {
            Hmi_bArray[105] = true;
            mf.AdsWritePlc();
        }

        private void button18_MouseUp(object sender, MouseEventArgs e)
        {
            Hmi_bArray[105] = false;
            mf.AdsWritePlc();
        }

        private void button19_MouseDown(object sender, MouseEventArgs e)
        {
            Hmi_bArray[106] = true;
            mf.AdsWritePlc();
        }

        private void button19_MouseUp(object sender, MouseEventArgs e)
        {
            Hmi_bArray[106] = false;
            mf.AdsWritePlc();
        }

        private void button20_MouseDown(object sender, MouseEventArgs e)
        {
            Hmi_bArray[107] = true;
            mf.AdsWritePlc();
        }

        private void button20_MouseUp(object sender, MouseEventArgs e)
        {
            Hmi_bArray[107] = false;
            mf.AdsWritePlc();
        }

        private void button21_MouseDown(object sender, MouseEventArgs e)
        {
            Hmi_bArray[108] = true;
            mf.AdsWritePlc();
        }

        private void button21_MouseUp(object sender, MouseEventArgs e)
        {
            Hmi_bArray[108] = false;
            mf.AdsWritePlc();
        }

        private void btn装载材料_MouseDown(object sender, MouseEventArgs e)
        {
            Hmi_bArray[67] = true;
            mf.AdsWritePlc1Bit(67, Hmi_bArray[67]);
        }

        private void btn装载材料_MouseUp(object sender, MouseEventArgs e)
        {
            Hmi_bArray[67] = false;
            mf.AdsWritePlc1Bit(67, Hmi_bArray[67]);
        }

        private void btn重置计数_MouseDown(object sender, MouseEventArgs e)
        {
            Hmi_bArray[65] = true;
            mf.AdsWritePlc1Bit(65, Hmi_bArray[65]);
        }

        private void btn重置计数_MouseUp(object sender, MouseEventArgs e)
        {
            Hmi_bArray[65] = false;
            mf.AdsWritePlc1Bit(65, Hmi_bArray[65]);
        }
        private void btmClrCount_Click(object sender, EventArgs e)
        {
            Hmi_iArray[62] = 0;
        }

        private void txb底部翻板定位_Click(object sender, EventArgs e)
        {
            txbActiveTxb = txb底部翻板定位;
            PopCal();
        }

        private void txb手动折弯角_Click(object sender, EventArgs e)
        {
            txbActiveTxb = txb手动折弯角;
            PopCal();
        }

        private void txb桌板定位_Click(object sender, EventArgs e)
        {
            txbActiveTxb = txb桌板定位;
            PopCal();
        }

        private void btn列表_切换重新抓取_Click(object sender, EventArgs e)
        {
            int i = dataGridView1.CurrentRow.Index;
            if (dataGridView1.Rows[i].DefaultCellStyle.ForeColor == Color.Yellow)
            {
                dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                dataGridView1.Rows[i].DefaultCellStyle.SelectionForeColor = Color.White;
            }
            else
            {
                dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.Yellow;
                dataGridView1.Rows[i].DefaultCellStyle.SelectionForeColor = Color.Yellow;
            }
        }

        private void dataGridView1_CellErrorTextNeeded(object sender, DataGridViewCellErrorTextNeededEventArgs e)
        {

        }

        private void btnPreAngle1_Click(object sender, EventArgs e)
        {



            string s = "";

            System.Windows.Forms.Button btn = (System.Windows.Forms.Button)sender;
            //if (btn.Name == "btnPreAngle1") { textBox1.Text = "{3}"; }
            //else if (btn.Name == "btnPreAngle2") { textBox1.Text = "{4}"; }
            //else if (btn.Name == "btnPreAngle3") { textBox1.Text = "{5}"; }
            //else if (btn.Name == "btnPreAngle4") { textBox1.Text = "{6}"; }
            //else if (btn.Name == "btnPreAngle5") { textBox1.Text = "{7}"; }
            //else if (btn.Name == "btnPreAngle6") { textBox1.Text = "{8}"; }
            //else if (btn.Name == "btnPreAngle7") { textBox1.Text = "{9}"; }
            //else if (btn.Name == "btnPreAngle8") { textBox1.Text = "{.}"; }
            //else if (btn.Name == "btnPreAngle9") { textBox1.Text = "{CLR}"; }
            //else if (btn.Name == "btnPreAngle10") { textBox1.Text = "{BACKSPACE}"; }
            //else if (btn.Name == "btnPreAngle11") { textBox1.Text = "-"; }
            //else if (btn.Name == "btnPreAngle12") { textBox1.Text = "-"; }


            dataGridView1.CurrentRow.Cells[2].Value = btn.Text.ToString();
            pnlQuickInput.Visible = false;
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txb开始序_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                MainFrm.Hmi_iArray[60] = Convert.ToInt16(txb开始序.Text);
                mf.AdsWritePlc1Int(60, MainFrm.Hmi_iArray[60]);
                cancelMode();

            }
        }

        private void cancelMode()   //取消半自动模式,进入手动模式
        {
            MainFrm.Hmi_iArray[0] = 3;
            mf.AdsWritePlc1Int(0, MainFrm.Hmi_iArray[0]);
        }

        private void btn挡块_MouseDown(object sender, MouseEventArgs e)
        {
            Hmi_bArray[87] = true;
            mf.AdsWritePlc();
        }

        private void btn挡块_MouseUp(object sender, MouseEventArgs e)
        {
            Hmi_bArray[87] = false;
            mf.AdsWritePlc();
        }

        
    }
}
