using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwinCAT.TypeSystem;

namespace JSZW400.SubWindows
{
    public partial class SubOPSlitter : UserControl
    {
        MainFrm mf;
        public SubOPSlitter(MainFrm fm1)
        {
            InitializeComponent(); setLang();
            this.mf = fm1;
        }

        private void SubOPSlitter_Load(object sender, EventArgs e)
        {

            dataGridView1.ColumnHeadersVisible = false;
            if (dataGridView1.RowCount < 1)
            {
                btn列表_之后插入.PerformClick();
            }
            txb单项宽.Tag = false;
            txb单项宽.GotFocus += new EventHandler(textBox_GotFocus);
            txb单项宽.MouseUp += new MouseEventHandler(textBox_MouseUp);
            txb单项数量.Tag = false;
            txb单项数量.GotFocus += new EventHandler(textBox_GotFocus);
            txb单项数量.MouseUp += new MouseEventHandler(textBox_MouseUp);
            txb总宽.Tag = false;
            txb总宽.GotFocus += new EventHandler(textBox_GotFocus);
            txb总宽.MouseUp += new MouseEventHandler(textBox_MouseUp);
        }

        public void setLang()
        {
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
            label3.Text = label6.Text = label7.Text = (MainFrm.Lang == 0) ? "边角料" : "Offcut";
            label9.Text = label10.Text = (MainFrm.Lang == 0) ? "毫米" : "mm";
            lb首先.Text = (MainFrm.Lang == 0) ? "首先" : "First";
            lb最后.Text = (MainFrm.Lang == 0) ? "最后" : "Last";
            label8.Text = (MainFrm.Lang == 0) ? "宽度" : "Width";
            label11.Text = (MainFrm.Lang == 0) ? "数量" : "Qty";

            btn装载材料.Text = (MainFrm.Lang == 0) ? "  装载材料" : "Load Material";
            btn分条机归原位.Text = (MainFrm.Lang == 0) ? "   分条机\r\n   归原位" : "Slitter Home";
            btn列表_之前插入.Text = (MainFrm.Lang == 0) ? " 在...之前插入" : "  Insert Before";
            btn列表_之后插入.Text = (MainFrm.Lang == 0) ? " 在...之后插入" : "  Insert After";
            btn清除.Text = (MainFrm.Lang == 0) ? "    清除  " : "     Delete     ";
            btn列表_上移.Text = (MainFrm.Lang == 0) ? "    上移  " : "     Move Up   ";
            btn列表_下移.Text = (MainFrm.Lang == 0) ? "    下移  " : "     Move Down   ";




        }

        void textBox_MouseUp(object sender, MouseEventArgs e)
        {
            TextBox txb = (TextBox)sender;
            //如果鼠标左键操作并且标记存在，则执行全选            
            if (e.Button == MouseButtons.Left && (bool)txb.Tag == true)
            {
                txb.SelectAll();
            }
            //取消全选标记             
            txb.Tag = false;

        }

        void textBox_GotFocus(object sender, EventArgs e)
        {
            TextBox txb = (TextBox)sender;
            txb.Tag = true;    //设置标记             
            //txb.SelectAll();   //注意1
            //oldVal = Convert.ToDouble(txb.Text);
        }






        private void timer500ms_Tick(object sender, EventArgs e)
        {
            pnl边角料选先.Visible = MainFrm.Hmi_bArray[81];
            pnl边角料选后.Visible = !MainFrm.Hmi_bArray[81];
            lb首先.ForeColor = MainFrm.Hmi_bArray[81] ? Color.FromArgb(96, 176, 255) : Color.White;
            lb最后.ForeColor = !MainFrm.Hmi_bArray[81] ? Color.FromArgb(96, 176, 255) : Color.White;
            sw边角料选.BackgroundImage = MainFrm.Hmi_bArray[81] ? global::JSZW400.Properties.Resources.sw_左右小开关1 : global::JSZW400.Properties.Resources.sw_左右小开关0;
            pnlSlitter.BackgroundImage = (MainFrm.Hmi_iArray[20] == 5) ? global::JSZW400.Properties.Resources.Slit5 : global::JSZW400.Properties.Resources.Slit6;
            lb生产数量状态.Text = MainFrm.Lang == 0 ? "当前数量 " : "Actual "
                + MainFrm.Hmi_iArray[80].ToString() + "/" + MainFrm.Hmi_iArray[81].ToString() +
                (MainFrm.Lang == 0 ? "; 任务号:" : ";  Task Num:")
                + MainFrm.Hmi_iArray[82].ToString();

        }

        void InsertRow(int i)
        {
            //dataGridView1.Rows.Clear();
            DataGridViewRow dr = new DataGridViewRow();
            dr.CreateCells(dataGridView1);
            dr.Cells[0].Value = MainFrm.Lang == 0 ? "宽度 0 mm; 数量 1" : "Width 0 mm; Qty 1";

            //添加的行作为第一行
            dataGridView1.Rows.Insert(i, dr);

        }

        private void btn列表_之前插入_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                InsertRow(dataGridView1.CurrentRow.Index);
                //更改选中行
                dataGridView1.ClearSelection();
                dataGridView1.Rows[dataGridView1.CurrentRow.Index - 1].Selected = true;
                dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.CurrentRow.Index - 1].Cells[0];
            }
            else
            {
                InsertRow(0);

            }
            FillTxbox();
        }

        private void btn列表_之后插入_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                InsertRow(dataGridView1.CurrentRow.Index + 1);
                //更改选中行
                dataGridView1.ClearSelection();
                dataGridView1.Rows[dataGridView1.CurrentRow.Index + 1].Selected = true;
                dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.CurrentRow.Index + 1].Cells[0];
            }
            else
            {
                InsertRow(0);
            }
            FillTxbox();
        }

        private void btn清除_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 1)
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
                MessageBox.Show(ex.ToString());
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
                MessageBox.Show(ex.ToString());
            }
            FillTxbox();

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            FillTxbox();

        }

        void FillTxbox()
        {
            int index = dataGridView1.SelectedRows[0].Index;
            string str = dataGridView1[0, index].Value.ToString();
            string[] strArray = str.Split(' '); //单字符切割(result : "aaa"  "bbscc"  "dd")
            txb单项宽.Text = strArray[1];
            txb单项数量.Text = strArray[4];



        }

        private void txb单项宽_KeyDown(object sender, KeyEventArgs e)
        {
            // 判断：如果输入的是回车键
            if (e.KeyCode == Keys.Enter)
            {
                int index = dataGridView1.SelectedRows[0].Index;
                txb单项宽.Text = string.Format("{0:F1}", Convert.ToSingle(txb单项宽.Text));
                if (MainFrm.Lang == 0)
                    dataGridView1[0, index].Value = "宽度 " + txb单项宽.Text + " mm; " + "数量 " + txb单项数量.Text;
                else
                    dataGridView1[0, index].Value = "Width " + txb单项宽.Text + " mm; " + "Qty " + txb单项数量.Text;

                CreateSlitterDraw();
                StoreData();
            }
        }

        private void txb单项数量_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int index = dataGridView1.SelectedRows[0].Index;
                txb单项宽.Text = string.Format("{0:F1}", Convert.ToSingle(txb单项宽.Text));
                if (MainFrm.Lang == 0)
                    dataGridView1[0, index].Value = "宽度 " + txb单项宽.Text + " mm; " + "数量 " + txb单项数量.Text;
                else
                    dataGridView1[0, index].Value = "Width " + txb单项宽.Text + " mm; " + "Qty " + txb单项数量.Text;

                CreateSlitterDraw();
                StoreData();
            }
        }


        private void btn边角料选_Click(object sender, EventArgs e)
        {
            MainFrm.Hmi_bArray[81] = !MainFrm.Hmi_bArray[81];
            sw边角料选.BackgroundImage = MainFrm.Hmi_bArray[81] ? global::JSZW400.Properties.Resources.sw_左右小开关1 : global::JSZW400.Properties.Resources.sw_左右小开关0;
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
                string str = row.Cells[0].Value.ToString();
                string[] strArray = str.Split(' ');
                if (Convert.ToInt32(strArray[4]) == 0 || Convert.ToSingle(strArray[1]) == 0)
                    continue;
                for (int i = 0; i < Convert.ToInt32(strArray[4]); i++)
                {
                    List_Width[List_count] = Convert.ToSingle(strArray[1]);
                    List_TotalWidth += List_Width[List_count];
                    List_count++;
                }
            }
            lb边角料长度_先.Text = (Convert.ToSingle(txb总宽.Text) - List_TotalWidth).ToString() + "mm";
            lb边角料长度_后.Text = lb边角料长度_先.Text;
            draw();
        }

        void StoreData()
        {
            /*
            for (int j = 0; j < MainFrm.Hmi_rSlitter.Length; j++)
                MainFrm.Hmi_rSlitter[j] = 0;

            int i = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string str = row.Cells[0].Value.ToString();
                string[] strArray = str.Split(' ');
                if (Convert.ToInt32(strArray[4]) == 0 || Convert.ToSingle(strArray[1]) == 0)
                    continue;
                MainFrm.Hmi_rSlitter[10 + i] = Convert.ToSingle(strArray[1]);
                MainFrm.Hmi_rSlitter[22 + i] = Convert.ToSingle(strArray[4]);

                i++;
            }
            MainFrm.Hmi_rSlitter[0] = Convert.ToSingle(txb总宽.Text);
            */
        }

        void ReadData()
        {
            txb总宽.Text = MainFrm.Hmi_rSlitter[0].ToString();

            int i = 0;
            while (MainFrm.Hmi_rSlitter[22 + i] > 0)
            {
                DataGridViewRow dr = new DataGridViewRow();
                dr.CreateCells(dataGridView1);
                if (MainFrm.Lang == 0)
                    dr.Cells[0].Value = "宽度 " + MainFrm.Hmi_rSlitter[10 + i].ToString() + " mm; " + "数量 " + MainFrm.Hmi_rSlitter[22 + i].ToString();
                else
                    dr.Cells[0].Value = "Width " + MainFrm.Hmi_rSlitter[10 + i].ToString() + " mm; " + "Qty " + MainFrm.Hmi_rSlitter[22 + i].ToString();
                //添加的行作为第一行
                dataGridView1.Rows.Insert(i, dr);



            }



        }

        void Dnload()
        {
            for (int j = 0; j < MainFrm.Hmi_rSlitter.Length; j++)
                MainFrm.Hmi_rSlitter[j] = 0;

            int i = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string str = row.Cells[0].Value.ToString();
                string[] strArray = str.Split(' ');
                if (Convert.ToInt32(strArray[4]) == 0 || Convert.ToSingle(strArray[1]) == 0)
                    continue;
                MainFrm.Hmi_rSlitter[10 + i] = Convert.ToSingle(strArray[1]);
                MainFrm.Hmi_rSlitter[22 + i] = Convert.ToSingle(strArray[4]);

                i++;
            }

            MainFrm.Hmi_rSlitter[0] = Convert.ToSingle(txb总宽.Text);
            MainFrm.Hmi_rSlitter[1] = (MainFrm.Hmi_rSlitter[0] - List_TotalWidth);
            MainFrm.Hmi_rSlitter[2] = (float)i;

            mf.AdsWritePlc();

        }
        private void button1_Click(object sender, EventArgs e)
        {

        }

        void draw()
        {
            int T总板宽 = Convert.ToInt32(Convert.ToSingle(txb总宽.Text));
            int 分条总宽 = Convert.ToInt32(List_TotalWidth);

            Graphics g1 = pictureBox1.CreateGraphics();
            g1.Clear(Color.FromArgb(70, 70, 70));
            GraphicsUnit units = GraphicsUnit.Pixel;
            RectangleF srcRect = new RectangleF(0, 0, 605, 241);
            g1.DrawImage((Image)global::JSZW400.Properties.Resources.bg_Slitter绘图2, 0, 0, srcRect, units);
            g1.SmoothingMode = SmoothingMode.AntiAlias;
            //高质量，低速度绘制
            g1.CompositingQuality = CompositingQuality.HighQuality;


            Int32 边 = 5;
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
        private FrmCalculator dlgCal = null;
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

        private Object txbActiveTxb;
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
                txb总宽.Text = string.Format("{0:F1}", Convert.ToSingle(txb总宽.Text));
                CreateSlitterDraw();
                StoreData();
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

        private void btn拉刀前进_MouseDown(object sender, MouseEventArgs e)
        {
            MainFrm.Hmi_bArray[88] = true;
            mf.AdsWritePlc();
        }

        private void btn拉刀前进_MouseUp(object sender, MouseEventArgs e)
        {
            MainFrm.Hmi_bArray[88] = false;
            mf.AdsWritePlc();
        }

        private void btn拉刀后退_MouseDown(object sender, MouseEventArgs e)
        {
            MainFrm.Hmi_bArray[89] = true;
            mf.AdsWritePlc();
        }

        private void btn拉刀后退_MouseUp(object sender, MouseEventArgs e)
        {
            MainFrm.Hmi_bArray[89] = false;
            mf.AdsWritePlc();
        }
    }
}
