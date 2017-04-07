using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Parsing;
using Prueba_Gramatica_IRONY_Proyecto2.Graficacion;
using System.Windows.Forms;

namespace Prueba_Gramatica_IRONY_Proyecto2.Analizador
{
    class Sintactico : Grammar
    {
        public static bool analizar(String cadena)
        {
            Grammar gramatica = new Gramatica();
            LanguageData lenguaje = new LanguageData(gramatica);
            Parser parser = new Parser(lenguaje);
            ParseTree arbol = parser.Parse(cadena);
            ParseTreeNode raiz = arbol.Root;

            if(raiz == null)
            {
                return false;
            }
            else
            {
                String cadenaDOT = ArchivoDot.getDot(raiz);
                Reporte rep = new Reporte();
                if (rep.generarImagenAST(cadenaDOT))
                {
                    MessageBox.Show("Archivo dot creado exitosamente");
                }
                return true;
            }
        }
    }
}
