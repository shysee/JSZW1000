using System.Drawing.Drawing2D;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace JSZW1000A.SubWindows
{

    public partial class SubOPAutoView : UserControl
    {
        MainFrm mf;
        bool isProc;
        private bool currentPreviewColorDown;
        private bool showingFlipCompletionState;
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
                lb颜色面.Font = lb正逆序.Font = new System.Drawing.Font("微软雅黑", 10.5F);

            }
            else
            {
                lb继续.Font = lb步骤.Font = lb分条开.Font = lb分条关.Font = label21.Font = label22.Font = label7.Font =
                label8.Font = label27.Font = label20.Font = label14.Font = new System.Drawing.Font("Calibri", 11.25F);

                label26.Font = lblFoldListTitle.Font = new System.Drawing.Font("Calibri", 15.75F);
                lblPreviewStartStep.Font = lblPreviewSpeed.Font = new System.Drawing.Font("Calibri", 12F);
                btn装载材料.Font = btn重置计数1.Font = btnSetZero.Font = btnPreViewSt.Font = btnNextPlan.Font = new System.Drawing.Font("Calibri", 11.25F);
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
        private void PreViewSt()
        {
            int currentIndex = GetCurrentPreviewStepIndex();
            if (!showingFlipCompletionState && ShouldFlipAfterCurrentPreviewStep(currentIndex))
            {
                Flip_DataProc();
                return;
            }

            showingFlipCompletionState = false;

            iDrawStep++;

            if (iDrawStep > (MainFrm.CurtOrder.lstSemiAuto.Count) * 2)
            {
                tmr预览.Enabled = false;
                return;
            }



            refshPoint();
            int idd = (int)((iDrawStep - 1) / 2);
            if (idd < 0) idd = 0;
            redrawPreView(MainFrm.CurtOrder.lstSemiAuto[idd].is色下);

        }

        private int GetCurrentPreviewStepIndex()
        {
            if (MainFrm.CurtOrder.lstSemiAuto.Count <= 0)
                return -1;

            if (iDrawStep <= 0)
                return 0;

            int half = (int)((iDrawStep - 1) / 2);
            return Math.Clamp(half, 0, MainFrm.CurtOrder.lstSemiAuto.Count - 1);
        }

        private bool ShouldFlipAfterCurrentPreviewStep(int currentStepIndex)
        {
            if (showingFlipCompletionState)
                return false;
            if (iDrawStep <= 0 || iDrawStep % 2 != 0)
                return false;
            if (currentStepIndex < 0 || currentStepIndex >= MainFrm.CurtOrder.lstSemiAuto.Count - 1)
                return false;

            return MainFrm.CurtOrder.lstSemiAuto[currentStepIndex].内外选择
                != MainFrm.CurtOrder.lstSemiAuto[currentStepIndex + 1].内外选择;
        }

        private int GetDisplayedPreviewStepIndex(int currentStepIndex)
        {
            if (currentStepIndex < 0)
                return -1;

            if (showingFlipCompletionState
                && currentStepIndex < MainFrm.CurtOrder.lstSemiAuto.Count - 1
                && MainFrm.CurtOrder.lstSemiAuto[currentStepIndex].内外选择 != MainFrm.CurtOrder.lstSemiAuto[currentStepIndex + 1].内外选择)
            {
                int nextIndex = currentStepIndex + 1;
                while (nextIndex < MainFrm.CurtOrder.lstSemiAuto.Count
                    && MainFrm.CurtOrder.lstSemiAuto[nextIndex].行动类型 == MainFrm.SemiAutoActionFlip)
                {
                    nextIndex++;
                }

                if (nextIndex < MainFrm.CurtOrder.lstSemiAuto.Count)
                    return nextIndex;
            }

            return currentStepIndex;
        }

        private List<Point> BuildPreviewSegmentsForDrawStep(int drawStep)
        {
            List<Point> previewSegments = new();
            currentPreviewPolyline.Clear();
            currentPreviewAppliedStepCount = 0;
            if (MainFrm.CurtOrder.lstSemiAuto.Count <= 0)
                return previewSegments;

            int currentIndex = GetCurrentPreviewStepIndex();
            if (currentIndex < 0)
                currentIndex = 0;

            bool afterCurrentFold = drawStep > 0 && drawStep % 2 == 0;
            int displayIndex = GetDisplayedPreviewStepIndex(currentIndex);
            if (displayIndex < 0)
                return previewSegments;

            int appliedStepCount = showingFlipCompletionState
                ? displayIndex
                : currentIndex + (afterCurrentFold ? 1 : 0);
            currentPreviewAppliedStepCount = appliedStepCount;
            // 预览需要从“未执行的初始态”逐步推进，而不是从最终板型 pxList 直接开画。
            bool applyFlipAfterLastIncludedStep = showingFlipCompletionState || !afterCurrentFold;
            List<PointF> stagedProfile = MainFrm.BuildSemiAutoPreviewStageProfile(
                MainFrm.CurtOrder,
                appliedStepCount,
                applyFlipAfterLastIncludedStep);
            if (stagedProfile.Count <= 1)
                return previewSegments;

            var displayStep = MainFrm.CurtOrder.lstSemiAuto[displayIndex];
            int anchorIndex = Math.Clamp(displayStep.坐标序号, 0, stagedProfile.Count - 1);
            // 画面左右完全由当前步的 A/B 设置决定：
            // 内外选择 0 => A-B，B 在左、A 在右成型；
            // 内外选择 1 => B-A，A 在左、B 在右成型。
            bool feedSideUsesLowerIndex = displayStep.内外选择 == 1;
            bool feedSideShouldDisplayLeft = true;
            int neighborIndex = feedSideUsesLowerIndex
                ? Math.Max(anchorIndex - 1, 0)
                : Math.Min(anchorIndex + 1, stagedProfile.Count - 1);
            if (neighborIndex == anchorIndex)
            {
                neighborIndex = feedSideUsesLowerIndex
                    ? Math.Min(anchorIndex + 1, stagedProfile.Count - 1)
                    : Math.Max(anchorIndex - 1, 0);
            }
            if (neighborIndex == anchorIndex)
                return previewSegments;

            PointF anchor = stagedProfile[anchorIndex];
            PointF neighbor = stagedProfile[neighborIndex];
            double angle = Math.Atan2(neighbor.Y - anchor.Y, neighbor.X - anchor.X);
            double targetHeading = feedSideShouldDisplayLeft ? Math.PI : 0.0;
            double rotateToTarget = targetHeading - angle;

            List<Point> transformed = new();
            foreach (PointF point in stagedProfile)
            {
                double dx = point.X - anchor.X;
                double dy = point.Y - anchor.Y;
                double rx = dx * Math.Cos(rotateToTarget) - dy * Math.Sin(rotateToTarget);
                double ry = dx * Math.Sin(rotateToTarget) + dy * Math.Cos(rotateToTarget);
                transformed.Add(new Point(
                    (int)Math.Round(cx + rx, MidpointRounding.AwayFromZero),
                    (int)Math.Round(cy - ry, MidpointRounding.AwayFromZero)));
            }

            foreach (Point point in transformed)
                currentPreviewPolyline.Add(point);

            for (int i = 1; i < transformed.Count; i++)
            {
                previewSegments.Add(transformed[i - 1]);
                previewSegments.Add(transformed[i]);
            }

            return previewSegments;
        }

        private bool TryGetAppliedSquashColorDown(int targetIndex, out bool colorDown)
        {
            colorDown = currentPreviewColorDown;
            int appliedStepCount = Math.Clamp(currentPreviewAppliedStepCount, 0, MainFrm.CurtOrder.lstSemiAuto.Count);
            bool found = false;
            for (int i = 0; i < appliedStepCount; i++)
            {
                var step = MainFrm.CurtOrder.lstSemiAuto[i];
                if (step.长角序号 != targetIndex)
                    continue;

                if (MainFrm.IsLegacySemiAutoPlaceholder(step) || MainFrm.IsSemiAutoSquashAction(step.行动类型))
                {
                    colorDown = step.is色下;
                    found = true;
                }
            }

            return found;
        }

        private void DrawPreviewSquash(Graphics graphic, Pen outlinePen)
        {
            if (currentPreviewPolyline.Count < 2)
                return;

            if (MainFrm.CurtOrder.lengAngle[0].Angle > 0
                && MainFrm.CurtOrder.lengAngle[0].Length > 0
                && TryGetAppliedSquashColorDown(0, out bool headColorDown))
            {
                DrawSinglePreviewSquash(
                    graphic,
                    outlinePen,
                    currentPreviewPolyline[0],
                    currentPreviewPolyline[1],
                    MainFrm.CurtOrder.lengAngle[0].Length,
                    (int)MainFrm.CurtOrder.lengAngle[0].Angle,
                    true,
                    headColorDown);
            }

            if (MainFrm.CurtOrder.lengAngle[99].Angle > 0
                && MainFrm.CurtOrder.lengAngle[99].Length > 0
                && TryGetAppliedSquashColorDown(99, out bool tailColorDown))
            {
                int last = currentPreviewPolyline.Count - 1;
                DrawSinglePreviewSquash(
                    graphic,
                    outlinePen,
                    currentPreviewPolyline[last],
                    currentPreviewPolyline[last - 1],
                    MainFrm.CurtOrder.lengAngle[99].Length,
                    (int)MainFrm.CurtOrder.lengAngle[99].Angle,
                    false,
                    tailColorDown);
            }
        }

        private static void DrawSinglePreviewSquash(Graphics graphic, Pen outlinePen, PointF pStart, PointF pRef, double len, int type, bool isHead, bool currentColorDown)
        {
            double dist = Math.Sqrt(Math.Pow(pStart.X - pRef.X, 2) + Math.Pow(pStart.Y - pRef.Y, 2));
            if (dist < 0.001)
                return;

            double dx = pStart.X - pRef.X;
            double dy = pStart.Y - pRef.Y;
            double unitX = dx / dist;
            double unitY = dy / dist;

            double widthOffset = 4.0;
            double lengthOffset = len;
            double perpX = -unitY * widthOffset;
            double perpY = unitX * widthOffset;

            bool isTypeUp = (type == 1 || type == 3);
            bool needInvert = isHead ? !isTypeUp : isTypeUp;
            if (!currentColorDown)
                needInvert = !needInvert;
            if (needInvert)
            {
                perpX = -perpX;
                perpY = -perpY;
            }

            float p0x = pStart.X + (float)perpX;
            float p0y = pStart.Y + (float)perpY;
            float p1x = p0x - (float)(unitX * lengthOffset);
            float p1y = p0y - (float)(unitY * lengthOffset);

            graphic.DrawLine(outlinePen, p0x, p0y, p1x, p1y);
            graphic.DrawLine(outlinePen, p0x, p0y, pStart.X, pStart.Y);
        }

        private void UpdatePreviewStepInfo()
        {
            int currentIndex = GetCurrentPreviewStepIndex();
            if (currentIndex < 0)
            {
                lb下一操作提示.Text = string.Empty;
                lblPlanSummary.Text = mf.GetCurrentFormalPlanSummaryText();
                RefreshPreviewInfoText(-1);
                return;
            }

            int displayIndex = GetDisplayedPreviewStepIndex(currentIndex);
            if (displayIndex < 0)
                return;

            var currentStep = MainFrm.CurtOrder.lstSemiAuto[displayIndex];
            lblPlanSummary.Text = mf.GetCurrentFormalPlanSummaryText();

            if (showingFlipCompletionState)
            {
                lb下一操作提示.Text = Strings.Get("AutoView.NextAction.FlipComplete");
            }
            else if (currentIndex < MainFrm.CurtOrder.lstSemiAuto.Count - 1
                && MainFrm.CurtOrder.lstSemiAuto[currentIndex].内外选择 != MainFrm.CurtOrder.lstSemiAuto[currentIndex + 1].内外选择)
            {
                lb下一操作提示.Text = Strings.Get("AutoView.NextAction.Flip");
            }
            else if (currentStep.行动类型 == 1 || currentStep.行动类型 == 2)
            {
                lb下一操作提示.Text = Strings.Get("AutoView.NextAction.Squash");
            }
            else if (currentStep.折弯方向 == 0)
            {
                lb下一操作提示.Text = Strings.Get("AutoView.NextAction.FoldUp");
            }
            else
            {
                lb下一操作提示.Text = Strings.Get("AutoView.NextAction.FoldDown");
            }

            RefreshPreviewInfoText(displayIndex);
        }

        private void RefreshPreviewInfoText(int displayIndex)
        {
            string nextAction = lb下一操作提示.Text?.Trim() ?? string.Empty;
            string foldList = BuildFoldListText(displayIndex);
            string runtimeMessages = mf?.GetRuntimeMessagesSnapshot()?.Trim() ?? string.Empty;
            bool showRuntimeMessages = !string.IsNullOrWhiteSpace(runtimeMessages);
            if (!string.Equals(rtbRuntimeMessages.Text, runtimeMessages, StringComparison.Ordinal))
                rtbRuntimeMessages.Text = runtimeMessages;
            if (lblRuntimeMessagesTitle.Visible != showRuntimeMessages)
                lblRuntimeMessagesTitle.Visible = showRuntimeMessages;
            if (rtbRuntimeMessages.Visible != showRuntimeMessages)
                rtbRuntimeMessages.Visible = showRuntimeMessages;

            var builder = new System.Text.StringBuilder();
            if (!string.IsNullOrWhiteSpace(nextAction))
                builder.AppendLine(nextAction);

            if (!string.IsNullOrWhiteSpace(foldList))
            {
                if (builder.Length > 0)
                    builder.AppendLine();
                builder.Append(foldList.TrimEnd());
            }

            string newText = builder.ToString();
            if (!string.Equals(rtbPreviewPlan.Text, newText, StringComparison.Ordinal))
                rtbPreviewPlan.Text = newText;
        }

        private string BuildFoldListText(int currentDisplayIndex)
        {
            if (MainFrm.CurtOrder.lstSemiAuto.Count <= 0)
                return string.Empty;

            var builder = new System.Text.StringBuilder();
            for (int i = 0; i < MainFrm.CurtOrder.lstSemiAuto.Count; i++)
            {
                MainFrm.SemiAutoType step = MainFrm.CurtOrder.lstSemiAuto[i];
                string prefix = i == currentDisplayIndex ? "> " : string.Empty;
                builder.AppendLine(prefix + BuildStepTitle(step, i + 1));

                foreach (string detail in BuildStepDetails(step))
                    builder.AppendLine(detail);
            }

            return builder.ToString();
        }

        private string BuildStepTitle(MainFrm.SemiAutoType step, int displayOrder)
        {
            string actionText = step.行动类型 switch
            {
                MainFrm.SemiAutoActionSlit => MainFrm.Lang == 0 ? "分条" : "Slit",
                MainFrm.SemiAutoActionFlip => MainFrm.Lang == 0 ? "翻面" : "Flip",
                MainFrm.SemiAutoActionSquash => MainFrm.Lang == 0 ? "压死边" : "Squash",
                MainFrm.SemiAutoActionOpenSquash => MainFrm.Lang == 0 ? "压开边" : "Open Squash",
                _ => MainFrm.Lang == 0 ? $"折弯 {step.折弯序号}" : $"Fold {step.折弯序号}"
            };

            return MainFrm.Lang == 0
                ? $"{displayOrder}. {actionText}"
                : $"Step {displayOrder}. {actionText}";
        }

        private IEnumerable<string> BuildStepDetails(MainFrm.SemiAutoType step)
        {
            yield return (MainFrm.Lang == 0 ? "后挡位置 " : "Backgauge ")
                + MainFrm.FormatDisplayLength(step.后挡位置)
                + (MainFrm.Lang == 0 ? $"；{LocalizationText.GripType(step.抓取类型)}" : $"; {LocalizationText.GripType(step.抓取类型)}");

            if (step.行动类型 == MainFrm.SemiAutoActionFlip)
            {
                yield return MainFrm.Lang == 0 ? "材料翻面" : "Material side flip";
                yield break;
            }

            if (step.行动类型 == MainFrm.SemiAutoActionSlit)
            {
                yield return MainFrm.Lang == 0 ? "执行分条动作" : "Execute slitting";
                yield break;
            }

            string foldDirection = LocalizationText.FoldDirectionShort(step.折弯方向);
            string springback = step.回弹值 == 0
                ? "+0.0"
                : $"{step.回弹值:+0.0;-0.0;0.0}";
            yield return (MainFrm.Lang == 0 ? "折弯角度 " : "Fold Angle ")
                + $"{step.折弯角度:0.0}° ({springback}°) {foldDirection}";

            string clampHeight = MainFrm.Lang == 0
                ? LocalizationText.ReleaseHeightShort(step.松开高度)
                : LocalizationText.ReleaseHeight(step.松开高度);
            string regrip = step.重新抓取 != 0
                ? (MainFrm.Lang == 0 ? "；重新抓取" : "; Regrip")
                : string.Empty;
            yield return (MainFrm.Lang == 0 ? "压钳高度 " : "Clamp Height ")
                + clampHeight
                + regrip;
        }

        void reGiveSquish(double d)
        {
            int k = 0;
            while (k < pxDraw.Count)
            {
                Point tmp = new Point();
                tmp = pxDraw[k];
                tmp.X = tmp.X + (int)d;
                pxDraw[k] = tmp;
                k++;
            }

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
                redrawPreView(MainFrm.CurtOrder.lstSemiAuto[0].is色下);
            txtPreviewDrawStep.Text = iDrawStep.ToString();
        }

        void refshPoint()
        {
            pxDraw = BuildPreviewSegmentsForDrawStep(iDrawStep);
        }
        private Point PointRotate(Point center, Point p1, double angle)
        {
            Point tmp = new Point();
            double angleHude = angle * Math.PI / 180;/*角度变成弧度*/
            double x1 = (p1.X - center.X) * Math.Cos(angleHude) + (p1.Y - center.Y) * Math.Sin(angleHude) + center.X;
            double y1 = -(p1.X - center.X) * Math.Sin(angleHude) + (p1.Y - center.Y) * Math.Cos(angleHude) + center.Y;
            tmp.X = (int)Math.Round(x1, 0, MidpointRounding.AwayFromZero);
            tmp.Y = (int)Math.Round(y1, 0, MidpointRounding.AwayFromZero);
            return tmp;
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

            redrawPreView(MainFrm.CurtOrder.lstSemiAuto[displayIndex].is色下);
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

        private void richMsgInfo_TextChanged(object sender, EventArgs e)
        {

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

        private void richMsgInfo_TextChanged_1(object sender, EventArgs e)
        {

        }
    }
}
