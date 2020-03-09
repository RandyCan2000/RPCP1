using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Proyecto1.Globale;
using Proyecto1.TDA;

namespace Proyecto1.Metodos
{
    class METODOS
    {
        public void CargarTablaToken(DataGridView DTG1, Stack<Token> tok) {
            DataTable Tab = new DataTable();
            Tab.Columns.Add("ID");
            Tab.Columns.Add("LEXEMA");
            for (int i = 0; i < tok.Count; i++) {
                DataRow Fila = Tab.NewRow();
                Fila["ID"] = tok.ElementAt(i).getID();
                Fila["LEXEMA"] = tok.ElementAt(i).getLexema();
                Tab.Rows.Add(Fila);
            }
            DTG1.DataSource = Tab;
            DTG1.Update();
        }

        public void CargarTablaErrores(DataGridView DTG1, Stack<Error> ERR)
        {
            DataTable Tab = new DataTable();
            Tab.Columns.Add("ID");
            Tab.Columns.Add("LEXEMA");
            Tab.Columns.Add("FILA");
            Tab.Columns.Add("COLUMNA");
            Tab.Columns.Add("ESPERADO");
            for (int i = 0; i < ERR.Count; i++)
            {
                DataRow Fila = Tab.NewRow();
                Fila["ID"] = ERR.ElementAt(i).getID();
                Fila["LEXEMA"] = ERR.ElementAt(i).getLexema();
                Fila["FILA"] = ERR.ElementAt(i).getFila();
                Fila["COLUMNA"] = ERR.ElementAt(i).getColumna();
                Fila["ESPERADO"] = ERR.ElementAt(i).getEsperado();
                Tab.Rows.Add(Fila);
            }
            DTG1.DataSource = Tab;
            DTG1.Update();
        }
        public String AbrirArchivo(String Ruta) {
            String Text = "";
            StreamReader Texto = new StreamReader(Ruta);
                String Linea = "";
                while (!Texto.EndOfStream)
                {
                    Linea = Texto.ReadLine();
                    Text += Linea + "\r\n";
                }
                Texto.Close();
            
            return Text;
        }
    }
}
