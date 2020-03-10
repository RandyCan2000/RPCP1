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
        public Globales G = new Globales();
        Stack<String> EXPSeparada;
        NodoT Inicio = null, Fin = null;
        int ContadorNodo = 0;
        public void CargarTablaToken(DataGridView DTG1, Stack<Token> tok) {
            DataTable Tab = new DataTable();
            Tab.Columns.Add("ID");
            Tab.Columns.Add("LEXEMA");
            for (int i = tok.Count-1; i >=0 ; i--) {
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
            for (int i = ERR.Count-1; i >=0; i--)
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
        public void BuscarExpReg(Stack<Token> TOKEN){
            Inicio = null; Fin = null;
            ContadorNodo = 0;
            for (int i = TOKEN.Count-1; i >= 0 ; i--)
            {
                if (TOKEN.ElementAt(i).getLexema().Equals("CONJ"))
                {
                    for (int index = i; i >=0; index--)
                    {
                        if (TOKEN.ElementAt(index).getLexema().Equals(";")) { i = index; break; }
                    }
                }
                else if (TOKEN.ElementAt(i).getLexema().Equals("->"))
                {
                    string Lexema = TOKEN.ElementAt(i - 1).getLexema();
                    SepararExpReg(Lexema);
                    i--;
                }
                else if (TOKEN.ElementAt(i).getLexema().Equals("%%")) {break;}
            }
        }

        public void SepararExpReg(String EXP)
        {
            Stack<String> EXPSeparada2 = new Stack<String>();
            EXPSeparada = new Stack<String>();
            String Pal = "";
            for (int i = 0; i < EXP.Length; i++)
            {
                if (EXP[i] == '.' || EXP[i] == '|' || EXP[i] == '*' || EXP[i] == '+' || EXP[i] == '?')
                { EXPSeparada2.Push(Char.ToString(EXP[i])); }
                else if (EXP[i] == '"')
                {
                    Pal = "\"";
                    for (int b = i + 1; b < EXP.Length; b++) { if (EXP[b] == '"') { Pal += EXP[b]; EXPSeparada2.Push(Pal); i = b; break; } else { Pal += EXP[b]; } }
                    Pal = "";
                }
                else if (EXP[i] == '{')
                {
                    for (int b = i; b < EXP.Length; b++) { if (EXP[b] == '}') { Pal += EXP[b]; EXPSeparada2.Push(Pal); i = b; break; } else { Pal += EXP[b]; } }
                    Pal = "";
                }
            }
            while (EXPSeparada2.Count!=0) {
                    EXPSeparada.Push(EXPSeparada2.Pop());
            }
            Console.WriteLine("EXP SEPARADA");
            //Metodos que deben generar el automata
            G.contadorArbol++;
        }

        public void GenerarThompshon() {
        
        }

        public void PlantillaKleen() {}
        public void PlantillaConcatenacion() { }
        public void PlantillaOr() { }
        public void PlantillaSignoInterrogacion() { }
        public void PlantillaCerraduraPositiva() { }
    }
}
