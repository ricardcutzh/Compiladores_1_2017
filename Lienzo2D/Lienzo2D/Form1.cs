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
        int contador = 0;
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
                RichTextBox principal = (RichTextBox)tabControl1.TabPages[0].Controls[0].Controls[1];
                RichTextBox nume = (RichTextBox)tabControl1.TabPages[0].Controls[0].Controls[0];
                nume.Text = "";
                principal.WordWrap = false;
                nume.WordWrap = false;
                principal.ScrollBars = RichTextBoxScrollBars.None;
                nume.ScrollBars = RichTextBoxScrollBars.None;
                if (principal.Lines.Count()>0)
                {
 
                    
                    int x = principal.Lines.Count();
                    if (devuelvemeAltura(x) > tabControl1.TabPages[0].Height)
                    {
                        principal.Height = devuelvemeAltura(x);
                        nume.Height = devuelvemeAltura(x);
                    }
                    else
                    {
                        principal.Height = tabControl1.Height;
                        nume.Height = tabControl1.Height;
                    }
                        for (int i = 0; i < x; i++)
                    {
                        nume.Text = nume.Text + i + "\n";

                    }
                }

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
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
            numl.Font = new Font("Consolas", 10);
            numl.SetBounds(0, 0, 30, tabControl1.Height);
            //numl.SetBounds(0, 0, 30, 20);
            numl.Text = "0";
            textEditor.SetBounds(30, 0, tabControl1.Width - 60, tabControl1.Height);
            //textEditor.SetBounds(30, 0, tabControl1.Width -60 , 20);
            textEditor.Text = "una linea";
            textEditor.AcceptsTab = true;
            textEditor.Font = new Font("Consolas", 10);
            pan.Controls.Add(numl);
            pan.Controls.Add(textEditor);
            nueva.Controls.Add(pan);
            tabControl1.TabPages.Add(nueva);
        }
        #endregion
    }
}
