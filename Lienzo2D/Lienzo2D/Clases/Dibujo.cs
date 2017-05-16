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
            Color colo = ColorTranslator.FromHtml(color);
            SolidBrush Lapiz = new SolidBrush(colo);
            Rectangle rec = new Rectangle(posx-diamtro/2, posy-diamtro/2, diamtro, diamtro);
            h.FillEllipse(Lapiz, rec);
            Lapiz.Dispose();
            h.Dispose();
        }

        public static void Pintar_Ovalo_Rectangulo(Graphics h, int posx, int posy, string cadena, int ancho, int alto, string fig)
        {
            switch (fig)
            {
                case "'r'":
                    {
                        PintarRectangulo(h, posx, posy, cadena, ancho, alto);
                        break;
                    }
                case "'o'":
                    {
                        PintarOvalo(h, posx, posy, cadena, ancho, alto);
                        break;
                    }
            }
        }

        public static void PintarRectangulo(Graphics h,int posx, int posy, string cadena, int ancho, int alto)
        {
            Color color = ColorTranslator.FromHtml(cadena);
            SolidBrush Lapiz = new SolidBrush(color);
            Rectangle r = new Rectangle(posx, posy, ancho, alto);
            h.FillRectangle(Lapiz, r);
            Lapiz.Dispose();
            h.Dispose();
        }

        public static void PintarOvalo(Graphics h, int posx, int posy, string cadena, int ancho, int alto)
        {
            Color color = ColorTranslator.FromHtml(cadena);
            SolidBrush Lapiz = new SolidBrush(color);
            Rectangle r = new Rectangle(posx-ancho/2, posy-alto/2, ancho, alto);
            h.FillEllipse(Lapiz, r);
            Lapiz.Dispose();
            h.Dispose();
        }
    }
}
