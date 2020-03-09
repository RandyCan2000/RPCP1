using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Proyecto1.Globale;
using Proyecto1.Metodos;

namespace Proyecto1
{
    
    public partial class Form1 : Form
    {
        Globales G = new Globales();
        
        METODOS M = new METODOS();
        public Form1()
        {
            InitializeComponent();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
                    }

        private void aBRIRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //creacion del tab page que contiene al richTextbox
            TabPage Nuevo = new TabPage();
            Nuevo.Text = "Nuevo.er";
            Nuevo.Name = "Nuevo.er";
            //creacion del richtextbox area de escritura
            RichTextBox Rich = new RichTextBox();
            Rich.Name = "Rich"+Nuevo.Name;
            //Redimencionar richtextbox
            Rich.SetBounds(0, 0, tabControl1.Width-10, tabControl1.Height-20);
            OpenFileDialog Buscador = new OpenFileDialog();
            Buscador.Filter = "ExpReg|*.er";
            if (Buscador.ShowDialog() == DialogResult.OK)
            {
                Rich.Text=M.AbrirArchivo(Buscador.FileName);
                Nuevo.Controls.Add(Rich);
                tabControl1.Controls.Add(Nuevo);
            }
            else { MessageBox.Show("Error no Selecciono ningun archivo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void aNALIZARToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Automata A = new Automata();
            string NombreRich = "Rich" + tabControl1.SelectedTab.Text;
            RichTextBox Rich = tabControl1.SelectedTab.Controls[NombreRich] as RichTextBox;
            A.AutomataConjuntos(Rich.Text,dataGridView1,dataGridView2);
        }
    }
}
