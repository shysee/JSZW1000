using JSZW1000A;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JSZW1000A
{
    public partial class FrmFeed : Form
    {

        MainFrm mf;

        public FrmFeed(MainFrm mf1)
        {
            InitializeComponent();
            this.mf = mf1;
        }


        // 后夹送位置和间距参数
        private const int PositionT1 = 909;
        private const int PositionT2 = 1614;
        private const int PositionT3 = 2409;
        private const int PositionT4 = 3114;

        private const int EdgeToT1 = 909;
        private const int EdgeToT4 = 886;
        private const int MaxLength = 4000;

        private int OffsetToT1 = (int)MainFrm.Hmi_rArray[107];//吸盘零点到设备边缘的偏移
        private int SuckerPos = (int)MainFrm.Hmi_rArray[108];//吸盘取料位置
        private int OffsetToHead = (int)MainFrm.Hmi_rArray[109];//板头到取料位置的偏移

        private int SuctionGroups; // 吸盘组数
        private int HeadPosition; // 板材头部位置
        private int TailPosition; // 板材尾部位置

        private double ReleasePos1, ReleasePos2;//释放板材位置1,2

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
        double totalWidth = 0;
        int FlodNum = 0;

        private void FrmFeed_Load(object sender, EventArgs e)
        {
            txb进料定位.Text = Convert.ToString(MainFrm.Hmi_rArray[57]);
            pos1.Text = Convert.ToString(MainFrm.Hmi_rArray[90]);
            pos2.Text = Convert.ToString(MainFrm.Hmi_rArray[91]);
            LoadDataFromFile(); // 加载数据

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            txb进料位置.Text = MainFrm.Hmi_rArray[40].ToString("f1");
            sw吸盘1.BackgroundImage = MainFrm.Hmi_bArray[55] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            sw吸盘2.BackgroundImage = MainFrm.Hmi_bArray[56] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
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
                        MessageBox.Show($"板材序号格式不正确，请输入有效的整数");
                        return;
                    }

                    if (!int.TryParse(row.Cells["Width"].Value?.ToString(), out int width))
                    {
                        MessageBox.Show($"板材{index}宽度格式不正确，请输入有效的整数");
                        return;
                    }

                    if (!int.TryParse(row.Cells["Length"].Value?.ToString(), out int length))
                    {
                        MessageBox.Show($"板材{index}长度格式不正确，请输入有效的整数");
                        return;
                    }

                    if (!int.TryParse(row.Cells["Quantity"].Value?.ToString(), out int quantity))
                    {
                        MessageBox.Show($"板材{index}数量格式不正确，请输入有效的整数");
                        return;
                    }

                    if (!int.TryParse(row.Cells["GroupsNeeded"].Value?.ToString(), out int groups))
                    {
                        MessageBox.Show($"板材{index}组数格式不正确，请输入有效的整数");
                        return;
                    }

                    if (!int.TryParse(row.Cells["Batch"].Value?.ToString(), out int batch))
                    {
                        MessageBox.Show($"板材{index}批次格式不正确，请输入有效的整数");
                        return;
                    }

                    var plate = new PlateInfo
                    {
                        Index = index,
                        Width = width,
                        Length = length,
                        Quantity = quantity,
                        Groups = groups,
                        Batch = batch
                    };
                    // 校验长度
                    if (plate.Length < 705 || plate.Length > 4000)
                    {
                        MessageBox.Show($"板材{plate.Index}长度无效，必须介于705mm和4000mm之间");
                        return;
                    }

                    // 校验宽度
                    if (plate.Width > 800)
                    {
                        MessageBox.Show($"板材{plate.Index}宽度不能超过800mm");
                        return;
                    }

                    // 校验组数
                    if (plate.Groups < 1 || plate.Groups > 2)
                    {
                        MessageBox.Show($"板材{plate.Index}组数需在1 - 2之间");
                        return;
                    }
                    // 更新表格数据
                    row.Cells["Total"].Value = plate.Total;
                    FlodNum = plate.Groups * plate.Batch;
                    var positions = CalculateReleasePos(plate.Length, FlodNum);
                    //var positions = CalculateReleasePos(plate.Length, plate.Groups);

                    ReleasePos1 = positions.ReleasePos1;
                    ReleasePos2 = positions.ReleasePos2;

                    pos1.Text = ReleasePos1.ToString();
                    pos2.Text = ReleasePos2.ToString();

                    plates.Add(plate);
                }
                AssignFirstRowToHmiArray();
                mf.AdsWritePlc();
                SaveDataToFile(); // 确认时额外保存
                MessageBox.Show("分配成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"错误：{ex.Message}");
            }
        }

        public (double ReleasePos1, double ReleasePos2) CalculateReleasePos(int length, int groupCount)
        {
            if (length < 705 || length > 4000)
                throw new ArgumentException("板材长度无效，必须介于705mm和4000mm之间");
            // 新增组数判断逻辑
            if (groupCount == 1)//单组逻辑
            {
                if (length < 1301)
                {
                    // 计算夹送组1-2位置
                    var group12 = CalculatePosition(length, PositionT1, PositionT2);

                    return (group12.Value, 0);
                }
                else
                {
                    // 计算夹送组2-3位置
                    var group23 = CalculatePosition(length, PositionT2, PositionT3);

                    return (group23.Value, 0);
                }
            }

            if (groupCount == 2)//2组逻辑
            {
                if (length > 1800)
                    throw new ArgumentException("双组放置时板材长度必须小于1800mm");

                // 计算夹送组1-2位置
                var group12 = CalculatePosition(length, PositionT1, PositionT2);
                // 计算夹送组3-4位置
                var group34 = CalculatePosition(length, PositionT3, PositionT4);

                if (!group12.IsValid || !group34.IsValid)
                    throw new InvalidOperationException("夹送组位置无效");

                // 验证位置不冲突
                if (group34.Value - group12.Value < length)
                    throw new InvalidOperationException("两组位置间隔不足，无法避免重叠");

                return (group12.Value, group34.Value);
            }

            throw new InvalidOperationException("无法找到满足条件的夹送组位置。");

        }

        private PositionResult CalculatePosition(int length, int C_i, int C_j)
        {
            double C_center = (C_i + C_j) / 2.0;
            double X = C_center + (length / 2.0) - OffsetToHead;

            // 计算允许范围
            int lowerBound = Math.Max((int)(C_j - OffsetToHead), length - OffsetToHead);
            int upperBound = Math.Min((int)(C_i + length - OffsetToHead), 3950);

            // 调整到有效范围
            X = Math.Clamp(X, lowerBound, upperBound);

            // 验证夹持条件
            double H = X + OffsetToHead;
            double T = H - length;

            return new PositionResult
            {
                Value = (double)Math.Round(X),
                IsValid = H >= C_j && T <= C_i && T >= 0 && H <= 4000
            };
        }

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
                    ""        // 总数（自动计算）
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"添加行失败: {ex.Message}");
            }
        }

        private void AssignFirstRowToHmiArray()
        {
            // 检查数据行是否存在且不是新行
            if (dataGridView1.Rows.Count == 0 || dataGridView1.Rows[0].IsNewRow)
            {
                MessageBox.Show("没有有效数据行可处理");
                return;
            }

            DataGridViewRow firstRow = dataGridView1.Rows[0];
            int startIndex = 90; // Hmi_iArray起始索引

            try
            {
                // 遍历前10列（索引90-95）
                for (int i = 0; i < 6; i++)
                {
                    // 检查列是否存在
                    if (i >= firstRow.Cells.Count)
                    {
                        MainFrm.Hmi_iArray[startIndex + i] = 0; // 填充默认值
                        continue;
                    }

                    object cellValue = firstRow.Cells[i + 1].Value;
                    int parsedValue = 0;

                    if (cellValue != null)
                    {
                        // 特殊处理第四列（厚度列）
                        if (i == 2)
                        {
                            parsedValue = cellValue switch
                            {
                                int intVal => intVal * 10,
                                double doubleVal => (int)Math.Round(doubleVal * 10),
                                string strVal when double.TryParse(strVal, out double dVal) => (int)Math.Round(dVal * 10),
                                _ => throw new InvalidCastException($"第{i + 1}列(厚度)数据类型不支持")
                            };
                        }
                        else // 其他列保持原有逻辑
                        {
                            parsedValue = cellValue switch
                            {
                                int intVal => intVal,
                                double doubleVal => (int)Math.Round(doubleVal),
                                string strVal when int.TryParse(strVal, out int result) => result,
                                _ => throw new InvalidCastException($"第{i + 1}列数据类型不支持")
                            };
                        }

                        // 溢出检查（所有列）
                        if (parsedValue < short.MinValue || parsedValue > short.MaxValue)
                            throw new OverflowException($"第{i + 1}列值{parsedValue}超出范围(-32768~32767)");
                    }

                    MainFrm.Hmi_iArray[startIndex + i] = (short)parsedValue;

                }
                MainFrm.Hmi_rArray[90] = (float)ReleasePos1;
                MainFrm.Hmi_rArray[91] = (float)ReleasePos2;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"数据转换失败: {ex.Message}");
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
                foreach (DataGridViewCell cell in row.Cells)
                {
                    string columnName = dataGridView1.Columns[cell.ColumnIndex].Name;
                    rowData[columnName] = cell.Value;
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
            var rows = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);

            dataGridView1.Rows.Clear();
            foreach (var rowData in rows)
            {
                int rowIndex = dataGridView1.Rows.Add();
                foreach (DataGridViewCell cell in dataGridView1.Rows[rowIndex].Cells)
                {
                    string columnName = dataGridView1.Columns[cell.ColumnIndex].Name;
                    if (rowData.TryGetValue(columnName, out object value))
                    {
                        cell.Value = value;
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
            sw吸盘2.BackgroundImage = MainFrm.Hmi_bArray[56] ? global::JSZW1000A .Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
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
                MainFrm.Hmi_rArray[57] = Convert.ToSingle(txb进料定位.Text);
                mf.AdsWritePlc1float(57, MainFrm.Hmi_rArray[57]);
                MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 12] = MainFrm.Hmi_rArray[57];
                mf.wrtConfigFile("[ManualOldSelect]", 12);
            }
        }


    }
}
