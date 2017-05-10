using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lienzo2D.Clases
{
    class Elemento
    {
        public String valor { get; }
        public String tipo { get; }

        public Elemento(String valor, String tipo)
        {
            this.valor = valor;
            this.tipo = tipo;
        }


    }
}
