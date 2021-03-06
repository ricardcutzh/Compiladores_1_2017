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
using Lienzo2D.Clases;
using Lienzo2D.Graficacion;
using System.Diagnostics;
using System.Text;
using System.IO;

namespace Lienzo2D
{
    public partial class Form1 : Form
    {
        //RUTA PARA GUARDAR ARCHIVOS
        public static String ruta = "";
        //LISTA DE ARBOLES POR ARCHIVO
        List<ArbolSintactico> Trees = new List<ArbolSintactico>();
        //OBJETO REPORTE
        Reporte rep = new Reporte();

        //TABLA GENERAL DE SIMBOLOS EN EL PROYECTO
        List<Simbolo> TablaGenera = new List<Simbolo>();

        //LISTA DE LIENZOS COMPILADOS
        List<Lienzo> LienzosCompilados = new List<Lienzo>();


        List<ErrorEnAnalisis> Errores = new List<ErrorEnAnalisis>();
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
            String nombre = Microsoft.VisualBasic.Interaction.InputBox("Nombre del Archivo: ", "New File (Nuevo Archivo)", "", 100, 100);
            if (nombre != "")
            {
                agregarUnaTabNueva(nombre);
            }
        }


        #region CODIGO DE TABS DINAMICAS
        //agregar una nueva tab al editor de texto Cuando Se crea un archivo en blanco
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

        private void addNewTabFromFile(String archivo, String texto)
        {
            TabPage nueva = new TabPage(archivo);
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
            numl.SetBounds(0, 0, 30, tabControl1.Height - 30);
            numl.Text = "0";
            numl.WordWrap = false;
            numl.ScrollBars = RichTextBoxScrollBars.None;
            textEditor.SetBounds(30, 0, tabControl1.Width - 60, tabControl1.Height - 30);
            textEditor.AcceptsTab = true;
            textEditor.Font = new Font("Consolas", 8);
            textEditor.WordWrap = false;
            textEditor.Text = texto;
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

            principal.Focus();
           
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
            principal.Focus();
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
                this.Errores.Clear();
                this.TablaGenera.Clear();
                this.LienzosCompilados.Clear();
                Sintactico.raizDeArbol = null;
                Sintactico.errores.Clear();
                TabPage tab = (TabPage)tabControl1.TabPages[tabControl1.SelectedIndex];
                RichTextBox EntradaAnalizar = (RichTextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls[0].Controls[1];
                bool restult = Sintactico.analizar(EntradaAnalizar.Text);
                if (restult)
                {
                    Resultado.Text = "Válido";
                    Resultado.ForeColor = Color.Green;
                    if (Sintactico.errores.Count > 0)
                    {
                        MessageBox.Show("El analisis del archivo aun contiene errores", "Errores en Entrada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                       
                        Tabla_Simbolos tabla = new Tabla_Simbolos(Sintactico.raizDeArbol);
                        tabla.generarme_tabla();
                        //List<Simbolo> simbolos = tabla.getTable();

                        //TablaSimbolosHTML reporte = new TablaSimbolosHTML(simbolos);
                        //reporte.generarTablaHTML();

                        Lienzo l = new Lienzo(tabla.getProcedimientos(), tabla.getFunciones(), tabla.getVariables(), tabla.getExtends(), tabla.getNombre(), tabla.getVisibilidad());
                        this.LienzosCompilados.Add(l);
                        AnalisisDeExtends(tabla.getExtends());
                        //Reporte re = new Reporte();
                        //re.ReporteDeErrores(tabla.semanticos(), "Errores");
                        MeterDatosAListasGenerales(tabla.semanticos(), tabla.getTable());

                        if (this.Errores.Count() > 0)
                        {
                            InfoErr.Text = "Existen Errores Semanticos";
                        }
                        else
                        {

                            Ejecucion ej = new Ejecucion(this.LienzosCompilados, this.TablaGenera);
                            AreaImagen nuevo = new AreaImagen();
                            nuevo.Show();
                            ej.IniciarEjecucion();
                            if (ej.getErrores().Count() > 0)
                             {
                                 InfoErr.Text = "Existen Errores Semánticos";
                                 foreach(ErrorEnAnalisis er in ej.getErrores())
                                 {
                                     this.Errores.Add(er);
                                 }
                             }
                             else
                             {
                                 InfoErr.Text = "Compilación Terminada";
                                 this.TablaGenera = ej.getTablaNueva();
                                nuevo.Visible = true;
                            }
                            
                        }

                        /*MessageBox.Show("Lienzos Compilados: " + this.LienzosCompilados.Count());
                        foreach (Lienzo h in this.LienzosCompilados)
                        {
                            h.ReporteDeLienzo();
                        }*/
                    }
                    
                }
                else
                {
                    Resultado.Text = "Inválido";
                    MessageBox.Show("El analisis del archivo aun contiene errores sin recuperación", "Errores en Entrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Resultado.ForeColor = Color.Red;
                }
            }
            else
            {
                MessageBox.Show("No hay Pestañas abiertas", "Error al Ejecutar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            if (Sintactico.raizDeArbol != null)
            {
                String cadenaArbol = ArchivoDot.getDot(Sintactico.raizDeArbol);
                rep.generarImagenAST(cadenaArbol);
            }
            else
            {
                MessageBox.Show("No existe un arbol Sintáctico", "Raiz de Arbol Nula", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void abrirASTVisorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Sintactico.raizDeArbol != null)
                {
                    Process.Start("C:\\Reportes\\AST.png");
                }
            }
            catch
            {
                MessageBox.Show("No existe un arbol Sintáctico", "Raiz de Arbol Nula", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (Sintactico.errores.Count() > 0)
            {
                if(rep.ReporteDeErrores(Sintactico.errores, tabControl1.TabPages[tabControl1.SelectedIndex].Text))
                {
                    Process.Start("C:\\Reportes\\ReporteErrores.html");
                }

            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)//ABRIR ARCHIVO CON EXTENSION .lz
        {
            Stream str;
            OpenFileDialog abrir = new OpenFileDialog();
            abrir.Filter = "lz Files |*.lz";
            if(abrir.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {   
                if((str = abrir.OpenFile()) != null)
                {
                    string archivo = abrir.FileName;
                    string texto = File.ReadAllText(archivo);
                    ruta = abrir.SafeFileName;
                    addNewTabFromFile(abrir.SafeFileName, texto);
                }
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (tabControl1.TabPages.Count > 0)
            {
                TabPage aux = tabControl1.TabPages[tabControl1.SelectedIndex];
                RichTextBox auxr = (RichTextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls[0].Controls[1];
                SaveFileDialog guardar = new SaveFileDialog();
                if (guardar.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        StreamWriter wr = new StreamWriter(guardar.FileName + ".lz");
                        ruta = guardar.FileName;
                        wr.Write(auxr.Text);
                        wr.Close();
                        MessageBox.Show("Archivo Guardado: " + guardar.FileName);
                    }
                    catch
                    {
                        MessageBox.Show("Error en la escritura de archivo", "Errores", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    
                }
            }
        }

        private void toolStripButton8_Click(object sender, EventArgs e)//MUESTRA LA TABLA DE SIMBOLOS
        {
            TablaSimbolosHTML tabla = new TablaSimbolosHTML(this.TablaGenera);
            tabla.generarTablaHTML();
            Process.Start("C:\\Reportes\\TablaDeSimbolos.html");
        }

        private void AnalisisDeExtends(List<String> Extends)
        {
            foreach(TabPage t in tabControl1.TabPages)
            {
                foreach(String name in Extends)
                {
                    if(name+".lz" == t.Text)
                    {
                        Sintactico.raizDeArbol = null;
                        Sintactico.errores.Clear();
                        TabPage aux = t;
                        RichTextBox aux1 = (RichTextBox)t.Controls[0].Controls[1];
                        bool result = Sintactico.analizar(aux1.Text);
                        if (result)
                        {
                            if (Sintactico.errores.Count() > 0)//SI EXISTEN ERRORES
                            {
                                foreach(ErrorEnAnalisis er in Sintactico.errores)
                                {
                                    this.Errores.Add(er);
                                }
                            }
                            Tabla_Simbolos tabla = new Tabla_Simbolos(Sintactico.raizDeArbol);
                            tabla.generarme_tabla();

                            Lienzo nuevo = new Lienzo(tabla.getProcedimientos(), tabla.getFunciones(), tabla.getVariables(), tabla.getExtends(), tabla.getNombre(), tabla.getVisibilidad());

                            this.LienzosCompilados.Add(nuevo);

                            AnalisisDeExtends(tabla.getExtends());

                            MeterDatosAListasGenerales(tabla.semanticos(), tabla.getTable());

                            
                        }
                        else
                        {
                            foreach(ErrorEnAnalisis er in Sintactico.errores)//ERRORES FALTALES
                            {
                                this.Errores.Add(er);
                            }
                        }
                    }
                }
            }

        }

        private string obtenerRuta(string ruta)
        {
            String[] v = ruta.Split('\\');
            int x = v.Length-1;
            string aux = v[x];
            string nueva_ruta = ruta.Replace(aux, "");
            return nueva_ruta;
        }

        private void MeterDatosAListasGenerales(List<ErrorEnAnalisis>errores, List<Simbolo> Simbolos)
        {
            foreach(ErrorEnAnalisis er in errores)
            {
                this.Errores.Add(er);
            }
            foreach(Simbolo c in Simbolos)
            {
                this.TablaGenera.Add(c);
            }
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            if (this.Errores.Count > 0)
            {
                Reporte re = new Reporte();
                re.ReporteDeErrores(this.Errores, "Errores");
                Process.Start("C:\\Reportes\\ReporteErrores.html");
            }
        }

        private void consolaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Console.WriteLine("hola mundo");
        }

        private void pruebaGraphicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //AreaImagen pr = new AreaImagen();
            //pr.Show();
        }

        private void guardarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.TabPages.Count > 0)
            {
                TabPage aux = tabControl1.TabPages[tabControl1.SelectedIndex];
                RichTextBox auxr = (RichTextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls[0].Controls[1];
                
                    try
                    {
                        string filename = aux.Text;
                        StreamWriter wr = new StreamWriter(obtenerRuta(ruta)+filename);
                        wr.Write(auxr.Text);
                        wr.Close();
                        MessageBox.Show("Archivo Guardado: " + ruta);
                    }
                    catch
                    {
                        MessageBox.Show("Error en la escritura de archivo", "Errores", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                
            }
        }
    }
}
