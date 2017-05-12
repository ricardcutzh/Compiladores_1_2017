using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Parsing;

namespace Lienzo2D.Clases
{
    class Procedimiento
    {
        ParseTreeNode Sentencias;
        String Nombre;
        List<Parametro> parametros = new List<Parametro>();
        public Procedimiento(ParseTreeNode sentencias, String nombre, List<Parametro> parametros)
        {
            this.Sentencias = sentencias;
            this.Nombre = nombre;
            this.parametros = parametros;
        }
    }
}
