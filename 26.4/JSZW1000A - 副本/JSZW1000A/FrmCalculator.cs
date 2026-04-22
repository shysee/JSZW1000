using JSZW1000A.SubWindows;

namespace JSZW1000A
{
    public partial class FrmCalculator : Form
    {

        UserControl userFather;

        public FrmCalculator(UserControl sub)
        {
            InitializeComponent();
            userFather = sub;
        }
        public string getval()
        {
            return textBox1.Text;
        }

        private void btnNum0_Click(object sender, EventArgs e)
        {

            System.Windows.Forms.Button btn = (System.Windows.Forms.Button)sender;
            textBox1.Text = "";
            if (btn.Name == "btnNum0")
            {
                textBox1.Text = "0";
            }
            else if (btn.Name == "btnNum1")
            {
                textBox1.Text = "1";
            }
            else if (btn.Name == "btnNum2")
            {
                textBox1.Text = "2";
            }
            else if (btn.Name == "btnNum3") { textBox1.Text = "3"; }
            else if (btn.Name == "btnNum4") { textBox1.Text = "4"; }
            else if (btn.Name == "btnNum5") { textBox1.Text = "5"; }
            else if (btn.Name == "btnNum6") { textBox1.Text = "6"; }
            else if (btn.Name == "btnNum7") { textBox1.Text = "7"; }
            else if (btn.Name == "btnNum8") { textBox1.Text = "8"; }
            else if (btn.Name == "btnNum9") { textBox1.Text = "9"; }
            else if (btn.Name == "btnNumPt") { textBox1.Text = "."; }
            else if (btn.Name == "btnClr") { textBox1.Text = "CLR"; }
            //else if (btn.Name == "btnBackspace") { textBox1.Text = "BACKSPACE"; }
            else if (btn.Name == "btnNeg") { textBox1.Text = "-"; }

            if (userFather is SubOPManual f1)
                f1.sendkey(textBox1.Text);
            if (userFather is SubOPSlitter f2)
                f2.sendkey(textBox1.Text);
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            if (userFather is SubOPManual f1)
                f1.sendkey("ENTER");
            if (userFather is SubOPSlitter f2)
                f2.sendkey("ENTER");

            this.Dispose();
        }

        private Point mouseOffset; // 鼠标移动距离
        private bool isMouseDown = false; // 是否按下鼠标

        private void FrmCalculator_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOffset = new Point(-e.X, -e.Y);
                isMouseDown = true;
            }
        }

        private void FrmCalculator_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = false;
            }
        }

        private void FrmCalculator_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                Location = mousePos;
            }
        }

        private void btnBackspace_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        //private void Form_MouseDown(object sender, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Left)
        //    {
        //        mouseOffset = new Point(-e.X, -e.Y);
        //        isMouseDown = true;
        //    }
        //}

        //private void Form_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (isMouseDown)
        //    {
        //        Point mousePos = Control.MousePosition;
        //        mousePos.Offset(mouseOffset.X, mouseOffset.Y);
        //        Location = mousePos;
        //    }
        //}

        //private void Form_MouseUp(object sender, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Left)
        //    {
        //        isMouseDown = false;
        //    }
        //}
    }
}
