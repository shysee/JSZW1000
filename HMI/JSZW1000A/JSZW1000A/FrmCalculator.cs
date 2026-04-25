using JSZW1000A.SubWindows;

namespace JSZW1000A
{
    public partial class FrmCalculator : Form
    {
        private bool _embeddedMode;

        UserControl userFather;

        public FrmCalculator(UserControl sub)
        {
            InitializeComponent();
            userFather = sub;
        }

        public void SetEmbeddedMode(bool embeddedMode)
        {
            _embeddedMode = embeddedMode;
            TopLevel = !embeddedMode;
            TopMost = false;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;
        }
        public string getval()
        {
            return textBox1.Text;
        }

        private void DispatchKey(string key)
        {
            if (userFather is SubOPManual manual)
                manual.sendkey(key);
            else if (userFather is SubOPSlitter slitter)
                slitter.sendkey(key);
            else if (userFather is SubOPAuto auto)
                auto.sendkey(key);
        }

        private void btnNum0_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn)
                return;

            textBox1.Text = btn.Name switch
            {
                "btnNum0" => "0",
                "btnNum1" => "1",
                "btnNum2" => "2",
                "btnNum3" => "3",
                "btnNum4" => "4",
                "btnNum5" => "5",
                "btnNum6" => "6",
                "btnNum7" => "7",
                "btnNum8" => "8",
                "btnNum9" => "9",
                "btnNumPt" => ".",
                "btnClr" => "CLR",
                "btnNeg" => "-",
                _ => ""
            };

            if (textBox1.Text.Length > 0)
                DispatchKey(textBox1.Text);
        }

        private void btnEnter_Click(object? sender, EventArgs e)
        {
            DispatchKey("ENTER");

            if (_embeddedMode)
                return;

            Dispose();
        }

        private Point mouseOffset; // 鼠标移动距离
        private bool isMouseDown = false; // 是否按下鼠标

        private void FrmCalculator_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOffset = new Point(-e.X, -e.Y);
                isMouseDown = true;
            }
        }

        private void FrmCalculator_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = false;
            }
        }

        private void FrmCalculator_MouseMove(object? sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                Location = mousePos;
            }
        }

        private void btnBackspace_Click(object? sender, EventArgs e)
        {
            if (_embeddedMode)
            {
                Hide();
                return;
            }

            Dispose();
        }
    }
}
