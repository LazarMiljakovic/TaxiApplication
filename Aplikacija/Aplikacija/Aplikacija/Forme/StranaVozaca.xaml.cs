using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using PolylinerNet;
using System.Net.Http;
using Newtonsoft.Json;
using Aplikacija.Forme;
using Rg.Plugins.Popup.Extensions;
using Aplikacija.Helperi;
using Aplikacija.Modeli;
using System.Collections.ObjectModel;
using Plugin.LocalNotification;

namespace Aplikacija
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StranaVozaca : ContentPage
    {
        double LA, LO;
        double LAP, LOP;
        int cena;
        List<PolylinePoint> listaKord = new List<PolylinePoint>(); 
        public Vozac u = new Vozac();
        public Voznja v = new Voznja();
        Polyline linija = new Polyline();
        Pin current = new Pin();
        Pin pocetna = new Pin();
        Pin krajnja = new Pin();

        public StranaVozaca()
        {
            
            InitializeComponent();
        }
        public StranaVozaca(Vozac u)
        {
            this.u = u;
            InitializeComponent();
            this.current = new Pin()
            {
                Label = "Trenutna Lokacija",
                Type = PinType.Place
            };
            map.Pins.Add(current);
            this.pocetna = new Pin()
            {
                Label = "Pocetna Lokacija",
                Type = PinType.Place
            };
            
            this.krajnja = new Pin()
            {
                Label = "Krajnja Lokacija",
                Type = PinType.Place
            };

        }

        private async void CrtajRutu(Voznja v)
        {
            Geocoder geo = new Geocoder();
            map.Polylines.Clear();
            IEnumerable<Position> lok1 = await geo.GetPositionsForAddressAsync(v.Pocetak);
            Position voznj1 = lok1.FirstOrDefault();
            this.pocetna.Position = new Position(voznj1.Latitude, voznj1.Longitude);
            IEnumerable<Position> lok = await geo.GetPositionsForAddressAsync(v.Kraj);
            Position voznj = lok.FirstOrDefault();
            this.krajnja.Position = new Position(voznj.Latitude, voznj.Longitude);


            string laa1 = voznj1.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string lao1 = voznj1.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string laa2 = voznj.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string lao2 = voznj.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);

          
            string ruta = "https://maps.googleapis.com/maps/api/directions/json?origin={" + laa1 + "," + lao1 + "}&destination={" + laa2 + "," + lao2 + "}&key=AIzaSyCv9WtYi_ABfp21tSFCvqbAV3vSoGJV73s";
            var hendeler = new HttpClientHandler();
            HttpClient klijent = new HttpClient(hendeler);
            string rutakodirana = await klijent.GetStringAsync(ruta);

            Polyliner polyliner = new Polyliner();
            var direkcija = JsonConvert.DeserializeObject<Acxi.Helpers.DirectionParser>(rutakodirana);
            
            var tacke = direkcija.routes[0].overview_polyline.points;
            


            listaKord = polyliner.Decode(tacke);
            Polyline linija = new Polyline()
            {
                StrokeWidth = 4,
                StrokeColor = Color.Red,


            };
            double distanca = 0;
            Location poc = new Location(listaKord[0].Latitude, listaKord[0].Longitude);
            Location kr = new Location();
            foreach (PolylinePoint p in listaKord)
            {
                Position a = new Position(p.Latitude, p.Longitude);
                linija.Positions.Add(a);
                kr.Latitude = a.Latitude;
                kr.Longitude = a.Longitude;
                distanca += Location.CalculateDistance(poc, kr, DistanceUnits.Kilometers);
                poc.Latitude = a.Latitude;
                poc.Longitude = a.Longitude;
            }
            map.Polylines.Add(linija);
        }



        private async void CrtajRutuDoKlijenta(Voznja v)
        {
            Geocoder geo = new Geocoder();
            
            IEnumerable<Position> lok = await geo.GetPositionsForAddressAsync(v.Pocetak);
            Position voznj = lok.FirstOrDefault();
            this.pocetna.Position = new Position(voznj.Latitude, voznj.Longitude);

            string laa1 = LA.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string lao1 = LO.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string laa2 = voznj.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string lao2 = voznj.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);

            string ruta = "https://maps.googleapis.com/maps/api/directions/json?origin={" + laa1 + "," + lao1 + "}&destination={" + laa2 + "," + lao2 + "}&key=AIzaSyCv9WtYi_ABfp21tSFCvqbAV3vSoGJV73s";
            var hendeler = new HttpClientHandler();
            HttpClient klijent = new HttpClient(hendeler);
            string rutakodirana = await klijent.GetStringAsync(ruta);

            Polyliner polyliner = new Polyliner();
            var direkcija = JsonConvert.DeserializeObject<Acxi.Helpers.DirectionParser>(rutakodirana);
            var tacke = direkcija.routes[0].overview_polyline.points;


            listaKord = polyliner.Decode(tacke);
            linija.StrokeColor = Color.Red;
            linija.StrokeWidth = 4;
            
            foreach (PolylinePoint p in listaKord)
            {
                Position a = new Position(p.Latitude, p.Longitude);
                linija.Positions.Add(a);
                
            }
            stig.IsVisible = true;
            

        }


        private void gl_Clicked(object sender, EventArgs e)
        {
            map.IsVisible = true;
            zarad.IsVisible = false;
            zaradlist.IsVisible = false;
            DataSource.Clear();
        }
        public ObservableCollection<Voznja> DataSource { get; set; }
        private async void zar_Clicked(object sender, EventArgs e)
        {
            map.IsVisible = false;
            zarad.IsVisible = true;
            zaradlist.IsVisible = true;
            FireBaseHelper f = new FireBaseHelper();
            List<Voznja> listavoznje = await f.VratiSveVoznjeVozaca(u);
            DataSource = new ObservableCollection<Voznja>();
            zaradlist.ItemsSource = DataSource;
            zaradlist.RowHeight = 60;
            foreach(Voznja a in listavoznje)
            {
                DataSource.Add(new Voznja() { cena = a.cena, Pocetak = a.Pocetak });
            }


        }
        EventiHelper eveno = new EventiHelper();
        private async void stig_Clicked(object sender, EventArgs e)
        {
            FireBaseHelper f = new FireBaseHelper();
            await f.PrihvatiVoznju(v, "Stigao");
            stig.IsVisible = false;
       
            pocni.IsVisible = true;
            otkazi.IsVisible = true;

        }

        private void Eveno_IstekloVUG(object sender, EventArgs e)
        {
            eveno.StatusStigaoOtkazana(u);
        }

        private async void Eveno_VoznjaUgasena(object sender, EventArgs e)
        {
            lokac.IsVisible = false;

            await DisplayAlert("Upozorenje", "Voznja je otkazana", "OK");
            even.VoznjaNadjena -= Even_VoznjaNadjena;
            even.Isteklo -= Even_Isteklo;
            FireBaseHelper f = new FireBaseHelper();
            stig.IsVisible = false;
            otkazi.IsVisible = false;
            await f.OfflineVozac(u);
            map.Polylines.Clear();
            map.Pins.Remove(pocetna);
            map.Pins.Remove(krajnja);
            upal.IsToggled = false;
            upal.ThumbColor = Color.Red;
            eveno.VoznjaUgasena -= Eveno_VoznjaUgasena;
            eveno.IstekloVUG -= Eveno_IstekloVUG;

        }

        private async void pocni_Clicked(object sender, EventArgs e)
        {
            LAP = LA;
            LOP = LO;
            CrtajRutu(v);
            
            lokac.Text = v.Kraj;
            FireBaseHelper f = new FireBaseHelper();
            await f.PrihvatiVoznju(v, "Poceta");
            pocni.IsVisible = false;
            zavrsi.IsVisible = true;
            otkazi.IsVisible = false;
        }

        private async Task<int> racunajCenuAsync()
        {
            Geocoder geo = new Geocoder();

            
            string laa1 = LAP.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string lao1 = LOP.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string laa2 = LA.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string lao2 = LO.ToString(System.Globalization.CultureInfo.InvariantCulture);

            string ruta = "https://maps.googleapis.com/maps/api/directions/json?origin={" + laa1 + "," + lao1 + "}&destination={" + laa2 + "," + lao2 + "}&key=AIzaSyCv9WtYi_ABfp21tSFCvqbAV3vSoGJV73s";
            var hendeler = new HttpClientHandler();
            HttpClient klijent = new HttpClient(hendeler);
            string rutakodirana = await klijent.GetStringAsync(ruta);

            Polyliner polyliner = new Polyliner();
            var direkcija = JsonConvert.DeserializeObject<Acxi.Helpers.DirectionParser>(rutakodirana);
            var tacke = direkcija.routes[0].overview_polyline.points;
            


            listaKord = polyliner.Decode(tacke);
            
            double distanca = 0;
            Location poc = new Location(listaKord[0].Latitude, listaKord[0].Longitude);
            Location kr = new Location();
            foreach (PolylinePoint p in listaKord)
            {
                Position a = new Position(p.Latitude, p.Longitude);
                kr.Latitude = a.Latitude;
                kr.Longitude = a.Longitude;
                distanca += Location.CalculateDistance(poc, kr, DistanceUnits.Kilometers);
                poc.Latitude = a.Latitude;
                poc.Longitude = a.Longitude;
            }



            double voznja = ((distanca * 50) + 130);
            int q = Convert.ToInt32(voznja);
            this.cena = q;
            
            
            await DisplayAlert("Kraj voznje", "Cena: " + cena, "OK");
            return this.cena;
        }

        private async void zavrsi_Clicked(object sender, EventArgs e)
        {
            
            lokac.IsVisible = false;
            zavrsi.IsVisible = false;
            FireBaseHelper f = new FireBaseHelper();
            var cen =await this.racunajCenuAsync();
            v.cena = cen;
            v.idVozaca = u.id;
            v.Prahivacena = "Zavrsena";
            await f.UbaciVoznjuFin(v);
            FireBaseHelper c = new FireBaseHelper();
            

            await c.OfflineVozac(u);
            
            map.Polylines.Clear();
            map.Pins.Remove(pocetna);
            map.Pins.Remove(krajnja);
            upal.IsToggled = false;
            upal.ThumbColor = Color.Red;
            even.VoznjaNadjena -= Even_VoznjaNadjena;
            even.Isteklo -= Even_Isteklo;



        }

        private async void online_Clicked(object sender, EventArgs e)
        {
            if (upal.IsToggled != true)
            {
                upal.IsToggled = true;
                upal.ThumbColor = Color.Green;
                var current = Connectivity.NetworkAccess;

                if (current == NetworkAccess.Internet)
                {
                    var a = await Geolocation.GetLocationAsync();
                    LA = a.Latitude;
                    LO = a.Longitude;

                    this.current.Position = new Position(LA, LO);
                    map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(LA, LO), Distance.FromKilometers(2)));
                    u.laTrenutna = LA;
                    u.loTrenutna = LO;
                    u.Spreman = "Spreman";
                    FireBaseHelper f = new FireBaseHelper();
                    await f.spremniVozac(u);
                    GoOnline();
                }
                else
                {
                    upal.IsToggled = false;
                    upal.ThumbColor = Color.Red;
                    await DisplayAlert("Upozorenje", "Niste konektovani na internet", "Potvrdi");
                }
            }
            else
            {
                var current = Connectivity.NetworkAccess;

                if (current == NetworkAccess.Internet)
                {
                    FireBaseHelper f = new FireBaseHelper();
                    await f.OfflineVozac(u);
                    upal.IsToggled = false;
                    upal.ThumbColor = Color.Red;
                }
                else
                {
                    upal.IsToggled = false;
                    upal.ThumbColor = Color.Red;
                    await DisplayAlert("Upozorenje", "Niste konektovani na internet", "Potvrdi");
                }
            }
        }
        EventiHelper even = new EventiHelper();
        EventiHelper even2 = new EventiHelper();

        private void GoOnline()
        {
            
            even2.OsveziLok(LA, LO);
            even2.Promenjena += Even_Promenjena;
            even2.IstekloPromenjana += Even2_IstekloPromenjana;

           

            even.StatusStigao(u);
            even.VoznjaNadjena += Even_VoznjaNadjena;
            even.Isteklo += Even_Isteklo;
            
            

        }

        private void Even2_IstekloPromenjana(object sender, EventArgs e)
        {
            even2.OsveziLok(LA,LO);
        }

        private void Even_Isteklo(object sender, EventArgs e)
        {
            even.StatusStigao(u);
        }

        private void Even_Promenjena(object sender, EventiHelper.LokLALO e)
        {
            LA = e.la;
            LO = e.lo;
            this.current.Position = new Position(LA, LO);            
            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(LA, LO), Distance.FromKilometers(4)));
            
        }

        private async void otkazi_Clicked(object sender, EventArgs e)
        {
            lokac.IsVisible = false;
            stig.IsVisible = false;
            otkazi.IsVisible = false;
            pocni.IsVisible = false;
            FireBaseHelper f = new FireBaseHelper();
            await f.PrihvatiVoznju(v, "Otkazana");
            FireBaseHelper c = new FireBaseHelper();

            await c.OfflineVozac(u);
            map.Polylines.Clear();
            map.Pins.Remove(pocetna);
            map.Pins.Remove(krajnja);
            
            upal.IsToggled = false;
            upal.ThumbColor = Color.Red;
            even.VoznjaNadjena -= Even_VoznjaNadjena;
            even.Isteklo -= Even_Isteklo;
        }

        private async void Even_VoznjaNadjena(object sender, EventiHelper.VoznjaEvnetArgs e)
        {
            v = e.voznja;
            map.Pins.Add(pocetna);
            map.Pins.Add(krajnja);
            bool odg = await DisplayAlert("Nova voznja", "Od:" + v.Pocetak + "  Do:" + v.Kraj + "  Cena:" + v.cena + " Napomena:" + v.Napomena, "Prihvati", "Odbij");
            if (odg == true)
            {
                lokac.IsVisible = true;
                lokac.Text = v.Pocetak;
                FireBaseHelper f = new FireBaseHelper();
                FireBaseHelper d = new FireBaseHelper();

                await f.PrihvatiVoznju(v, "DA");
                even.VoznjaNadjena -= Even_VoznjaNadjena;
                even.Isteklo -= Even_Isteklo;

                CrtajRutuDoKlijenta(v);
                await d.DodajVozacaVoznji(v, u.id);
                eveno.StatusStigaoOtkazana(u);
                eveno.VoznjaUgasena += Eveno_VoznjaUgasena;
                eveno.IstekloVUG += Eveno_IstekloVUG;


            }
            else
            {

                
                FireBaseHelper s = new FireBaseHelper();
                await s.UpdateStatus(u.id, "Spreman");
            }
        }

        



    }
}