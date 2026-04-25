using ClosedXML.Excel;
using System.Globalization;
using TextBox = System.Windows.Forms.TextBox;

namespace JSZW1000A.SubWindows
{
    public partial class SubOPSetting : UserControl
    {
        public MainFrm mf = null!;
        private bool isLanguageSelectorUpdating;
        private readonly Label[] lblFlipParameterNames;
        private readonly TextBox[] txbFlipParameters;
        private readonly Label[] lblFlipParameterUnits;
        private int lastAngleMappingIndex;
        public SubOPSetting()
        {
            InitializeComponent();
            lblFlipParameterNames = new[] { lblFlipParameter00, lblFlipParameter01, lblFlipParameter02, lblFlipParameter03, lblFlipParameter04 };
            txbFlipParameters = new[] { txbFlipParameter00, txbFlipParameter01, txbFlipParameter02, txbFlipParameter03, txbFlipParameter04 };
            lblFlipParameterUnits = new[] { lblFlipParameterUnit00, lblFlipParameterUnit01, lblFlipParameterUnit02, lblFlipParameterUnit03, lblFlipParameterUnit04 };
            foreach (TextBox textBox in txbFlipParameters)
                TextBoxInputBehavior.AttachSelectAllOnFocus(textBox);
            setLang();
        }
        bool sw_角度映射 = false;

        private void SubOPSetting_Load(object sender, EventArgs e)
        {
            sw_角度映射 = false;
            ApplyAngleAreaLayout();
            LoadInit();
            RefreshLanguageSelector();
            RefreshDisplayUnitSelector();
            gbxLanguage.Visible = true;
            gbxDisplayUnit.Visible = true;
            RefreshAngleMappingLayer();
            RefreshFlipParameterVisibility();

            lb后挡参数14.Visible = lb后挡参数15.Visible = lb后挡参数16.Visible = lb后挡参数17.Visible =
            lb后挡参数mm14.Visible = lb后挡参数mm15.Visible = lb后挡参数mm16.Visible = lb后挡参数mm17.Visible =
            txb后挡参数14.Visible = txb后挡参数15.Visible = txb后挡参数16.Visible = txb后挡参数17.Visible = (MainFrm.ConfigData[MainFrm.L1_GlobalSwitch + 9] > 400);
            txb后挡参数18.Visible = lb后挡参数18.Visible = lb后挡参数mm18.Visible = (MainFrm.ConfigData[MainFrm.L1_GlobalSwitch + 9] > 800);

            groupBox6.Visible = (MainFrm.ConfigData[MainFrm.L1_GlobalSwitch + 10] > 0);
        }

        private void ApplyAngleAreaLayout()
        {
            gbxLanguage.Location = new Point(1080, 67);
            gbxLanguage.Size = new Size(400, 62);
            gbxDisplayUnit.Location = new Point(1080, 136);
            gbxDisplayUnit.Size = new Size(400, 62);
            groupBox5.Location = new Point(418, 554);
            groupBox5.Size = new Size(367, 240);
            //gbxFlipParameters.Location = new Point(1080, 205);
            //gbxFlipParameters.Size = new Size(420, 244);
            lblFlipFormula.Location = new Point(1085, 455);
            btn解耦归零.Location = new Point(564, 833);
            btn导入进阶参数.Location = new Point(818, 833);
        }

        private void RefreshAngleMappingLayer()
        {
            gbx角度映射.Visible = sw_角度映射;
            if (sw_角度映射)
            {
                gbx角度映射.BringToFront();
                return;
            }

            gbxLanguage.BringToFront();
            gbxDisplayUnit.BringToFront();
            if (gbxFlipParameters.Visible)
                gbxFlipParameters.BringToFront();
            if (lblFlipFormula.Visible)
                lblFlipFormula.BringToFront();
        }

        private void RefreshFlipParameterVisibility()
        {
            bool visible = MainFrm.Hmi_bArray[6];
            gbxFlipParameters.Visible = visible;
            lblFlipFormula.Visible = visible;
        }

        private void setLang()
        {
            LocalizationManager.ApplyResources(this);

            if (MainFrm.Lang == 0)
            {
                label5.Font = label1.Font = label2.Font = label3.Font = label4.Font = label6.Font = label9.Font = label11.Font = label13.Font =
                label16.Font = label19.Font = label20.Font = label18.Font = label15.Font = label21.Font = label22.Font = label23.Font =
                label24.Font = label25.Font = label26.Font = label27.Font = label28.Font = label73.Font = label71.Font = label72.Font = label70.Font = label51.Font =
                label50.Font = label69.Font = label53.Font = label52.Font = new System.Drawing.Font("微软雅黑", 12F);
                label74.Font = label75.Font = label76.Font = label77.Font = label78.Font =
                label7.Font = label8.Font = label10.Font = label12.Font = label14.Font = label17.Font = label33.Font = label34.Font =
                label35.Font = label36.Font = label37.Font = label38.Font = label39.Font = label40.Font = label41.Font =
                label48.Font = label49.Font = label97.Font = label98.Font = label99.Font = label100.Font = label101.Font = label102.Font = label103.Font =
                label47.Font = label81.Font = label82.Font = label83.Font = label84.Font = label85.Font = label86.Font = label87.Font = label88.Font = new System.Drawing.Font("微软雅黑", 10F);
                groupBox1.Font = groupBox2.Font = groupBox3.Font = groupBox4.Font = groupBox5.Font = gbxFlipParameters.Font = gbx角度映射.Font = new System.Drawing.Font("微软雅黑", 15.75F);
                foreach (Label label in lblFlipParameterNames)
                    label.Font = new Font("微软雅黑", 12F);
                foreach (Label label in lblFlipParameterUnits)
                    label.Font = new Font("微软雅黑", 10F);
                lblFlipFormula.Font = new Font("微软雅黑", 9.5F);
                btn解耦归零.Font = btn导入角度映像.Font = btn导入进阶参数.Font = btn角度映射保存.Font = btn角度映射下载.Font =
                    btn角度映射新增.Font = btn角度映射删除.Font = new System.Drawing.Font("宋体", 10F);
            }
            else
            {
                label5.Font = label1.Font = label2.Font = label3.Font = label4.Font = label6.Font = label9.Font = label11.Font = label13.Font =
                label16.Font = label19.Font = label20.Font = label18.Font = label15.Font = label21.Font = label22.Font = label23.Font =
                label24.Font = label25.Font = label26.Font = label27.Font = label28.Font = label73.Font = label71.Font = label72.Font = label70.Font = label51.Font =
                label50.Font = label69.Font = label53.Font = label52.Font = new System.Drawing.Font("Calibri", 12F);
                label74.Font = label75.Font = label76.Font = label77.Font = label78.Font =
                label7.Font = label8.Font = label10.Font = label12.Font = label14.Font = label17.Font = label33.Font = label34.Font =
                label35.Font = label36.Font = label37.Font = label38.Font = label39.Font = label40.Font = label41.Font =
                label48.Font = label49.Font = label97.Font = label98.Font = label99.Font = label100.Font = label101.Font = label102.Font = label103.Font =
                label47.Font = label81.Font = label82.Font = label83.Font = label84.Font = label85.Font = label86.Font = label87.Font = label88.Font = new System.Drawing.Font("Calibri", 10F);
                groupBox1.Font = groupBox2.Font = groupBox3.Font = groupBox4.Font = groupBox5.Font = gbxFlipParameters.Font = gbx角度映射.Font = new System.Drawing.Font("Calibri", 15.75F);
                foreach (Label label in lblFlipParameterNames)
                    label.Font = new Font("Calibri", 12F);
                foreach (Label label in lblFlipParameterUnits)
                    label.Font = new Font("Calibri", 10F);
                lblFlipFormula.Font = new Font("Calibri", 9.5F);
                btn解耦归零.Font = btn导入角度映像.Font = btn导入进阶参数.Font = btn角度映射保存.Font = btn角度映射下载.Font =
                    btn角度映射新增.Font = btn角度映射删除.Font = new System.Drawing.Font("Calibri", 10F);
            }
            lblLanguage.Text = Strings.Get("Setting.Language.Label");
            gbxLanguage.Text = Strings.Get("Setting.Language.Title");
            btnApplyLanguage.Text = Strings.Get("Setting.Language.Apply");
            lblDisplayUnit.Text = Strings.Get("Setting.DisplayUnit.Label");
            gbxDisplayUnit.Text = Strings.Get("Setting.DisplayUnit.Title");
            btnApplyDisplayUnit.Text = Strings.Get("Setting.DisplayUnit.Apply");
            groupBox1.Text = Strings.Get("Setting.Group.Clamp");
            groupBox2.Text = Strings.Get("Setting.Group.BackGaugeApron");
            groupBox3.Text = Strings.Get("Setting.Group.PresetAngles");
            groupBox4.Text = Strings.Get("Setting.Group.PresetLengths");
            groupBox5.Text = Strings.Get("Setting.Group.BackGaugeHome");
            groupBox6.Text = Strings.Get("Setting.Group.Feed");
            gbxFlipParameters.Text = Strings.Get("Setting.Group.FlipParameters");
            gbx角度映射.Text = Strings.Get("Setting.Group.AngleMap");
            label5.Text = Strings.Get("Setting.Option.SlitterSpeed");
            label1.Text = Strings.Get("Setting.Option.LoadingTables");
            label2.Text = Strings.Get("Setting.Option.AngleDisplay");
            label3.Text = Strings.Get("Setting.Option.UnclampMode");
            label4.Text = Strings.Get("Setting.Option.AutoFlip");
            label30.Text = Strings.Get("Setting.Option.AutoFeed");
            label6.Text = Strings.Get("Setting.ReleaseHeight.Low");
            label9.Text = Strings.Get("Setting.ReleaseHeight.Medium");
            label11.Text = Strings.Get("Setting.ReleaseHeight.High");
            label13.Text = Strings.Get("Setting.ReleaseHeight.Maximum");
            label16.Text = Strings.Get("Setting.Clamp.HalfClamp");
            label19.Text = Strings.Get("Setting.Clamp.SquashFoldClampHeight");
            label20.Text = Strings.Get("Setting.Clamp.LastFoldUnclampHeight");
            label130.Text = Strings.Get("Setting.Clamp.DefaultApronRetract");
            label131.Text = Strings.Get("Setting.Clamp.OpenSquashCloseHeight");
            label18.Text = Strings.Get("Setting.BackGauge.MaxPosition");
            label15.Text = Strings.Get("Setting.BackGauge.MinPosition");
            label21.Text = Strings.Get("Setting.BackGauge.SlitOffset");
            label22.Text = Strings.Get("Setting.BackGauge.TopApronZero");
            label23.Text = Strings.Get("Setting.BackGauge.BottomApronZero");
            label24.Text = Strings.Get("Setting.BackGauge.UpsideSquashStop");
            label25.Text = Strings.Get("Setting.BackGauge.DownsideSquashStop");
            label26.Text = Strings.Get("Setting.BackGauge.OutsideSquashStop");
            label27.Text = Strings.Get("Setting.BackGauge.OpenUpsideSquashStop");
            label28.Text = Strings.Get("Setting.BackGauge.OpenDownsideSquashStop");
            lblFlipParameterNames[0].Text = Strings.Get("Setting.Flip.TableToClampDistance");
            lblFlipParameterNames[1].Text = Strings.Get("Setting.Flip.SuctionCupInstallPosition");
            lblFlipParameterNames[2].Text = Strings.Get("Setting.Flip.PickupJawHeight");
            lblFlipParameterNames[3].Text = Strings.Get("Setting.Flip.TopApronPickupAngle");
            lblFlipParameterNames[4].Text = Strings.Get("Setting.Flip.TableOffset");
            lblFlipFormula.Text = Strings.Get("Setting.Flip.FormulaDescription");
            label73.Text = Strings.Get("Setting.AngleMap.Title");
            label72.Text = Strings.Get("Setting.AngleMap.TableName");
            label70.Text = Strings.Get("Setting.AngleMap.Material");
            label71.Text = Strings.Get("Setting.AngleMap.Strength");
            label51.Text = Strings.Get("Setting.AngleMap.Thickness");
            label50.Text = Strings.Get("Setting.AngleMap.MachineGauge");
            label69.Text = Strings.Get("Setting.AngleMap.FoldAngleRange");
            label53.Text = Strings.Get("Setting.AngleMap.BottomOffset");
            label52.Text = Strings.Get("Setting.AngleMap.TopOffset");
            label74.Text = Strings.Get("Setting.AngleMap.Half");
            label120.Text = Strings.Get("Setting.Feed.ZeroOffset");
            label121.Text = Strings.Get("Setting.Feed.HeadPickupOffset");
            label122.Text = Strings.Get("Setting.Feed.PickupJawHeight");
            label123.Text = Strings.Get("Setting.Feed.Position");
            btn解耦归零.Text = Strings.Get("Setting.Action.DecoupleServoZero");
            btn导入角度映像.Text = Strings.Get("Setting.Action.LoadImageFile");
            btn导入进阶参数.Text = Strings.Get("Setting.Action.LoadAdvancedParameters");
            btn角度映射保存.Text = Strings.Get("Setting.Action.SaveAngleMap");
            btn角度映射下载.Text = Strings.Get("Setting.Action.DownloadAngleMap");
            btn角度映射新增.Text = Strings.Get("Setting.Action.AddAngleMap");
            btn角度映射删除.Text = Strings.Get("Setting.Action.DeleteAngleMap");
            label114.Text = Strings.Get("Setting.AngleMap.OffsetHint");
            string mm = MainFrm.GetLengthUnitLabel();
            string deg = Strings.Get("Common.Deg");
            foreach (Label unitLabel in lblFlipParameterUnits)
                unitLabel.Text = mm;
            lb后挡参数mm10.Text = lb后挡参数mm11.Text = lb后挡参数mm12.Text = lb后挡参数mm13.Text =
                lb后挡参数mm14.Text = lb后挡参数mm15.Text = lb后挡参数mm16.Text = lb后挡参数mm17.Text = lb后挡参数mm18.Text = mm;
            label7.Text = label8.Text = label10.Text = label12.Text = label14.Text = label17.Text = label116.Text = label132.Text = label33.Text = label34.Text =
                label35.Text = label36.Text = label37.Text = label38.Text = label39.Text = label40.Text = label41.Text =
                label48.Text = label49.Text = label97.Text = label98.Text = label99.Text = label100.Text = label101.Text = label102.Text = label103.Text =
                label124.Text = label125.Text = label126.Text = label127.Text = mm;
            label47.Text = label81.Text = label82.Text = label83.Text = label84.Text = label85.Text = label86.Text = label87.Text = label88.Text = deg;
            RefreshReleaseHeightSelector();
            RefreshLanguageSelector();
        }

        private void RefreshReleaseHeightSelector()
        {
            int selectedIndex = cbx末次抬钳高.SelectedIndex;
            cbx末次抬钳高.SelectedIndexChanged -= cbx末次抬钳高_SelectedIndexChanged;
            cbx末次抬钳高.Items.Clear();
            cbx末次抬钳高.Items.Add(Strings.Get("Setting.ReleaseHeight.Low"));
            cbx末次抬钳高.Items.Add(Strings.Get("Setting.ReleaseHeight.Medium"));
            cbx末次抬钳高.Items.Add(Strings.Get("Setting.ReleaseHeight.High"));
            cbx末次抬钳高.Items.Add(Strings.Get("Setting.ReleaseHeight.Maximum"));
            cbx末次抬钳高.SelectedIndex = Math.Clamp(selectedIndex, 0, cbx末次抬钳高.Items.Count - 1);
            cbx末次抬钳高.SelectedIndexChanged += cbx末次抬钳高_SelectedIndexChanged;
        }

        private bool lastAdsConn = false;
        private int Delay0 = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            RefreshAngleMappingLayer();
            RefreshFlipParameterVisibility();
            sw角度映射.BackgroundImage = sw_角度映射 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            sw分条机速度.BackgroundImage = MainFrm.Hmi_bArray[2] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            sw加载桌板.BackgroundImage = MainFrm.Hmi_bArray[3] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            sw角度显示.BackgroundImage = MainFrm.Hmi_bArray[4] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            sw松开方式1.BackgroundImage = MainFrm.Hmi_bArray[5] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            sw自动翻转.BackgroundImage = MainFrm.Hmi_bArray[6] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            sw自动进料.BackgroundImage = MainFrm.Hmi_bArray[7] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            //PLC连接跳变时,刷新一次
            switch (Delay0)
            {
                case 0:
                    if (MainFrm.AdsConn && !lastAdsConn)
                        lastAdsConn = MainFrm.AdsConn;
                    if (MainFrm.AdsConn && lastAdsConn)
                        Delay0 = 1;
                    break;
                case 1:
                    Delay0++; break;
                case 2:
                    Delay0++;
                    break;
                case 3:
                    LoadInit(); Delay0++;
                    break;

            }

        }

        private void sw角度映射_Click(object sender, EventArgs e)
        {
            sw_角度映射 = !sw_角度映射;
            sw角度映射.BackgroundImage = sw_角度映射 ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            RefreshAngleMappingLayer();
            if (sw_角度映射) { Load角度映射(); }

        }

        void LoadInit()
        {
            txb夹钳_低.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[100]);
            txb夹钳_中.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[101]);
            txb夹钳_高.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[102]);
            txb夹钳_最大.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[103]);
            txb夹钳_半夹钳.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[104]);
            txb夹钳_挤压折弯.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[105]);
            TextBoxInputBehavior.AttachSelectAllOnFocus(txb夹钳_低);
            TextBoxInputBehavior.AttachSelectAllOnFocus(txb夹钳_中);
            TextBoxInputBehavior.AttachSelectAllOnFocus(txb夹钳_高);
            TextBoxInputBehavior.AttachSelectAllOnFocus(txb夹钳_最大);
            TextBoxInputBehavior.AttachSelectAllOnFocus(txb夹钳_半夹钳);
            TextBoxInputBehavior.AttachSelectAllOnFocus(txb夹钳_挤压折弯);
            //cbx挤压压钳高.SelectedIndex = Convert.ToInt32(MainFrm.Hmi_rArray[105]);
            cbx末次抬钳高.SelectedIndex = Convert.ToInt32(MainFrm.Hmi_rArray[106]);
            RefreshReleaseHeightSelector();

            int defaultRetractValue = Convert.ToInt32(MainFrm.ConfigData[MainFrm.L6_MachineSetup + 9]);
            if (defaultRetractValue < 2 || defaultRetractValue > 4)
                defaultRetractValue = 4;
            cbx默认滑台收缩值.SelectedIndex = defaultRetractValue - 2;

            txb开口挤压下降高度.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[129]);
            TextBoxInputBehavior.AttachSelectAllOnFocus(txb开口挤压下降高度);

            txb进料参数00.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[107]);
            txb进料参数03.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[108]);
            txb进料参数01.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[109]);
            txb进料参数02.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[94]);
            LoadFlipParameters();

            for (int i = 0; i < MainFrm.L3_BackgaugeApron - 2; i++)
            {
                System.Windows.Forms.TextBox txb = ((System.Windows.Forms.TextBox)this.Controls.Find("txb后挡参数" + string.Format("{0:D2}", i), true)[0]);
                txb.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[110 + i]);
                TextBoxInputBehavior.AttachSelectAllOnFocus(txb);
            }

            RefreshAngleMappingSelector(mf.cbx材料选择.SelectedIndex);
            //cbx角度补偿0.SelectedIndex = Convert.ToInt32(MainFrm.Hmi_rArray[56]);
            //Load角度映射();
            UpdateToggleStateTexts();
        }

        private void RefreshLanguageSelector()
        {
            if (cbxLanguage == null)
                return;

            isLanguageSelectorUpdating = true;
            cbxLanguage.Items.Clear();
            cbxLanguage.Items.Add(LocalizationManager.GetLanguageDisplayName(AppLanguage.ZhCn));
            cbxLanguage.Items.Add(LocalizationManager.GetLanguageDisplayName(AppLanguage.EnUs));
            cbxLanguage.Items.Add(LocalizationManager.GetLanguageDisplayName(AppLanguage.FrFr));
            cbxLanguage.Items.Add(LocalizationManager.GetLanguageDisplayName(AppLanguage.RuRu));
            cbxLanguage.SelectedIndex = Math.Clamp(LocalizationManager.ToLegacyLanguageId(LocalizationManager.CurrentLanguage), 0, cbxLanguage.Items.Count - 1);
            btnApplyLanguage.Enabled = false;
            isLanguageSelectorUpdating = false;
        }

        private void RefreshDisplayUnitSelector()
        {
            if (cbxDisplayUnit == null)
                return;

            isDisplayUnitSelectorUpdating = true;
            cbxDisplayUnit.Items.Clear();
            cbxDisplayUnit.Items.Add(DisplayUnitManager.GetDisplayName(DisplayLengthUnit.Inch));
            cbxDisplayUnit.Items.Add(DisplayUnitManager.GetDisplayName(DisplayLengthUnit.Millimeter));
            cbxDisplayUnit.SelectedIndex = DisplayUnitManager.CurrentDisplayUnit == DisplayLengthUnit.Inch ? 0 : 1;
            btnApplyDisplayUnit.Enabled = false;
            isDisplayUnitSelectorUpdating = false;
        }

        private void UpdateToggleStateTexts()
        {
            label75.Text = LocalizationText.EnabledDisabled(MainFrm.Hmi_bArray[3]);
            label77.Text = LocalizationText.AutoManual(MainFrm.Hmi_bArray[5]);
            label78.Text = LocalizationText.EnabledDisabled(MainFrm.Hmi_bArray[6]);
            label29.Text = LocalizationText.EnabledDisabled(MainFrm.Hmi_bArray[7]);
        }

        private void LoadFlipParameters()
        {
            for (int i = 0; i < txbFlipParameters.Length; i++)
            {
                txbFlipParameters[i].Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[95 + i]);
            }
        }

        private void txbFlipParameter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            SaveFlipParameters();
        }

        private void SaveFlipParameters()
        {
            for (int i = 0; i < txbFlipParameters.Length; i++)
            {
                MainFrm.Hmi_rArray[95 + i] = MainFrm.ParseDisplayLengthFloatOrZero(txbFlipParameters[i].Text);
                txbFlipParameters[i].Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[95 + i]);
                MainFrm.ConfigData[MainFrm.L8_AutoFeedPara + 5 + i] = MainFrm.Hmi_rArray[95 + i];
            }

            mf.AdsWritePlcFloat();
            for (int i = 5; i <= 9; i++)
                mf.wrtConfigFile("[AutoFeedPara]", i);
        }
        private TextBox AngleOffsetTextBox(string prefix, int index)
        {
            return (TextBox)this.Controls.Find(prefix + string.Format("{0:D2}", index), true)[0];
        }

        private void RefreshAngleMappingSelector(int selectedIndex)
        {
            int count = MainFrm.GetAngleAdditCount();
            cbx角度补偿0.SelectedIndexChanged -= cbx角度补偿0_SelectedIndexChanged;
            cbx角度补偿0.Items.Clear();
            for (int i = 0; i < count; i++)
                cbx角度补偿0.Items.Add(MainFrm.angleAddit[i].Type);

            if (count > 0)
            {
                lastAngleMappingIndex = Math.Clamp(selectedIndex, 0, count - 1);
                cbx角度补偿0.SelectedIndex = lastAngleMappingIndex;
                mf.RefreshAngleAdditMaterialList(lastAngleMappingIndex);
            }
            else
            {
                lastAngleMappingIndex = -1;
                cbx角度补偿0.SelectedIndex = -1;
                mf.RefreshAngleAdditMaterialList(-1);
            }

            cbx角度补偿0.SelectedIndexChanged += cbx角度补偿0_SelectedIndexChanged;
            Load角度映射();
        }

        private void ClearAngleMappingInputs()
        {
            txb角度补偿2.Text = "";
            txb角度补偿3.Text = "";
            txb角度补偿4.Text = "";
            txb角度补偿5.Text = "";
            for (int i = 0; i < MainFrm.AngleAdditVisibleRows; i++)
            {
                AngleOffsetTextBox("txb底部补偿", i).Text = "0.00";
                AngleOffsetTextBox("txb顶部补偿", i).Text = "0.00";
            }
        }

        private void Load角度映射()    //flag:选项从文件读取或手动选择
        {
            int id = cbx角度补偿0.SelectedIndex;
            if (!MainFrm.IsValidAngleAdditIndex(id))
            {
                ClearAngleMappingInputs();
                return;
            }

            lastAngleMappingIndex = id;
            MainFrm.EnsureAngleAdditItem(id);
            txb角度补偿2.Text = MainFrm.angleAddit[id].Material;
            txb角度补偿3.Text = MainFrm.angleAddit[id].Strength;
            txb角度补偿4.Text = MainFrm.angleAddit[id].Thickness;
            txb角度补偿5.Text = MainFrm.angleAddit[id].MachingGauging;
            for (int i = 0; i < MainFrm.AngleAdditVisibleRows; i++)
            {
                AngleOffsetTextBox("txb底部补偿", i).Text = string.Format(CultureInfo.CurrentCulture, "{0:F2}", MainFrm.angleAddit[id].AngleRange[i]);
                AngleOffsetTextBox("txb顶部补偿", i).Text = string.Format(CultureInfo.CurrentCulture, "{0:F2}", MainFrm.angleAddit[id].AngleRange[i + MainFrm.AngleAdditVisibleRows]);
            }
        }

        private void PersistAngleMappingSelection(int id)
        {
            if (!MainFrm.IsValidAngleAdditIndex(id))
                return;

            MainFrm.Hmi_rArray[56] = Convert.ToInt16(id);
            MainFrm.ConfigData[MainFrm.L7_ManualOldSelect + 9] = MainFrm.Hmi_rArray[56];
            mf.AdsWritePlc1float(56, MainFrm.Hmi_rArray[56]);
            mf.wrtConfigFile("[ManualOldSelect]", 9);
        }

        private void cbx角度补偿0_SelectedIndexChanged(object? sender, EventArgs e)
        {
            int id = cbx角度补偿0.SelectedIndex;
            if (!MainFrm.IsValidAngleAdditIndex(id))
                return;

            lastAngleMappingIndex = id;
            PersistAngleMappingSelection(id);
            Load角度映射();
        }
        private void sw分条机速度_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[2] = !MainFrm.Hmi_bArray[2];
            sw分条机速度.BackgroundImage = MainFrm.Hmi_bArray[2] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            mf.AdsWritePlc1Bit(2, MainFrm.Hmi_bArray[2]);
            MainFrm.ConfigData[MainFrm.L1_GlobalSwitch + 3] = Convert.ToSingle(MainFrm.Hmi_bArray[2]);
            mf.wrtConfigFile("[GlobalSwitch]", 3);
        }

        private void sw加载桌板_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[3] = !MainFrm.Hmi_bArray[3];
            sw加载桌板.BackgroundImage = MainFrm.Hmi_bArray[3] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            label75.Text = LocalizationText.EnabledDisabled(MainFrm.Hmi_bArray[3]);
            mf.AdsWritePlc1Bit(3, MainFrm.Hmi_bArray[3]);
            MainFrm.ConfigData[MainFrm.L1_GlobalSwitch + 4] = Convert.ToSingle(MainFrm.Hmi_bArray[3]);
            mf.wrtConfigFile("[GlobalSwitch]", 4);
        }

        private void sw角度显示_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[4] = !MainFrm.Hmi_bArray[4];
            sw角度显示.BackgroundImage = MainFrm.Hmi_bArray[4] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            mf.AdsWritePlc1Bit(4, MainFrm.Hmi_bArray[4]);
            MainFrm.ConfigData[MainFrm.L1_GlobalSwitch + 5] = Convert.ToSingle(MainFrm.Hmi_bArray[4]);
            mf.wrtConfigFile("[GlobalSwitch]", 5);
        }

        private void sw松开方式1_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[5] = !MainFrm.Hmi_bArray[5];
            sw松开方式1.BackgroundImage = MainFrm.Hmi_bArray[5] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            label77.Text = LocalizationText.AutoManual(MainFrm.Hmi_bArray[5]);
            mf.AdsWritePlc1Bit(5, MainFrm.Hmi_bArray[5]);
            MainFrm.ConfigData[MainFrm.L1_GlobalSwitch + 6] = Convert.ToSingle(MainFrm.Hmi_bArray[5]);
            mf.wrtConfigFile("[GlobalSwitch]", 6);
        }

        private void sw自动翻转_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[6] = !MainFrm.Hmi_bArray[6];
            sw自动翻转.BackgroundImage = MainFrm.Hmi_bArray[6] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            label78.Text = LocalizationText.EnabledDisabled(MainFrm.Hmi_bArray[6]);
            mf.AdsWritePlc1Bit(6, MainFrm.Hmi_bArray[6]);
            MainFrm.ConfigData[MainFrm.L1_GlobalSwitch + 7] = Convert.ToSingle(MainFrm.Hmi_bArray[6]);
            mf.wrtConfigFile("[GlobalSwitch]", 7);
            RefreshFlipParameterVisibility();
        }

        private void sw自动进料_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[7] = !MainFrm.Hmi_bArray[7];
            sw自动进料.BackgroundImage = MainFrm.Hmi_bArray[7] ? global::JSZW1000A.Properties.Resources.sw_左右小开关1 : global::JSZW1000A.Properties.Resources.sw_左右小开关0;
            label29.Text = LocalizationText.EnabledDisabled(MainFrm.Hmi_bArray[7]);
            mf.AdsWritePlc1Bit(7, MainFrm.Hmi_bArray[7]);
            MainFrm.ConfigData[MainFrm.L1_GlobalSwitch + 10] = Convert.ToSingle(MainFrm.Hmi_bArray[7]);
            mf.wrtConfigFile("[GlobalSwitch]", 10);

            groupBox6.Visible = (MainFrm.ConfigData[MainFrm.L1_GlobalSwitch + 10] > 0);
        }

        //后挡参数列中,有任何一个敲回车,下载该列数据到PLC并存入config.ini
        private void txb后挡参数0_KeyDown(object sender, KeyEventArgs e)
        {
            int idx = 0;
            // 判断：如果输入的是回车键
            if (e.KeyCode == Keys.Enter)
            {
                while (MainFrm.ConfigData[MainFrm.L3_BackgaugeApron + idx + 1] < 9990)
                {
                    TextBox txb = (TextBox)this.Controls.Find("txb后挡参数" + string.Format("{0:D2}", idx), true)[0];
                    MainFrm.Hmi_rArray[110 + idx] = MainFrm.ParseDisplayLengthFloatOrZero(txb.Text);
                    txb.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[110 + idx]);
                    MainFrm.ConfigData[MainFrm.L3_BackgaugeApron + idx + 1] = MainFrm.Hmi_rArray[110 + idx];
                    idx++;
                }
                mf.AdsWritePlcFloat();
                mf.wrtConfigFile("[BackgaugeApron]", 0);

            }
        }

        private void txb夹钳_低_KeyDown(object sender, KeyEventArgs e)
        {
            // 判断：如果输入的是回车键
            if (e.KeyCode == Keys.Enter)
            {
                MainFrm.Hmi_rArray[100] = MainFrm.ParseDisplayLengthFloatOrZero(txb夹钳_低.Text);
                MainFrm.Hmi_rArray[101] = MainFrm.ParseDisplayLengthFloatOrZero(txb夹钳_中.Text);
                MainFrm.Hmi_rArray[102] = MainFrm.ParseDisplayLengthFloatOrZero(txb夹钳_高.Text);
                MainFrm.Hmi_rArray[103] = MainFrm.ParseDisplayLengthFloatOrZero(txb夹钳_最大.Text);
                MainFrm.Hmi_rArray[104] = MainFrm.ParseDisplayLengthFloatOrZero(txb夹钳_半夹钳.Text);
                MainFrm.Hmi_rArray[105] = MainFrm.ParseDisplayLengthFloatOrZero(txb夹钳_挤压折弯.Text);
                txb夹钳_低.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[100]);
                txb夹钳_中.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[101]);
                txb夹钳_高.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[102]);
                txb夹钳_最大.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[103]);
                txb夹钳_半夹钳.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[104]);
                txb夹钳_挤压折弯.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[105]);

                MainFrm.ConfigData[MainFrm.L2_ClampHeight + 1] = MainFrm.Hmi_rArray[100];
                MainFrm.ConfigData[MainFrm.L2_ClampHeight + 2] = MainFrm.Hmi_rArray[101];
                MainFrm.ConfigData[MainFrm.L2_ClampHeight + 3] = MainFrm.Hmi_rArray[102];
                MainFrm.ConfigData[MainFrm.L2_ClampHeight + 4] = MainFrm.Hmi_rArray[103];
                MainFrm.ConfigData[MainFrm.L2_ClampHeight + 5] = MainFrm.Hmi_rArray[104];
                MainFrm.ConfigData[MainFrm.L2_ClampHeight + 6] = MainFrm.Hmi_rArray[105];

                mf.AdsWritePlcFloat();
                mf.wrtConfigFile("[ClampHeight]", 1);
                mf.wrtConfigFile("[ClampHeight]", 2);
                mf.wrtConfigFile("[ClampHeight]", 3);
                mf.wrtConfigFile("[ClampHeight]", 4);
                mf.wrtConfigFile("[ClampHeight]", 5);
                mf.wrtConfigFile("[ClampHeight]", 6);
            }
        }

        private void cbx末次抬钳高_SelectedIndexChanged(object? sender, EventArgs e)
        {
            MainFrm.Hmi_rArray[106] = Convert.ToSingle(cbx末次抬钳高.SelectedIndex);
            MainFrm.ConfigData[MainFrm.L2_ClampHeight + 7] = MainFrm.Hmi_rArray[106];
            mf.AdsWritePlc1float(106, MainFrm.Hmi_rArray[106]);
            mf.wrtConfigFile("[ClampHeight]", 7);
        }

        private void cbx默认滑台收缩值_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainFrm.ConfigData[MainFrm.L6_MachineSetup + 9] = Convert.ToSingle(cbx默认滑台收缩值.SelectedIndex + 2);
            mf.wrtConfigFile("[MachineSetup]", 9);
        }

        private void txb开口挤压下降高度_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                MainFrm.Hmi_rArray[129] = MainFrm.ParseDisplayLengthFloatOrZero(txb开口挤压下降高度.Text);
                txb开口挤压下降高度.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[129]);
                MainFrm.ConfigData[MainFrm.L6_MachineSetup + 8] = MainFrm.Hmi_rArray[129];
                mf.AdsWritePlc1float(129, MainFrm.Hmi_rArray[129]);
                mf.wrtConfigFile("[MachineSetup]", 8);
            }
        }

        private void btn角度映射保存_Click(object sender, EventArgs e)
        {
            int id = GetEditableAngleMappingIndex();
            if (!TryApplyAngleMappingInputs(id))
                return;

            mf.wrtAngleAdditFile(MainFrm.angleAddit[id].Type);
            RefreshAngleMappingSelector(id);
        }

        private void btn角度映射下载_Click(object sender, EventArgs e)
        {
            int id = GetEditableAngleMappingIndex();
            if (!TryApplyAngleMappingInputs(id))
                return;

            mf.AdsWritePlcFloat();
            PersistAngleMappingSelection(id);
            mf.RefreshAngleAdditMaterialList(id);
        }

        private int GetEditableAngleMappingIndex()
        {
            int count = MainFrm.GetAngleAdditCount();
            if (cbx角度补偿0.SelectedIndex >= 0 && cbx角度补偿0.SelectedIndex < count)
                return cbx角度补偿0.SelectedIndex;
            if (lastAngleMappingIndex >= 0 && lastAngleMappingIndex < count)
                return lastAngleMappingIndex;
            return -1;
        }

        private bool TryParseAngleOffset(TextBox textBox, out float value)
        {
            string text = textBox.Text.Trim();
            if (!float.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value)
                && !float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            {
                MessageBox.Show(Strings.Get("Setting.AngleMap.OffsetNumeric"));
                textBox.Focus();
                textBox.SelectAll();
                return false;
            }

            if (!MainFrm.IsAngleAdditOffsetInRange(value))
            {
                MessageBox.Show(Strings.Get("Setting.AngleMap.OffsetRange"));
                textBox.Focus();
                textBox.SelectAll();
                return false;
            }

            return true;
        }

        private bool TryParseMeasuredAngleOffset(out float value)
        {
            if (!MainFrm.TryParseAngleAdditMeasuredOffset(txb角度补偿5.Text, out value))
            {
                MessageBox.Show(Strings.Get("Setting.AngleMap.MeasuredOffsetNumeric"));
                txb角度补偿5.Focus();
                txb角度补偿5.SelectAll();
                return false;
            }

            if (!MainFrm.IsAngleAdditOffsetInRange(value))
            {
                MessageBox.Show(Strings.Get("Setting.AngleMap.MeasuredOffsetRange"));
                txb角度补偿5.Focus();
                txb角度补偿5.SelectAll();
                return false;
            }

            return true;
        }

        private bool TryValidateEffectiveAngleOffset(TextBox textBox, float effectiveOffset, string apronName, int row)
        {
            if (MainFrm.IsAngleAdditOffsetInRange(effectiveOffset))
                return true;

            string rowText = row.ToString(CultureInfo.CurrentCulture);
            string valueText = effectiveOffset.ToString("F2", CultureInfo.CurrentCulture);
            MessageBox.Show(string.Format(
                CultureInfo.CurrentCulture,
                Strings.Get("Setting.AngleMap.EffectiveOffsetOutOfRange"),
                apronName,
                rowText,
                valueText));
            textBox.Focus();
            textBox.SelectAll();
            return false;
        }

        private bool TryApplyAngleMappingInputs(int id)
        {
            if (!MainFrm.IsValidAngleAdditIndex(id))
            {
                MessageBox.Show(Strings.Get("Setting.AngleMap.SelectOrAddFirst"));
                cbx角度补偿0.Focus();
                return false;
            }

            string type = MainFrm.NormalizeAngleAdditType(cbx角度补偿0.Text);
            if (string.IsNullOrWhiteSpace(type))
            {
                MessageBox.Show(Strings.Get("Setting.AngleMap.TableNameRequired"));
                cbx角度补偿0.Focus();
                return false;
            }

            if (MainFrm.HasAngleAdditName(type, id))
            {
                MessageBox.Show(Strings.Get("Setting.AngleMap.TableNameExists"));
                cbx角度补偿0.Focus();
                return false;
            }

            MainFrm.EnsureAngleAdditItem(id);
            if (!TryParseMeasuredAngleOffset(out float measuredOffset))
                return false;

            MainFrm.angleAddit[id].Type = type;
            MainFrm.angleAddit[id].Material = txb角度补偿2.Text;
            MainFrm.angleAddit[id].Strength = txb角度补偿3.Text;
            MainFrm.angleAddit[id].Thickness = txb角度补偿4.Text;
            MainFrm.angleAddit[id].MachingGauging = measuredOffset.ToString("G9", CultureInfo.InvariantCulture);
            for (int i = 0; i < MainFrm.AngleAdditVisibleRows; i++)
            {
                TextBox bottomTextBox = AngleOffsetTextBox("txb底部补偿", i);
                TextBox topTextBox = AngleOffsetTextBox("txb顶部补偿", i);
                if (!TryParseAngleOffset(bottomTextBox, out float bottomOffset)
                    || !TryParseAngleOffset(topTextBox, out float topOffset))
                    return false;

                if (!TryValidateEffectiveAngleOffset(bottomTextBox, bottomOffset + measuredOffset, Strings.Get("Setting.AngleMap.BottomApron"), i + 1)
                    || !TryValidateEffectiveAngleOffset(topTextBox, topOffset + measuredOffset, Strings.Get("Setting.AngleMap.TopApron"), i + 1))
                    return false;

                MainFrm.angleAddit[id].AngleRange[i] = bottomOffset;
                MainFrm.angleAddit[id].AngleRange[i + MainFrm.AngleAdditVisibleRows] = topOffset;
                bottomTextBox.Text = string.Format(CultureInfo.CurrentCulture, "{0:F2}", bottomOffset);
                topTextBox.Text = string.Format(CultureInfo.CurrentCulture, "{0:F2}", topOffset);
            }

            if (!MainFrm.TryApplyAngleAdditToHmiArrays(id, out string errorMessage))
            {
                MessageBox.Show(errorMessage);
                return false;
            }

            txb角度补偿5.Text = string.Format(CultureInfo.CurrentCulture, "{0:F2}", measuredOffset);
            cbx角度补偿0.Text = type;
            lastAngleMappingIndex = id;
            return true;
        }

        private string BuildNewAngleMappingName()
        {
            string prefix = Strings.Get("Setting.AngleMap.NewTablePrefix");
            for (int i = 1; i <= MainFrm.AngleAdditTableCapacity; i++)
            {
                string name = MainFrm.NormalizeAngleAdditType(prefix + i.ToString(CultureInfo.InvariantCulture));
                if (!MainFrm.HasAngleAdditName(name, -1))
                    return name;
            }

            return MainFrm.NormalizeAngleAdditType(prefix);
        }

        private MainFrm.AngleAddit CopyAngleMappingFromCurrent(int sourceIndex, string newName)
        {
            MainFrm.EnsureAngleAdditItem(sourceIndex);
            MainFrm.AngleAddit copiedItem = MainFrm.CreateEmptyAngleAddit(newName);
            copiedItem.Material = MainFrm.angleAddit[sourceIndex].Material;
            copiedItem.Strength = MainFrm.angleAddit[sourceIndex].Strength;
            copiedItem.Thickness = MainFrm.angleAddit[sourceIndex].Thickness;
            copiedItem.MachingGauging = MainFrm.angleAddit[sourceIndex].MachingGauging;

            for (int i = 0; i < MainFrm.AngleAdditRangeCount; i++)
                copiedItem.AngleRange[i] = MainFrm.angleAddit[sourceIndex].AngleRange[i];

            return copiedItem;
        }

        private void btn角度映射新增_Click(object sender, EventArgs e)
        {
            int count = MainFrm.GetAngleAdditCount();
            if (count >= MainFrm.AngleAdditTableCapacity)
            {
                MessageBox.Show(Strings.Get("Setting.AngleMap.CapacityFull"));
                return;
            }

            int sourceIndex = GetEditableAngleMappingIndex();
            if (!MainFrm.IsValidAngleAdditIndex(sourceIndex))
            {
                MessageBox.Show(Strings.Get("Setting.AngleMap.SelectCopySource"));
                return;
            }

            MainFrm.angleAddit[count] = CopyAngleMappingFromCurrent(sourceIndex, BuildNewAngleMappingName());
            mf.wrtAngleAdditFile(MainFrm.angleAddit[count].Type);
            RefreshAngleMappingSelector(count);
            PersistAngleMappingSelection(count);
            cbx角度补偿0.Focus();
            cbx角度补偿0.SelectAll();
        }

        private void btn角度映射删除_Click(object sender, EventArgs e)
        {
            int id = GetEditableAngleMappingIndex();
            int count = MainFrm.GetAngleAdditCount();
            if (id < 0 || id >= count)
            {
                MessageBox.Show(Strings.Get("Setting.AngleMap.SelectDeleteTarget"));
                return;
            }
            if (count <= 1)
            {
                MessageBox.Show(Strings.Get("Setting.AngleMap.AtLeastOneRemain"));
                return;
            }

            string message = string.Format(
                CultureInfo.CurrentCulture,
                Strings.Get("Setting.AngleMap.DeleteConfirm"),
                MainFrm.angleAddit[id].Type);
            using DialogAsk dlgTips = new DialogAsk("", message);
            if (dlgTips.ShowDialog() != DialogResult.OK)
                return;

            for (int i = id; i < count - 1; i++)
                MainFrm.angleAddit[i] = MainFrm.angleAddit[i + 1];
            MainFrm.angleAddit[count - 1] = MainFrm.CreateEmptyAngleAddit();

            int selectedIndex = Math.Clamp(id, 0, count - 2);
            mf.wrtAngleAdditFile("");
            RefreshAngleMappingSelector(selectedIndex);
            PersistAngleMappingSelection(selectedIndex);
        }



        private static short ReadCellInt16(IXLWorksheet worksheet, int row, int column)
        {
            IXLCell cell = worksheet.Cell(row, column);
            if (cell.TryGetValue(out double numericValue))
                return Convert.ToInt16(numericValue);

            return Convert.ToInt16(cell.GetFormattedString());
        }

        private static float ReadCellSingleOrZero(IXLWorksheet worksheet, int row, int column)
        {
            IXLCell cell = worksheet.Cell(row, column);
            if (cell.IsEmpty())
                return 0;

            string text = cell.GetFormattedString();
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            if (cell.TryGetValue(out double numericValue))
                return Convert.ToSingle(numericValue);

            return Convert.ToSingle(text);
        }

        string filename = Path.Combine(System.Windows.Forms.Application.StartupPath, "AngleMap.xlsx");
        private void btn导入角度映像_Click(object sender, EventArgs e)
        {
            using XLWorkbook wbk = new XLWorkbook(filename);
            IXLWorksheet sh = wbk.Worksheet(1);
            for (int i = 0; i <= 378; i++)
            {
                if (i <= 155)
                {
                    MainFrm.Hmi_iAngleMapTop[i] = ReadCellInt16(sh, i + 2, 2);
                    MainFrm.Hmi_iAngleMapBtm[i] = ReadCellInt16(sh, i + 2, 3);
                }
                MainFrm.Hmi_iHeightMap[i] = ReadCellInt16(sh, i + 2, 4);
            }
            MainFrm.Hmi_rArray[1] = ReadCellInt16(sh, 2, 5);
            MainFrm.Hmi_rArray[2] = ReadCellInt16(sh, 222, 5);
            MainFrm.Hmi_rArray[3] = ReadCellInt16(sh, 2, 6);
            MainFrm.Hmi_rArray[4] = ReadCellInt16(sh, 222, 6);
            MainFrm.Hmi_rArray[5] = ReadCellInt16(sh, 2, 7);
            MainFrm.Hmi_rArray[6] = ReadCellInt16(sh, 222, 7);
            mf.AdsWritePlc_AngleMap();
            mf.AdsWritePlc1float(1, MainFrm.Hmi_rArray[1]);
            mf.AdsWritePlc1float(2, MainFrm.Hmi_rArray[2]);
            mf.AdsWritePlc1float(3, MainFrm.Hmi_rArray[3]);
            mf.AdsWritePlc1float(4, MainFrm.Hmi_rArray[4]);
            mf.AdsWritePlc1float(5, MainFrm.Hmi_rArray[5]);
            mf.AdsWritePlc1float(6, MainFrm.Hmi_rArray[6]);

            MessageBox.Show(Strings.Get("Setting.AngleMapUpdated"));
        }

        string filename2 = Path.Combine(System.Windows.Forms.Application.StartupPath, "Folder data values.xlsx");
        private void btn导入进阶参数_Click(object sender, EventArgs e)
        {
            string s0 = Strings.Get("Setting.ImportAdvancedParametersConfirm");
            DialogAsk dlgTips = new DialogAsk("", s0);
            //dlgTips.StartPosition = FormStartPosition.Manual;
            //dlgTips.Location = new Point(500, 200);           

            if (dlgTips.ShowDialog() == DialogResult.OK)
            {
                using XLWorkbook wbk = new XLWorkbook(filename2);
                IXLWorksheet sh = wbk.Worksheet(1);
                for (int i = 0; i < 100; i++)
                {
                    MainFrm.Hmi_rAdvPara[i] = ReadCellSingleOrZero(sh, i + 2, 3);
                }

                mf.AdsWritePlc_AdvPara();
                MessageBox.Show(Strings.Get("Setting.AdvancedParametersUpdated"));

            }
            else
            {
                ;
            }



        }

        private void btn解耦归零_MouseDown(object sender, MouseEventArgs e)
        {
            MainFrm.Hmi_bArray[11] = true;
            mf.AdsWritePlc();
        }

        private void btn解耦归零_MouseUp(object sender, MouseEventArgs e)
        {
            MainFrm.Hmi_bArray[11] = true;
            mf.AdsWritePlc();
        }

       

        private void txb进料参数0_KeyDown(object sender, KeyEventArgs e)
        {

            // 判断：如果输入的是回车键
            if (e.KeyCode == Keys.Enter)
            {
                MainFrm.Hmi_rArray[107] = MainFrm.ParseDisplayLengthFloatOrZero(txb进料参数00.Text);
                MainFrm.Hmi_rArray[109] = MainFrm.ParseDisplayLengthFloatOrZero(txb进料参数01.Text);
                MainFrm.Hmi_rArray[94] = MainFrm.ParseDisplayLengthFloatOrZero(txb进料参数02.Text);
                MainFrm.Hmi_rArray[108] = MainFrm.ParseDisplayLengthFloatOrZero(txb进料参数03.Text);
                txb进料参数00.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[107]);
                txb进料参数01.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[109]);
                txb进料参数02.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[94]);
                txb进料参数03.Text = MainFrm.FormatDisplayLength(MainFrm.Hmi_rArray[108]);

                MainFrm.ConfigData[MainFrm.L8_AutoFeedPara + 1] = MainFrm.Hmi_rArray[107];
                MainFrm.ConfigData[MainFrm.L8_AutoFeedPara + 2] = MainFrm.Hmi_rArray[109];
                MainFrm.ConfigData[MainFrm.L8_AutoFeedPara + 3] = MainFrm.Hmi_rArray[94];
                MainFrm.ConfigData[MainFrm.L8_AutoFeedPara + 4] = MainFrm.Hmi_rArray[108];

                mf.AdsWritePlcFloat();
                mf.wrtConfigFile("[AutoFeedPara]", 1);
                mf.wrtConfigFile("[AutoFeedPara]", 2);
                mf.wrtConfigFile("[AutoFeedPara]", 3);
                mf.wrtConfigFile("[AutoFeedPara]", 4);
            }
        }

        private void cbxLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLanguageSelectorUpdating || cbxLanguage.SelectedIndex < 0)
                return;

            AppLanguage selectedLanguage = (AppLanguage)cbxLanguage.SelectedIndex;
            btnApplyLanguage.Enabled = selectedLanguage != LocalizationManager.ReadConfiguredLanguage();
        }

        private bool isDisplayUnitSelectorUpdating;

        private void cbxDisplayUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isDisplayUnitSelectorUpdating || cbxDisplayUnit.SelectedIndex < 0)
                return;

            DisplayLengthUnit selectedUnit = cbxDisplayUnit.SelectedIndex == 0
                ? DisplayLengthUnit.Inch
                : DisplayLengthUnit.Millimeter;
            btnApplyDisplayUnit.Enabled = selectedUnit != DisplayUnitManager.ReadConfiguredDisplayUnit();
        }

        private void btnApplyLanguage_Click(object sender, EventArgs e)
        {
            if (cbxLanguage.SelectedIndex < 0)
                return;

            AppLanguage selectedLanguage = (AppLanguage)cbxLanguage.SelectedIndex;
            AppLanguage configuredLanguage = LocalizationManager.ReadConfiguredLanguage();
            if (selectedLanguage == configuredLanguage)
            {
                btnApplyLanguage.Enabled = false;
                return;
            }

            if (!LocalizationManager.SaveLanguage(selectedLanguage))
                return;

            btnApplyLanguage.Enabled = false;
            LocalizationManager.PromptReload(this);
            Application.Exit();
        }

        private void btnApplyDisplayUnit_Click(object sender, EventArgs e)
        {
            if (cbxDisplayUnit.SelectedIndex < 0)
                return;

            DisplayLengthUnit selectedUnit = cbxDisplayUnit.SelectedIndex == 0
                ? DisplayLengthUnit.Inch
                : DisplayLengthUnit.Millimeter;
            DisplayLengthUnit configuredUnit = DisplayUnitManager.ReadConfiguredDisplayUnit();
            if (selectedUnit == configuredUnit)
            {
                btnApplyDisplayUnit.Enabled = false;
                return;
            }

            if (!DisplayUnitManager.SaveDisplayUnit(selectedUnit))
            {
                MessageBox.Show(
                    this,
                    Strings.Get("Setting.DisplayUnit.SaveFailed", "显示单位保存失败，请检查 Config.ini。"),
                    Strings.Get("Common.ErrorTitle", "错误"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            DisplayUnitManager.UseSessionDisplayUnit(selectedUnit);
            btnApplyDisplayUnit.Enabled = false;

            mf.setLang();
            setLang();
            LoadInit();
            RefreshDisplayUnitSelector();
        }
    }
}
