using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lienzo2D.Clases
{
    class Variable
    {
        public string nombre { get; set; }
        public string valor { get; set; }
        public string tipo { get; set; }
        public string ambito { get; set; }
        public bool conservar { get; set; }
        public bool esGlobal { get; set; }
        public List<int> dimensiones = new List<int>();
        public bool esArreglo;
        public List<List<int>> Valores = new List<List<int>>();

        public Variable(string nombre, string valor, string tipo, string ambito, bool conservar, bool esGlobal)
        {
            this.nombre = nombre;
            this.valor = valor;
            this.tipo = tipo;
            this.ambito = ambito;
            this.conservar = conservar;
            this.esGlobal = esGlobal;
        }

        public Variable(string nombre,List<List<int>> Valores, string tipo, string ambito, bool conservar, bool esGlobal, bool esArreglo)
        {
            this.nombre = nombre;
            this.Valores = Valores;
            this.tipo = tipo;
            this.conservar = conservar;
            this.esGlobal = esGlobal;
            this.esArreglo = esArreglo;
            this.ambito = ambito;
        }

        
    }
}
