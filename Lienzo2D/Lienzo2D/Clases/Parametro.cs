using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lienzo2D.Clases
{
    class Parametro
    {
        public string nombre;
        public string tipo;
        public string valor { get; set; }

        public Parametro(string nombre, string tipo)
        {
            this.nombre = nombre;
            this.tipo = tipo;
        }


    }
}
