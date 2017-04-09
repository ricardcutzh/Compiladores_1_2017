﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lienzo2D.Analizador;

namespace Lienzo2D
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
            

        }

        private void toolStripContainer1_TopToolStripPanel_Click(object sender, EventArgs e)
        {

        }

        private void PanelEdicion_Paint(object sender, PaintEventArgs e)
        {

        }

        private void addTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //agregarUnaTabNueva();
        }

        

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (tabControl1.TabCount > 0)
            {
                actualizaNumeroDeLineas();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
            TrackPosition.Start();
        }


        private int devuelvemeAltura(int lineas)
        {
            return lineas * 16;
        }


        private void toolStripButton1_Click(object sender, EventArgs e)//NUEVO LIENZO
        {
            String nombre = Microsoft.VisualBasic.Interaction.InputBox("Nombre del Archivo: ", "New File (Nuevo Archivo)", "Nuevo", 100, 100);
            agregarUnaTabNueva(nombre);
        }


        #region CODIGO DE TABS DINAMICAS
        //agregar una nueva tab al editor de texto
        private void agregarUnaTabNueva(String nombre)
        {
            TabPage nueva = new TabPage(nombre + ".lz");
            Panel pan = new Panel();
            pan.AutoScroll = false;
            pan.HorizontalScroll.Enabled = false;
            pan.HorizontalScroll.Visible = false;
            pan.HorizontalScroll.Maximum = 0;
            pan.AutoScroll = true;
            pan.SetBounds(0, 0, tabControl1.Width - 10, tabControl1.Height - 10);
            nueva.Controls.Add(pan);
            RichTextBox textEditor = new RichTextBox();
            RichTextBox numl = new RichTextBox();
            numl.ReadOnly = true;
            numl.Font = new Font("Consolas", 8);
            numl.SetBounds(0, 0, 30, tabControl1.Height-30);
            //numl.SetBounds(0, 0, 30, 20);
            numl.Text = "0";
            numl.WordWrap = false;
            numl.ScrollBars = RichTextBoxScrollBars.None;
            textEditor.SetBounds(30, 0, tabControl1.Width - 60, tabControl1.Height-30);
            //textEditor.SetBounds(30, 0, tabControl1.Width -60 , 20);
            textEditor.AcceptsTab = true;
            textEditor.Font = new Font("Consolas", 8);
            textEditor.WordWrap = false;
            textEditor.TextChanged += new EventHandler(eventocambio);
            textEditor.ScrollBars = RichTextBoxScrollBars.None;
            pan.Controls.Add(numl);//posición 0 del panel
            pan.Controls.Add(textEditor);// posición 1 del panel
            nueva.Controls.Add(pan);// posicion 0 del tabpage
            tabControl1.TabPages.Add(nueva);
        }

        //Actualizando el contador de lineas para cada tab
        private void actualizaNumeroDeLineas()
        {
            RichTextBox principal = (RichTextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls[0].Controls[1];
            RichTextBox lineas = (RichTextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls[0].Controls[0];
            lineas.Text = "";
            if (principal.Lines.Count() > 0)
            {
                int linenumber = principal.Lines.Count();
                if (devuelvemeAltura(linenumber) > tabControl1.TabPages[tabControl1.SelectedIndex].Height)
                {
                    principal.Height = devuelvemeAltura(linenumber);
                    lineas.Height = devuelvemeAltura(linenumber);
                }
                else
                {
                    principal.Height = tabControl1.Height-30;
                    lineas.Height = tabControl1.Height - 30;
                }
                for(int i = 0; i<linenumber; i++)
                {
                    lineas.Text = lineas.Text+ i + "\n";
                }
            }
            principal.Focus();
        }

        //Colorando Texto
        private void syntaxColoring()
        {
            RichTextBox principal = (RichTextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls[0].Controls[1];
            string reservadas = @"\b(publico|privado|Conservar|Lienzo|extiende|var|retorna|si|sino|para|mientras|hacer|Principal)";
            MatchCollection typeMatches = Regex.Matches(principal.Text, reservadas);

            string comentarios = @"(\>\>.+$|\<\-.+\-\>$)";
            MatchCollection coment = Regex.Matches(principal.Text, comentarios, RegexOptions.Multiline);

            


            int originalIndex = principal.SelectionStart;
            int originalLength = principal.SelectionLength;
            Color originalColor = Color.Black;

            principal.SelectionStart = 0;
            principal.SelectionLength = principal.Text.Length;
            principal.SelectionColor = originalColor;

            foreach (Match m in typeMatches)
            {
                principal.SelectionStart = m.Index;
                principal.SelectionLength = m.Length;
                principal.SelectionColor = Color.Blue;
            }

            foreach(Match m in coment)
            {
                principal.SelectionStart = m.Index;
                principal.SelectionLength = m.Length;
                principal.SelectionColor = Color.Green;
            }


           
        }

        private void eventocambio(object sender, EventArgs e)
        {
            RichTextBox principal = (RichTextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls[0].Controls[1];
            string reservadas = @"\b(publico|privado|Conservar|Lienzo|extiende|var|retorna|si|sino|para|mientras|hacer|Principal)";
            MatchCollection typeMatches = Regex.Matches(principal.Text, reservadas);

            string comentarios = @"(\>\>.+$|(\<\-(\s*|.*?)*\-\>))";
            MatchCollection coment = Regex.Matches(principal.Text, comentarios, RegexOptions.Multiline);

            string strings = "\".+?\"";
            MatchCollection stringMatches = Regex.Matches(principal.Text, strings);

            string tipos = @"(arreglo|doble|boolean|entero|caracter|cadena)";
            MatchCollection tiposM = Regex.Matches(principal.Text, tipos);

            string delim = @"(\$)";
            MatchCollection del = Regex.Matches(principal.Text, delim);

            int originalIndex = principal.SelectionStart;
            int originalLength = principal.SelectionLength;
            Color originalColor = Color.Black;

            principal.SelectionStart = 0;
            principal.SelectionLength = principal.Text.Length;
            principal.SelectionColor = originalColor;

            foreach (Match m in typeMatches)
            {
                principal.SelectionStart = m.Index;
                principal.SelectionLength = m.Length;
                principal.SelectionColor = Color.Blue;
            }

            foreach (Match m in coment)
            {
                principal.SelectionStart = m.Index;
                principal.SelectionLength = m.Length;
                principal.SelectionColor = Color.Green;
            }

            foreach (Match m in stringMatches)
            {
                principal.SelectionStart = m.Index;
                principal.SelectionLength = m.Length;
                principal.SelectionColor = Color.Brown;
            }

            foreach (Match m in tiposM)
            {
                principal.SelectionStart = m.Index;
                principal.SelectionLength = m.Length;
                principal.SelectionColor = Color.DarkCyan;
            }

            foreach(Match m in del)
            {
                principal.SelectionStart = m.Index;
                principal.SelectionLength = m.Length;
                principal.SelectionColor = Color.Purple;
            }

            principal.SelectionStart = originalIndex;
            principal.SelectionLength = originalLength;
            principal.SelectionColor = originalColor;
        }

        #endregion

        private void TrackPosition_Tick(object sender, EventArgs e)//ACTUALIZA LA LINEA POSICIÓN DEL LIENZO EN QUE SE TRABAJA
        {
            if (tabControl1.TabPages.Count > 0)
            {
                TabPage tab = (TabPage)tabControl1.TabPages[tabControl1.SelectedIndex];
                RichTextBox aux = (RichTextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls[0].Controls[1];
                int line = aux.GetLineFromCharIndex(aux.GetFirstCharIndexOfCurrentLine());
                int colum = 1 + aux.SelectionStart - aux.GetFirstCharIndexOfCurrentLine();
                LienzoNombre.Text = tab.Text;
                label2.Text = "Linea: " + line.ToString() + " Columna: " + colum.ToString();
            }  
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            //syntaxColoring();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (tabControl1.TabPages.Count > 0)
            {
                TabPage tab = (TabPage)tabControl1.TabPages[tabControl1.SelectedIndex];
                RichTextBox EntradaAnalizar = (RichTextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls[0].Controls[1];
                bool restult = Sintactico.analizar(EntradaAnalizar.Text);
                if (restult)
                {
                    Resultado.Text = "Válido";
                    Resultado.ForeColor = Color.Green;
                }
                else
                {
                    Resultado.Text = "Inválido";
                    Resultado.ForeColor = Color.Red;
                }
            }
            else
            {
                MessageBox.Show("No hay Pestañas abiertas", "Error al Ejecutar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
