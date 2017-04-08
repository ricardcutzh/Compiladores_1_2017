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
    public partial class Editor : Control
    {
        public Editor()
        {
            InitializeComponent();
            Bandera.Text = "esto es una prueba";
            Bandera.SetBounds(100, 200, 100, 100);
            panel1.Controls.Add(Bandera);
            tabControl1.TabPages.Add("nueva");
            tabControl1.SelectedTab.Controls.Add(Bandera);
            tabControl1.Refresh();
            
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
    }
}
