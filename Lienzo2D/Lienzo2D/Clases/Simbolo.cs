using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lienzo2D.Clases
{
    class Simbolo
    {
        public string nombre;
        public string tipo;
        public string visibilidad;
        public string valor;
        public string ambito;
        public bool conservar;

        public Simbolo(string nombre, string tipo, string visibilidad, string valor, string ambito, bool conservar)
        {
            this.nombre = nombre;
            this.tipo = tipo;
            this.visibilidad = visibilidad;
            this.valor = valor;
            this.ambito = ambito;
            this.conservar = conservar;
        }

        public Simbolo()
        {
            //auxiliar  
        }
    }
}
