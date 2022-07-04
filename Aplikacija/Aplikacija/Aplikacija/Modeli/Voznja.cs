using System;
using System.Collections.Generic;
using System.Text;

namespace Aplikacija.Modeli
{
    public class Voznja
    {
        public string Pocetak { get; set; }
        public string Kraj { get; set; }

        public int cena { get; set; }
        public double kilometraza { get; set; }
        public DateTime vreme { get; set; }

        public int BrojTelefonaVozaca { get; set; }

        public string ImeiPrezime { get; set; }
        public string Napomena { get; set; }

        public string Prahivacena { get; set; }
        public int idVozaca { get; set; }

        public Voznja()
        {

        }
    }
}
