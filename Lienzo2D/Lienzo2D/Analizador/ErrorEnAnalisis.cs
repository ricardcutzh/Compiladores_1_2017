using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lienzo2D.Analizador
{
    class ErrorEnAnalisis
    {
        String Error { get; }
        String tipo { get; }
        int linea { get; }
        int columna { get; }

        public ErrorEnAnalisis(String error, String tipo, int linea, int columna)
        {
            this.Error = error;
            this.tipo = tipo;
            this.linea = linea;
            this.columna = columna;
        }

        public String getError()
        {
            return this.Error;
        }

        public String getTipo()
        {
            return this.tipo;
        }

        public int getLinea()
        {
            return this.linea;
        }

        public int getColumna()
        {
            return this.columna;
        }
    }
}
