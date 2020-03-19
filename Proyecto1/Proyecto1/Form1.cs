using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
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
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
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
            //creacion del richtextbox area de escritura
            RichTextBox Rich = new RichTextBox();
            //Redimencionar richtextbox
            Rich.SetBounds(0, 0, tabControl1.Width-10, tabControl1.Height-20);
            OpenFileDialog Buscador = new OpenFileDialog();
            Buscador.Filter = "ExpReg|*.er";
            if (Buscador.ShowDialog() == DialogResult.OK)
            {
                Rich.Text=M.AbrirArchivo(Buscador.FileName);
                Nuevo.Text = Buscador.SafeFileName;
                Rich.Name = "Rich" + Buscador.SafeFileName;
                Nuevo.Controls.Add(Rich);
                tabControl1.Controls.Add(Nuevo);
            }
            else { MessageBox.Show("Error no Selecciono ningun archivo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void aNALIZARToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try {
                Automata A = new Automata();
                string NombreRich = "Rich" + tabControl1.SelectedTab.Text;
                RichTextBox Rich = tabControl1.SelectedTab.Controls[NombreRich] as RichTextBox;
                A.AutomataConjuntos(Rich.Text, dataGridView1, dataGridView2);
                M.RecuperarImagenes();
            } catch (Exception E) {
                MessageBox.Show("NO HAY NADA QUE ANALIZAR", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            comboBox1.Items.Clear();
            try {
                String[] Img = M.RecuperarImagenes();
                foreach (String IMAGEN in Img) {
                    comboBox1.Items.Add(IMAGEN);
                }
                comboBox1.Update();
            } catch (Exception E) {
                MessageBox.Show("IMAGEN NO EXISTENTE", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SeleccionarIMG(object sender, EventArgs e)
        {
            try {
                pictureBox1.Image = Image.FromFile(@"C:\GraphvizC\Imagen\" + comboBox1.SelectedItem);
                
            } catch (Exception E) { }
        }

        private void iMAGENESToolStripMenuItem_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            try
            {
                String[] Img = M.RecuperarImagenes();
                foreach (String IMAGEN in Img)
                {
                    comboBox1.Items.Add(IMAGEN);
                }
                comboBox1.Update();
            }
            catch (Exception E) { }
        }

        private void xMLTOKENSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try { Process.Start(@"C:\GraphvizC\Archivos\XMLTOKENS.xml"); } catch (Exception E) {
                MessageBox.Show("ARCHIVO NO EXISTENTE", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        

        private void xMLERRORESToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(@"C:\GraphvizC\Archivos\XMLERRORES.xml");
            }
            catch (Exception E) {
                MessageBox.Show("ARCHIVO NO EXISTENTE", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gENERARPDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(@"C:\GraphvizC\Archivos\XMLERRORES.pdf");
            }
            catch (Exception E)
            {
                MessageBox.Show("ARCHIVO NO EXISTENTE", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
