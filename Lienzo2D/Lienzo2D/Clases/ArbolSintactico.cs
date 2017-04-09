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
        String rutaImagen;
        public ArbolSintactico()
        {
            //Constructor por defecto
        }
        public ArbolSintactico(ParseTreeNode raiz)
        {
            this.root = raiz;
        }

        public String getRutaImagen()
        {

            return this.rutaImagen;
        }
    }
}
