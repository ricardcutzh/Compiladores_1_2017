using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Parsing;

namespace Lienzo2D.Graficacion
{
    class ArchivoDot
    {
        public static int contador;
        public static String grafo;

        public static String getDot(ParseTreeNode raiz)
        {
            grafo = "digraph G {";
            grafo += "nodo0[label=\"" + quitarComillas(raiz.ToString()) + "\"];\n";
            contador = 1;
            recorreArbolAST("nodo0", raiz);
            grafo += "}";
            return grafo;
        }

        private static void recorreArbolAST(String padre, ParseTreeNode hijos)
        {
            foreach (ParseTreeNode hijo in hijos.ChildNodes)
            {
                String nombrehijo = "nodo" + contador.ToString();
                grafo += nombrehijo + "[label=\"" + quitarComillas(hijo.ToString()) + "\"];\n";
                grafo += padre + "->" + nombrehijo + ";\n";
                contador++;
                recorreArbolAST(nombrehijo, hijo);
            }
        }

        private static String quitarComillas(String cadena)
        {
            cadena = cadena.Replace("\\", "\\\\");
            cadena = cadena.Replace("\"", "\\\"");
            return cadena;
        }
    }
}
