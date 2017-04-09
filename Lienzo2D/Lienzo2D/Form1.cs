using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        }

        //Colorando Texto

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
    }
}
