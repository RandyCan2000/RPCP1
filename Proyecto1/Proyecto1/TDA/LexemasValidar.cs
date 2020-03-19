using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1.TDA
{
    class LexemasValidar
    {
        String Lexema;
        String Aceptado;

        public LexemasValidar(string lexema)
        {
            Lexema = lexema;
            Aceptado = "";
        }

        public string Lexema1 { get => Lexema; set => Lexema = value; }
        public string Aceptado1 { get => Aceptado; set => Aceptado = value; }
    }
}
