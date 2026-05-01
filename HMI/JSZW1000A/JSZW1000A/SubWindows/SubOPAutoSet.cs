using System.Drawing.Drawing2D;
using System.Reflection;

namespace JSZW1000A.SubWindows
{
    public partial class SubOPAutoSet : UserControl
    {
        private MainFrm? mf;
        private bool initialized;
        private readonly List<PointF> pxListZoom = new();
        private readonly Dictionary<int, PointF> anchors = new();
        private readonly List<MainFrm.SemiAutoType> draftSemiAutoSteps = new();
        private readonly List<MainFrm.SemiAutoType> originalSemiAutoSteps = new();
        private double zoom = 1.0;
        private int selStep = 1;
        private int sel折弯序号 = 0;
        private double db抓取类型子项 = 0;
        private double db松开高度子项 = 0;
        private double db内外选择子项 = 0;
        private double db折弯方向子项 = 0;
        private string originalPlanOrigin = MainFrm.SemiAutoPlanOriginGeneratedSelected;
        private bool originalManualEdited;
        private bool layoutConfirmed;

        public SubOPAutoSet()
        {
            InitializeComponent();
            InitializePage();
        }

        public SubOPAutoSet(MainFrm fm1) : this()
        {
            mf = fm1;
        }

        private void InitializePage()
        {
            if (initialized)
                return;

            initialized = true;
            Load += SubOPAutoSet_Load;
            ParentChanged += SubOPAutoSet_ParentChanged;
            panel1.Paint += panel1_Paint;
            panel1.Resize += panel1_Resize;
            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, panel1, new object[] { true });

            sw正逆序.Click += sw正逆序_Click;
            button3.Click += sw颜色面_Click;
            label27.Click += lb正逆序_Click;
            label26.Click += lb颜色面_Click;
            button2.Click += btnResetLayoutDraft_Click;
            btnConfirmLayout.Click += btnConfirmLayout_Click;

            btnMoveFront.Click += btnMoveFront_Click;
            btnMoveRear.Click += btnMoveRear_Click;
            btn抓取类型.Click += btn抓取类型_Click;
            lb抓取类型_推动.Click += lb抓取类型_推动_Click;
            lb抓取类型_抓取.Click += lb抓取类型_推动_Click;
            lb抓取类型_超程抓取.Click += lb抓取类型_推动_Click;
            btn松开高度.Click += btn松开高度_Click;
            lb松开高度_低.Click += lb松开高度_低_Click;
            lb松开高度_中.Click += lb松开高度_低_Click;
            lb松开高度_高.Click += lb松开高度_低_Click;
            lb松开高度_最大.Click += lb松开高度_低_Click;
            btn内外选择.Click += btn内外选择_Click;
            lb内外选择_A在外.Click += lb内外选择_A在外_Click;
            lb内外选择_B在外.Click += lb内外选择_A在外_Click;
            btn折弯方向.Click += btn折弯方向_Click;
            pnl折弯方向_向下.Click += pnl折弯方向_向下_Click;
            pnl折弯方向_向上.Click += pnl折弯方向_向下_Click;
        }

        private void SubOPAutoSet_Load(object? sender, EventArgs e)
        {
            LocalizationManager.ApplyResources(this);
            panel3.Visible = true;
            panel3.BringToFront();
            panel1.BackColor = Color.FromArgb(33, 40, 48);
            sw正逆序.Enabled = false;
            button3.Enabled = false;
            label27.Enabled = false;
            label26.Enabled = false;
            CaptureLayoutDraft();
            RefreshLayoutSetting();
        }

        private void CaptureLayoutDraft()
        {
            originalSemiAutoSteps.Clear();
            originalSemiAutoSteps.AddRange(CloneSteps(MainFrm.CurtOrder.lstSemiAuto));
            draftSemiAutoSteps.Clear();
            draftSemiAutoSteps.AddRange(CloneSteps(MainFrm.CurtOrder.lstSemiAuto));
            originalPlanOrigin = MainFrm.CurtOrder.SemiAutoPlanOrigin;
            originalManualEdited = mf?.HasManualSemiAutoEdits() ?? false;
            layoutConfirmed = false;
        }

        private static List<MainFrm.SemiAutoType> CloneSteps(IReadOnlyList<MainFrm.SemiAutoType> source)
        {
            return new List<MainFrm.SemiAutoType>(source);
        }

        private List<MainFrm.SemiAutoType> WorkingSteps => draftSemiAutoSteps;

        private void SubOPAutoSet_ParentChanged(object? sender, EventArgs e)
        {
            if (Parent == null && !layoutConfirmed)
                RestoreOriginalLayoutState();
        }

        private void RestoreOriginalLayoutState()
        {
            if (mf == null)
                return;

            mf.RestoreSemiAutoPlanEditingState(originalSemiAutoSteps, originalManualEdited, originalPlanOrigin);
            CaptureLayoutDraft();
            RefreshLayoutSetting();
        }

        private MainFrm.OrderType CreateDraftOrder()
        {
            MainFrm.OrderType order = MainFrm.CurtOrder;
            order.pxList = new List<PointF>(MainFrm.CurtOrder.pxList);
            order.lstSemiAuto = CloneSteps(WorkingSteps);
            order.lengAngle = (MainFrm.LengAngle[])MainFrm.CurtOrder.lengAngle.Clone();
            return order;
        }

        private void RecalculateDraftDerivedState()
        {
            MainFrm.OrderType draftOrder = CreateDraftOrder();
            MainFrm.RebuildSemiAutoDerivedState(ref draftOrder);
            draftSemiAutoSteps.Clear();
            draftSemiAutoSteps.AddRange(CloneSteps(draftOrder.lstSemiAuto));
        }

        private void panel1_Resize(object? sender, EventArgs e)
        {
            if (!IsHandleCreated || mf == null)
                return;
            RefreshLayoutSetting();
        }

        private void RefreshLayoutSetting()
        {
            panel3.Visible = true;
            ApplyLocalizedStaticTexts();
            SyncTopState();
            RebuildZoomedProfile();
            EnsureSelectedStateValid();
            RebuildFoldButtons();
            UpdateSelectedStepPanels();
            panel1.Invalidate();
        }

        private void ApplyLocalizedStaticTexts()
        {
            btn装载材料.Text = Strings.Get("Auto.Action.LoadMaterial");
            btn重置计数.Text = Strings.Get("Auto.Action.ResetCount");
            btnMoveRear.Text = Strings.Get("AutoView.Action.MoveRear");
            btnMoveFront.Text = Strings.Get("AutoView.Action.MoveFront");
            button2.Text = MainFrm.Lang == 0 ? "重置\r\n布置" : "Reset\r\nlayout";
            btnConfirmLayout.Text = MainFrm.Lang == 0 ? "确\r\n认" : "OK";

            label28.Text = Strings.Get("AutoSet.Label.FeedColorSide");
            label29.Text = Strings.Get("AutoSet.Label.FoldSequence");
            label18.Text = Strings.Get("AutoSet.Label.PendingSlitWidth");
            label7.Text = Strings.Get("Auto.Label.CalculatedWidth");
            label22.Text = Strings.Get("Auto.Label.TopSpringback");
            label21.Text = Strings.Get("Auto.Label.BottomSpringback");
            label17.Text = Strings.Get("AutoView.Label.BackGaugePosition");
            label15.Text = Strings.Get("AutoView.Label.SpringbackAngle");
            label9.Text = Strings.Get("AutoView.Label.FoldDirection");
            label11.Text = Strings.Get("AutoView.Label.SelectedOutsideClamp");
            label14.Text = Strings.Get("AutoView.Label.GripType");
            label25.Text = Strings.Get("AutoView.Label.ReleaseHeight");

            lb分条开.Text = Strings.Get("Auto.Toggle.Slit");
            lb分条关.Text = Strings.Get("Auto.Toggle.Off");
            lb内外选择_A在外.Text = Strings.Get("AutoView.ClampOutside.A");
            lb内外选择_B在外.Text = Strings.Get("AutoView.ClampOutside.B");
            lb抓取类型_推动.Text = Strings.Get("AutoView.GripType.Push");
            lb抓取类型_抓取.Text = Strings.Get("AutoView.GripType.Grip");
            lb抓取类型_超程抓取.Text = Strings.Get("AutoView.GripType.Overgrip");
            lb松开高度_低.Text = Strings.Get("AutoView.ReleaseHeight.Low");
            lb松开高度_中.Text = Strings.Get("AutoView.ReleaseHeight.Medium");
            lb松开高度_高.Text = Strings.Get("AutoView.ReleaseHeight.High");
            lb松开高度_最大.Text = Strings.Get("AutoView.ReleaseHeight.Max");

            string unitText = MainFrm.GetLengthUnitLabel();
            label20.Text = unitText;
            label8.Text = unitText;
            label23.Text = "°";
            label24.Text = "°";
        }

        private void SyncTopState()
        {
            if (mf == null)
                return;

            pnlAuto.BackgroundImage = MainFrm.Lang == 0
                ? Properties.Resources.AutoOrig1_zh_CHS
                : Properties.Resources.AutoOrig1;
            txbSpringTop.Text = MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 10].ToString("F2");
            txbSpringBtm.Text = MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 11].ToString("F2");
            txb计算总宽.Text = MainFrm.FormatDisplayLength(MainFrm.CurtOrder.Width);
            double pendingSlitWidth = MainFrm.CurtOrder.边做边分切整板宽 > 0
                ? MainFrm.CurtOrder.边做边分切整板宽
                : MainFrm.CurtOrder.Width;
            txb待分条板宽.Text = MainFrm.FormatDisplayLength(pendingSlitWidth);

            label27.Text = LocalizationText.OrderDirection(MainFrm.CurtOrder.st逆序);
            label26.Text = LocalizationText.ColorSide(MainFrm.CurtOrder.st色下);

            sw正逆序.BackgroundImage = MainFrm.CurtOrder.st逆序
                ? Properties.Resources.sw_左右小开关1
                : Properties.Resources.sw_左右小开关0;
            button3.BackgroundImage = MainFrm.CurtOrder.st色下
                ? Properties.Resources.sw_左右小开关1
                : Properties.Resources.sw_左右小开关0;
        }

        private void RebuildZoomedProfile()
        {
            pxListZoom.Clear();
            if (MainFrm.CurtOrder.pxList.Count <= 0 || panel1.Width <= 0 || panel1.Height <= 0)
                return;

            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;
            foreach (PointF pt in MainFrm.CurtOrder.pxList)
            {
                if (pt.X < minX) minX = pt.X;
                if (pt.X > maxX) maxX = pt.X;
                if (pt.Y < minY) minY = pt.Y;
                if (pt.Y > maxY) maxY = pt.Y;
            }

            float cx = panel1.Width / 2f;
            float cy = panel1.Height / 2f;
            float rangeX = Math.Max(1f, maxX - minX);
            float rangeY = Math.Max(1f, maxY - minY);
            float availW = Math.Max(1f, panel1.Width - 260);
            float availH = Math.Max(1f, panel1.Height - 260);
            zoom = Math.Min(availW / rangeX, availH / rangeY);

            double ox = (maxX + minX) / 2.0;
            double oy = (maxY + minY) / 2.0;
            double deltaX = ox - cx;
            double deltaY = oy - cy;

            foreach (PointF src in MainFrm.CurtOrder.pxList)
            {
                pxListZoom.Add(new PointF(
                    (float)((src.X - deltaX - cx) * zoom + cx),
                    (float)((src.Y - deltaY - cy) * zoom + cy)));
            }
        }

        private void EnsureSelectedStateValid()
        {
            bool found = false;
            int displayIndex = 1;
            int firstFoldOrder = 0;

            for (int i = 0; i < MainFrm.CurtOrder.lstSemiAuto.Count; i++)
            {
                MainFrm.SemiAutoType step = MainFrm.CurtOrder.lstSemiAuto[i];
                if (step.行动类型 != MainFrm.SemiAutoActionFold)
                    continue;

                if (firstFoldOrder == 0)
                    firstFoldOrder = step.折弯序号;

                if (step.折弯序号 == sel折弯序号)
                {
                    selStep = displayIndex;
                    found = true;
                    break;
                }
                displayIndex++;
            }

            if (!found)
            {
                sel折弯序号 = firstFoldOrder;
                selStep = 1;
            }
        }

        private void RebuildFoldButtons()
        {
            anchors.Clear();
            panel1.Controls.Clear();
            if (pxListZoom.Count <= 0)
                return;

            float cx = panel1.Width / 2f;
            float cy = panel1.Height / 2f;
            int displayIndex = 1;

            for (int i = 0; i < MainFrm.CurtOrder.lstSemiAuto.Count; i++)
            {
                MainFrm.SemiAutoType step = MainFrm.CurtOrder.lstSemiAuto[i];
                if (step.行动类型 != MainFrm.SemiAutoActionFold)
                    continue;

                int pxIdx = Math.Clamp(step.坐标序号, 0, pxListZoom.Count - 1);
                PointF anchor = pxListZoom[pxIdx];
                anchors[step.折弯序号] = anchor;

                Button btn = new Button
                {
                    Name = "btnFun" + displayIndex.ToString("D2"),
                    Font = new Font("宋体", 9F, FontStyle.Bold, GraphicsUnit.Point),
                    Size = new Size(80, 32),
                    Tag = step.折弯序号,
                    Text = GetStepButtonText(step, displayIndex),
                    BackColor = step.折弯序号 == sel折弯序号 ? Color.FromArgb(161, 0, 0) : Color.SeaShell,
                    ForeColor = step.折弯序号 == sel折弯序号 ? Color.White : Color.Black,
                    FlatStyle = FlatStyle.Popup,
                    Location = Point.Round(GetStepButtonLocation(step, anchor, cx, cy))
                };
                btn.Click += myButton_Click;
                panel1.Controls.Add(btn);
                displayIndex++;
            }
        }

        private PointF GetStepButtonLocation(MainFrm.SemiAutoType step, PointF anchor, float cx, float cy)
        {
            PointF loc = anchor;
            if (step.折弯角度 == 3.001)
            {
                loc.X -= 50;
                loc.Y -= 40;
            }
            else if (step.折弯角度 == 3.99)
            {
                loc.X += 10;
                loc.Y -= 40;
            }
            else
            {
                loc.X += anchor.X < cx ? -80 : 40;
                loc.Y += anchor.Y < cy ? -40 : 40;
            }
            return loc;
        }

        private string GetStepButtonText(MainFrm.SemiAutoType step, int displayIndex)
        {
            string title = Strings.Get("AutoSet.FoldPrefix") + displayIndex.ToString("D2") + "\r\n";
            string foldDirection = LocalizationText.FoldDirectionShort(step.折弯方向);
            string release = LocalizationText.ReleaseHeightShort(step.松开高度);
            string side = step.内外选择 == 1 ? "B" : "A";
            return title + foldDirection + " " + release + side;
        }

        private void UpdateSelectedStepPanels()
        {
            MainFrm.SemiAutoType step = default;
            bool found = false;
            for (int i = 0; i < MainFrm.CurtOrder.lstSemiAuto.Count; i++)
            {
                if (MainFrm.CurtOrder.lstSemiAuto[i].折弯序号 != sel折弯序号)
                    continue;

                step = MainFrm.CurtOrder.lstSemiAuto[i];
                found = true;
                break;
            }

            if (!found)
            {
                label12.Text = string.Empty;
                label19.Text = string.Empty;
                label32.Text = string.Empty;
                label1.Text = string.Empty;
                return;
            }

            db抓取类型子项 = step.抓取类型;
            db松开高度子项 = step.松开高度;
            db内外选择子项 = step.内外选择;
            db折弯方向子项 = step.折弯方向;
            label19.Text = MainFrm.FormatDisplayLength(step.后挡位置);
            label32.Text = step.折弯序号.ToString();
            label1.Text = selStep.ToString();
            label12.Text = string.Empty;
            foreach (Control ctrl in panel1.Controls)
            {
                if (ctrl is Button btn && btn.Tag is int order && order == sel折弯序号)
                {
                    label12.Text = btn.Name;
                    break;
                }
            }

            btn抓取类型.BackgroundImage = (int)db抓取类型子项 switch
            {
                0 => Properties.Resources.btm_3档开关彩1,
                1 => Properties.Resources.btm_3档开关彩2,
                _ => Properties.Resources.btm_3档开关彩3
            };
            lb抓取类型_推动.ForeColor = (int)db抓取类型子项 == 0 ? Color.FromArgb(240, 28, 28) : Color.White;
            lb抓取类型_抓取.ForeColor = (int)db抓取类型子项 == 1 ? Color.FromArgb(240, 134, 81) : Color.White;
            lb抓取类型_超程抓取.ForeColor = (int)db抓取类型子项 == 2 ? Color.FromArgb(72, 209, 72) : Color.White;

            btn松开高度.BackgroundImage = (int)db松开高度子项 switch
            {
                0 => Properties.Resources.btm_4档开关1,
                1 => Properties.Resources.btm_4档开关2,
                2 => Properties.Resources.btm_4档开关3,
                _ => Properties.Resources.btm_4档开关4
            };
            lb松开高度_低.ForeColor = (int)db松开高度子项 == 0 ? Color.FromArgb(96, 176, 255) : Color.White;
            lb松开高度_中.ForeColor = (int)db松开高度子项 == 1 ? Color.FromArgb(96, 176, 255) : Color.White;
            lb松开高度_高.ForeColor = (int)db松开高度子项 == 2 ? Color.FromArgb(96, 176, 255) : Color.White;
            lb松开高度_最大.ForeColor = (int)db松开高度子项 == 3 ? Color.FromArgb(96, 176, 255) : Color.White;

            btn内外选择.BackgroundImage = (int)db内外选择子项 == 0
                ? Properties.Resources.btm_单工双工0
                : Properties.Resources.btm_单工双工1;
            lb折弯方向AB.Text = (int)db内外选择子项 == 1 ? "B" : "A";
            lb折弯方向AB.ForeColor = (int)db内外选择子项 == 1 ? Color.FromArgb(255, 128, 0) : Color.FromArgb(96, 176, 255);
            lb内外选择_A在外.ForeColor = (int)db内外选择子项 == 0 ? Color.FromArgb(72, 209, 72) : Color.White;
            lb内外选择_B在外.ForeColor = (int)db内外选择子项 == 1 ? Color.FromArgb(240, 134, 81) : Color.White;

            btn折弯方向.BackgroundImage = (int)db折弯方向子项 == 0
                ? Properties.Resources.btm_单工双工0
                : Properties.Resources.btm_单工双工1;
            pnl当前折弯方向.BackgroundImage = (int)db折弯方向子项 == 0
                ? Properties.Resources.BottomApron4
                : Properties.Resources.TopApron4;
        }

        private void panel1_Paint(object? sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.Clear(Color.FromArgb(33, 40, 48));

            if (pxListZoom.Count <= 1)
                return;

            using Pen profilePen = new(Color.FromArgb(119, 151, 217), 3);
            for (int i = 1; i < pxListZoom.Count; i++)
                e.Graphics.DrawLine(profilePen, pxListZoom[i - 1], pxListZoom[i]);

            DrawSquash(e.Graphics);

            using Pen connectorPen = new(Color.PeachPuff, 1);
            foreach (Control ctrl in panel1.Controls)
            {
                if (ctrl is not Button btn || btn.Tag is not int order || !anchors.TryGetValue(order, out PointF anchor))
                    continue;
                e.Graphics.DrawLine(connectorPen, anchor, GetConnectorPoint(btn.Bounds, anchor));
            }

            using SolidBrush aBrush = new(Color.FromArgb(128, 255, 128));
            using SolidBrush bBrush = new(Color.FromArgb(255, 128, 0));
            using Font pointFont = new("Arial", 21.75F, FontStyle.Regular, GraphicsUnit.Point);
            e.Graphics.DrawString("A", pointFont, aBrush, pxListZoom[0].X - 40, pxListZoom[0].Y);
            e.Graphics.DrawString("B", pointFont, bBrush, pxListZoom[^1].X + 10, pxListZoom[^1].Y);
        }

        private void DrawSquash(Graphics g)
        {
            if (pxListZoom.Count <= 1)
                return;

            using Pen squashPen = new(Color.FromArgb(155, 187, 253), 4);
            if (MainFrm.CurtOrder.lengAngle[0].Angle > 0)
            {
                PointF p0 = pxListZoom[0];
                PointF p1 = pxListZoom[1];
                double len0 = MainFrm.CurtOrder.lengAngle[0].Length;
                double deltaY = (p1.X - p0.X) * 3.5 * zoom / Math.Sqrt(Math.Pow(p0.X - p1.X, 2) + Math.Pow(p0.Y - p1.Y, 2));
                double deltaX = (p0.Y - p1.Y) * 3.5 * zoom / Math.Sqrt(Math.Pow(p0.X - p1.X, 2) + Math.Pow(p0.Y - p1.Y, 2));
                double stepY = (p1.Y - p0.Y) * len0 * zoom / Math.Sqrt(Math.Pow(p0.X - p1.X, 2) + Math.Pow(p0.Y - p1.Y, 2));
                double stepX = (p1.X - p0.X) * len0 * zoom / Math.Sqrt(Math.Pow(p0.X - p1.X, 2) + Math.Pow(p0.Y - p1.Y, 2));

                PointF s0;
                PointF s1;
                if (MainFrm.CurtOrder.lengAngle[0].Angle == 2 || MainFrm.CurtOrder.lengAngle[0].Angle == 4)
                {
                    s0 = new PointF(p0.X + (float)deltaX, p0.Y + (float)deltaY);
                    s1 = new PointF(s0.X + (float)stepX, s0.Y + (float)stepY);
                }
                else
                {
                    s0 = new PointF(p0.X - (float)deltaX, p0.Y - (float)deltaY);
                    s1 = new PointF(s0.X + (float)stepX, s0.Y + (float)stepY);
                }
                g.DrawLine(squashPen, s0, s1);
                g.DrawLine(squashPen, s0, p0);
            }

            if (MainFrm.CurtOrder.lengAngle[99].Angle > 0)
            {
                int m = pxListZoom.Count - 1;
                PointF p0 = pxListZoom[m - 1];
                PointF p1 = pxListZoom[m];
                double len99 = MainFrm.CurtOrder.lengAngle[99].Length;
                double deltaY = (p1.X - p0.X) * 3.5 * zoom / Math.Sqrt(Math.Pow(p0.X - p1.X, 2) + Math.Pow(p0.Y - p1.Y, 2));
                double deltaX = (p0.Y - p1.Y) * 3.5 * zoom / Math.Sqrt(Math.Pow(p0.X - p1.X, 2) + Math.Pow(p0.Y - p1.Y, 2));
                double stepY = (p1.Y - p0.Y) * len99 * zoom / Math.Sqrt(Math.Pow(p0.X - p1.X, 2) + Math.Pow(p0.Y - p1.Y, 2));
                double stepX = (p1.X - p0.X) * len99 * zoom / Math.Sqrt(Math.Pow(p0.X - p1.X, 2) + Math.Pow(p0.Y - p1.Y, 2));

                PointF s0;
                PointF s1;
                if (MainFrm.CurtOrder.lengAngle[99].Angle == 2 || MainFrm.CurtOrder.lengAngle[99].Angle == 4)
                {
                    s0 = new PointF(p1.X + (float)deltaX, p1.Y + (float)deltaY);
                    s1 = new PointF(s0.X - (float)stepX, s0.Y - (float)stepY);
                }
                else
                {
                    s0 = new PointF(p1.X - (float)deltaX, p1.Y - (float)deltaY);
                    s1 = new PointF(s0.X - (float)stepX, s0.Y - (float)stepY);
                }
                g.DrawLine(squashPen, s0, s1);
                g.DrawLine(squashPen, s0, p1);
            }
        }

        private static PointF GetConnectorPoint(Rectangle bounds, PointF anchor)
        {
            float x = anchor.X < bounds.Left ? bounds.Left : (anchor.X > bounds.Right ? bounds.Right : anchor.X);
            float y = anchor.Y < bounds.Top ? bounds.Top : (anchor.Y > bounds.Bottom ? bounds.Bottom : anchor.Y);
            return new PointF(x, y);
        }

        private void myButton_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int order)
                return;

            sel折弯序号 = order;
            if (btn.Name.Length >= 8 && int.TryParse(btn.Name.Substring(6, 2), out int displayIndex))
                selStep = displayIndex;

            RefreshLayoutSetting();
        }

        private void btnMoveFront_Click(object? sender, EventArgs e)
        {
            int i上移步 = 0;
            int i移动量 = 0;
            if (selStep <= 1)
                return;

            for (int i = 0; i < MainFrm.CurtOrder.lstSemiAuto.Count; i++)
            {
                if (MainFrm.CurtOrder.lstSemiAuto[i].折弯序号 != sel折弯序号)
                    continue;

                if (i <= 0)
                    return;

                i上移步 = (MainFrm.CurtOrder.lstSemiAuto[i - 1].行动类型 == MainFrm.SemiAutoActionSquash
                        || MainFrm.CurtOrder.lstSemiAuto[i - 1].行动类型 == MainFrm.SemiAutoActionOpenSquash) ? 2 : 1;
                i移动量 = (MainFrm.CurtOrder.lstSemiAuto[i].折弯角度 == 3.99 || MainFrm.CurtOrder.lstSemiAuto[i].折弯角度 == 3.001) ? 2 : 1;
                break;
            }

            for (int i = 0; i < MainFrm.CurtOrder.lstSemiAuto.Count; i++)
            {
                if (MainFrm.CurtOrder.lstSemiAuto[i].折弯序号 != sel折弯序号)
                    continue;

                var temp1 = MainFrm.CurtOrder.lstSemiAuto[i];
                temp1.折弯序号 -= i上移步;
                MainFrm.CurtOrder.lstSemiAuto[i] = temp1;

                if (i移动量 == 2)
                {
                    var temp2 = MainFrm.CurtOrder.lstSemiAuto[i + 1];
                    temp2.折弯序号 -= i上移步;
                    MainFrm.CurtOrder.lstSemiAuto[i + 1] = temp2;
                }

                var temp21 = MainFrm.CurtOrder.lstSemiAuto[i - i上移步];
                temp21.折弯序号 += i移动量;
                MainFrm.CurtOrder.lstSemiAuto[i - i上移步] = temp21;

                if (i上移步 == 2)
                {
                    var temp22 = MainFrm.CurtOrder.lstSemiAuto[i - i上移步 + 1];
                    temp22.折弯序号 += i移动量;
                    MainFrm.CurtOrder.lstSemiAuto[i - i上移步 + 1] = temp22;
                }

                sel折弯序号 -= i上移步;
                break;
            }

            SortSemiLst();
            MainFrm.RebuildSemiAutoDerivedState(ref MainFrm.CurtOrder);
            mf?.MarkSemiAutoStepsManuallyEdited();
            RefreshLayoutSetting();
        }

        private void btnMoveRear_Click(object? sender, EventArgs e)
        {
            int i下移量 = 0;
            int i上移量 = 0;
            if (sel折弯序号 >= MainFrm.CurtOrder.lstSemiAuto.Count)
                return;

            for (int i = 0; i < MainFrm.CurtOrder.lstSemiAuto.Count; i++)
            {
                if (MainFrm.CurtOrder.lstSemiAuto[i].折弯序号 != sel折弯序号)
                    continue;

                if (MainFrm.CurtOrder.lstSemiAuto[i].折弯角度 == 3.99 || MainFrm.CurtOrder.lstSemiAuto[i].折弯角度 == 3.001)
                {
                    i下移量 = 2;
                    i上移量 = ((i + 2) < MainFrm.CurtOrder.lstSemiAuto.Count
                        && (MainFrm.CurtOrder.lstSemiAuto[i + 2].折弯角度 == 3.99 || MainFrm.CurtOrder.lstSemiAuto[i + 2].折弯角度 == 3.001)) ? 2 : 1;
                }
                else
                {
                    i下移量 = 1;
                    i上移量 = ((i + 1) < MainFrm.CurtOrder.lstSemiAuto.Count
                        && (MainFrm.CurtOrder.lstSemiAuto[i + 1].折弯角度 == 3.99 || MainFrm.CurtOrder.lstSemiAuto[i + 1].折弯角度 == 3.001)) ? 2 : 1;
                }
                break;
            }

            for (int i = MainFrm.CurtOrder.lstSemiAuto.Count - 1; i >= 0; i--)
            {
                if (MainFrm.CurtOrder.lstSemiAuto[i].折弯序号 != sel折弯序号)
                    continue;

                var temp11 = MainFrm.CurtOrder.lstSemiAuto[i];
                temp11.折弯序号 += i上移量;
                MainFrm.CurtOrder.lstSemiAuto[i] = temp11;

                if (i下移量 == 2)
                {
                    var temp12 = MainFrm.CurtOrder.lstSemiAuto[i + 1];
                    temp12.折弯序号 += i上移量;
                    MainFrm.CurtOrder.lstSemiAuto[i + 1] = temp12;
                }

                var temp21 = MainFrm.CurtOrder.lstSemiAuto[i + i下移量];
                temp21.折弯序号 -= i下移量;
                MainFrm.CurtOrder.lstSemiAuto[i + i下移量] = temp21;

                if (i上移量 == 2)
                {
                    var temp22 = MainFrm.CurtOrder.lstSemiAuto[i + i下移量 + 1];
                    temp22.折弯序号 -= i下移量;
                    MainFrm.CurtOrder.lstSemiAuto[i + i下移量 + 1] = temp22;
                }

                sel折弯序号 += i下移量;
                break;
            }

            SortSemiLst();
            MainFrm.RebuildSemiAutoDerivedState(ref MainFrm.CurtOrder);
            mf?.MarkSemiAutoStepsManuallyEdited();
            RefreshLayoutSetting();
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

        private void btn内外选择_Click(object? sender, EventArgs e)
        {
            UpdateSelectedFoldStep(step =>
            {
                db内外选择子项 = 1 - step.内外选择;
                step.内外选择 = (int)db内外选择子项;
                return step;
            }, true);
        }

        private void lb内外选择_A在外_Click(object? sender, EventArgs e)
        {
            if (sender is not Label lb)
                return;

            UpdateSelectedFoldStep(step =>
            {
                db内外选择子项 = lb.Name == "lb内外选择_B在外" ? 1 : 0;
                step.内外选择 = (int)db内外选择子项;
                return step;
            }, true);
        }

        private void btn抓取类型_Click(object? sender, EventArgs e)
        {
            UpdateSelectedFoldStep(step =>
            {
                db抓取类型子项 = (step.抓取类型 + 1) % 3;
                step.抓取类型 = (int)db抓取类型子项;
                return step;
            }, false);
        }

        private void lb抓取类型_推动_Click(object? sender, EventArgs e)
        {
            if (sender is not Label lb)
                return;

            UpdateSelectedFoldStep(step =>
            {
                db抓取类型子项 = lb.Name switch
                {
                    "lb抓取类型_推动" => 0,
                    "lb抓取类型_抓取" => 1,
                    _ => 2
                };
                step.抓取类型 = (int)db抓取类型子项;
                return step;
            }, false);
        }

        private void btn松开高度_Click(object? sender, EventArgs e)
        {
            UpdateSelectedFoldStep(step =>
            {
                db松开高度子项 = (step.松开高度 + 1) % 4;
                step.松开高度 = (int)db松开高度子项;
                return step;
            }, false);
        }

        private void lb松开高度_低_Click(object? sender, EventArgs e)
        {
            if (sender is not Label lb)
                return;

            UpdateSelectedFoldStep(step =>
            {
                db松开高度子项 = lb.Name switch
                {
                    "lb松开高度_低" => 0,
                    "lb松开高度_中" => 1,
                    "lb松开高度_高" => 2,
                    _ => 3
                };
                step.松开高度 = (int)db松开高度子项;
                return step;
            }, false);
        }

        private void btn折弯方向_Click(object? sender, EventArgs e)
        {
            UpdateSelectedFoldStep(step =>
            {
                db折弯方向子项 = (step.折弯方向 + 1) % 2;
                step.折弯方向 = (int)db折弯方向子项;
                return step;
            }, true);
        }

        private void pnl折弯方向_向下_Click(object? sender, EventArgs e)
        {
            if (sender is not Panel pnl)
                return;

            UpdateSelectedFoldStep(step =>
            {
                db折弯方向子项 = pnl.Name == "pnl折弯方向_向上" ? 1 : 0;
                step.折弯方向 = (int)db折弯方向子项;
                return step;
            }, true);
        }

        private void btnPlus_Click(object? sender, EventArgs e)
        {
            UpdateSelectedFoldStep(step =>
            {
                if (step.回弹值 < 9)
                    step.回弹值 += 0.5;
                return step;
            }, false);
        }

        private void btnMinus_Click(object? sender, EventArgs e)
        {
            UpdateSelectedFoldStep(step =>
            {
                if (step.回弹值 > -9)
                    step.回弹值 -= 0.5;
                return step;
            }, false);
        }

        private void UpdateSelectedFoldStep(Func<MainFrm.SemiAutoType, MainFrm.SemiAutoType> updater, bool recalcBackGauge)
        {
            for (int i = 0; i < MainFrm.CurtOrder.lstSemiAuto.Count; i++)
            {
                if (MainFrm.CurtOrder.lstSemiAuto[i].折弯序号 != sel折弯序号)
                    continue;

                MainFrm.CurtOrder.lstSemiAuto[i] = updater(MainFrm.CurtOrder.lstSemiAuto[i]);
                if (recalcBackGauge)
                    MainFrm.RebuildSemiAutoDerivedState(ref MainFrm.CurtOrder);
                mf?.MarkSemiAutoStepsManuallyEdited();
                break;
            }

            RefreshLayoutSetting();
        }

        private void sw正逆序_Click(object? sender, EventArgs e)
        {
            MainFrm.CurtOrder.st逆序 = !MainFrm.CurtOrder.st逆序;
            mf?.create生产序列();
            RefreshLayoutSetting();
        }

        private void sw颜色面_Click(object? sender, EventArgs e)
        {
            MainFrm.CurtOrder.st色下 = !MainFrm.CurtOrder.st色下;
            mf?.create生产序列();
            RefreshLayoutSetting();
        }

        private void lb正逆序_Click(object? sender, EventArgs e)
        {
            sw正逆序_Click(sender, e);
        }

        private void lb颜色面_Click(object? sender, EventArgs e)
        {
            sw颜色面_Click(sender, e);
        }

        private void btnRstView_Click(object? sender, EventArgs e)
        {
            RestoreOriginalLayoutState();
            selStep = 1;
            sel折弯序号 = 0;
        }

        private void btnResetLayoutDraft_Click(object? sender, EventArgs e)
        {
            RestoreOriginalLayoutState();
            selStep = 1;
            sel折弯序号 = 0;
        }

        private void btnConfirmLayout_Click(object? sender, EventArgs e)
        {
            if (mf == null)
                return;

            MainFrm.SemiAutoPlanValidationResult validation = mf.ValidateCurrentFormalSemiAutoPlan(MainFrm.CurtOrder.lstSemiAuto);
            if (!validation.IsAccepted)
            {
                MessageBox.Show(validation.Message, "布置确认失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (validation.RequiresConfirmation)
            {
                DialogResult confirm = MessageBox.Show(
                    validation.Message + "\r\n\r\n是否仍然确认当前布置？",
                    "布置风险确认",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning);
                if (confirm != DialogResult.OK)
                    return;
            }

            MainFrm.RebuildSemiAutoDerivedState(ref MainFrm.CurtOrder);
            mf.MarkSemiAutoStepsManuallyEdited();
            mf.subOPManual?.LoadGridFromCurrentOrder();
            layoutConfirmed = true;
            mf.ReturnToFoldPreviewFromLayout();
        }
    }
}
