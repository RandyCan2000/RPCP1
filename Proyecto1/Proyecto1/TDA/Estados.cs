using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1.TDA
{
    class Estados
    {

        String Estado;
        String Conjunto;

        public Estados(string estado, string conjunto)
        {
            Estado = estado;
            Conjunto = conjunto;
        }

        public string Estado1 { get => Estado; set => Estado = value; }
        public string Conjunto1 { get => Conjunto; set => Conjunto = value; }
    }
}
