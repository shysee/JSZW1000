using System.Drawing.Drawing2D;
using System.Reflection; // 记得引用这个

namespace JSZW400.SubWindows
{
    public partial class SubOPAutoDraw : UserControl
    {
        bool bAlignGrid = true;
        int GridSize = 20;
        int GridToLength = 20;
        private Point p1; // 只需要一个点变量用于临时存储
        MainFrm mf;

        public SubOPAutoDraw(MainFrm fm1)
        {
            InitializeComponent();
            setLang();
            this.mf = fm1;

            // 开启 UserControl 的双缓冲
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            this.UpdateStyles();

            // 强制开启 panel2 的双缓冲 (解决画图闪烁和黑屏的关键)
            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, panel2, new object[] { true });
        }

        private void SubOPAuto2_Load(object sender, EventArgs e)
        {
            MainFrm.QuickDrawList.Clear();
            panel3.Visible = false;
            timer1.Start();
        }

        private void setLang()
        {
            // ... (语言设置代码保持不变) ...
            if (MainFrm.Lang == 0)
            {
                label15.Font = label2.Font = label3.Font = lb网格状态.Font = label4.Font = label5.Font = lb分条开.Font = label6.Font = new System.Drawing.Font("宋体", 11.25F);
            }
            else
            {
                label15.Font = label2.Font = label3.Font = lb网格状态.Font = label4.Font = label5.Font = lb分条开.Font = label6.Font = new System.Drawing.Font("Calibri", 11.25F);
            }

            label15.Text = (MainFrm.Lang == 0) ? "对齐网格" : "Snap to Grid";
            lb网格状态.Text = (MainFrm.Lang == 0) ? "关" : "Off";
            label2.Text = (MainFrm.Lang == 0) ? "网格单元\r\n长度单位" : "Grid Unit\r\nLength";
            label3.Text = (MainFrm.Lang == 0) ? "毫米" : "mm";
            label4.Text = (MainFrm.Lang == 0) ? "精细" : "FINE";
            lb分条开.Text = (MainFrm.Lang == 0) ? "合适" : "MEDIUM";
            label5.Text = (MainFrm.Lang == 0) ? "粗糙" : "COARSE";
            label6.Text = (MainFrm.Lang == 0) ? "网格大小" : "Grid Size";
            btn清除.Text = (MainFrm.Lang == 0) ? " 清除" : " Clear";
            btn撤销.Text = (MainFrm.Lang == 0) ? " 撤销" : " Undo";
        }

        // ==========================================
        // 核心绘图逻辑 (Paint 事件)
        // ==========================================
        // 确保事件名称对应，建议改名为 panel2_Paint 以免混淆
        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            // 直接使用事件参数提供的 Graphics，这是 panel2 的画布
            redrawGraphics(e.Graphics);
        }

        void redrawGraphics(Graphics g1)
        {
            try
            {
                g1.SmoothingMode = SmoothingMode.AntiAlias;
                g1.CompositingQuality = CompositingQuality.HighQuality;

                // 1. 填充背景色 (确保这里的颜色和您设计器里 panel2 的 BackColor 一致或覆盖它)
                g1.Clear(Color.FromArgb(33, 40, 48));

                // 2. 绘制网格
                using (Pen gridPen = new Pen(Color.FromArgb(100, 100, 100), 1))
                {
                    if (GridSize <= 0) GridSize = 20;

                    // 使用 panel2.Width 和 panel2.Height 确保网格铺满控件
                    for (int i = 0; i <= panel2.Height; i += GridSize)
                    {
                        g1.DrawLine(gridPen, 0, i, panel2.Width, i);
                    }
                    for (int j = 0; j <= panel2.Width; j += GridSize)
                    {
                        g1.DrawLine(gridPen, j, 0, j, panel2.Height);
                    }
                }

                // 3. 绘制线条和点
                if (MainFrm.QuickDrawList.Count > 0)
                {
                    using (Pen pointPen = new Pen(Color.FromArgb(246, 203, 181), 2))
                    using (Pen linePen = new Pen(Color.FromArgb(119, 151, 217), 4))
                    {
                        // 画起点
                        Rectangle startRect = new Rectangle((int)(MainFrm.QuickDrawList[0].X - 4), (int)(MainFrm.QuickDrawList[0].Y - 4), 8, 8);
                        g1.DrawRectangle(pointPen, startRect);

                        for (int k = 1; k < MainFrm.QuickDrawList.Count; k++)
                        {
                            // 画连线
                            g1.DrawLine(linePen, MainFrm.QuickDrawList[k - 1], MainFrm.QuickDrawList[k]);
                            // 画节点
                            Rectangle rect = new Rectangle((int)(MainFrm.QuickDrawList[k].X - 4), (int)(MainFrm.QuickDrawList[k].Y - 4), 8, 8);
                            g1.DrawRectangle(pointPen, rect);
                        }
                    }
                }
            }
            catch (Exception) { }
        }
        // ==========================================
        // 鼠标交互逻辑
        // ==========================================
        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            Point newPoint = new Point();

            if (bAlignGrid)
            {
                // 吸附逻辑
                int modX = e.X % GridSize;
                int intX = (e.X / GridSize);
                newPoint.X = (modX > GridSize / 2) ? (intX + 1) * GridSize : intX * GridSize;

                int modY = e.Y % GridSize;
                int intY = (e.Y / GridSize);
                newPoint.Y = (modY > GridSize / 2) ? (intY + 1) * GridSize : intY * GridSize;
            }
            else
            {
                newPoint.X = e.X;
                newPoint.Y = e.Y;
            }

            // 添加点
            MainFrm.QuickDrawList.Add(newPoint);

            // 使用 Invalidate 触发 Paint，而不是 CreateGraphics
            panel2.Invalidate();
        }

        // ==========================================
        // 按钮功能 (清除/撤销)
        // ==========================================
        private void btn清除_Click(object sender, EventArgs e)
        {
            MainFrm.QuickDrawList.Clear();
            // 修正：OrderType是struct，不能与null比较。可用引用类型判断或直接调用clr()（假定CurtOrder已初始化）。
            MainFrm.CurtOrder.clr();

            // 只调用 Invalidate，不要 redrawGraphics
            panel2.Invalidate();
        }

        private void btn撤销_Click(object sender, EventArgs e)
        {
            if (MainFrm.QuickDrawList.Count > 0)
            {
                MainFrm.QuickDrawList.RemoveAt(MainFrm.QuickDrawList.Count - 1);
                panel2.Invalidate();
            }
        }

        // ==========================================
        // 对外公开的计算方法 (供 MainFrm 调用)
        // ==========================================

        public bool ExecuteCalculation()
        {
            if (MainFrm.QuickDrawList.Count < 2)
            {
                MessageBox.Show("请先绘制图形！");
                return false;
            }

            try
            {
                // 1. 初始化
                MainFrm.CurtOrder.pxList.Clear();
                if (MainFrm.CurtOrder.lengAngle == null) MainFrm.CurtOrder.lengAngle = new MainFrm.LengAngle[100];

                for (int i = 0; i < 100; i++)
                {
                    MainFrm.CurtOrder.lengAngle[i] = new MainFrm.LengAngle();
                }

                foreach (var p in MainFrm.QuickDrawList)
                {
                    MainFrm.CurtOrder.pxList.Add(new PointF(p.X, p.Y));
                }

                // 2. 计算长度和绝对角度
                List<double> absAngles = new List<double>();

                for (int k = 1; k < MainFrm.QuickDrawList.Count; k++)
                {
                    double dx = MainFrm.QuickDrawList[k].X - MainFrm.QuickDrawList[k - 1].X;
                    double dy = MainFrm.QuickDrawList[k].Y - MainFrm.QuickDrawList[k - 1].Y;

                    // 计算长度
                    double pixelLen = Math.Sqrt(dx * dx + dy * dy);
                    double realLen = pixelLen * ((double)GridToLength / GridSize);
                    MainFrm.CurtOrder.lengAngle[k].Length = Math.Round(realLen, 1);
                    MainFrm.CurtOrder.lengAngle[k].TaperWidth = MainFrm.CurtOrder.lengAngle[k].Length;

                    // 计算角度 (屏幕坐标系直接计算，顺时针为正)
                    double radian = Math.Atan2(dy, dx);
                    absAngles.Add(radian * (180.0 / Math.PI));
                }

                // 3. 计算相对折弯角 (工件内角)
                MainFrm.CurtOrder.lengAngle[1].Angle = 0;

                for (int i = 1; i < absAngles.Count; i++)
                {
                    int targetIndex = i + 1;
                    double currAbs = absAngles[i];
                    double prevAbs = absAngles[i - 1];

                    // 计算偏转角
                    double diff = currAbs - prevAbs;

                    // 归一化
                    while (diff > 180) diff -= 360;
                    while (diff <= -180) diff += 360;

                    // 转换为工件内角 (180 - 偏转)
                    double machineAngle = 180 - Math.Abs(diff);

                    // 赋予符号 (屏幕坐标系：负数代表向上弯/逆时针)
                    if (diff < 0)
                    {
                        machineAngle = -machineAngle;
                    }

                    // 【关键修改】去掉这里的负号，直接存储。
                    // 这样：向上画 -> diff为负 -> machineAngle为负。
                    MainFrm.CurtOrder.lengAngle[targetIndex].Angle = -Math.Round(machineAngle, 1);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("计算错误: " + ex.Message);
                return false;
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            sw对齐网格.BackgroundImage = bAlignGrid ? global::JSZW400.Properties.Resources.sw_左右小开关1 : global::JSZW400.Properties.Resources.sw_左右小开关0;
            lb网格状态.Text = (MainFrm.Lang == 0) ? (bAlignGrid ? "开" : "关") : (bAlignGrid ? "ON" : "OFF");
            lb网格状态.ForeColor = bAlignGrid ? Color.FromArgb(96, 176, 255) : Color.White;
            txb网格单元长.Text = GridToLength.ToString();
        }

        private void sw对齐网格_Click(object sender, EventArgs e)
        {
            bAlignGrid = !bAlignGrid;
        }

        private void sw网格大小_Click(object sender, EventArgs e)
        {
            // 如果需要切换网格大小，可在此实现
            // GridSize = (GridSize == 20) ? 40 : 20;
            // panel2.Invalidate();
        }

        private void btn_插入_Click(object sender, EventArgs e)
        {
            panel3.Visible = !panel3.Visible;
        }


        private void btn确定_Click(object sender, EventArgs e)
        {
            int tn = Convert.ToInt16(tbn.Text);
            double tl1 = Convert.ToDouble(tbl1.Text);
            double tw1 = Convert.ToDouble(tbw1.Text);
            double tw2 = Convert.ToDouble(tbw2.Text);
            double th = Convert.ToDouble(tbh.Text);
            double tl2 = Convert.ToDouble(tbl2.Text);
            GenerateTrapezoidalWave(tn, tl1, tw1, tw2, th, tl2, false);

        }


        public void GenerateTrapezoidalWave(int waveCount, double startLen, double topWidth, double bottomWidth, double height, double bottomFlatLen, bool isArc)
        {
            // 1. 清空旧数据
            MainFrm.QuickDrawList.Clear();

            // 2. 设定起始点 (屏幕左侧中间)
            // 注意：AutoDraw 内部是像素坐标，我们需要把物理尺寸映射到像素
            // 假设比例尺 Scale = GridSize / GridToLength (例如 20px / 20mm = 1.0)
            double scale = (double)GridSize / GridToLength;

            double startX = 50; // 起始 X 坐标
            double startY = panel2.Height / 2 + height * scale / 2; // 起始 Y 坐标 (让图形垂直居中)

            PointF currentP = new PointF((float)startX, (float)startY);
            MainFrm.QuickDrawList.Add(new Point((int)currentP.X, (int)currentP.Y)); // 加入起点

            // 3. 绘制第一段直线 (L)
            currentP.X += (float)(startLen * scale);
            MainFrm.QuickDrawList.Add(new Point((int)currentP.X, (int)currentP.Y));

            // 4. 循环生成波形
            // 一个梯形波 = 上升斜边 -> 顶平 -> 下降斜边 -> 底平

            // 计算斜边的水平投影长度 (a)
            // 根据图示 W2是底宽(包含斜边投影?) 还是 W2是波的总宽?
            // 通常梯形定义：
            // 如果 W1=顶宽, W2=底总宽(斜+底平+斜?) 或者 W2是波长?
            // 看你原代码：a = (t15 - t20) / 2; (底宽 - 顶宽)/2 -> 这暗示 W2 是包含了两个斜边投影的“下底总宽”

            double run = (bottomWidth - topWidth) / 2.0; // 水平投影
                                                         // 斜边长度 c = Sqrt(run^2 + height^2)，但在画图中我们需要的是坐标，不是斜边长

            for (int i = 0; i < waveCount; i++)
            {
                // ----------------- 上升段 -----------------
                // X 增加 run, Y 减少 height (屏幕Y向上为负)
                currentP.X += (float)(run * scale);
                currentP.Y -= (float)(height * scale);
                MainFrm.QuickDrawList.Add(new Point((int)currentP.X, (int)currentP.Y));

                // ----------------- 顶部平段 (W1) -----------------
                // X 增加 topWidth, Y 不变
                currentP.X += (float)(topWidth * scale);
                MainFrm.QuickDrawList.Add(new Point((int)currentP.X, (int)currentP.Y));

                // ----------------- 下降段 -----------------
                // X 增加 run, Y 增加 height (回到基准线)
                currentP.X += (float)(run * scale);
                currentP.Y += (float)(height * scale);
                MainFrm.QuickDrawList.Add(new Point((int)currentP.X, (int)currentP.Y));

                // ----------------- 底部平段 (L1) -----------------
                // 如果不是最后一个波，或者即使是最后一个波也要画底平段连接下一个结构？
                // 根据原图，最后一个波后面直接接尾巴 L，所以只有前 N-1 个波有底平段？
                // 或者底平段就是波与波之间的间隔。
                if (i < waveCount - 1)
                {
                    currentP.X += (float)(bottomFlatLen * scale);
                    MainFrm.QuickDrawList.Add(new Point((int)currentP.X, (int)currentP.Y));
                }
            }

            // 5. 绘制最后一段直线 (L, 这里的 L1 似乎在图中是指底平段，结束段通常也是 L=startLen)
            // 根据原代码 Prolist.Rows[DataGrid1_L - 2].Cells[1 + 2].Value = t17; 暗示结束段也是 t17
            currentP.X += (float)(startLen * scale);
            MainFrm.QuickDrawList.Add(new Point((int)currentP.X, (int)currentP.Y));

            // 6. 触发重绘
            panel2.Invalidate();

            // 7. (可选) 自动触发计算，把点转换成 Order 数据
            ExecuteCalculation();
        }


    }
}