using System.Drawing.Drawing2D;

namespace JSZW400.SubWindows
{
    public partial class SubOPLibrary : UserControl
    {
        MainFrm mf;
        string path1 = MainFrm.ConfigStr[1];
        public int selNum = 0;
        private List<DrawingPanel> drawingPanels; // 存储所有 DrawingPanel
        private DrawingPanel lastSelectedPanel; // 上一个选中的 DrawingPanel
        private int userControlWidth = 181; // 用户控件的宽度
        private int userControlHeight = 181; // 用户控件的高度
        private bool isFirstOpen = true;

        public SubOPLibrary(MainFrm fm1)
        {
            InitializeComponent(); setLang();
            this.mf = fm1;

            lbCurtPath.Text = path1;
            drawingPanels = new List<DrawingPanel>();   // 初始化列表
            lastSelectedPanel = null;                   // 初始化上一个选中的 DrawingPanel
        }

        private void SubOPLibrary_Load(object sender, EventArgs e)
        {
            this.flowLayoutPanel1.AutoScroll = true;
            mf.LoadOrderFile(path1);
            refPreview();


            new TouchScroll(this.flowLayoutPanel1);
        }

        private void setLang()
        {
            if (MainFrm.Lang == 0)
            {
                lbCurtPath.Font = new System.Drawing.Font("Microsoft YaHei UI", 16F);
                lb长度.Font=lb厚度.Font=lb材料.Font=lb名称.Font=lb客户.Font=lb备注.Font = new System.Drawing.Font("Microsoft YaHei UI", 13F);
                btn选择目录.Font = btn重新读取.Font = btn清除.Font = new System.Drawing.Font("宋体", 11.25F);
            }
            else
            {
                lbCurtPath.Font = new System.Drawing.Font("Calibri", 16F);
                lb长度.Font = lb厚度.Font = lb材料.Font = lb名称.Font = lb客户.Font = lb备注.Font = new System.Drawing.Font("Calibri", 13F);
                btn选择目录.Font = btn重新读取.Font = btn清除.Font = new System.Drawing.Font("Calibri", 11.25F);
            }

            lbCurtPath.Text = (MainFrm.Lang == 0) ? "当前路径..." : "Current Path..";
            lb长度.Text = (MainFrm.Lang == 0) ? "长度：" : "Length:";
            lb厚度.Text = (MainFrm.Lang == 0) ? "厚度：" : "Thickness:";
            lb材料.Text = (MainFrm.Lang == 0) ? "材料：" : "Material:";
            lb名称.Text = (MainFrm.Lang == 0) ? "名称：" : "Name:";
            lb客户.Text = (MainFrm.Lang == 0) ? "客户：" : "Customer:";
            lb备注.Text = (MainFrm.Lang == 0) ? "备注：" : "Notes:";
            
            btn选择目录.Text = (MainFrm.Lang == 0) ? "选择\r\n 目录" : "Select\r\nDirectory";
            btn重新读取.Text = (MainFrm.Lang == 0) ? " 重新\r\n 读取" : "Re-read";
            btn清除.Text = (MainFrm.Lang == 0) ? " 清除" : "Delete";

        }

            private void btn重新读取_Click(object sender, EventArgs e)
        {
            mf.LoadOrderFile(path1);

            //this.listView1.FullRowSelect = true;
            //listView1.Items.Clear();
            //refreshLstview();
            refPreview();
        }

        private void refPreview()
        {
            flowLayoutPanel1.Controls.Clear();
            drawingPanels.Clear();          // 清空之前的DrawingPanel列表
            for (int i = 0; i < MainFrm.GblOrder.Count; i++)  //添加10行数据
            {
                DrawingPanel drawingPanel = new DrawingPanel(i, mf, this);

                drawingPanel.Location = new Point(0, 0);
                drawingPanel.Size = new Size(userControlWidth, userControlHeight);
                drawingPanel.Margin = new Padding(1);

                flowLayoutPanel1.Controls.Add(drawingPanel);
                drawingPanels.Add(drawingPanel);            // 将DrawingPanel添加到列表中

                /*
                px += userControlWidth + spacing;

                // 检查是否达到每行的最后一个控件
                if ((i + 1) % columns == 0)
                {
                    // 重置水平位置
                    px = 0;

                    // 更新垂直位置
                    py += userControlHeight + columnSpacing;
                }
                */
            }
        }

        private void btn清除_Click(object sender, EventArgs e)
        {
            /*
            int idx = listView1.FocusedItem.Index;
            mf.Remove1Line(idx);
            btn重新读取.PerformClick();
            */
            if (selNum != 0)
            {
                DeleteIniFile(selNum);
                mf.LoadOrderFile(path1);
                refPreview();
                selNum = 0;
            }
        }
        private void DeleteIniFile(int idx)
        {
            string fileName = $"{path1}\\{MainFrm.GblOrder[idx].Name}.ini";
            if (File.Exists(fileName))
            {
                try
                {
                    File.Delete(fileName);
                }
                catch (Exception ex)
                {
                    if (MainFrm.Lang==0)
                        MessageBox.Show($"删除文件时出错: {ex.Message}");
                    else
                        MessageBox.Show($"Error deleting file: {ex.Message}");
                }
            }
        }
        private void btn选择目录_Click(object sender, EventArgs e)
        {
            选择目录();
        }
        private void lbCurtPath_Click(object sender, EventArgs e)
        {
            选择目录();
        }

        private void 选择目录()
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();

            // 设置对话框的描述文本
            folderDialog.Description = (MainFrm.Lang == 0) ? "请选择一个文件夹" : "Please Select a Folder";



            // 显示文件夹选择对话框
            DialogResult result = folderDialog.ShowDialog();

            // 检查用户是否选择了文件夹
            if (result == DialogResult.OK)
            {
                string folderPath = folderDialog.SelectedPath;

                try
                {
                    // 使用Process.Start来打开文件夹
                    mf.LoadOrderFile(folderPath);
                    refPreview();
                    MainFrm.ConfigStr[1] = folderPath;
                    mf.wrtConfigFile("[OtherConfig]", 2);
                    lbCurtPath.Text = MainFrm.ConfigStr[1];
                }
                catch (Exception ex)
                {
                    // 如果发生错误，显示错误消息
                    MessageBox.Show((MainFrm.Lang == 0) ? "打开文件夹时发生错误: " : "An error occurred while opening the folder" + ex.Message);
                }
            }
        }

        double zoom = 1.0;
        public List<PointF> pxList_Zoom = new List<PointF>();
        public void PanelClicked(DrawingPanel panel, int idx)
        {
            // 重置上一个选中的DrawingPanel的边框样式
            if (lastSelectedPanel != null)
            {
                lastSelectedPanel.BorderStyle = BorderStyle.None;
            }

            // 设置当前选中的DrawingPanel的边框样式
            panel.BorderStyle = BorderStyle.FixedSingle;

            // 更新上一个选中的DrawingPanel
            lastSelectedPanel = panel;

            // 回传idx的值
            selNum = idx;
            // 执行其他逻辑，例如更新UI或处理数据


            lb长度.Text = (MainFrm.Lang == 0) ? "长度：" : "Length:" + MainFrm.GblOrder[selNum].SheetLength.ToString();
            lb厚度.Text = (MainFrm.Lang == 0) ? "厚度：" : "Thickness:" + MainFrm.GblOrder[selNum].Thickness.ToString();
            lb材料.Text = (MainFrm.Lang == 0) ? "材料：" : "Material:" + MainFrm.GblOrder[selNum].MaterialName.ToString();
            lb名称.Text = (MainFrm.Lang == 0) ? "名称：" : "Name:" + MainFrm.GblOrder[selNum].Name.ToString();
            lb客户.Text = (MainFrm.Lang == 0) ? "客户：" : "Customer:" + MainFrm.GblOrder[selNum].Customer.ToString();
            lb备注.Text = (MainFrm.Lang == 0) ? "备注：" : "Notes:" + MainFrm.GblOrder[selNum].Remark.ToString();
        }

        public class DrawingPanel : Panel
        {
            private int idx; // 用于存储Panel序号
            private int chose;
            double zoom = 1.0;
            public List<PointF> pxList_Zoom = new List<PointF>();
            MainFrm MF;
            SubOPLibrary parent;

            public DrawingPanel(int number, MainFrm mainFrm, SubOPLibrary parentControl)
            {
                idx = number;
                MF = mainFrm;
                parent = parentControl; // 获取SubOPKu的实例
                this.Size = new Size(188, 188); // 设置绘图Panel的尺寸
                this.Location = new Point(0, 0);

                this.MouseDoubleClick += DrawingPanel_MouseDoubleClick;// 鼠标双击事件
                this.MouseClick += DrawingPanel_MouseClick;// 鼠标点击事件
                this.Paint += DrawingPanel_Paint;// 订阅Paint事件


                Label label1 = loadname(idx);
                Label label2 = new Label
                {
                    TextAlign = System.Drawing.ContentAlignment.TopLeft,
                    AutoSize = false,
                    Size = new Size((int)52, (int)20),
                    Location = new Point(0, 134),
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(33, 40, 48),
                    Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point),
                    
                    Text = "[" +MainFrm.GblOrder[idx].Width.ToString()+"]"
                };
                this.Controls.Add(label1); this.Controls.Add(label2);
            }

            private void DrawingPanel_Paint(object? sender, PaintEventArgs e)
            {
                Graphics g1 = e.Graphics;
                float minX = 1000, maxX = 0, minY = 1000, maxY = 0;
                int i = 0;
                while (i < MainFrm.GblOrder[idx].pxList.Count)
                {
                    if (MainFrm.GblOrder[idx].pxList[i].X < minX)
                        minX = MainFrm.GblOrder[idx].pxList[i].X;
                    if (MainFrm.GblOrder[idx].pxList[i].X > maxX)
                        maxX = MainFrm.GblOrder[idx].pxList[i].X;
                    if (MainFrm.GblOrder[idx].pxList[i].Y < minY)
                        minY = MainFrm.GblOrder[idx].pxList[i].Y;
                    if (MainFrm.GblOrder[idx].pxList[i].Y > maxY)
                        maxY = MainFrm.GblOrder[idx].pxList[i].Y;
                    i++;
                }

                float cx = this.Size.Width / 2, cy = (this.Size.Height - 30) / 2;

                double zoom_x = ((double)this.Width - 60) / ((double)maxX - (double)minX);
                double zoom_y = ((double)this.Height - 60) / ((double)maxY - (double)minY);
                if (zoom_x < zoom_y)
                    zoom = zoom_x;
                else
                    zoom = zoom_y;

                double ox = Convert.ToDouble(maxX - minX) / 2 + minX;
                double oy = Convert.ToDouble(maxY - minY) / 2 + minY;
                double deltaX = (ox - cx);
                double deltaY = (oy - cy);

                i = 0;
                pxList_Zoom.Clear();
                while (i < MainFrm.GblOrder[idx].pxList.Count)
                {
                    PointF p = new PointF();
                    if (i == 0)
                    {
                        p.X = (float)((Convert.ToDouble(MainFrm.GblOrder[idx].pxList[i].X) - deltaX - cx) * zoom + cx);  //起始位置需要动态
                        p.Y = (float)((Convert.ToDouble(MainFrm.GblOrder[idx].pxList[i].Y) - deltaY - cy) * zoom + cy);
                    }
                    else
                    {
                        float p2x = (float)((Convert.ToDouble(MainFrm.GblOrder[idx].pxList[i].X) - deltaX - cx) * zoom + cx);
                        float p2y = (float)((Convert.ToDouble(MainFrm.GblOrder[idx].pxList[i].Y) - deltaY - cy) * zoom + cy);
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
                Pen myPen1 = new Pen(Color.FromArgb(119, 151, 217), 2);
                int k = 1;
                if (pxList_Zoom.Count > 0)
                {
                    while (k < pxList_Zoom.Count)
                    {
                        g1.DrawLine(myPen1, pxList_Zoom[k - 1].X, pxList_Zoom[k - 1].Y, pxList_Zoom[k].X, pxList_Zoom[k].Y);
                        k++;
                    }
                }
            }

            private void DrawingPanel_MouseClick(object? sender, MouseEventArgs e)
            {
                // 触发父控件的PanelClicked方法，并传递当前的DrawingPanel实例和idx
                parent.PanelClicked(this, idx);
            }

            private void DrawingPanel_MouseDoubleClick(object? sender, MouseEventArgs e)
            {
                //string name = MainFrm.GblOrder[idx].Name;
                int i = 0;
                //while (MainFrm.GblOrder[i].Name != name)
                //{
                //    i++;
                //}
                i = idx;
                //传值赋值,傻办法
                MainFrm.CurtOrder.Name = MainFrm.GblOrder[i].Name;
                MainFrm.CurtOrder.Customer = MainFrm.GblOrder[i].Customer;
                MainFrm.CurtOrder.MaterialName = MainFrm.GblOrder[i].MaterialName;
                MainFrm.CurtOrder.Remark = MainFrm.GblOrder[i].Remark;
                MainFrm.CurtOrder.isTaper = MainFrm.GblOrder[i].isTaper;
                MainFrm.CurtOrder.TaperLength = MainFrm.GblOrder[i].TaperLength;
                MainFrm.CurtOrder.Width = MainFrm.GblOrder[i].Width;
                MainFrm.CurtOrder.Thickness = MainFrm.GblOrder[i].Thickness;
                MainFrm.CurtOrder.SheetLength = MainFrm.GblOrder[i].SheetLength;
                MainFrm.CurtOrder.SlitterWid = MainFrm.GblOrder[i].SlitterWid;
                MainFrm.CurtOrder.pxList = MainFrm.GblOrder[i].pxList.GetRange(0, MainFrm.GblOrder[i].pxList.Count);
                MainFrm.CurtOrder.lstSemiAuto = MainFrm.GblOrder[i].lstSemiAuto.GetRange(0, MainFrm.GblOrder[i].lstSemiAuto.Count);
                for (int j = 0; j < 100; j++)
                {
                    MainFrm.CurtOrder.lengAngle[j].Length = MainFrm.GblOrder[i].lengAngle[j].Length;
                    MainFrm.CurtOrder.lengAngle[j].Angle = MainFrm.GblOrder[i].lengAngle[j].Angle;
                    MainFrm.CurtOrder.lengAngle[j].TaperWidth = MainFrm.GblOrder[i].lengAngle[j].TaperWidth;
                    MainFrm.CurtOrder.lengAngle[j].YinYang = MainFrm.GblOrder[i].lengAngle[j].YinYang;
                }

                //Cancel Mode
                MainFrm.Hmi_iArray[0] = 3;
                MF.AdsWritePlc1Int(0, MainFrm.Hmi_iArray[0]);
                MF.切入自动1(false, true);
            }

            private Label loadname(int idx)
            {
                Label name = new Label
                {
                    Size = new Size((int)187.5, (int)30),
                    Location = new Point(0, 152),
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(33, 40, 48),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point),

                    Text = MainFrm.GblOrder[idx].Name
                };
                return name;
            }
        }

        public class TouchScroll
        {
            private Point mouseDownPoint;
            private FlowLayoutPanel parentPanel;

            public TouchScroll(FlowLayoutPanel panel)
            {
                this.parentPanel = panel;
                AssignEvent(this.parentPanel);
            }

            private void AssignEvent(Control control)
            {
                control.MouseDown += Control_MouseDown;
                control.MouseMove += Control_MouseMove; ;
                foreach (Control child in control.Controls)
                {
                    AssignEvent(child);
                }
            }

            private void Control_MouseMove(object sender, MouseEventArgs e)
            {
                if (e.Button != MouseButtons.Left)
                    return;

                Point pointDifference = new Point(Cursor.Position.X + this.mouseDownPoint.X, Cursor.Position.Y - this.mouseDownPoint.Y);

                if ((this.mouseDownPoint.X == Cursor.Position.X) && (this.mouseDownPoint.Y == Cursor.Position.Y))
                    return;

                Point current = this.parentPanel.AutoScrollPosition;
                this.parentPanel.AutoScrollPosition = new Point(Math.Abs(current.X) - pointDifference.X, Math.Abs(current.Y) - pointDifference.Y);
                this.mouseDownPoint = Cursor.Position;
            }

            private void Control_MouseDown(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                    this.mouseDownPoint = Cursor.Position;
            }
        }


    }
}
