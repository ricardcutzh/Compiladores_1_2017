using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;

namespace Lienzo2D.Clases
{
    class ArbolSintactico
    {
        ParseTreeNode root;
        String nombreDeArbol;
        int idDeArbol;
        public ArbolSintactico()
        {
            //Constructor por defecto
        }
        public ArbolSintactico(ParseTreeNode raiz, String nombre, int idArbol)
        {
            this.nombreDeArbol = nombre;
            this.root = raiz;
        }
    }
}
