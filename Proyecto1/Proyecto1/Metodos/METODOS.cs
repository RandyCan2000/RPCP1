                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Proyecto1.Globale;
using Proyecto1.TDA;

namespace Proyecto1.Metodos
{
    class METODOS
    {
        public Globales G = new Globales();
        Stack<String> EXPSeparada;
        Stack<NodoT> NODOS = null;
        Stack<TAFD> TabTranAFD = null;
        NodoT Inicio = null, Fin = null;
        int ContadorNodo = 0;
        int ContadorAFN = 0;
        String TEXTOAFN = "";
        TE[] TranEpsilon = null;
        String TranEps = "";
        Stack<string> Alfabeto = null;
        Stack<String[]> Conjuntos = null;
        Stack<LexemasValidar> LAV = null;
        public void CargarTablaToken(DataGridView DTG1, Stack<Token> tok) {
            DataTable Tab = new DataTable();
            Tab.Columns.Add("ID");
            Tab.Columns.Add("LEXEMA");
            for (int i = tok.Count - 1; i >= 0; i--) {
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
            for (int i = ERR.Count - 1; i >= 0; i--)
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

        public void CargarConsola(DataGridView Consola) {
            DataTable Tab = new DataTable();
            Tab.Columns.Add("LEXEMA");
            Tab.Columns.Add("ESTADO");
            foreach (LexemasValidar Aux in LAV)
            {
                DataRow Fila = Tab.NewRow();
                Fila["LEXEMA"] = Aux.Lexema1;
                if (!Aux.Aceptado1.Equals(""))
                {
                    Fila["ESTADO"] = Aux.Aceptado1;
                }
                else {
                    Fila["ESTADO"] = "NO ACEPTADO CON NINGUNA EXPRESION REGULAR";
                }
                Tab.Rows.Add(Fila);
            }
            Consola.DataSource = Tab;
            Consola.Update();
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
        public void BuscarExpReg(Stack<Token> TOKEN,DataGridView Consola) {
            for (int i = TOKEN.Count - 1; i >= 0; i--)
            {
                if (TOKEN.ElementAt(i).getLexema().Equals("CONJ"))
                {
                    for (int index = i; i >= 0; index--)
                    {
                        if (TOKEN.ElementAt(index).getLexema().Equals(";")) { i = index; break; }
                    }
                }
                else if (TOKEN.ElementAt(i).getLexema().Equals("->"))
                {
                    string Lexema = TOKEN.ElementAt(i - 1).getLexema();
                    Inicio = null; Fin = null;
                    ContadorNodo = 0;
                    NODOS = new Stack<NodoT>();
                    Alfabeto = new Stack<string>();
                    TEXTOAFN = "";
                    Console.WriteLine("NPILA:" + NODOS.Count);
                    SepararExpReg(Lexema);
                    i--;
                }
                else if (TOKEN.ElementAt(i).getLexema().Equals("%%")) { break; }
            }
            ImprimriLexemas();
            CargarConsola(Consola);
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
            while (EXPSeparada2.Count != 0) {
                EXPSeparada.Push(EXPSeparada2.Pop());
            }
            Console.WriteLine("EXP SEPARADA");
            GenerarThompshon();
            GenerarAFN();
            GenerarAFD();
            TabTransicionesAFD();
            GenerarAFDImagen();
            GeneraAutomata();
            ContadorAFN++;
            //Metodos que deben generar el automata
            G.contadorArbol++;
        }

        public void GenerarThompshon() {
            String ValExp = EXPSeparada.Pop();
            NodoT Aux = null;
            if (ValExp.Equals("*")) {
                Aux = PlantillaKleen(Inicio);
            }
            else if (ValExp.Equals("|")) {
                Aux = PlantillaOr(Inicio);
            }
            else if (ValExp.Equals("+")) {
                PlantillaCerraduraPositiva(Inicio);
            }
            else if (ValExp.Equals("?")) {
                PlantillaSignoInterrogacion(Inicio);
            }
            else if (ValExp.Equals(".")) {
                Aux = PlantillaConcatenacion(Inicio);
            }
            while (EXPSeparada.Count() != 0) {
                ValExp = EXPSeparada.Pop();
                if (ValExp.Equals("*"))
                {
                    PlantillaKleen(Fin);
                }
                else if (ValExp.Equals("|"))
                {
                    PlantillaOr(Fin);
                }
                else if (ValExp.Equals("+"))
                {
                    PlantillaCerraduraPositiva(Fin);
                }
                else if (ValExp.Equals("?"))
                {
                    PlantillaSignoInterrogacion(Fin);
                }
                else if (ValExp.Equals("."))
                {
                    PlantillaConcatenacion(Fin);
                }
            }
            Console.WriteLine(ContadorNodo);
        }

        public NodoT PlantillaKleen(NodoT Nodo) {
            NodoT Nodo1 = new NodoT(ContadorNodo); ContadorNodo++;
            if (Nodo == null) { Inicio = Nodo1; }
            else { Nodo.NodoArriba1 = Nodo1; Nodo.ValNodoArriba1 = G.Eps; }
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
                PlantillaSignoInterrogacion(Nodo2);
            }
            else if (ValExp.Equals("+"))
            {
                PlantillaCerraduraPositiva(Nodo2);
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
                if (!ValExp.Equals(G.Eps) && !Alfabeto.Contains(ValExp)) { Alfabeto.Push(ValExp); }
            }
            //Se Trabaja con el 3er nodo de la plantilla
            Nodo3.NodoArriba1 = Nodo2;
            Nodo3.ValNodoArriba1 = G.Eps;
            Nodo3.NodoAbajo1 = Nodo4;
            Nodo3.ValNodoAbajo1 = G.Eps;
            NODOS.Push(Nodo1); NODOS.Push(Nodo2); NODOS.Push(Nodo3); NODOS.Push(Nodo4);
            Fin = Nodo4;
            //se retorna el 4to nodo de la plantilla
            return Nodo4;
        }
        public NodoT PlantillaConcatenacion(NodoT Nodo) {
            NodoT Nodo1 = new NodoT(ContadorNodo); ContadorNodo++;
            NodoT Nodo2 = new NodoT(ContadorNodo); ContadorNodo++;
            NodoT Nodo3 = new NodoT(ContadorNodo); ContadorNodo++;
            //se valida el inicio
            if (Nodo == null) { Inicio = Nodo1; }
            else { Nodo.NodoArriba1 = Nodo1; Nodo.ValNodoArriba1 = G.Eps; }
            //se extrae el valor de la exp regular
            String ValExp = EXPSeparada.Pop();
            //se valida que tipo de valor se extrajo Nodo1
            if (ValExp.Equals("*"))
            {
                NodoT Aux = PlantillaKleen(Nodo1);
                Aux.NodoArriba1 = Nodo2;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("|"))
            {
                NodoT Aux = PlantillaOr(Nodo1);
                Aux.NodoArriba1 = Nodo2;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else if (ValExp.Equals("?"))
            {
                PlantillaSignoInterrogacion(Nodo1);
            }
            else if (ValExp.Equals("+"))
            {
                PlantillaCerraduraPositiva(Nodo1);
            }
            else if (ValExp.Equals("."))
            {
                NodoT Aux = PlantillaConcatenacion(Nodo1);
                Aux.NodoArriba1 = Nodo2;
                Aux.ValNodoArriba1 = G.Eps;
            }
            else
            {
                Nodo1.NodoArriba1 = Nodo2;
                Nodo1.ValNodoArriba1 = ValExp;
                if (!ValExp.Equals(G.Eps) && !Alfabeto.Contains(ValExp)) { Alfabeto.Push(ValExp); }
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
                PlantillaSignoInterrogacion(Nodo2);
            }
            else if (ValExp.Equals("+"))
            {
                PlantillaCerraduraPositiva(Nodo2);
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
                if (!ValExp.Equals(G.Eps) && !Alfabeto.Contains(ValExp)) { Alfabeto.Push(ValExp); }
            }
            NODOS.Push(Nodo1); NODOS.Push(Nodo2); NODOS.Push(Nodo3);
            //se retorna el ultimo nodo
            Fin = Nodo3;
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
                PlantillaSignoInterrogacion(Nodo2);
            }
            else if (ValExp.Equals("+"))
            {
                PlantillaCerraduraPositiva(Nodo2);
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
                if (!ValExp.Equals(G.Eps) && !Alfabeto.Contains(ValExp)) { Alfabeto.Push(ValExp); }
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
                PlantillaSignoInterrogacion(Nodo3);
            }
            else if (ValExp.Equals("+"))
            {
                PlantillaCerraduraPositiva(Nodo3);
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
                if (!ValExp.Equals(G.Eps) && !Alfabeto.Contains(ValExp)) { Alfabeto.Push(ValExp); }
            }
            //Se trabaja con el nodo 4 y 5
            Nodo4.NodoArriba1 = Nodo6; Nodo4.ValNodoArriba1 = G.Eps;
            Nodo5.NodoArriba1 = Nodo6; Nodo5.ValNodoArriba1 = G.Eps;
            //Se retorna el 6to nodo
            NODOS.Push(Nodo1); NODOS.Push(Nodo2); NODOS.Push(Nodo3); NODOS.Push(Nodo4); NODOS.Push(Nodo5); NODOS.Push(Nodo6);
            Fin = Nodo6;
            return Nodo6;
        }
        public void PlantillaSignoInterrogacion(NodoT Nodo) {
            string ValExp = EXPSeparada.Pop();
            EXPSeparada.Push(G.Eps);
            EXPSeparada.Push(ValExp);
            PlantillaConcatenacion(Nodo);
        }
        public void PlantillaCerraduraPositiva(NodoT Nodo) {
            String ValExp = EXPSeparada.Pop();
            EXPSeparada.Push(ValExp);
            EXPSeparada.Push("*");
            EXPSeparada.Push(ValExp);
            PlantillaConcatenacion(Nodo);
        }

        public void GenerarAFN() {
            try
            {
                StreamWriter archivo = new StreamWriter("C:\\GraphvizC\\dot\\AFN" + ContadorAFN + ".dot");
                archivo.WriteLine("digraph G{");
                GenerarAFNRecursivo(Inicio);
                archivo.WriteLine(TEXTOAFN);
                archivo.WriteLine("}");
                archivo.Close();
            }
            catch (Exception E) { MessageBox.Show("Error al guardar", "No Guardado", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            Process process = new Process();
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = @"C:\Program Files (x86)\Graphviz2.38\bin\dot.exe";
            process.StartInfo.Arguments = string.Format(@"-Tpng {0} -o  {1} ", @"C:\GraphvizC\dot\AFN" + ContadorAFN + ".dot", @"C:\GraphvizC\Imagen\AFN" + ContadorAFN + ".png");
            process.Start();
            process.WaitForExit();
        }


        public void GenerarAFNRecursivo(NodoT Nodo) {
            if (Nodo.Leido1 == false) {

                TEXTOAFN += "N" + Nodo.NumNodo1 + "[label =\"" + Nodo.NumNodo1 + "\"];\n";
                if (Nodo.NodoAbajo1 == null && Nodo.NodoArriba1 == null)
                {
                    TEXTOAFN += "N" + Nodo.NumNodo1 + " [shape = doublecircle];\n";
                    Fin = Nodo;
                }
                Nodo.Leido1 = true;
                if (Nodo.NodoArriba1 != null)
                {
                    GenerarAFNRecursivo(Nodo.NodoArriba1);
                    String Texto = "";
                    for (int j = 0; j < Nodo.ValNodoArriba1.Length; j++)
                    {
                        if (Nodo.ValNodoArriba1[j] != '"')
                        {
                            Texto += Nodo.ValNodoArriba1[j];
                        }
                    }
                    TEXTOAFN += "N" + Nodo.NumNodo1 + "-> N" + Nodo.NodoArriba1.NumNodo1 + "[label=\"" + Texto + "\"];";
                }
                if (Nodo.NodoAbajo1 != null)
                {
                    GenerarAFNRecursivo(Nodo.NodoAbajo1);
                    String Texto = "";
                    for (int j = 0; j < Nodo.ValNodoAbajo1.Length; j++)
                    {
                        if (Nodo.ValNodoAbajo1[j] != '"')
                        {
                            Texto += Nodo.ValNodoAbajo1[j];
                        }
                    }
                    TEXTOAFN += "N" + Nodo.NumNodo1 + "-> N" + Nodo.NodoAbajo1.NumNodo1 + "[label=\"" + Texto + "\"];";
                }
            }
        }

        //devuelve el valor de lectura de los nodos a false
        public void DevolverValLecturaFalse(NodoT Nodo) {
            if (Nodo.Leido1 != false) {
                Nodo.Leido1 = false;
                if (Nodo.NodoArriba1 != null)
                {
                    DevolverValLecturaFalse(Nodo.NodoArriba1);
                }
                if (Nodo.NodoAbajo1 != null)
                {
                    DevolverValLecturaFalse(Nodo.NodoAbajo1);
                }
            }
        }

        public void GenerarAFD() {
            TranEpsilon = new TE[ContadorNodo];
            int ContArreglo = 0;
            for (int i = 0; i < NODOS.Count(); i++) {
                NodoT aux = NODOS.ElementAt(i);
                TranEps = "";
                for (int j = 0; j < NODOS.Count(); j++) { NODOS.ElementAt(j).Leido1 = false; }
                BuscarTranEpsilon(aux);
                TranEpsilon[ContArreglo] = new TE();
                TranEpsilon[ContArreglo].Estado1 = aux.NumNodo1;
                TranEpsilon[ContArreglo].ConjuntoEps1 = TranEps;
                ContArreglo++;
            }
            //Se ordena el arreglo de estados
            TE[] Ordenado = new TE[ContadorNodo];
            int Cont = 0;
            for (int i = 0; i < ContadorNodo; i++) {
                for (int j = 0; j < TranEpsilon.Length; j++) {
                    if (TranEpsilon[j].Estado1 == i) {
                        Ordenado[Cont] = new TE();
                        Ordenado[Cont].Estado1 = TranEpsilon[j].Estado1;
                        //se ordena el conjunto del estado encontrado
                        String[] ConjuntoEps = TranEpsilon[j].ConjuntoEps1.Split(',');
                        int[] ConjOrdenado = new int[ConjuntoEps.Length];
                        int Contador = 0;
                        foreach (String Num in ConjuntoEps) {
                            ConjOrdenado[Contador] = Convert.ToInt32(Num);
                            Contador++;
                        }
                        Array.Sort(ConjOrdenado);
                        String ConjuntoOrdenadoCadena = "";
                        foreach (int Num in ConjOrdenado) {
                            if (ConjuntoOrdenadoCadena.Equals("")) { ConjuntoOrdenadoCadena = Convert.ToString(Num); }
                            else { ConjuntoOrdenadoCadena += "," + Convert.ToString(Num); }
                        }
                        Ordenado[Cont].ConjuntoEps1 = ConjuntoOrdenadoCadena;
                        Cont++;
                        break;
                    }
                }
            }
            TranEpsilon = Ordenado;
            //Se imprime el arrglo
            for (int i = 0; i < TranEpsilon.Length; i++)
            {
                Console.WriteLine("ESTADO: " + TranEpsilon[i].Estado1 + " CONJ: " + TranEpsilon[i].ConjuntoEps1);
            }
            //Se imprime el alfabeto
            for (int i = 0; i < Alfabeto.Count; i++)
            {
                Console.WriteLine("Opcion: " + Alfabeto.ElementAt(i));
            }
        }

        public void BuscarTranEpsilon(NodoT Nodo) {
            if (Nodo.Leido1 == false) {
                Nodo.Leido1 = true;
                if (TranEps.Equals("")) { TranEps = Convert.ToString(Nodo.NumNodo1); }
                else { TranEps += "," + Convert.ToString(Nodo.NumNodo1); }
                if (Nodo.NodoArriba1 != null)
                {
                    if (Nodo.ValNodoArriba1.Equals(G.Eps)) { BuscarTranEpsilon(Nodo.NodoArriba1); }
                }
                if (Nodo.NodoAbajo1 != null)
                {
                    if (Nodo.ValNodoAbajo1.Equals(G.Eps)) { BuscarTranEpsilon(Nodo.NodoAbajo1); }
                }
            }
        }

        public void TabTransicionesAFD() {
            TabTranAFD = new Stack<TAFD>();
            Stack<String> Conj = new Stack<string>();
            Stack<String> ConjRecorrido = new Stack<string>();
            TAFD Encabezado = new TAFD(Alfabeto.Count());
            for (int i = 0; i < Alfabeto.Count(); i++) {
                Encabezado.Opciones1[i] = Alfabeto.ElementAt(i);
            }
            TabTranAFD.Push(Encabezado);
            TAFD Nuevo = new TAFD(Alfabeto.Count());
            int Contador = 0;
            for (int i = 0; i < Alfabeto.Count(); i++) {
                String Conjunto = "";
                String[] EstadosSeparados = TranEpsilon[0].ConjuntoEps1.Split(',');
                Nuevo.Estado1 = Convert.ToString(TranEpsilon[0].Estado1);
                foreach (String Estado in EstadosSeparados) {
                    int numEstado = Convert.ToInt32(Estado);
                    for (int j = 0; j < NODOS.Count; j++) {
                        if (NODOS.ElementAt(j).NumNodo1 == numEstado) {
                            if (NODOS.ElementAt(j).ValNodoArriba1.Equals(Alfabeto.ElementAt(i))) {
                                if (Conjunto.Equals("")) { Conjunto = Convert.ToString(NODOS.ElementAt(j).NodoArriba1.NumNodo1); }
                                else { Conjunto += "," + Convert.ToString(NODOS.ElementAt(j).NodoArriba1.NumNodo1); }
                            }
                            if (NODOS.ElementAt(j).ValNodoAbajo1.Equals(Alfabeto.ElementAt(i)))
                            {
                                if (Conjunto.Equals("")) { Conjunto = Convert.ToString(NODOS.ElementAt(j).NodoAbajo1.NumNodo1); }
                                else { Conjunto += "," + Convert.ToString(NODOS.ElementAt(j).NodoAbajo1.NumNodo1); }
                            }
                            break;
                        }
                    }
                }
                Nuevo.Opciones1[Contador] = Conjunto;
                if (!Conjunto.Equals("")) { Conj.Push(Conjunto); }
                Contador++;
            }
            TabTranAFD.Push(Nuevo);
            TAFD NuevoAux = null;
            while (Conj.Count() != 0) {
                String C = Conj.Pop();
                ConjRecorrido.Push(C);
                String[] C_A_R = C.Split(',');
                foreach (String Aux in C_A_R) {
                    Contador = 0;
                    NuevoAux = new TAFD(Alfabeto.Count());
                    NuevoAux.Estado1 = C;
                    try {
                        for (int i = 0; i < Alfabeto.Count(); i++)
                        {
                            String Conjunto = "";
                            String[] EstadosSeparados = TranEpsilon[Convert.ToInt32(Aux)].ConjuntoEps1.Split(',');
                            foreach (String Estado in EstadosSeparados)
                            {
                                int numEstado = Convert.ToInt32(Estado);
                                for (int j = 0; j < NODOS.Count; j++)
                                {
                                    if (NODOS.ElementAt(j).NumNodo1 == numEstado)
                                    {
                                        if (NODOS.ElementAt(j).ValNodoArriba1.Equals(Alfabeto.ElementAt(i)))
                                        {
                                            if (Conjunto.Equals("")) { Conjunto = Convert.ToString(NODOS.ElementAt(j).NodoArriba1.NumNodo1); }
                                            else { Conjunto += "," + Convert.ToString(NODOS.ElementAt(j).NodoArriba1.NumNodo1); }
                                        }
                                        if (NODOS.ElementAt(j).ValNodoAbajo1.Equals(Alfabeto.ElementAt(i)))
                                        {
                                            if (Conjunto.Equals("")) { Conjunto = Convert.ToString(NODOS.ElementAt(j).NodoAbajo1.NumNodo1); }
                                            else { Conjunto += "," + Convert.ToString(NODOS.ElementAt(j).NodoAbajo1.NumNodo1); }
                                        }
                                        break;
                                    }

                                }
                            }
                            NuevoAux.Opciones1[Contador] = Conjunto;
                            if (!Conjunto.Equals("") && !ConjRecorrido.Contains(Conjunto)) { Conj.Push(Conjunto); }
                            Contador++;
                        }
                    } catch (Exception E) { }
                    Boolean insertar = true;
                    for (int q = 0; q < TabTranAFD.Count(); q++) {
                        if (TabTranAFD.ElementAt(q).Estado1.Equals(NuevoAux.Estado1)) { insertar = false; break; }
                    }
                    if (insertar == true) { TabTranAFD.Push(NuevoAux); }
                }
            }

        }

        public void GenerarAFDImagen() {
            //Recorrer Otros Conjuntos
            String Text = "";
            for (int i = TabTranAFD.Count - 1; i >= 0; i--)
            {
                Text = "Estado: " + TabTranAFD.ElementAt(i).Estado1 + " ";
                foreach (String Aux in TabTranAFD.ElementAt(i).Opciones1)
                {
                    Text += "Opcion:" + Aux + " ";
                }
                Console.WriteLine(Text);
            }
            try
            {
                StreamWriter archivo = new StreamWriter("C:\\GraphvizC\\dot\\TABTRAN" + ContadorAFN + ".dot");
                archivo.WriteLine("digraph G{");
                archivo.WriteLine("Tansiciones [label=< \n " +
                    "<table border = \"0\" cellborder = \"1\" cellspacing = \"0\"> \n" +
                    "<tr> \n" +
                    "<td><i> ESTADOS </i></td> \n");
                foreach (String Opcion in TabTranAFD.ElementAt(TabTranAFD.Count - 1).Opciones1) {
                    String Texto = "";
                    for (int a = 0; a < Opcion.Length; a++)
                    {
                        if ((int)Opcion[a] == 62) { Texto += "MQ"; }
                        else if ((int)Opcion[a] == 60) { Texto += "MQ"; }
                        else { Texto += Opcion[a]; }
                    }
                    archivo.WriteLine("<td><i> " + Texto + " </i></td> \n");
                }
                archivo.WriteLine("</tr>\n");

                for (int i = TabTranAFD.Count - 2; i >= 0; i--)
                {
                    archivo.WriteLine("<tr>\n");
                    archivo.WriteLine("<td>" + TabTranAFD.ElementAt(i).Estado1 + "</td> \n");
                    foreach (String Opcion in TabTranAFD.ElementAt(i).Opciones1)
                    {
                        archivo.WriteLine("<td>" + Opcion + "</td> \n");
                    }
                    archivo.WriteLine("</tr>\n");
                }
                archivo.WriteLine("</table> \n" +
                ">];");
                archivo.WriteLine("}");
                archivo.Close();
            }
            catch (Exception E) { MessageBox.Show("Error al guardar TabTran", "No Guardado", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            Process process = new Process();
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = @"C:\Program Files (x86)\Graphviz2.38\bin\dot.exe";
            process.StartInfo.Arguments = string.Format(@"-Tpng {0} -o  {1} ", @"C:\GraphvizC\dot\TABTRAN" + ContadorAFN + ".dot", @"C:\GraphvizC\Imagen\TABTRAN" + ContadorAFN + ".png");
            process.Start();
            process.WaitForExit();
        }

        public void GeneraAutomata() {
            Stack<Estados> NombreEstados = new Stack<Estados>();
            Stack<TAFD> TAFDAUX = new Stack<TAFD>();
            int Contador = 0;
            for (int i = TabTranAFD.Count - 1; i >= 0; i--) {
                TAFD a = TabTranAFD.ElementAt(i);
                if (!a.Estado1.Equals("")) {
                    Estados NewEstado = new Estados("Q" + Contador, a.Estado1);
                    Console.WriteLine("Q" + Contador + " " + a.Estado1);
                    NombreEstados.Push(NewEstado);
                    Contador++;
                }
            }
            foreach (TAFD a in TabTranAFD)
            {
                TAFD Nuevo = new TAFD(Alfabeto.Count);
                for (int i = 0; i < NombreEstados.Count; i++) {
                    String Estado = NombreEstados.ElementAt(i).Conjunto1;
                    String NomEstado = NombreEstados.ElementAt(i).Estado1;
                    if (a.Estado1.Equals(Estado)) { Nuevo.Estado1 = NomEstado; break; }
                }
                for (int i = 0; i < a.Opciones1.Length; i++) {
                    String Estado = a.Opciones1[i];
                    for (int j = 0; j < NombreEstados.Count; j++) {
                        if (Estado.Equals(NombreEstados.ElementAt(j).Conjunto1)) { Nuevo.Opciones1[i] = NombreEstados.ElementAt(j).Estado1; }
                    }
                }
                TAFDAUX.Push(Nuevo);
            }
            try
            {
                StreamWriter archivo = new StreamWriter("C:\\GraphvizC\\dot\\AUTOMATA" + ContadorAFN + ".dot");
                archivo.WriteLine("digraph G{");
                foreach (Estados a in NombreEstados) {
                    archivo.WriteLine(a.Estado1 + " [label=\"" + a.Estado1 + "\"];");
                }
                for (int i = 0; i < TAFDAUX.Count; i++)
                {
                    String Estado = TAFDAUX.ElementAt(i).Estado1;
                    for (int j = 0; j < TAFDAUX.ElementAt(i).Opciones1.Length; j++) {
                        if (!TAFDAUX.ElementAt(i).Opciones1[j].Equals("")) {
                            String aNodo = TAFDAUX.ElementAt(i).Opciones1[j];
                            String Texto = "";
                            for (int k = 0; k < TabTranAFD.ElementAt(TabTranAFD.Count - 1).Opciones1[j].Length; k++) {
                                if (TabTranAFD.ElementAt(TabTranAFD.Count - 1).Opciones1[j][k] != '"') {
                                    Texto += TabTranAFD.ElementAt(TabTranAFD.Count - 1).Opciones1[j][k];
                                }
                            }
                            archivo.WriteLine(Estado + "->" + aNodo + "[label=\"" + Texto + "\"];");
                        }
                    }
                }
                archivo.WriteLine("}");
                archivo.Close();
            }
            catch (Exception E) { MessageBox.Show("Error al guardar Automata", "No Guardado", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            Process process = new Process();
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = @"C:\Program Files (x86)\Graphviz2.38\bin\dot.exe";
            process.StartInfo.Arguments = string.Format(@"-Tpng {0} -o  {1} ", @"C:\GraphvizC\dot\AUTOMATA" + ContadorAFN + ".dot", @"C:\GraphvizC\Imagen\AUTOMATA" + ContadorAFN + ".png");
            process.Start();
            process.WaitForExit();
            try { ValidarLexemas(TAFDAUX); }
            catch (Exception E) { Console.WriteLine("ERROR AL VALIDAR LEXEMA CON UNA EXPRESION"+ E); }
        }

        public Boolean BuscarNodoAceptacion(NodoT Nodo) {
            Boolean Aceptar = false;
            Nodo.Leido1 = true;
            if (Nodo == Fin) { Aceptar = true; }
            if (Nodo.NodoArriba1 != null && Nodo.ValNodoArriba1.Equals(G.Eps) && Nodo.Leido1 == false) {
                Aceptar = BuscarNodoAceptacion(Nodo.NodoArriba1);
            }
            if (Nodo.NodoAbajo1 != null && Nodo.ValNodoAbajo1.Equals(G.Eps) && Nodo.Leido1 == false) {
                Aceptar = BuscarNodoAceptacion(Nodo.NodoAbajo1);
            }
            return Aceptar;
        }

        public String[] RecuperarImagenes() {
            DirectoryInfo di = new DirectoryInfo(@"C:\GraphvizC\Imagen");
            var Directorio = di.GetFiles();
            int Contador = 0;
            foreach (var fi in Directorio)
            {
                Contador++;
            }
            String[] Dir = new String[Contador];
            Contador = 0;
            foreach (var fi in Directorio) {
                Dir[Contador] = Convert.ToString(fi);
                Contador++;
            }
            return Dir;
        }

        public void CrearXMLToken(Stack<Token> tokens) {
            StreamWriter archivo = new StreamWriter(@"C:\GraphvizC\Archivos\XMLTOKENS.xml");
            archivo.WriteLine("<ListaToken>");
            foreach (Token Aux in tokens) {
                archivo.WriteLine("<Token>");
                archivo.WriteLine("<ID>" + Aux.getID() + "</ID>");
                String Texto = "";
                for (int i = 0; i < Aux.getLexema().Length; i++) {
                    if (Aux.getLexema()[i] == '>') { Texto += "MAYORQUE"; }
                    else if (Aux.getLexema()[i] == '<') { Texto += "MENORQUE"; }
                    else { Texto += Aux.getLexema()[i]; }
                }
                archivo.WriteLine("<LEXEMA>" + Texto + "</LEXEMA>");
                archivo.WriteLine("</Token>");
            }
            archivo.WriteLine("</ListaToken>");
            archivo.Close();
        }
        public void CrearXMLErrores(Stack<Error> Errores) {
            StreamWriter archivo = new StreamWriter(@"C:\GraphvizC\Archivos\XMLERRORES.xml");
            archivo.WriteLine("<ListaErrores>");
            foreach (Error Aux in Errores) {
                archivo.WriteLine("<ERRORES>");
                archivo.WriteLine("<ID>" + Aux.getID() + "</ID>");
                String Texto = "";
                for (int i = 0; i < Aux.getLexema().Length; i++)
                {
                    if (Aux.getLexema()[i] == '>') { Texto += "MAYORQUE"; }
                    else if (Aux.getLexema()[i] == '<') { Texto += "MENORQUE"; }
                    else { Texto += Aux.getLexema()[i]; }
                }
                archivo.WriteLine("<LEXEMA>" + Texto + "</LEXEMA>");
                archivo.WriteLine("<FILA>" + Aux.getFila() + "</FILA>");
                archivo.WriteLine("<COLUMNA>" + Aux.getColumna() + "</COLUMNA>");
                Texto = "";
                for (int i = 0; i < Aux.getEsperado().Length; i++)
                {
                    if (Aux.getEsperado()[i] == '>') { Texto += "MAYORQUE"; }
                    else if (Aux.getEsperado()[i] == '<') { Texto += "MENORQUE"; }
                    else { Texto += Aux.getEsperado()[i]; }
                }
                archivo.WriteLine("<ESPERADO>" + Texto + "</ESPERADO>");
                archivo.WriteLine("</ERRORES>");
            }

            archivo.WriteLine("</ListaErrores>");
            archivo.Close();
        }
        public void CrearPDFErrores(Stack<Error> Error) {
            FileStream fs = new FileStream(@"C:\GraphvizC\Archivos\XMLERRORES.pdf", FileMode.Create);
            Document documento = new Document(iTextSharp.text.PageSize.LETTER, 50, 50, 30, 30);
            PdfWriter pw = PdfWriter.GetInstance(documento, fs);
            documento.Open();
            PdfPTable table = new PdfPTable(5);
            table.AddCell("ID");
            table.AddCell("LEXEMA");
            table.AddCell("FILA");
            table.AddCell("COLUMNA");
            table.AddCell("ESPERADO");
            foreach (Error Aux in Error) {
                table.AddCell(Convert.ToString(Aux.getID()));
                table.AddCell(Aux.getLexema());
                table.AddCell(Convert.ToString(Aux.getFila()));
                table.AddCell(Convert.ToString(Aux.getColumna()));
                table.AddCell(Aux.getEsperado());
            }
            documento.Add(table);
            documento.Close();
        }
        public void CrearConjuntos(Stack<Token> Tokens) {
            Conjuntos = new Stack<string[]>();
            for (int i = Tokens.Count - 1; i >= 0; i--) {
                if (Tokens.ElementAt(i).getLexema().Equals("CONJ")) {
                    if (Tokens.ElementAt(i - 4).getLexema().Equals("{")) {
                        String Conj = Tokens.ElementAt(i - 2).getLexema() + "," + Tokens.ElementAt(i - 5).getLexema();
                        String[] Conjunto = Conj.Split(',');
                        Conjuntos.Push(Conjunto);
                        i = i - 5;
                    }
                    else {
                        try {
                            String Conj = Tokens.ElementAt(i - 4).getLexema();
                            String[] Conjunto = Conj.Split('~');
                            int Inicio = (int)Convert.ToChar(Conjunto[0]);
                            int Fin = (int)Convert.ToChar(Conjunto[1]);
                            String ConjFinal = "{" + Tokens.ElementAt(i - 2).getLexema() + "}";
                            if (Inicio < Fin)
                            {
                                for (int j = Inicio; j <= Fin; j++)
                                {
                                    ConjFinal += "," + Convert.ToChar(j);
                                }
                            }
                            else
                            {
                                for (int j = Inicio; j >= Fin; j--) {
                                    ConjFinal += "," + Convert.ToChar(j);
                                }
                            }
                            String[] Conjunto2 = ConjFinal.Split(',');
                            Conjuntos.Push(Conjunto2);
                            i = i - 4;
                        }
                        catch (Exception E) {
                            Console.WriteLine("NO SE CREO EL CONJUNTO CORRECTAMENTE");
                        }
                    }
                }
            }
            ImprimirConjuntos();
            RecuperarLexemas(Tokens);
        }

        public void ImprimirConjuntos() {
            foreach (String[] Conj in Conjuntos) {
                foreach (String ElementoConj in Conj) {
                    Console.WriteLine(ElementoConj);
                }
            }
        }

        public void RecuperarLexemas(Stack<Token> tokens) {
            LAV = new Stack<LexemasValidar>();
            for (int i = tokens.Count - 1; i >= 0; i--) {
                if (tokens.ElementAt(i).getLexema().Equals("{") || tokens.ElementAt(i).getLexema().Equals("}") || tokens.ElementAt(i).getLexema().Equals(";")) { }
                else if (tokens.ElementAt(i).getLexema().Equals("//")) { i = i - 1; }
                else if (tokens.ElementAt(i).getLexema().Equals("CONJ")) { i = i - 6; }
                else if (tokens.ElementAt(i).getLexema().Equals("<!")) { i = i - 2; }
                else {
                    if (tokens.ElementAt(i - 1).getLexema().Equals("->")) { i = i - 2; }
                    else if (tokens.ElementAt(i - 1).getLexema().Equals(":")) {
                        LexemasValidar Nuevo = new LexemasValidar(tokens.ElementAt(i - 3).getLexema());
                        LAV.Push(Nuevo);
                    }
                }
            }
        }
        public void ImprimriLexemas() {
            foreach (LexemasValidar Aux in LAV) {
                Console.WriteLine(Aux.Lexema1+"-"+Aux.Aceptado1);
            }
        }

        public void ValidarLexemas(Stack<TAFD> Tran) {
            Boolean Termino = true;
            Stack<char> Lexema = null;
            int NumeroIntentos = 0;
            foreach (LexemasValidar Aux in LAV) {
                Termino = true;
                Console.WriteLine("VALIDAR LEXEMA");
                if (Aux.Aceptado1.Equals("")) {
                    Lexema = new Stack<char>();
                    for (int i = 0; i < Aux.Lexema1.Length; i++) {
                        Lexema.Push(Aux.Lexema1[i]);
                    }
                    int num = 0 + 1;//Estado actual +1 
                    for (int i = Lexema.Count - 1; i >= 0; i--)
                    {
                        char Letra = Lexema.ElementAt(i);
                        TAFD a = Tran.ElementAt(num);
                        NumeroIntentos = 0;
                        for (int j = 0; j < a.Opciones1.Length; j++)
                        {
                            String b = a.Opciones1[j];
                            if (!b.Equals(""))
                            {
                                if (Alfabeto.ElementAt(j)[0] == '"')
                                {
                                    if (Alfabeto.ElementAt(j)[1] == Letra) {
                                        for (int W=0;W<Tran.Count;W++) {
                                            if (Tran.ElementAt(W).Estado1.Equals(b)) { num = W; break; }
                                        }
                                    }
                                    else { NumeroIntentos++; }
                                }
                                else
                                {
                                    Boolean encontro = false;
                                    Boolean encontroLetra = false;
                                    foreach (String[] Conj in Conjuntos) {
                                        if (Conj[0].Equals(Alfabeto.ElementAt(j))) {
                                            encontro = true;
                                            for (int t = 1; t < Conj.Length; t++) {
                                                if (Conj[t].Equals(Letra)) {
                                                    for (int W = 0; W < Tran.Count; W++)
                                                    {
                                                        if (Tran.ElementAt(W).Estado1.Equals(b)) { num = W; break; }
                                                    }
                                                    encontroLetra = true; break; }
                                            }
                                            break;
                                        }
                                    }
                                    if (encontro == true && encontroLetra == true) { }
                                    else { NumeroIntentos++; }
                                }
                            }
                            else { NumeroIntentos++; }
                            if (NumeroIntentos == a.Opciones1.Length) { Termino = false; break; }
                        }
                        if (Termino == false) { break; }
                    }
                    if (Termino == true) { Aux.Aceptado1 = "ACEPTADO CON EXPRECION REGULAR NUMERO " + ContadorAFN; }
                }
            }
        }

    }
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       