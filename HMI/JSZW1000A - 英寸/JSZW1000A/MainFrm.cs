using System.Collections;
using System.Globalization;
using System.Text;
using TwinCAT.Ads;

namespace JSZW1000A
{
    public partial class MainFrm : Form
    {
        public struct AngleAddit
        {
            public string Type = "", Material = "", Strength = "", MachingGauging = "";
            public string Thickness = "";
            public float[] AngleRange = new float[50];      //前20个下翻补偿,后20个上翻补偿
            public AngleAddit()
            {
                for (int i = 0; i < 50; i++)
                {
                    AngleRange[i] = 0;
                }
            }
            public void setAngleRange(int i, float val)
            {
                AngleRange[i] = val;
            }
        }
        public static AngleAddit[] angleAddit = new AngleAddit[400];
        public static float SpringTop = 0, SpringBtm = 0;
        public static int Lang = 0;
        public struct LengAngle
        {
            public double Length;
            public double Angle;
            public double TaperWidth;
            public bool YinYang;
            public bool isRayAngle;
            public double RayAngle_R, RayAngle_Num;

        }
        public struct SemiAutoType
        {
            //public int UnClampHt, FoldType, ApronRetract, GripType;
            //public float Angle, Pos, SpringBack;
            public int 折弯序号;
            public int 行动类型, 松开高度, 折弯方向, 翻板收缩值, 抓取类型;
            public double 折弯角度, 后挡位置, 回弹值;
            public int 长角序号;    //0:无挤压 1:首挤压 99:尾挤压
            public int 坐标序号;
            public int 重新抓取;
            public bool is色下;
            public double 锥度斜率;
            public int 操作提示;    //0:不需要操作 1:翻面
            public int 内外选择;    //0:A-B(A在外侧)     1:B-A (B在外侧)
        }

        public struct OrderType
        {
            public double TopSpring = 0.0, BtmSpring = 0.0;
            public bool isSlitter = false;
            public bool 边做边分切启用 = false;
            public bool st逆序 = false, st色下 = false;
            public double SlitterWid = 0;
            public double 边做边分切整板宽 = 0.0, 边做边分切前废料 = 0.0;
            public int 边做边分切块数 = 0;
            public string Name = "", Customer = "", MaterialName = "", Remark = "";
            public double Width = 0.0, Thickness = 0.0, SheetLength = 0.0;
            public LengAngle[] lengAngle = new LengAngle[100];
            public List<PointF> pxList = new List<PointF>();
            public List<SemiAutoType> lstSemiAuto = new List<SemiAutoType>();
            public bool isTaper = false;
            public double TaperWidth = 0.0, TaperLength = 0.0;
            public bool 显示朝向已初始化 = false;
            public double 显示起始角度 = 180.0;
            public bool 生产序列已生成 = false;
            public bool 半自动步骤已手动编辑 = false;

            public OrderType()
            {
                for (int i = 0; i < 100; i++)
                {
                    lengAngle[i].Length = 0; lengAngle[i].Angle = 0; lengAngle[i].YinYang = false;
                    lengAngle[i].TaperWidth = 0.0;
                }
            }
            public void clr()
            {
                Name = ""; Customer = ""; MaterialName = ""; Remark = "";
                Width = 0.0; Thickness = 0.0; SheetLength = 0.0; SlitterWid = 0;
                边做边分切启用 = false;
                边做边分切整板宽 = 0.0;
                边做边分切前废料 = 0.0;
                边做边分切块数 = 0;
                pxList.Clear();
                lstSemiAuto.Clear();
                显示朝向已初始化 = false;
                显示起始角度 = 180.0;
                半自动步骤已手动编辑 = false;
                for (int i = 0; i < 100; i++)
                {
                    lengAngle[i].Length = 0; lengAngle[i].Angle = 0; lengAngle[i].YinYang = false;
                    lengAngle[i].TaperWidth = 0.0;
                }

            }
        }
        public static List<OrderType> GblOrder = new List<OrderType>();
        public static OrderType CurtOrder = new OrderType();
        public static List<PointF> QuickDrawList = new List<PointF>();

        public static double[] SlopeAngle = new double[100];

        public SubWindows.SubOPManual subOPManual;
        public SubWindows.SubOPSetting subOPSetting;
        public SubWindows.SubOPSlitter subOPSlitter;
        public SubWindows.SubOPLibrary subOPLibrary;
        public SubWindows.SubOPAutoSet subOPAutoSet;
        public SubWindows.SubOPAuto subOPAuto1;
        public SubWindows.SubOPAutoDraw subOPAutoDraw;
        public SubWindows.SubOPAutoView subOPAutoView;
        public SubWindows.SubCheckItem SubCheckItem;


        //ADS
        private ArrayList notificationHandles;
        private TcAdsClient adsClient;
        public static bool AdsConn;
        public int hbb1;
        private int H_bArray, H_iArray, H_rArray;     //PLC变量对应的句柄
        private int H_iSemiAuto, H_iAuto, H_rSlitter;     //PLC变量对应的句柄
        private int H_iAngleMap;
        private int H_iAngleMapTop, H_iAngleMapBtm, H_iHeightMap;
        private int H_rAdvPara;
        private int H_handle1, H_handle2;

        static public bool[] Hmi_bArray = new bool[300];
        static public Int16[] Hmi_iArray = new Int16[300];
        static public float[] Hmi_rArray = new float[300];

        static public Int16[] Hmi_iSemiAuto = new Int16[300];
        static public Int16[] Hmi_iAuto = new Int16[300];
        static public float[] Hmi_rSlitter = new float[30];
        static public Int16[] Hmi_iAngleMapTop = new Int16[200];
        static public Int16[] Hmi_iAngleMapBtm = new Int16[200];
        static public Int16[] Hmi_iHeightMap = new Int16[400];
        static public Int16[] Hmi_iAngleMap = new Int16[200];
        static public float[] Hmi_rAdvPara = new float[100];      //进阶参数

        static public bool gbl_BootOK = false;

        string myConnectionString = @"provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\JSZW1000A.accdb";


        static public int[] cfg_GlobalSwitch = new int[20];
        static public float[] cfg_ClampHeight = new float[20];
        static public float[] cfg_BackgaugeApron = new float[20];
        static public float[] cfg_PresetLengths = new float[20];
        static public float[] cfg_PresetAngles = new float[20];
        static public float[] cfg_MachineSetup = new float[20];
        static public float[] cfg_ManualOldSelect = new float[20];
        static public string[] Materials = new string[10];

        //ConfigData,所有下标定义为CONST
        static public float[] ConfigData = new float[200];
        static public string[] ConfigStr = new string[200];
        //以下CONST变量的值,为Config.ini文件中  对应行号-1
        public const int L1_GlobalSwitch = 0, L2_ClampHeight = 12, L3_BackgaugeApron = 21;
        public const int L4_PresetLengths = 42, L5_PresetAngles = 54, L6_MachineSetup = 66, L7_ManualOldSelect = 77, L8_AutoFeedPara = 91, L9_OtherConfig = 96;




        public MainFrm()
        {
            InitializeComponent();


        }

        // 定义你的密码（也可以从配置文件读取）
        //private const string SETTING_PASSWORD = "84880388";


        private void btn导航_设置_Click(object sender, EventArgs e)
        {
            //// 初始化登录框状态
            //tbload.Text = "";           // 清空旧密码
            //tbload.PasswordChar = '*';  // 设置掩码（输入时显示星号）
            //plload.Visible = true;      // 显示登录面板
            //plload.BringToFront();      // 确保面板在最上层
            //tbload.Focus();             // 让光标自动进入输入框

            LoadSettingPage();      // 执行原先的跳转逻辑
        }

        private void tbload_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                plload.Visible = false;      // 隐藏登录面板
            }
        }

        private void btn登录_Click(object sender, EventArgs e)
        {
            //string inputPwd = tbload.Text.Trim(); // 获取输入并去除空格

            //if (inputPwd == SETTING_PASSWORD)
            //{
            //    // 密码正确
            //    plload.Visible = false; // 隐藏登录框
            //    LoadSettingPage();      // 执行原先的跳转逻辑
            //}
            //else
            //{
            //    // 密码错误
            //    MessageBox.Show("密码错误，请重试！", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    plload.Visible = false;      // 隐藏登录面板
            //    //tbload.SelectAll();     // 选中错误的密码方便重输
            //    //tbload.Focus();
            //}
        }

        private void LoadSettingPage()
        {
            // 创建 SubOPSetting 实例并设置相关属性
            SubWindows.SubOPSetting subOPSetting = new SubWindows.SubOPSetting();
            subOPSetting.mf = this;
            // 显示 SubOPSetting 窗口
            subOPSetting.Show();

            // 清空 gpbSubWin 控件集合，并添加 subOPSetting 控件
            gpbSubWin.Controls.Clear();
            gpbSubWin.Controls.Add(subOPSetting);

            // 设置按钮的前景色
            btn导航_设置.ForeColor = Color.FromArgb(96, 176, 255);
            // 注意：这里要把其他按钮变回白色
            btn导航_手动.ForeColor = btn导航_自动.ForeColor = btn导航_库.ForeColor = btn导航_分条.ForeColor = Color.White;

            // 隐藏多个面板和按钮
            pnl锥度设定.Visible = false;
            pnl自动4视图.Visible = pnl角度尺寸.Visible = btn折弯预览.Visible = btn重置视图.Visible = false;
            btn颜色侧翻.Visible = btn发送到半自动.Visible = btn撤消.Visible = btn重复.Visible = false;
            btn构图完成.Visible = btn保存.Visible = btn另存为.Visible = btn手动强制.Visible = btnFeed.Visible = false;
        }

        private void btn导航_分条_Click(object sender, EventArgs e)
        {
            // 创建 SubOPSlitter 实例并传入当前对象
            subOPSlitter = new SubWindows.SubOPSlitter(this);
            // 显示 SubOPSlitter 窗口
            subOPSlitter.Show();

            // 清空 gpbSubWin 中的控件并添加 SubOPSlitter 控件
            gpbSubWin.Controls.Clear();
            gpbSubWin.Controls.Add(subOPSlitter);

            // 设置按钮颜色，点击的按钮设置为指定颜色，其他导航按钮设置为白色
            btn导航_分条.ForeColor = Color.FromArgb(96, 176, 255);
            btn导航_手动.ForeColor = btn导航_自动.ForeColor = btn导航_库.ForeColor = btn导航_设置.ForeColor = Color.White;

            // 隐藏多个面板和按钮
            pnl锥度设定.Visible = false;
            pnl自动4视图.Visible = pnl角度尺寸.Visible = btn折弯预览.Visible = btn重置视图.Visible = false;
            btn颜色侧翻.Visible = btn发送到半自动.Visible = btn撤消.Visible = btn重复.Visible = false;
            btn构图完成.Visible = btn保存.Visible = btn另存为.Visible = btn手动强制.Visible = btnFeed.Visible = false;
        }

        private void 导航_CheckItem()
        {
            // 创建 SubCheckItem 实例并传入当前对象
            SubCheckItem = new SubWindows.SubCheckItem(this);
            // 显示 SubCheckItem 窗口
            SubCheckItem.Show();

            // 清空 gpbSubWin 中的控件并添加 SubCheckItem 控件
            gpbSubWin.Controls.Clear();
            gpbSubWin.Controls.Add(SubCheckItem);

            // 显示 btn条款确定 按钮
            btn条款确定.Visible = true;

            // 隐藏部分按钮和面板
            btn手动强制.Visible = btnFeed.Visible = btn保存.Visible = btn另存为.Visible = panel9.Visible = false;

            // 隐藏面板和设置按钮颜色
            pnl锥度设定.Visible = false;
            btn导航_手动.ForeColor = Color.FromArgb(96, 176, 255);
            btn导航_自动.ForeColor = btn导航_库.ForeColor = btn导航_设置.ForeColor = btn导航_分条.ForeColor = Color.White;

            // 隐藏多个面板和按钮
            pnl自动4视图.Visible = pnl角度尺寸.Visible = btn折弯预览.Visible = btn重置视图.Visible = btnFeed.Visible = false;
            btn颜色侧翻.Visible = btn发送到半自动.Visible = btn撤消.Visible = btn重复.Visible = btn构图完成.Visible = false;
        }

        private void btn导航_手动_Click(object sender, EventArgs e)
        {
            fun导航_手动();
        }
        private void fun导航_手动()
        {
            // 创建 SubOPManual 实例并传入当前对象
            subOPManual = new SubWindows.SubOPManual(this);
            // 显示 SubOPManual 窗口
            subOPManual.Show();

            // 清空 gpbSubWin 中的控件并添加 SubOPManual 控件
            gpbSubWin.Controls.Clear();
            gpbSubWin.Controls.Add(subOPManual);

            // 显示部分按钮和面板
            panel9.Visible = btn手动强制.Visible = btnFeed.Visible = btn保存.Visible = btn另存为.Visible = true;

            // 隐藏面板并设置按钮颜色
            pnl锥度设定.Visible = false;
            btn导航_手动.ForeColor = Color.FromArgb(96, 176, 255);
            btn导航_自动.ForeColor = btn导航_库.ForeColor = btn导航_设置.ForeColor = btn导航_分条.ForeColor = Color.White;

            // 隐藏多个面板和按钮
            pnl自动4视图.Visible = pnl角度尺寸.Visible = btn折弯预览.Visible = btn重置视图.Visible =  false;
            btn颜色侧翻.Visible = btn发送到半自动.Visible = btn撤消.Visible = btn重复.Visible = btn构图完成.Visible = false;

            btnFeed.Visible = (ConfigData[L1_GlobalSwitch + 10] > 0);
            
        }

        private void btn导航_自动_Click(object sender, EventArgs e)
        {
            // 自动按钮入口与分度盘/库双击/绘图返回统一，全部交给切入自动1收口 UI 状态。
            切入自动1(false, false);
        }

        private void btn导航_库_Click(object sender, EventArgs e)
        {
            // 创建 SubOPLibrary 实例并传入当前对象
            subOPLibrary = new SubWindows.SubOPLibrary(this);
            // 显示 SubOPLibrary 窗口
            subOPLibrary.Show();

            // 清空 gpbSubWin 中的控件并添加 SubOPLibrary 控件
            gpbSubWin.Controls.Clear();
            gpbSubWin.Controls.Add(subOPLibrary);

            // 设置按钮颜色，点击的按钮设置为指定颜色，其他导航按钮设置为白色
            btn导航_库.ForeColor = Color.FromArgb(96, 176, 255);
            btn导航_自动.ForeColor = btn导航_手动.ForeColor = btn导航_设置.ForeColor = btn导航_分条.ForeColor = Color.White;

            // 隐藏面板和按钮
            pnl锥度设定.Visible = false;
            pnl自动4视图.Visible = pnl角度尺寸.Visible = btn折弯预览.Visible = btn重置视图.Visible = false;
            btn颜色侧翻.Visible = btn发送到半自动.Visible = btn撤消.Visible = btn重复.Visible = btn构图完成.Visible = btn手动强制.Visible = btnFeed.Visible = false;
        }

        public bool b条款确认 = false;

        private void btn条款确定_Click(object sender, EventArgs e)
        {
            b条款确认 = true;
            if (b条款确认)
            {
                EnterMainManualPage();
            }
            else
            {
                MessageBox.Show(Strings.Get("MainFrm.StartupCheckRequired"));
            }

        }
        private void InitAct()
        {
            MainFrm.Hmi_bArray[0] = true;
            MainFrm.Hmi_bArray[1] = true;

            for (int i = 0; i < 20; i++)
            {
                angleAddit[i].AngleRange = new float[40];
            }
            CurtOrder.lengAngle = new LengAngle[100];
            LoadParaFile(1);
            LoadInitSet();
            LoadOrderFile(MainFrm.ConfigStr[1]);
            setLang();


            int idx = 0;
            while (angleAddit[idx].Type != null)
            {
                cbx材料选择.Items.Add(angleAddit[idx].Type);
                idx++;
            }
            cbx材料选择.SelectedIndex = Convert.ToInt32(MainFrm.Hmi_rArray[56]);

            AdsConnEx();
            timer1s.Start();
        }
        public void setLang()
        {
            LocalizationManager.ApplyResources(this);

            if (MainFrm.Lang == 0)
            {
                lb下导_模式.Font = new System.Drawing.Font("微软雅黑", 18F);
                lb下导_夹钳高度.Font = new System.Drawing.Font("微软雅黑", 12F);
                lb下导_角度.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F);
                lbPLC消息.Font = new System.Drawing.Font("Microsoft YaHei UI", 12.75F);
                btnMsgClr.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
                lb油泵A.Font = lb油泵B.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);

                btn导航_自动.Font = btn导航_手动.Font = new System.Drawing.Font("微软雅黑", 20F);
                btn导航_库.Font = btn导航_设置.Font = btn导航_分条.Font = new System.Drawing.Font("微软雅黑", 16F);

                lb自动_折弯生产.Font = lb自动_折弯预览.Font = lb自动_工作单设定.Font = lb自动_快速构图.Font =
                lb工作单_角度尺寸.Font = lb工作单_角度.Font = lb工作单_尺寸.Font = lb工作单_比例.Font = new System.Drawing.Font("宋体", 12.75F);
                lb锥度设定.Font = lb锥度设定状态.Font = new System.Drawing.Font("宋体", 11.25F);

                btn条款确定.Font = btn构图完成.Font = btn手动强制.Font =
                btn折弯预览.Font = btn重置视图.Font = btn颜色侧翻.Font = btn发送到半自动.Font = btn撤消.Font = btn重复.Font = btn保存.Font =
                btn另存为.Font = new System.Drawing.Font("宋体", 10F);
                lbOilLevel.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            }
            else
            {
                lb下导_模式.Font = new System.Drawing.Font("Calibri", 16);
                lb下导_夹钳高度.Font = new System.Drawing.Font("Calibri", 11F);
                lb下导_角度.Font = new System.Drawing.Font("Calibri", 12F);
                lbPLC消息.Font = new System.Drawing.Font("Calibri", 12.75F);
                btnMsgClr.Font = new System.Drawing.Font("Calibri", 9F);
                lb油泵A.Font = lb油泵B.Font = new System.Drawing.Font("Calibri", 10F);

                btn导航_自动.Font = btn导航_手动.Font = new System.Drawing.Font("Calibri", 16F);
                btn导航_库.Font = btn导航_设置.Font = btn导航_分条.Font = new System.Drawing.Font("Calibri", 14F);

                lb自动_折弯生产.Font = lb自动_折弯预览.Font = lb自动_工作单设定.Font = lb自动_快速构图.Font =
                lb工作单_角度尺寸.Font = lb工作单_角度.Font = lb工作单_尺寸.Font = lb工作单_比例.Font = lb锥度设定.Font = new System.Drawing.Font("Calibri", 11F);

                btn条款确定.Font = btn构图完成.Font = btn手动强制.Font = btn折弯预览.Font = btn重置视图.Font = btn颜色侧翻.Font = btn发送到半自动.Font = btn撤消.Font = btn重复.Font = btn保存.Font =
                btn另存为.Font = new System.Drawing.Font("Calibri", 10F);
                lbOilLevel.Font = new System.Drawing.Font("Calibri", 10F);
            }
            btn条款确定.Text = Strings.Get("MainFrm.Action.Confirm");
            lb下导_模式.Text = Strings.Get("MainFrm.Footer.Mode");
            lb下导_夹钳高度.Text = Strings.Get("MainFrm.Footer.ClampHeight");
            lb下导_角度.Text = Strings.Get("MainFrm.Footer.Angle");
            lbPLC消息.Text = Strings.Get("MainFrm.Status.PlcMessage");
            btnMsgClr.Text = Strings.Get("MainFrm.Action.ClearMessage");
            lb油泵A.Text = Strings.Get("MainFrm.Status.PumpA");
            lb油泵B.Text = Strings.Get("MainFrm.Status.PumpB");
            lbTSlide.Text = Strings.Get("MainFrm.Status.TopSlide");
            lbTFold.Text = Strings.Get("MainFrm.Status.TopFold");
            lbBSlide.Text = Strings.Get("MainFrm.Status.BottomSlide");
            lbBFold.Text = Strings.Get("MainFrm.Status.BottomFold");
            lbServoRdy.Text = Strings.Get("MainFrm.Status.Servo");
            lbCoupling.Text = Strings.Get("MainFrm.Status.Couple");
            lb钳口移动R.Text = Strings.Get("MainFrm.Status.ClampMoveRear");
            lb钳口移动F.Text = Strings.Get("MainFrm.Status.ClampMoveFront");
            lb钳口移动M.Text = Strings.Get("MainFrm.Status.ClampMoveMiddle");
            btn导航_自动.Text = Strings.Get("MainFrm.Nav.Auto");
            btn导航_手动.Text = Strings.Get("MainFrm.Nav.Manual");
            btn导航_库.Text = Strings.Get("MainFrm.Nav.Library");
            btn导航_设置.Text = Strings.Get("MainFrm.Nav.Setup");
            btn导航_分条.Text = Strings.Get("MainFrm.Nav.Slitter");
            lb自动_折弯生产.Text = Strings.Get("MainFrm.Section.Production");
            lb自动_折弯预览.Text = Strings.Get("MainFrm.Section.FoldPreview");
            lb自动_工作单设定.Text = Strings.Get("MainFrm.Section.JobSetup");
            lb自动_快速构图.Text = Strings.Get("MainFrm.Section.QuickDraw");
            lb工作单_角度尺寸.Text = Strings.Get("MainFrm.Worksheet.AnglesDimensions");
            lb工作单_角度.Text = Strings.Get("MainFrm.Worksheet.Angles");
            lb工作单_尺寸.Text = Strings.Get("MainFrm.Worksheet.Dimensions");
            lb工作单_比例.Text = Strings.Get("MainFrm.Worksheet.Scaled");
            lb锥度设定.Text = Strings.Get("MainFrm.Taper.Setup");
            lb锥度单位.Text = GetLengthUnitLabel();
            string unitTag = "[" + GetLengthUnitLabel() + "]";
            label25.Text = label23.Text = label18.Text = label16.Text = label14.Text = label10.Text = label8.Text = label5.Text = label13.Text = unitTag;
            lb锥度设定状态.Text = Strings.Get("MainFrm.Taper.Off");
            btn构图完成.Text = Strings.Get("MainFrm.Action.Done");
            btn手动强制.Text = Strings.Get("MainFrm.Action.ManualForce");
            btn折弯预览.Text = Strings.Get("MainFrm.Action.FoldPreview");
            btn重置视图.Text = Strings.Get("MainFrm.Action.ResetView");
            btn颜色侧翻.Text = Strings.Get("MainFrm.Action.ColorSideFlip");
            btn发送到半自动.Text = Strings.Get("MainFrm.Action.SendSemiAuto");
            btn撤消.Text = Strings.Get("MainFrm.Action.Undo");
            btn重复.Text = Strings.Get("MainFrm.Action.Redo");
            btn保存.Text = Strings.Get("MainFrm.Action.Save");
            btn另存为.Text = Strings.Get("MainFrm.Action.SaveAs");
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            subOPSlitter = new SubWindows.SubOPSlitter(this);

            LoadParaFile(0);
            MainFrm.Lang = LocalizationManager.ToLegacyLanguageId(LocalizationManager.CurrentLanguage);
            setLang();
            导航_CheckItem();
            AdsConnEx();
        }

        //private void button2_Click_2(object sender, EventArgs e)
        //{
        //    OrderType odr = new OrderType();
        //    odr.Name = "123";
        //    odr.Width = 1.23;
        //    odr.lengAngle[0].Length = 1000;
        //    odr.lengAngle[0].Angle = 90.0;
        //    Point p1 = new Point();
        //    p1.X = 65; p1.Y = 72;
        //    odr.pxList.Add(p1);
        //    GblOrder.Add(odr);

        //}


        //string path1 = @"C:\Jing Gong Flashings\OrderLibrary.txt";
        string path = MainFrm.ConfigStr[1];
        string path2 = System.Windows.Forms.Application.StartupPath + @"Config.ini";
        string path3 = System.Windows.Forms.Application.StartupPath + @"angleAddit.txt";

        public void LoadOrderFile(string path)
        {
            GblOrder.Clear();
            string[] iniFiles = Directory.GetFiles(path, "*.ini");
            foreach (string file in iniFiles)
            {
                string content = File.ReadAllText(file);
                string contentWithoutNewLines = content.Replace("\r\n", "").Replace("\n", "").Replace(" ", "");
                if (string.IsNullOrEmpty(contentWithoutNewLines))
                    continue;
                string[] separators = { ",", ":" };
                string[] s1 = contentWithoutNewLines.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                OrderType odr1 = new OrderType();
                odr1.Name = s1[1].Replace("\"", "").Trim();
                odr1.Width = Convert.ToDouble(s1[3]);
                odr1.SheetLength = Convert.ToDouble(s1[5]);
                odr1.Thickness = Convert.ToDouble(s1[7]);
                odr1.MaterialName = s1[9].Replace("\"", "");
                odr1.Customer = s1[11].Replace("\"", "");
                odr1.Remark = s1[13].Replace("\"", "");
                odr1.isTaper = Convert.ToBoolean(s1[15].Replace("\"", ""));
                odr1.TaperLength = Convert.ToDouble(s1[17]);
                if (s1.Length > 19)
                    DeserializeInlineSlitReserve(s1[19].Replace("\"", ""), ref odr1);

                SemiAutoType odrSemi = new SemiAutoType();

                int i = 23;
                while (s1[i].Trim() != "pxList")
                {
                    string[] s2 = s1[i].Split("/");

                    odrSemi.折弯序号 = (i - 22);
                    odrSemi.行动类型 = Convert.ToInt32(s2[0]);
                    odrSemi.折弯方向 = Convert.ToInt32(s2[1]);
                    odrSemi.折弯角度 = Math.Round(Convert.ToSingle(s2[2]), 3);
                    odrSemi.回弹值 = Math.Round(Convert.ToSingle(s2[3]), 2);
                    odrSemi.后挡位置 = Math.Round(Convert.ToSingle(s2[4]), 2);
                    odrSemi.抓取类型 = Convert.ToInt32(s2[5]);
                    odrSemi.松开高度 = Convert.ToInt32(s2[6]);
                    odrSemi.翻板收缩值 = Convert.ToInt32(s2[7]);
                    odrSemi.重新抓取 = Convert.ToInt32(s2[8]);
                    odr1.lstSemiAuto.Add(odrSemi);
                    i++;
                }
                i++;
                while (s1[i].Trim() != "LengthAngle")
                {
                    string[] s2 = s1[i].Split('/');
                    if (s2.Length >= 2)
                    {
                        if (float.TryParse(s2[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float x)
                            && float.TryParse(s2[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float y))
                        {
                            odr1.pxList.Add(new PointF(x, y));
                        }
                        else
                        {
                            // 记录/跳过错误项，避免抛出异常
                            // e.g. Log($"Invalid pxList entry: {s1[i]} in file {file}");
                        }
                    }
                    i++;
                }
                i++;
                int j = 0;
                while (i < s1.Length - 1)
                {
                    string[] s2 = s1[i].Split("/");

                    odr1.lengAngle[j].Length = Convert.ToDouble(s2[0]);
                    odr1.lengAngle[j].TaperWidth = Convert.ToDouble(s2[1]);
                    odr1.lengAngle[j].Angle = Convert.ToDouble(s2[2]);
                    i++; j++;
                }
                string[] s22 = s1[i].Split("/");
                odr1.lengAngle[99].Length = Convert.ToDouble(s22[0]);
                odr1.lengAngle[99].TaperWidth = Convert.ToDouble(s22[1]);
                odr1.lengAngle[99].Angle = Convert.ToDouble(s22[2]);

                GblOrder.Add(odr1);

            }


        }
        public void LoadOrderFile000()
        {
            if (File.Exists(path))
            {
                GblOrder.Clear();
                string[] lines = System.IO.File.ReadAllLines(path, Encoding.Default);
                //依次读取每行数据
                foreach (string s in lines)
                {
                    if (string.IsNullOrEmpty(s))
                        continue;
                    string[] separators = { ",", ":" };
                    string[] s1 = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                    OrderType odr1 = new OrderType();
                    odr1.Name = s1[1].Replace("\"", "").Trim(); odr1.Width = Convert.ToDouble(s1[3]); odr1.SheetLength = Convert.ToDouble(s1[5]);
                    odr1.Thickness = Convert.ToDouble(s1[7]);
                    odr1.MaterialName = s1[9].Replace("\"", "");
                    odr1.Customer = s1[11].Replace("\"", "");
                    odr1.Remark = s1[13].Replace("\"", "");
                    odr1.isTaper = Convert.ToBoolean(s1[15].Replace("\"", ""));
                    odr1.TaperLength = Convert.ToDouble(s1[17]);
                    if (s1.Length > 19)
                        DeserializeInlineSlitReserve(s1[19].Replace("\"", ""), ref odr1);

                    SemiAutoType odrSemi = new SemiAutoType();

                    int i = 23;
                    while (s1[i].Trim() != "pxList")
                    {
                        string[] s2 = s1[i].Split("/");

                        odrSemi.折弯序号 = (i - 22);
                        odrSemi.行动类型 = Convert.ToInt32(s2[0]);
                        odrSemi.折弯方向 = Convert.ToInt32(s2[1]);
                        odrSemi.折弯角度 = Math.Round(Convert.ToSingle(s2[2]), 3);
                        odrSemi.回弹值 = Math.Round(Convert.ToSingle(s2[3]), 2);
                        odrSemi.后挡位置 = Math.Round(Convert.ToSingle(s2[4]), 2);
                        odrSemi.抓取类型 = Convert.ToInt32(s2[5]);
                        odrSemi.松开高度 = Convert.ToInt32(s2[6]);
                        odrSemi.翻板收缩值 = Convert.ToInt32(s2[7]);
                        odrSemi.重新抓取 = Convert.ToInt32(s2[8]);
                        odr1.lstSemiAuto.Add(odrSemi);
                        i++;
                    }
                    i++;
                    while (s1[i].Trim() != "LengthAngle")
                    {
                        string[] s2 = s1[i].Split("/");

                        odr1.pxList.Add(new Point(Convert.ToInt32(s2[0]), Convert.ToInt32(s2[1])));
                        i++;
                    }
                    i++;
                    int j = 0;
                    while (i < s1.Length - 1)
                    {
                        string[] s2 = s1[i].Split("/");

                        odr1.lengAngle[j].Length = Convert.ToDouble(s2[0]);
                        odr1.lengAngle[j].TaperWidth = Convert.ToDouble(s2[1]);
                        odr1.lengAngle[j].Angle = Convert.ToDouble(s2[2]);
                        i++; j++;
                    }
                    string[] s22 = s1[i].Split("/");
                    odr1.lengAngle[99].Length = Convert.ToDouble(s22[0]);
                    odr1.lengAngle[99].TaperWidth = Convert.ToDouble(s22[1]);
                    odr1.lengAngle[99].Angle = Convert.ToDouble(s22[2]);

                    GblOrder.Add(odr1);
                }
            }
        }

        private void LoadParaFile(int sel)         //0:仅配置文件  1:配置文件+角度文件
        {
            if (File.Exists(path2))
            {
                string[] lines = System.IO.File.ReadAllLines(path2, Encoding.Default);
                int i = 0, j = 0, k = 0;
                bool isStr = false;
                //依次读取每行数据
                foreach (string s in lines)
                {
                    if (s == "[GlobalSwitch]")
                    { ConfigData[j++] = 9990; isStr = false; }
                    else if (s == "[ClampHeight]")
                    { ConfigData[j++] = 9991; isStr = false; }
                    else if (s == "[BackgaugeApron]")
                    { ConfigData[j++] = 9992; isStr = false; }
                    else if (s == "[PresetLengths]")
                    { ConfigData[j++] = 9993; isStr = false; }
                    else if (s == "[PresetAngles]")
                    { ConfigData[j++] = 9994; isStr = false; }
                    else if (s == "[MachineSetup]")
                    { ConfigData[j++] = 9995; isStr = false; }
                    else if (s == "[ManualOldSelect]")
                    { ConfigData[j++] = 9996; isStr = false; }
                    else if (s == "[AutoFeedPara]")
                    { ConfigData[j++] = 9997; isStr = false; }
                    else if (s == "[OtherConfig]")
                    { ConfigData[j++] = 9998; isStr = true; }
                    else if (s.Trim() == "")
                    {
                        if (!isStr)
                            ConfigData[j++] = 9999;
                    }
                    else if (s.Length >= 5 && s.Substring(0, 5) == "-----")
                    { ConfigData[j++] = 9999; isStr = false; }
                    else
                    {
                        if (isStr)
                        {
                            string[] s1 = s.Split("#");
                            ConfigStr[k++] = s1[s1.Length - 1].Trim();
                        }
                        else
                        {
                            string[] s1 = s.Split(" ");
                            ConfigData[j++] = Convert.ToSingle(s1[s1.Length - 1]);
                        }
                    }
                }
            }
            if (sel == 0) return;

            if (File.Exists(path3))
            {
                string[] lines = System.IO.File.ReadAllLines(path3, Encoding.Default);
                int idx = 0;
                //依次读取每行数据
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Substring(0, 1) == "[")
                    {
                        angleAddit[idx].Type = lines[i];
                        angleAddit[idx].Material = lines[i + 1].Substring(9);
                        angleAddit[idx].Strength = lines[i + 2].Substring(9);
                        angleAddit[idx].Thickness = lines[i + 3].Substring(10);
                        angleAddit[idx].MachingGauging = lines[i + 4].Substring(15);
                        for (int j = 0; j < 30; j++)
                        {
                            string[] s1 = lines[i + 5 + j].Split("=");
                            angleAddit[idx].AngleRange[j] = Convert.ToSingle(s1[1]);
                        }
                        idx++;
                    }
                }
            }
        }

        private void LoadInitSet()
        {
            MainFrm.Hmi_bArray[0] = Convert.ToBoolean(ConfigData[L1_GlobalSwitch + 1]);
            MainFrm.Hmi_bArray[1] = Convert.ToBoolean(ConfigData[L1_GlobalSwitch + 2]);
            MainFrm.Hmi_bArray[2] = Convert.ToBoolean(ConfigData[L1_GlobalSwitch + 3]);
            MainFrm.Hmi_bArray[3] = Convert.ToBoolean(ConfigData[L1_GlobalSwitch + 4]);
            MainFrm.Hmi_bArray[4] = Convert.ToBoolean(ConfigData[L1_GlobalSwitch + 5]);
            MainFrm.Hmi_bArray[5] = Convert.ToBoolean(ConfigData[L1_GlobalSwitch + 6]);
            MainFrm.Hmi_bArray[6] = Convert.ToBoolean(ConfigData[L1_GlobalSwitch + 7]);
            MainFrm.Hmi_bArray[7] = Convert.ToBoolean(ConfigData[L1_GlobalSwitch + 10]);

            MainFrm.Hmi_rArray[100] = ConfigData[L2_ClampHeight + 1];
            MainFrm.Hmi_rArray[101] = ConfigData[L2_ClampHeight + 2];
            MainFrm.Hmi_rArray[102] = ConfigData[L2_ClampHeight + 3];
            MainFrm.Hmi_rArray[103] = ConfigData[L2_ClampHeight + 4];
            MainFrm.Hmi_rArray[104] = ConfigData[L2_ClampHeight + 5];
            MainFrm.Hmi_rArray[105] = ConfigData[L2_ClampHeight + 6];
            MainFrm.Hmi_rArray[106] = ConfigData[L2_ClampHeight + 7];

            MainFrm.Hmi_rArray[110] = ConfigData[L3_BackgaugeApron + 1];
            MainFrm.Hmi_rArray[111] = ConfigData[L3_BackgaugeApron + 2];
            MainFrm.Hmi_rArray[112] = ConfigData[L3_BackgaugeApron + 3];
            MainFrm.Hmi_rArray[113] = ConfigData[L3_BackgaugeApron + 4];
            MainFrm.Hmi_rArray[114] = ConfigData[L3_BackgaugeApron + 5];
            MainFrm.Hmi_rArray[115] = ConfigData[L3_BackgaugeApron + 6];
            MainFrm.Hmi_rArray[116] = ConfigData[L3_BackgaugeApron + 7];
            MainFrm.Hmi_rArray[117] = ConfigData[L3_BackgaugeApron + 8];
            MainFrm.Hmi_rArray[118] = ConfigData[L3_BackgaugeApron + 9];
            MainFrm.Hmi_rArray[119] = ConfigData[L3_BackgaugeApron + 10];
            MainFrm.Hmi_rArray[120] = ConfigData[L3_BackgaugeApron + 11];
            MainFrm.Hmi_rArray[121] = ConfigData[L3_BackgaugeApron + 12];
            MainFrm.Hmi_rArray[122] = ConfigData[L3_BackgaugeApron + 13];
            MainFrm.Hmi_rArray[123] = ConfigData[L3_BackgaugeApron + 14];
            MainFrm.Hmi_rArray[95] = ConfigData[L8_AutoFeedPara + 5];
            MainFrm.Hmi_rArray[96] = ConfigData[L8_AutoFeedPara + 6];
            MainFrm.Hmi_rArray[97] = ConfigData[L8_AutoFeedPara + 7];
            MainFrm.Hmi_rArray[98] = ConfigData[L8_AutoFeedPara + 8];
            MainFrm.Hmi_rArray[99] = ConfigData[L8_AutoFeedPara + 9];
            MainFrm.Hmi_rArray[124] = ConfigData[L3_BackgaugeApron + 15];
            MainFrm.Hmi_rArray[125] = ConfigData[L3_BackgaugeApron + 16];
            MainFrm.Hmi_rArray[126] = ConfigData[L3_BackgaugeApron + 17];
            MainFrm.Hmi_rArray[127] = ConfigData[L3_BackgaugeApron + 18];
            MainFrm.Hmi_rArray[128] = ConfigData[L3_BackgaugeApron + 19];
            MainFrm.Hmi_rArray[129] = ConfigData[L6_MachineSetup + 8];

            MainFrm.Hmi_bArray[45] = Convert.ToBoolean(ConfigData[MainFrm.L7_ManualOldSelect + 1]);
            MainFrm.Hmi_bArray[46] = Convert.ToBoolean(ConfigData[MainFrm.L7_ManualOldSelect + 2]);

            MainFrm.Hmi_rArray[50] = ConfigData[MainFrm.L7_ManualOldSelect + 3];
            MainFrm.Hmi_rArray[51] = ConfigData[MainFrm.L7_ManualOldSelect + 4];
            MainFrm.Hmi_rArray[52] = ConfigData[MainFrm.L7_ManualOldSelect + 5];
            MainFrm.Hmi_rArray[53] = ConfigData[MainFrm.L7_ManualOldSelect + 6];
            MainFrm.Hmi_rArray[54] = ConfigData[MainFrm.L7_ManualOldSelect + 7];
            MainFrm.Hmi_rArray[55] = ConfigData[MainFrm.L7_ManualOldSelect + 8];
            MainFrm.Hmi_rArray[56] = ConfigData[MainFrm.L7_ManualOldSelect + 9];

            MainFrm.SpringTop = MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 10];
            MainFrm.SpringBtm = MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 11];

            for (int i = 0; i < 15; i++)
            {
                Int16 id = (Int16)MainFrm.Hmi_rArray[56];
                MainFrm.Hmi_rArray[150 + i] = angleAddit[id].AngleRange[i];   //下补偿角度
                MainFrm.Hmi_rArray[170 + i] = angleAddit[id].AngleRange[i];   //上补偿角度
            }


            pnlMT5.Visible = pnlMT6.Visible = pnlMT7.Visible = pnlMT8.Visible =
            lbMT5.Visible = lbMT6.Visible = lbMT7.Visible = lbMT8.Visible = (ConfigData[L1_GlobalSwitch + 9] > 400);
            pnlMT9.Visible = lbMT9.Visible = (ConfigData[L1_GlobalSwitch + 9] > 800);
        }

        public void wrtOrderList(string name)
        {
            //if (File.Exists(path1))
            //{
            //    string[] lines = System.IO.File.ReadAllLines(path1);
            //    int idx = 0;

            //    while (lines[idx].Trim() != name)
            //    { idx++; }

            //    File.WriteAllLines(path1, lines);
            //}
        }


        //暂时只能按名字更新,
        //按名字更新,每组35行.名字1行+参数4行+角度15行+角2度15行.
        public void wrtAngleAdditFile(string type)
        {
            if (File.Exists(path3))
            {
                string[] lines = System.IO.File.ReadAllLines(path3);
                int idx = 0;
                while (angleAddit[idx].Type != type && angleAddit[idx].Type != "")
                { idx++; }

                lines[idx * 35] = angleAddit[idx].Type;
                lines[idx * 35 + 1] = "Material=" + angleAddit[idx].Material;
                lines[idx * 35 + 2] = "Strength=" + angleAddit[idx].Strength;
                lines[idx * 35 + 3] = "Thickness=" + angleAddit[idx].Thickness;
                lines[idx * 35 + 4] = "MachineGauging=" + angleAddit[idx].MachingGauging;

                for (int j = 0; j < 30; j++)
                {
                    string str;
                    if (j < 15)
                        str = "AngleRange" + Convert.ToString((j + 1) * 10) + "=";
                    else
                        str = "Angle2Range" + Convert.ToString((j - 14) * 10) + "=";
                    lines[idx * 35 + 5 + j] = str + angleAddit[idx].AngleRange[j].ToString();
                }
                File.WriteAllLines(path3, lines);
            }
        }

        //分类名字+分类下的行号,两个标志写入,0:所有行都写入
        public void wrtConfigFile(string type, int line)
        {
            if (File.Exists(path2))
            {
                string[] lines = System.IO.File.ReadAllLines(path2);
                int idx = 0;
                int start = 0;
                int end;
                if (line > 0)
                { start = end = line; }
                else
                { start = 0; end = 99; }


                while (lines[idx].Trim() != type)
                { idx++; }
                if (type == "[GlobalSwitch]")
                {
                    while (start <= end && lines[idx + start].Trim() != "")
                    {
                        string[] s1 = lines[idx + start].Split(" ");
                        string str = lines[idx + start].Substring(0, lines[idx + start].Length - s1[s1.Length - 1].Length);
                        lines[idx + start] = str + string.Format("{0:F2}", ConfigData[idx + start]);
                        start++;
                    }
                }
                else if (type == "[ClampHeight]" || type == "[BackgaugeApron]" || type == "[PresetLengths]" || type == "[PresetAngles]" || type == "[MachineSetup]" || type == "[ManualOldSelect]")
                {
                    while (start <= end && lines[idx + start].Substring(0, 5) != "-----")
                    {
                        string[] s1 = lines[idx + start].Split(" ");
                        if (start == 0)
                        {
                            ;
                        }
                        else
                        {
                            string str = lines[idx + start].Substring(0, lines[idx + start].Length - s1[s1.Length - 1].Length);
                            lines[idx + start] = str + string.Format("{0:F2}", ConfigData[idx + start]);
                        }
                        start++;
                    }
                    idx++;
                }
                else if (type == "[OtherConfig]")
                {
                    while (start <= end && lines[idx + start].Substring(0, 5) != "-----")
                    {
                        string[] s1 = lines[idx + start].Split("#");
                        if (start == 0)
                        {
                            ;
                        }
                        else
                        {
                            lines[idx + start] = s1[0].Trim() + " # " + ConfigStr[start - 1];
                        }

                        start++;
                    }
                }
                File.WriteAllLines(path2, lines);
            }
        }

        private string Generate1Order(string name, IReadOnlyList<SemiAutoType>? stepsOverride = null)
        {
            CurtOrder.Name = name;
            IReadOnlyList<SemiAutoType> steps = stepsOverride ?? CurtOrder.lstSemiAuto;
            string str = "Name:\"" + CurtOrder.Name + "\",";
            str += " Width:" + CurtOrder.Width.ToString() + ",";
            str += " SheetLength:" + CurtOrder.SheetLength.ToString() + ",";
            str += " Thickness:" + CurtOrder.Thickness.ToString() + ",";
            str += " MaterialName:\"" + CurtOrder.MaterialName.ToString() + "\",";
            str += " Customer:\"" + CurtOrder.Customer.ToString() + "\",";
            str += " Remark:\"" + CurtOrder.Remark + "\",";
            str += " isTaper:\"" + CurtOrder.isTaper + "\",";
            str += " TaperLength:" + CurtOrder.TaperLength + ",";
            str += " Reserve3:\"" + SerializeInlineSlitReserve(CurtOrder) + "\",";
            str += @" Reserve4:"" "",";
            str += " SemiAutoList:";
            for (int i = 0; i < steps.Count; i++)
            {
                str += steps[i].行动类型.ToString() + "/" + steps[i].折弯方向.ToString() + "/" + steps[i].折弯角度.ToString() + "/";
                str += steps[i].回弹值.ToString() + "/" + steps[i].后挡位置.ToString() + "/" + steps[i].抓取类型.ToString() + "/";
                str += steps[i].松开高度.ToString() + "/" + steps[i].翻板收缩值.ToString() + "/" + steps[i].重新抓取.ToString() + ",";
            }
            str += " pxList:";
            for (int i = 0; i < CurtOrder.pxList.Count; i++)
            {
                str += CurtOrder.pxList[i].X.ToString() + "/" + CurtOrder.pxList[i].Y.ToString() + ",";
            }
            str += "LengthAngle:";
            str += CurtOrder.lengAngle[0].Length.ToString() + "/" + CurtOrder.lengAngle[0].TaperWidth.ToString() + "/" + CurtOrder.lengAngle[0].Angle.ToString() + ",";
            int j = 1;
            while (CurtOrder.lengAngle[j].Angle > 0 || CurtOrder.lengAngle[j].Length > 0)
            {
                str += CurtOrder.lengAngle[j].Length.ToString() + "/" + CurtOrder.lengAngle[j].TaperWidth.ToString() + "/" + CurtOrder.lengAngle[j].Angle.ToString() + ",";
                j++;
            }
            str += CurtOrder.lengAngle[99].Length.ToString() + "/" + CurtOrder.lengAngle[99].TaperWidth.ToString() + "/" + CurtOrder.lengAngle[99].Angle.ToString() + ",";

            return str;
        }

        private IReadOnlyList<SemiAutoType>? GetCurrentManualStepsForExport()
        {
            if (subOPManual != null && subOPManual.HasGridSteps())
                return subOPManual.BuildGridSemiAutoSnapshot();

            return null;
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            //wrtAngleMap("[bense0.6]");
            wrtConfigFile("[ClampHeight]", 3);
        }
        private void adsClient_AdsNotificationEx(object sender, AdsNotificationExEventArgs e)
        {
            //TextBox textBox = (TextBox)e.UserData;
            Type type = e.Value.GetType();
            int NotiHandle = e.NotificationHandle;
            if (type == typeof(string) || type.IsPrimitive)
                //textBox.Text = e.Value.ToString();
                ;
            else if (type == typeof(bool[]))
            {
                Hmi_bArray = (bool[])e.Value;
            }
            else if (type == typeof(Int16[]) && NotiHandle == 2)     //1.Hmi_bArray  2.Hmi_iArray    3.Hmi_rArray    4.Hmi_iSemiAuto 5.Hmi_iAuto 6.Hmi_iSlitter
            {
                Hmi_iArray = (Int16[])e.Value;
            }
            else if (type == typeof(Int16[]) && NotiHandle == 4)
            {
                Hmi_iSemiAuto = (Int16[])e.Value;
            }
            else if (type == typeof(Int16[]) && NotiHandle == 5)
            {
                Hmi_iAuto = (Int16[])e.Value;
            }

            else if (type == typeof(Int16[]) && NotiHandle == 7)
            {
                Hmi_iAngleMapTop = (Int16[])e.Value;
            }
            else if (type == typeof(Int16[]) && NotiHandle == 8)
            {
                Hmi_iAngleMapBtm = (Int16[])e.Value;
            }
            else if (type == typeof(Int16[]) && NotiHandle == 9)
            {
                Hmi_iHeightMap = (Int16[])e.Value;
            }
            //else if (type == typeof(float[]))
            //{
            //    Hmi_rArray = (float[])e.Value;
            //}
            else if (type == typeof(float[]) && NotiHandle == 3)
            {
                Hmi_rArray = (float[])e.Value;
            }


        }
        private void AdsConnect()
        {
            return;
            //ADS
            adsClient = new TcAdsClient();
            notificationHandles = new ArrayList();
            byte[] netidva = new byte[6];

            netidva[0] = 192;
            netidva[1] = 168;
            netidva[2] = 78;
            netidva[3] = 10;

            netidva[4] = 1;
            netidva[5] = 1;
            AmsNetId netid = new AmsNetId(netidva);
            adsClient.AdsNotificationEx += new AdsNotificationExEventHandler(adsClient_AdsNotificationEx);
            try
            {
                adsClient.Connect(netid, 851);

                H_bArray = adsClient.CreateVariableHandle("GVL.Hmi_bArray");     //必须要加"."
                H_iArray = adsClient.CreateVariableHandle("GVL.Hmi_iArray");
                H_rArray = adsClient.CreateVariableHandle("GVL.Hmi_rArray");
                H_iSemiAuto = adsClient.CreateVariableHandle("GVL.Hmi_iSemiAuto");
                H_iAuto = adsClient.CreateVariableHandle("GVL.Hmi_iAuto");
                H_rSlitter = adsClient.CreateVariableHandle("GVL.Hmi_rSlitter");
                H_iAngleMapTop = adsClient.CreateVariableHandle("GVL.AngleMap_Top");
                H_iAngleMapBtm = adsClient.CreateVariableHandle("GVL.AngleMap_Btm");
                H_iHeightMap = adsClient.CreateVariableHandle("GVL.HeightMap");
                H_handle1 = adsClient.CreateVariableHandle("GVL.Hmi_bArray[20]");
                H_rAdvPara = adsClient.CreateVariableHandle("GVL.AdvPara");
                AdsConn = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                AdsConn = false;
            }
        }

        bool bNotification;
        public void AdsNotificationEn()
        {
            bNotification = true;
            if (bNotification)
            {
                notificationHandles.Clear();
                try
                {
                    //register notification     
                    notificationHandles.Add(adsClient.AddDeviceNotificationEx("GVL.Hmi_bArray", AdsTransMode.OnChange, 300, 0, Hmi_bArray, typeof(Boolean[]), new int[] { 300 }));
                    notificationHandles.Add(adsClient.AddDeviceNotificationEx("GVL.Hmi_iArray", AdsTransMode.OnChange, 330, 0, Hmi_iArray, typeof(Int16[]), new int[] { 300 }));
                    notificationHandles.Add(adsClient.AddDeviceNotificationEx("GVL.Hmi_rArray", AdsTransMode.OnChange, 370, 0, Hmi_rArray, typeof(float[]), new int[] { 300 }));
                    notificationHandles.Add(adsClient.AddDeviceNotificationEx("GVL.Hmi_iSemiAuto", AdsTransMode.OnChange, 1000, 0, Hmi_iSemiAuto, typeof(Int16[]), new int[] { 300 }));
                    notificationHandles.Add(adsClient.AddDeviceNotificationEx("GVL.Hmi_iAuto", AdsTransMode.OnChange, 1000, 0, Hmi_iAuto, typeof(Int16[]), new int[] { 300 }));
                    notificationHandles.Add(adsClient.AddDeviceNotificationEx("GVL.Hmi_rSlitter", AdsTransMode.OnChange, 1000, 0, Hmi_rSlitter, typeof(float[]), new int[] { 30 }));
                    notificationHandles.Add(adsClient.AddDeviceNotificationEx("GVL.AngleMap_Top", AdsTransMode.OnChange, 1099, 0, Hmi_iAngleMapTop, typeof(Int16[]), new int[] { 200 }));
                    notificationHandles.Add(adsClient.AddDeviceNotificationEx("GVL.AngleMap_Btm", AdsTransMode.OnChange, 1099, 0, Hmi_iAngleMapBtm, typeof(Int16[]), new int[] { 200 }));
                    notificationHandles.Add(adsClient.AddDeviceNotificationEx("GVL.HeightMap", AdsTransMode.OnChange, 1099, 0, Hmi_iHeightMap, typeof(Int16[]), new int[] { 400 }));
                    notificationHandles.Add(adsClient.AddDeviceNotificationEx("GVL.AdvPara", AdsTransMode.OnChange, 1000, 0, Hmi_rAdvPara, typeof(float[]), new int[] { 100 }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                //delete registered notifications.
                try
                {
                    foreach (int handle in notificationHandles)
                        adsClient.DeleteDeviceNotification(handle);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                notificationHandles.Clear();
            }
        }
        public void AdsWritePlc()
        {
            if (AdsConn)       //连接上时才通信,免得在测试时总是出现提示框
            {
                try
                {
                    adsClient.WriteAny(H_bArray, Hmi_bArray);
                    adsClient.WriteAny(H_iArray, Hmi_iArray);
                    adsClient.WriteAny(H_rArray, Hmi_rArray);
                    adsClient.WriteAny(H_iSemiAuto, Hmi_iSemiAuto);
                    adsClient.WriteAny(H_iAuto, Hmi_iAuto);
                    adsClient.WriteAny(H_rSlitter, Hmi_rSlitter);
                    adsClient.WriteAny(H_iAngleMapTop, Hmi_iAngleMapTop);
                    adsClient.WriteAny(H_iAngleMapBtm, Hmi_iAngleMapBtm);
                    adsClient.WriteAny(H_iHeightMap, Hmi_iHeightMap);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            AdsWritePlc1float(30, (float)23.78);
        }
        public void AdsWritePlc1Bit(int addr, bool b1)
        {
            if (AdsConn)       //连接上时才通信,免得在测试时总是出现提示框
            {
                try
                {
                    int H_handle = adsClient.CreateVariableHandle("GVL.Hmi_bArray[" + addr.ToString() + "]");
                    adsClient.WriteAny(H_handle, b1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public void AdsWritePlc1Int(int addr, Int16 int1)
        {
            if (AdsConn)       //连接上时才通信,免得在测试时总是出现提示框
            {
                try
                {
                    int H_handle = adsClient.CreateVariableHandle("GVL.Hmi_iArray[" + addr.ToString() + "]");
                    adsClient.WriteAny(H_handle, int1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public void AdsWritePlc1float(int addr, float flt1)
        {
            if (AdsConn)       //连接上时才通信,免得在测试时总是出现提示框
            {
                try
                {
                    int H_handle = adsClient.CreateVariableHandle("GVL.Hmi_rArray[" + addr.ToString() + "]");
                    adsClient.WriteAny(H_handle, flt1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public void AdsWritePlcFloat()
        {
            if (AdsConn)       //连接上时才通信,免得在测试时总是出现提示框
            {
                try
                {
                    adsClient.WriteAny(H_rArray, Hmi_rArray);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public void AdsWritePlc_SemiAuto()
        {
            if (AdsConn)       //连接上时才通信,免得在测试时总是出现提示框
            {
                try
                {
                    adsClient.WriteAny(H_iSemiAuto, Hmi_iSemiAuto);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public void AdsWritePlc_AngleMap()
        {
            if (AdsConn)       //连接上时才通信,免得在测试时总是出现提示框
            {
                try
                {
                    adsClient.WriteAny(H_iAngleMapTop, Hmi_iAngleMapTop);
                    adsClient.WriteAny(H_iAngleMapBtm, Hmi_iAngleMapBtm);
                    adsClient.WriteAny(H_iHeightMap, Hmi_iHeightMap);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public void AdsWritePlc_AdvPara()
        {
            if (AdsConn)       //连接上时才通信,免得在测试时总是出现提示框
            {
                try
                {
                    adsClient.WriteAny(H_rAdvPara, Hmi_rAdvPara);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void lbAdsConn_Click(object sender, EventArgs e)
        {
            AdsConnEx();
        }
        void AdsConnEx()
        {
            //return;
            if (!AdsConn)
                AdsConnect();
            if (AdsConn)
            {
                //MessageBox.Show("Connect PLC Successful!", "CONNECT", MessageBoxButtons.OK);
                lbAdsConn.BackColor = Color.Green;
                AdsNotificationEn();
            }
            else
                lbAdsConn.BackColor = Color.Red;
        }

        private void lbExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void sw单工双工_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[0] = !MainFrm.Hmi_bArray[0];
            sw单工双工.BackgroundImage = MainFrm.Hmi_bArray[0] ? global::JSZW1000A.Properties.Resources.btm_单工双工1 : global::JSZW1000A.Properties.Resources.btm_单工双工0;

            AdsWritePlc1Bit(0, MainFrm.Hmi_bArray[0]);

            MainFrm.ConfigData[MainFrm.L1_GlobalSwitch + 1] = Convert.ToSingle(MainFrm.Hmi_bArray[0]);
            wrtConfigFile("[GlobalSwitch]", 1);
        }
        private void pnl双工_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[0] = true;
            AdsWritePlc1Bit(0, MainFrm.Hmi_bArray[0]);

            MainFrm.ConfigData[MainFrm.L1_GlobalSwitch + 1] = Convert.ToSingle(MainFrm.Hmi_bArray[0]);
            wrtConfigFile("[GlobalSwitch]", 1);
        }

        private void pnl单工_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[0] = false;
            AdsWritePlc1Bit(0, MainFrm.Hmi_bArray[0]);

            MainFrm.ConfigData[MainFrm.L1_GlobalSwitch + 1] = Convert.ToSingle(MainFrm.Hmi_bArray[0]);
            wrtConfigFile("[GlobalSwitch]", 1);
        }

        private void sw轻型重型_Click(object sender, EventArgs e)
        {
            string s0 = "";
            if (MainFrm.Hmi_bArray[1])
                s0 = Strings.Get("MainFrm.SwitchHeavyConfirm");
            else
                s0 = Strings.Get("MainFrm.SwitchLightConfirm");
            DialogAsk dlgTips = new DialogAsk("", s0);
            dlgTips.StartPosition = FormStartPosition.Manual;
            dlgTips.Location = new Point(500, 200);

            if (dlgTips.ShowDialog() == DialogResult.OK)
            {
                MainFrm.Hmi_bArray[1] = !MainFrm.Hmi_bArray[1];
                sw轻型重型.BackgroundImage = MainFrm.Hmi_bArray[1] ? global::JSZW1000A.Properties.Resources.btm_轻重型1 : global::JSZW1000A.Properties.Resources.btm_轻重型0;
                AdsWritePlc1Bit(1, MainFrm.Hmi_bArray[1]);

                MainFrm.ConfigData[MainFrm.L1_GlobalSwitch + 2] = Convert.ToSingle(MainFrm.Hmi_bArray[1]);
                wrtConfigFile("[GlobalSwitch]", 2);
            }
            else
            {
                ;
            }
        }
        private void pnl轻型_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[1] = true;
            AdsWritePlc1Bit(1, MainFrm.Hmi_bArray[1]);

            MainFrm.ConfigData[MainFrm.L1_GlobalSwitch + 2] = Convert.ToSingle(MainFrm.Hmi_bArray[1]);
            wrtConfigFile("[GlobalSwitch]", 2);
        }

        private void pnl重型_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[1] = false;
            AdsWritePlc1Bit(1, MainFrm.Hmi_bArray[1]);

            MainFrm.ConfigData[MainFrm.L1_GlobalSwitch + 2] = Convert.ToSingle(MainFrm.Hmi_bArray[1]);
            wrtConfigFile("[GlobalSwitch]", 2);
        }

        //private void btn构图完成_Click(object sender, EventArgs e)
        //{
        //    MainFrm.CurtOrder.pxList = MainFrm.QuickDrawList.GetRange(0, MainFrm.QuickDrawList.Count);
        //    //Cancel Mode
        //    MainFrm.Hmi_iArray[0] = 3;
        //    AdsWritePlc1Int(0, MainFrm.Hmi_iArray[0]);
        //    切入自动1(true, true);
        //}
        private void btn构图完成_Click(object sender, EventArgs e)
        {
            // 1. 检查当前是否在“快速构图”界面
            // 确保 subOPAutoDraw 实例存在，且正在显示
            if (subOPAutoDraw == null)
            {
                return; // 或者提示错误
            }

            // 2. 遥控子控件进行计算
            // 调用我们在第一步写的 public 方法
            bool isSuccess = subOPAutoDraw.ExecuteCalculation();

            // 如果计算失败（比如没画图），就终止，不跳转
            if (!isSuccess) return;

            // 3. 写入 PLC 状态 (退出画图模式)
            MainFrm.Hmi_iArray[0] = 3;
            AdsWritePlc1Int(0, MainFrm.Hmi_iArray[0]);

            // 4. 跳转到 Auto1 界面
            // 参数说明：
            // isCal = false：数据已经算好了，SubOPAuto1 直接根据数据画图，不要反算
            // 需生成序列 = true：这是新图，需要生成折弯序列
            切入自动1(false, true);
        
        }
        public void 切入自动1(bool isCal, bool 需生成序列)     //是否计算长度角度
        {
            // 设置生产序列已生成状态
            CurtOrder.生产序列已生成 = !需生成序列;

            // 创建并显示 SubOPAuto1 子窗口
            subOPAuto1 = new SubWindows.SubOPAuto(this, isCal);
            subOPAuto1.Show();

            // 清空并添加子窗口到控件集合
            gpbSubWin.Controls.Clear();
            gpbSubWin.Controls.Add(subOPAuto1);

            // 更改锥度设定
            Change锥度设定(CurtOrder.isTaper);

            // 设置自动工作单选值并显示工作单
            db自动工作单选 = 1;
            工作单显示();

            // 设置面板可见性
            pnl锥度设定.Visible = pnl自动4视图.Visible = pnl角度尺寸.Visible = true;
            txb锥度长度.Visible = lb锥度单位.Visible = false;

            // 设置按钮可见性
            btn折弯预览.Visible = true;
            btn折弯预览.Text = Strings.Get("MainFrm.FoldPreview.Layout");
            btn折弯预览.Image = global::JSZW1000A.Properties.Resources._123;
            btn折弯预览.Tag = "layout";
            btn重置视图.Visible = btn颜色侧翻.Visible = btn发送到半自动.Visible = btn撤消.Visible = btn重复.Visible = true;
            btn构图完成.Visible = btn手动强制.Visible = btnFeed.Visible = false;


            // 设置导航按钮颜色
            btn导航_自动.ForeColor = Color.FromArgb(96, 176, 255);
            btn导航_手动.ForeColor = btn导航_库.ForeColor = btn导航_设置.ForeColor = btn导航_分条.ForeColor = Color.White;
        }

        public void 切入自动绘图()
        {
            // 创建并显示 SubOPAutoDraw 子窗口
            subOPAutoDraw = new SubWindows.SubOPAutoDraw(this);
            subOPAutoDraw.Show();

            // 清空并添加子窗口到控件集合
            gpbSubWin.Controls.Clear();
            gpbSubWin.Controls.Add(subOPAutoDraw);

            // 设置面板可见性
            pnl锥度设定.Visible = pnl角度尺寸.Visible = false;
            pnl自动4视图.Visible = true;

            // 设置按钮可见性
            btn折弯预览.Visible = btn重置视图.Visible = btn颜色侧翻.Visible = btn发送到半自动.Visible = btn撤消.Visible = btn重复.Visible = false;
            btn构图完成.Visible = true;

            // 设置导航按钮颜色
            btn导航_自动.ForeColor = Color.FromArgb(96, 176, 255);
            btn导航_手动.ForeColor = btn导航_库.ForeColor = btn导航_设置.ForeColor = Color.White;
        }

        public double db工作单子项 = 0;
        private void lb工作单_角度尺寸_Click(object sender, EventArgs e)
        {
            Label btn = (Label)sender;
            if (btn.Name == "lb工作单_角度尺寸") { db工作单子项 = 0; }
            else if (btn.Name == "lb工作单_尺寸") { db工作单子项 = 1; }
            else if (btn.Name == "lb工作单_角度") { db工作单子项 = 2; }
            else if (btn.Name == "比例") { db工作单子项 = 3; }
            subOPAuto1.controlVis((int)db工作单子项);
            工作单子项显示();
        }
        private void btn角度尺寸_Click(object sender, EventArgs e)
        {
            db工作单子项++;
            db工作单子项 = (db工作单子项) % 4;
            subOPAuto1.controlVis((int)db工作单子项);
            工作单子项显示();
        }
        private void 工作单子项显示()
        {
            if ((int)db工作单子项 == 0)
                btn角度尺寸.Image = global::JSZW1000A.Properties.Resources.btm_4档开关1;
            else if ((int)db工作单子项 == 1)
                btn角度尺寸.Image = global::JSZW1000A.Properties.Resources.btm_4档开关2;
            else if ((int)db工作单子项 == 2)
                btn角度尺寸.Image = global::JSZW1000A.Properties.Resources.btm_4档开关3;
            else if ((int)db工作单子项 == 3)
                btn角度尺寸.Image = global::JSZW1000A.Properties.Resources.btm_4档开关4;

            lb工作单_角度尺寸.ForeColor = ((int)db工作单子项 == 0) ? Color.FromArgb(96, 176, 255) : Color.White;
            lb工作单_尺寸.ForeColor = ((int)db工作单子项 == 1) ? Color.FromArgb(96, 176, 255) : Color.White;
            lb工作单_角度.ForeColor = ((int)db工作单子项 == 2) ? Color.FromArgb(96, 176, 255) : Color.White;
            lb工作单_比例.ForeColor = ((int)db工作单子项 == 3) ? Color.FromArgb(96, 176, 255) : Color.White;

        }

        private void btn保存_Click(object sender, EventArgs e)
        {

            // 获取当前配置的文件路径
            string baseDirectory = MainFrm.ConfigStr[1];


            // 如果还没有关联的文件路径，使用另存为逻辑
            if (string.IsNullOrEmpty(MainFrm.ConfigStr[1]))
            {
                btn另存为_Click(sender, e);
                return;
            }

            try
            {
                // 确保目录存在
                if (!Directory.Exists(baseDirectory))
                {
                    Directory.CreateDirectory(baseDirectory);
                }

                // 构建完整文件路径
                string fileName = $"{CurtOrder.Name}.ini";
                string filePath = Path.Combine(baseDirectory, fileName);

                // 生成订单内容
                string orderContent = Generate1Order(CurtOrder.Name, GetCurrentManualStepsForExport());

                if (string.IsNullOrEmpty(orderContent))
                {
                    MessageBox.Show(Strings.Get("MainFrm.Save.EmptyOrderData"));
                    return;
                }
                // 保存文件（无论是否存在都直接覆盖）
                File.WriteAllText(filePath, orderContent);

                // 更新状态
                Console.WriteLine(string.Format(Strings.Get("MainFrm.Save.LogSaved"), filePath));
                MessageBox.Show(string.Format(Strings.Get("MainFrm.Save.SuccessMessage"), CurtOrder.Name),
                                Strings.Get("MainFrm.Save.SuccessTitle"),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Strings.Get("MainFrm.Save.LogSaveError"), ex.Message));
                MessageBox.Show(string.Format(Strings.Get("MainFrm.Save.FailedMessage"), ex.Message),
                                Strings.Get("Common.ErrorTitle"),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void btn另存为_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = Strings.Get("MainFrm.Save.FileFilter");
            saveFileDialog1.Title = Strings.Get("MainFrm.Save.FileDialogTitle");
            saveFileDialog1.InitialDirectory = @"C:\Jing Gong Flashings\Library";
            string Autosavename = AutoLoadName();
            saveFileDialog1.FileName = Autosavename;
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // 如果用户点击了"保存"按钮
            {
                string filePath = saveFileDialog1.FileName;

                CurtOrder.Name = Path.GetFileNameWithoutExtension(filePath);
                string s = Generate1Order(CurtOrder.Name, GetCurrentManualStepsForExport());

                // 如果文件已存在，则清空原文件内容
                if (File.Exists(filePath))
                {
                    try
                    {
                        // 清空文件内容
                        File.WriteAllText(filePath, "");
                        // 写入新内容
                        File.AppendAllText(filePath, s);
                        Console.WriteLine(Strings.Get("MainFrm.Save.LogOverwritten"));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format(Strings.Get("MainFrm.Save.LogSaveError"), ex.Message));
                    }
                }
                else // 文件名不重复，直接创建新文件并保存
                {
                    try
                    {
                        // 创建新文件并写入内容
                        File.WriteAllText(filePath, s);
                        Console.WriteLine(Strings.Get("MainFrm.Save.LogCreated"));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format(Strings.Get("MainFrm.Save.LogCreateError"), ex.Message));
                    }
                }
            }
            else
            {
                Console.WriteLine(Strings.Get("MainFrm.Save.LogCancelled"));
            }
        }

        private string AutoLoadName()
        {
            System.DateTime ctm = new System.DateTime();
            ctm = System.DateTime.Now; // 取当前年月日时分秒
            string s = ctm.Year.ToString() + String.Format("{0:D2}", ctm.Month)
                + String.Format("{0:D2}", ctm.Day) + "_" + String.Format("{0:D2}", ctm.Hour) +
                String.Format("{0:D2}", ctm.Minute);
            string s1 = s.Substring(2, s.Length - 2);
            string s2 = "Fold_" + s1;
            return s2;
        }

        double db自动工作单选 = 0;

        private void lb自动_折弯预览_Click(object sender, EventArgs e)
        {
            db自动工作单选 = 2;
            切入折弯预览(true);
        }

        private void lb自动_折弯生产_Click(object sender, EventArgs e)
        {
            db自动工作单选 = 3;
            切入折弯预览(true);
            btn折弯预览.Visible = false;
        }

        private void 切入折弯预览(bool proc)      //true:折弯预览 false:折弯设置
        {
            // 手动在 set 页调整过步骤时，预览直接使用当前步骤表，不再重建覆盖。
            if (HasManualSemiAutoEdits())
            {
                NormalizeGeneratedSemiAutoSequence();
                CurtOrder.生产序列已生成 = true;
            }
            // 检查生产序列是否生成，若未生成则创建
            else if (!CurtOrder.生产序列已生成)
            {
                create生产序列();
                CurtOrder.生产序列已生成 = true;
            }

            // 创建并显示 SubOPAutoView 子窗口
            if (proc)
            {
                subOPAutoView = new SubWindows.SubOPAutoView(this, true);
                subOPAutoView.Show();
            }

            // 清空并添加子窗口到控件集合
            gpbSubWin.Controls.Clear();
            if (proc)
            {
                gpbSubWin.Controls.Add(subOPAutoView);
            }
            else
            {
                subOPAutoSet = new SubWindows.SubOPAutoSet(this);
                subOPAutoSet.Show();
                gpbSubWin.Controls.Add(subOPAutoSet);
            }

            if (!proc)
            {
                db自动工作单选 = 1;
            }

            // 显示工作单
            工作单显示();

            // 设置面板可见性
            pnl锥度设定.Visible = pnl自动4视图.Visible = true;
            pnl角度尺寸.Visible = false;

            // 设置按钮可见性
            btn重置视图.Visible = true;
            btn颜色侧翻.Visible = btn撤消.Visible = btn重复.Visible = false;
            btn发送到半自动.Visible = true;
            btn构图完成.Visible = false;

            // 设置按钮文本和图片
            if (proc)
            {
                btn折弯预览.Visible = false;
                btn折弯预览.Tag = null;
            }
            else
            {
                btn折弯预览.Visible = false;
                btn折弯预览.Tag = null;
            }
        }

        private void lb自动_快速构图_Click(object sender, EventArgs e)
        {
            切入自动绘图();
            db自动工作单选 = 0;
            工作单显示();
        }

        private void lb自动_工作单设定_Click(object sender, EventArgs e)
        {
            切入自动1(false, false);
            db自动工作单选 = 1;
            工作单显示();
        }

        private void btn自动_工作单选_Click(object sender, EventArgs e)
        {
            db自动工作单选++;
            db自动工作单选 = (db自动工作单选) % 4;
            工作单显示();
            if (db自动工作单选 == 0)
                切入自动绘图();
            else if (db自动工作单选 == 1)
                切入自动1(false, false);
            else if (db自动工作单选 == 2)
                切入折弯预览(true);
            else if (db自动工作单选 == 3)
            {
                切入折弯预览(true);
            }

        }

        private void EnterMainManualPage()
        {
            btn条款确定.Visible = false;

            if (SubCheckItem != null)
            {
                gpbSubWin.Controls.Remove(SubCheckItem);
                SubCheckItem.Dispose();
                SubCheckItem = null;
            }

            InitAct();
            fun导航_手动();
            ConfigData[8] = (float)MainFrm.Lang;
            wrtConfigFile("[GlobalSwitch]", 8);
        }

        private void 工作单显示()
        {
            if ((int)db自动工作单选 == 0)
                btn自动_工作单选.Image = global::JSZW1000A.Properties.Resources.btm_4档开关1;
            else if ((int)db自动工作单选 == 1)
                btn自动_工作单选.Image = global::JSZW1000A.Properties.Resources.btm_4档开关2;
            else if ((int)db自动工作单选 == 2)
                btn自动_工作单选.Image = global::JSZW1000A.Properties.Resources.btm_4档开关3;
            else if ((int)db自动工作单选 == 3)
                btn自动_工作单选.Image = global::JSZW1000A.Properties.Resources.btm_4档开关4;

            lb自动_快速构图.ForeColor = ((int)db自动工作单选 == 0) ? Color.FromArgb(96, 176, 255) : Color.White;
            lb自动_工作单设定.ForeColor = ((int)db自动工作单选 == 1) ? Color.FromArgb(96, 176, 255) : Color.White;
            lb自动_折弯预览.ForeColor = ((int)db自动工作单选 == 2) ? Color.FromArgb(96, 176, 255) : Color.White;
            lb自动_折弯生产.ForeColor = ((int)db自动工作单选 == 3) ? Color.FromArgb(96, 176, 255) : Color.White;
        }

        private void cbx材料选择_SelectedIndexChanged(object sender, EventArgs e)
        {
            //write para
            int id = cbx材料选择.SelectedIndex;
            for (int i = 0; i < 15; i++)
            {
                MainFrm.Hmi_rArray[150 + i] = MainFrm.angleAddit[id].AngleRange[i];
                MainFrm.Hmi_rArray[170 + i] = MainFrm.angleAddit[id].AngleRange[i + 15];
            }
            AdsWritePlcFloat();
            //write 1 flag
            MainFrm.Hmi_rArray[56] = Convert.ToInt16(cbx材料选择.SelectedIndex);
            MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 9] = MainFrm.Hmi_rArray[56];
            AdsWritePlc1float(56, MainFrm.Hmi_rArray[56]);
            wrtConfigFile("[ManualOldSelect]", 9);
        }

        private void btn发送到半自动_Click(object sender, EventArgs e)
        {
            if (HasManualSemiAutoEdits())
            {
                NormalizeGeneratedSemiAutoSequence();
                CurtOrder.生产序列已生成 = true;
            }
            else
            {
                create生产序列();
            }
            subOPManual?.LoadGridFromCurrentOrder();
            MainFrm.Hmi_bArray[60] = false;
            btn导航_手动.PerformClick();
        }

        public void create生产序列()
        {
            ResetSemiAutoManualEditFlag();
            bool savedSlitter = CurtOrder.isSlitter;

            if (TryBuildInlineSlitSequenceFromCurrentOrder())
            {
                CurtOrder.isSlitter = savedSlitter;
                return;
            }

            CurtOrder.isSlitter = savedSlitter;
            create标准生产序列();
        }

        private bool TryBuildInlineSlitSequenceFromCurrentOrder()
        {
            if (!TryGetInlineSlitPlan(out InlineSlitPlan plan))
            {
                return false;
            }

            bool savedSlitter = CurtOrder.isSlitter;
            CurtOrder.isSlitter = false;
            create标准生产序列();

            List<SemiAutoType> baseSteps = new List<SemiAutoType>(CurtOrder.lstSemiAuto);
            bool built = TryBuildInlineSlitSequence(baseSteps, plan, out List<SemiAutoType> sequence);
            if (built)
            {
                CurtOrder.lstSemiAuto = sequence;
                NormalizeGeneratedSemiAutoSequence();
            }

            CurtOrder.isSlitter = savedSlitter;
            return built;
        }

        private void create标准生产序列()
        {
            bool is逆序 = MainFrm.CurtOrder.st逆序;
            bool is色下 = MainFrm.CurtOrder.st色下;

            MainFrm.CurtOrder.TaperLength = MainFrm.ParseDisplayLengthOrZero(txb锥度长度.Text);
            int idx0 = 1;
            double TaperExWidth = 0;
            while (MainFrm.CurtOrder.lengAngle[idx0].TaperWidth > 0)
            {
                TaperExWidth += MainFrm.CurtOrder.lengAngle[idx0].TaperWidth;
                idx0++;
            }
            TaperExWidth += MainFrm.CurtOrder.lengAngle[0].TaperWidth;
            TaperExWidth += MainFrm.CurtOrder.lengAngle[99].TaperWidth;

            if (MainFrm.CurtOrder.isTaper)
                CurtOrder.isSlitter = true;

            CurtOrder.lstSemiAuto.Clear();
            int idx = 0;
            double ExLength = CurtOrder.Width;
            SemiAutoType odrSemi = new SemiAutoType();
            if (CurtOrder.isSlitter)
            {
                odrSemi.折弯序号 = (idx + 1);
                odrSemi.行动类型 = SemiAutoActionSlit;
                odrSemi.折弯方向 = 0;
                odrSemi.折弯角度 = 888.00;
                odrSemi.回弹值 = 4.00;
                odrSemi.后挡位置 = RoundBackGaugePosition(ExLength);
                odrSemi.抓取类型 = 1;
                odrSemi.松开高度 = 0;
                odrSemi.翻板收缩值 = GetDefaultSemiAutoRetractValue();
                odrSemi.重新抓取 = 0;
                odrSemi.锥度斜率 = (CurtOrder.TaperLength == 0) ? 0 : (TaperExWidth - ExLength) / CurtOrder.TaperLength * 100000;
                idx++;
                CurtOrder.lstSemiAuto.Add(odrSemi);
            }

            // 处理首尾挤压/拍扁操作时，添加逆序超程逻辑0
            int i1 = 0, iAddr = 0;
            if (!is逆序)  //正序
                iAddr = 99;
            else
                iAddr = 0;
            while (i1 < 2)
            {
                if (CurtOrder.lengAngle[iAddr].Angle > 0 && CurtOrder.lengAngle[iAddr].Length > 0)
                {
                    int _bSquashDir = 0;    //0:向上  1:向下
                    ExLength -= CurtOrder.lengAngle[iAddr].Length;
                    TaperExWidth -= CurtOrder.lengAngle[iAddr].TaperWidth;
                    odrSemi.折弯序号 = (idx + 1);
                    odrSemi.行动类型 = SemiAutoActionFold;
                    if ((!is色下 && (CurtOrder.lengAngle[iAddr].Angle == 2 || CurtOrder.lengAngle[iAddr].Angle == 4))
                        || (is色下 && (CurtOrder.lengAngle[iAddr].Angle == 1 || CurtOrder.lengAngle[iAddr].Angle == 3)))
                        odrSemi.折弯方向 = _bSquashDir = 1;
                    else
                        odrSemi.折弯方向 = _bSquashDir = 0;
                    odrSemi.折弯角度 = (iAddr == 0) ? 3.001 : 3.99;
                    odrSemi.回弹值 = 0.00;
                    odrSemi.后挡位置 = RoundBackGaugePosition(ExLength);
                    if (i1 == 0)
                        odrSemi.抓取类型 = 1;
                    else
                        odrSemi.抓取类型 = 2;

                    if (odrSemi.折弯方向 == 0)
                        odrSemi.松开高度 = 0;
                    else
                        odrSemi.松开高度 = 1;



                    odrSemi.翻板收缩值 = GetDefaultSemiAutoRetractValue();
                    odrSemi.重新抓取 = 0;
                    odrSemi.锥度斜率 = (CurtOrder.TaperLength == 0) ? 0 : (TaperExWidth - ExLength) / CurtOrder.TaperLength * 100000;
                    odrSemi.长角序号 = (iAddr == 0) ? 0 : 99;
                    odrSemi.is色下 = is色下;
                    odrSemi.操作提示 = 0;

                    if ((iAddr == 0 && is逆序) || (iAddr == 99 && !is逆序))        //首挤压+逆序,
                    {
                        is色下 = !is色下;

                        odrSemi.操作提示 = 1;
                        is逆序 = !is逆序;       //这种条件下的第1下拍扁为换序
                    }
                    odrSemi.内外选择 = is逆序 ? 1 : 0;
                    if (odrSemi.操作提示 == 1)
                        is逆序 = !is逆序;       //如翻面,再换序
                    if (iAddr == 99)
                        odrSemi.坐标序号 = MainFrm.CurtOrder.pxList.Count - 1;
                    else
                        odrSemi.坐标序号 = 0;

                    idx++;
                    CurtOrder.lstSemiAuto.Add(odrSemi);


                    //  注释挤压作为单独条目程序
                    odrSemi.折弯序号 = (idx + 1);
                    odrSemi.行动类型 = (CurtOrder.lengAngle[iAddr].Angle == 1 || CurtOrder.lengAngle[iAddr].Angle == 2)
                        ? SemiAutoActionSquash
                        : SemiAutoActionOpenSquash;
                    odrSemi.折弯方向 = (CurtOrder.lengAngle[iAddr].Angle == 2 || CurtOrder.lengAngle[iAddr].Angle == 4) ? 1 : 0;
                    odrSemi.折弯角度 = 0.0;
                    odrSemi.回弹值 = 0.00;
                    double d1 = 0;
                    if (_bSquashDir == 0 && (CurtOrder.lengAngle[iAddr].Angle == 1 || CurtOrder.lengAngle[iAddr].Angle == 2))
                        d1 = Hmi_rArray[115];
                    else if (_bSquashDir == 0 && (CurtOrder.lengAngle[iAddr].Angle == 3 || CurtOrder.lengAngle[iAddr].Angle == 4))
                        d1 = Hmi_rArray[118];
                    else if (_bSquashDir == 1 && (CurtOrder.lengAngle[iAddr].Angle == 1 || CurtOrder.lengAngle[iAddr].Angle == 2))
                        d1 = Hmi_rArray[116];
                    else if (_bSquashDir == 1 && (CurtOrder.lengAngle[iAddr].Angle == 3 || CurtOrder.lengAngle[iAddr].Angle == 4))
                        d1 = Hmi_rArray[119];

                    odrSemi.后挡位置 = RoundBackGaugePosition(ExLength + d1);
                    odrSemi.抓取类型 = CurtOrder.lstSemiAuto[CurtOrder.lstSemiAuto.Count - 1].抓取类型;

                    if (i1 == 0 && CurtOrder.lengAngle[99 - iAddr].Angle > 0 && CurtOrder.lengAngle[99 - iAddr].Length > 0)
                        odrSemi.松开高度 = 2;
                    else
                        odrSemi.松开高度 = 0;
                    odrSemi.翻板收缩值 = GetDefaultSemiAutoRetractValue();
                    odrSemi.重新抓取 = 0;
                    odrSemi.锥度斜率 = (CurtOrder.TaperLength == 0) ? 0 : (TaperExWidth - ExLength) / CurtOrder.TaperLength * 100000;
                    idx++;
                    CurtOrder.lstSemiAuto.Add(odrSemi);
                }
                iAddr = 99 - iAddr;
                i1++;
            }

            int i = 1;
            int max = 0;
            while (CurtOrder.lengAngle[i].Length > 0)
                i++;
            max = i; i = 1;

            if (!is逆序)   //b正逆序 false时,为正序
            {
                for (int j = 1; j < max - 1; j++)      //正序
                {
                    ExLength -= CurtOrder.lengAngle[j].Length;
                    TaperExWidth -= CurtOrder.lengAngle[j].TaperWidth;
                    odrSemi.折弯序号 = (idx + 1);
                    odrSemi.行动类型 = SemiAutoActionFold;
                    if ((!is色下 && (CurtOrder.lengAngle[j + 1].Angle > 0)
                            || (is色下 && CurtOrder.lengAngle[j + 1].Angle < 0)))
                        odrSemi.折弯方向 = 0;
                    else
                        odrSemi.折弯方向 = 1;
                    odrSemi.折弯角度 = Math.Abs(CurtOrder.lengAngle[j + 1].Angle);
                    odrSemi.回弹值 = (CurtOrder.lengAngle[j + 1].Angle > 0) ? CurtOrder.TopSpring : CurtOrder.BtmSpring;
                    odrSemi.后挡位置 = RoundBackGaugePosition(ExLength);
                    if (CurtOrder.lengAngle[99].Angle != 0)
                    {
                        odrSemi.抓取类型 = 2; // 挤压操作强制超程夹取
                    }
                    else
                    {
                        if (ExLength >= 20 && ExLength <= 45 && odrSemi.抓取类型 == 2)
                            odrSemi.抓取类型 = 1;
                        else if (ExLength < 20)
                            odrSemi.抓取类型 = 0;
                        else if (CurtOrder.lstSemiAuto.Count() > 0)
                            odrSemi.抓取类型 = CurtOrder.lstSemiAuto[CurtOrder.lstSemiAuto.Count() - 1].抓取类型;
                        else
                            odrSemi.抓取类型 = 1;
                    }
                    odrSemi.松开高度 = 0;
                    odrSemi.翻板收缩值 = GetDefaultSemiAutoRetractValue();
                    odrSemi.重新抓取 = 0;
                    odrSemi.锥度斜率 = (CurtOrder.TaperLength == 0) ? 0 : (TaperExWidth - ExLength) / CurtOrder.TaperLength * 100000;
                    odrSemi.操作提示 = 0;
                    odrSemi.长角序号 = j;
                    odrSemi.坐标序号 = j;
                    odrSemi.内外选择 = is逆序 ? 1 : 0;
                    odrSemi.is色下 = is色下;
                    idx++;
                    CurtOrder.lstSemiAuto.Add(odrSemi);
                    i++;
                }
            }
            else        //b正逆序 true时,为逆序
            {
                for (int j = max - 1; j > 1; j--)     //反序
                {
                    ExLength -= CurtOrder.lengAngle[j].Length;
                    TaperExWidth -= CurtOrder.lengAngle[j].TaperWidth;
                    odrSemi.折弯序号 = (idx + 1);
                    odrSemi.行动类型 = SemiAutoActionFold;
                    if ((!is色下 && CurtOrder.lengAngle[j].Angle > 0)
                            || (is色下 && CurtOrder.lengAngle[j].Angle < 0))
                        odrSemi.折弯方向 = 0;
                    else
                        odrSemi.折弯方向 = 1;
                    odrSemi.折弯角度 = Math.Abs(CurtOrder.lengAngle[j].Angle);
                    odrSemi.回弹值 = (CurtOrder.lengAngle[j].Angle > 0) ? CurtOrder.TopSpring : CurtOrder.BtmSpring;
                    odrSemi.后挡位置 = RoundBackGaugePosition(ExLength);
                    if (is逆序 && (CurtOrder.lengAngle[0].Angle != 0))
                    {
                        odrSemi.抓取类型 = 2; // 挤压操作强制超程夹取
                    }
                    else
                    {
                        if (ExLength >= 20 && ExLength <= 45 && odrSemi.抓取类型 == 2)
                            odrSemi.抓取类型 = 1;
                        else if (ExLength < 20)
                            odrSemi.抓取类型 = 0;
                        else if (CurtOrder.lstSemiAuto.Count() > 0)
                            odrSemi.抓取类型 = CurtOrder.lstSemiAuto[CurtOrder.lstSemiAuto.Count() - 1].抓取类型;
                        else
                            odrSemi.抓取类型 = 1;
                    }
                    odrSemi.松开高度 = 0;
                    odrSemi.翻板收缩值 = GetDefaultSemiAutoRetractValue();
                    odrSemi.重新抓取 = 0;
                    odrSemi.锥度斜率 = (CurtOrder.TaperLength == 0) ? 0 : (TaperExWidth - ExLength) / CurtOrder.TaperLength * 100000;
                    odrSemi.长角序号 = j - 1;
                    odrSemi.操作提示 = 0;
                    odrSemi.坐标序号 = j - 1;
                    odrSemi.内外选择 = is逆序 ? 1 : 0;
                    odrSemi.is色下 = is色下;
                    idx++;
                    CurtOrder.lstSemiAuto.Add(odrSemi);
                    i++;
                }
            }

            NormalizeGeneratedSemiAutoSequence();
        }

        private void btnMsgClr_Click(object sender, EventArgs e)
        {
            richMsgInfo.Clear();
        }

        private void MainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            string message = Strings.Get("MainFrm.Exit.ConfirmMessage");
            string caption = Strings.Get("MainFrm.Exit.ConfirmTitle");
            MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
            DialogResult result = MessageBox.Show(message, caption, buttons);
            if (result != DialogResult.OK)
                e.Cancel = true;
            if (AdsConn)
                adsClient.Dispose();
        }

        private void sw锥度设定_Click(object sender, EventArgs e)
        {
            bool old1 = MainFrm.CurtOrder.isTaper;
            MainFrm.CurtOrder.isTaper = !MainFrm.CurtOrder.isTaper;
            Change锥度设定(!old1 && MainFrm.CurtOrder.isTaper);
        }

        private void Change锥度设定(bool b)
        {
            MainFrm.Hmi_rArray[0] = Convert.ToSingle(MainFrm.CurtOrder.TaperLength);
            txb锥度长度.Text = MainFrm.FormatDisplayLength(MainFrm.CurtOrder.TaperLength);

            sw锥度设定.BackgroundImage = MainFrm.CurtOrder.isTaper ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            lb锥度单位.Visible = txb锥度长度.Visible = MainFrm.CurtOrder.isTaper;

            MainFrm.Hmi_bArray[15] = MainFrm.CurtOrder.isTaper;

            subOPAuto1.reCreateTaperTxb(b);
            AdsWritePlc1Bit(15, MainFrm.Hmi_bArray[15]);
            MainFrm.Hmi_rArray[0] = Convert.ToSingle(MainFrm.CurtOrder.TaperLength);
            AdsWritePlc1float(0, MainFrm.Hmi_rArray[0]);
        }

        private void txb锥度长度_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox txb = (TextBox)sender;
            if (e.KeyCode == Keys.Enter)
            {
                if (MainFrm.TryParseDisplayLength(txb.Text, out double taperLengthMm))
                {
                    MainFrm.CurtOrder.TaperLength = taperLengthMm;
                    txb.Text = MainFrm.FormatDisplayLength(taperLengthMm);
                }
            }
            MainFrm.Hmi_rArray[0] = Convert.ToSingle(MainFrm.CurtOrder.TaperLength);
            AdsWritePlc1float(0, MainFrm.Hmi_rArray[0]);
        }

        private void btn手动强制_Click(object sender, EventArgs e)
        {
            FrmForceMan dlgForceMan = new FrmForceMan(this);
            dlgForceMan.StartPosition = FormStartPosition.Manual;
            dlgForceMan.Location = new Point(270, 300);
            dlgForceMan.ShowDialog();
        }

        private void btn折弯预览_Click(object sender, EventArgs e)
        {
            string? mode = btn折弯预览.Tag as string;
            if (mode == "preview")
            {
                切入折弯预览(true);
            }
            else if (mode == "layout")
            {
                切入折弯预览(false);
            }
        }



        int tLoadData = 0;

        int Delay0 = 0;
        private void timer1s_Tick(object sender, EventArgs e)
        {
            lb实际压力值.Text = Hmi_rArray[32].ToString("f1");
            lbMT1.Text = FormatDisplayLength(Hmi_rArray[21]);
            lbMT2.Text = FormatDisplayLength(Hmi_rArray[22]);
            lbMT3.Text = FormatDisplayLength(Hmi_rArray[23]);
            lbMT4.Text = FormatDisplayLength(Hmi_rArray[24]);
            lbMT5.Text = (lbMT5.Visible) ? FormatDisplayLength(Hmi_rArray[25]) : "0.0";
            lbMT6.Text = (lbMT6.Visible) ? FormatDisplayLength(Hmi_rArray[36]) : "0.0";
            lbMT7.Text = (lbMT7.Visible) ? FormatDisplayLength(Hmi_rArray[37]) : "0.0";
            lbMT8.Text = (lbMT8.Visible) ? FormatDisplayLength(Hmi_rArray[38]) : "0.0";
            lbMT9.Text = (lbMT9.Visible) ? FormatDisplayLength(Hmi_rArray[41]) : "0.0";

            if (Hmi_rArray[32] > 2 && Hmi_rArray[32] <= 5)
                pnlPressure.BackgroundImage = global::JSZW1000A.Properties.Resources.Pressure1;
            else if (Hmi_rArray[32] > 5 && Hmi_rArray[32] <= 8)
                pnlPressure.BackgroundImage = global::JSZW1000A.Properties.Resources.Pressure2;
            else if (Hmi_rArray[32] > 8)
                pnlPressure.BackgroundImage = global::JSZW1000A.Properties.Resources.Pressure3;
            else
                pnlPressure.BackgroundImage = global::JSZW1000A.Properties.Resources.Pressure0;
            if (Hmi_bArray[12])     //激光挡指禁用
                lb激光挡指.Image = global::JSZW1000A.Properties.Resources.LED_red;
            else
                lb激光挡指.Image = Hmi_bArray[23] ? global::JSZW1000A.Properties.Resources.LED_blue : null;
            lbLED_white.Image = Hmi_bArray[24] ? global::JSZW1000A.Properties.Resources.LED_white : null;
            lbLED_yellow.Image = Hmi_bArray[25] ? global::JSZW1000A.Properties.Resources.LED_yellow : null;
            lbLED_green1.Image = Hmi_bArray[26] ? global::JSZW1000A.Properties.Resources.LED_green : null;
            lbLED_green2.Image = Hmi_bArray[27] ? global::JSZW1000A.Properties.Resources.LED_green : null;
            lbLED_green3.Image = Hmi_bArray[28] ? global::JSZW1000A.Properties.Resources.LED_green : null;
            lbLED_green4.Image = Hmi_bArray[29] ? global::JSZW1000A.Properties.Resources.LED_green : null;

            lbServoRdy.BackColor = Hmi_bArray[30] ? Color.Green : Color.Red;
            lbCoupling.BackColor = Hmi_bArray[31] ? Color.Green : Color.SkyBlue;

            lbTSlide.BackColor = Hmi_bArray[32] ? Color.Green : Color.Gray;
            lbTFold.BackColor = Hmi_bArray[33] ? Color.Green : Color.Gray;
            lbBSlide.BackColor = Hmi_bArray[34] ? Color.Green : Color.Gray;
            lbBFold.BackColor = Hmi_bArray[35] ? Color.Green : Color.Gray;
            label6.BackColor = Hmi_bArray[36] ? Color.Green : Color.Gray;
            label19.BackColor = Hmi_bArray[37] ? Color.Green : Color.Gray;
            label12.BackColor = Hmi_bArray[38] ? Color.Green : Color.Gray;
            label20.ForeColor = Hmi_bArray[39] ? Color.Green : Color.Gray;

            lb钳口移动F.BackColor = Hmi_bArray[100] ? Color.Green : Color.Gray;
            lb钳口移动M.BackColor = Hmi_bArray[101] ? Color.Green : Color.Gray;
            lb钳口移动R.BackColor = Hmi_bArray[102] ? Color.Green : Color.Gray;
            lb纵切0S.BackColor = Hmi_bArray[104] ? Color.Green : Color.Gray;
            lb纵切0L.BackColor = Hmi_bArray[105] ? Color.Green : Color.Gray;
            lb纵切1S.BackColor = Hmi_bArray[106] ? Color.Green : Color.Gray;
            lb纵切1L.BackColor = Hmi_bArray[107] ? Color.Green : Color.Gray;

            sw锥度设定.BackgroundImage = MainFrm.Hmi_bArray[15] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            lb锥度单位.Visible = txb锥度长度.Visible = MainFrm.Hmi_bArray[15];
            txbPumpASpd.Text = Hmi_rArray[34].ToString("f1");
            txbPumpBSpd.Text = Hmi_rArray[35].ToString("f1");
            txbOilTemp.Text = Hmi_rArray[33].ToString("f1");
            if (MainFrm.Hmi_rArray[33] < 30.0) txbOilTemp.ForeColor = Color.White;
            else if (MainFrm.Hmi_rArray[33] >= 30.0 && MainFrm.Hmi_rArray[33] <= 55.0) txbOilTemp.ForeColor = Color.Yellow;
            else txbOilTemp.ForeColor = Color.Red;
            lbOilLevel.Text = Hmi_bArray[103]
                ? Strings.Get("MainFrm.OilLevel.Low")
                : Strings.Get("MainFrm.OilLevel.Normal");


            lbOilLevel.ForeColor = Hmi_bArray[103] ? Color.Red : Color.White;

            if (tLoadData < 4 && !AdsConn)
                tLoadData++;
            else if (tLoadData < 999)
            {
                tLoadData = 999;
                gbl_BootOK = true;
            }



            switch (Delay0)
            {
                case 0:
                    if (MainFrm.AdsConn)
                        Delay0++;
                    break;
                case 1:
                case 2:
                    Delay0++;
                    break;
                case 3:
                    //cbx材料选择.SelectedIndex = Convert.ToInt32(MainFrm.Hmi_rArray[56]);
                    break;
            }

            lbDateTime.Text = DateTime.Now.ToString();
            sw单工双工.BackgroundImage = MainFrm.Hmi_bArray[0] ? global::JSZW1000A.Properties.Resources.btm_单工双工1 : global::JSZW1000A.Properties.Resources.btm_单工双工0;
            sw轻型重型.BackgroundImage = MainFrm.Hmi_bArray[1] ? global::JSZW1000A.Properties.Resources.btm_轻重型1 : global::JSZW1000A.Properties.Resources.btm_轻重型0;

            pnl单工.BackgroundImage = MainFrm.Hmi_bArray[0] ? global::JSZW1000A.Properties.Resources.Simplex3_zh_CHS : global::JSZW1000A.Properties.Resources.Simplex1_zh_CHS;
            pnl双工.BackgroundImage = MainFrm.Hmi_bArray[0] ? global::JSZW1000A.Properties.Resources.Duplex1_zh_CHS : global::JSZW1000A.Properties.Resources.Duplex3_zh_CHS;
            pnl轻型.BackgroundImage = MainFrm.Lang == 0 ?
                (MainFrm.Hmi_bArray[1] ? global::JSZW1000A.Properties.Resources.Light2_zh_CHS : global::JSZW1000A.Properties.Resources.Light1_zh_CHS) :
                (MainFrm.Hmi_bArray[1] ? global::JSZW1000A.Properties.Resources.Light2 : global::JSZW1000A.Properties.Resources.Light1);

            pnl重型.BackgroundImage = MainFrm.Lang == 0 ?
                (MainFrm.Hmi_bArray[1] ? global::JSZW1000A.Properties.Resources.Heavy1_zh_CHS : global::JSZW1000A.Properties.Resources.Heavy2_zh_CHS) :
                (MainFrm.Hmi_bArray[1] ? global::JSZW1000A.Properties.Resources.Heavy1 : global::JSZW1000A.Properties.Resources.Heavy2);



            lb夹钳高度.Text = FormatDisplayLength(Hmi_rArray[20]);
            lb成型角度.Text = Hmi_rArray[28].ToString("f1");
            lb翻板角度.Text = Hmi_rArray[30].ToString("f1");

            if (MainFrm.Hmi_iArray[20] == 3)
                pnl模式显示.BackgroundImage = global::JSZW1000A.Properties.Resources.Manual;
            else if (MainFrm.Hmi_iArray[20] == 4)
                pnl模式显示.BackgroundImage = global::JSZW1000A.Properties.Resources.Bike;
            else if (MainFrm.Hmi_iArray[20] == 5)
                pnl模式显示.BackgroundImage = global::JSZW1000A.Properties.Resources.Slit6;
            else
                pnl模式显示.BackgroundImage = global::JSZW1000A.Properties.Resources.null0;

            pnlOperator1.Visible = MainFrm.Hmi_iArray[21] >= 1;
            pnlOperator2.Visible = MainFrm.Hmi_iArray[21] >= 2;
            //pnlOperator3.Visible = MainFrm.Hmi_iArray[21] >= 3;
            //pnlOperator4.Visible = MainFrm.Hmi_iArray[21] >= 4;

            //------------------------------------  richTextBox 提示消息  -------------------------------------------

            for (int i = 0; i < 14; i++)
            {
                short p = Convert.ToInt16(Math.Pow(2, i));
                if ((Hmi_iArray[26] & p) != (lastErrCode26 & p) && ((Hmi_iArray[26] & p) == p))     //不等,且为1
                    msgAdd(LocalizationText.ErrorMessage(i));
            }
            lastErrCode26 = Hmi_iArray[26];

            for (int i = 0; i < 14; i++)
            {
                short p = Convert.ToInt16(Math.Pow(2, i));
                if ((Hmi_iArray[24] & p) != (lastWarnCode24 & p) && ((Hmi_iArray[24] & p) == p))
                    msgAdd(LocalizationText.WarningMessage(i));
            }
            lastWarnCode24 = Hmi_iArray[24];

            if (Hmi_iArray[22] != lastTipCode22 && Hmi_iArray[22] > 0)
                msgAdd(Strings.Get("MainFrm.TipPrefix") + LocalizationText.TipMessage(0, Hmi_iArray[22]));
            lastTipCode22 = Hmi_iArray[22];



            //-----------------------------Tip Windows Message--------------------------------------------------
            if ((Hmi_iArray[22]) != (lastTipWinCode22) && Hmi_iArray[22] > 0)
            {
                lastTipWinCode22 = Hmi_iArray[22];
                FrmTips dlgTips = new FrmTips(this, LocalizationText.TipMessage(0, Hmi_iArray[22]));
                dlgTips.StartPosition = FormStartPosition.Manual;
                dlgTips.Location = new Point(500, 200);
                bTipFlag = true;
                dlgTips.ShowDialog();
            }
            if (Hmi_iArray[22] == 0)
            {
                lastTipWinCode22 = Hmi_iArray[22];
                bTipFlag = false;
            }



            //if (false && !bTipFlag)
            //{
            //    FrmTips dlgTips = new FrmTips(this,new string s);
            //    dlgTips.StartPosition = FormStartPosition.Manual;
            //    dlgTips.Location = new Point(500, 200);
            //    bTipFlag = true;
            //    dlgTips.ShowDialog();

            //}



        }
        short lastErrCode26 = 0, lastErrCode27 = 0;
        short lastWarnCode24 = 0, lastWarnCode25 = 0;

        private void lb激光挡指_Click(object sender, EventArgs e)
        {
            Hmi_bArray[12] = !Hmi_bArray[12];
            AdsWritePlc1Bit(12, Hmi_bArray[12]);
        }

        short lastTipCode22 = 0, lastTipCode23 = 0;
        short lastTipWinCode22 = 0;
        public bool bTipFlag = false;

        void msgAdd(string s)      //i:  0-msg  1:warn   2:error
        {
            string sDT = "[" + DateTime.Now.ToString("T", DateTimeFormatInfo.InvariantInfo) + "] ";
            if (String.IsNullOrEmpty(richMsgInfo.Text))
                richMsgInfo.AppendText(sDT + s);
            else
                richMsgInfo.Text = richMsgInfo.Text.Insert(0, sDT + s + "\r\n");
        }

        public void ShowTips(int i, int j)
        {
            FrmTips dlgTips = new FrmTips(this, LocalizationText.TipMessage(i, j));
            dlgTips.StartPosition = FormStartPosition.Manual;
            dlgTips.Location = new Point(500, 200);
            dlgTips.ShowDialog();
        }

        public void gbl装载材料MouseDown()
        {
            Hmi_bArray[67] = true;
            AdsWritePlc1Bit(67, Hmi_bArray[67]);
        }

        public void gbl装载材料MouseUp()
        {
            Hmi_bArray[67] = false;
            AdsWritePlc1Bit(67, Hmi_bArray[67]);
        }

        public void gbl开始自动Click(bool IsSlitter, bool IsCreate)
        {
            for (int i = 0; i < MainFrm.Hmi_iSemiAuto.Length; i++)
            {
                MainFrm.Hmi_iSemiAuto[i] = 0;
            }
            if (IsCreate)
            {
                create生产序列();           //  --->MainFrm.CurtOrder.lstSemiAuto
            }

            Create生产数据(IsSlitter);      //  --->MainFrm.Hmi_iSemiAuto
            AdsWritePlc_SemiAuto();
        }
        public void gbl开始自动MouseDown()
        {
            MainFrm.Hmi_bArray[71] = false;
            AdsWritePlc1Bit(71, MainFrm.Hmi_bArray[71]);

            if (MainFrm.Hmi_iArray[0] == 6)
                MainFrm.Hmi_iArray[0] = 3;
            else
                MainFrm.Hmi_iArray[0] = 6;
            AdsWritePlc1Int(0, MainFrm.Hmi_iArray[0]);
        }
        public void gbl开始自动MouseUp()
        {
            //MainFrm.Hmi_bArray[70] = true;
            //AdsWritePlc1Bit(70, MainFrm.Hmi_bArray[70]);
        }

        public void gblSpringSt(float ftTop, float ftBtm)
        {
            MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 10] = ftTop;
            wrtConfigFile("[ManualOldSelect]", 10);
            MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 11] = ftBtm;
            wrtConfigFile("[ManualOldSelect]", 11);
            MainFrm.SpringTop = ftTop;
            MainFrm.SpringBtm = ftBtm;
        }


        private void Create生产数据(bool IsSlitter)
        {
            PackSemiAutoStepsToPlc(CurtOrder.lstSemiAuto, MainFrm.Hmi_iSemiAuto);
        }

        private void btnFeed_Click(object sender, EventArgs e)
        {
            FrmFeed dlgFeed = new FrmFeed(this);
            dlgFeed.StartPosition = FormStartPosition.Manual;
            dlgFeed.Location = new Point(270, 200);
            dlgFeed.ShowDialog();



        }

        private void btn重置视图_Click(object sender, EventArgs e)
        {

        }

    }
}
