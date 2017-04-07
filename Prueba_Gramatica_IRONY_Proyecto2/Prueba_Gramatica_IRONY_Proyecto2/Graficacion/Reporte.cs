using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Prueba_Gramatica_IRONY_Proyecto2.Graficacion
{
    class Reporte
    {
        public Reporte()
        {
            //CONSTRUCTOR POR DEFECTO
        }

        public bool generarImagenAST(String CadenaDot)
        {
            try
            {
                String ruta = "C:\\Reportes\\AST.dot";
                File.WriteAllText(ruta, CadenaDot);
                Process.Start("cmd.exe", "/k cd C:\\Program Files (x86)\\Graphviz2.38\\bin & dot -Tpng C:\\Reportes\\AST.dot  -o C:\\Reportes\\AST.png");
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
