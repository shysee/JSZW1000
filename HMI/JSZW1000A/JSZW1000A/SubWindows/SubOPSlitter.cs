using System.Drawing.Drawing2D;

namespace JSZW1000A.SubWindows
{
    public partial class SubOPSlitter : UserControl
    {
        private const int MaxSlitterRows = 8;

        MainFrm mf;

        private sealed class SlitterRowData
        {
            public SlitterRowData(double widthMm, int quantity)
            {
                WidthMm = widthMm;
                Quantity = quantity;
            }

            public double WidthMm { get; }
            public int Quantity { get; }
        }

        public SubOPSlitter(MainFrm fm1)
        {
            InitializeComponent(); setLang();
            this.mf = fm1;
        }

        private void ApplyInitialDisplayValues()
        {
            if (MainFrm.Hmi_rSlitter[0] > 0)
            {
                ReadData();
                return;
            }

            if (MainFrm.TryParseLengthByUnit(txb总宽.Text, DisplayLengthUnit.Millimeter, out double totalWidthMm))
                txb总宽.Text = MainFrm.FormatDisplayLength(totalWidthMm);
            if (MainFrm.TryParseLengthByUnit(txb单项宽.Text, DisplayLengthUnit.Millimeter, out double itemWidthMm))
                txb单项宽.Text = MainFrm.FormatDisplayLength(itemWidthMm);

            lb边角料长度_先.Text = MainFrm.FormatDisplayLengthWithUnit(0);
            lb边角料长度_后.Text = lb边角料长度_先.Text;
        }

        private void SubOPSlitter_Load(object sender, EventArgs e)
        {

            dataGridView1.ColumnHeadersVisible = false;
            if (dataGridView1.RowCount < 1)
            {
                btn列表_之后插入.PerformClick();
            }
            TextBoxInputBehavior.AttachSelectAllOnFocus(txb单项宽);
            TextBoxInputBehavior.AttachSelectAllOnFocus(txb单项数量);
            TextBoxInputBehavior.AttachSelectAllOnFocus(txb总宽);
            ApplyInitialDisplayValues();
        }

        public void setLang()
        {
            LocalizationManager.ApplyResources(this);
            if (MainFrm.Lang == 0)
            {
                label3.Font = label6.Font = label7.Font = lb首先.Font = lb最后.Font = label8.Font = label9.Font = label10.Font = label11.Font = new System.Drawing.Font("宋体", 11.25F);

                lb生产数量状态.Font = new System.Drawing.Font("宋体", 15F);
                btn装载材料.Font = btn分条机归原位.Font = new System.Drawing.Font("宋体", 11.25F);
                btn列表_之前插入.Font = btn列表_之后插入.Font = btn清除.Font = btn列表_上移.Font = btn列表_下移.Font = new System.Drawing.Font("宋体", 10F);
            }
            else
            {
                label3.Font = label6.Font = label7.Font = lb首先.Font = lb最后.Font = label8.Font = label9.Font = label10.Font = label11.Font = new System.Drawing.Font("Calibri", 11F);

                lb生产数量状态.Font = new System.Drawing.Font("Calibri", 15F);
                btn装载材料.Font = btn分条机归原位.Font = new System.Drawing.Font("Calibri", 11.25F);
                btn列表_之前插入.Font = btn列表_之后插入.Font = btn清除.Font = btn列表_上移.Font = btn列表_下移.Font = new System.Drawing.Font("Calibri", 11.25F);


            }
            string mm = MainFrm.GetLengthUnitLabel();
            label3.Text = label6.Text = label7.Text = Strings.Get("Slitter.Label.Offcut");
            lb首先.Text = Strings.Get("Slitter.Label.First");
            lb最后.Text = Strings.Get("Slitter.Label.Last");
            label8.Text = Strings.Get("Slitter.Label.Width");
            label11.Text = Strings.Get("Slitter.Label.Quantity");
            btn装载材料.Text = Strings.Get("Slitter.Action.LoadMaterial");
            btn分条机归原位.Text = Strings.Get("Slitter.Action.Home");
            btn列表_之前插入.Text = Strings.Get("Slitter.Action.InsertBefore");
            btn列表_之后插入.Text = Strings.Get("Slitter.Action.InsertAfter");
            btn清除.Text = Strings.Get("Slitter.Action.Delete");
            btn列表_上移.Text = Strings.Get("Slitter.Action.MoveUp");
            btn列表_下移.Text = Strings.Get("Slitter.Action.MoveDown");
            label9.Text = label10.Text = mm;
            RefreshSlitterRowsForDisplay();
        }

        private static string FormatSlitterRowText(double widthMm, int quantity)
        {
            return string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                Strings.Get("Slitter.RowTemplate"),
                MainFrm.FormatDisplayLength(widthMm),
                quantity.ToString(System.Globalization.CultureInfo.InvariantCulture),
                MainFrm.GetLengthUnitLabel());
        }

        private static void SetSlitterRow(DataGridViewRow row, double widthMm, int quantity)
        {
            row.Tag = new SlitterRowData(widthMm, quantity);
            row.Cells[0].Value = FormatSlitterRowText(widthMm, quantity);
        }

        private static bool TryReadSlitterRow(DataGridViewRow row, out double widthMm, out int quantity, out string widthText, out string quantityText)
        {
            widthMm = 0;
            quantity = 0;
            widthText = "";
            quantityText = "";

            if (row.Tag is SlitterRowData rowData)
            {
                widthMm = rowData.WidthMm;
                quantity = rowData.Quantity;
                widthText = MainFrm.FormatDisplayLength(widthMm);
                quantityText = quantity.ToString(System.Globalization.CultureInfo.InvariantCulture);
                return true;
            }

            string? rowText = Convert.ToString(row.Cells[0].Value);
            if (string.IsNullOrWhiteSpace(rowText))
                return false;

            var matches = System.Text.RegularExpressions.Regex.Matches(rowText, @"[-+]?\d+(?:[.,]\d+)?");
            if (matches.Count < 2)
                return false;

            widthText = matches[0].Value;
            quantityText = matches[matches.Count - 1].Value;
            DisplayLengthUnit savedUnit = DetectSlitterRowUnit(rowText);
            if (!MainFrm.TryParseLengthByUnit(widthText, savedUnit, out widthMm))
                return false;
            if (!int.TryParse(quantityText, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out quantity))
                return false;

            SetSlitterRow(row, widthMm, quantity);
            return true;
        }

        private static DisplayLengthUnit DetectSlitterRowUnit(string rowText)
        {
            if (rowText.Contains("mm", StringComparison.OrdinalIgnoreCase) || rowText.Contains("毫米", StringComparison.OrdinalIgnoreCase))
                return DisplayLengthUnit.Millimeter;
            if (rowText.Contains("in", StringComparison.OrdinalIgnoreCase) || rowText.Contains("英寸", StringComparison.OrdinalIgnoreCase) || rowText.Contains("\"", StringComparison.Ordinal))
                return DisplayLengthUnit.Inch;

            return DisplayUnitManager.CurrentDisplayUnit;
        }

        private void RefreshSlitterRowsForDisplay()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (TryReadSlitterRow(row, out double widthMm, out int quantity, out _, out _))
                    SetSlitterRow(row, widthMm, quantity);
            }
        }

        private static int ParseQuantityOrZero(string? text)
        {
            return int.TryParse(text, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out int quantity)
                ? quantity
                : 0;
        }

        private void ShowSlitterOperationError(string actionLabelKey, Exception ex)
        {
            string detail = string.IsNullOrWhiteSpace(ex.Message) ? ex.GetType().Name : ex.Message;
            MessageBox.Show(
                string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    Strings.Get("Slitter.Error.ActionFailed"),
                    Strings.Get(actionLabelKey),
                    detail),
                Strings.Get("Common.ErrorTitle"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }






        private void timer500ms_Tick(object sender, EventArgs e)
        {
            pnl边角料选先.Visible = MainFrm.Hmi_bArray[81];
            pnl边角料选后.Visible = !MainFrm.Hmi_bArray[81];
            lb首先.ForeColor = MainFrm.Hmi_bArray[81] ? Color.FromArgb(96, 176, 255) : Color.White;
            lb最后.ForeColor = !MainFrm.Hmi_bArray[81] ? Color.FromArgb(96, 176, 255) : Color.White;
            sw边角料选.BackgroundImage = MainFrm.Hmi_bArray[81] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            pnlSlitter.BackgroundImage = (MainFrm.Hmi_iArray[20] == 5) ? global::JSZW1000A.Properties.Resources.Slit5 : global::JSZW1000A.Properties.Resources.Slit6;
            lb生产数量状态.Text = string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                Strings.Get("Slitter.ProductionStatus"),
                MainFrm.Hmi_iArray[80],
                MainFrm.Hmi_iArray[81],
                MainFrm.Hmi_iArray[82]);

        }

        bool InsertRow(int i)
        {
            if (dataGridView1.RowCount >= MaxSlitterRows)
                return false;

            //dataGridView1.Rows.Clear();
            DataGridViewRow dr = new DataGridViewRow();
            dr.CreateCells(dataGridView1);
            SetSlitterRow(dr, 0, 1);

            //添加的行作为第一行
            dataGridView1.Rows.Insert(i, dr);
            return true;

        }

        private void btn列表_之前插入_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0 && dataGridView1.CurrentRow != null)
            {
                int currentIndex = dataGridView1.CurrentRow.Index;
                if (!InsertRow(currentIndex))
                    return;
                //更改选中行
                dataGridView1.ClearSelection();
                dataGridView1.Rows[currentIndex].Selected = true;
                dataGridView1.CurrentCell = dataGridView1.Rows[currentIndex].Cells[0];
            }
            else
            {
                if (!InsertRow(0))
                    return;

            }
            FillTxbox();
        }

        private void btn列表_之后插入_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0 && dataGridView1.CurrentRow != null)
            {
                int currentIndex = dataGridView1.CurrentRow.Index;
                if (!InsertRow(currentIndex + 1))
                    return;
                //更改选中行
                dataGridView1.ClearSelection();
                dataGridView1.Rows[currentIndex + 1].Selected = true;
                dataGridView1.CurrentCell = dataGridView1.Rows[currentIndex + 1].Cells[0];
            }
            else
            {
                if (!InsertRow(0))
                    return;
            }
            FillTxbox();
        }

        private void btn清除_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 1 && dataGridView1.CurrentRow != null)
            {
                dataGridView1.Rows.Remove(dataGridView1.CurrentRow);
                FillTxbox();
            }

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
                ShowSlitterOperationError("Slitter.Action.MoveUp", ex);
            }
            FillTxbox();
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
                ShowSlitterOperationError("Slitter.Action.MoveDown", ex);
            }
            FillTxbox();

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            FillTxbox();

        }

        void FillTxbox()
        {
            if (dataGridView1.SelectedRows.Count == 0)
                return;

            int index = dataGridView1.SelectedRows[0].Index;
            if (!TryReadSlitterRow(dataGridView1.Rows[index], out _, out _, out string widthText, out string quantityText))
                return;

            if (MainFrm.TryParseLengthByUnit(widthText, DisplayUnitManager.CurrentDisplayUnit, out double widthMm))
                txb单项宽.Text = MainFrm.FormatDisplayLength(widthMm);
            else
                txb单项宽.Text = widthText;
            txb单项数量.Text = quantityText;



        }

        private void txb单项宽_KeyDown(object sender, KeyEventArgs e)
        {
            // 判断：如果输入的是回车键
            if (e.KeyCode == Keys.Enter)
            {
                int index = dataGridView1.SelectedRows[0].Index;
                double widthMm = MainFrm.ParseDisplayLengthOrZero(txb单项宽.Text);
                int quantity = ParseQuantityOrZero(txb单项数量.Text);
                txb单项宽.Text = MainFrm.FormatDisplayLength(widthMm);
                SetSlitterRow(dataGridView1.Rows[index], widthMm, quantity);

                CreateSlitterDraw();
            }
        }

        private void txb单项数量_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int index = dataGridView1.SelectedRows[0].Index;
                double widthMm = MainFrm.ParseDisplayLengthOrZero(txb单项宽.Text);
                int quantity = ParseQuantityOrZero(txb单项数量.Text);
                txb单项宽.Text = MainFrm.FormatDisplayLength(widthMm);
                SetSlitterRow(dataGridView1.Rows[index], widthMm, quantity);

                CreateSlitterDraw();
            }
        }


        private void btn边角料选_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[81] = !MainFrm.Hmi_bArray[81];
            sw边角料选.BackgroundImage = MainFrm.Hmi_bArray[81] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            mf.AdsWritePlc();
            CreateSlitterDraw();
        }



        float[] List_Width = new float[100];
        int List_count = 0;
        float List_TotalWidth = 0;

        void CreateSlitterDraw()
        {
            for (int i = 0; i < 100; i++)
                List_Width[i] = 0;
            List_count = 0;
            List_TotalWidth = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!TryReadSlitterRow(row, out double widthMm, out int quantity, out _, out _) || quantity == 0 || widthMm == 0)
                    continue;
                for (int i = 0; i < quantity; i++)
                {
                    List_Width[List_count] = (float)widthMm;
                    List_TotalWidth += List_Width[List_count];
                    List_count++;
                }
            }
            double totalWidthMm = MainFrm.ParseDisplayLengthOrZero(txb总宽.Text);
            lb边角料长度_先.Text = MainFrm.FormatDisplayLengthWithUnit(totalWidthMm - List_TotalWidth);
            lb边角料长度_后.Text = lb边角料长度_先.Text;
            draw();
        }

        void ReadData()
        {
            txb总宽.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rSlitter[0]);

            dataGridView1.Rows.Clear();

            int i = 0;
            while (i < MaxSlitterRows && MainFrm.Hmi_rSlitter[22 + i] > 0)
            {
                DataGridViewRow dr = new DataGridViewRow();
                dr.CreateCells(dataGridView1);
                SetSlitterRow(
                    dr,
                    MainFrm.Hmi_rSlitter[10 + i],
                    (int)Math.Round(MainFrm.Hmi_rSlitter[22 + i]));
                //添加的行作为第一行
                dataGridView1.Rows.Insert(i, dr);
                i++;
            }



        }

        void Dnload()
        {
            for (int j = 0; j < MainFrm.Hmi_rSlitter.Length; j++)
                MainFrm.Hmi_rSlitter[j] = 0;

            int i = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (i >= MaxSlitterRows)
                    break;
                if (!TryReadSlitterRow(row, out double widthMm, out int quantity, out _, out _) || quantity == 0 || widthMm == 0)
                    continue;
                MainFrm.Hmi_rSlitter[10 + i] = (float)widthMm;
                MainFrm.Hmi_rSlitter[22 + i] = quantity;

                i++;
            }

            MainFrm.Hmi_rSlitter[0] = MainFrm.ParseDisplayLengthFloatOrZero(txb总宽.Text);
            MainFrm.Hmi_rSlitter[1] = (MainFrm.Hmi_rSlitter[0] - List_TotalWidth);
            MainFrm.Hmi_rSlitter[2] = (float)i;

            mf.AdsWritePlc();

        }
        private void button1_Click(object sender, EventArgs e)
        {

        }

        void draw()
        {
            int T总板宽 = Math.Max(1, Convert.ToInt32(MainFrm.ParseDisplayLengthOrZero(txb总宽.Text)));
            int 分条总宽 = Convert.ToInt32(List_TotalWidth);

            Graphics g1 = pictureBox1.CreateGraphics();
            g1.Clear(Color.FromArgb(70, 70, 70));
            GraphicsUnit units = GraphicsUnit.Pixel;
            RectangleF srcRect = new RectangleF(0, 0, 605, 241);
            g1.DrawImage((Image)global::JSZW1000A.Properties.Resources.bg_Slitter绘图2, 0, 0, srcRect, units);
            g1.SmoothingMode = SmoothingMode.AntiAlias;
            //高质量，低速度绘制
            g1.CompositingQuality = CompositingQuality.HighQuality;


            Int32 上X = 150 + 1, 上Y = 5, 下X = 8, 下Y = 240 - 5;
            var PointArray = new Point[4] { new Point(上X, 0 + 上Y), new Point(0 + 下X, 下Y), new Point(分条总宽 * 600 / T总板宽 - 5, 下Y), new Point(分条总宽 * 300 / T总板宽 + 上X, 上Y) };
            if (MainFrm.Hmi_bArray[81])
                PointArray = new Point[4] { new Point(605 - 上X, 0 + 上Y), new Point(605 - 下X, 下Y), new Point(605 - (分条总宽 * 600 / T总板宽 - 5), 下Y), new Point(605 - (分条总宽 * 300 / T总板宽 + 上X), 上Y) };
            //g1.DrawPolygon(myPen, PointArray);
            SolidBrush blueBrush = new SolidBrush(Color.FromArgb(70, 70, 70));
            g1.FillPolygon(blueBrush, PointArray);


            Pen myPen = new Pen(Color.FromArgb(248, 162, 115), 3);
            int tol = 0;
            for (int i = 0; i < List_count; i++)
            {

                if (!MainFrm.Hmi_bArray[81])
                {
                    tol += Convert.ToInt32(List_Width[i]);
                    g1.DrawLine(myPen, tol * 600 / T总板宽 - 2, 下Y, tol * 300 / T总板宽 + 150 + 2, 0 + 上Y);
                }
                else
                {
                    tol += Convert.ToInt32(List_Width[List_count - i - 1]);
                    g1.DrawLine(myPen, 605 - (tol * 600 / T总板宽 - 2), 下Y, 605 - (tol * 300 / T总板宽 + 150 + 2), 0 + 上Y);
                }

            }

        }


        private void pnlSlitter_MouseUp(object sender, MouseEventArgs e)
        {
            //MainFrm.Hmi_bArray[80] = true;
            //mf.AdsWritePlc1Bit(80, MainFrm.Hmi_bArray[80]);
        }

        private void pnlSlitter_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainFrm.Hmi_iArray[0] == 5)
                MainFrm.Hmi_iArray[0] = 3;
            else
                MainFrm.Hmi_iArray[0] = 5;
            mf.AdsWritePlc1Int(0, MainFrm.Hmi_iArray[0]);
        }

        private void pnlSlitter_Click(object sender, EventArgs e)
        {
            Dnload();
        }

        private void btn装载材料_MouseDown(object sender, MouseEventArgs e)
        {
            MainFrm.Hmi_bArray[82] = true;
            mf.AdsWritePlc1Bit(82, MainFrm.Hmi_bArray[82]);
        }

        private void btn装载材料_MouseUp(object sender, MouseEventArgs e)
        {
            MainFrm.Hmi_bArray[82] = false;
            mf.AdsWritePlc1Bit(82, MainFrm.Hmi_bArray[82]);
        }

        private void btn分条机归原位_MouseDown(object sender, MouseEventArgs e)
        {
            MainFrm.Hmi_bArray[83] = true;
            mf.AdsWritePlc1Bit(83, MainFrm.Hmi_bArray[83]);
        }

        private void btn分条机归原位_MouseUp(object sender, MouseEventArgs e)
        {
            MainFrm.Hmi_bArray[83] = false;
            mf.AdsWritePlc1Bit(83, MainFrm.Hmi_bArray[83]);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            panel2.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
        }

        private void txb单项宽_Click(object sender, EventArgs e)
        {
            txbActiveTxb = txb单项宽;
            PopCal();
        }
        private FrmCalculator? dlgCal;
        private void PopCal()
        {
            if (dlgCal == null || dlgCal.IsDisposed)
            {
                dlgCal = new FrmCalculator(this);
                dlgCal.StartPosition = FormStartPosition.Manual;
                dlgCal.Location = new Point(890, 270);
                dlgCal.TopMost = true;
                dlgCal.Show();
            }
        }

        private Object? txbActiveTxb;
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


        }

        private void txb单项数量_Click(object sender, EventArgs e)
        {
            txbActiveTxb = txb单项数量;
            PopCal();
        }

        private void txb总宽_Click(object sender, EventArgs e)
        {
            txbActiveTxb = txb总宽;
            PopCal();
        }

        private void txb总宽_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double totalWidthMm = MainFrm.ParseDisplayLengthOrZero(txb总宽.Text);
                txb总宽.Text = MainFrm.FormatDisplayLength(totalWidthMm);
                CreateSlitterDraw();
            }
        }

        private void SubOPSlitter_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                timer200ms.Enabled = true;
            }
        }

        private void timer200ms_Tick(object sender, EventArgs e)
        {
            CreateSlitterDraw();
            timer200ms.Enabled = false;
        }

        private void txb单项宽_TextChanged(object sender, EventArgs e)
        {
            MainFrm.Hmi_iArray[0] = 2;
            mf.AdsWritePlc1Int(0, MainFrm.Hmi_iArray[0]);
        }
    }
}
