using System;
using Proyecto1.TDA;
using Proyecto1.Globale;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Proyecto1.Metodos
{
    class Automata
    {
        
        public Globales G = new Globales();
        int Estado = 0;
        String Token = "";
        Boolean AceptadoConjuntos = true;
        char Caracter;
        Error ERRC;
        int Fila = 1;
        int Columna = 1;
        public void AutomataConjuntos(String Linea,DataGridView DTG1, DataGridView DTG2)
        {
            G.TOKEN = new Stack<Token>();
            G.ERROR = new Stack<Error>();
            Boolean Terminar = false;
            Boolean ValuarExpReg = false;
            Boolean ESPACIOS = false;
            int CorchetesExpReg = 0;
            int ComillasExpReg = 0;
            for (int Col = 0; Col < Linea.Length; Col++)
            {
                Columna++;
                Caracter = Linea[Col];
                if (Caracter == '\n' || Caracter == '\t') { }
                else if ((int)Caracter <= 31 || (int)Caracter >= 127)
                {
                    Estado = 200;
                    ERRC = new Error(G.ERROR.Count, Char.ToString(Caracter), Fila, Columna, "NO VALIDO");
                }
                if (Caracter == '\n') { Fila++; Columna = 0; }
                if (Terminar == true && Caracter != ' ' && Caracter != '\t' && Caracter != '\n')
                {
                    Estado = 200;
                    ERRC = new Error(G.ERROR.Count, Char.ToString(Caracter), Fila, Columna, "NADA");
                }
                switch (Estado)
                {
                    case 0:
                        if (Caracter == ' ' || Caracter == '\n' || Caracter == '\t') { }
                        else if (Caracter == '{')
                        {
                            Estado = 1;
                            Token TOK = new Token(G.TOKEN.Count, "{");
                            Console.WriteLine(TOK.toString());
                            G.TOKEN.Push(TOK);
                        }
                        else
                        {
                            ERRC = new Error(G.ERROR.Count, Char.ToString(Caracter), Fila, Columna, "{");
                            Console.WriteLine(ERRC.toString());
                            G.ERROR.Push(ERRC);
                            Col = Linea.Length;
                        }
                        break;
                    case 1:
                        if (Caracter == ' ' || Caracter == '\n' || Caracter == '\t') { }
                        else if (Caracter == 'C')
                        {
                            Token += Caracter;
                            Estado = 2;
                        }
                        else if (Caracter == '/')
                        {
                            Estado = 4;
                            Token += Caracter;
                        }
                        else if (Caracter == '<')
                        {
                            Estado = 9;
                            Token = "<";
                        }
                        else if (Caracter == '}')
                        {
                            Token Z1 = new Token(G.TOKEN.Count, "}");
                            Console.WriteLine(Z1.toString());
                            G.TOKEN.Push(Z1);
                            Terminar = true;
                        }
                        /*else if (Caracter == '%')
                        {
                            Estado = 26;
                            Token = "%";
                        }*/
                        else
                        {
                            // Nombre Expresiones
                            Estado = 20;
                            Token = Char.ToString(Caracter);
                        }
                        break;
                    case 2:
                        switch (Caracter)
                        {
                            case 'O':
                                Estado = 2; Token += Caracter;
                                break;
                            case 'N':
                                Estado = 2; Token += Caracter;
                                break;
                            case 'J':
                                Estado = 3; Token += Caracter;
                                Token b1 = new Token(G.TOKEN.Count, Token);
                                Console.WriteLine(b1.toString());
                                G.TOKEN.Push(b1); Token = "";
                                break;
                            default:
                                Estado = 200;
                                ERRC = new Error(G.ERROR.Count, Token, Fila, Columna, "CONJ");
                                break;
                        }
                        break;
                    case 3:
                        if (Caracter == ' ' || Caracter == '\t') { }
                        else if (Caracter == ':')
                        {
                            Token bh = new Token(G.TOKEN.Count, ":");
                            Console.WriteLine(bh.toString());
                            G.TOKEN.Push(bh);
                            Token = "";
                            Estado = 5;
                        }
                        else
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, Char.ToString(Caracter), Fila, Columna, ":");
                        }
                        break;
                    case 4:
                        if (Caracter == '/')
                        {
                            Token += Caracter;
                            Token a1 = new Token(G.TOKEN.Count, Token); Console.WriteLine(a1.toString()); G.TOKEN.Push(a1); Token = "";
                            for (int aux = Col + 1; aux < Linea.Length; aux++)
                            {
                                if (Linea[aux] == '\n')
                                {
                                    Col = aux; Columna = 0; Fila++;
                                    break;
                                }
                                else { Token += Linea[aux]; }
                            }
                            Token b2 = new Token(G.TOKEN.Count, Token);
                            Console.WriteLine(b2.toString());
                            G.TOKEN.Push(b2);
                            Token = "";
                            if (ValuarExpReg == false) { Estado = 1; }
                            else { Estado = 30; }

                        }
                        break;
                    case 5:
                        if (Caracter == ' ' || Caracter == '\t') { }
                        else if (Caracter == '\n')
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, "SALTO DE LINEA", Fila, Columna, "Variable");
                        }
                        else
                        {
                            Token += Caracter;
                            Estado = 6;
                        }
                        break;
                    case 6:
                        if (Caracter == ' ' || Caracter == '\t')
                        {
                            Token b3 = new Token(G.TOKEN.Count, Token);
                            Console.WriteLine(b3.toString());
                            G.TOKEN.Push(b3); Token = ""; Estado = 7;
                        }
                        else if (Caracter == '-')
                        {
                            Token b4 = new Token(G.TOKEN.Count, Token);
                            Console.WriteLine(b4.toString());
                            G.TOKEN.Push(b4); Token = "-"; Estado = 8;
                        }
                        else if (Caracter == '\n')
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, "SALTO DE LINEA", Fila, Columna, "->");
                        }
                        else
                        {
                            if (Caracter == '-')
                            {
                                Token b5 = new Token(G.TOKEN.Count, Token);
                                Console.WriteLine(b5.toString());
                                Token = "";
                                G.TOKEN.Push(b5);
                                Estado = 7;
                                Token = "-";
                            }
                            else { Token += Caracter; }

                        }
                        break;
                    case 7:
                        if (Caracter == ' ' || Caracter == '\t') { }
                        else if (Caracter == '\n')
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, "SALTO DE LINEA", Fila, Columna, "-");
                        }
                        else if (Caracter == '-')
                        {
                            Token = "-";
                            Estado = 8;
                        }
                        else
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, Char.ToString(Caracter), Fila, Columna, "-");
                        }
                        break;
                    case 8:
                        // signo > mayor que
                        if ((int)Caracter == 62)
                        {
                            Token b6 = new Token(G.TOKEN.Count, "->");
                            Console.WriteLine(b6.toString());
                            Token = "";
                            G.TOKEN.Push(b6);
                            Estado = 11;
                        }
                        else
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, Token, Fila, Columna, "->");
                        }
                        break;
                    case 9:
                        if (Caracter == '!')
                        {
                            Token += '!';
                            Token b7 = new Token(G.TOKEN.Count, Token);
                            Console.WriteLine(b7.toString());
                            G.TOKEN.Push(b7);
                            Token = "";
                            Estado = 10;
                        }
                        else
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, Token, Fila, Columna, "<!");
                        }
                        break;
                    case 10:
                        for (int aux = Col; aux < Linea.Length; aux++)
                        {
                            try
                            {
                                if (Linea[aux] == '!' && Linea[aux + 1] == '>')
                                {
                                    Col = aux + 1;
                                    break;
                                }
                                else if (Linea[aux] == '\n')
                                {
                                    Token += Linea[aux];
                                    Fila++; Columna = 1;
                                }
                                else
                                {
                                    Token += Linea[aux];
                                }
                            }
                            catch (Exception E)
                            {
                                aux--;
                                Estado = 200;
                                ERRC = new Error(G.ERROR.Count, Token, Fila, Columna, "!>");
                            }
                        }
                        Token b = new Token(G.TOKEN.Count, Token);
                        Console.WriteLine(b.toString());
                        G.TOKEN.Push(b);
                        Token = "";
                        Token a = new Token(G.TOKEN.Count, "!>");
                        Console.WriteLine(a.toString());
                        G.TOKEN.Push(a);
                        Token = "";
                        if (ValuarExpReg == false) { Estado = 1; }
                        else { Estado = 30; }
                        break;
                    case 11:
                        if (Caracter == '{')
                        {
                            Token Z = new Token(G.TOKEN.Count, "{");
                            Console.WriteLine(Z.toString());
                            G.TOKEN.Push(Z);
                            Estado = 12;
                        }
                        else if (Caracter == ' ' || Caracter == '\t') { }
                        else if (Caracter == '\n')
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, "SALTO DE LINEA", Fila, Columna, "-");
                        }
                        else
                        {
                            try
                            {
                                int q = Int32.Parse(Char.ToString(Caracter));
                                Console.WriteLine(q);
                                Token = Char.ToString(Caracter);
                                Estado = 18;
                            }
                            catch (Exception E)
                            {
                                Token = Char.ToString(Caracter);
                                Estado = 15;
                            }

                        }
                        break;
                    case 12:
                        if (Caracter == '\t' || Caracter == ' ') { }
                        else
                        {
                            Token = Char.ToString(Caracter);
                            Estado = 13;
                        }
                        break;
                    case 13:
                        if (Caracter == '}')
                        {
                            Token Z = new Token(G.TOKEN.Count, Token);
                            Console.WriteLine(Z.toString());
                            G.TOKEN.Push(Z);
                            Token Z1 = new Token(G.TOKEN.Count, "}");
                            Console.WriteLine(Z1.toString());
                            G.TOKEN.Push(Z1);
                            Token = "";
                            Estado = 14;
                        }
                        else if (Caracter == ',')
                        {
                            Token += Caracter;
                            Estado = 13;
                        }
                        else if (Caracter == ' ')
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, " ", Fila, Columna, "Valores sin Espacio en Conjunto");
                        }
                        else
                        {
                            Token += Caracter;
                            Estado = 13;
                        }
                        break;
                    case 14:
                        if (Caracter == '\t' || Caracter == ' ') { }
                        else if (Caracter == ';')
                        {
                            Token Z1 = new Token(G.TOKEN.Count, ";");
                            Console.WriteLine(Z1.toString());
                            G.TOKEN.Push(Z1);
                            Token = "";
                            Estado = 1;
                        }
                        else
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, Char.ToString(Caracter), Fila, Columna, ";");
                        }
                        break;
                    case 15:
                        if (Caracter == ' ' || Caracter == '\t') { }
                        else if (Caracter == '~')
                        {
                            Token += Caracter;
                            Estado = 16;
                        }
                        else
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, Char.ToString(Caracter), Fila, Columna, "~");
                        }
                        break;
                    case 16:
                        if (Caracter == ' ' || Caracter == '\t') { }
                        else if (Caracter == '\n')
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, Char.ToString(Caracter), Fila, Columna, "Letra");
                        }
                        else
                        {
                            try
                            {
                                int q = Int32.Parse(Char.ToString(Caracter));
                                Console.WriteLine(q);
                                Estado = 200;
                                ERRC = new Error(G.ERROR.Count, Char.ToString(Caracter), Fila, Columna, "Letra");
                            }
                            catch (Exception E)
                            {
                                Token += Caracter;
                                Token Z1 = new Token(G.TOKEN.Count, Token);
                                Console.WriteLine(Z1.toString());
                                G.TOKEN.Push(Z1);
                                Token = "";
                                Estado = 17;
                            }
                        }
                        break;
                    case 17:
                        if (Caracter == ' ' || Caracter == '\t') { }
                        else if (Caracter == ';')
                        {
                            Token Z1 = new Token(G.TOKEN.Count, ";");
                            Console.WriteLine(Z1.toString());
                            G.TOKEN.Push(Z1);
                            Token = "";
                            Estado = 1;
                        }
                        else
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, "SALTO DE LINEA", Fila, Columna, ";");
                        }
                        break;
                    case 18:
                        if (Caracter == '\n' || Caracter == ' ' || Caracter == '\n')
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, "SIN ESPACIOS EN CONJUNTO", Fila, Columna, "~");
                        }
                        else if (Caracter == '~')
                        {
                            Token += Caracter;
                            Estado = 19;
                        }
                        else
                        {
                            try
                            {
                                int q = Int32.Parse(Char.ToString(Caracter));
                                Console.WriteLine(q);
                                Token += Caracter;
                            }
                            catch (Exception E)
                            {
                                Estado = 200;
                                ERRC = new Error(G.ERROR.Count, Char.ToString(Caracter), Fila, Columna, "Digito");
                            }
                        }
                        break;
                    case 19:
                        if (Caracter == '\n' || Caracter == ' ' || Caracter == '\t')
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, "SIN ESPACIOS EN CONJUNTO", Fila, Columna, "DIGITO");
                        }
                        else if (Caracter == ';')
                        {
                            Token Z1 = new Token(G.TOKEN.Count, Token);
                            Console.WriteLine(Z1.toString());
                            G.TOKEN.Push(Z1);
                            Z1 = new Token(G.TOKEN.Count, ";");
                            Console.WriteLine(Z1.toString());
                            G.TOKEN.Push(Z1);
                            Token = "";
                            Estado = 1;
                        }
                        else
                        {
                            try
                            {
                                int q = Int32.Parse(Char.ToString(Caracter));
                                Console.WriteLine(q);
                                Token += Caracter;
                            }
                            catch (Exception E)
                            {
                                Estado = 200;
                                ERRC = new Error(G.ERROR.Count, Char.ToString(Caracter), Fila, Columna, "Digito");
                            }
                        }
                        break;
                    case 20:
                        if (Caracter == ' ' || Caracter == '\t')
                        {
                            Token Z1 = new Token(G.TOKEN.Count, Token);
                            Console.WriteLine(Z1.toString());
                            G.TOKEN.Push(Z1);
                            Token = "";
                            Estado = 21;
                        }
                        else if (Caracter == '-')
                        {
                            Token Z1 = new Token(G.TOKEN.Count, Token);
                            Console.WriteLine(Z1.toString());
                            G.TOKEN.Push(Z1);
                            Token = "-";
                            Estado = 22;
                        } else if (Caracter==':') {
                            Token Z1 = new Token(G.TOKEN.Count, ":");
                            Console.WriteLine(Z1.toString());
                            G.TOKEN.Push(Z1);
                            Token = "";
                            Estado = 34;
                        }
                        else if (Caracter == '\n')
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, "SALTO DE LINEA", Fila, Columna, "->");
                        }
                        else
                        {
                            Token += Caracter;
                        }
                        break;
                    case 21:
                        if (Caracter == ' ' || Caracter == '\t') { }
                        else if (Caracter == '\n')
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, "SALTO DE LINEA", Fila, Columna, "->");
                        }
                        else if (Caracter == '-')
                        {
                            Token += Caracter;
                            Estado = 22;
                        } else if (Caracter==':') {
                            Token Z1 = new Token(G.TOKEN.Count, ":");
                            Console.WriteLine(Z1.toString());
                            G.TOKEN.Push(Z1);
                            Token = "";
                            Estado = 34;
                        }
                        else
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, Char.ToString(Caracter), Fila, Columna, "->");
                        }
                        break;
                    case 22:
                        //signo mayor que >
                        if ((int)Caracter == 62)
                        {
                            Token += Caracter;
                            Token Z1 = new Token(G.TOKEN.Count, Token);
                            Console.WriteLine(Z1.toString());
                            G.TOKEN.Push(Z1);
                            Token = "";
                            Estado = 23;
                        }
                        else
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, Char.ToString(Caracter), Fila, Columna, "->");
                        }
                        break;
                    case 23:
                        if (Caracter == ' ' || Caracter == '\t') { }
                        else if (Caracter == '\n')
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, "SALTO DE LINEA", Fila, Columna, "EXPRESION REGULAR");
                        }
                        else
                        {
                            Col--;
                            Estado = 24;
                        }
                        break;
                    case 24:
                        if (Caracter == '\"')
                        {
                            Token += Caracter;
                            ComillasExpReg++;
                            ESPACIOS = true;
                            Estado = 25;
                        } else if (Caracter=='[') {
                            Token += "\"";
                            if (Linea[Col + 1] == ':')
                            {
                                for (int i = Col + 2; i < Linea.Length; i++)
                                {
                                    if (Linea[i] == ':' && Linea[i + 1] == ']')
                                    {
                                        Token += "\"";
                                        Col = i + 1;
                                        break;
                                    } else if (Linea[i]=='\n') {
                                        Col = i;
                                        Estado = 200;
                                        ERRC = new Error(G.ERROR.Count,"SALTO DE LINEA", Fila, Columna, "NO SALTO DE LINEA EN EXP REGULAR");
                                        break;
                                    }
                                    else { Token += Linea[i]; }
                                }
                            }
                            else {
                                Estado = 200;
                                ERRC = new Error(G.ERROR.Count, Char.ToString(Linea[Col + 1]), Fila, Columna, ":");
                            }
                        }
                        else if (Caracter == '{')
                        {
                            Token += Caracter;
                            CorchetesExpReg++;
                            Estado = 25;
                        }
                        else if (Caracter == ';')
                        {
                            Estado = 1;
                            Token Z1 = new Token(G.TOKEN.Count, Token);
                            Console.WriteLine(Z1.toString());
                            G.TOKEN.Push(Z1);
                            if (CorchetesExpReg != 0)
                            {
                                Estado = 200;
                                ERRC = new Error(G.ERROR.Count, Token, Fila, Columna, "FALTAN CORCHETES DE CIERRE");
                            }
                            else if (ComillasExpReg != 0)
                            {
                                Estado = 200;
                                ERRC = new Error(G.ERROR.Count, Token, Fila, Columna, "FALTAN COMILLAS DE CIERRE");
                            }
                            Token = "";
                            Z1 = new Token(G.TOKEN.Count, ";");
                            Console.WriteLine(Z1.toString());
                            G.TOKEN.Push(Z1);
                        }
                        else if (Caracter == ' ' || Caracter == '\t' || Caracter == '\n')
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, "ESPACIO, TAB o ENTER", Fila, Columna, "SIN ESPACIOS FUERA DE COMILLAS");
                        }
                        else
                        {
                            Token += Caracter;
                        }
                        break;
                    case 25:
                        if (Caracter == '\"')
                        {
                            Token += Caracter;
                            ComillasExpReg--;
                            ESPACIOS = false;
                            Estado = 24;
                        }
                        else if (Caracter == '}')
                        {
                            Token += Caracter;
                            CorchetesExpReg--;
                            Estado = 24;
                        }
                        else if (Caracter == '{')
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, "{", Fila, Columna, "NO ABRIR CORCHETE ANTES DE CERRARLO");
                        }
                        else if (Caracter == ';')
                        {
                            Estado = 1;
                            Token Z1 = new Token(G.TOKEN.Count, Token);
                            Console.WriteLine(Z1.toString());
                            G.TOKEN.Push(Z1);
                            if (CorchetesExpReg != 0)
                            {
                                Estado = 200;
                                ERRC = new Error(G.ERROR.Count, Token, Fila, Columna, "FALTAN CORCHETES DE CIERRE");
                            }
                            else if (ComillasExpReg != 0)
                            {
                                Estado = 200;
                                ERRC = new Error(G.ERROR.Count, Token, Fila, Columna, "FALTAN COMILLAS DE CIERRE");
                            }
                            Token = "";
                            Z1 = new Token(G.TOKEN.Count, ";");
                            Console.WriteLine(Z1.toString());
                            G.TOKEN.Push(Z1);
                        }
                        else if (Caracter == ' ' && ESPACIOS == true)
                        {
                            Token += Caracter;
                        }
                        else if ((Caracter == ' ' && ESPACIOS == false) || Caracter == '\t' || Caracter == '\n')
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, "ESPACIO, TAB o ENTER", Fila, Columna, "SIN ESPACIOS FUERA DE COMILLAS");
                        }
                        else
                        {
                            Token += Caracter;
                        }
                        break;
                    case 26:
                        if (Caracter == '%')
                        {
                            Token += Caracter;
                            Token Z1 = new Token(G.TOKEN.Count, Token);
                            Console.WriteLine(Z1.toString());
                            G.TOKEN.Push(Z1);
                            Token = "";
                            Estado = 27;
                        }
                        else
                        {
                            Token += Caracter;
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, Token, Fila, Columna, "%%");
                        }
                        break;
                    case 27:
                        if (Caracter == ' ' || Caracter == '\t') { }
                        else if (Caracter == '\n') { Estado = 28; }
                        else
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, Char.ToString(Caracter), Fila, Columna, "%%");
                        }
                        break;
                    case 28:
                        if (Caracter == '%')
                        {
                            Token = "%";
                            Estado = 29;
                        }
                        else if (Caracter == ' ' || Caracter == '\t' || Caracter == '\n') { }
                        else
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, Char.ToString(Caracter), Fila, Columna, "%%");
                        }
                        break;
                    case 29:
                        if (Caracter == '%')
                        {
                            Token += Caracter;
                            Token Z1 = new Token(G.TOKEN.Count, Token);
                            Console.WriteLine(Z1.toString());
                            G.TOKEN.Push(Z1);
                            Token = "";
                            Estado = 30;
                            ValuarExpReg = true;
                        }
                        else
                        {
                            Token += Caracter;
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, Token, Fila, Columna, "%%");
                        }
                        break;
                    case 30:
                        if (Caracter == ' ' || Caracter == '\t' || Caracter == '\n') { }
                        else if (Caracter == '/')
                        {
                            Estado = 4;
                            Token += Caracter;
                        }
                        else if (Caracter == '<')
                        {
                            Estado = 9;
                            Token = "<";
                        }
                        else if (Caracter == '}')
                        {
                            Token Z1 = new Token(G.TOKEN.Count, "}");
                            Console.WriteLine(Z1.toString());
                            G.TOKEN.Push(Z1);
                            Terminar = true;
                        }
                        else
                        {
                            Token = Char.ToString(Caracter);
                            Estado = 31;
                        }
                        break;
                    case 31:
                        if (Caracter == ' ' || Caracter == '-' || Caracter == '\t')
                        {
                            Token Z1 = new Token(G.TOKEN.Count, Token);
                            Console.WriteLine(Z1.toString());
                            G.TOKEN.Push(Z1);
                            if (Caracter == '-')
                            {
                                Token = "-";
                                Estado = 33;
                            }
                            else
                            {
                                Token = "";
                                Estado = 32;
                            }
                        }
                        else if (Caracter == '\n')
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, "ENTER", Fila, Columna, "SIN SALTO DE LINEA");
                        }
                        else
                        {
                            Token += Caracter;
                        }
                        break;
                    case 32:
                        if (Caracter == ' ' || Caracter == '\t') { }
                        else if (Caracter == '\n')
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, "ENTER", Fila, Columna, "SIN SALTO DE LINEA");
                        }
                        else if (Caracter == '-')
                        {
                            Token = "-";
                            Estado = 33;
                        }
                        break;
                    case 33:
                        if (Caracter == '>')
                        {
                            Token += Caracter;
                            Token Z1 = new Token(G.TOKEN.Count, Token);
                            Console.WriteLine(Z1.toString());
                            G.TOKEN.Push(Z1);
                            Token = "";
                            Estado = 34;
                        }
                        else
                        {
                            Token += Caracter;
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, Token, Fila, Columna, "->");
                        }
                        break;
                    case 34:
                        if (Caracter == ' ' || Caracter == '\t') { }
                        else if (Caracter == '\"')
                        {
                            Token = Char.ToString(Caracter);
                            Token Z1 = new Token(G.TOKEN.Count, Token);
                            Console.WriteLine(Z1.toString());
                            G.TOKEN.Push(Z1);
                            Token = "";
                            Estado = 35;
                        }
                        else
                        {
                            Token += Caracter;
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, Token, Fila, Columna, "\"");
                        }
                        break;
                    case 35:
                        for (int q = Col; q < Linea.Length; q++)
                        {
                            if (Linea[q] == '\"')
                            {
                                Token Z1 = new Token(G.TOKEN.Count, Token);
                                Console.WriteLine(Z1.toString());
                                G.TOKEN.Push(Z1);
                                Token = "";
                                Z1 = new Token(G.TOKEN.Count, "\"");
                                Console.WriteLine(Z1.toString());
                                G.TOKEN.Push(Z1);
                                Estado = 36;
                                Col = q;
                                break;
                            }
                            else
                            {
                                Token += Linea[q];
                            }
                        }
                        break;
                    case 36:
                        if (Caracter == ' ' || Caracter == '\t') { }
                        else if (Caracter == '\n')
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, "Enter", Fila, Columna, "SIN SALTO DE LINEA");
                        }
                        else if (Caracter == ';')
                        {
                            Token Z1 = new Token(G.TOKEN.Count, ";");
                            Console.WriteLine(Z1.toString());
                            G.TOKEN.Push(Z1);
                            Estado = 1;
                        }
                        else
                        {
                            Estado = 200;
                            ERRC = new Error(G.ERROR.Count, Char.ToString(Caracter), Fila, Columna, ";");
                        }
                        break;
                    //error automata
                    case 200:
                        G.ERROR.Push(ERRC);
                        Console.WriteLine(ERRC.toString());
                        Estado = 1;
                        break;
                }
            }
            METODOS M = new METODOS();
            M.CargarTablaToken(DTG1,G.TOKEN);
            M.CargarTablaErrores(DTG2, G.ERROR);
            if (G.ERROR.Count==0) {
                G.contadorArbol = 0;
                M.BuscarExpReg(G.TOKEN);
            }
        }
    }
}
