using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
        Stack<NodoT> NODOS = null;
        Stack<TAFD> TabTranAFD = null; 
        NodoT Inicio = null, Fin = null;
        int ContadorNodo = 0;
        int ContadorAFN = 0;
        String TEXTOAFN = "";
        TE[] TranEpsilon = null;
        String TranEps = "";
        Stack<string> Alfabeto = null;
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
        public void BuscarExpReg(Stack<Token> TOKEN) {
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
                    PlantillaKleen(Aux);
                }
                else if (ValExp.Equals("|"))
                {
                    PlantillaOr(Aux);
                }
                else if (ValExp.Equals("+"))
                {
                    PlantillaCerraduraPositiva(Aux);
                }
                else if (ValExp.Equals("?"))
                {
                    PlantillaSignoInterrogacion(Aux);
                }
                else if (ValExp.Equals("."))
                {
                    PlantillaConcatenacion(Aux);
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
            for (int i=0;i<NODOS.Count();i++){
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
                for (int j=0;j<TranEpsilon.Length;j++) {
                    if (TranEpsilon[j].Estado1==i) {
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
                            else { ConjuntoOrdenadoCadena += ","+Convert.ToString(Num); }
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
            for (int i=0;i<Alfabeto.Count();i++) {
                Encabezado.Opciones1[i] =Alfabeto.ElementAt(i);
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
                        if (NODOS.ElementAt(j).NumNodo1==numEstado) {
                            if (NODOS.ElementAt(j).ValNodoArriba1.Equals(Alfabeto.ElementAt(i))) {
                                if (Conjunto.Equals("")) { Conjunto = Convert.ToString(NODOS.ElementAt(j).NodoArriba1.NumNodo1); }
                                else { Conjunto += ","+Convert.ToString(NODOS.ElementAt(j).NodoArriba1.NumNodo1); }
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
            //Recorrer Demas Conjuntos
            String Text = "";
            for (int i=TabTranAFD.Count-1;i>=0;i--) {
                Text = "Estado: "+TabTranAFD.ElementAt(i).Estado1+" ";
                foreach (String Aux in TabTranAFD.ElementAt(i).Opciones1) {
                    Text += "Opcion:" + Aux+" ";
                }
                Console.WriteLine(Text);
            }
        }

    }
}
