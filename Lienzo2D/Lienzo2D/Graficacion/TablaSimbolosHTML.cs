using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lienzo2D.Clases;
using System.IO;
namespace Lienzo2D.Graficacion
{
    class TablaSimbolosHTML
    {
        List<Simbolo> Simbolos = new List<Simbolo>();

        int año = DateTime.Now.Year;
        int mes = DateTime.Now.Month;
        int dia = DateTime.Now.Day;
        int hora = DateTime.Now.Hour;
        int minuto = DateTime.Now.Minute;
        int segundo = DateTime.Now.Second;

        public TablaSimbolosHTML(List<Simbolo> Simbolos)
        {
            this.Simbolos = Simbolos;
            //TABLA DE SIMBOLS
        }


        public Boolean generarTablaHTML()
        {
            Boolean bandera = false;
            try
            {
                StreamWriter wr = new StreamWriter("C:\\Reportes\\TablaDeSimbolos.html");
                wr.WriteLine("<html>");
                wr.WriteLine("<head>");
                wr.WriteLine("<title>Tabla de Simbolos | Lienzo 2D</title>");
                wr.WriteLine("<link href=\"bootstrap/css/bootstrap.min.css\" rel=\"stylesheet\">");
                wr.WriteLine("<link rel=\"stylesheet\" href=\"https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css \" integrity=\"sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u\" crossorigin=\"anonymous\">");
                wr.WriteLine("</head>");
                wr.WriteLine("<body style=\"background-color:darkgray; \">");
                wr.WriteLine("<br>");
                wr.WriteLine("<div class=\"container\">");

                wr.WriteLine("<div class=\"jumbotron\">");
                wr.WriteLine("<h1>Tabla de Simbolos</h1>");
                wr.WriteLine("<p>");
                wr.WriteLine("<h4>Reporte Tabla de Simbolos </h4>");
                wr.WriteLine("</p>");
                wr.WriteLine("<p>");
                wr.WriteLine("<h5>: Dia de Ejecucion: " + dia + " de " + getMes(mes) + " del " + año + " </h5>");
                wr.WriteLine("</p>");
                wr.WriteLine("<p>");
                wr.WriteLine("<h5>: Hora de Ejecucion: " + hora + ":" + minuto + ":" + segundo + " </h5>");
                wr.WriteLine("</p>");
                wr.WriteLine("<p>");
                wr.WriteLine("<h5>: Nombre: Ricardo Cutz Hernandez, 201503476 </h5>");
                wr.WriteLine("</p>");
                wr.WriteLine("</div>");

                wr.WriteLine("<div class=\"panel panel-primary\">");
                wr.WriteLine("<div class=\"panel-heading\">Tabla de Simbolos</div>");
                wr.WriteLine("<div class=\"panel-body\">");

                wr.WriteLine("<p>");

                wr.WriteLine("</p>");

                wr.Write("<table class=\"table table-bordered table-striped table-hover\">");
                wr.Write("<tr>");
                wr.Write("<th>Nombre</th>");
                wr.Write("<th>Tipo</th>");
                wr.Write("<th>Visibilidad</th>");
                //wr.Write("<th>Valor</th>");
                wr.Write("<th>Ambito</th>");
                wr.Write("<th>Conservar</th>");
                wr.Write("<th>Es Arreglo</th>");
                wr.Write("<th>Dimensiones</th>");
                wr.Write("</tr>");
                //AQUI IRIA LA TABLA
                foreach (Simbolo sim in this.Simbolos)
                {
                    wr.Write("<tr>");
                    wr.Write("<td>" + sim.nombre + "</td>");
                    wr.Write("<td>" + sim.tipo + "</td>");
                    wr.Write("<td>" + sim.visibilidad + "</td>");
                    //wr.Write("<td>" + sim.valor + "</td>");
                    wr.Write("<td>" + sim.ambito + "</td>");
                    wr.Write("<td>" + Aplica(sim.conservar) + "</td>");
                    wr.Write("<td>" + Aplica(sim.esArreglo) + "</td>");
                    wr.Write("<td>" + dimensiones(sim.dimensiones) + "</td>");
                }
                wr.Write("</table>");

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
                bandera = false;
            }
            return bandera;
        }

        private String getMes(int mes)
        {
            String m = "";
            switch (mes)
            {
                case 1:
                    m = "Enero";
                    break;
                case 2:
                    m = "Febrero";
                    break;
                case 3:
                    m = "Marzo";
                    break;
                case 4:
                    m = "Abril";
                    break;
                case 5:
                    m = "Mayo";
                    break;
                case 6:
                    m = "Junio";
                    break;
                case 7:
                    m = "Julio";
                    break;
                case 8:
                    m = "Agosto";
                    break;
                case 9:
                    m = "Septiembre";
                    break;
                case 10:
                    m = "Octubre";
                    break;
                case 11:
                    m = "Noviembre";
                    break;
                default:
                    m = "Diciembre";
                    break;
            }
            return m;

        }

        private String Aplica(Boolean val)
        {
            if (val)
            {
                return "Si";
            }
            else
            {
                return "No";
            }
        }

        private String dimensiones(int dim)
        {
            if (dim == 0)
            {
                return "No Aplica";
            }
            else
            {
                return dim.ToString();
            }
        }
    }
}
