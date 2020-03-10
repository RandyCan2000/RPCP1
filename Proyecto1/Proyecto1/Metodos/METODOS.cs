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
                    Inicio = null; Fin = null;
                    ContadorNodo = 0;
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
            GenerarThompshon();
            //Metodos que deben generar el automata
            G.contadorArbol++;
        }

        public void GenerarThompshon() {
            String ValExp = EXPSeparada.Pop();
            if (ValExp.Equals("*")) {
                Fin = PlantillaKleen(Inicio);
            }
            else if (ValExp.Equals("|")) {
                Fin = PlantillaOr(Inicio);
            }
            else if (ValExp.Equals("+")) {
                Fin = PlantillaCerraduraPositiva(Inicio);
            }
            else if (ValExp.Equals("?")) {
                Fin = PlantillaSignoInterrogacion(Inicio);
            }
            else if (ValExp.Equals(".")) {
                Fin = PlantillaConcatenacion(Inicio);
            }
            Console.WriteLine(ContadorNodo);
        }

        public NodoT PlantillaKleen(NodoT Nodo) {
            NodoT Nodo1 = new NodoT(ContadorNodo); ContadorNodo++;
            if (Nodo == null) { Inicio = Nodo1; }
            else { Nodo.NodoArriba1 = Nodo1;Nodo.ValNodoArriba1 = G.Eps; }
            NodoT Nodo2 = new NodoT(ContadorNodo); ContadorNodo++;
            NodoT Nodo3 = new NodoT(ContadorNodo); ContadorNodo++;
            NodoT Nodo4 = new NodoT(ContadorNodo); ContadorNodo++;
            //Se Trabajo con el primer nodo de la plantilla de la cerradura de kleen
            Nodo1.NodoArriba1 = Nodo2;
            Nodo1.ValNodoArriba1 = G.Eps;
            Nodo1.NodoAbajo1 = Nodo4;
            Nodo1.ValNodoAbajo1 = G.Eps;
            //Se trabaja con el segundo nodo de la plantilla
            String ValExp = EXPSeparada.Pop();
            if (ValExp.Equals("*")) { 
                NodoT AUX=PlantillaKleen(Nodo2);
                AUX.NodoArriba1 = Nodo3;
                AUX.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("|")) {
                NodoT AUX = PlantillaOr(Nodo2);
                AUX.NodoArriba1 = Nodo3;
                AUX.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("?")) {
                NodoT AUX = PlantillaSignoInterrogacion(Nodo2);
                AUX.NodoArriba1 = Nodo3;
                AUX.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("+")) {
                NodoT AUX = PlantillaCerraduraPositiva(Nodo2);
                AUX.NodoArriba1 = Nodo3;
                AUX.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals(".")) {
                NodoT AUX = PlantillaConcatenacion(Nodo2);
                AUX.NodoArriba1 = Nodo3;
                AUX.ValNodoArriba1 = G.Eps;
            }
            else {
                Nodo2.NodoArriba1 = Nodo3;
                Nodo2.ValNodoArriba1 = ValExp;
            }
            //Se Trabaja con el 3er nodo de la plantilla
            Nodo3.NodoArriba1 = Nodo2;
            Nodo3.ValNodoArriba1 = G.Eps;
            Nodo3.NodoAbajo1 = Nodo4;
            Nodo3.ValNodoAbajo1 = G.Eps;
            //se retorna el 4to nodo de la plantilla
            return Nodo4;
        }
        public NodoT PlantillaConcatenacion(NodoT Nodo) {
            NodoT Nodo1 = new NodoT(ContadorNodo);ContadorNodo++;
            NodoT Nodo2 = new NodoT(ContadorNodo); ContadorNodo++;
            NodoT Nodo3 = new NodoT(ContadorNodo); ContadorNodo++;
            //se valida el inicio
            if (Nodo == null) { Inicio = Nodo1; }
            else { Nodo.NodoArriba1 = Nodo1; Nodo.ValNodoArriba1 = G.Eps; }
            //se extrae el valor de la exp regular
            String ValExp = EXPSeparada.Pop();
            //se valida que tipo de valor se extrajo Nodo1
            if (ValExp.Equals("*")){
                NodoT Aux = PlantillaKleen(Nodo1);
                Aux.NodoArriba1 = Nodo2;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("|")){
                NodoT Aux = PlantillaOr(Nodo1);
                Aux.NodoArriba1 = Nodo2;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("?")){
                NodoT Aux = PlantillaSignoInterrogacion(Nodo1);
                Aux.NodoArriba1 = Nodo2;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("+")){
                NodoT Aux = PlantillaCerraduraPositiva(Nodo1);
                Aux.NodoArriba1 = Nodo2;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals(".")){
                NodoT Aux = PlantillaConcatenacion(Nodo1);
                Aux.NodoArriba1 = Nodo2;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else{
                Nodo1.NodoArriba1 = Nodo2;
                Nodo1.ValNodoArriba1 = ValExp;
            }
            //se Trabaja con el nodo 2
            ValExp = EXPSeparada.Pop();
            //se valida que tipo de valor se extrajo
            if (ValExp.Equals("*"))
            {
                NodoT Aux = PlantillaKleen(Nodo2);
                Aux.NodoArriba1 = Nodo3;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("|"))
            {
                NodoT Aux = PlantillaOr(Nodo2);
                Aux.NodoArriba1 = Nodo3;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("?"))
            {
                NodoT Aux = PlantillaSignoInterrogacion(Nodo2);
                Aux.NodoArriba1 = Nodo3;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("+"))
            {
                NodoT Aux = PlantillaCerraduraPositiva(Nodo2);
                Aux.NodoArriba1 = Nodo3;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("."))
            {
                NodoT Aux = PlantillaConcatenacion(Nodo2);
                Aux.NodoArriba1 = Nodo3;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else
            {
                Nodo2.NodoArriba1 = Nodo3;
                Nodo2.ValNodoArriba1 = ValExp;
            }
            //se retorna el ultimo nodo
            return Nodo3;
        }
        public NodoT PlantillaOr(NodoT Nodo) {
            NodoT Nodo1 = new NodoT(ContadorNodo); ContadorNodo++;
            NodoT Nodo2 = new NodoT(ContadorNodo); ContadorNodo++;
            NodoT Nodo3 = new NodoT(ContadorNodo); ContadorNodo++;
            NodoT Nodo4 = new NodoT(ContadorNodo); ContadorNodo++;
            NodoT Nodo5 = new NodoT(ContadorNodo); ContadorNodo++;
            NodoT Nodo6 = new NodoT(ContadorNodo); ContadorNodo++;
            //Validar Inicio
            if (Nodo == null) { Inicio = Nodo1; }
            else { Nodo.NodoArriba1 = Nodo1; Nodo.ValNodoArriba1 = G.Eps; }
            //Se Trabaja con el nodo1
            Nodo1.NodoArriba1 = Nodo2;Nodo1.ValNodoArriba1 = G.Eps;
            Nodo1.NodoAbajo1 = Nodo3;Nodo1.ValNodoAbajo1 = G.Eps;
            //Se trabaja con el nodo 2
            String ValExp = EXPSeparada.Pop();
            if (ValExp.Equals("*"))
            {
                NodoT Aux = PlantillaKleen(Nodo2);
                Aux.NodoArriba1 = Nodo4;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("|"))
            {
                NodoT Aux = PlantillaOr(Nodo2);
                Aux.NodoArriba1 = Nodo4;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("?"))
            {
                NodoT Aux = PlantillaSignoInterrogacion(Nodo2);
                Aux.NodoArriba1 = Nodo4;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("+"))
            {
                NodoT Aux = PlantillaCerraduraPositiva(Nodo2);
                Aux.NodoArriba1 = Nodo4;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("."))
            {
                NodoT Aux = PlantillaConcatenacion(Nodo2);
                Aux.NodoArriba1 = Nodo4;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else
            {
                Nodo2.NodoArriba1 = Nodo4;
                Nodo2.ValNodoArriba1 = ValExp;
            }
            //Se Trabaja con el nodo 3
            ValExp = EXPSeparada.Pop();
            if (ValExp.Equals("*"))
            {
                NodoT Aux = PlantillaKleen(Nodo3);
                Aux.NodoArriba1 = Nodo5;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("|"))
            {
                NodoT Aux = PlantillaOr(Nodo3);
                Aux.NodoArriba1 = Nodo5;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("?"))
            {
                NodoT Aux = PlantillaSignoInterrogacion(Nodo3);
                Aux.NodoArriba1 = Nodo5;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("+"))
            {
                NodoT Aux = PlantillaCerraduraPositiva(Nodo3);
                Aux.NodoArriba1 = Nodo5;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("."))
            {
                NodoT Aux = PlantillaConcatenacion(Nodo3);
                Aux.NodoArriba1 = Nodo5;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else
            {
                Nodo3.NodoArriba1 = Nodo5;
                Nodo3.ValNodoArriba1 = ValExp;
            }
            //Se trabaja con el nodo 4 y 5
            Nodo4.NodoArriba1 = Nodo6; Nodo4.ValNodoArriba1 = G.Eps;
            Nodo5.NodoArriba1 = Nodo6; Nodo5.ValNodoArriba1 = G.Eps;
            //Se retorna el 6to nodo
            return Nodo6;
        }
        public NodoT PlantillaSignoInterrogacion(NodoT Nodo) {
            NodoT Nodo1 = new NodoT(ContadorNodo); ContadorNodo++;
            NodoT Nodo2 = new NodoT(ContadorNodo); ContadorNodo++;
            NodoT Nodo3 = new NodoT(ContadorNodo); ContadorNodo++;
            NodoT Nodo4 = new NodoT(ContadorNodo); ContadorNodo++;
            NodoT Nodo5 = new NodoT(ContadorNodo); ContadorNodo++;
            NodoT Nodo6 = new NodoT(ContadorNodo); ContadorNodo++;
            //Validar Inicio
            if (Nodo == null) { Inicio = Nodo1; }
            else { Nodo.NodoArriba1 = Nodo1; Nodo.ValNodoArriba1 = G.Eps; }
            //Se Trabaja con el nodo1
            Nodo1.NodoArriba1 = Nodo2; Nodo1.ValNodoArriba1 = G.Eps;
            Nodo1.NodoAbajo1 = Nodo3; Nodo1.ValNodoAbajo1 = G.Eps;
            //Se trabaja con el nodo 2
            String ValExp = EXPSeparada.Pop();
            if (ValExp.Equals("*"))
            {
                NodoT Aux = PlantillaKleen(Nodo2);
                Aux.NodoArriba1 = Nodo4;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("|"))
            {
                NodoT Aux = PlantillaOr(Nodo2);
                Aux.NodoArriba1 = Nodo4;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("?"))
            {
                NodoT Aux = PlantillaSignoInterrogacion(Nodo2);
                Aux.NodoArriba1 = Nodo4;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("+"))
            {
                NodoT Aux = PlantillaCerraduraPositiva(Nodo2);
                Aux.NodoArriba1 = Nodo4;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("."))
            {
                NodoT Aux = PlantillaConcatenacion(Nodo2);
                Aux.NodoArriba1 = Nodo4;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else
            {
                Nodo2.NodoArriba1 = Nodo4;
                Nodo2.ValNodoArriba1 = ValExp;
            }
            //Se Trabaja con el nodo 3
            Nodo3.NodoArriba1 = Nodo5;Nodo3.ValNodoArriba1 = G.Eps;
            //Se trabaja con el nodo 4 y 5
            Nodo4.NodoArriba1 = Nodo6; Nodo4.ValNodoArriba1 = G.Eps;
            Nodo5.NodoArriba1 = Nodo6; Nodo5.ValNodoArriba1 = G.Eps;
            //Se retorna el 6to nodo
            return Nodo6;
        }
        public NodoT PlantillaCerraduraPositiva(NodoT Nodo) {
            NodoT Nodo1 = new NodoT(ContadorNodo); ContadorNodo++;
            if (Nodo == null) { Inicio = Nodo1; }
            else { Nodo.NodoArriba1 = Nodo1; Nodo.ValNodoArriba1 = G.Eps; }
            NodoT Nodo2 = new NodoT(ContadorNodo); ContadorNodo++;
            NodoT Nodo3 = new NodoT(ContadorNodo); ContadorNodo++;
            NodoT Nodo4 = new NodoT(ContadorNodo); ContadorNodo++;
            //Se Trabajo con el primer nodo de la plantilla de la cerradura de kleen
            Nodo1.NodoArriba1 = Nodo2;
            Nodo1.ValNodoArriba1 = G.Eps;
            //Se trabaja con el segundo nodo de la plantilla
            String ValExp = EXPSeparada.Pop();
            if (ValExp.Equals("*"))
            {
                NodoT AUX = PlantillaKleen(Nodo2);
                AUX.NodoArriba1 = Nodo3;
                AUX.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("|"))
            {
                NodoT AUX = PlantillaOr(Nodo2);
                AUX.NodoArriba1 = Nodo3;
                AUX.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("?"))
            {
                NodoT AUX = PlantillaSignoInterrogacion(Nodo2);
                AUX.NodoArriba1 = Nodo3;
                AUX.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("+"))
            {
                NodoT AUX = PlantillaCerraduraPositiva(Nodo2);
                AUX.NodoArriba1 = Nodo3;
                AUX.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("."))
            {
                NodoT AUX = PlantillaConcatenacion(Nodo2);
                AUX.NodoArriba1 = Nodo3;
                AUX.ValNodoArriba1 = G.Eps;
            }
            else
            {
                Nodo2.NodoArriba1 = Nodo3;
                Nodo2.ValNodoArriba1 = ValExp;
            }
            //Se Trabaja con el 3er nodo de la plantilla
            Nodo3.NodoArriba1 = Nodo2;
            Nodo3.ValNodoArriba1 = G.Eps;
            Nodo3.NodoAbajo1 = Nodo4;
            Nodo3.ValNodoAbajo1 = G.Eps;
            //se retorna el 4to nodo de la plantilla
            return Nodo4;
        }
    }
}
