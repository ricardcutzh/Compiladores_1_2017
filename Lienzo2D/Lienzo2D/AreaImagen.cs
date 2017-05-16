using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lienzo2D.Clases;
using System.Drawing.Imaging;

namespace Lienzo2D
{
    public partial class AreaImagen : Form
    {
        public AreaImagen()
        {
            InitializeComponent();
        }

        public static Panel p = new Panel();
        public static Bitmap map = new Bitmap(1000, 1000);
        public static Graphics g;

        private void AreaImagen_Load(object sender, EventArgs e)
        {
            //g = Picture.CreateGraphics();
            
            g = Graphics.FromImage(map);
        }

     

        private void Picture_Paint(object sender, PaintEventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Salida.SizeMode = PictureBoxSizeMode.AutoSize;
            Salida.Refresh();
            Salida.Image = map;
            Salida.Refresh();
        }
    }
}
