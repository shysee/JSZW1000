using Newtonsoft.Json;
using System.Data;

namespace JSZW1000A
{
    public partial class FrmFeed : Form
    {
        MainFrm mf;

        public FrmFeed(MainFrm mf1)
        {
            InitializeComponent();
            this.mf = mf1;
            setLang();
        }

        private void setLang()
        {
            LocalizationManager.ApplyResources(this);

            if (MainFrm.Lang == 0)
            {
                btn列表插入.Font = btn列表_清除.Font = new System.Drawing.Font("宋体", 10F);
                btn料架后退.Font = btn料架前进.Font = btn移动装料架.Font = btn确认.Font = button1.Font = new System.Drawing.Font("宋体", 11.25F);
                label117.Font = label119.Font = label5.Font = label7.Font = new System.Drawing.Font("微软雅黑", 12F);
                label9.Font = label6.Font = new System.Drawing.Font("宋体", 11.25F);
            }
            else
            {
                btn列表插入.Font = btn列表_清除.Font = btn料架后退.Font = btn料架前进.Font = btn移动装料架.Font = btn确认.Font = button1.Font = new System.Drawing.Font("Calibri", 11F);
                label117.Font = label119.Font = label5.Font = label7.Font = label9.Font = label6.Font = new System.Drawing.Font("Calibri", 11F);
            }

            string unit = MainFrm.GetLengthUnitLabel();
            Index.HeaderText = Strings.Get("Feed.Column.Index");
            Length.HeaderText = Strings.Format("Feed.Column.Length", unit);
            WidthColumn.HeaderText = Strings.Format("Feed.Column.Width", unit);
            Thickness.HeaderText = Strings.Format("Feed.Column.Thickness", unit);
            Quantity.HeaderText = Strings.Get("Feed.Column.Quantity");
            GroupsNeeded.HeaderText = Strings.Get("Feed.Column.Groups");
            Batch.HeaderText = Strings.Get("Feed.Column.Batch");
            Total.HeaderText = Strings.Get("Feed.Column.Total");

            btn列表插入.Text = Strings.Get("Feed.Action.Insert");
            btn列表_清除.Text = Strings.Get("Feed.Action.Delete");
            btn确认.Text = Strings.Get("Feed.Action.Confirm");
            button1.Text = Strings.Get("Feed.Action.Back");
            btn料架前进.Text = Strings.Get("Feed.Action.RackForward");
            btn料架后退.Text = Strings.Get("Feed.Action.RackBackward");
            btn移动装料架.Text = Strings.Get("Feed.Action.MoveRack");

            label1.Text = Strings.Format("Feed.Label.SuctionGroup", 1);
            label2.Text = Strings.Format("Feed.Label.SuctionGroup", 2);
            label3.Text = Strings.Format("Feed.Label.SuctionGroup", 3);
            label4.Text = Strings.Format("Feed.Label.SuctionGroup", 4);
            label119.Text = Strings.Format("Feed.Label.Position", 1);
            label117.Text = Strings.Format("Feed.Label.Position", 2);
            label7.Text = Strings.Format("Feed.Label.Position", 3);
            label5.Text = Strings.Format("Feed.Label.Position", 4);
            label9.Text = label6.Text = unit;
            Text = Strings.Get("Feed.Title", "Feed");
        }

        private static string FormatFeedLength(double mm)
        {
            return MainFrm.FormatDisplayLengthWithUnit(mm);
        }


        // 后夹送位置和间距参数
        private const double PositionT1 = 783.5;
        private const double PositionT2 = 1303.5;
        private const double PositionT3 = 2098.5;
        private const double PositionT4 = 2618.5;
        private const double PositionT5 = 3685;
        private const double PositionT6 = 5000;
        private const double PositionT7 = 6315;
        private const double PositionT8 = 7358.5;
        private const double PositionT8B = 7878.5;
        private const double PositionT9 = 8673.5;
        private const double PositionT9B = 9193.5;

        private const int MaxLength = 10000;

        private double ReleasePos1, ReleasePos2, ReleasePos3, ReleasePos4;//释放板材位置1,2,3,4

        public class PlateInfo
        {
            public int Index { get; set; }
            public int Width { get; set; }
            public int Length { get; set; }
            public double Thickness { get; set; }
            public int Quantity { get; set; }
            public int Groups { get; set; } // 每块板材所需的组数
            public int Batch { get; set; } // 一次折板需要送料的批次
            public int Total => Quantity * Groups; // 总数 = 数量 × 组数
        }

        private class PositionResult
        {
            public double Value { get; set; }
            public bool IsValid { get; set; }
        }

        List<PlateInfo> plates = new List<PlateInfo>();
        int FlodNum = 0;

        private void FrmFeed_Load(object sender, EventArgs e)
        {
            txb进料定位.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[57]);
            pos1.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[90]);
            pos2.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[91]);
            pos3.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[92]);
            pos4.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[93]);
            LoadDataFromFile(); // 加载数据

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            txb进料位置.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[40]);
            sw吸盘1.BackgroundImage = MainFrm.Hmi_bArray[55] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            sw吸盘2.BackgroundImage = MainFrm.Hmi_bArray[56] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            sw吸盘3.BackgroundImage = MainFrm.Hmi_bArray[57] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            //sw吸盘4.BackgroundImage = MainFrm.Hmi_bArray[58] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
        }

        private void btn确认_Click(object sender, EventArgs e)
        {
            try
            {
                var plates = new List<PlateInfo>();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow) continue;

                    // 改进数据转换，使用TryParse避免格式错误
                    if (!int.TryParse(row.Cells["Index"].Value?.ToString(), out int index))
                    {
                        MessageBox.Show(Strings.Get("Feed.Error.InvalidIndex"));
                        return;
                    }

                    if (!MainFrm.TryParseDisplayLength(row.Cells["Width"].Value?.ToString(), out double widthMm))
                    {
                        MessageBox.Show(string.Format(System.Globalization.CultureInfo.InvariantCulture, Strings.Get("Feed.Error.InvalidWidth"), index));
                        return;
                    }
                    int width = Convert.ToInt32(Math.Round(widthMm, MidpointRounding.AwayFromZero));

                    if (!MainFrm.TryParseDisplayLength(row.Cells["Length"].Value?.ToString(), out double lengthMm))
                    {
                        MessageBox.Show(string.Format(System.Globalization.CultureInfo.InvariantCulture, Strings.Get("Feed.Error.InvalidLength"), index));
                        return;
                    }
                    int length = Convert.ToInt32(Math.Round(lengthMm, MidpointRounding.AwayFromZero));

                    if (!int.TryParse(row.Cells["Quantity"].Value?.ToString(), out int quantity))
                    {
                        MessageBox.Show(string.Format(System.Globalization.CultureInfo.InvariantCulture, Strings.Get("Feed.Error.InvalidQuantity"), index));
                        return;
                    }

                    if (!int.TryParse(row.Cells["GroupsNeeded"].Value?.ToString(), out int groups))
                    {
                        MessageBox.Show(string.Format(System.Globalization.CultureInfo.InvariantCulture, Strings.Get("Feed.Error.InvalidGroups"), index));
                        return;
                    }

                    if (!int.TryParse(row.Cells["Batch"].Value?.ToString(), out int batch))
                    {
                        MessageBox.Show(string.Format(System.Globalization.CultureInfo.InvariantCulture, Strings.Get("Feed.Error.InvalidBatch"), index));
                        return;
                    }

                    var plate = new PlateInfo
                    {
                        Index = index,
                        Width = width,
                        Length = length,
                        Thickness = MainFrm.ParseDisplayLengthOrZero(row.Cells["Thickness"].Value?.ToString()),
                        Quantity = quantity,
                        Groups = groups,
                        Batch = batch
                    };

                    // 校验长度
                    if (plate.Length < 600 || plate.Length > 10000)
                    {
                        MessageBox.Show(Strings.Format("Feed.Error.LengthOutOfRange", plate.Index, FormatFeedLength(600), FormatFeedLength(10000)));
                        return;
                    }

                    // 校验宽度
                    if (plate.Width > 1200)
                    {
                        MessageBox.Show(Strings.Format("Feed.Error.WidthTooLarge", plate.Index, FormatFeedLength(1200)));
                        return;
                    }

                    // 校验组数
                    if (plate.Groups < 1 || plate.Groups > 3)
                    {
                        MessageBox.Show(string.Format(System.Globalization.CultureInfo.InvariantCulture, Strings.Get("Feed.Error.GroupsOutOfRange"), plate.Index));
                        return;
                    }

                    // 更新表格数据
                    row.Cells["Total"].Value = plate.Total;
                    FlodNum = plate.Groups * plate.Batch;
                    var positions = CalculateReleasePos(plate.Length, FlodNum);

                    ReleasePos1 = positions.ReleasePos1;
                    ReleasePos2 = positions.ReleasePos2;
                    ReleasePos3 = positions.ReleasePos3;
                    ReleasePos4 = positions.ReleasePos4;

                    pos1.Text = MainFrm.FormatDisplayLength(ReleasePos1);
                    pos2.Text = MainFrm.FormatDisplayLength(ReleasePos2);
                    pos3.Text = MainFrm.FormatDisplayLength(ReleasePos3);
                    pos4.Text = MainFrm.FormatDisplayLength(ReleasePos4);
                    plates.Add(plate);
                }
                AssignFirstRowToHmiArray();
                mf.AdsWritePlc();
                SaveDataToFile(); // 确认时额外保存
                MessageBox.Show(Strings.Get("Feed.Success.Assigned"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(System.Globalization.CultureInfo.InvariantCulture, Strings.Get("Feed.Error.Generic"), ex.Message));
            }
        }



        // 核心计算方法：返回4个释放位置（0表示未使用）
        public (double ReleasePos1, double ReleasePos2, double ReleasePos3, double ReleasePos4)
            CalculateReleasePos(int length, int groupCount)
        {
            // 基础校验：长度/组数范围
            if (length < 600 || length > 10000)
                throw new ArgumentException(Strings.Format("Feed.Error.PlateLengthRange60010000", FormatFeedLength(600), FormatFeedLength(10000)));
            if (groupCount < 1 || groupCount > 4)
                throw new ArgumentException(Strings.Get("Feed.Error.PlateCountRange1To4"));

            return groupCount switch
            {
                1 => CalculateSingleGroup(length),
                2 => CalculateTwoGroups(length),
                3 => CalculateThreeGroups(length),
                4 => CalculateFourGroups(length),
                _ => throw new InvalidOperationException(Strings.Get("Feed.Error.InvalidGroupCount"))
            };
        }
        #region 1块板材计算逻辑（含T3-T4强制覆盖）
        private (double, double, double, double) CalculateSingleGroup(int length)
        {
            if (length < 600 || length > 10000)
                throw new ArgumentException(Strings.Get("Feed.Error.InvalidPlateLength"));

            if (length < 5000)
            {
                // 强制覆盖T3-T4夹送组，满足T < 2088.5且H > 2628.5
                double t3 = PositionT3;
                double t4 = PositionT4;
                double minH = t4 + 10;  // 头部≥2628.5
                double maxT = t3 - 10;  // 尾部≤2088.5

                // 计算最小长度：L_min = minH - maxT = 2628.5 - 2088.5 = 540mm（已由L≥600保证）
                if (length < 540)
                    throw new ArgumentException(Strings.Get("Feed.Error.LengthCannotCoverT3T4"));

                // 以T3-T4中心为基准，调整至满足约束
                double groupCenter = (t3 + t4) / 2;  // 2358.5mm
                double idealH = groupCenter + length / 2;  // 理想头部位置

                // 应用约束：H ≥ minH 且 T = H - L ≤ maxT
                if (idealH < minH)
                {
                    // 头部不足下限，需右移至minH
                    idealH = minH;
                }
                else if (idealH - length > maxT)
                {
                    // 尾部超出上限，需左移至T=maxT → H=maxT+L
                    idealH = maxT + length;
                }

                // 验证最终位置有效性
                if (idealH < minH || idealH - length > maxT)
                    throw new InvalidOperationException(Strings.Get("Feed.Error.CannotPlaceOnT3T4"));

                return (idealH, 0, 0, 0);  // 返回头部位置
            }
            else
            {
                // 长板材逻辑（L≥5000mm），以设备中点5000mm为中心
                double center = 5000;
                double headPos = center + length / 2;  // 头部位置=中心+L/2
                //headPos = Math.Max(headPos, 10000);  // 头部≤10000
                //headPos = Math.Min(0, length);  // 尾部≥0 → 头部≥L
                return (headPos, 0, 0, 0);
            }
        }
        #endregion

        #region 2块板材计算逻辑
        private (double, double, double, double) CalculateTwoGroups(int length)
        {
            if (length > 5000)  // 长板材无法双组放置
                throw new ArgumentException(Strings.Format("Feed.Error.TwoGroupsLengthMax5000", FormatFeedLength(5000)));

            // 候选夹送组对（左→右，不重叠）
            var groupPairs = new (double TRight1, double TLeft1, double TRight2, double TLeft2)[]
            {
                (PositionT1, PositionT2, PositionT3, PositionT4),    // 左侧两组
                (PositionT2, PositionT3, PositionT5, PositionT6),    // 跨区组合
                (PositionT2, PositionT4, PositionT6, PositionT7)    // 中部两组
            };

            foreach (var pair in groupPairs)
            {
                var pos1 = CalculatePosition(length, pair.TRight1, pair.TLeft1);
                var pos2 = CalculatePosition(length, pair.TRight2, pair.TLeft2);

                if (pos1.IsValid && pos2.IsValid &&
                    pos2.Value - (pos1.Value + length) >= 50)  // 头尾间距≥50
                {
                    return (pos1.Value, pos2.Value, 0, 0);
                }
            }

            throw new InvalidOperationException(Strings.Get("Feed.Error.NoTwoGroupPosition"));
        }
        #endregion

        #region 3块板材计算逻辑
        private (double, double, double, double) CalculateThreeGroups(int length)
        {
            if (length < 800 || length > 2500)
                throw new ArgumentException(Strings.Format("Feed.Error.ThreeGroupsLengthRange8002500", FormatFeedLength(800), FormatFeedLength(2500)));

            // 候选夹送组对（左→中→右）
            var groupTriples = new (double T1, double T2, double T3, double T4, double T5, double T6)[]
            {
              // T1-T2, T3-T4, T8-T8B
            (PositionT1, PositionT2, PositionT3, PositionT4, PositionT8, PositionT8B),
            // T2-T3, T5-T6, T9-T9B
            (PositionT2, PositionT3, PositionT5, PositionT6, PositionT9, PositionT9B)
            };

            foreach (var triple in groupTriples)
            {
                var pos1 = CalculatePosition(length, triple.T1, triple.T2);
                var pos2 = CalculatePosition(length, triple.T3, triple.T4);
                var pos3 = CalculatePosition(length, triple.T5, triple.T6);

                if (pos1.IsValid && pos2.IsValid && pos3.IsValid &&
                    pos2.Value - (pos1.Value + length) >= 50 &&  // 1→2间距
                    pos3.Value - (pos2.Value + length) >= 50)     // 2→3间距
                {
                    return (pos1.Value, pos2.Value, pos3.Value, 0);
                }
            }

            throw new InvalidOperationException(Strings.Get("Feed.Error.NoThreeGroupPosition"));
        }
        #endregion

        #region 4块板材计算逻辑
        private (double, double, double, double) CalculateFourGroups(int length)
        {
            if (length < 600 || length >= 1300)
                throw new ArgumentException(Strings.Format("Feed.Error.FourGroupsLengthRange6001300", FormatFeedLength(600), FormatFeedLength(1300)));

            // 固定使用密集夹送组对（间距520）
            var pos1 = CalculatePosition(length, PositionT1, PositionT2);
            var pos2 = CalculatePosition(length, PositionT3, PositionT4);
            var pos3 = CalculatePosition(length, PositionT8, PositionT8B);
            var pos4 = CalculatePosition(length, PositionT9, PositionT9B);

            if (!pos1.IsValid || !pos2.IsValid || !pos3.IsValid || !pos4.IsValid)
                throw new InvalidOperationException(Strings.Get("Feed.Error.DenseGroupPositionInvalid"));

            // 验证组间间隔（1→2, 2→3, 3→4）
            if (pos2.Value - (pos1.Value + length) < 50 ||
                pos3.Value - (pos2.Value + length) < 50 ||
                pos4.Value - (pos3.Value + length) < 50)
            {
                throw new InvalidOperationException(Strings.Get("Feed.Error.FourGroupSpacingInsufficient"));
            }

            return (pos1.Value, pos2.Value, pos3.Value, pos4.Value);
        }
        #endregion

        // 辅助方法：计算板材释放位置（中心对准+边缘安全距离）
        private (bool IsValid, double Value) CalculatePosition(int length, double tRight, double tLeft)
        {
            // 夹送组间距
            double groupSpan = tLeft - tRight;

            // 板材需覆盖夹送组：H ≥ tLeft + 10（头部安全距离）
            // T = H - length ≤ tRight - 10（尾部安全距离）
            double minH = tLeft + 10;                 // 最小头部位置
            double maxH = tRight - 10 + length;       // 最大头部位置（T ≤ tRight - 10）

            if (minH > maxH) return (false, 0);      // 长度不足覆盖夹送组

            // 优先中心对准（板材中心=夹送组中心）
            double groupCenter = (tLeft + tRight) / 2;
            double plateCenter = groupCenter;
            double headPos = plateCenter + length / 2;  // 头部位置 = 中心 + L/2

            // 确保头部位置在安全范围内
            if (headPos > minH || headPos < maxH)
                headPos = Math.Clamp(headPos, minH, maxH);  // 超出则取边界

            // 验证最终位置有效性
            double tailPos = headPos - length;  // 计算尾部位置用于验证
            if (headPos < tLeft + 10 || tailPos > tRight - 10)
                return (false, 0);

            return (true, headPos);
        }
        //public (double ReleasePos1, double ReleasePos2, double ReleasePos3, double ReleasePos4) CalculateReleasePos(int length, int groupCount)
        //{
        //    if (length < 600 || length > 4000)
        //        throw new ArgumentException("板材长度无效，必须介于705mm和4000mm之间");
        //    // 新增组数判断逻辑
        //    if (groupCount == 1)//单组逻辑
        //    {
        //        if (length < 795)
        //        {
        //            // 计算夹送组1-2位置
        //            var group12 = CalculatePosition(length, PositionT1, PositionT2);

        //            return (group12.Value, 0, 0,0);
        //        }
        //        else
        //        {
        //            // 计算夹送组2-3位置
        //            var group23 = CalculatePosition(length, PositionT2, PositionT3);

        //            return (group23.Value, 0, 0, 0);
        //        }
        //    }

        //    if (groupCount == 2)//2组逻辑
        //    {
        //        if (length > 1800)
        //            throw new ArgumentException("双组放置时板材长度必须小于1800mm");

        //        // 计算夹送组1-2位置
        //        var group12 = CalculatePosition(length, PositionT1, PositionT2);
        //        // 计算夹送组3-4位置
        //        var group34 = CalculatePosition(length, PositionT3, PositionT4);

        //        if (!group12.IsValid || !group34.IsValid)
        //            throw new InvalidOperationException("夹送组位置无效");

        //        // 验证位置不冲突
        //        if (group34.Value - group12.Value < length)
        //            throw new InvalidOperationException("两组位置间隔不足，无法避免重叠");

        //        return (group12.Value, group34.Value, 0,0);
        //    }

        //    throw new InvalidOperationException("无法找到满足条件的夹送组位置。");

        //}

        //private PositionResult CalculatePosition(int length, double C_i, double C_j)
        //{
        //    double C_center = (C_i + C_j) / 2.0;
        //    double X = C_center + (length / 2.0) - OffsetToHead;

        //    // 计算允许范围
        //    int lowerBound = Math.Max((int)(C_j - OffsetToHead), length - OffsetToHead);
        //    int upperBound = Math.Min((int)(C_i + length - OffsetToHead), 3950);

        //    // 调整到有效范围
        //    X = Math.Clamp(X, lowerBound, upperBound);

        //    // 验证夹持条件
        //    double H = X + OffsetToHead;
        //    double T = H - length;

        //    return new PositionResult
        //    {
        //        Value = (double)Math.Round(X),
        //        IsValid = H >= C_j && T <= C_i && T >= 0 && H <= 4000
        //    };
        //}

        private void btn列表插入_Click(object sender, EventArgs e)
        {
            try
            {
                // 结束当前编辑（避免未提交的数据导致异常）
                if (dataGridView1.CurrentCell != null && dataGridView1.IsCurrentCellInEditMode)
                {
                    dataGridView1.EndEdit();
                }

                // 添加新行并自动生成序号
                int newIndex = 1;
                if (dataGridView1.Rows.Count > 0 && !dataGridView1.Rows[0].IsNewRow)
                {
                    newIndex = dataGridView1.Rows.Cast<DataGridViewRow>()
                        .Where(r => !r.IsNewRow)
                        .Max(r => Convert.ToInt32(r.Cells["Index"].Value)) + 1;
                }

                dataGridView1.Rows.Add(
                    newIndex,  // 序号
                    "",        // 长度（留空待输入）
                    "",        // 宽度（留空待输入）
                    "",        // 厚度（留空待输入）
                    "",        // 块数（留空待输入）
                    "",        // 组数（留空待输入）
                    "",        // 批次（留空待输入）
                    ""        // 总数（自动计算）
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(System.Globalization.CultureInfo.InvariantCulture, Strings.Get("Feed.Error.AddRowFailed"), ex.Message));
            }
        }

        private void AssignFirstRowToHmiArray()
        {
            // 检查数据行是否存在且不是新行
            if (dataGridView1.Rows.Count == 0 || dataGridView1.Rows[0].IsNewRow)
            {
                MessageBox.Show(Strings.Get("Feed.Error.NoValidRows"));
                return;
            }

            DataGridViewRow firstRow = dataGridView1.Rows[0];
            int startIndex = 90; // Hmi_iArray起始索引

            try
            {
                // 遍历前10列（索引90-95）
                for (int i = 0; i < 7; i++)
                {
                    // 检查列是否存在
                    if (i >= firstRow.Cells.Count)
                    {
                        MainFrm.Hmi_iArray[startIndex + i] = 0; // 填充默认值
                        continue;
                    }

                    object? cellValue = firstRow.Cells[i + 1].Value;
                    int parsedValue = 0;

                    if (cellValue != null)
                    {
                        string cellString = cellValue.ToString() ?? string.Empty;

                        // 特殊处理第四列（厚度列）
                        if (i == 0 || i == 1)
                        {
                            if (!MainFrm.TryParseDisplayLength(cellString, out double lengthMm))
                            {
                                MessageBox.Show(string.Format(System.Globalization.CultureInfo.InvariantCulture, Strings.Get("Feed.Error.InvalidIntegerColumn"), i + 1));
                                return;
                            }
                            parsedValue = (int)Math.Round(lengthMm);
                        }
                        else if (i == 2)
                        {
                            if (!MainFrm.TryParseDisplayLength(cellString, out double thicknessMm))
                            {
                                MessageBox.Show(string.Format(System.Globalization.CultureInfo.InvariantCulture, Strings.Get("Feed.Error.InvalidThicknessColumn"), i + 1));
                                return;
                            }
                            parsedValue = (int)Math.Round(thicknessMm * 10);
                        }
                        else // 其他列保持原有逻辑
                        {
                            if (!int.TryParse(cellString, out int result))
                            {
                                MessageBox.Show(string.Format(System.Globalization.CultureInfo.InvariantCulture, Strings.Get("Feed.Error.InvalidIntegerColumn"), i + 1));
                                return;
                            }
                            parsedValue = result;
                        }

                        // 溢出检查（所有列）
                        if (parsedValue < short.MinValue || parsedValue > short.MaxValue)
                        {
                            MessageBox.Show(string.Format(System.Globalization.CultureInfo.InvariantCulture, Strings.Get("Feed.Error.ValueOutOfRange"), i + 1, parsedValue));
                            return;
                        }
                    }

                    MainFrm.Hmi_iArray[startIndex + i] = (short)parsedValue;
                }

                MainFrm.Hmi_rArray[90] = (float)ReleasePos1;
                MainFrm.Hmi_rArray[91] = (float)ReleasePos2;
                MainFrm.Hmi_rArray[92] = (float)ReleasePos3;
                MainFrm.Hmi_rArray[93] = (float)ReleasePos4;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(System.Globalization.CultureInfo.InvariantCulture, Strings.Get("Feed.Error.DataConversion"), ex.Message));
            }
        }


        private static readonly string DataFilePath = Path.Combine(Application.StartupPath, "FeedData.json");

        // 保存数据到文件
        private void SaveDataToFile()
        {
            var rows = new List<Dictionary<string, object>>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                var rowData = new Dictionary<string, object>();
                rowData["LengthUnit"] = DisplayUnitManager.ToConfigToken(DisplayUnitManager.CurrentDisplayUnit);
                foreach (DataGridViewCell cell in row.Cells)
                {
                    string columnName = dataGridView1.Columns[cell.ColumnIndex].Name;
                        rowData[columnName] = cell.Value ?? string.Empty;
                }
                rows.Add(rowData);
            }

            string json = JsonConvert.SerializeObject(rows, Formatting.Indented);
            File.WriteAllText(DataFilePath, json);
        }

        // 从文件加载数据
        private void LoadDataFromFile()
        {
            if (!File.Exists(DataFilePath)) return;

            string json = File.ReadAllText(DataFilePath);
            var rows = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json) ?? new List<Dictionary<string, object>>();

            dataGridView1.Rows.Clear();
            foreach (var rowData in rows)
            {
                DisplayLengthUnit savedUnit = DisplayLengthUnit.Millimeter;
                if (rowData.TryGetValue("LengthUnit", out object? unitToken))
                {
                    savedUnit = string.Equals(unitToken?.ToString(), "mm", StringComparison.OrdinalIgnoreCase)
                        ? DisplayLengthUnit.Millimeter
                        : DisplayLengthUnit.Inch;
                }
                int rowIndex = dataGridView1.Rows.Add();
                foreach (DataGridViewCell cell in dataGridView1.Rows[rowIndex].Cells)
                {
                    string columnName = dataGridView1.Columns[cell.ColumnIndex].Name;
                    if (rowData.TryGetValue(columnName, out object? value))
                    {
                        if ((columnName == "Length" || columnName == "Width" || columnName == "Thickness")
                            && MainFrm.TryParseLengthByUnit(value?.ToString(), savedUnit, out double mmValue))
                        {
                            cell.Value = MainFrm.FormatDisplayLength(mmValue);
                        }
                        else
                        {
                            cell.Value = value;
                        }
                    }
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SaveDataToFile(); // 关闭时保存数据
            base.OnFormClosing(e);
        }

        private void sw吸盘1_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[55] = !MainFrm.Hmi_bArray[55];
            sw吸盘1.BackgroundImage = MainFrm.Hmi_bArray[55] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            mf.AdsWritePlc();
        }

        private void sw吸盘2_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[56] = !MainFrm.Hmi_bArray[56];
            sw吸盘2.BackgroundImage = MainFrm.Hmi_bArray[56] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            mf.AdsWritePlc();
        }

        private void sw吸盘3_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[57] = !MainFrm.Hmi_bArray[57];
            sw吸盘3.BackgroundImage = MainFrm.Hmi_bArray[57] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            mf.AdsWritePlc();
        }

        private void sw吸盘4_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[58] = !MainFrm.Hmi_bArray[58];
            sw吸盘4.BackgroundImage = MainFrm.Hmi_bArray[58] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            mf.AdsWritePlc();

        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        private void btn列表_清除_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null || dataGridView1.CurrentRow.Index < 0)
                return;

            dataGridView1.Rows.Remove(dataGridView1.CurrentRow);
            if (dataGridView1.CurrentRow == null)       //删除之后为空,跳出
                return;
            //列表_更改序号及选中行(dataGridView1.CurrentRow.Index, dataGridView1.CurrentRow.Index);
        }

        private void btn_MouseDown(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Name == "btn移动装料架") { MainFrm.Hmi_bArray[54] = true; mf.AdsWritePlc(); }
            else if (btn.Name == "btn料架前进") { MainFrm.Hmi_bArray[130] = true; mf.AdsWritePlc(); }
            else if (btn.Name == "btn料架后退") { MainFrm.Hmi_bArray[131] = true; mf.AdsWritePlc(); }

        }

        private void btn_MouseUp(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Name == "btn移动装料架") { MainFrm.Hmi_bArray[54] = false; mf.AdsWritePlc(); }
            else if (btn.Name == "btn料架前进") { MainFrm.Hmi_bArray[130] = false; mf.AdsWritePlc(); }
            else if (btn.Name == "btn料架后退") { MainFrm.Hmi_bArray[131] = false; mf.AdsWritePlc(); }
        }

        private void txb进料定位_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // 验证输入是否为有效的浮点数
                if (MainFrm.TryParseDisplayLength(txb进料定位.Text, out double valueMm))
                {
                    MainFrm.Hmi_rArray[57] = (float)valueMm;
                    txb进料定位.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[57]);
                    mf.AdsWritePlc1float(57, MainFrm.Hmi_rArray[57]);
                    MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 12] = MainFrm.Hmi_rArray[57];
                    mf.wrtConfigFile("[ManualOldSelect]", 12);
                }
                else
                {
                    MessageBox.Show(Strings.Get("Feed.Error.InvalidFeedPosition"));
                }
            }
        }


    }
}
