using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Prueba_Gramatica_IRONY_Proyecto2.Analizador;

namespace Prueba_Gramatica_IRONY_Proyecto2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool resultado = Sintactico.analizar(Entrada.Text);
            if (resultado)
            {
                Salida.Text = "Entrada valida";
            }
            else
            {
                Salida.Text = "Entrada invalida";
            }
        }
    }
}
