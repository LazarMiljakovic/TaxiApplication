using System;
using System.Collections.Generic;
using System.Text;

namespace Aplikacija.Modeli
{
    public class Vozac
    {
        public int id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Registracija { get; set; }
        public int Vozilo { get; set; }
        public string Vrsta { get; set; }
        public string sifra { get; set; }

        public double laTrenutna { get; set; }
        public double loTrenutna { get; set; }
        public string Spreman { get; set; }
        public Vozac()
        {

        }

    }
}
