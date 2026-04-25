using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using static JSZW1000A.MainFrm;

namespace JSZW1000A.SubWindows
{
    public partial class SubOPAuto : UserControl
    {
        MainFrm mf;

        // 缓存缩放后的屏幕坐标点，供绘图和TextBox定位使用
        private List<PointF> pxList_Zoom = new List<PointF>();
        private double zoom = 1.0;

        // 用于记录上次输入的值，用于计算差值
        private double oldVal = 0.0001;
        private TextBox? SelTxb; // 当前选中的TextBox
        private FrmCalculator? dlgCal;
        private bool suppressSelectAllOnFocus;
        private const int CalculatorLeft = 21;
        private const int CalculatorTop = 21;

        public SubOPAuto(MainFrm fm1, bool isCal)
        {
            InitializeComponent();

            // 1. 开启 UserControl 自身的双缓冲
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            // 2. 【核心】强行开启 panel1 的双缓冲
            // 注意：如果你的画板叫 panel2，请把下面的 panel1 改为 panel2
            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, panel1, new object[] { true });

            setLang();
            this.mf = fm1;
            CalLength(isCal);
        }

        private void SubOPAuto1_Load(object sender, EventArgs e)
        {
            InitUIEvents();
            InitDataDisplay();

            // 开启双缓冲，防止闪烁
            typeof(Panel).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)!
                          .SetValue(panel1, true);

            panel1.Paint += panel1_Paint;

            // 构造阶段已经按 Designer 默认项创建过挤压按钮；这里在语言资源加载后重建一次，
            // 确保图上挤压类型文本与当前语言一致。
            RefreshAll();
        }

        private void InitUIEvents()
        {
            txb工作单名称.Tag = false;
            txb工作单名称.GotFocus += new EventHandler(textBox_GotFocus);
            txb工作单名称.MouseUp += new MouseEventHandler(textBox_MouseUp);

            txb挤压长度.Tag = false;
            txb挤压长度.GotFocus += new EventHandler(textBox_GotFocus);
            txb挤压长度.MouseUp += new MouseEventHandler(textBox_MouseUp);
            txb挤压长度.Click += new EventHandler(txb挤压长度_Click);

            pnl左工具栏1.Parent = this;
            pnl左工具栏1.Visible = true;
            pnl左工具栏2.Parent = this;
            pnl左工具栏2.Location = new Point(2, 101);
            pnl左工具栏2.Visible = false;
        }

        private void InitDataDisplay()
        {
            txb工作单名称.Text = MainFrm.CurtOrder.Name;
            txb客户.Text = MainFrm.CurtOrder.Customer;
            txb材料.Text = MainFrm.CurtOrder.MaterialName;
            txb长度.Text = MainFrm.FormatDisplayLength(MainFrm.CurtOrder.SheetLength);
            txb厚度.Text = MainFrm.FormatDisplayLength(MainFrm.CurtOrder.Thickness);
            txb备注.Text = MainFrm.CurtOrder.Remark;

            cbx挤压类型.Items.Clear();
            cbx挤压类型.Items.AddRange(new object[]
            {
                Strings.Get("Auto.GripType.None"),
                Strings.Get("Auto.GripType.Up"),
                Strings.Get("Auto.GripType.Down"),
                Strings.Get("Auto.GripType.OpenUp"),
                Strings.Get("Auto.GripType.OpenDown")
            });

            txbSpringTop.Text = string.Format("{0:F2}", MainFrm.SpringTop);
            txbSpringBtm.Text = string.Format("{0:F2}", MainFrm.SpringBtm);
            RefreshInlineSlitButton();
        }

        private void RefreshInlineSlitButton()
        {
            string text = Strings.Get("Auto.InlineSlitButton");
            if (TryGetInlineSlitPieceCount(MainFrm.CurtOrder, out int pieceCount, out _))
                text += " x" + pieceCount;

            button3.Text = text;
            button3.ForeColor = MainFrm.CurtOrder.边做边分切启用
                ? Color.FromArgb(96, 176, 255)
                : Color.FromArgb(208, 208, 208);
        }

        // ==========================================
        // 核心绘图逻辑 (GDI+ 修正版)
        // ==========================================

        private void panel1_Paint(object? sender, PaintEventArgs e)
        {
            // 所有绘图必须在这里进行
            DrawGraphicsInternal(e.Graphics);
        }

        /// <summary>
        /// 全局刷新方法：重新计算坐标 -> 计算缩放 -> 更新控件位置 -> 触发重绘
        /// </summary>
        public void RefreshAll()
        {
            // 1. 根据长度和角度计算物理坐标 (pxList)
            CalculatePhysicalPoints();

            // 2. 计算缩放并生成屏幕坐标 (pxList_Zoom)
            CalculateZoomAndScreenPoints();

            // 3. 重建或更新输入框位置
            reCreateTxb();
            reCreateTaperTxb(false);

            // 4. 触发 Paint 事件进行连线绘制
            panel1.Invalidate();
        }

        private void RefreshViewOnly()
        {
            CalculateZoomAndScreenPoints();
            reCreateTxb();
            reCreateTaperTxb(false);
            panel1.Invalidate();
        }

        private void InitializeDisplayHeadingFromCurrentProfile()
        {
            if (MainFrm.CurtOrder.pxList != null && MainFrm.CurtOrder.pxList.Count > 1)
            {
                float dx = MainFrm.CurtOrder.pxList[1].X - MainFrm.CurtOrder.pxList[0].X;
                float dy = MainFrm.CurtOrder.pxList[1].Y - MainFrm.CurtOrder.pxList[0].Y;
                MainFrm.CurtOrder.显示起始角度 = Math.Atan2(dy, -dx) * 180.0 / Math.PI;
            }
            else
            {
                MainFrm.CurtOrder.显示起始角度 = 180.0;
            }

            MainFrm.CurtOrder.显示朝向已初始化 = true;
        }

        private void MarkProfileChanged()
        {
            mf.MarkSemiAutoSequenceStale();
        }

        private void DrawGraphicsInternal(Graphics g)
        {
            try
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.Clear(Color.FromArgb(33, 40, 48)); // 背景色

                if (pxList_Zoom == null || pxList_Zoom.Count < 2) return;

                // 绘制折弯线段
                using (Pen myPen1 = new Pen(Color.FromArgb(119, 151, 217), 4))
                {
                    for (int k = 1; k < pxList_Zoom.Count; k++)
                    {
                        g.DrawLine(myPen1, pxList_Zoom[k - 1], pxList_Zoom[k]);
                    }
                }

                // 绘制挤压 (Squash)
                if (MainFrm.CurtOrder.lengAngle[0].Angle > 0 || MainFrm.CurtOrder.lengAngle[99].Angle > 0)
                {
                    drawSquash(g, MainFrm.CurtOrder.lengAngle[0].Length, MainFrm.CurtOrder.lengAngle[99].Length);
                }

                // 绘制辅助连接线
                ConnLine(g);
            }
            catch (Exception ex)
            {
                // 异常保护，防止红叉
                Console.WriteLine("Paint Error: " + ex.Message);
            }
        }

        /// <summary>
        /// 计算所有点的几何重心
        /// </summary>
        private PointF GetCentroid()
        {
            if (pxList_Zoom.Count == 0) return new PointF(0, 0);
            float sumX = 0, sumY = 0;
            foreach (var p in pxList_Zoom)
            {
                sumX += p.X;
                sumY += p.Y;
            }
            return new PointF(sumX / pxList_Zoom.Count, sumY / pxList_Zoom.Count);
        }

        /// <summary>
        /// 获取线段 P1->P2 的单位法向量（垂直方向）
        /// 并在必要时翻转，使其指向远离重心的方向
        /// </summary>

        private PointF GetOutwardNormal(PointF p1, PointF p2, PointF centroid)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            float len = (float)Math.Sqrt(dx * dx + dy * dy);
            if (len < 0.1f) return new PointF(0, -1f);

            // 两个可能的法向量（左/右手法则）
            PointF n1 = new PointF(-dy / len, dx / len);   // 逆时针90度
            PointF n2 = new PointF(dy / len, -dx / len);   // 顺时针90度

            // 取线段中点
            float midX = (p1.X + p2.X) / 2f;
            float midY = (p1.Y + p2.Y) / 2f;

            // 计算中点到重心的向量
            float toCenterX = centroid.X - midX;
            float toCenterY = centroid.Y - midY;

            // 选择与“指向重心”反方向的那个法向量（点积 < 0）
            float dot1 = n1.X * toCenterX + n1.Y * toCenterY;
            // 如果 dot1 < 0，说明 n1 指向远离重心 → 就是要的
            return dot1 < 0 ? n1 : n2;
        }
        /// <summary>
        /// 碰撞检测：检查 rect 是否与 list 中的任何矩形相交
        /// </summary>
        private bool IsColliding(Rectangle newRect, List<Rectangle> existingRects)
        {
            foreach (var rect in existingRects)
            {
                // 稍微扩大一点判定范围 (Inflate)，制造间隙
                Rectangle padded = rect;
                padded.Inflate(2, 2);
                if (newRect.IntersectsWith(padded))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 确保矩形在 Panel 范围内，不超出边界
        /// </summary>
        private Point EnsureInBounds(Point pos, Size size, int padding = 5)
        {
            int x = pos.X;
            int y = pos.Y;
            int maxW = panel1.Width;
            int maxH = panel1.Height;

            // 修正 X 轴 (左边和右边)
            if (x < padding) x = padding;
            if (x + size.Width > maxW - padding) x = maxW - size.Width - padding;

            // 修正 Y 轴 (上边和下边)
            if (y < padding) y = padding;
            if (y + size.Height > maxH - padding) y = maxH - size.Height - padding;

            return new Point(x, y);
        }

        /// <summary>
        /// 计算顶点 P2 处的角平分线方向（单位向量），并指向图形外侧
        /// </summary>
        private PointF GetBisectorDirection(PointF p1, PointF p2, PointF p3, PointF centroid)
        {
            // 1. 计算两个邻边向量 (指向 p2)
            // 向量 V1: P1 -> P2
            float v1x = p1.X - p2.X;
            float v1y = p1.Y - p2.Y;
            // 向量 V2: P3 -> P2
            float v2x = p3.X - p2.X;
            float v2y = p3.Y - p2.Y;

            // 2. 归一化 (变成单位向量)
            float len1 = (float)Math.Sqrt(v1x * v1x + v1y * v1y);
            float len2 = (float)Math.Sqrt(v2x * v2x + v2y * v2y);

            if (len1 < 0.1 || len2 < 0.1) return new PointF(0, -1); // 异常保护

            v1x /= len1; v1y /= len1;
            v2x /= len2; v2y /= len2;

            // 3. 向量相加得到平分线 (V_bisect)
            // 因为 V1 和 V2 都是从 P2 发出的(或者指向P2)，这里我们要取夹角的平分线
            // 向量 P2->P1 和 P2->P3 的和
            float bx = v1x + v2x;
            float by = v1y + v2y;
            float blen = (float)Math.Sqrt(bx * bx + by * by);

            // 4. 处理 180 度直线的情况 (模长接近0)
            if (blen < 0.05)
            {
                // 取 V1 的垂线
                bx = -v1y;
                by = v1x;
            }
            else
            {
                bx /= blen;
                by /= blen;
            }

            // 5. 重心排斥：判断方向是否指向重心，如果是，则反转
            // 计算 P2 指向 重心 的向量
            float cx = centroid.X - p2.X;
            float cy = centroid.Y - p2.Y;

            // 点积判断夹角
            float dot = bx * cx + by * cy;

            // 如果点积 > 0，说明平分线指向重心（内侧），需要翻转指向外侧
            if (dot > 0)
            {
                bx = -bx;
                by = -by;
            }

            return new PointF(bx, by);
        }
        // ==========================================
        // 数据计算逻辑 (精度优化版)
        // ==========================================

        /// <summary>
        /// 步骤1：根据 Length 和 Angle 计算实际物理坐标 (pxList)
        /// </summary>
        public void CalculatePhysicalPoints()
        {
            var lengAngleData = MainFrm.CurtOrder.lengAngle;

            // 1. 获取初始绝对角度 (Heading)
            double currentHeading = 0;

            if (!MainFrm.CurtOrder.显示朝向已初始化
                && MainFrm.CurtOrder.pxList != null
                && MainFrm.CurtOrder.pxList.Count > 1)
            {
                float dx = MainFrm.CurtOrder.pxList[1].X - MainFrm.CurtOrder.pxList[0].X;
                float dy = MainFrm.CurtOrder.pxList[1].Y - MainFrm.CurtOrder.pxList[0].Y;
                MainFrm.CurtOrder.显示起始角度 = Math.Atan2(dy, -dx) * 180.0 / Math.PI;
                MainFrm.CurtOrder.显示朝向已初始化 = true;
            }
            else if (!MainFrm.CurtOrder.显示朝向已初始化)
            {
                MainFrm.CurtOrder.显示起始角度 = 180; // 默认向左
                MainFrm.CurtOrder.显示朝向已初始化 = true;
            }

            currentHeading = MainFrm.CurtOrder.显示起始角度;

            var newPxList = new List<PointF>();
            float currentX = 0;
            float currentY = 0;
            newPxList.Add(new PointF(currentX, currentY));

            // 3. 遍历数据重建图形
            for (int i = 1; i < 100; i++)
            {
                // =================================================================
                // 【核心修改】终止条件判断：连续两个点长度为0才结束
                // =================================================================
                bool isCurrentZero = Math.Abs(lengAngleData[i].Length) < 0.001;

                // 检查下一个点是否也为0 (防止数组越界)
                bool isNextZero = true;
                if (i + 1 < 100)
                {
                    isNextZero = Math.Abs(lengAngleData[i + 1].Length) < 0.001;
                }

                // 只有当“当前是0”且“下一个也是0”时，才认为数据结束了，跳出循环
                if (isCurrentZero && isNextZero)
                {
                    break;
                }
                // 如果当前是0但下一个不是0，循环继续执行 -> 
                // 结果是绘制一个长度为0的线（点位置不变），但角度会继续累加，实现“跳过”效果
                // =================================================================

                double len = lengAngleData[i].Length;
                double includedAngle = lengAngleData[i].Angle;

                double deflection = 180 - Math.Abs(includedAngle);
                if (includedAngle < 0) deflection = -deflection;

                currentHeading -= deflection;

                double rad = currentHeading * Math.PI / 180.0;

                float nextX = currentX + (float)(len * Math.Cos(rad));
                float nextY = currentY + (float)(len * Math.Sin(rad));

                newPxList.Add(new PointF(nextX, nextY));
                currentX = nextX;
                currentY = nextY;
            }

            MainFrm.CurtOrder.pxList = newPxList;
        }


        /// <summary>
        /// 步骤2：根据 Panel 大小计算缩放比例，生成屏幕坐标 (pxList_Zoom)
        /// </summary>
        private void CalculateZoomAndScreenPoints()
        {
            if (MainFrm.CurtOrder.pxList == null || MainFrm.CurtOrder.pxList.Count == 0) return;

            // 获取边界
            float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
            foreach (var pt in MainFrm.CurtOrder.pxList)
            {
                if (pt.X < minX) minX = pt.X;
                if (pt.X > maxX) maxX = pt.X;
                if (pt.Y < minY) minY = pt.Y;
                if (pt.Y > maxY) maxY = pt.Y;
            }

            float rangeX = Math.Max(0.1f, maxX - minX);
            float rangeY = Math.Max(0.1f, maxY - minY);

            float cx = panel1.Width / 2f;
            float cy = panel1.Height / 2f;
            float availW = Math.Max(1, panel1.Width - 260); // 留边距
            float availH = Math.Max(1, panel1.Height - 260);

            // 计算缩放比例
            if (availW / rangeX < availH / rangeY)
                zoom = availW / rangeX;
            else
                zoom = availH / rangeY;

            // 限制最大最小缩放（可选）
            if (zoom == 0) zoom = 1;

            double ox = (maxX + minX) / 2.0;
            double oy = (maxY + minY) / 2.0;
            double deltaX = (ox - cx); // 中心点偏移量
            double deltaY = (oy - cy);

            pxList_Zoom.Clear();
            // 应用缩放变换公式： Screen = (World - Offset - Center) * Zoom + Center
            // 修正后的公式逻辑：先移到中心，再缩放
            // 原始逻辑复用：
            for (int i = 0; i < MainFrm.CurtOrder.pxList.Count; i++)
            {
                float pX = (float)((MainFrm.CurtOrder.pxList[i].X - deltaX - cx) * zoom + cx);
                float pY = (float)((MainFrm.CurtOrder.pxList[i].Y - deltaY - cy) * zoom + cy);
                pxList_Zoom.Add(new PointF(pX, pY));
            }
        }

        // ==========================================
        // UI 控件管理
        // ==========================================

        public void reCreateTxb()
        {
            if (pxList_Zoom.Count <= 1) return;

            panel1.SuspendLayout();
            try
            {
                panel1.Controls.Clear();

                List<Rectangle> occupiedRects = new List<Rectangle>();
                PointF centroid = GetCentroid();
                int panelMidY = panel1.Height / 2;

                // ================= 1. 创建长度 (Length) 输入框 =================
                // 策略：跟随法线，贴近图形
                for (int i = 0; i < pxList_Zoom.Count - 1; i++)
                {
                    TextBox newTxt = CreateBaseTextBox("txbLength" + string.Format("{0:D2}", i));
                    newTxt.TabIndex = i;
                    newTxt.Text = MainFrm.FormatDisplayLength(MainFrm.CurtOrder.lengAngle[i + 1].Length);
                    newTxt.BackColor = Color.LightGreen;
                    newTxt.TextAlign = HorizontalAlignment.Center;
                    newTxt.KeyUp += new KeyEventHandler(textBox_KeyUp);

                    PointF p1 = pxList_Zoom[i];
                    PointF p2 = pxList_Zoom[i + 1];
                    PointF mid = new PointF((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);

                    // 获取向外的法向量
                    PointF normal = GetOutwardNormal(p1, p2, centroid);

                    // 基础距离：保持在 40~60 之间错位，贴近线段
                    float currentDist = 40 + (i % 2) * 20;
                    if (i == 0) currentDist += 30;         // 第一段再往外推 30px
                    // 计算位置
                    Point pos = new Point(
                        (int)(mid.X + normal.X * currentDist),
                        (int)(mid.Y + normal.Y * currentDist)
                    );

                    // 居中修正
                    pos.X -= newTxt.Width / 2;
                    pos.Y -= newTxt.Height / 2;

                    // 【关键修复】边界限制：先限制一次，防止离谱的溢出
                    pos = EnsureInBounds(pos, newTxt.Size);

                    // 碰撞推演
                    Rectangle testRect = new Rectangle(pos, newTxt.Size);
                    int retry = 0;
                    while (IsColliding(testRect, occupiedRects) && retry < 15)
                    {
                        // 如果撞了，稍微往外挪一点，但不要挪太远
                        currentDist += 10;
                        Point newPosRaw = new Point(
                            (int)(mid.X + normal.X * currentDist),
                            (int)(mid.Y + normal.Y * currentDist)
                        );
                        // 每次挪动都要重新计算居中并检查边界
                        pos = EnsureInBounds(new Point(newPosRaw.X - newTxt.Width / 2, newPosRaw.Y - newTxt.Height / 2), newTxt.Size);
                        testRect = new Rectangle(pos, newTxt.Size);
                        retry++;
                    }

                    newTxt.Location = pos;
                    occupiedRects.Add(testRect);
                    panel1.Controls.Add(newTxt);
                }

                // ================= 2. 创建角度 (Angle) 输入框 =================
                // 策略：放置在角平分线上 (Angle Bisector)
                for (int i = 0; i < pxList_Zoom.Count - 2; i++)
                {
                    TextBox angleTxt = CreateBaseTextBox("txbAngles" + string.Format("{0:D2}", i));
                    angleTxt.TabIndex = 100 + i;
                    angleTxt.Text = Convert.ToString(Math.Round(MainFrm.CurtOrder.lengAngle[i + 2].Angle, 1, MidpointRounding.AwayFromZero));
                    angleTxt.BackColor = Color.PeachPuff;
                    angleTxt.TextAlign = HorizontalAlignment.Center;

                    // 获取三个点：前点、当前顶点、后点
                    PointF pPrev = pxList_Zoom[i];
                    PointF pCurr = pxList_Zoom[i + 1];
                    PointF pNext = pxList_Zoom[i + 2];

                    // 1. 获取指向外侧的角平分线向量
                    PointF bisector = GetBisectorDirection(pPrev, pCurr, pNext, centroid);

                    // 2. 基础距离设置
                    // 角度标签通常比长度标签远一点，避免重叠
                    // 增加错位逻辑：奇数和偶数距离不同，防止相邻的锐角标签打架
                    float dist = 70 + (i % 2) * 30;

                    // 3. 计算初始位置：顶点 + 向量 * 距离
                    Point pos = new Point(
                        (int)(pCurr.X + bisector.X * dist),
                        (int)(pCurr.Y + bisector.Y * dist)
                    );

                    // 居中修正 (TextBox左上角坐标)
                    pos.X -= angleTxt.Width / 2;
                    pos.Y -= angleTxt.Height / 2;

                    // 边界限制 (防止跑出屏幕)
                    pos = EnsureInBounds(pos, angleTxt.Size);

                    // 4. 碰撞推演 (沿平分线方向向外推)
                    Rectangle testRect = new Rectangle(pos, angleTxt.Size);
                    int retry = 0;
                    while (IsColliding(testRect, occupiedRects) && retry < 20)
                    {
                        dist += 15; // 每次往外推 15px

                        // 重新计算位置
                        Point newPosRaw = new Point(
                            (int)(pCurr.X + bisector.X * dist),
                            (int)(pCurr.Y + bisector.Y * dist)
                        );

                        // 重新居中并限制边界
                        pos = EnsureInBounds(new Point(newPosRaw.X - angleTxt.Width / 2, newPosRaw.Y - angleTxt.Height / 2), angleTxt.Size);
                        testRect = new Rectangle(pos, angleTxt.Size);
                        retry++;
                    }

                    angleTxt.Location = pos;
                    occupiedRects.Add(testRect);
                    panel1.Controls.Add(angleTxt);
                }

                // 3. 挤压按钮 (同样加边界限制)
                if (pxList_Zoom.Count > 0)
                {
                    AddSquashButton(0, "btnSquash0", pxList_Zoom[0], -70, 5);
                    AddSquashButton(99, "btnSquash1", pxList_Zoom[pxList_Zoom.Count - 1], -70, 5);
                }
            }
            finally
            {
                panel1.ResumeLayout();
            }
        }

        private TextBox CreateBaseTextBox(string name)
        {
            TextBox tb = new TextBox();
            tb.Name = name;
            tb.Font = new Font("Calibri", 13F, FontStyle.Regular);
            tb.Size = new Size(60, 30);
            tb.Width = 50;
            tb.Click += new EventHandler(textBox_Click);
            tb.KeyDown += new KeyEventHandler(textBox_KeyDown);
            tb.GotFocus += new EventHandler(textBox_GotFocus);
            tb.MouseUp += new MouseEventHandler(textBox_MouseUp);
            tb.Tag = false;
            return tb;
        }

        private void AddSquashButton(int index, string name, PointF loc, int offX, int offY)
        {
            Button btn = new Button();
            btn.Name = name;
            btn.Font = new Font("宋体", 9F);
            btn.Size = new Size(65, 25);

            // 设置按钮文字
            int angleIdx = Convert.ToInt32(MainFrm.CurtOrder.lengAngle[index == 0 ? 0 : 99].Angle);
            if (angleIdx >= 0 && angleIdx < cbx挤压类型.Items.Count)
            {
                btn.Text = angleIdx == 0
                    ? Strings.Get("Auto.GripType.None")
                    : Convert.ToString(cbx挤压类型.Items[angleIdx]) ?? string.Empty;
            }

            btn.BackColor = Color.SlateGray;
            btn.FlatStyle = FlatStyle.Popup;
            btn.Click += new EventHandler(myButton_Click);

            // ================================================================
            // 核心算法：基于向量的动态布局
            // ================================================================

            // 1. 计算向外的延伸向量 (Direction Vector)
            float dirX = 0;
            float dirY = 0;

            if (pxList_Zoom.Count > 1)
            {
                if (index == 0) // 头部：方向是 P1 -> P0 (向外延伸)
                {
                    dirX = pxList_Zoom[0].X - pxList_Zoom[1].X;
                    dirY = pxList_Zoom[0].Y - pxList_Zoom[1].Y;
                }
                else // 尾部：方向是 P(n-1) -> P(n) (向外延伸)
                {
                    int n = pxList_Zoom.Count - 1;
                    dirX = pxList_Zoom[n].X - pxList_Zoom[n - 1].X;
                    dirY = pxList_Zoom[n].Y - pxList_Zoom[n - 1].Y;
                }
            }

            // 归一化向量 (变成长度为1的单位向量)
            float len = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
            if (len < 0.1f) len = 1; // 防止除零
            dirX /= len;
            dirY /= len;

            // 2. 动态计算位置
            // 我们希望按钮沿着线段方向往外跑，不挡住线
            // Gap 是按钮与端点的距离
            int gap = 20;

            int finalX = (int)loc.X;
            int finalY = (int)loc.Y;

            // --- 智能象限判断 ---

            // 如果线是水平向左 (dirX < -0.5)
            // 按钮放在左边 (X 减去宽度)
            if (dirX < -0.5)
            {
                finalX = (int)loc.X - btn.Width - gap;
                // 微调Y轴：尽量让点在按钮右上角 -> 按钮下沉
                finalY = (int)loc.Y + 5;
            }
            // 如果线是水平向右 (dirX > 0.5)
            // 按钮放在右边
            else if (dirX > 0.5)
            {
                finalX = (int)loc.X + gap;
                // 按钮下沉
                finalY = (int)loc.Y + 5;
            }
            // 如果线是垂直向上 (dirY < 0)
            // 按钮放在上边
            else if (dirY < 0)
            {
                finalX = (int)loc.X - btn.Width / 2; // X居中
                finalY = (int)loc.Y - btn.Height - gap;
            }
            // 如果线是垂直向下 (dirY > 0)
            // 按钮放在下边
            else
            {
                finalX = (int)loc.X - btn.Width / 2; // X居中
                finalY = (int)loc.Y + gap;
            }

            // 3. 边界限制 (防止按钮跑出屏幕)
            Point proposed = new Point(finalX, finalY);
            proposed = EnsureInBounds(proposed, btn.Size, 5);

            btn.Location = proposed;
            panel1.Controls.Add(btn);
        }

        public void reCreateTaperTxb(bool b)
        {
            // 1. 先挂起布局，防止闪烁
            panel1.SuspendLayout();

            // 2. 清除旧的 Taper 输入框 (倒序删除是安全的)
            for (int i = panel1.Controls.Count - 1; i >= 0; i--)
            {
                if (panel1.Controls[i].Name.StartsWith("txbTaperW"))
                {
                    panel1.Controls.RemoveAt(i);
                }
            }

            // 3. 如果未开启锥度功能，直接返回
            if (!MainFrm.CurtOrder.isTaper)
            {
                panel1.ResumeLayout();
                return;
            }

            // ==========================================
            // 布局参数设置
            // ==========================================
            int maxPerRow = 20;  // 每行最多显示个数
            int startX = 10;     // 起始 X 坐标 (左边距)
            int startY = 10;     // 起始 Y 坐标 (顶边距)
            int boxW = 50;       // 控件宽度
            int boxH = 30;       // 控件高度
            int gapX = 5;        // 水平间距
            int gapY = 5;        // 垂直间距 (行距)

            // ==========================================
            // 循环创建控件
            // ==========================================
            // 锥度数据通常对应线段数量，所以是 pxList_Zoom.Count - 1
            for (int i = 0; i < pxList_Zoom.Count - 1; i++)
            {
                // 确保数据初始化
                if (MainFrm.CurtOrder.lengAngle[i + 1].TaperWidth == 0)
                    MainFrm.CurtOrder.lengAngle[i + 1].TaperWidth = MainFrm.CurtOrder.lengAngle[i + 1].Length;

                // 创建 TextBox
                TextBox newTxt = new TextBox();
                newTxt.Name = "txbTaperW" + string.Format("{0:D2}", i);
                newTxt.Font = new System.Drawing.Font("Calibri", 13F, FontStyle.Regular, GraphicsUnit.Point);
                newTxt.Size = new Size(boxW, boxH);
                newTxt.TabIndex = 200 + i;
                newTxt.Text = MainFrm.FormatDisplayLength(MainFrm.CurtOrder.lengAngle[i + 1].TaperWidth);
                newTxt.BackColor = Color.LightSkyBlue; // 保持你截图中的蓝色风格
                newTxt.TextAlign = HorizontalAlignment.Center;
                newTxt.Tag = false; // 用于全选标记

                // 绑定事件
                newTxt.Click += new EventHandler(textBox_Click);
                newTxt.KeyDown += new KeyEventHandler(textBox_KeyDown);
                newTxt.GotFocus += new EventHandler(textBox_GotFocus);
                newTxt.MouseUp += new MouseEventHandler(textBox_MouseUp);

                // ==========================================
                // 核心计算逻辑：换行处理
                // ==========================================
                int row = i / maxPerRow; // 计算当前是第几行 (0, 1, 2...)
                int col = i % maxPerRow; // 计算当前是第几列 (0 ~ 19)

                int x = startX + col * (boxW + gapX);
                int y = startY + row * (boxH + gapY);

                newTxt.Location = new Point(x, y);

                panel1.Controls.Add(newTxt);
            }

            // 4. 恢复布局
            panel1.ResumeLayout(false);
            
        }

        // ==========================================
        // 辅助绘图 (挤压与连线)
        // ==========================================

        private void drawSquash(Graphics g, double SquaL0, double SquaL99)
        {
            if (pxList_Zoom.Count < 2) return;
            using (Pen myPen2 = new Pen(Color.FromArgb(155, 187, 253), 4))
            {
                // 头部挤压 (Index 0) - 传入 isHead = true
                if (MainFrm.CurtOrder.lengAngle[0].Angle > 0)
                {
                    DrawSingleSquash(g, myPen2, pxList_Zoom[0], pxList_Zoom[1], SquaL0, (int)MainFrm.CurtOrder.lengAngle[0].Angle, true);
                }
                // 尾部挤压 (Index 99) - 传入 isHead = false
                if (MainFrm.CurtOrder.lengAngle[99].Angle > 0)
                {
                    int m = pxList_Zoom.Count - 1;
                    DrawSingleSquash(g, myPen2, pxList_Zoom[m], pxList_Zoom[m - 1], SquaL99, (int)MainFrm.CurtOrder.lengAngle[99].Angle, false);
                }
            }
        }

        // 增加了一个 bool isHead 参数来区分头尾
        private void DrawSingleSquash(Graphics g, Pen p, PointF pStart, PointF pRef, double len, int type, bool isHead)
        {
            double dist = Math.Sqrt(Math.Pow(pStart.X - pRef.X, 2) + Math.Pow(pStart.Y - pRef.Y, 2));
            if (dist < 0.001) return;

            // 1. 计算基准向量 (Start -> Ref)
            // 头部是 P0 -> P1 (向右? 不，通常 Start是端点，Ref是内侧点。所以头部是 P0->P1，如果P0在左，则是向右)
            // 等等，之前的逻辑是：
            // Head: pStart=P0, pRef=P1. 向量 P0->P1.
            // Tail: pStart=Pm, pRef=Pm-1. 向量 Pm->Pm-1.
            // 这两个向量都是指向“图形内部”的。

            double dx = pStart.X - pRef.X; // 这是一个指向外部的向量
            double dy = pStart.Y - pRef.Y;

            double unitX = dx / dist;
            double unitY = dy / dist;

            // 2. 基础偏移量
            double widthOffset = 3.5 * zoom;
            double lengthOffset = len * zoom;

            // 3. 计算基础法线 (旋转90度)
            // 这里的数学原理：向量(x, y) 旋转90度 -> (-y, x)
            double perpX = -unitY * widthOffset;
            double perpY = unitX * widthOffset;

            // 4. 判断挤压方向 (向上/向下)
            // 类型：1=UP, 2=DOWN, 3=OPEN UP, 4=OPEN DOWN
            bool isTypeUp = (type == 1 || type == 3);

            // 5. 核心修正逻辑：
            // 头部和尾部的向量方向相反，所以翻转逻辑互斥
            bool needInvert = false;

            if (isHead)
            {
                // 对于头部，计算出的基准法线默认是“向上”的
                // 如果用户选的是“向下”(Down)，我们需要翻转
                if (!isTypeUp) needInvert = true;
            }
            else
            {
                // 对于尾部，计算出的基准法线默认是“向下”的
                // 如果用户选的是“向上”(Up)，我们需要翻转
                if (isTypeUp) needInvert = true;
            }

            // 执行翻转
            if (needInvert)
            {
                perpX = -perpX;
                perpY = -perpY;
            }

            // 6. 计算坐标并绘制
            float p0x = pStart.X + (float)perpX;
            float p0y = pStart.Y + (float)perpY;

            // 挤压的尾巴向“内”延伸，所以用 pRef 方向的向量 (即 -unitX)
            // 注意：这里要沿着线段反方向回去
            // unitX 是 pStart - pRef (指向外)。所以往里走要减去
            float p1x = p0x - (float)(unitX * lengthOffset);
            float p1y = p0y - (float)(unitY * lengthOffset);

            g.DrawLine(p, p0x, p0y, p1x, p1y);       // 画平行线
            g.DrawLine(p, p0x, p0y, pStart.X, pStart.Y); // 画连接头部的短竖线
        }
        public void ConnLine(Graphics g)
        {
            // Length 线 (绿色, 实线)
            using (Pen myPenLength = new Pen(Color.FromArgb(150, 144, 238, 144), 2))
            // Angle 线 (橙色, 虚线 - 模仿图示中心线)
            using (Pen myPenAngle = new Pen(Color.FromArgb(200, 255, 160, 122), 1))
            {
                //myPenAngle.DashStyle = DashStyle.Dash; // 设置为虚线

                foreach (Control c in panel1.Controls)
                {
                    if (c is TextBox tb && tb.Visible)
                    {
                        // 获取 TextBox 中心
                        Point center = new Point(c.Left + c.Width / 2, c.Top + c.Height / 2);

                        if (tb.Name.StartsWith("txbLength"))
                        {
                            if (int.TryParse(tb.Name.Substring(9), out int idx) && idx < pxList_Zoom.Count - 1)
                            {
                                // 连到线段中点
                                PointF mid = new PointF(
                                    (pxList_Zoom[idx].X + pxList_Zoom[idx + 1].X) / 2,
                                    (pxList_Zoom[idx].Y + pxList_Zoom[idx + 1].Y) / 2
                                );
                                g.DrawLine(myPenLength, center.X, center.Y, mid.X, mid.Y);
                            }
                        }
                        else if (tb.Name.StartsWith("txbAngles"))
                        {
                            if (int.TryParse(tb.Name.Substring(9), out int idx) && idx + 1 < pxList_Zoom.Count)
                            {
                                // 连到顶点 (Vertex)
                                PointF pt = pxList_Zoom[idx + 1];
                                g.DrawLine(myPenAngle, center.X, center.Y, pt.X, pt.Y);
                            }
                        }
                    }
                }
            }
        }
        // ==========================================
        // 交互事件处理
        // ==========================================

        void textBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (sender is not TextBox txb)
                    return;
                if (TxbKeyEvent(txb))
                {
                    MarkProfileChanged();
                    RefreshAll();     // 刷新界面
                    FocusNextTextBox(txb.Name);
                }
                e.SuppressKeyPress = true; // 消除 "叮" 声
            }
            // controlVis((int)mf.db工作单子项); // 如果需要控制显示隐藏
        }

        void textBox_KeyUp(object? sender, KeyEventArgs e)
        {
            // 可以在这里处理实时校验，但尽量不要在这里重绘，性能开销大
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                // 1. 获取当前获得焦点的控件
                TextBox? currentTxb = null;
                foreach (Control c in panel1.Controls)
                {
                    if (c is TextBox txb && txb.Focused)
                    {
                        currentTxb = txb;
                        break;
                    }
                }

                if (currentTxb != null)
                {
                    // --- 【修复1：图形未更新问题】 ---
                    // 先保存当前输入的数据到数据模型 (lengAngle)
                    if (!TxbKeyEvent(currentTxb))
                        return true;

                    // 关键：必须调用全流程刷新方法！
                    // 这会重新计算 pxList (CalculatePhysicalPoints) 并重建所有 TextBox (reCreateTxb)
                    // 如果你不调用这个，图形不会变，且下面的 Find 操作找不到新位置的控件
                    MarkProfileChanged();
                    RefreshAll();

                    if (FocusNextTextBox(currentTxb.Name))
                        return true; // 拦截系统默认 Tab，防止焦点乱跑
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private bool FocusNextTextBox(string currentName)
        {
            string nextName = "";

            if (currentName.Length >= 11)
            {
                string prefix = currentName.Substring(0, 9);
                if (int.TryParse(currentName.Substring(9), out int currentIdx))
                {
                    string tryNextName = prefix + (currentIdx + 1).ToString("D2");
                    Control[] found = panel1.Controls.Find(tryNextName, true);

                    if (found.Length > 0)
                    {
                        nextName = tryNextName;
                    }
                    else if (prefix == "txbLength")
                    {
                        nextName = "txbAngles00";
                    }
                    else if (prefix == "txbAngles")
                    {
                        nextName = "txbLength00";
                    }
                }
            }

            if (!string.IsNullOrEmpty(nextName))
            {
                Control[] targets = panel1.Controls.Find(nextName, true);
                if (targets.Length > 0 && targets[0] is TextBox nextBox)
                {
                    SelTxb = nextBox;
                    nextBox.Focus();
                    nextBox.SelectAll();
                    PopCal();
                    return true;
                }

                string loopBackName = currentName.Substring(0, 9) + "00";
                Control[] loopTargets = panel1.Controls.Find(loopBackName, true);
                if (loopTargets.Length > 0 && loopTargets[0] is TextBox loopBox)
                {
                    SelTxb = loopBox;
                    loopBox.Focus();
                    loopBox.SelectAll();
                    PopCal();
                    return true;
                }
            }

            return false;
        }

        void textBox_GotFocus(object? sender, EventArgs e)
        {
            if (sender is not TextBox txb)
                return;

            if (suppressSelectAllOnFocus)
            {
                suppressSelectAllOnFocus = false;
                txb.Tag = false;
                double.TryParse(txb.Text, out oldVal);
                return;
            }

            txb.Tag = true;
            txb.SelectAll();
            double.TryParse(txb.Text, out oldVal);
        }

        void textBox_MouseUp(object? sender, MouseEventArgs e)
        {
            if (sender is not TextBox txb)
                return;
            if (e.Button == MouseButtons.Left && txb.Tag is bool tag && tag)
            {
                txb.SelectAll();
            }
            txb.Tag = false;
        }

        void textBox_Click(object? sender, EventArgs e)
        {
            if (sender is not TextBox clickedTextBox)
                return;

            SelTxb = clickedTextBox;
            double.TryParse(SelTxb.Text, out oldVal);

            pnl左工具栏1.Visible = false;
            pnl左工具栏2.Visible = true;
            pnl左工具栏2.Location = new Point(2, 101);

            if (SelTxb.Name.StartsWith("txbLength"))
            {
                pnl挤压功能选.Visible = false;
                pnl长度功能选.Visible = true;
                pnl角度功能选.Visible = false;
            }
            else if (SelTxb.Name.StartsWith("txbAngles"))
            {
                pnl挤压功能选.Visible = false;
                pnl长度功能选.Visible = false;
                pnl角度功能选.Visible = true;
            }

            PopCal();
        }

        private void PopCal()
        {
            if (dlgCal == null || dlgCal.IsDisposed)
            {
                dlgCal = new FrmCalculator(this);
                dlgCal.SetEmbeddedMode(true);
                dlgCal.StartPosition = FormStartPosition.Manual;
                dlgCal.Location = new Point(CalculatorLeft, CalculatorTop);
                dlgCal.Visible = false;
                pnl左工具栏2.Controls.Add(dlgCal);
            }

            dlgCal.Location = new Point(CalculatorLeft, CalculatorTop);
            dlgCal.Show();
            dlgCal.BringToFront();
        }

        public void sendkey(string input)
        {
            if (SelTxb == null || SelTxb.IsDisposed)
                return;

            suppressSelectAllOnFocus = true;
            SelTxb.Focus();

            if (input == "ENTER")
            {
                string currentName = SelTxb.Name;
                if (!TxbKeyEvent(SelTxb))
                    return;

                MarkProfileChanged();
                if (SelTxb.Name.StartsWith("txbTaperW"))
                    RefreshViewOnly();
                else
                    RefreshAll();

                FocusNextTextBox(currentName);
                return;
            }

            if (input == "CLR")
            {
                SelTxb.Text = "0";
                SelTxb.SelectionStart = SelTxb.TextLength;
                return;
            }

            string currentText = SelTxb.Text ?? string.Empty;
            int selectionStart = SelTxb.SelectionStart;
            int selectionLength = SelTxb.SelectionLength;

            if (selectionLength > 0)
            {
                currentText = currentText.Remove(selectionStart, selectionLength);
            }

            if (input == ".")
            {
                if (currentText.Contains('.'))
                    return;

                if (string.IsNullOrEmpty(currentText) || currentText == "-")
                    currentText += "0";
            }
            else if (input == "-")
            {
                if (string.IsNullOrEmpty(currentText))
                {
                    SelTxb.Text = "-";
                    SelTxb.SelectionStart = SelTxb.TextLength;
                    SelTxb.SelectionLength = 0;
                    return;
                }

                currentText = currentText.StartsWith("-")
                    ? currentText.Substring(1)
                    : "-" + currentText;
                SelTxb.Text = currentText;
                SelTxb.SelectionStart = SelTxb.TextLength;
                SelTxb.SelectionLength = 0;
                return;
            }

            currentText = currentText.Insert(selectionStart, input);
            SelTxb.Text = currentText;
            SelTxb.SelectionStart = Math.Min(selectionStart + input.Length, SelTxb.TextLength);
            SelTxb.SelectionLength = 0;
        }

        private bool TryShowInputError(TextBox txb, string message)
        {
            MessageBox.Show(message);
            txb.Focus();
            txb.SelectAll();
            return false;
        }

        private bool ApplySquashLength()
        {
            if (!MainFrm.TryParseDisplayLength(txb挤压长度.Text, out double lengthMm))
                return TryShowInputError(txb挤压长度, Strings.Get("Auto.Error.InvalidNumber"));

            if (lengthMm < 0 || lengthMm > 1250)
                return TryShowInputError(txb挤压长度, Strings.Get("Auto.Error.LengthRange"));

            int idx = (selSquash == 0) ? 0 : 99;
            MainFrm.CurtOrder.lengAngle[idx].Length = Math.Round(lengthMm, 1, MidpointRounding.AwayFromZero);
            MainFrm.CurtOrder.lengAngle[idx].TaperWidth = MainFrm.CurtOrder.lengAngle[idx].Length;
            txb挤压长度.Text = MainFrm.FormatDisplayLength(MainFrm.CurtOrder.lengAngle[idx].Length);
            oldVal = MainFrm.CurtOrder.lengAngle[idx].Length;
            return true;
        }

        bool TxbKeyEvent(TextBox txb)
        {
            if (txb == txb挤压长度 || txb.Name == "txb挤压长度")
                return ApplySquashLength();

            // 1. 数据有效性检查：如果输入为空或非数字，直接返回，不做处理
            if (string.IsNullOrWhiteSpace(txb.Text) || !double.TryParse(txb.Text, out double val))
                return TryShowInputError(txb, Strings.Get("Auto.Error.InvalidNumber"));

            // 2. 解析控件名称，提取前缀和索引
            // 假设命名格式严格为：前缀(9字符) + 索引(2字符)，如 "txbLength01"
            if (txb.Name.Length < 11) return false;

            string prefix = txb.Name.Substring(0, 9);
            string indexStr = txb.Name.Substring(9);

            if (!int.TryParse(indexStr, out int i)) return false; // 无法解析索引则退出

            // 3. 获取数据引用以便操作
            var dataList = MainFrm.CurtOrder.lengAngle;
            int maxCount = dataList.Length;

            // 4. 根据类型更新数据 (带越界检查)
            if (prefix == "txbLength")
            {
                if (!MainFrm.TryParseDisplayLength(txb.Text, out double lengthMm))
                    return TryShowInputError(txb, Strings.Get("Auto.Error.InvalidNumber"));

                // Length 对应索引 i + 1
                if (i + 1 < maxCount)
                {
                    if (lengthMm < 0 || lengthMm > 1250)
                    {
                        return TryShowInputError(txb, Strings.Get("Auto.Error.LengthRange"));
                    }

                    dataList[i + 1].Length = Math.Round(lengthMm, 1, MidpointRounding.AwayFromZero);
                    txb.Text = MainFrm.FormatDisplayLength(dataList[i + 1].Length);

                    // 可选：通常修改长度时，如果未启用锥度，锥度宽应同步等于长度
                    // if (!MainFrm.CurtOrder.isTaper) 
                    //     dataList[i + 1].TaperWidth = dataList[i + 1].Length;
                }
            }
            else if (prefix == "txbAngles")
            {
                // Angle 对应索引 i + 2
                if (i + 2 < maxCount)
                {
                    // --- 符号锁定逻辑 ---
                    // 如果旧值为负且新输入为正，或者旧值为正且新输入为负，强制翻转新值符号
                    // 目的：防止用户仅仅想修改角度大小时意外改变了折弯方向
                    // (前提是 oldVal 在 GotFocus 时已被正确赋值)
                    if ((oldVal < 0 && val > 0) || (oldVal > 0 && val < 0))
                    {
                        val = -val;
                    }

                    if (val < -180 || val > 180)
                    {
                        return TryShowInputError(txb, Strings.Get("Auto.Error.AngleRange"));
                    }

                    dataList[i + 2].Angle = Math.Round(val, 1, MidpointRounding.AwayFromZero);
                }
            }
            else if (prefix == "txbTaperW")
            {
                if (!MainFrm.TryParseDisplayLength(txb.Text, out double taperWidthMm))
                    return TryShowInputError(txb, Strings.Get("Auto.Error.InvalidNumber"));

                // TaperWidth 对应索引 i + 1
                if (i + 1 < maxCount)
                {
                    dataList[i + 1].TaperWidth = Math.Round(taperWidthMm, 1, MidpointRounding.AwayFromZero);
                    txb.Text = MainFrm.FormatDisplayLength(dataList[i + 1].TaperWidth);
                }
            }

            // 5. 更新 oldVal
            // 这一步很重要：如果用户按了 Enter 更新了值，但焦点没有离开，
            // 再次修改时 oldVal 应该是刚才更新过的值。
            oldVal = val;
            return true;
        }

        private int _currentVisMode = 0;

        public void controlVis(int vis)   //0:角度+尺寸  1:尺寸(隐藏角度)  2:角度(隐藏尺寸)  3：都不显示
        {
            _currentVisMode = vis; // 记录当前模式

            panel1.SuspendLayout(); // 挂起布局，避免闪烁

            Control.ControlCollection pnlControls = panel1.Controls;
            foreach (Control control in pnlControls)
            {
                if (control is TextBox)
                {
                    bool shouldShow = true;

                    // 逻辑判断：根据 vis 的值决定是否隐藏
                    if (vis == 1 && control.Name.StartsWith("txbAngles")) // 模式1：隐藏角度
                    {
                        shouldShow = false;
                    }
                    else if (vis == 2 && control.Name.StartsWith("txbLength")) // 模式2：隐藏尺寸
                    {
                        shouldShow = false;
                    }
                    else if (vis == 3) // 模式3：都不显示（比如显示比例时）
                    {
                        shouldShow = false;
                    }

                    // 设置控件可见性
                    control.Visible = shouldShow;
                }
            }

            panel1.ResumeLayout();

            // 关键：触发重绘，这将调用 panel1_Paint -> ConnLine
            panel1.Invalidate();
        }
        // ==========================================
        // 按钮功能区 (插入/删除/倒角//圆弧 
        // ==========================================

        private void btn插入角度_Click(object sender, EventArgs e)
        {
            if (SelTxb == null || !SelTxb.Name.StartsWith("txbLength")) return;

            int i = Convert.ToInt32(SelTxb.Name.Substring(9, 2)) + 1;
            LengAngle[] oldArr = MainFrm.CurtOrder.lengAngle;
            LengAngle[] newArray = new LengAngle[oldArr.Length];

            // 复制前半部分
            Array.Copy(oldArr, 0, newArray, 0, i + 1);

            // 修改当前段
            newArray[i].Length /= 2;
            newArray[i].TaperWidth /= 2;

            // 插入新段
            newArray[i + 1] = new LengAngle
            {
                Length = oldArr[i].Length / 2,
                Angle = 180,
                TaperWidth = oldArr[i].TaperWidth / 2
            };

            // 复制后半部分 (错位)
            for (int j = i + 1; j < 98; j++)
            {
                newArray[j + 1] = oldArr[j];
            }
            newArray[99] = oldArr[99];

            MainFrm.CurtOrder.lengAngle = newArray;
            MarkProfileChanged();
            RefreshAll();
        }

        private void btn删除角度_Click(object sender, EventArgs e)
        {
            if (SelTxb == null || !SelTxb.Name.StartsWith("txbAngles")) return;

            int i = Convert.ToInt32(SelTxb.Name.Substring(9, 2)) + 2;
            if (i <= 0 || i >= 98) return;

            // 合并长度
            MainFrm.CurtOrder.lengAngle[i - 1].Length += MainFrm.CurtOrder.lengAngle[i].Length;

            LengAngle[] oldArr = MainFrm.CurtOrder.lengAngle;
            LengAngle[] newArray = new LengAngle[oldArr.Length];

            // 复制前半部分
            Array.Copy(oldArr, 0, newArray, 0, i);

            // 移动后半部分
            for (int j = i + 1; j < 99; j++)
            {
                newArray[j - 1] = oldArr[j];
            }

            newArray[98] = new LengAngle();
            newArray[99] = oldArr[99]; // 保持尾部

            MainFrm.CurtOrder.lengAngle = newArray;
            MarkProfileChanged();
            RefreshAll();
        }

        private void btn角度换向_Click(object sender, EventArgs e)
        {
            if (SelTxb == null)
                return;

            int i = Convert.ToInt32(SelTxb.Name.Substring(9, 2));

            // 无需计算delta_A，直接对anglelength中的angle元素取反

            MainFrm.CurtOrder.lengAngle[i + 2].Angle *= -1;

            //重新绘制图形

            SelTxb = ((TextBox)this.Controls.Find(SelTxb.Name, true)[0]);

            MarkProfileChanged();
            RefreshAll();
        }
  
        private bool open放射角 = false;
        private void btn放射角_Click(object sender, EventArgs e)
        {
            open放射角 = !open放射角;
            btn放射角.BackgroundImage = open放射角 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            btn放射角.ForeColor = open放射角 ? Color.FromArgb(96, 176, 255) : Color.White;

            if (open放射角)
            {
                txb半径.Visible = txb分段数.Visible = true;
                lab10.Visible = lab11.Visible = lab12.Visible = true;

            }
            else
            {
                txb半径.Visible = txb分段数.Visible = false;
                lab10.Visible = lab11.Visible = lab12.Visible = false;
            }

        }

        private void txb半径_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == (char)Keys.Enter)
            {
                AddAngle();

            }

            RefreshAll();

        }

        private void txb分段数_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                AddAngle();

            }
            RefreshAll();
        }

        public void AddAngle()
        {
            if (SelTxb == null || !SelTxb.Name.StartsWith("txbAngles")) return;
            int i = Convert.ToInt32(SelTxb.Name.Substring(9, 2)) + 2;

            double a = MainFrm.CurtOrder.lengAngle[i].Angle;
            double r = 0; double.TryParse(txb半径.Text, out r);
            int n = 0; int.TryParse(txb分段数.Text, out n);

            if (n <= 0 || r <= 0) return;

            var result = CalculateFilletParameters(a, r, n);
            double a1 = result.angle;
            double r1 = result.chordLength;
            double b = result.apLength;

            // 修正前后段长度
            MainFrm.CurtOrder.lengAngle[i - 1].Length = Math.Max(0, MainFrm.CurtOrder.lengAngle[i - 1].Length - b + r1 / 2);
            MainFrm.CurtOrder.lengAngle[i].Length = Math.Max(0, MainFrm.CurtOrder.lengAngle[i].Length - b + r1 / 2);

            LengAngle[] oldArr = MainFrm.CurtOrder.lengAngle;
            LengAngle[] newArray = new LengAngle[oldArr.Length];

            // 1. 复制直到 i
            Array.Copy(oldArr, 0, newArray, 0, i); // 复制 0 到 i-1

            // 2. 插入 n 个分段
            // 注意：原逻辑有点混淆，这里假设是把角i替换成n个小角
            // 简单处理：向后移动数组腾出空间
            // 这是一个复杂的数组操作，原逻辑似乎有覆盖风险，这里保持原逻辑意图但增加安全性

            // 暂保持原逻辑结构：
            // 复制后面部分
            for (int j = i; j < 99 - n; j++)
            {
                newArray[j - 1 + n] = oldArr[j];
            }

            // 填充中间圆弧段
            for (int j = i; j < i + n - 1; j++)
            {
                newArray[j] = new LengAngle { Angle = a1, Length = r1, TaperWidth = r1 };
            }
            // 最后一个圆弧段
            if (i + n - 1 < 99)
                newArray[i + n - 1] = new LengAngle { Angle = a1, Length = newArray[i + n - 1].Length, TaperWidth = newArray[i + n - 1].TaperWidth }; // 这里可能需要视具体业务逻辑调整

            newArray[99] = oldArr[99];
            MainFrm.CurtOrder.lengAngle = newArray;

            MarkProfileChanged();
            RefreshAll();
        }

        // ==========================================
        // 其他基础事件与工具方法
        // ==========================================

        private void panel1_Click(object sender, EventArgs e)
        {
            pnl左工具栏1.Visible = true;
            pnl左工具栏1.Location = new Point(2, 101);
            pnl左工具栏2.Visible = false;
        }

        private void txb挤压长度_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (ApplySquashLength())
                {
                    MarkProfileChanged();
                    RefreshAll();
                }
                e.SuppressKeyPress = true;
            }
        }

        private void txb挤压长度_Click(object? sender, EventArgs e)
        {
            SelectSquashLengthTextBox();
        }

        private void SelectSquashLengthTextBox()
        {
            SelTxb = txb挤压长度;
            double.TryParse(txb挤压长度.Text, out oldVal);

            pnl左工具栏1.Visible = false;
            pnl左工具栏2.Visible = true;
            pnl左工具栏2.Location = new Point(2, 101);

            pnl挤压功能选.Visible = true;
            pnl长度功能选.Visible = false;
            pnl角度功能选.Visible = false;

            txb挤压长度.Focus();
            txb挤压长度.SelectAll();
            PopCal();
        }

        private void cbx挤压类型_SelectedIndexChanged(object sender, EventArgs e)
        {
            int type = cbx挤压类型.SelectedIndex;
            int idx = (selSquash == 0) ? 0 : 99;

            MainFrm.CurtOrder.lengAngle[idx].Angle = type;

            double len = MainFrm.CurtOrder.lengAngle[idx].Length;
            if (len == 0)
            {
                len = 15;
                MainFrm.CurtOrder.lengAngle[idx].Length = 15;
            }
            txb挤压长度.Text = MainFrm.FormatDisplayLength(len);

            MarkProfileChanged();
            RefreshAll();
        }

        int selSquash = 0;
        void myButton_Click(object? sender, EventArgs e)
        {
            pnl左工具栏1.Visible = false;
            pnl左工具栏2.Visible = true;
            pnl左工具栏2.Location = new Point(2, 101);

            pnl挤压功能选.Visible = true;
            pnl长度功能选.Visible = false;
            pnl角度功能选.Visible = false;

            if (sender is not Button btn)
                return;

            int idx = (btn.Name == "btnSquash0") ? 0 : 99;
            selSquash = idx;

            int type = (int)MainFrm.CurtOrder.lengAngle[idx].Angle;
            if (type >= 0 && type < cbx挤压类型.Items.Count)
                cbx挤压类型.SelectedIndex = type;

            double len = MainFrm.CurtOrder.lengAngle[idx].Length;
            txb挤压长度.Text = MainFrm.FormatDisplayLength(len == 0 ? 15 : len);
            SelectSquashLengthTextBox();
        }

        // 保持原有的语言设置和Helper方法
        private void setLang()
        {
            LocalizationManager.ApplyResources(this);
            if (MainFrm.Lang == 0)
            {
                lb继续.Font = lb步骤.Font = lb分条开.Font = lb分条关.Font = label21.Font = label22.Font = label7.Font = label8.Font = label14.Font =
                label25.Font = label26.Font = label11.Font = label9.Font = label15.Font = label16.Font = new System.Drawing.Font("宋体", 11.25F);
                label2.Font = label3.Font = label4.Font = label5.Font = label6.Font = label17.Font = label12.Font = label19.Font = label1.Font = new System.Drawing.Font("微软雅黑", 12F);
                btn装载材料.Font = btn重置计数.Font = btn插入角度.Font = btn删除角度.Font = button3.Font = new System.Drawing.Font("宋体", 11.25F);
                lb颜色面.Font = lb正逆序.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            }
            else
            {
                lb继续.Font = lb步骤.Font = lb分条开.Font = lb分条关.Font = label21.Font = label22.Font = label7.Font = label8.Font = label14.Font =
                label25.Font = label26.Font = label11.Font = label9.Font = label15.Font = label16.Font = new System.Drawing.Font("Calibri", 11.25F);
                label2.Font = label3.Font = label4.Font = label5.Font = label6.Font = label17.Font = label12.Font = label19.Font = label1.Font = new System.Drawing.Font("Calibri", 12F);
                btn装载材料.Font = btn重置计数.Font = btn插入角度.Font = btn删除角度.Font = button3.Font = new System.Drawing.Font("Calibri", 11.25F);
                lb颜色面.Font = lb正逆序.Font = new System.Drawing.Font("Calibri", 10.5F);
            }
            lb继续.Text = Strings.Get("Auto.Toggle.Continue");
            lb步骤.Text = Strings.Get("Auto.Toggle.Step");
            lb分条开.Text = Strings.Get("Auto.Toggle.Slit");
            lb分条关.Text = Strings.Get("Auto.Toggle.Off");
            label21.Text = Strings.Get("Auto.Label.TopSpringback");
            label22.Text = Strings.Get("Auto.Label.BottomSpringback");
            label7.Text = Strings.Get("Auto.Label.CalculatedWidth");
            label14.Text = Strings.Get("Auto.Label.FoldSequence");
            label25.Text = Strings.Get("Auto.Label.FeedColorSide");
            label26.Text = Strings.Get("Auto.Label.Count");
            label2.Text = Strings.Get("Auto.Label.JobName");
            label3.Text = Strings.Get("Auto.Label.Customer");
            label4.Text = Strings.Get("Auto.Label.Material");
            label5.Text = Strings.Get("Auto.Label.Length");
            label6.Text = Strings.Get("Auto.Label.Thickness");
            label17.Text = Strings.Get("Auto.Label.Notes");
            label12.Text = Strings.Get("Auto.Label.SquashLength");
            label19.Text = Strings.Get("Auto.Label.SquashFoldType");
            label1.Text = Strings.Get("Auto.Label.RadialFold");
            btn装载材料.Text = Strings.Get("Auto.Action.LoadMaterial");
            btn重置计数.Text = Strings.Get("Auto.Action.ResetCount");
            btn插入角度.Text = Strings.Get("Auto.Action.InsertAngle");
            btn删除角度.Text = Strings.Get("Auto.Action.DeleteAngle");
            string unitText = MainFrm.GetLengthUnitLabel();
            label8.Text = label11.Text = label15.Text = label16.Text = unitText;
            RefreshInlineSlitButton();
        }

        private void CalLength(bool isCal)
        {
            if (isCal)
            {
                RefreshAll();
                return;
            }

            InitializeDisplayHeadingFromCurrentProfile();
            MainFrm.CurtOrder.Width = MainFrm.CalculateOrderWidth(MainFrm.CurtOrder.lengAngle);
            RefreshViewOnly();
        }

        public static (double chordLength, double angle, double apLength) CalculateFilletParameters(double a, double r, int n)
        {
            double absA = Math.Abs(a);
            double centralAngle = 180 - absA;
            double segmentAngleDeg = centralAngle / n;
            double halfAngleRad = (segmentAngleDeg / 2) * (Math.PI / 180);
            double chordLength = 2 * r * Math.Sin(halfAngleRad);
            double halfAngleApRad = (absA / 2) * (Math.PI / 180);
            double apLength = r / Math.Tan(halfAngleApRad);
            double calculatedAngle = 180 - segmentAngleDeg;
            if (a < 0) calculatedAngle = -calculatedAngle;
            return (chordLength, calculatedAngle, apLength);
        }

        // 占位符，保持其他Timer和Switch事件编译通过
        private void timer1_Tick(object sender, EventArgs e)
        {
            pnlAuto.BackgroundImage = MainFrm.Hmi_iArray[20] == 6
                ? (MainFrm.Lang == 0 ? global::JSZW1000A.Properties.Resources.AutoStart : global::JSZW1000A.Properties.Resources.AutoStart1)
                : (MainFrm.Lang == 0 ? global::JSZW1000A.Properties.Resources.AutoOrig1_zh_CHS : global::JSZW1000A.Properties.Resources.AutoOrig1);

            sw继续步骤.Image = MainFrm.Hmi_bArray[71] ? global::JSZW1000A.Properties.Resources.btm_2档开关1 : global::JSZW1000A.Properties.Resources.btm_2档开关0;
            lb继续.ForeColor = MainFrm.Hmi_bArray[71] ? Color.FromArgb(96, 176, 255) : Color.White;
            lb步骤.ForeColor = !MainFrm.Hmi_bArray[71] ? Color.FromArgb(96, 176, 255) : Color.White;

            sw分条开关.Image = MainFrm.Hmi_bArray[62] ? global::JSZW1000A.Properties.Resources.btm_分条开关1 : global::JSZW1000A.Properties.Resources.btm_分条开关0;
            lb分条开.ForeColor = MainFrm.Hmi_bArray[62] ? Color.FromArgb(96, 176, 255) : Color.White;
            lb分条关.ForeColor = !MainFrm.Hmi_bArray[62] ? Color.FromArgb(96, 176, 255) : Color.White;

            MainFrm.CurtOrder.Width = MainFrm.CalculateOrderWidth(MainFrm.CurtOrder.lengAngle);

            txb计算总宽.Text = MainFrm.FormatDisplayLength(MainFrm.CurtOrder.Width);
            MainFrm.CurtOrder.isSlitter = MainFrm.Hmi_bArray[62];

            MainFrm.CurtOrder.BtmSpring = Convert.ToDouble(txbSpringBtm.Text);
            MainFrm.CurtOrder.TopSpring = Convert.ToDouble(txbSpringTop.Text);

            MainFrm.CurtOrder.Customer = txb客户.Text;
            MainFrm.CurtOrder.MaterialName = txb材料.Text;
            bool succ0 = MainFrm.TryParseDisplayLength(txb长度.Text, out double sheetLengthMm);
            bool succ1 = MainFrm.TryParseDisplayLength(txb厚度.Text, out double thicknessMm);
            if (succ0)
                MainFrm.CurtOrder.SheetLength = sheetLengthMm;
            if (succ1)
                MainFrm.CurtOrder.Thickness = thicknessMm;
            MainFrm.CurtOrder.Remark = txb备注.Text;

            sw正逆序.BackgroundImage = MainFrm.CurtOrder.st逆序 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            lb正逆序.Text = LocalizationText.OrderDirection(MainFrm.CurtOrder.st逆序);
            lb正逆序.ForeColor = MainFrm.CurtOrder.st逆序 ? Color.FromArgb(96, 176, 255) : Color.White;
            sw颜色面.BackgroundImage = MainFrm.CurtOrder.st色下 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            lb颜色面.Text = LocalizationText.ColorSide(MainFrm.CurtOrder.st色下);
            lb颜色面.ForeColor = MainFrm.CurtOrder.st色下 ? Color.FromArgb(96, 176, 255) : Color.White;
            RefreshInlineSlitButton();
        }

        public bool b正逆序 = false;
        private void sw正逆序_Click(object sender, EventArgs e)
        {
            MainFrm.CurtOrder.st逆序 = !MainFrm.CurtOrder.st逆序;
            sw正逆序.BackgroundImage = MainFrm.CurtOrder.st逆序 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            lb正逆序.Text = LocalizationText.OrderDirection(MainFrm.CurtOrder.st逆序);
            lb正逆序.ForeColor = MainFrm.CurtOrder.st逆序 ? Color.FromArgb(96, 176, 255) : Color.White;
            MarkProfileChanged();
        }

        private void sw颜色面_Click(object sender, EventArgs e)
        {
            MainFrm.CurtOrder.st色下 = !MainFrm.CurtOrder.st色下;
            sw颜色面.BackgroundImage = MainFrm.CurtOrder.st色下 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            lb颜色面.Text = LocalizationText.ColorSide(MainFrm.CurtOrder.st色下);
            lb颜色面.ForeColor = MainFrm.CurtOrder.st色下 ? Color.FromArgb(96, 176, 255) : Color.White;
            MarkProfileChanged();
        }

        private void sw分条开关_Click(object sender, EventArgs e)
        {
            string s0 = "";
            if (!MainFrm.Hmi_bArray[62])
                s0 = Strings.Get("Common.ConfirmEnableSlitter");

            DialogAsk dlgTips = new DialogAsk("", s0);
            dlgTips.StartPosition = FormStartPosition.Manual;
            dlgTips.Location = new Point(500, 200);
            if ((!MainFrm.Hmi_bArray[62] && dlgTips.ShowDialog() == DialogResult.OK) || MainFrm.Hmi_bArray[62])
            {
                MainFrm.Hmi_bArray[62] = !MainFrm.Hmi_bArray[62];
                MainFrm.Hmi_bArray[72] = MainFrm.Hmi_bArray[62];
                sw分条开关.Image = MainFrm.Hmi_bArray[62] ? global::JSZW1000A.Properties.Resources.btm_分条开关1 : global::JSZW1000A.Properties.Resources.btm_分条开关0;
                mf.AdsWritePlc1Bit(62, MainFrm.Hmi_bArray[62]);
            }
        }
        private void sw继续步骤_Click(object sender, EventArgs e) { /* 保持原逻辑 */ }
        private void txbSpringTop_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                mf.gblSpringSt(Convert.ToSingle(txbSpringTop.Text), Convert.ToSingle(txbSpringBtm.Text));
            }
        }
        private void btn装载材料_MouseDown(object sender, MouseEventArgs e) { mf.gbl装载材料MouseUp(); }
        private void btn装载材料_MouseUp(object sender, MouseEventArgs e) { mf.gbl装载材料MouseDown(); }
        private void btn重置计数_Click(object sender, EventArgs e)
        {
            Hmi_iArray[62] = 0;
        }
        private void pnlAuto_Click(object sender, EventArgs e) { mf.gbl开始自动Click(MainFrm.Hmi_bArray[62], true); }
        private void pnlAuto_MouseDown(object sender, MouseEventArgs e) { mf.gbl开始自动MouseDown(); }
        private void pnlAuto_MouseUp(object sender, MouseEventArgs e) { mf.gbl开始自动MouseUp(); }

        private void button3_Click(object sender, EventArgs e)
        {
            if (MainFrm.CurtOrder.Width <= 0)
            {
                MessageBox.Show(
                    Strings.Get("Auto.Error.ProfileRequired"),
                    Strings.Get("Common.TipsTitle"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            FrmInlineSlit dlgInline = new FrmInlineSlit(
                MainFrm.CurtOrder.Width,
                MainFrm.CurtOrder.边做边分切启用,
                MainFrm.CurtOrder.边做边分切整板宽,
                MainFrm.CurtOrder.边做边分切块数);
            dlgInline.StartPosition = FormStartPosition.Manual;
            dlgInline.Location = new Point(420, 180);
            if (dlgInline.ShowDialog() != DialogResult.OK)
                return;

            MainFrm.CurtOrder.边做边分切启用 = dlgInline.InlineEnabled;
            MainFrm.CurtOrder.边做边分切整板宽 = dlgInline.TotalWidth;
            MainFrm.CurtOrder.边做边分切块数 = dlgInline.PieceCount;
            MainFrm.CurtOrder.边做边分切前废料 = Math.Max(0, dlgInline.TotalWidth - MainFrm.CurtOrder.Width * dlgInline.PieceCount);
            if (!dlgInline.InlineEnabled)
            {
                MainFrm.CurtOrder.边做边分切整板宽 = 0;
                MainFrm.CurtOrder.边做边分切块数 = 0;
                MainFrm.CurtOrder.边做边分切前废料 = 0;
            }

            if (dlgInline.InlineEnabled && !MainFrm.Hmi_bArray[62])
            {
                MainFrm.Hmi_bArray[62] = true;
                MainFrm.Hmi_bArray[72] = true;
                mf.AdsWritePlc1Bit(62, MainFrm.Hmi_bArray[62]);
            }

            MarkProfileChanged();
            RefreshInlineSlitButton();
        }




    }
}
