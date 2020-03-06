using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1.TDA
{
    class Token
    {
        private int ID;
        private String Lexema;

        public Token(int ID, String Lexema)
        {
            this.ID = ID;
            this.Lexema = Lexema;
        }

        public Token(){}
        public int getID(){return ID;}
        public void setID(int ID){this.ID = ID;}
        public String getLexema(){return Lexema;}
        public void setLexema(String Lexema){this.Lexema = Lexema;}
        public String toString(){return this.ID + " " + this.Lexema;}
    }
}
