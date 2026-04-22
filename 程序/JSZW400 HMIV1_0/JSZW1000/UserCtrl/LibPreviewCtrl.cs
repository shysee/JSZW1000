using JSZW400.SubWindows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JSZW400.UserCtrl
{
    public partial class LibPreviewCtrl : UserControl
    {
        MainFrm mf;
        SubOPLibrary parent;
        public SubOPLibrary subOpLib;
        public int idx;
        double zoom = 1.0;
        public List<Point> pxList_Zoom = new List<Point>();
        public LibPreviewCtrl(int id, MainFrm mf, SubOPLibrary parent)
        {
            InitializeComponent();
            this.idx = id;
            this.mf = mf;
            this.parent = parent;
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            

         
        }

        private void LibPreviewCtrl_Load(object sender, EventArgs e)
        {
            lbName.Text = MainFrm.GblOrder[idx].Name;
            
        }

        private void paint()
        {
            Graphics g1 = panel1.CreateGraphics();
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

            int cx = this.Size.Width / 2, cy = (this.Size.Height - 30) / 2;

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
                Point p = new Point();
                if (i == 0)
                {
                    p.X = Convert.ToInt32((Convert.ToDouble(MainFrm.GblOrder[idx].pxList[i].X) - deltaX - cx) * zoom + cx);  //起始位置需要动态
                    p.Y = Convert.ToInt32((Convert.ToDouble(MainFrm.GblOrder[idx].pxList[i].Y) - deltaY - cy) * zoom + cy);
                }
                else
                {
                    int p2x = Convert.ToInt32((Convert.ToDouble(MainFrm.GblOrder[idx].pxList[i].X) - deltaX - cx) * zoom + cx);
                    int p2y = Convert.ToInt32((Convert.ToDouble(MainFrm.GblOrder[idx].pxList[i].Y) - deltaY - cy) * zoom + cy);
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

        private void LibPreviewCtrl_Paint(object sender, PaintEventArgs e)
        {
            paint();
        }
    }
}
