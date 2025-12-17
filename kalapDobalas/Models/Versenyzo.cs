using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kalapDobalas.Models
{
    internal class Versenyzo
    {
        public int Id { get; set; }
        public string Nev { get; set; }

        // A három kör pontszámai és időeredményei
        public int Pont1 { get; set; }
        public double Ido1 { get; set; }
        public int Pont2 { get; set; }
        public double Ido2 { get; set; }
        public int Pont3 { get; set; }
        public double Ido3 { get; set; }

        // Ezeket számoljuk majd ki a rangsoroláshoz
        public int LegjobbPontszam { get; set; }
        public double LegjobbIdo { get; set; } // A legjobb ponthoz tartozó leggyorsabb idő
    }
}
