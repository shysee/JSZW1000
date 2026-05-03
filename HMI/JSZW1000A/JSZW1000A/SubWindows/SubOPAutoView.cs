using System.Drawing.Drawing2D;
namespace JSZW1000A.SubWindows
{

    public partial class SubOPAutoView : UserControl
    {
        MainFrm mf;
        bool isProc;
        private bool currentPreviewColorDown;
        private bool showingFlipCompletionState;
        private bool showStructureExplanationMode;
        private readonly List<PointF> currentPreviewPolyline = new();
        private int currentPreviewAppliedStepCount;
        public SubOPAutoView(MainFrm fm1, bool proc)
        {
            InitializeComponent();
            pictureBox1.Paint += pictureBox1_Paint;
            setLang();
            this.mf = fm1;
            this.isProc = proc;
            sw正逆序.Click += sw正逆序_Click;
            sw颜色面.Click += sw颜色面_Click;
        }

        private void setLang()
        {
            LocalizationManager.ApplyResources(this);

            if (MainFrm.Lang == 0)
            {
                lb继续.Font = lb步骤.Font = lb分条开.Font = lb分条关.Font = label21.Font = label22.Font = label7.Font =
                    label8.Font = label27.Font = label20.Font = label14.Font = new System.Drawing.Font("宋体", 11.25F);

                label26.Font = lblFoldListTitle.Font = new System.Drawing.Font("Microsoft YaHei UI", 15.75F);
                lblPreviewStartStep.Font = lblPreviewSpeed.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F);

                btn装载材料.Font = btn重置计数1.Font = btnSetZero.Font = btnPreViewSt.Font = btnNextPlan.Font = new System.Drawing.Font("宋体", 11.25F);
                btnPreviewTextMode.Font = new System.Drawing.Font("宋体", 9.5F);
                lb颜色面.Font = lb正逆序.Font = new System.Drawing.Font("微软雅黑", 10.5F);

            }
            else
            {
                lb继续.Font = lb步骤.Font = lb分条开.Font = lb分条关.Font = label21.Font = label22.Font = label7.Font =
                label8.Font = label27.Font = label20.Font = label14.Font = new System.Drawing.Font("Calibri", 11.25F);

                label26.Font = lblFoldListTitle.Font = new System.Drawing.Font("Calibri", 15.75F);
                lblPreviewStartStep.Font = lblPreviewSpeed.Font = new System.Drawing.Font("Calibri", 12F);
                btn装载材料.Font = btn重置计数1.Font = btnSetZero.Font = btnPreViewSt.Font = btnNextPlan.Font = new System.Drawing.Font("Calibri", 11.25F);
                btnPreviewTextMode.Font = new System.Drawing.Font("Calibri", 9.5F);
                lb颜色面.Font = lb正逆序.Font = new System.Drawing.Font("Calibri", 10.5F);
            }
            lb继续.Text = Strings.Get("AutoView.Toggle.Continue");
            lb步骤.Text = Strings.Get("AutoView.Toggle.Step");
            lb分条开.Text = Strings.Get("AutoView.Toggle.Slit");
            lb分条关.Text = Strings.Get("AutoView.Toggle.Off");
            label21.Text = Strings.Get("AutoView.Label.TopSpringback");
            label22.Text = Strings.Get("AutoView.Label.BottomSpringback");
            label7.Text = Strings.Get("AutoView.Label.CalculatedWidth");
            label27.Text = Strings.Get("AutoView.Label.FoldSequence");
            label20.Text = Strings.Get("AutoView.Label.FeedColorSide");
            label14.Text = Strings.Get("AutoView.Label.Count");
            label26.Text = Strings.Get("AutoView.Label.OperationTips");
            lblFoldListTitle.Text = Strings.Get("AutoView.Label.FoldList");
            lblPreviewStartStep.Text = Strings.Get("AutoView.Label.StartStep");
            lblPreviewSpeed.Text = MainFrm.Lang == 0 ? "速 度\r\n调 节" : "Speed\r\nControl";
            btnNextPlan.Text = MainFrm.Lang == 0 ? "下一个方案" : "Next Plan";
            btn装载材料.Text = Strings.Get("AutoView.Action.LoadMaterial");
            btn重置计数1.Text = Strings.Get("AutoView.Action.ResetCount");
            btnSetZero.Text = Strings.Get("AutoView.Action.ResetSteps");
            btnPreViewSt.Text = Strings.Get("AutoView.Action.Run");
            label8.Text = MainFrm.GetLengthUnitLabel();
            lb正逆序.Text = LocalizationText.OrderDirection(MainFrm.CurtOrder.st逆序);
            lb颜色面.Text = LocalizationText.ColorSide(MainFrm.CurtOrder.st色下);
            UpdatePreviewModeVisuals();
        }

        public List<Point> pxDraw = new List<Point>();
        private void SubOPAutoView_Load(object sender, EventArgs e)
        {
            pnl左工具栏1.Parent = this;
            pnl左工具栏1.Visible = false;
            pnl左工具栏1.Location = new Point(2, 101);
            txbSpringTop.Text = string.Format("{0:F2}", MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 10]);
            txbSpringBtm.Text = string.Format("{0:F2}", MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 11]);
            RefreshPreviewState();
        }

        // View 页只保留预览职责，旧的布置/设置模式已迁到 SubOPAutoSet。
        private void RefreshPreviewState()
        {
            // 手工在 set 页调好的步骤表必须优先保留，不能先被几何兜底重建覆盖。
            if (mf != null && mf.HasManualSemiAutoEdits())
            {
                mf.NormalizeGeneratedSemiAutoSequence();
            }
            else if (mf != null && !mf.HasValidSemiAutoGeometryData())
            {
                mf.create生产序列();
            }

            pnl左工具栏1.Visible = true;
            pnl左工具栏1.Enabled = true;
            UpdatePreviewModeVisuals();
            InitDraw(true);
        }

        public void stSetting()
        {
            RefreshPreviewState();
        }

        public void stPreView()
        {
            RefreshPreviewState();
        }

        private void SortSemiLst()
        {
            for (int i = 0; i < MainFrm.CurtOrder.lstSemiAuto.Count - 1; i++)
            {
                int minIndex = i;
                for (int j = i + 1; j < MainFrm.CurtOrder.lstSemiAuto.Count; j++)
                {
                    if (MainFrm.CurtOrder.lstSemiAuto[j].折弯序号 < MainFrm.CurtOrder.lstSemiAuto[minIndex].折弯序号)
                        minIndex = j;
                }
                var temp = MainFrm.CurtOrder.lstSemiAuto[minIndex];
                MainFrm.CurtOrder.lstSemiAuto[minIndex] = MainFrm.CurtOrder.lstSemiAuto[i];
                MainFrm.CurtOrder.lstSemiAuto[i] = temp;
            }
        }
        /*--------------------------------------------------------------------------------------
         ------------------------------以下预览功能代码-----------------------------------------
         ---------------------------------------------------------------------------------------*/
        int cx = 0, cy = 0;
        int iDrawStep = 0;
        private void btnPreViewSt_Click(object sender, EventArgs e)
        {
            PreViewSt();
        }
        private void InitDraw(bool isPreView)       //isPreView:是否需要绘初图
        {
            iDrawStep = 0;
            showingFlipCompletionState = false;
            SortSemiLst();
            cx = pictureBox1.Size.Width / 2; cy = pictureBox1.Size.Height / 2;
            pictureBox1.Controls.Clear();
            pxDraw = BuildPreviewSegmentsForDrawStep(iDrawStep);

            if (!isPreView)
                return;

            if (MainFrm.CurtOrder.lstSemiAuto.Count > 0)
                redrawPreView(currentPreviewColorDown);
            txtPreviewDrawStep.Text = iDrawStep.ToString();
        }

        void refshPoint()
        {
            pxDraw = BuildPreviewSegmentsForDrawStep(iDrawStep);
        }

        Bitmap image1 = new Bitmap(1180, 805);

        private void pictureBox1_Paint(object? sender, PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(image1, 0, 0);
        }

        void redrawPreView(bool is色下0)
        {
            currentPreviewColorDown = is色下0;
            using (Graphics graphic = Graphics.FromImage(image1))
            {
                Image myImage = (Image)global::JSZW1000A.Properties.Resources.预览设备图虚化;

                graphic.DrawImage(myImage, 0, 0, myImage.Width, myImage.Height);

                //绘制虚线
                Pen myPen0 = new Pen(Color.Green, 1);
                myPen0.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash; //虚线
                graphic.DrawLine(myPen0, cx, 0 + 100, cx, pictureBox1.Size.Height - 100);

                Pen myPen1 = new Pen(Color.White, 4);
                Pen myPen2 = new Pen(Color.FromArgb(255, 87, 34), 1);

                int k = 1;
                if (pxDraw.Count > 0)
                {
                    while (k < pxDraw.Count)
                    {
                        graphic.DrawLine(myPen1, pxDraw[k - 1].X, pxDraw[k - 1].Y, pxDraw[k].X, pxDraw[k].Y);

                        //画颜色线
                        if (is色下0)
                            graphic.DrawLine(myPen2, pxDraw[k - 1].X, pxDraw[k - 1].Y + 5, pxDraw[k].X, pxDraw[k].Y + 5);
                        else
                            graphic.DrawLine(myPen2, pxDraw[k - 1].X, pxDraw[k - 1].Y - 5, pxDraw[k].X, pxDraw[k].Y - 5);

                        k++; k++;
                    }
                }

                DrawPreviewSquash(graphic, myPen1);
            }
            pictureBox1.Invalidate();
            UpdatePreviewStepInfo();
        }

        private void btnSetZero_Click(object sender, EventArgs e)
        {
            iDrawStep = 0;
            showingFlipCompletionState = false;
            pxDraw.Clear();
            InitDraw(true);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdatePreviewStepInfo();
            txtPreviewDrawStep.Text = iDrawStep.ToString();
            RefreshPreviewInfoText(GetDisplayedPreviewStepIndex(GetCurrentPreviewStepIndex()));
            btnNextPlan.Enabled = mf.CanPreviewNextSemiAutoPlan();


            btn自动预览.BackgroundImage = tmr预览.Enabled ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            lb自动预览.Text = tmr预览.Enabled ? Strings.Get("AutoView.Preview.Auto") : Strings.Get("AutoView.Preview.Jog");
            lb自动预览.ForeColor = tmr预览.Enabled ? Color.FromArgb(96, 176, 255) : Color.White;

            pnlAuto.BackgroundImage = MainFrm.Hmi_iArray[20] == 6
                ? (MainFrm.Lang == 0 ? global::JSZW1000A.Properties.Resources.AutoStart : global::JSZW1000A.Properties.Resources.AutoStart1)
                : (MainFrm.Lang == 0 ? global::JSZW1000A.Properties.Resources.AutoOrig1_zh_CHS : global::JSZW1000A.Properties.Resources.AutoOrig1);
            txb计算总宽.Text = MainFrm.FormatDisplayLength(MainFrm.CurtOrder.Width);

            sw继续步骤.Image = MainFrm.Hmi_bArray[71] ? global::JSZW1000A.Properties.Resources.btm_2档开关1 : global::JSZW1000A.Properties.Resources.btm_2档开关0;
            lb继续.ForeColor = MainFrm.Hmi_bArray[71] ? Color.FromArgb(96, 176, 255) : Color.White;
            lb步骤.ForeColor = !MainFrm.Hmi_bArray[71] ? Color.FromArgb(96, 176, 255) : Color.White;

            sw分条开关.Image = MainFrm.Hmi_bArray[72] ? global::JSZW1000A.Properties.Resources.btm_分条开关1 : global::JSZW1000A.Properties.Resources.btm_分条开关0;
            lb分条开.ForeColor = MainFrm.Hmi_bArray[72] ? Color.FromArgb(96, 176, 255) : Color.White;
            lb分条关.ForeColor = !MainFrm.Hmi_bArray[72] ? Color.FromArgb(96, 176, 255) : Color.White;

            sw正逆序.BackgroundImage = MainFrm.CurtOrder.st逆序 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            lb正逆序.Text = LocalizationText.OrderDirection(MainFrm.CurtOrder.st逆序);
            lb正逆序.ForeColor = MainFrm.CurtOrder.st逆序 ? Color.FromArgb(96, 176, 255) : Color.White;
            sw颜色面.BackgroundImage = MainFrm.CurtOrder.st色下 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            lb颜色面.Text = LocalizationText.ColorSide(MainFrm.CurtOrder.st色下);
            lb颜色面.ForeColor = MainFrm.CurtOrder.st色下 ? Color.FromArgb(96, 176, 255) : Color.White;
        }

        private void tmr预览_Tick(object sender, EventArgs e)
        {
            btnPreViewSt.PerformClick();
        }

        private void btn自动预览_Click(object sender, EventArgs e)
        {
            tmr预览.Enabled = !tmr预览.Enabled;
            btn自动预览.BackgroundImage = tmr预览.Enabled ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            lb自动预览.Text = tmr预览.Enabled ? Strings.Get("AutoView.Preview.Auto") : Strings.Get("AutoView.Preview.Jog");
            lb自动预览.ForeColor = tmr预览.Enabled ? Color.FromArgb(96, 176, 255) : Color.White;
            if (tmr预览.Enabled)
                btnSetZero.PerformClick();
        }

        private void btnNextPlan_Click(object sender, EventArgs e)
        {
            if (!mf.TryPreviewNextSemiAutoPlan())
                return;

            RefreshPreviewState();
        }

        private void btnPreviewTextMode_Click(object? sender, EventArgs e)
        {
            showStructureExplanationMode = !showStructureExplanationMode;
            UpdatePreviewModeVisuals();
            RefreshPreviewInfoText(GetDisplayedPreviewStepIndex(GetCurrentPreviewStepIndex()));
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            tmr预览.Interval = trackBar1.Maximum - trackBar1.Value;
        }



        private void Flip_DataProc()
        {
            showingFlipCompletionState = true;
            pxDraw = BuildPreviewSegmentsForDrawStep(iDrawStep);
            int currentIndex = GetCurrentPreviewStepIndex();
            int displayIndex = GetDisplayedPreviewStepIndex(currentIndex);
            if (displayIndex < 0)
                return;

            redrawPreView(currentPreviewColorDown);
        }

        /*--------------------------------------------------------------------------------------
        ------------------------------以下菜单栏代码-----------------------------------------
        ---------------------------------------------------------------------------------------*/
        private void btn装载材料_MouseUp(object sender, MouseEventArgs e)
        {
            mf.gbl装载材料MouseUp();
        }

        private void btn装载材料_MouseDown(object sender, MouseEventArgs e)
        {
            mf.gbl装载材料MouseDown();
        }

        private void pnlAuto_Click(object sender, EventArgs e)
        {
            mf.gbl开始自动Click(MainFrm.Hmi_bArray[72], false);
        }

        private void pnlAuto_MouseDown(object sender, MouseEventArgs e)
        {
            mf.gbl开始自动MouseDown();
        }

        private void sw分条开关_Click(object sender, EventArgs e)
        {
            string s0 = "";
            if (!MainFrm.Hmi_bArray[72])
                s0 = Strings.Get("Common.ConfirmEnableSlitter");

            DialogAsk dlgTips = new DialogAsk("", s0);
            dlgTips.StartPosition = FormStartPosition.Manual;
            dlgTips.Location = new Point(500, 200);
            if ((!MainFrm.Hmi_bArray[72] && dlgTips.ShowDialog() == DialogResult.OK) || MainFrm.Hmi_bArray[72])
            {
                MainFrm.Hmi_bArray[72] = !MainFrm.Hmi_bArray[72];
                sw分条开关.Image = MainFrm.Hmi_bArray[72] ? global::JSZW1000A.Properties.Resources.btm_分条开关1 : global::JSZW1000A.Properties.Resources.btm_分条开关0;
                mf.AdsWritePlc1Bit(72, MainFrm.Hmi_bArray[72]);
            }
        }

        private void sw继续步骤_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[71] = !MainFrm.Hmi_bArray[71];
            sw继续步骤.Image = MainFrm.Hmi_bArray[71] ? global::JSZW1000A.Properties.Resources.btm_2档开关1 : global::JSZW1000A.Properties.Resources.btm_2档开关0;
            mf.AdsWritePlc1Bit(71, MainFrm.Hmi_bArray[71]);
        }

        private void sw正逆序_Click(object? sender, EventArgs e)
        {
            MainFrm.CurtOrder.st逆序 = !MainFrm.CurtOrder.st逆序;
            if (mf.TryApplyPreviewPreferences())
                RefreshPreviewState();
        }

        private void sw颜色面_Click(object? sender, EventArgs e)
        {
            MainFrm.CurtOrder.st色下 = !MainFrm.CurtOrder.st色下;
            if (mf.TryApplyPreviewPreferences())
                RefreshPreviewState();
            else
            {
                mf.NormalizeGeneratedSemiAutoSequence();
                MainFrm.RebuildSemiAutoDerivedState(ref MainFrm.CurtOrder);
                RefreshPreviewState();
            }
        }

        private void txbSpringTop_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                mf.gblSpringSt(Convert.ToSingle(txbSpringTop.Text), Convert.ToSingle(txbSpringBtm.Text));
            }
        }



        private void pnlAuto_MouseUp(object sender, MouseEventArgs e)
        {
            mf.gbl开始自动MouseUp();
        }

    }
}
