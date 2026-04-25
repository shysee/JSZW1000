using netDxf;
using netDxf.Entities;
using System.Drawing.Drawing2D;
using static JSZW1000A.MainFrm;

namespace JSZW1000A.SubWindows
{
    public partial class SubOPLibrary : UserControl
    {
        MainFrm mf;
        string path1 = MainFrm.ConfigStr[1];
        public int selNum = -1;
        private List<DrawingPanel> drawingPanels; // 存储所有 DrawingPanel
        private DrawingPanel? lastSelectedPanel; // 上一个选中的 DrawingPanel
        private int userControlWidth = 181; // 用户控件的宽度
        private int userControlHeight = 181; // 用户控件的高度

        public SubOPLibrary(MainFrm fm1)
        {
            InitializeComponent(); setLang();
            this.mf = fm1;

            drawingPanels = new List<DrawingPanel>();   // 初始化列表
            lastSelectedPanel = null;                   // 初始化上一个选中的 DrawingPanel
            UpdateCurrentPathLabel();
            ResetSelectedOrderSummary();
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
            LocalizationManager.ApplyResources(this);
            if (MainFrm.Lang == 0)
            {
                lbCurtPath.Font = new System.Drawing.Font("Microsoft YaHei UI", 16F);
                lb长度.Font = lb厚度.Font = lb材料.Font = lb名称.Font = lb客户.Font = lb备注.Font = new System.Drawing.Font("Microsoft YaHei UI", 13F);
                btn选择目录.Font = btn重新读取.Font = btn清除.Font = new System.Drawing.Font("宋体", 11.25F);
            }
            else
            {
                lbCurtPath.Font = new System.Drawing.Font("Calibri", 16F);
                lb长度.Font = lb厚度.Font = lb材料.Font = lb名称.Font = lb客户.Font = lb备注.Font = new System.Drawing.Font("Calibri", 13F);
                btn选择目录.Font = btn重新读取.Font = btn清除.Font = new System.Drawing.Font("Calibri", 11.25F);
            }
            lbCurtPath.Text = Strings.Get("Library.PathLabel");
            lb长度.Text = Strings.Get("Library.LengthLabel");
            lb厚度.Text = Strings.Get("Library.ThicknessLabel");
            lb材料.Text = Strings.Get("Library.MaterialLabel");
            lb名称.Text = Strings.Get("Library.NameLabel");
            lb客户.Text = Strings.Get("Library.CustomerLabel");
            lb备注.Text = Strings.Get("Library.NotesLabel");
            btn选择目录.Text = Strings.Get("Library.SelectDirectory");
            btn重新读取.Text = Strings.Get("Library.Reload");
            btn清除.Text = Strings.Get("Library.Delete");
            btn导入DXF.Text = Strings.Get("Library.ImportDxf");
            UpdateCurrentPathLabel();
            if (lastSelectedPanel != null && selNum >= 0 && selNum < MainFrm.GblOrder.Count)
                UpdateSelectedOrderSummary();
            else
                ResetSelectedOrderSummary();
        }

        private void UpdateCurrentPathLabel()
        {
            lbCurtPath.Text = string.IsNullOrWhiteSpace(path1)
                ? Strings.Get("Library.PathLabel")
                : Strings.Get("Library.PathLabel") + path1;
        }

        private void ResetSelectedOrderSummary()
        {
            lb长度.Text = Strings.Get("Library.LengthLabel");
            lb厚度.Text = Strings.Get("Library.ThicknessLabel");
            lb材料.Text = Strings.Get("Library.MaterialLabel");
            lb名称.Text = Strings.Get("Library.NameLabel");
            lb客户.Text = Strings.Get("Library.CustomerLabel");
            lb备注.Text = Strings.Get("Library.NotesLabel");
        }

        private void UpdateSelectedOrderSummary()
        {
            if (selNum < 0 || selNum >= MainFrm.GblOrder.Count)
            {
                ResetSelectedOrderSummary();
                return;
            }

            var order = MainFrm.GblOrder[selNum];
            lb长度.Text = Strings.Get("Library.LengthLabel") + MainFrm.FormatDisplayLengthWithUnit(order.SheetLength);
            lb厚度.Text = Strings.Get("Library.ThicknessLabel") + MainFrm.FormatDisplayLengthWithUnit(order.Thickness);
            lb材料.Text = Strings.Get("Library.MaterialLabel") + order.MaterialName;
            lb名称.Text = Strings.Get("Library.NameLabel") + order.Name;
            lb客户.Text = Strings.Get("Library.CustomerLabel") + order.Customer;
            lb备注.Text = Strings.Get("Library.NotesLabel") + order.Remark;
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
            lastSelectedPanel = null;
            selNum = -1;
            ResetSelectedOrderSummary();
            for (int i = 0; i < MainFrm.GblOrder.Count; i++)  //添加10行数据
            {
                DrawingPanel drawingPanel = new DrawingPanel(i, mf, this);

                drawingPanel.Location = new System.Drawing.Point(0, 0);
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
            if (selNum >= 0 && selNum < MainFrm.GblOrder.Count)
            {
                DeleteIniFile(selNum);
                mf.LoadOrderFile(path1);
                refPreview();
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
                    MessageBox.Show(string.Format(System.Globalization.CultureInfo.InvariantCulture, Strings.Get("Library.Message.DeleteFileError"), ex.Message));
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
            folderDialog.Description = Strings.Get("Library.FolderDialog.Description");

            // 显示文件夹选择对话框
            DialogResult result = folderDialog.ShowDialog();

            // 检查用户是否选择了文件夹
            if (result == DialogResult.OK)
            {
                string folderPath = folderDialog.SelectedPath;

                try
                {
                    // 使用Process.Start来打开文件夹
                    path1 = folderPath;
                    mf.LoadOrderFile(path1);
                    refPreview();
                    MainFrm.ConfigStr[1] = path1;
                    mf.wrtConfigFile("[OtherConfig]", 2);
                    UpdateCurrentPathLabel();
                }
                catch (Exception ex)
                {
                    // 如果发生错误，显示错误消息
                    MessageBox.Show(string.Format(System.Globalization.CultureInfo.InvariantCulture, Strings.Get("Library.Message.OpenFolderError"), ex.Message));
                }
            }
        }

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

            UpdateSelectedOrderSummary();
        }

        public class DrawingPanel : Panel
        {
            private int idx; // 用于存储Panel序号
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
                this.Location = new System.Drawing.Point(0, 0);

                this.MouseDoubleClick += DrawingPanel_MouseDoubleClick;// 鼠标双击事件
                this.MouseClick += DrawingPanel_MouseClick;// 鼠标点击事件
                this.Paint += DrawingPanel_Paint;// 订阅Paint事件


                System.Windows.Forms.Label label1 = loadname(idx);
                System.Windows.Forms.Label label2 = new System.Windows.Forms.Label
                {
                    TextAlign = System.Drawing.ContentAlignment.TopLeft,
                    AutoSize = false,
                    Size = new Size((int)52, (int)20),
                    Location = new System.Drawing.Point(0, 134),
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(33, 40, 48),
                    Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point),

                    Text = "[" + MainFrm.FormatDisplayLength(MainFrm.GblOrder[idx].Width) + "]"
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
                MF.subOPManual?.LoadGridFromCurrentOrder();
                MF.切入自动1(false, true);
            }

            private System.Windows.Forms.Label loadname(int idx)
            {
                System.Windows.Forms.Label name = new System.Windows.Forms.Label
                {
                    Size = new Size((int)187.5, (int)30),
                    Location = new System.Drawing.Point(0, 152),
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
            private System.Drawing.Point mouseDownPoint;
            private FlowLayoutPanel parentPanel;

            public TouchScroll(FlowLayoutPanel panel)
            {
                this.parentPanel = panel;
                AssignEvent(this.parentPanel);
            }

            private void AssignEvent(Control control)
            {
                control.MouseDown += Control_MouseDown;
                control.MouseMove += Control_MouseMove;
                foreach (Control child in control.Controls)
                {
                    AssignEvent(child);
                }
            }

            private void Control_MouseMove(object? sender, MouseEventArgs e)
            {
                if (e.Button != MouseButtons.Left)
                    return;

                System.Drawing.Point pointDifference = new System.Drawing.Point(Cursor.Position.X + this.mouseDownPoint.X, Cursor.Position.Y - this.mouseDownPoint.Y);

                if ((this.mouseDownPoint.X == Cursor.Position.X) && (this.mouseDownPoint.Y == Cursor.Position.Y))
                    return;

                System.Drawing.Point current = this.parentPanel.AutoScrollPosition;
                this.parentPanel.AutoScrollPosition = new System.Drawing.Point(Math.Abs(current.X) - pointDifference.X, Math.Abs(current.Y) - pointDifference.Y);
                this.mouseDownPoint = Cursor.Position;
            }

            private void Control_MouseDown(object? sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                    this.mouseDownPoint = Cursor.Position;
            }
        }


        private string fil = "";
        private void btn导入DXF_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                //Title = (MainFrm.Lang == 0) ? "选择DXF文件：" : "Select DXF file:",
                //Filter = (MainFrm.Lang == 0) ? "DXF文件 (*.dxf)|*.dxf：" : "DXF files (*.dxf)|*.dxf:",
                Title = Strings.Get("Library.DxfDialog.Title"),
                Filter = Strings.Get("Library.DxfDialog.Filter"),
                Multiselect = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    foreach (var item in openFileDialog.FileNames)
                    {
                        // 加载DXF文件
                        DxfDocument doc = DxfDocument.Load(item);
                        fil = item;
                        // 修改以下代码
                        foreach (EntityObject entity in doc.Entities.All)
                        {
                            switch (entity.Type)
                            {
                                case EntityType.Line:
                                    ProcessLine((netDxf.Entities.Line)entity);
                                    break;
                                case EntityType.MText:
                                    ProcessMtext((MText)entity);
                                    break;
                                case EntityType.Polyline2D:
                                    ProcessPolyline2D((Polyline2D)entity);
                                    break;
                                    // 添加其他实体类型的处理...
                            }
                        }

                        createFile();
                        pxList0.Clear();



                    }
                    MessageBox.Show(string.Format(System.Globalization.CultureInfo.InvariantCulture, Strings.Get("Library.Message.DxfImportSuccess"), openFileDialog.FileNames.Length));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format(System.Globalization.CultureInfo.InvariantCulture, Strings.Get("Library.Message.DxfImportFailed"), ex.Message));
                }
            }
            mf.LoadOrderFile(path1);
            refPreview();
        }
        public List<PointF> pxList0 = new List<PointF>();
        // 处理线段
        private void ProcessLine(netDxf.Entities.Line line)
        {
            System.Drawing.Point start = ConvertToGridPoint((float)line.StartPoint.X, (float)line.StartPoint.Y * (-1));
            System.Drawing.Point end = ConvertToGridPoint((float)line.EndPoint.X, (float)line.EndPoint.Y * (-1));
            pxList0.Add(start);
            pxList0.Add(end);
        }

        // 处理二维多段线（新版 Polyline2D）
        private void ProcessPolyline2D(Polyline2D polyline)
        {
            foreach (Polyline2DVertex vertex in polyline.Vertexes)
            {
                System.Drawing.PointF currentPoint = ConvertToGridPoint((float)vertex.Position.X, (float)vertex.Position.Y);

                // 跳过重复点
                if (MainFrm.QuickDrawList.Count == 0 ||
                    !MainFrm.QuickDrawList.Last().Equals(currentPoint))
                {
                    MainFrm.QuickDrawList.Add(currentPoint);
                }
            }
        }

        string sBasicInfo0 = "", sSquash0 = "";
        string[] sBasicInfo = new string[20], sSquash = new string[20];
        string[] sprit = new string[20];
        private void ProcessMtext(netDxf.Entities.MText mtext)
        {
            sprit = mtext.Value.ToString().Split('[', ']');



            if (sprit[0].Contains("Basic I"))
            {
                sBasicInfo0 = mtext.Value.ToString().Replace("\\P", "");

                sBasicInfo = sBasicInfo0.Split('[', ']');
                for (int i = 0; i < sBasicInfo.Length; i++)
                {
                    sBasicInfo[i] = sBasicInfo[i].Trim();
                }
            }
            else if (sprit[0].Contains("Squash:"))
            {
                sSquash0 = mtext.Value.ToString().Replace("\\P", "");
                sSquash = sSquash0.Split('[', ']');
                for (int i = 0; i < sSquash.Length; i++)
                {
                    sSquash[i] = sSquash[i].Trim();
                }
            }
        }

        private void createFile()
        {
            tmpOrder.pxList.Clear();
            tmpOrder.clr();

            for (int i = 0; i < pxList0.Count; i += 2)
            {
                tmpOrder.pxList.Add(pxList0[i]);
            }
            tmpOrder.pxList.Add(pxList0[pxList0.Count - 1]);

            CalLength(true);
            string s = Generate1Order("");

            string[] filesp = fil.Split('\\');

            string filePath = path1 + "\\" + filesp[filesp.Length - 1].Substring(0, filesp[filesp.Length - 1].Length - 4) + ".ini";
            bool fileExists = File.Exists(filePath);
            try
            {
                File.WriteAllText(filePath, s);
                Console.WriteLine(fileExists ? Strings.Get("Library.Log.FileOverwritten") : Strings.Get("Library.Log.FileCreated"));
            }
            catch (Exception ex)
            {
                string key = fileExists ? "Library.Log.SaveFileError" : "Library.Log.CreateFileError";
                Console.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, Strings.Get(key), ex.Message));
            }
        }

        private System.Drawing.Point ConvertToGridPoint(float x, float y)
        {
            // DXF单位转像素（示例：1 CAD单位 = GridToLength像素）
            int panelX = (int)(x);
            int panelY = (int)(y);

            // 网格对齐
            //if (bAlignGrid)
            //{
            //    panelX = (panelX / GridSize) * GridSize;
            //    panelY = (panelY / GridSize) * GridSize;
            //}

            return new System.Drawing.Point(panelX, panelY);
        }



        private string Generate1Order(string name)
        {
            foreach (var item in tmpOrder.lengAngle)
            {
                tmpOrder.Width += item.Length;
            }
            tmpOrder.Width += Convert.ToDouble(sSquash[3]) + Convert.ToDouble(sSquash[7]);


            string str = "Name:\"" + sBasicInfo[1] + "\",";
            str += " Width:" + tmpOrder.Width.ToString() + ",";
            str += " SheetLength:" + sBasicInfo[7] + ",";
            str += " Thickness:" + sBasicInfo[9] + ",";
            str += " MaterialName:\"" + sBasicInfo[5] + "\",";
            str += " Customer:\"" + sBasicInfo[3] + "\",";
            str += " Remark:\"" + sBasicInfo[11] + "\",";
            str += " isTaper:\"" + "False" + "\",";
            str += " TaperLength:" + "0" + ",";
            str += @" Reserve3:"" "",";
            str += @" Reserve4:"" "",";
            str += " SemiAutoList:";
            //for (int i = 0; i < CurtOrder.lstSemiAuto.Count; i++)
            //{
            //    str += CurtOrder.lstSemiAuto[i].行动类型.ToString() + "/" + CurtOrder.lstSemiAuto[i].折弯方向.ToString() + "/" + CurtOrder.lstSemiAuto[i].折弯角度.ToString() + "/";
            //    str += CurtOrder.lstSemiAuto[i].回弹值.ToString() + "/" + CurtOrder.lstSemiAuto[i].后挡位置.ToString() + "/" + CurtOrder.lstSemiAuto[i].抓取类型.ToString() + "/";
            //    str += CurtOrder.lstSemiAuto[i].松开高度.ToString() + "/" + CurtOrder.lstSemiAuto[i].翻板收缩值.ToString() + "/" + CurtOrder.lstSemiAuto[i].重新抓取.ToString() + ",";
            //}
            str += " pxList:";
            for (int i = 0; i < tmpOrder.pxList.Count; i++)
            {
                str += tmpOrder.pxList[i].X.ToString() + "/" + tmpOrder.pxList[i].Y.ToString() + ",";
            }
            str += "LengthAngle:";
            str += tmpOrder.lengAngle[0].Length.ToString() + "/" + tmpOrder.lengAngle[0].TaperWidth.ToString() + "/" + tmpOrder.lengAngle[0].Angle.ToString() + ",";
            int j = 1;
            while (tmpOrder.lengAngle[j].Angle > 0 || tmpOrder.lengAngle[j].Length > 0)
            {
                str += tmpOrder.lengAngle[j].Length.ToString() + "/" + tmpOrder.lengAngle[j].TaperWidth.ToString() + "/" + tmpOrder.lengAngle[j].Angle.ToString() + ",";
                j++;
            }
            str += tmpOrder.lengAngle[99].Length.ToString() + "/" + tmpOrder.lengAngle[99].TaperWidth.ToString() + "/" + tmpOrder.lengAngle[99].Angle.ToString() + ",";

            return str;
        }

        OrderType tmpOrder = new OrderType();
        double[] SlopeAngle = new double[100];
        private void CalLength(bool Fun)     //F:仅计算斜率  T:计算斜率+长度角度
        {
            int k = 1;
            double len = 0;
            double angle;
            double radians;
            int GridToLength = 1, GridSize = 1;
            if (Fun)
                for (int i = 0; i < tmpOrder.lengAngle.Length; i++)
                {
                    tmpOrder.lengAngle[i].Length = 0;
                    tmpOrder.lengAngle[i].Angle = 0;
                    tmpOrder.lengAngle[i].TaperWidth = 0;
                    tmpOrder.lengAngle[i].YinYang = false;
                }
            for (int i = 0; i < SlopeAngle.Length; i++)
            {
                SlopeAngle[i] = 0;
            }

            if (tmpOrder.pxList.Count > 0)
            {
                while (k < tmpOrder.pxList.Count)
                {
                    len = Math.Sqrt(Math.Pow((tmpOrder.pxList[k].X - tmpOrder.pxList[k - 1].X), 2) + Math.Pow((tmpOrder.pxList[k].Y - tmpOrder.pxList[k - 1].Y), 2));
                    radians = Math.Atan2((tmpOrder.pxList[k].Y - tmpOrder.pxList[k - 1].Y), tmpOrder.pxList[k].X - tmpOrder.pxList[k - 1].X);
                    angle = (-1) * radians * (180 / Math.PI);
                    if (Fun)
                        tmpOrder.lengAngle[k].Length = Math.Round(len * GridToLength / GridSize);

                    SlopeAngle[k] = angle;   //记录斜率角

                    k++;
                }
            }

            if (!Fun) return;

            int k2 = 1; double ang;
            while (k2 < tmpOrder.pxList.Count)
            {
                if (k2 == 1)    //第一个特殊处理
                    ang = 0;
                else if (SlopeAngle[k2] > 0)
                    ang = SlopeAngle[k2 - 1] + (180 - SlopeAngle[k2]);
                else
                    ang = SlopeAngle[k2 - 1] - (180 + SlopeAngle[k2]);

                if (ang >= 180 && ang <= 360)
                    tmpOrder.lengAngle[k2].Angle = Math.Round(ang - 360, 2);
                else if (ang >= -360 && ang <= -180)
                    tmpOrder.lengAngle[k2].Angle = Math.Round(ang + 360, 2);
                else
                    tmpOrder.lengAngle[k2].Angle = Math.Round(ang, 2);

                k2++;
            }

            //两端挤压处理

            tmpOrder.lengAngle[99].Angle = Convert.ToDouble(sSquash[5]);
            if (tmpOrder.lengAngle[99].Angle > 0)
                tmpOrder.lengAngle[99].Length = tmpOrder.lengAngle[99].TaperWidth = Convert.ToDouble(sSquash[7]);
            else
                tmpOrder.lengAngle[99].Length = tmpOrder.lengAngle[99].TaperWidth = 0.0;


            tmpOrder.lengAngle[0].Angle = Convert.ToDouble(sSquash[1]);
            if (tmpOrder.lengAngle[0].Angle > 0)
                tmpOrder.lengAngle[0].Length = tmpOrder.lengAngle[0].TaperWidth = Convert.ToDouble(sSquash[3]);
            else
                tmpOrder.lengAngle[0].Length = tmpOrder.lengAngle[0].TaperWidth = 0.0;


        }





    }
}
