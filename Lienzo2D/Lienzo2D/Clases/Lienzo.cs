using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Lienzo2D.Clases
{
    class Lienzo
    {
        //ESTE OBJETO SERA EL QUE ME AYUDARÁ A EJECUTAR EL CÓDIGO
        public List<Procedimiento> Procedimientos = new List<Procedimiento>();
        public List<Funcion> Funciones = new List<Funcion>();
        public List<Variable> Variables = new List<Variable>();
        public List<String> Extends = new List<String>();
        public String nombre;
        public String visibilidad;
        List<Lienzo> LienzosExtend = new List<Lienzo>();

        public Lienzo(List<Procedimiento> Procedimientos, List<Funcion> Funciones, List<Variable> Variables, List<String> Extends, String nombre, String visibilidad)
        {
            this.Procedimientos = Procedimientos;
            this.Funciones = Funciones;
            this.Variables = Variables;
            this.Extends = Extends;
            this.nombre = nombre;
            this.visibilidad = visibilidad;
        }

        
        public void ReporteDeLienzo()
        {
            String rep = "Nombre: " + this.nombre + " |  Visibilidad: " + this.visibilidad + "\n";
            rep = rep + "Numero de Procedimientos: " + Procedimientos.Count() + " \n";
            rep = rep + "Numero de Funciones: " + Funciones.Count() + "\n";
            rep = rep + "Numero de Variables: " + Variables.Count() + "\n";
            rep = rep + "Numero de Extends: " + Extends.Count() + "\n";
            MessageBox.Show(rep);
        }

        public void ReporteDeFunciones()
        {
            string cadena = "Reporte de Funciones: \n";
            foreach(Funcion f in this.Funciones)
            {
                cadena = cadena + "Nombre: " + f.Nombre+"\n";
                cadena = cadena + "Tipo: " + f.Tipo + "\n";
                cadena = cadena + "# De Parametros: " + f.Parametros.Count() + "\n";
            }

            MessageBox.Show(cadena);
        }

        public void ReporteDeProcedimientos()
        {
            string cadena = "Reporte de Procedimientos: \n";
            foreach (Procedimiento p in this.Procedimientos)
            {
                cadena = cadena + "Nombre: " + p.Nombre + " \n";
                cadena = cadena + "# De Parametros: " + p.parametros.Count() + " \n";
            }
            MessageBox.Show(cadena);
        }

        public void addToExtendedLienzo(Lienzo nuevo)
        {
            this.LienzosExtend.Add(nuevo);
        }

        public void reporteVariables()
        {
            string cadena = "Variables: \n";
            foreach(Variable v in this.Variables)
            {
                if (!v.esArreglo)
                {
                    cadena = cadena + "Nombre: " + v.nombre + " | Valor: " + v.valor + " | Ambito: "+v.ambito+" |\n";
                }        
            }

            MessageBox.Show(cadena);
        }

        public void reporteDeArreglos()
        {
            string cadena = "Arreglos: \n";
            foreach(Variable v in this.Variables)
            {
                if (v.esArreglo)
                {
                    cadena = cadena + "Nombre: " + v.nombre + " Ambito "+v.ambito+"\n";
                    cadena = cadena + "Valores: \n";
                    foreach(List<int> l in v.Valores)
                    {
                        foreach(int n in l)
                        {
                            cadena = cadena + "| " + n + " |\n";
                        }
                    }
                    cadena = cadena + "Fin\n"; 
                }
            }
            MessageBox.Show(cadena);
        }
    }
}
