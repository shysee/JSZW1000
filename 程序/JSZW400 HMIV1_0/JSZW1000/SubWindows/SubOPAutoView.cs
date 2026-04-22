using System.Drawing.Drawing2D;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace JSZW400.SubWindows
{

    public partial class SubOPAutoView : UserControl
    {
        MainFrm mf;
        bool isProc;
        public SubOPAutoView(MainFrm fm1, bool proc)
        {
            InitializeComponent(); setLang();
            this.mf = fm1;
            this.isProc = proc;
        }

        private void setLang()
        {
            if (MainFrm.Lang == 0)
            {
                lb继续.Font = lb步骤.Font = lb分条开.Font = lb分条关.Font = label21.Font = label22.Font = label7.Font =
                    label8.Font = label27.Font = label20.Font = label14.Font = new System.Drawing.Font("宋体", 11.25F);

                lb松开高度_最大.Font = lb松开高度_高.Font = lb松开高度_中.Font = lb松开高度_低.Font = lb内外选择_B在外.Font =
                    lb内外选择_A在外.Font = label11.Font = label10.Font = label9.Font = lb抓取类型_超程抓取.Font = lb抓取类型_抓取.Font =
                    lb抓取类型_推动.Font = label5.Font = label15.Font = new System.Drawing.Font("宋体", 12.75F);

                label26.Font = label33.Font = new System.Drawing.Font("Microsoft YaHei UI", 15.75F);
                label25.Font = label2.Font = label3.Font = label35.Font = label29.Font = label30.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F);

                btn装载材料.Font = btn重置计数1.Font = btnSetZero.Font = btnPreViewSt.Font = btnRstView.Font =
                    btnMoveRear.Font = btnMoveFront.Font = new System.Drawing.Font("宋体", 11.25F);
                lb颜色面.Font = lb正逆序.Font = new System.Drawing.Font("微软雅黑", 10.5F);

            }
            else
            {
                lb继续.Font = lb步骤.Font = lb分条开.Font = lb分条关.Font = label21.Font = label22.Font = label7.Font =
                    label8.Font = label27.Font = label20.Font = label14.Font = new System.Drawing.Font("Calibri", 11.25F);

                lb松开高度_最大.Font = lb松开高度_高.Font = lb松开高度_中.Font = lb松开高度_低.Font = lb内外选择_B在外.Font =
                    lb内外选择_A在外.Font = label11.Font = label10.Font = label9.Font = lb抓取类型_超程抓取.Font = lb抓取类型_抓取.Font =
                    lb抓取类型_推动.Font = label5.Font = label15.Font = new System.Drawing.Font("Calibri", 12.75F);
                label26.Font = label33.Font = new System.Drawing.Font("Calibri", 15.75F);
                label25.Font = label2.Font = label3.Font = label35.Font = label29.Font = label30.Font = new System.Drawing.Font("Calibri", 12F);
                btn装载材料.Font = btn重置计数1.Font = btnSetZero.Font = btnPreViewSt.Font = btnRstView.Font =
                    btnMoveRear.Font = btnMoveFront.Font = new System.Drawing.Font("Calibri", 11.25F);
                lb颜色面.Font = lb正逆序.Font = new System.Drawing.Font("Calibri", 10.5F);
            }

            lb继续.Text = (MainFrm.Lang == 0) ? "连续" : "CONTINUE";
            lb步骤.Text = (MainFrm.Lang == 0) ? "步骤" : "STEP";
            lb分条开.Text = (MainFrm.Lang == 0) ? "分条" : "SLIT";
            lb分条关.Text = (MainFrm.Lang == 0) ? "关" : "OFF";
            label21.Text = (MainFrm.Lang == 0) ? "顶部\r\n回弹" : "TOP\r\nSPRB";
            label22.Text = (MainFrm.Lang == 0) ? "底部\r\n回弹" : "BOT.\r\nSPRB";
            label7.Text = (MainFrm.Lang == 0) ? "计算的板材宽度:" : "CALC. WIDTH";
            label8.Text = (MainFrm.Lang == 0) ? "毫米" : "mm";
            label27.Text = (MainFrm.Lang == 0) ? "折弯顺序:" : "FOLD SEQU.";
            label20.Text = (MainFrm.Lang == 0) ? "上料颜色面:" : "FEED COLOUR";
            label14.Text = (MainFrm.Lang == 0) ? "计数:" : "COUNT";

            lb松开高度_最大.Text = (MainFrm.Lang == 0) ? "最大" : "MAX";
            lb松开高度_高.Text = (MainFrm.Lang == 0) ? "高" : "HIGH";
            lb松开高度_中.Text = (MainFrm.Lang == 0) ? "中" : "MEDIUM";
            lb松开高度_低.Text = (MainFrm.Lang == 0) ? "低" : "LOW";
            lb内外选择_B在外.Text = (MainFrm.Lang == 0) ? "B在外" : "B at Out";
            lb内外选择_A在外.Text = (MainFrm.Lang == 0) ? "A在外" : "A at Out";
            label11.Text = (MainFrm.Lang == 0) ? "松开夹钳\r\n高度" : "UNCLAMP\r\n  HEIGHT";
            label10.Text = (MainFrm.Lang == 0) ? "已选中\r\n钳口外" : "Selected Out\r\nside Clamp";

            label9.Text = (MainFrm.Lang == 0) ? "折弯方向" : "FOLD DIRECTION";
            lb抓取类型_超程抓取.Text = (MainFrm.Lang == 0) ? "超程抓取" : "OVERGRIP";
            lb抓取类型_抓取.Text = (MainFrm.Lang == 0) ? "抓取" : "GRIP";
            lb抓取类型_推动.Text = (MainFrm.Lang == 0) ? "推动" : "PUSH";
            label5.Text = (MainFrm.Lang == 0) ? "抓取\r\n类型" : "GRIP\r\nTYPE";
            label15.Text = (MainFrm.Lang == 0) ? "反弹\r\n角度" : "SPRINGBACK\r\nANGLE";

            label26.Text = (MainFrm.Lang == 0) ? "操作提示:" : "Operation Tips:";
            label33.Text = (MainFrm.Lang == 0) ? "折弯列表:" : "Fold List:";

            label25.Text = (MainFrm.Lang == 0) ? "后挡位置" : "Backgauge Pos:";
            label2.Text = (MainFrm.Lang == 0) ? "点动预览" : "Step:";
            label3.Text = (MainFrm.Lang == 0) ? "预 览 速 度 调 节" : "Preview Speed Control";
            label35.Text = (MainFrm.Lang == 0) ? "开始步骤" : "Start Step";
            label29.Text = (MainFrm.Lang == 0) ? "当前步骤" : "Current Step";
            label30.Text = (MainFrm.Lang == 0) ? "预 览 速 度 调 节" : "Preview Speed Control";

            btn装载材料.Text = (MainFrm.Lang == 0) ? " 装载\r\n 材料" : "Load  \r\n Material";
            btn重置计数1.Text = (MainFrm.Lang == 0) ? " 重置\r\n 计数" : "Reset\r\n Count";
            btnSetZero.Text = (MainFrm.Lang == 0) ? " 重置\r\n 步骤" : "Reset\r\n Steps";
            btnPreViewSt.Text = (MainFrm.Lang == 0) ? "运行" : "Run";
            btnRstView.Text = (MainFrm.Lang == 0) ? " 重置\r\n 订单" : "Reset\r\n Order";
            btnMoveRear.Text = (MainFrm.Lang == 0) ? " 移动\r\n 向后" : "Move\r\n Down";
            btnMoveFront.Text = (MainFrm.Lang == 0) ? " 移动\r\n 向前" : "Move\r\nUp    ";

            lb正逆序.Text = (MainFrm.Lang == 0) ? (MainFrm.CurtOrder.st逆序 ? "逆序" : "正序") : (MainFrm.CurtOrder.st逆序 ? "Reverse Order" : "Positive Order");
            lb颜色面.Text = (MainFrm.Lang == 0) ? (MainFrm.CurtOrder.st色下 ? "颜色向下" : "颜色向上") : (MainFrm.CurtOrder.st色下 ? "ColourSide Below" : "ColourSide Top");




        }

        public List<Point> pxDraw = new List<Point>();
        private void SubOPAutoView_Load(object sender, EventArgs e)
        {
            pnl左工具栏1.Parent = this;
            pnl左工具栏2.Parent = this;
            pnl左工具栏3.Parent = this;
            pnl左工具栏1.Visible = false;
            pnl左工具栏2.Visible = false;
            pnl左工具栏3.Visible = false;
            pnl左工具栏1.Location = new Point(2, 101);
            pnl左工具栏2.Location = new Point(2, 101);
            pnl左工具栏3.Location = new Point(2, 101);
            txbSpringTop.Text = string.Format("{0:F2}", MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 10]);
            txbSpringBtm.Text = string.Format("{0:F2}", MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 11]);
            if (isProc)
            {
                stPreView();
            }
            else
            {
                stSetting();
            }
        }

        public void stSetting()
        {
            pnl左工具栏1.Visible = false;
            pnl左工具栏2.Visible = true;
            pnl左工具栏3.Visible = false;
            InitDraw(false);
            Graphics g1 = this.pictureBox1.CreateGraphics();
            redrawGraphics(g1);
            reCreateBtn();
        }

        public void stPreView()
        {
            pnl左工具栏1.Visible = true;
            pnl左工具栏2.Visible = false; pnl左工具栏3.Visible = false;
            InitDraw(true);
        }

        private void btnRstView_Click(object sender, EventArgs e)
        {
            mf.create生产序列();
            stSetting();
        }

        double zoom = 1.0;
        public List<PointF> pxList_Zoom = new List<PointF>();
        void redrawGraphics(Graphics g1)
        {
            float minX = 1000, maxX = 0, minY = 1000, maxY = 0;
            int i = 0;
            while (i < MainFrm.CurtOrder.pxList.Count)
            {
                if (MainFrm.CurtOrder.pxList[i].X < minX)
                    minX = MainFrm.CurtOrder.pxList[i].X;
                if (MainFrm.CurtOrder.pxList[i].X > maxX)
                    maxX = MainFrm.CurtOrder.pxList[i].X;
                if (MainFrm.CurtOrder.pxList[i].Y < minY)
                    minY = MainFrm.CurtOrder.pxList[i].Y;
                if (MainFrm.CurtOrder.pxList[i].Y > maxY)
                    maxY = MainFrm.CurtOrder.pxList[i].Y;
                i++;
            }

            int cx = pictureBox1.Size.Width / 2, cy = pictureBox1.Size.Height / 2;
            if ((float)(pictureBox1.Width - 260) / (float)(maxX - minX) < (float)(pictureBox1.Height - 260) / (float)(maxY - minY))
                zoom = (double)(pictureBox1.Width - 260) / (double)(maxX - minX);
            else
                zoom = (double)(pictureBox1.Height - 260) / (double)(maxY - minY);

            double ox = Convert.ToDouble(maxX - minX) / 2 + minX;
            double oy = Convert.ToDouble(maxY - minY) / 2 + minY;
            double deltaX = (ox - cx);
            double deltaY = (oy - cy);

            i = 0;
            pxList_Zoom.Clear();
            while (i < MainFrm.CurtOrder.pxList.Count)
            {
                Point p = new Point();
                if (i == 0)
                {
                    p.X = Convert.ToInt32((Convert.ToDouble(MainFrm.CurtOrder.pxList[i].X) - deltaX - cx) * zoom + cx);  //起始位置需要动态
                    p.Y = Convert.ToInt32((Convert.ToDouble(MainFrm.CurtOrder.pxList[i].Y) - deltaY - cy) * zoom + cy);
                    //p.X = 100;  //起始位置需要动态
                    //p.Y = 500;
                }
                else
                {
                    int p2x = Convert.ToInt32((Convert.ToDouble(MainFrm.CurtOrder.pxList[i].X) - deltaX - cx) * zoom + cx);
                    int p2y = Convert.ToInt32((Convert.ToDouble(MainFrm.CurtOrder.pxList[i].Y) - deltaY - cy) * zoom + cy);
                    p.X = p2x;
                    p.Y = p2y;
                }

                pxList_Zoom.Add(p);
                i++;
            }


            g1.SmoothingMode = SmoothingMode.AntiAlias;
            g1.CompositingQuality = CompositingQuality.HighQuality;
            g1.Clear(Color.FromArgb(33, 40, 48));
            //绘制折线图
            Pen myPen0 = new Pen(Color.FromArgb(246, 203, 181), 2);
            Pen myPen1 = new Pen(Color.FromArgb(119, 151, 217), 3);
            int k = 1;
            if (MainFrm.CurtOrder.pxList.Count > 0)
            {
                while (k < MainFrm.CurtOrder.pxList.Count)
                {
                    //g1.DrawLine(myPen1, MainFrm.CurtOrder.pxList[k - 1].X, MainFrm.CurtOrder.pxList[k - 1].Y, MainFrm.CurtOrder.pxList[k].X, MainFrm.CurtOrder.pxList[k].Y);
                    g1.DrawLine(myPen1, pxList_Zoom[k - 1].X, pxList_Zoom[k - 1].Y, pxList_Zoom[k].X, pxList_Zoom[k].Y);

                    k++;
                }
            }


            if (MainFrm.CurtOrder.pxList.Count > 2 && (MainFrm.CurtOrder.lengAngle[0].Angle > 0 || MainFrm.CurtOrder.lengAngle[99].Angle > 0))
                drawSquash(MainFrm.CurtOrder.lengAngle[0].Length, MainFrm.CurtOrder.lengAngle[99].Length);

            ConnLine();

        }

        private void drawSquash(double SquaL0, double SquaL99)
        {
            Graphics g1 = pictureBox1.CreateGraphics();
            Pen myPen2 = new Pen(Color.FromArgb(155, 187, 253), 4);
            PointF pSqua0 = new PointF(), pSqua1 = new PointF();
            PointF pSqua90 = new PointF(), pSqua91 = new PointF();
            if (MainFrm.CurtOrder.lengAngle[0].Angle > 0)
            {
                double p0_delta_y = (pxList_Zoom[1].X - pxList_Zoom[0].X) * 3.5 * zoom / Math.Sqrt(Math.Pow(pxList_Zoom[0].X - pxList_Zoom[1].X, 2) + Math.Pow(pxList_Zoom[0].Y - pxList_Zoom[1].Y, 2));
                double p0_delta_x = (pxList_Zoom[0].Y - pxList_Zoom[1].Y) * 3.5 * zoom / Math.Sqrt(Math.Pow(pxList_Zoom[0].X - pxList_Zoom[1].X, 2) + Math.Pow(pxList_Zoom[0].Y - pxList_Zoom[1].Y, 2));

                double p1_delta_y = (pxList_Zoom[1].Y - pxList_Zoom[0].Y) * SquaL0 * zoom / Math.Sqrt(Math.Pow(pxList_Zoom[0].X - pxList_Zoom[1].X, 2) + Math.Pow(pxList_Zoom[0].Y - pxList_Zoom[1].Y, 2));
                double p1_delta_x = (pxList_Zoom[1].X - pxList_Zoom[0].X) * SquaL0 * zoom / Math.Sqrt(Math.Pow(pxList_Zoom[0].X - pxList_Zoom[1].X, 2) + Math.Pow(pxList_Zoom[0].Y - pxList_Zoom[1].Y, 2));
                //向下挤压
                if (MainFrm.CurtOrder.lengAngle[0].Angle == 2 || MainFrm.CurtOrder.lengAngle[0].Angle == 4)
                {
                    pSqua0.X = pxList_Zoom[0].X + (float)p0_delta_x;
                    pSqua0.Y = pxList_Zoom[0].Y + (float)p0_delta_y;
                    pSqua1.X = pSqua0.X + (float)p1_delta_x;
                    pSqua1.Y = pSqua0.Y + (float)p1_delta_y;
                }
                //向上挤压
                if (MainFrm.CurtOrder.lengAngle[0].Angle == 1 || MainFrm.CurtOrder.lengAngle[0].Angle == 3)
                {
                    pSqua0.X = pxList_Zoom[0].X - (float)p0_delta_x;
                    pSqua0.Y = pxList_Zoom[0].Y - (float)p0_delta_y;
                    pSqua1.X = pSqua0.X + (float)p1_delta_x;
                    pSqua1.Y = pSqua0.Y + (float)p1_delta_y;
                }

                g1.DrawLine(myPen2, pSqua0.X, pSqua0.Y, pSqua1.X, pSqua1.Y);
                g1.DrawLine(myPen2, pSqua0.X, pSqua0.Y, pxList_Zoom[0].X, pxList_Zoom[0].Y);
            }
            if (MainFrm.CurtOrder.lengAngle[99].Angle > 0)
            {
                int m = MainFrm.CurtOrder.pxList.Count - 1;
                double p90_delta_y = (pxList_Zoom[m].X - pxList_Zoom[m - 1].X) * 3.5 * zoom / Math.Sqrt(Math.Pow(pxList_Zoom[m - 1].X - pxList_Zoom[m].X, 2) + Math.Pow(pxList_Zoom[m - 1].Y - pxList_Zoom[m].Y, 2));
                double p90_delta_x = (pxList_Zoom[m - 1].Y - pxList_Zoom[m].Y) * 3.5 * zoom / Math.Sqrt(Math.Pow(pxList_Zoom[m - 1].X - pxList_Zoom[m].X, 2) + Math.Pow(pxList_Zoom[m - 1].Y - pxList_Zoom[m].Y, 2));
                double p91_delta_y = (pxList_Zoom[m].Y - pxList_Zoom[m - 1].Y) * SquaL99 * zoom / Math.Sqrt(Math.Pow(pxList_Zoom[m - 1].X - pxList_Zoom[m].X, 2) + Math.Pow(pxList_Zoom[m - 1].Y - pxList_Zoom[m].Y, 2));
                double p91_delta_x = (pxList_Zoom[m].X - pxList_Zoom[m - 1].X) * SquaL99 * zoom / Math.Sqrt(Math.Pow(pxList_Zoom[m - 1].X - pxList_Zoom[m].X, 2) + Math.Pow(pxList_Zoom[m - 1].Y - pxList_Zoom[m].Y, 2));
                //向下挤压
                if (MainFrm.CurtOrder.lengAngle[99].Angle == 2 || MainFrm.CurtOrder.lengAngle[99].Angle == 4)
                {
                    pSqua90.X = pxList_Zoom[m].X + (float)p90_delta_x;
                    pSqua90.Y = pxList_Zoom[m].Y + (float)p90_delta_y;
                    pSqua91.X = pSqua90.X - (float)p91_delta_x;
                    pSqua91.Y = pSqua90.Y - (float)p91_delta_y;
                }
                //向上挤压
                if (MainFrm.CurtOrder.lengAngle[99].Angle == 1 || MainFrm.CurtOrder.lengAngle[99].Angle == 3)
                {
                    pSqua90.X = pxList_Zoom[m].X - (float)p90_delta_x;
                    pSqua90.Y = pxList_Zoom[m].Y - (float)p90_delta_y;
                    pSqua91.X = pSqua90.X - (float)p91_delta_x;
                    pSqua91.Y = pSqua90.Y - (float)p91_delta_y;
                }

                g1.DrawLine(myPen2, pSqua90.X, pSqua90.Y, pSqua91.X, pSqua91.Y);
                g1.DrawLine(myPen2, pSqua90.X, pSqua90.Y, pxList_Zoom[m].X, pxList_Zoom[m].Y);
            }
        }

        public void ConnLine()
        {
            Graphics g1 = pictureBox1.CreateGraphics();
            float cx = pictureBox1.Size.Width / 2, cy = pictureBox1.Size.Height / 2;
            float myText_H = 32, myText_W = 45;
            Pen myPen4 = new Pen(Color.PeachPuff, 1);
            for (int i = 0; i < pxList_Zoom.Count - 2; i++)
            {
                PointF Lxy2 = new PointF();
                if (pxList_Zoom[i + 1].X < cx && pxList_Zoom[i + 1].Y < cy)
                {
                    Lxy2.X = (pxList_Zoom[i + 1].X) - 80 + myText_W;
                    Lxy2.Y = (pxList_Zoom[i + 1].Y) - 40 + myText_H;
                }
                else if (pxList_Zoom[i + 1].X >= cx && pxList_Zoom[i + 1].Y < cy)
                {
                    Lxy2.X = (pxList_Zoom[i + 1].X) + 40;
                    Lxy2.Y = (pxList_Zoom[i + 1].Y) - 40 + myText_H;
                }
                else if (pxList_Zoom[i + 1].X < cx && pxList_Zoom[i + 1].Y >= cy)
                {
                    Lxy2.X = (pxList_Zoom[i + 1].X) - 80 + myText_W;
                    Lxy2.Y = (pxList_Zoom[i + 1].Y) + 40;
                }
                else if (pxList_Zoom[i + 1].X >= cx && pxList_Zoom[i + 1].Y >= cy)
                {
                    Lxy2.X = (pxList_Zoom[i + 1].X) + 40;
                    Lxy2.Y = (pxList_Zoom[i + 1].Y) + 40;
                }
                g1.DrawLine(myPen4, Lxy2.X, Lxy2.Y, (pxList_Zoom[i + 1].X), pxList_Zoom[i + 1].Y);
            }
        }

        public void reCreateBtn()
        {
            Graphics g1 = pictureBox1.CreateGraphics();
            if (MainFrm.CurtOrder.pxList.Count <= 0)
                return;
            float cx = pictureBox1.Size.Width / 2, cy = pictureBox1.Size.Height / 2;

            pictureBox1.Controls.Clear();
            int k = 0;
            int idx = 0;
            for (int i = 0; i <= MainFrm.CurtOrder.lstSemiAuto.Count - 1; i++)
            {
                if (MainFrm.CurtOrder.lstSemiAuto[i].行动类型 > 0)
                    continue;

                Button FunBtn0 = new Button();
                FunBtn0.Name = "btnFun" + string.Format("{0:D2}", idx + 1); idx++;
                FunBtn0.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
                //FunBtn0.Size = new System.Drawing.Size(60, 32);
                FunBtn0.Size = new System.Drawing.Size(80, 32);
                FunBtn0.Text = "折弯 00?";
                FunBtn0.Tag = 0;        //存储实际折弯序号,与显示折弯号有区别
                FunBtn0.BackColor = Color.SeaShell;
                PointF Lxy2 = new PointF();

                int px_idx = MainFrm.CurtOrder.lstSemiAuto[i].坐标序号;
                if (MainFrm.CurtOrder.lstSemiAuto[i].行动类型 == 0 && MainFrm.CurtOrder.lstSemiAuto[i].折弯角度 == 3.001)
                {
                    Lxy2.X = pxList_Zoom[px_idx].X - 50; Lxy2.Y = pxList_Zoom[px_idx].Y - 40;
                    k++;
                }
                else if (MainFrm.CurtOrder.lstSemiAuto[i].行动类型 == 0 && MainFrm.CurtOrder.lstSemiAuto[i].折弯角度 == 3.99)
                {
                    Lxy2.X = pxList_Zoom[px_idx].X + 10; Lxy2.Y = pxList_Zoom[px_idx].Y - 40;
                    k++;
                }
                else if (MainFrm.CurtOrder.lstSemiAuto[i].行动类型 == 0)
                {
                    if (pxList_Zoom[px_idx].X < cx)
                        Lxy2.X = (pxList_Zoom[px_idx].X) - 80;
                    else
                        Lxy2.X = (pxList_Zoom[px_idx].X) + 40;
                    if (pxList_Zoom[px_idx].Y < cy)
                        Lxy2.Y = (pxList_Zoom[px_idx].Y) - 40;
                    else
                        Lxy2.Y = (pxList_Zoom[px_idx].Y) + 40;
                }

                FunBtn0.Location = new Point((int)Lxy2.X, (int)Lxy2.Y);
                FunBtn0.FlatStyle = System.Windows.Forms.FlatStyle.Popup;

                FunBtn0.Click += new EventHandler(myButton_Click);
                pictureBox1.Controls.Add(FunBtn0);

            }
            //if (MainFrm.CurtOrder.lengAngle[0].Angle == 2)
            //{
            //    Button FunBtn0 = new Button();
            //    FunBtn0.Name = "btnFun" + string.Format("{0:D2}", 0);
            //    FunBtn0.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            //    FunBtn0.Size = new System.Drawing.Size(60, 32);
            //    FunBtn0.Text = "折弯 00?";
            //    FunBtn0.BackColor = Color.SeaShell;
            //    FunBtn0.Location = new Point(pxList_Zoom[0].X - 60, pxList_Zoom[0].Y);
            //    FunBtn0.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            //    FunBtn0.Click += new EventHandler(myButton_Click);
            //    panel1.Controls.Add(FunBtn0);
            //}
            //if (MainFrm.CurtOrder.lengAngle[99].Angle == 2)
            //{
            //    Button FunBtn0 = new Button();
            //    FunBtn0.Name = "btnFun" + string.Format("{0:D2}", 99);
            //    FunBtn0.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            //    FunBtn0.Size = new System.Drawing.Size(60, 32);
            //    FunBtn0.Text = "折弯 00?";
            //    FunBtn0.BackColor = Color.SeaShell;
            //    FunBtn0.Location = new Point(pxList_Zoom[pxList_Zoom.Count - 1].X + 20, pxList_Zoom[pxList_Zoom.Count - 1].Y);
            //    FunBtn0.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            //    FunBtn0.Click += new EventHandler(myButton_Click);
            //    panel1.Controls.Add(FunBtn0);
            //}



            reViewMyButton();

            Label FirstPt0 = new Label();
            FirstPt0.Name = "lbFirstPt";
            FirstPt0.Font = new System.Drawing.Font("Arial", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FirstPt0.Size = new System.Drawing.Size(34, 34);
            FirstPt0.Text = "A";
            FirstPt0.BackColor = Color.FromArgb(33, 40, 48);
            FirstPt0.ForeColor = Color.FromArgb(128, 255, 128);
            FirstPt0.Location = new Point((int)pxList_Zoom[0].X - 40, (int)pxList_Zoom[0].Y);
            pictureBox1.Controls.Add(FirstPt0);

            Label EndPt99 = new Label();
            EndPt99.Name = "lbEndPt";
            EndPt99.Font = new System.Drawing.Font("Arial", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            EndPt99.Size = new System.Drawing.Size(34, 34);
            EndPt99.Text = "B";
            EndPt99.BackColor = Color.FromArgb(33, 40, 48);
            EndPt99.ForeColor = Color.FromArgb(255, 128, 0);
            EndPt99.Location = new Point((int)pxList_Zoom[pxList_Zoom.Count - 1].X + 10, (int)pxList_Zoom[pxList_Zoom.Count - 1].Y);
            pictureBox1.Controls.Add(EndPt99);
        }


        int selStep = 1;
        int selBtnNm = 0;
        int sel折弯序号 = 0;
        void myButton_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            string s = btn.Text.Substring(3, 2);
            selStep = Convert.ToInt16(s);
            selBtnNm = Convert.ToInt16(btn.Name.Substring(6, 2));
            selBtnx = btn;
            sel折弯序号 = Convert.ToInt16(btn.Tag);

            label12.Text = btn.Name;

            reViewMyButton();

            //pnl左工具栏1.Visible = false;
            //pnl左工具栏2.Visible = true;
            //pnl左工具栏2.Location = new Point(2, 101);

            //Button btn = (Button)sender;
            //pnl挤压功能选.Visible = true;
            //pnl长度功能选.Visible = false;
            //pnl角度功能选.Visible = false;
            //if (btn.Name == "btnSquash0")
            //{
            //    selSquash = 0;
            //    cbx挤压类型.SelectedIndex = Convert.ToInt32(MainFrm.CurtOrder.lengAngle[0].Angle);
            //    txb挤压长度.Text = Convert.ToString(MainFrm.CurtOrder.lengAngle[0].Length);
            //}
            //else if (btn.Name == "btnSquash1")
            //{
            //    selSquash = 99;
            //    cbx挤压类型.SelectedIndex = Convert.ToInt32(MainFrm.CurtOrder.lengAngle[99].Angle);
            //    txb挤压长度.Text = Convert.ToString(MainFrm.CurtOrder.lengAngle[99].Length);
            //}
        }

        void reViewMyButton()
        {
            Button btnx = new Button();
            int idx = 0;
            int i折弯序号 = 1;
            for (int i = 0; i <= MainFrm.CurtOrder.lstSemiAuto.Count - 1; i++)
            {
                if (MainFrm.CurtOrder.lstSemiAuto[i].行动类型 > 0)
                    continue;

                btnx = ((Button)this.Controls.Find("btnFun" + string.Format("{0:D2}", idx + 1), true)[0]); idx++;
                btnx.Tag = MainFrm.CurtOrder.lstSemiAuto[i].折弯序号;
                btnx.Text = "折弯 " + string.Format("{0:D2}", i折弯序号) + "\r\n";
                i折弯序号++;

                if (MainFrm.CurtOrder.lstSemiAuto[i].回弹值 > 0)
                    btnx.Text += "+";
                btnx.Text += MainFrm.CurtOrder.lstSemiAuto[i].回弹值.ToString() + " ";
                if (MainFrm.CurtOrder.lstSemiAuto[i].松开高度 == 0)
                    btnx.Text += "低";
                else if (MainFrm.CurtOrder.lstSemiAuto[i].松开高度 == 1)
                    btnx.Text += "中";
                else if (MainFrm.CurtOrder.lstSemiAuto[i].松开高度 == 2)
                    btnx.Text += "高";
                else if (MainFrm.CurtOrder.lstSemiAuto[i].松开高度 == 3)
                    btnx.Text += "最大";
                //btnx.Text += i.ToString();
                if (MainFrm.CurtOrder.lstSemiAuto[i].内外选择 == 1)
                    btnx.Text += "B";
                else
                    btnx.Text += "A";

                btnx.Text += "|" + btnx.Tag.ToString();

                //int i2 = Convert.ToInt16(btnx.Text.Substring(3, 2));

                int i2 = MainFrm.CurtOrder.lstSemiAuto[i].折弯序号;
                btnx.BackColor = (i2 == sel折弯序号) ? Color.FromArgb(161, 0, 0) : Color.SeaShell;
                btnx.ForeColor = (i2 == sel折弯序号) ? Color.White : Color.Black;




            }

            for (int i = 0; i < MainFrm.CurtOrder.lstSemiAuto.Count; i++)
            {
                if (sel折弯序号 == MainFrm.CurtOrder.lstSemiAuto[i].折弯序号)
                {
                    db抓取类型子项 = MainFrm.CurtOrder.lstSemiAuto[i].抓取类型;
                    db松开高度子项 = MainFrm.CurtOrder.lstSemiAuto[i].松开高度;
                    db内外选择子项 = MainFrm.CurtOrder.lstSemiAuto[i].内外选择;
                    db折弯方向子项 = MainFrm.CurtOrder.lstSemiAuto[i].折弯方向;
                    txbSpringBend.Text = MainFrm.CurtOrder.lstSemiAuto[i].回弹值.ToString();
                    break;
                }
            }

            抓取类型显示();
            松开高度显示();
            内外选择显示();
            折弯方向显示();
        }
        Button selBtnx = new Button();
        private void btnMoveFront_Click(object sender, EventArgs e)
        {
            int i上移步 = 0, i移动量 = 0;
            if (selStep > 1)
            {
                for (int i = 0; i < MainFrm.CurtOrder.lstSemiAuto.Count; i++)
                {
                    if (MainFrm.CurtOrder.lstSemiAuto[i].折弯序号 == sel折弯序号)
                    {
                        if (MainFrm.CurtOrder.lstSemiAuto[i - 1].行动类型 == 1 || MainFrm.CurtOrder.lstSemiAuto[i - 1].行动类型 == 2)
                            i上移步 = 2;
                        else if (MainFrm.CurtOrder.lstSemiAuto[i - 1].行动类型 == 0)
                            i上移步 = 1;
                        if (MainFrm.CurtOrder.lstSemiAuto[i].折弯角度 == 3.99 || MainFrm.CurtOrder.lstSemiAuto[i].折弯角度 == 3.001)
                            i移动量 = 2;
                        else
                            i移动量 = 1;
                    }
                }

                for (int i = 0; i < MainFrm.CurtOrder.lstSemiAuto.Count; i++)
                {
                    if (MainFrm.CurtOrder.lstSemiAuto[i].折弯序号 == sel折弯序号)
                    {
                        var temp1 = MainFrm.CurtOrder.lstSemiAuto[i];
                        temp1.折弯序号 = temp1.折弯序号 - i上移步;
                        MainFrm.CurtOrder.lstSemiAuto[i] = temp1;
                        if (i移动量 == 2)
                        {
                            var temp2 = MainFrm.CurtOrder.lstSemiAuto[i + 1];
                            temp2.折弯序号 = temp2.折弯序号 - i上移步;
                            MainFrm.CurtOrder.lstSemiAuto[i + 1] = temp2;
                        }

                        //前端需要移动之数据
                        var temp21 = MainFrm.CurtOrder.lstSemiAuto[i - i上移步];
                        temp21.折弯序号 = temp21.折弯序号 + i移动量;
                        MainFrm.CurtOrder.lstSemiAuto[i - i上移步] = temp21;
                        if (i上移步 == 2)
                        {
                            var temp22 = MainFrm.CurtOrder.lstSemiAuto[i - i上移步 + 1];
                            temp22.折弯序号 = temp22.折弯序号 + i移动量;
                            MainFrm.CurtOrder.lstSemiAuto[i - i上移步 + 1] = temp22;
                        }
                        //互换按钮Name
                        Button tmpBtn = new Button();
                        tmpBtn = ((Button)this.Controls.Find("btnFun" + string.Format("{0:D2}", selStep - 1), true)[0]);
                        tmpBtn.Name = "btnFun" + string.Format("{0:D2}", selStep);

                        //互换按钮Name
                        selBtnx.Name = "btnFun" + string.Format("{0:D2}", selStep - 1);
                    }
                }
                sel折弯序号 = sel折弯序号 - i上移步;
                selStep--;
                SortSemiLst();
                reViewMyButton();
            }
        }

        private void btnMoveRear_Click(object sender, EventArgs e)
        {
            int i下移量 = 0, i上移量 = 0;
            if (sel折弯序号 < MainFrm.CurtOrder.lstSemiAuto.Count)
            {
                for (int i = 0; i < MainFrm.CurtOrder.lstSemiAuto.Count; i++)
                {
                    if (MainFrm.CurtOrder.lstSemiAuto[i].折弯序号 == sel折弯序号)
                    {
                        if (MainFrm.CurtOrder.lstSemiAuto[i].折弯角度 == 3.99 || MainFrm.CurtOrder.lstSemiAuto[i].折弯角度 == 3.001)
                        {
                            i下移量 = 2;
                            if ((i + 2) < MainFrm.CurtOrder.lstSemiAuto.Count &&
                                (MainFrm.CurtOrder.lstSemiAuto[i + 2].折弯角度 == 3.99 || MainFrm.CurtOrder.lstSemiAuto[i + 2].折弯角度 == 3.001))
                                i上移量 = 2;
                            else
                                i上移量 = 1;
                        }
                        else
                        {
                            i下移量 = 1;
                            if ((i + 1) < MainFrm.CurtOrder.lstSemiAuto.Count &&
                                (MainFrm.CurtOrder.lstSemiAuto[i + 1].折弯角度 == 3.99 || MainFrm.CurtOrder.lstSemiAuto[i + 1].折弯角度 == 3.001))
                                i上移量 = 2;
                            else
                                i上移量 = 1;
                        }
                    }
                }

                for (int i = MainFrm.CurtOrder.lstSemiAuto.Count - 1; i >= 0; i--)
                {
                    if (MainFrm.CurtOrder.lstSemiAuto[i].折弯序号 == sel折弯序号)
                    {
                        var temp11 = MainFrm.CurtOrder.lstSemiAuto[i];
                        temp11.折弯序号 = temp11.折弯序号 + i上移量;
                        MainFrm.CurtOrder.lstSemiAuto[i] = temp11;
                        if (i下移量 == 2)
                        {
                            var temp12 = MainFrm.CurtOrder.lstSemiAuto[i + 1];
                            temp12.折弯序号 = temp12.折弯序号 + i上移量;
                            MainFrm.CurtOrder.lstSemiAuto[i + 1] = temp12;
                        }
                        //下方的上移                        
                        var temp21 = MainFrm.CurtOrder.lstSemiAuto[i + i下移量];
                        temp21.折弯序号 = temp21.折弯序号 - i下移量;
                        MainFrm.CurtOrder.lstSemiAuto[i + i下移量] = temp21;
                        if (i上移量 == 2)
                        {
                            var temp22 = MainFrm.CurtOrder.lstSemiAuto[i + i下移量 + 1];
                            temp22.折弯序号 = temp22.折弯序号 - i下移量;
                            MainFrm.CurtOrder.lstSemiAuto[i + i下移量 + 1] = temp22;
                        }
                        //互换按钮Name
                        Button tmpBtn = new Button();
                        tmpBtn = ((Button)this.Controls.Find("btnFun" + string.Format("{0:D2}", selStep + 1), true)[0]);
                        tmpBtn.Name = "btnFun" + string.Format("{0:D2}", selStep);
                        //互换按钮Name
                        selBtnx.Name = "btnFun" + string.Format("{0:D2}", selStep + 1);
                    }
                }
                sel折弯序号 = sel折弯序号 + i下移量;
                selStep++;
                SortSemiLst();
                reViewMyButton();
            }
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

        double db内外选择子项 = 0;
        private void 内外选择显示()
        {
            if ((int)db内外选择子项 == 0)
            {
                btn内外选择.Image = global::JSZW400.Properties.Resources.btm_单工双工0;
                this.lb折弯方向AB.ForeColor = Color.FromArgb(96, 176, 255);
                this.lb折弯方向AB.Text = "A";
            }

            else if ((int)db内外选择子项 == 1)
            {
                btn内外选择.Image = global::JSZW400.Properties.Resources.btm_单工双工1;
                this.lb折弯方向AB.ForeColor = Color.FromArgb(255, 128, 0);
                this.lb折弯方向AB.Text = "B";
            }


            lb内外选择_A在外.ForeColor = ((int)db内外选择子项 == 0) ? Color.FromArgb(72, 209, 72) : Color.White;
            lb内外选择_B在外.ForeColor = ((int)db内外选择子项 == 1) ? Color.FromArgb(240, 134, 81) : Color.White;
        }

        private void btn内外选择_Click(object sender, EventArgs e)
        {
            db内外选择子项++;
            db内外选择子项 = (db内外选择子项) % 2;
            内外选择显示();
            var temp = MainFrm.CurtOrder.lstSemiAuto[sel折弯序号 - 1];
            temp.内外选择 = (int)db内外选择子项;
            temp.折弯方向 = 1 - temp.折弯方向;
            MainFrm.CurtOrder.lstSemiAuto[sel折弯序号 - 1] = temp;

            重建后挡料(sel折弯序号);
            reViewMyButton();
        }

        private void lb内外选择_A在外_Click(object sender, EventArgs e)
        {
            Label btn = (Label)sender;
            if (btn.Name == "lb内外选择_B在外") { db内外选择子项 = 1; }
            else if (btn.Name == "lb内外选择_A在外") { db内外选择子项 = 0; }
            内外选择显示();
            var temp = MainFrm.CurtOrder.lstSemiAuto[sel折弯序号 - 1];
            temp.内外选择 = (int)db内外选择子项;
            temp.折弯方向 = 1 - temp.折弯方向;
            MainFrm.CurtOrder.lstSemiAuto[sel折弯序号 - 1] = temp;

            重建后挡料(sel折弯序号);
            reViewMyButton();
        }

        double db抓取类型子项 = 0;
        private void 抓取类型显示()
        {
            if ((int)db抓取类型子项 == 0)
                btn抓取类型.Image = global::JSZW400.Properties.Resources.btm_3档开关彩1;
            else if ((int)db抓取类型子项 == 1)
                btn抓取类型.Image = global::JSZW400.Properties.Resources.btm_3档开关彩2;
            else if ((int)db抓取类型子项 == 2)
                btn抓取类型.Image = global::JSZW400.Properties.Resources.btm_3档开关彩3;

            lb抓取类型_推动.ForeColor = ((int)db抓取类型子项 == 0) ? Color.FromArgb(240, 28, 28) : Color.White;
            lb抓取类型_抓取.ForeColor = ((int)db抓取类型子项 == 1) ? Color.FromArgb(240, 134, 81) : Color.White;
            lb抓取类型_超程抓取.ForeColor = ((int)db抓取类型子项 == 2) ? Color.FromArgb(72, 209, 72) : Color.White;
        }
        private void btn抓取类型_Click(object sender, EventArgs e)
        {
            db抓取类型子项++;
            db抓取类型子项 = (db抓取类型子项) % 3;
            抓取类型显示();
            var temp = MainFrm.CurtOrder.lstSemiAuto[sel折弯序号 - 1];
            temp.抓取类型 = (int)db抓取类型子项;
            MainFrm.CurtOrder.lstSemiAuto[sel折弯序号 - 1] = temp;
            reViewMyButton();
        }

        private void lb抓取类型_推动_Click(object sender, EventArgs e)
        {
            Label btn = (Label)sender;
            if (btn.Name == "lb抓取类型_推动") { db抓取类型子项 = 0; }
            else if (btn.Name == "lb抓取类型_抓取") { db抓取类型子项 = 1; }
            else if (btn.Name == "lb抓取类型_超程抓取") { db抓取类型子项 = 2; }
            抓取类型显示();
            var temp = MainFrm.CurtOrder.lstSemiAuto[sel折弯序号 - 1];
            temp.抓取类型 = (int)db抓取类型子项;
            MainFrm.CurtOrder.lstSemiAuto[sel折弯序号 - 1] = temp;
            reViewMyButton();
        }

        double db松开高度子项 = 0;
        private void btn松开高度_Click(object sender, EventArgs e)
        {
            db松开高度子项++;
            db松开高度子项 = (db松开高度子项) % 4;
            松开高度显示();
            //MainFrm.CurtOrder.lstSemiAuto[selStep].抓取类型 =(int)db抓取类型子项;
            var temp = MainFrm.CurtOrder.lstSemiAuto[sel折弯序号 - 1];
            temp.松开高度 = (int)db松开高度子项;
            MainFrm.CurtOrder.lstSemiAuto[sel折弯序号 - 1] = temp;

            reViewMyButton();
        }

        private void lb松开高度_低_Click(object sender, EventArgs e)
        {
            Label btn = (Label)sender;
            if (btn.Name == "lb松开高度_低") { db松开高度子项 = 0; }
            else if (btn.Name == "lb松开高度_中") { db松开高度子项 = 1; }
            else if (btn.Name == "lb松开高度_高") { db松开高度子项 = 2; }
            else if (btn.Name == "lb松开高度_最大") { db松开高度子项 = 3; }
            松开高度显示();
            var temp = MainFrm.CurtOrder.lstSemiAuto[sel折弯序号 - 1];
            temp.松开高度 = (int)db松开高度子项;
            MainFrm.CurtOrder.lstSemiAuto[sel折弯序号 - 1] = temp;
            reViewMyButton();
        }
        private void 松开高度显示()
        {
            if ((int)db松开高度子项 == 0)
                btn松开高度.Image = global::JSZW400.Properties.Resources.btm_4档开关1;
            else if ((int)db松开高度子项 == 1)
                btn松开高度.Image = global::JSZW400.Properties.Resources.btm_4档开关2;
            else if ((int)db松开高度子项 == 2)
                btn松开高度.Image = global::JSZW400.Properties.Resources.btm_4档开关3;
            else
                btn松开高度.Image = global::JSZW400.Properties.Resources.btm_4档开关4;

            lb松开高度_低.ForeColor = ((int)db松开高度子项 == 0) ? Color.FromArgb(96, 176, 255) : Color.White;
            lb松开高度_中.ForeColor = ((int)db松开高度子项 == 1) ? Color.FromArgb(96, 176, 255) : Color.White;
            lb松开高度_高.ForeColor = ((int)db松开高度子项 == 2) ? Color.FromArgb(96, 176, 255) : Color.White;
            lb松开高度_最大.ForeColor = ((int)db松开高度子项 == 3) ? Color.FromArgb(96, 176, 255) : Color.White;
        }

        double db折弯方向子项 = 0;
        private void btn折弯方向_Click(object sender, EventArgs e)
        {
            db折弯方向子项++;
            db折弯方向子项 = (db折弯方向子项) % 2;
            折弯方向显示();
            var temp = MainFrm.CurtOrder.lstSemiAuto[sel折弯序号 - 1];
            temp.折弯方向 = (int)db折弯方向子项;
            MainFrm.CurtOrder.lstSemiAuto[sel折弯序号 - 1] = temp;

            reViewMyButton();
        }

        private void pnl折弯方向_向下_Click(object sender, EventArgs e)
        {
            Panel pnl = (Panel)sender;
            if (pnl.Name == "pnl折弯方向_向下") { db折弯方向子项 = 0; }
            else if (pnl.Name == "pnl折弯方向_向上") { db折弯方向子项 = 1; }
            折弯方向显示();
            var temp = MainFrm.CurtOrder.lstSemiAuto[sel折弯序号 - 1];
            temp.折弯方向 = (int)db折弯方向子项;
            MainFrm.CurtOrder.lstSemiAuto[sel折弯序号 - 1] = temp;
            reViewMyButton();
        }

        private void 折弯方向显示()
        {
            if ((int)db折弯方向子项 == 0)
            {
                btn折弯方向.Image = global::JSZW400.Properties.Resources.btm_单工双工0;
                pnl当前折弯方向.BackgroundImage = global::JSZW400.Properties.Resources.BottomApron4;
            }
            else if ((int)db折弯方向子项 == 1)
            {
                btn折弯方向.Image = global::JSZW400.Properties.Resources.btm_单工双工1;
                pnl当前折弯方向.BackgroundImage = global::JSZW400.Properties.Resources.TopApron4;
            }
        }

        private void btnPlus_Click(object sender, EventArgs e)
        {
            var temp = MainFrm.CurtOrder.lstSemiAuto[sel折弯序号 - 1];
            if (temp.回弹值 < 9)
                temp.回弹值 = temp.回弹值 + 0.5;
            MainFrm.CurtOrder.lstSemiAuto[sel折弯序号 - 1] = temp;

            reViewMyButton();
        }

        private void btnMinus_Click(object sender, EventArgs e)
        {
            var temp = MainFrm.CurtOrder.lstSemiAuto[sel折弯序号 - 1];
            if (temp.回弹值 > -9)
                temp.回弹值 = temp.回弹值 - 0.5;
            MainFrm.CurtOrder.lstSemiAuto[sel折弯序号 - 1] = temp;

            reViewMyButton();
        }

        private void 重建后挡料(int start)
        {
            SortSemiLst();
            double leng = 0;
            double ExLeng = MainFrm.CurtOrder.Width;      //后挡位置:为总宽度-已折长度


            for (int j = 0; j < MainFrm.CurtOrder.lstSemiAuto.Count; j++)
            {
                int idx = 0;
                if (MainFrm.CurtOrder.lstSemiAuto[j].长角序号 == 0 || MainFrm.CurtOrder.lstSemiAuto[j].长角序号 == 99)
                    idx = MainFrm.CurtOrder.lstSemiAuto[j].长角序号;
                else if (MainFrm.CurtOrder.lstSemiAuto[j].内外选择 == 0)
                    idx = MainFrm.CurtOrder.lstSemiAuto[j].长角序号;
                else if (MainFrm.CurtOrder.lstSemiAuto[j].内外选择 == 1)
                    idx = MainFrm.CurtOrder.lstSemiAuto[j].长角序号 + 1;


                ExLeng -= MainFrm.CurtOrder.lengAngle[idx].Length;
                var temp = MainFrm.CurtOrder.lstSemiAuto[j];
                if (j > 0)
                {
                    if (MainFrm.CurtOrder.lstSemiAuto[j].内外选择 == MainFrm.CurtOrder.lstSemiAuto[j - 1].内外选择)
                        temp.is色下 = MainFrm.CurtOrder.lstSemiAuto[j - 1].is色下;
                    else
                        temp.is色下 = !MainFrm.CurtOrder.lstSemiAuto[j - 1].is色下;
                }
                temp.内外选择 = 1 - MainFrm.CurtOrder.lstSemiAuto[j].内外选择;
                temp.后挡位置 = ExLeng;
                MainFrm.CurtOrder.lstSemiAuto[j] = temp;
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
            if (lb下一操作提示.Text == "翻面")
            {
                Flip_DataProc();
                return;
            }


            iDrawStep++;

            if (iDrawStep > (MainFrm.CurtOrder.lstSemiAuto.Count) * 2)
            {
                tmr预览.Enabled = false;
                return;
            }



            refshPoint();
            Graphics g1 = pictureBox1.CreateGraphics();
            int idd = (int)((iDrawStep - 1) / 2);
            if (idd < 0) idd = 0;
            //redrawPreView(g1, MainFrm.CurtOrder.lstSemiAuto[idd].is色下);
            redrawPreView(MainFrm.CurtOrder.lstSemiAuto[idd].is色下);

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
            int i = 0;
            //int leng = 0;
            iDrawStep = 0;
            SortSemiLst();
            cx = pictureBox1.Size.Width / 2; cy = pictureBox1.Size.Height / 2;
            pictureBox1.Controls.Clear();
            pxDraw.Clear();
            pxDraw.Add(new Point(cx - 0, cy + 0));
            double ExLeng = MainFrm.CurtOrder.Width;
            double leng = 0;
            Point px = new Point();
            while (i < MainFrm.CurtOrder.lstSemiAuto.Count)
            {
                if (MainFrm.CurtOrder.lstSemiAuto[i].行动类型 == 1 || MainFrm.CurtOrder.lstSemiAuto[i].行动类型 == 2)
                {
                    ;
                }
                else
                {
                    leng += ExLeng - MainFrm.CurtOrder.lstSemiAuto[i].后挡位置;
                    px.X = cx - (int)leng;
                    px.Y = 0 + cy;
                }

                pxDraw.Add(px);
                pxDraw.Add(px);
                ExLeng = MainFrm.CurtOrder.lstSemiAuto[i].后挡位置;
                i++;
            }

            leng += ExLeng;
            pxDraw.Add(new Point(cx - (int)leng, cy + 0));

            if (!isPreView)
                return;

            Graphics g1 = pictureBox1.CreateGraphics();
            int idd = (int)((iDrawStep - 1) / 2);
            if (idd < 0) idd = 0;
            if (idd >= MainFrm.CurtOrder.lstSemiAuto.Count)
                MessageBox.Show((MainFrm.Lang == 0) ? "步骤超出" : "Exceeding the Steps");
            else
                //redrawPreView(g1, MainFrm.CurtOrder.lstSemiAuto[idd].is色下);
                redrawPreView(MainFrm.CurtOrder.lstSemiAuto[idd].is色下);
            txbDrawStep.Text = iDrawStep.ToString();
        }

        void refshPoint()
        {
            int k = 0;
            double angle = 0;
            double len = 0;
            int half = (int)((iDrawStep - 1) / 2);

            /** *iDrawStep:
             * 1,3,5,7,9... 移动
             * 2,4,6,8,10... 折弯
             */
            if (iDrawStep % 2 == 0) //折弯
            {
                len = 0;
                double ang = 0;
                if (MainFrm.CurtOrder.lstSemiAuto[half].折弯角度 == 3.001 || MainFrm.CurtOrder.lstSemiAuto[half].折弯角度 == 3.99)
                    ang = 30;
                else
                    ang = MainFrm.CurtOrder.lstSemiAuto[half].折弯角度;

                if ((MainFrm.CurtOrder.lstSemiAuto[half].行动类型 == 0 && MainFrm.CurtOrder.lstSemiAuto[half].折弯方向 == 0))
                    angle = 180 - ang;
                else if (MainFrm.CurtOrder.lstSemiAuto[half].行动类型 == 0 && MainFrm.CurtOrder.lstSemiAuto[half].折弯方向 == 1)
                    angle = 180 + ang;
                k = 翻面前步数;
                while (k < pxDraw.Count - 1)
                {
                    Point tmp = new Point();

                    if (k < iDrawStep)
                    {
                        tmp = PointRotate(pxDraw[iDrawStep - 1], pxDraw[k], angle);
                        pxDraw[k] = tmp;
                        tmp = PointRotate(pxDraw[iDrawStep - 1], pxDraw[k + 1], angle);
                        pxDraw[k + 1] = tmp;
                    }
                    k++; k++;
                }
            }
            else                //移动
            {
                double i1 = Math.Abs(pxDraw[iDrawStep].X - pxDraw[iDrawStep - 1].X);  // MainFrm.CurtOrder.lengAngle[idx + 1].Length;
                len = i1;


                if (MainFrm.CurtOrder.lstSemiAuto[half].行动类型 == 1 || MainFrm.CurtOrder.lstSemiAuto[half].行动类型 == 2)
                {
                    len = -4;
                    dReGive = (-1) * len;
                }
                else
                {
                    len += dReGive;
                    dReGive = 0;
                }




                k = 0;
                while (k < pxDraw.Count)
                {
                    Point tmp = new Point();
                    tmp = pxDraw[k];
                    tmp.X = tmp.X + (int)len;
                    pxDraw[k] = tmp;
                    k++;
                }
            }
        }
        double dReGive = 0;

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

        private void button2_Click(object sender, EventArgs e)
        {
            drimg();
        }
        Bitmap image1 = new Bitmap(1180, 805);
        void drimg()
        {
            using (Graphics graphic = Graphics.FromImage(image1))
            {
                Image myImage = (Image)global::JSZW400.Properties.Resources.预览设备图虚化;

                graphic.DrawImage(myImage, 0, 0, myImage.Width, myImage.Height);
                var p = new Point(DateTime.Now.Second * 3, DateTime.Now.Second * 3);
                graphic.DrawLine(Pens.Red, p, new Point(200, 200));
            }
            pictureBox1.Image?.Dispose();
            pictureBox1.Image = (Image)image1.Clone();
        }

        void redrawPreView(bool is色下0)
        {
            using (Graphics graphic = Graphics.FromImage(image1))
            {
                Image myImage = (Image)global::JSZW400.Properties.Resources.预览设备图虚化;

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
                        int ls = (k - 1) / 2;
                        if (ls < MainFrm.CurtOrder.lstSemiAuto.Count
                            && (MainFrm.CurtOrder.lstSemiAuto[ls].行动类型 == 1 || MainFrm.CurtOrder.lstSemiAuto[ls].行动类型 == 2))
                        {
                            ;
                        }
                        else
                            graphic.DrawLine(myPen1, pxDraw[k - 1].X, pxDraw[k - 1].Y, pxDraw[k].X, pxDraw[k].Y);

                        //画颜色线
                        if (is色下0)
                            graphic.DrawLine(myPen2, pxDraw[k - 1].X, pxDraw[k - 1].Y + 5, pxDraw[k].X, pxDraw[k].Y + 5);
                        else
                            graphic.DrawLine(myPen2, pxDraw[k - 1].X, pxDraw[k - 1].Y - 5, pxDraw[k].X, pxDraw[k].Y - 5);

                        k++; k++;
                    }
                }
            }
            pictureBox1.Image?.Dispose();
            pictureBox1.Image = (Image)image1.Clone();

            //下一操作提示
            int half = (int)((iDrawStep - 1) / 2);
            if (half < 0) half = 0;
            if (half >= 0 && half < MainFrm.CurtOrder.lstSemiAuto.Count - 1
                && MainFrm.CurtOrder.lstSemiAuto[half].内外选择 != MainFrm.CurtOrder.lstSemiAuto[(iDrawStep) / 2].内外选择)
            {
                lb下一操作提示.Text = "翻面";
            }

            //else if (MainFrm.CurtOrder.lstSemiAuto[half].折弯角度 == 3.001 || MainFrm.CurtOrder.lstSemiAuto[half].折弯角度 == 3.99)
            else if (MainFrm.CurtOrder.lstSemiAuto[half].行动类型 == 1 || MainFrm.CurtOrder.lstSemiAuto[half].行动类型 == 2)
                lb下一操作提示.Text = "挤压";
            else if (MainFrm.CurtOrder.lstSemiAuto[half].折弯方向 == 0)
                lb下一操作提示.Text = "向上折弯";
            else if (MainFrm.CurtOrder.lstSemiAuto[half].折弯方向 == 1)
                lb下一操作提示.Text = "向下折弯";

            label6.Text = MainFrm.CurtOrder.lstSemiAuto[half].后挡位置.ToString();


        }

        void redrawPreView(Graphics g1, bool is色下0)
        {
            g1.SmoothingMode = SmoothingMode.AntiAlias;
            g1.CompositingQuality = CompositingQuality.HighQuality;
            g1.Clear(Color.FromArgb(33, 40, 48));
            //
            //
            //RectangleF srcRect = new RectangleF(0, 0, 546, 805);
            //GraphicsUnit units = GraphicsUnit.Pixel;
            //g1.DrawImage((Image)global::JSZW400.Properties.Resources.预览背景图0, 0, 0, srcRect, units);

            //绘制背景图
            RectangleF srcRect = new RectangleF(0, 0, 1180, 805);
            GraphicsUnit units = GraphicsUnit.Pixel;
            //g1.DrawImage((Image)global::JSZW400.Properties.Resources.预览设备图, 0, 0, srcRect, units);
            Point pxi = new Point(0, 0);
            Image myImage = (Image)global::JSZW400.Properties.Resources.预览设备图虚化;
            //myImage.SetResolution(96, 96);
            g1.DrawImage(myImage, 0, 0, myImage.Width, myImage.Height);

            //绘制折线图
            Pen myPen0 = new Pen(Color.Green, 1);
            myPen0.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash; //虚线

            //Pen myPen1 = new Pen(Color.FromArgb(119, 151, 217), 6);
            //Pen myPen1 = new Pen(Color.FromArgb(198, 255, 00), 4);
            Pen myPen1 = new Pen(Color.White, 4);
            g1.DrawLine(myPen0, cx, 0 + 100, cx, pictureBox1.Size.Height - 100);
            //Pen myPen2 = new Pen(Color.FromArgb(255, 255, 0), 1);
            Pen myPen2 = new Pen(Color.FromArgb(255, 87, 34), 1);

            int k = 1;
            if (pxDraw.Count > 0)
            {
                while (k < pxDraw.Count)
                {
                    int ls = (k - 1) / 2;
                    if (ls < MainFrm.CurtOrder.lstSemiAuto.Count
                        && (MainFrm.CurtOrder.lstSemiAuto[ls].行动类型 == 1 || MainFrm.CurtOrder.lstSemiAuto[ls].行动类型 == 2))
                    {
                        ;
                    }
                    else
                        g1.DrawLine(myPen1, pxDraw[k - 1].X, pxDraw[k - 1].Y, pxDraw[k].X, pxDraw[k].Y);

                    //画颜色线
                    if (is色下0)
                        g1.DrawLine(myPen2, pxDraw[k - 1].X, pxDraw[k - 1].Y + 5, pxDraw[k].X, pxDraw[k].Y + 5);
                    else
                        g1.DrawLine(myPen2, pxDraw[k - 1].X, pxDraw[k - 1].Y - 5, pxDraw[k].X, pxDraw[k].Y - 5);

                    k++; k++;
                }
            }


            //下一操作提示
            int half = (int)((iDrawStep - 1) / 2);
            if (half < 0) half = 0;
            if (half >= 0 && half < MainFrm.CurtOrder.lstSemiAuto.Count - 1
                && MainFrm.CurtOrder.lstSemiAuto[half].内外选择 != MainFrm.CurtOrder.lstSemiAuto[(iDrawStep) / 2].内外选择)
            {
                lb下一操作提示.Text = "翻面";
            }


            //else if (MainFrm.CurtOrder.lstSemiAuto[half].折弯角度 == 3.001 || MainFrm.CurtOrder.lstSemiAuto[half].折弯角度 == 3.99)
            else if (MainFrm.CurtOrder.lstSemiAuto[half].行动类型 == 1 || MainFrm.CurtOrder.lstSemiAuto[half].行动类型 == 2)
                lb下一操作提示.Text = "挤压";
            else if (MainFrm.CurtOrder.lstSemiAuto[half].折弯方向 == 0)
                lb下一操作提示.Text = "向上折弯";
            else if (MainFrm.CurtOrder.lstSemiAuto[half].折弯方向 == 1)
                lb下一操作提示.Text = "向下折弯";


            label6.Text = MainFrm.CurtOrder.lstSemiAuto[half].后挡位置.ToString();
        }

        private void btnSetZero_Click(object sender, EventArgs e)
        {
            iDrawStep = 0;
            翻面前步数 = 0;
            pxDraw.Clear();
            InitDraw(true);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (sel折弯序号 > 0)
                label19.Text = MainFrm.CurtOrder.lstSemiAuto[sel折弯序号 - 1].后挡位置.ToString();
            label32.Text = sel折弯序号.ToString();
            label4.Text = selStep.ToString();


            txbDrawStep.Text = iDrawStep.ToString();
            txbDrawStep2.Text = iDrawStep.ToString();
            txb当前步序.Text = (MainFrm.Hmi_iArray[70] + 1).ToString();


            btn自动预览.BackgroundImage = tmr预览.Enabled ? global::JSZW400.Properties.Resources.sw_左右小开关1 : global::JSZW400.Properties.Resources.sw_左右小开关0;
            lb自动预览.Text = tmr预览.Enabled ? "自动预览" : "点动预览";
            lb自动预览.ForeColor = tmr预览.Enabled ? Color.FromArgb(96, 176, 255) : Color.White;

            pnlAuto.BackgroundImage = MainFrm.Hmi_iArray[20] == 6 ? global::JSZW400.Properties.Resources.AutoStart : global::JSZW400.Properties.Resources.AutoOrig1_zh_CHS;
            txb计算总宽.Text = MainFrm.CurtOrder.Width.ToString();

            sw继续步骤.Image = MainFrm.Hmi_bArray[71] ? global::JSZW400.Properties.Resources.btm_2档开关1 : global::JSZW400.Properties.Resources.btm_2档开关0;
            lb继续.ForeColor = MainFrm.Hmi_bArray[71] ? Color.FromArgb(96, 176, 255) : Color.White;
            lb步骤.ForeColor = !MainFrm.Hmi_bArray[71] ? Color.FromArgb(96, 176, 255) : Color.White;

            sw分条开关.Image = MainFrm.Hmi_bArray[72] ? global::JSZW400.Properties.Resources.btm_分条开关1 : global::JSZW400.Properties.Resources.btm_分条开关0;
            lb分条开.ForeColor = MainFrm.Hmi_bArray[72] ? Color.FromArgb(96, 176, 255) : Color.White;
            lb分条关.ForeColor = !MainFrm.Hmi_bArray[72] ? Color.FromArgb(96, 176, 255) : Color.White;

            sw正逆序.BackgroundImage = MainFrm.CurtOrder.st逆序 ? global::JSZW400.Properties.Resources.sw_左右小开关1 : global::JSZW400.Properties.Resources.sw_左右小开关0;
            lb正逆序.Text = (MainFrm.Lang == 0) ? (MainFrm.CurtOrder.st逆序 ? "逆序" : "正序") : (MainFrm.CurtOrder.st逆序 ? "Reverse Order" : "Positive Order");
            lb正逆序.ForeColor = MainFrm.CurtOrder.st逆序 ? Color.FromArgb(96, 176, 255) : Color.White;
            sw颜色面.BackgroundImage = MainFrm.CurtOrder.st色下 ? global::JSZW400.Properties.Resources.sw_左右小开关1 : global::JSZW400.Properties.Resources.sw_左右小开关0;
            lb颜色面.Text = (MainFrm.Lang == 0) ? (MainFrm.CurtOrder.st色下 ? "颜色向下" : "颜色向上") : (MainFrm.CurtOrder.st色下 ? "ColourSide Below" : "ColourSide Top");
            lb颜色面.ForeColor = MainFrm.CurtOrder.st色下 ? Color.FromArgb(96, 176, 255) : Color.White;
        }

        int 翻面前步数 = 0;

        private void tmr预览_Tick(object sender, EventArgs e)
        {
            btnPreViewSt.PerformClick();
        }

        private void btn自动预览_Click(object sender, EventArgs e)
        {
            tmr预览.Enabled = !tmr预览.Enabled;
            btn自动预览.BackgroundImage = tmr预览.Enabled ? global::JSZW400.Properties.Resources.sw_左右小开关1 : global::JSZW400.Properties.Resources.sw_左右小开关0;
            lb自动预览.Text = tmr预览.Enabled ? "自动预览" : "点动预览";
            lb自动预览.ForeColor = tmr预览.Enabled ? Color.FromArgb(96, 176, 255) : Color.White;
            if (tmr预览.Enabled)
                btnSetZero.PerformClick();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            tmr预览.Interval = trackBar1.Maximum - trackBar1.Value;
        }



        private void Flip_DataProc()
        {
            翻面前步数 = iDrawStep;
            int k = 0;
            //------------------------------翻转4步骤-------------------------
            int min = 9999, max = 0;
            //Step1. 寻找旋转中心点
            for (int i = 0; i < pxDraw.Count; i++)
            {
                if (pxDraw[i].X > max)
                    max = pxDraw[i].X;
                if (pxDraw[i].X < min)
                    min = pxDraw[i].X;
            }
            //Step2. 按图形中心,旋转180度
            Point ptc = new Point((min + max) / 2, 402);
            while (k < pxDraw.Count)
            {
                Point tmp = new Point();
                tmp = PointRotate(ptc, pxDraw[k], 180);
                pxDraw[k] = tmp;
                k++;
            }
            //Step3. 图形左移,移动距离=折过的X方向长度
            k = 0;
            while (k < pxDraw.Count)
            {
                Point tmp = new Point();
                tmp = pxDraw[k];
                tmp.X = tmp.X - (max - cx);
                pxDraw[k] = tmp;
                k++;
            }
            //Step4. 重新建立未折部分的坐标点
            double leng = 0;
            double ExLeng = MainFrm.CurtOrder.lstSemiAuto[iDrawStep / 2 - 1].后挡位置;      //后挡位置:为总宽度-已折长度
            Point tmp1 = new Point();
            tmp1.X = cx;
            tmp1.Y = cy - 0;
            pxDraw[iDrawStep] = tmp1;
            for (int j = iDrawStep + 1; j < pxDraw.Count - 2; j = j + 2)
            {
                if (MainFrm.CurtOrder.lstSemiAuto[j / 2].行动类型 == 1 || MainFrm.CurtOrder.lstSemiAuto[j / 2].行动类型 == 2)
                {
                    ;
                }
                else
                {
                    leng += ExLeng - MainFrm.CurtOrder.lstSemiAuto[j / 2].后挡位置;
                    tmp1.X = cx - (int)leng;
                    tmp1.Y = cy;
                }
                pxDraw[j] = tmp1;
                pxDraw[j + 1] = tmp1;
                ExLeng = MainFrm.CurtOrder.lstSemiAuto[j / 2].后挡位置;
            }
            leng += ExLeng;
            tmp1.X = cx - (int)leng;
            tmp1.Y = cy;
            pxDraw[pxDraw.Count - 1] = tmp1;


            Graphics g1 = pictureBox1.CreateGraphics();
            //redrawPreView(g1, MainFrm.CurtOrder.lstSemiAuto[(iDrawStep - 1) / 2 + 1].is色下);
            redrawPreView(MainFrm.CurtOrder.lstSemiAuto[(iDrawStep - 1) / 2 + 1].is色下);
            lb下一操作提示.Text = "翻面完成";

            dReGive = 0;

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
                s0 = (MainFrm.Lang == 0) ? "确定需要启用纵切吗？" : "You Want to Enable Slitter?";

            DialogAsk dlgTips = new DialogAsk("", s0);
            dlgTips.StartPosition = FormStartPosition.Manual;
            dlgTips.Location = new Point(500, 200);
            if ((!MainFrm.Hmi_bArray[72] && dlgTips.ShowDialog() == DialogResult.OK) || MainFrm.Hmi_bArray[72])
            {
                MainFrm.Hmi_bArray[72] = !MainFrm.Hmi_bArray[72];
                sw分条开关.Image = MainFrm.Hmi_bArray[72] ? global::JSZW400.Properties.Resources.btm_分条开关1 : global::JSZW400.Properties.Resources.btm_分条开关0;
                mf.AdsWritePlc1Bit(72, MainFrm.Hmi_bArray[72]);
            }
        }

        private void sw继续步骤_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[71] = !MainFrm.Hmi_bArray[71];
            sw继续步骤.Image = MainFrm.Hmi_bArray[71] ? global::JSZW400.Properties.Resources.btm_2档开关1 : global::JSZW400.Properties.Resources.btm_2档开关0;
            mf.AdsWritePlc1Bit(71, MainFrm.Hmi_bArray[71]);
        }

        private void timer2s_Tick(object sender, EventArgs e)
        {
            if (!pnl左工具栏3.Visible)
                return;
            if (iDrawStep < (MainFrm.Hmi_iArray[70] + 1) * 2)
                PreViewSt();
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
