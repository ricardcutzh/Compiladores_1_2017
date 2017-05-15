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

namespace Lienzo2D
{
    public partial class AreaImagen : Form
    {
        public AreaImagen()
        {
            InitializeComponent();
        }

        public static Graphics g;

        private void AreaImagen_Load(object sender, EventArgs e)
        {
            g = Picture.CreateGraphics();
            this.BringToFront();
        }

     

        private void Picture_Paint(object sender, PaintEventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Dibujo.figuraPrueba(g);
               
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
