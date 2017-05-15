using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lienzo2D.Clases
{
    class Dibujo
    {
        public static Graphics figura;
        static Pen lapiz = new Pen(Color.Blue);

        public static void figuraPrueba(Graphics h)
        {
            SolidBrush mybrush = new SolidBrush(Color.Black);
            Pen pen = new Pen(mybrush);
            h.DrawLine(pen,10,10,50,50);
            mybrush.Dispose();
            h.Dispose();
        }

        public static void Pintar_Punt(Graphics h, int posx, int posy, string color, int diamtro)
        {

        }
    }
}
