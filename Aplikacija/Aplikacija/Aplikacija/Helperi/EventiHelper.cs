using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Aplikacija.Helperi;
using Aplikacija.Modeli;
using Newtonsoft.Json;
using PolylinerNet;
using Xamarin.Essentials;
using Xamarin.Forms.GoogleMaps;

namespace Aplikacija.Helperi
{
    public class EventiHelper
    {
        public event EventHandler VoznjaOtkazana;
        public event EventHandler Stigao;
        public event EventHandler<int> PrihvacenaVoznja;
        public event EventHandler DrugiVozac;
        public event EventHandler Pocela;
        public event EventHandler Zavrsena;
        public event EventHandler Isteklo;
        public event EventHandler IstekloStigao;
        public event EventHandler IstekloPocela;
        public event EventHandler IstekloZavrsila;
        public event EventHandler IstekloPromenjana;
        public event EventHandler VoznjaUgasena;
        public event EventHandler IstekloVUG;
        public event EventHandler NemaVozaca;
        public event EventHandler Otkazanaa;
        public event EventHandler<VozacEvnetArgs> NoviVozac;
        public event EventHandler<VoznjaEvnetArgs> VoznjaNadjena;
        public event EventHandler<LokLALO> Promenjena;
        public int vreme;
        public List<Vozac> lista = new List<Vozac>();


        public class LokLALO : EventArgs
        {
            public double la { get; set; }
            public double lo { get; set; }

        }
        public class VoznjaEvnetArgs : EventArgs
        {
            public Voznja voznja { get; set; }
        }
        public class VozacEvnetArgs : EventArgs
        {
            public Vozac vozac { get; set; }
        }

        string status;
        public async void StatusStigao(Vozac u)
        {
            FireBaseHelper f = new FireBaseHelper();
            status = await f.VratiStatus(u.id);
            if (status != "Spreman" && status != "Otkazana" && status != "Ugasena" && status != null)
            {
                FireBaseHelper d = new FireBaseHelper();
                Voznja voznjaE = new Voznja();
                voznjaE = await d.VratiVoznjuVozacu(status);
                if (voznjaE.ImeiPrezime != null)
                {
                    VoznjaNadjena?.Invoke(this, new VoznjaEvnetArgs { voznja = voznjaE });
                }
            }
            else if (status == "Ugasena")
            {
                VoznjaOtkazana?.Invoke(this, new EventArgs());
            }
            else if( status =="Spreman")
            {
                Isteklo?.Invoke(this, new EventArgs());
            }
        }
        public async void StatusStigaoOtkazana(Vozac u)
        {
            FireBaseHelper f = new FireBaseHelper();
            status = await f.VratiStatus(u.id);
            if (status == "Ugasena")
            {
                VoznjaUgasena?.Invoke(this, new EventArgs());
            }
            else
            {
                IstekloVUG?.Invoke(this, new EventArgs());
            }
        }

        public async void OsveziLok(double la,double lo)
        {
            FireBaseHelper f = new FireBaseHelper();
            var lokacija = await Geolocation.GetLocationAsync();
            if (lokacija != null)
            {
                if(lokacija.Latitude != la || lokacija.Longitude != lo)
                {
                    Promenjena?.Invoke(this, new LokLALO { la = lokacija.Latitude,lo = lokacija.Longitude});
                }
                else
                {
                    IstekloPromenjana?.Invoke(this, new EventArgs());
                }
            }

        }

        public async void NadjenVozac(Voznja v)
        {
            FireBaseHelper f = new FireBaseHelper();
            var z = await f.StatusPrihvacenje(v);
            var a = await f.VratiVoznjuKlijentu(v);
            if(z == "DA"|| a.idVozaca != 0)
            {
                PrihvacenaVoznja?.Invoke(this, vreme);
            }
            else if(z=="Otkazana")
            {
                DrugiVozac?.Invoke(this, new EventArgs());
            }
            else
            {
                Isteklo?.Invoke(this, new EventArgs());
            }
        }

        public async void TraziVozaca(Voznja v,List<Vozac> lista)
        {
            try
            {
                this.lista = lista;
                FireBaseHelper f = new FireBaseHelper();
                

                var Blizvozac = await this.RacunajNajblizegAsync(this.lista, v);
                if (Blizvozac != null)
                {
                    var idv = await f.VratiVoznju(v);
                    FireBaseHelper d = new FireBaseHelper();
                    await d.UpdateVoznju(Blizvozac.id, idv);
                    if (idv != null)
                    {
                        NoviVozac?.Invoke(this, new VozacEvnetArgs { vozac = Blizvozac });
                    }

                }
                else
                {
                    NemaVozaca?.Invoke(this, new VozacEvnetArgs { vozac = Blizvozac });
                }
            }
            catch (Exception ec)
            {
                Debug.WriteLine(ec.Message);
            }
            
        }
        public void IzbaciSaListe(Vozac v)
        {
            lista.Remove(v);
        }

        

        public async Task<Vozac> RacunajNajblizegAsync(List<Vozac> lista,Voznja v)
        {
            double minKm = 50.00;
            Vozac spr = new Vozac();
            Geocoder geo = new Geocoder();
            
            IEnumerable<Position> lok = await geo.GetPositionsForAddressAsync(v.Pocetak);
            Position voznj = lok.FirstOrDefault();
            string laa1 = voznj.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string lao1 = voznj.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            foreach (Vozac vozac in lista)
            {
                string laa2 = vozac.laTrenutna.ToString(System.Globalization.CultureInfo.InvariantCulture);
                string lao2 = vozac.loTrenutna.ToString(System.Globalization.CultureInfo.InvariantCulture);
                string ruta = "https://maps.googleapis.com/maps/api/directions/json?origin={" + laa1 + "," + lao1 + "}&destination={" + laa2 + "," + lao2 + "}&key=AIzaSyCv9WtYi_ABfp21tSFCvqbAV3vSoGJV73s";
                var hendeler = new HttpClientHandler();
                HttpClient klijent = new HttpClient(hendeler);
                string rutakodirana = await klijent.GetStringAsync(ruta);
                Polyliner polyliner = new Polyliner();

                var direkcija = JsonConvert.DeserializeObject<Acxi.Helpers.DirectionParser>(rutakodirana);
               
                var tacke = direkcija.routes[0].overview_polyline.points;
                List<PolylinePoint> listaKord = polyliner.Decode(tacke);
                double distanca = 0;
                Location poc = new Location(listaKord[0].Latitude, listaKord[0].Longitude);
                Location kr = new Location();
                foreach (PolylinePoint p in listaKord)
                {
                    kr.Latitude = p.Latitude;
                    kr.Longitude = p.Longitude;
                    distanca += Location.CalculateDistance(poc, kr, DistanceUnits.Kilometers);
                    poc.Latitude = p.Latitude;
                    poc.Longitude = p.Longitude;
                }
                if (minKm > distanca)
                {
                    minKm = distanca;
                    spr = vozac;
                }
                
                double time = (distanca/(35*1000/3600)*10);
                int vrm = Convert.ToInt32(time);
                this.vreme = vrm;
            }
            return spr;
        }
        public int VratiVreme()
        {
            return this.vreme;
        }

        public async void OcekujeSeVoziloE(Voznja v)
        {
            FireBaseHelper f = new FireBaseHelper();
            var s = await f.StatusPrihvacenje(v);
            if (s == "Stigao")
            {
                Stigao?.Invoke(this, new EventArgs());
                

            }
            else
            {
                IstekloStigao?.Invoke(this, new EventArgs());
            }
        }

        public async void PocetaVoznja(Voznja v)
        {
            FireBaseHelper f = new FireBaseHelper();
            var z = await f.StatusPrihvacenje(v);
            
            if (z == "Poceta")
            {
                Pocela?.Invoke(this, new EventArgs());

            }
            else if(z == "Otkazana")
            {
                Otkazanaa?.Invoke(this, new EventArgs());
            }
            else
            {
                IstekloPocela?.Invoke(this, new EventArgs());
            }
        }
        public async void ZavrsenaVoznja(Voznja v)
        {
            FireBaseHelper f = new FireBaseHelper();
            var z = await f.StatusPrihvacenje(v);
            if (z == "Zavrsena")
            {
                Zavrsena?.Invoke(this, new EventArgs());

            }
            else
            {
                IstekloZavrsila?.Invoke(this, new EventArgs());
            }
        }
    }
        
}
