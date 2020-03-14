using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1.TDA
{
    class TAFD
    {
        String Estado;
        String[] Opciones;

        public TAFD(int Nalfabeto){
            Estado="";
            Opciones = new String[Nalfabeto];
            for (int i=0;i<Opciones.Length;i++) { Opciones[i] = ""; }
        }

        public string Estado1 { get => Estado; set => Estado = value; }
        public string[] Opciones1 { get => Opciones; set => Opciones = value; }
    }
}
