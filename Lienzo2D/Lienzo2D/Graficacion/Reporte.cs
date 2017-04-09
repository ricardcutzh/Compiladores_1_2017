using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
namespace Lienzo2D.Graficacion
{
    class Reporte
    {
        String rutaImagen { get; set; }
        public Reporte()
        {
            //CONSTRUCTOR POR DEFECTO
        }

        public bool generarImagenAST(String CadenaDot)
        {
            String ruta;
            try
            {
                ruta = "C:\\Reportes\\AST.dot";
                //ruta = "AST.dot";
                File.WriteAllText(ruta, CadenaDot);
                Process.Start("cmd.exe", "/c cd C:\\Program Files (x86)\\Graphviz2.38\\bin & dot -Tpng C:\\Reportes\\AST.dot  -o C:\\Reportes\\AST.png");
                System.Threading.Thread.Sleep(2000);
                crearHTMLArbol();
                Process.Start("C:\\Reportes\\ArbolAst.html");
            }
            catch
            {
                return false;
            }
            rutaImagen = ruta;
            return true;
        }

        private void crearHTMLArbol()
        {
            try
            {
                StreamWriter wr = new StreamWriter("C:\\Reportes\\ArbolAst.html");
                wr.WriteLine("<html>");
                wr.WriteLine("<head>");
                wr.WriteLine("<title>Arbol AST | Lienzo 2D</title>");
                wr.WriteLine("<link href=\"bootstrap/css/bootstrap.min.css\" rel=\"stylesheet\">");
                wr.WriteLine("<link rel=\"stylesheet\" href=\"https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css \" integrity=\"sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u\" crossorigin=\"anonymous\">");
                wr.WriteLine("</head>");
                wr.WriteLine("<body style=\"background-color:darkgray; \">");
                wr.WriteLine("<br>");
                wr.WriteLine("<div class=\"container\">");

                wr.WriteLine("<div class=\"jumbotron\">");
                wr.WriteLine("<h1>Arbol AST</h1>");
                wr.WriteLine("<p>");
                wr.WriteLine("<h4>Reporte de Arbol AST: LIENZO 2D</h4>");
                wr.WriteLine("</p>");
                wr.WriteLine("</div>");

                wr.WriteLine("<div class=\"panel panel-primary\">");
                wr.WriteLine("<div class=\"panel-heading\">Imagen AST</div>");
                wr.WriteLine("<div class=\"panel-body\">");
                wr.WriteLine("<p>");
                wr.WriteLine("<img src=\"AST.png\" class=\"img-responsive\" alt=\"Responsive image\">");
                wr.WriteLine("</p>");
                wr.WriteLine("</div>");
                wr.WriteLine("</div>");

                wr.WriteLine("</div>");
                wr.WriteLine("<script src=\"https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js \"></script>");
                wr.WriteLine("<script src=\"bootstrap/js/bootstrap.min.js\"></script>");
                wr.WriteLine("<script src=\"https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js \" integrity=\"sha384-Tc5IQib027qvyjSMfHjOMaLkfuWVxZxUPnCJA7l2mCWNIpG9mGCD8wGNIcPD7Txa\" crossorigin=\"anonymous\"></script>");
                wr.WriteLine("</body>");
                wr.WriteLine("</html>");

                wr.Close();
            }
            catch
            {
                MessageBox.Show("Error En la creación de HTML");
            }
        }
      
    }
}
