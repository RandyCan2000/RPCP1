﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1.TDA
{
    //TDA DE TRANSICIONES EPSILON
    class TE
    {
        int Estado;
        string ConjuntoEps;

        public TE(){
            Estado = 0;
            ConjuntoEps = "";
        }

        public int Estado1 { get => Estado; set => Estado = value; }
        public string ConjuntoEps1 { get => ConjuntoEps; set => ConjuntoEps = value; }

    }
}
