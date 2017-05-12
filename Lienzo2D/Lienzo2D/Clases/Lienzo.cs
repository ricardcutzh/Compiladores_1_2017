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
        List<Procedimiento> Procedimientos = new List<Procedimiento>();
        List<Funcion> Funciones = new List<Funcion>();
        List<Variable> Variables = new List<Variable>();
        List<String> Extends = new List<String>();
        String nombre;
        String visibilidad;

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
    }
}
