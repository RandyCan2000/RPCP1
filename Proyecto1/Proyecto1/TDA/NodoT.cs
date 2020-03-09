using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1.TDA
{
    class NodoT
    {
        private int NumNodo;
        private NodoT NodoArriba;
        private string ValNodoArriba;
        private NodoT NodoAbajo;
        private string ValNodoAbajo;

        public NodoT()
        {
            NumNodo=0;
            NodoArriba = null;
            ValNodoArriba = null;
            NodoAbajo = null;
            ValNodoAbajo = null;
        }

        public int NumNodo1 { get => NumNodo; set => NumNodo = value; }
        public string ValNodoArriba1 { get => ValNodoArriba; set => ValNodoArriba = value; }
        public string ValNodoAbajo1 { get => ValNodoAbajo; set => ValNodoAbajo = value; }
        internal NodoT NodoArriba1 { get => NodoArriba; set => NodoArriba = value; }
        internal NodoT NodoAbajo1 { get => NodoAbajo; set => NodoAbajo = value; }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
