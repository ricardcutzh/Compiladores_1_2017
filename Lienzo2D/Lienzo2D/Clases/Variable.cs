using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lienzo2D.Clases
{
    class Variable
    {
        public string nombre { get; }
        public string valor { get; }
        public string tipo { get; }
        public string ambito { get; }
        public bool conservar { get; }
        public bool esGlobal { get; }

        public Variable(string nombre, string valor, string tipo, string ambito, bool conservar, bool esGlobal)
        {
            this.nombre = nombre;
            this.valor = valor;
            this.tipo = tipo;
            this.ambito = ambito;
            this.conservar = conservar;
            this.esGlobal = esGlobal;
        }
    }
}
