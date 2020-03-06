using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1.TDA
{
    class Error
    {
        private int ID;
        private String Lexema;
        private int fila;
        private int Columna;
        private String Esperado;

        public Error(int ID, String Lexema, int fila, int Columna, String Esperado)
        {
            this.ID = ID;
            this.Lexema = Lexema;
            this.fila = fila;
            this.Columna = Columna;
            this.Esperado = Esperado;
        }
        public Error()
        {
        }
        public int getID(){return ID;}
        public void setID(int ID){this.ID = ID;}
        public String getLexema(){return Lexema;}
        public void setLexema(String Lexema){this.Lexema = Lexema;}
        public int getFila(){return fila;}
        public void setFila(int fila){this.fila = fila;}
        public int getColumna(){return Columna;}
        public void setColumna(int Columna){this.Columna = Columna;}
        public String getEsperado()
        {return Esperado;}
        public void setEsperado(String Esperado){this.Esperado = Esperado;}
        public String toString(){return this.ID + ":" + this.Lexema + ":" + this.Esperado + ":" + this.fila + ":" + this.Columna;}
    }
}
