using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace JSZW1000A
{
    public sealed class FrmInlineSlit : Form
    {
        private readonly double unitWidth;
        private readonly CheckBox chkEnable;
        private readonly TextBox txbTotalWidth;
        private readonly TextBox txbPieceCount;
        private readonly Label lbSummary;

        public FrmInlineSlit(double unitWidth, bool enabled, double totalWidth, int pieceCount)
        {
            this.unitWidth = unitWidth;

            BackColor = Color.FromArgb(67, 67, 67);
            ForeColor = Color.White;
            ClientSize = new Size(420, 250);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = Strings.Get("InlineSlit.Title");

            Font labelFont = MainFrm.Lang == 0
                ? new Font("宋体", 11.25F)
                : new Font("Calibri", 11.25F);
            Font valueFont = new Font("Calibri", 12F);

            Label lbCurrentWidth = new Label
            {
                AutoSize = true,
                Font = labelFont,
                ForeColor = Color.FromArgb(208, 208, 208),
                Location = new Point(24, 25),
                Text = Strings.Get("InlineSlit.CurrentWidth")
                    + MainFrm.FormatDisplayLengthWithUnit(unitWidth),
            };
            Controls.Add(lbCurrentWidth);

            chkEnable = new CheckBox
            {
                AutoSize = true,
                Font = labelFont,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Location = new Point(24, 58),
                Text = Strings.Get("InlineSlit.Enable"),
                Checked = enabled,
            };
            chkEnable.CheckedChanged += (_, _) => UpdateSummary();
            Controls.Add(chkEnable);

            Label lbTotalWidth = new Label
            {
                AutoSize = true,
                Font = labelFont,
                ForeColor = Color.FromArgb(208, 208, 208),
                Location = new Point(24, 96),
                Text = Strings.Get("InlineSlit.SheetWidth"),
            };
            Controls.Add(lbTotalWidth);

            txbTotalWidth = new TextBox
            {
                BackColor = Color.FromArgb(104, 110, 114),
                BorderStyle = BorderStyle.FixedSingle,
                Font = valueFont,
                ForeColor = Color.White,
                Location = new Point(128, 91),
                Size = new Size(95, 27),
                Text = totalWidth > 0
                    ? MainFrm.FormatDisplayLength(totalWidth)
                    : MainFrm.FormatDisplayLength(unitWidth),
            };
            txbTotalWidth.TextChanged += (_, _) => UpdateSummary();
            Controls.Add(txbTotalWidth);

            Label lbPieceCount = new Label
            {
                AutoSize = true,
                Font = labelFont,
                ForeColor = Color.FromArgb(208, 208, 208),
                Location = new Point(24, 133),
                Text = Strings.Get("InlineSlit.PieceCount"),
            };
            Controls.Add(lbPieceCount);

            txbPieceCount = new TextBox
            {
                BackColor = Color.FromArgb(104, 110, 114),
                BorderStyle = BorderStyle.FixedSingle,
                Font = valueFont,
                ForeColor = Color.White,
                Location = new Point(128, 128),
                Size = new Size(95, 27),
                Text = pieceCount > 0 ? pieceCount.ToString(CultureInfo.InvariantCulture) : "2",
            };
            txbPieceCount.TextChanged += (_, _) => UpdateSummary();
            Controls.Add(txbPieceCount);

            lbSummary = new Label
            {
                Font = labelFont,
                ForeColor = Color.FromArgb(208, 208, 208),
                Location = new Point(24, 171),
                Size = new Size(372, 34),
            };
            Controls.Add(lbSummary);

            Button btnOk = new Button
            {
                BackColor = Color.FromArgb(80, 80, 80),
                FlatStyle = FlatStyle.Flat,
                Font = labelFont,
                ForeColor = Color.White,
                Location = new Point(214, 210),
                Size = new Size(82, 30),
                Text = Strings.Get("InlineSlit.Ok"),
            };
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.Click += BtnOk_Click;
            Controls.Add(btnOk);

            Button btnCancel = new Button
            {
                BackColor = Color.FromArgb(80, 80, 80),
                FlatStyle = FlatStyle.Flat,
                Font = labelFont,
                ForeColor = Color.White,
                Location = new Point(314, 210),
                Size = new Size(82, 30),
                Text = Strings.Get("InlineSlit.Cancel"),
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (_, _) => DialogResult = DialogResult.Cancel;
            Controls.Add(btnCancel);

            AcceptButton = btnOk;
            CancelButton = btnCancel;

            UpdateSummary();
        }

        public bool InlineEnabled { get; private set; }
        public double TotalWidth { get; private set; }
        public int PieceCount { get; private set; }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            if (!chkEnable.Checked)
            {
                InlineEnabled = false;
                TotalWidth = 0;
                PieceCount = 0;
                DialogResult = DialogResult.OK;
                return;
            }

            if (!TryParseInputs(out double totalWidth, out int pieceCount, out _, out string message))
            {
                MessageBox.Show(
                    message,
                    Strings.Get("InlineSlit.InvalidTitle"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            InlineEnabled = true;
            TotalWidth = totalWidth;
            PieceCount = pieceCount;
            DialogResult = DialogResult.OK;
        }

        private void UpdateSummary()
        {
            if (!chkEnable.Checked)
            {
                lbSummary.ForeColor = Color.FromArgb(208, 208, 208);
                lbSummary.Text = Strings.Get("InlineSlit.DisabledSummary");
                return;
            }

            if (!TryParseInputs(out _, out int pieceCount, out double frontOffcutWidth, out string message))
            {
                lbSummary.ForeColor = Color.FromArgb(255, 210, 120);
                lbSummary.Text = message;
                return;
            }

            lbSummary.ForeColor = Color.FromArgb(128, 255, 128);
            lbSummary.Text = string.Format(
                CultureInfo.InvariantCulture,
                Strings.Get("InlineSlit.EnabledSummary"),
                pieceCount,
                MainFrm.FormatDisplayLength(frontOffcutWidth));
        }

        private bool TryParseInputs(out double totalWidth, out int pieceCount, out double frontOffcutWidth, out string message)
        {
            totalWidth = 0;
            pieceCount = 0;
            frontOffcutWidth = 0;
            message = "";

            if (!MainFrm.TryParseDisplayLength(txbTotalWidth.Text, out totalWidth))
            {
                message = Strings.Get("InlineSlit.Error.InvalidSheetWidth");
                return false;
            }

            if (!int.TryParse(txbPieceCount.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out pieceCount))
            {
                message = Strings.Get("InlineSlit.Error.InvalidPieceCount");
                return false;
            }

            if (totalWidth <= unitWidth)
            {
                message = Strings.Get("InlineSlit.Error.SheetWidthTooSmall");
                return false;
            }

            if (pieceCount <= 1)
            {
                message = Strings.Get("InlineSlit.Error.PieceCountTooSmall");
                return false;
            }

            frontOffcutWidth = totalWidth - unitWidth * pieceCount;
            if (frontOffcutWidth < -0.5)
            {
                message = Strings.Get("InlineSlit.Error.SheetWidthInsufficient");
                return false;
            }

            if (Math.Abs(frontOffcutWidth) <= 0.5)
                frontOffcutWidth = 0;

            return true;
        }
    }
}
