using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using Irony.Interpreter;
using Irony.Ast;

namespace Lienzo2D.Analizador
{
    class Sintactico
    {
        public static List<ErrorEnAnalisis> errores = new List<ErrorEnAnalisis>();
        public static ParseTreeNode raizDeArbol;

        public static bool analizar(String cadena)
        {
            Gramatica gramatica = new Gramatica();
            LanguageData lenguaje = new LanguageData(gramatica);
            Parser parser = new Parser(lenguaje);
            ParseTree arbol = parser.Parse(cadena);
            ParseTreeNode raiz = arbol.Root;
            errores = gramatica.getErrores();
            if (raiz == null)
            {
                return false;
            }
            else
            {
                raizDeArbol = raiz;
                return true;
            }
        }
    }
}
