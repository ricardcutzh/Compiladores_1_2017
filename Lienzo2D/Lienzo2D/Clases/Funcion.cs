﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Parsing;
using Irony.Interpreter;

namespace Lienzo2D.Clases
{
    class Funcion
    {
        public String Nombre;
        public String Tipo;
        public ParseTreeNode Sentencias;
        public List<Parametro> Parametros = new List<Parametro>();

        public Funcion(ParseTreeNode Sentencias, string Nombre, string Tipo, List<Parametro> Parametros)
        {
            this.Nombre = Nombre;
            this.Tipo = Tipo;
            this.Sentencias = Sentencias;
            this.Parametros = Parametros;
        }
    }
}
