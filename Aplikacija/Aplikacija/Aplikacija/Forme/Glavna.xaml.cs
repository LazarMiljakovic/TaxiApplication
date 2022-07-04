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
using System.Threading;

namespace Aplikacija
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Glavna : ContentPage
    {
        int time;
        double LA, LO;
        int cena;
        double kilometraza = 0;
        List<PolylinePoint> listaKord = new List<PolylinePoint>();
        string pocetak;
        string kraj;
        User u = new User();
        string napomena;
        Voznja g = new Voznja();
        public Glavna(User u)
        {
            this.u = u;
            InitializeComponent();
            this.current = new Pin()
            {
                Label = "Trenutna Lokacija",
                Type = PinType.Place
            };


        }

        

        private async void LoadMap(double la, double lo)
        {
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                Pin pin = new Pin()
                {
                    Label = "Lokacija",
                    Position = new Position(la, lo),
                    Type = PinType.Place
                };
                map.Pins.Add(pin);
                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(la, lo), Distance.FromKilometers(4)));
            }
            else
            {
                await DisplayAlert("Upozorenje", "Niste konektovani na internet", "Potvrdi");
            }
            
        }

        private async void SaLokacije(object sender, EventArgs e)
        {

            map.Polylines.Clear();
            string lokacijaPolazna = await DisplayPromptAsync("Polazna lokacija", "","Potvrdi","Trenutna");
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                if (lokacijaPolazna != null)
                {

                    Geocoder geo = new Geocoder();
                    IEnumerable<Position> lok = await geo.GetPositionsForAddressAsync(lokacijaPolazna);
                    Position p = lok.FirstOrDefault();
                    string kordinate = $"{p.Latitude},{p.Longitude}";
                    IEnumerable<string> poc = await geo.GetAddressesForPositionAsync(p);
                    pocetak = poc.FirstOrDefault();
                    map.Pins.Clear();
                    LoadMap(p.Latitude, p.Longitude);
                    saL.Text = pocetak;
                    Location l = new Location(p.Latitude, p.Longitude);
                    NaLokaciju(p, l);
                }
                else
                {


                    Geocoder geo = new Geocoder();
                    Location location = await Geolocation.GetLocationAsync();
                    LA = location.Latitude;
                    LO = location.Longitude;
                    Position c = new Position(LA, LO);
                    map.Pins.Clear();
                    LoadMap(LA, LO);
                    IEnumerable<string> poc = await geo.GetAddressesForPositionAsync(c);
                    pocetak = poc.FirstOrDefault();
                    saL.Text = pocetak;
                    Location l = new Location(LA, LO);
                    NaLokaciju(c, l);
                }
            }
            else
            {
                await DisplayAlert("Upozorenje", "Niste konektovani na internet", "Potvrdi");
            }

            
            
        }

        private async void NaLokaciju(Position a,Location s)
        {
            
            string lokacijaKrajnja = await DisplayPromptAsync("Krajnja lokacija", "");
            napomena = await DisplayPromptAsync("Napomena za vozaca", "", "Potvrdi", "Nastavi bez napomene");

            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                if (lokacijaKrajnja != null)
                {


                    Geocoder geo = new Geocoder();
                    IEnumerable<Position> lok = await geo.GetPositionsForAddressAsync(lokacijaKrajnja);
                    Position p = lok.FirstOrDefault();
                    string kordinate = $"{p.Latitude},{p.Longitude}";
                    IEnumerable<string> kr = await geo.GetAddressesForPositionAsync(p);
                    kraj = kr.FirstOrDefault();
                    LoadMap(p.Latitude, p.Longitude);
                    naL.Text = kraj;
                    CrtajRutu(a.Latitude, a.Longitude, p.Latitude, p.Longitude);





                }
            }
            else
            {
                await DisplayAlert("Upozorenje", "Niste konektovani na internet", "Potvrdi");
            }
            
        }

        private async void CrtajRutu(double la1, double lo1, double la2, double lo2)
        {
            string laa1 = la1.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string lao1 = lo1.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string laa2 = la2.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string lao2 = lo2.ToString(System.Globalization.CultureInfo.InvariantCulture);

            string ruta = "https://maps.googleapis.com/maps/api/directions/json?origin={"+laa1+","+lao1+ "}&destination={" + laa2 + "," + lao2 + "}&key=AIzaSyCv9WtYi_ABfp21tSFCvqbAV3vSoGJV73s";
            var hendeler = new HttpClientHandler();
            HttpClient klijent = new HttpClient(hendeler);
            string rutakodirana = await klijent.GetStringAsync(ruta);

            Polyliner polyliner = new Polyliner();
            var direkcija = JsonConvert.DeserializeObject<Acxi.Helpers.DirectionParser>(rutakodirana);
            var vreme = JsonConvert.DeserializeObject<Acxi.Helpers.Duration>(rutakodirana);
            var tacke = direkcija.routes[0].overview_polyline.points;
            time = vreme.value;
            

            listaKord= polyliner.Decode(tacke);
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
            
            
            
            double voznja = ((distanca * 50) + 130);
            int v = Convert.ToInt32(voznja);
            this.cena = v;
            this.kilometraza = distanca;
            Cena.Text = v.ToString() + " din";
            map.Polylines.Add(linija);
        }

        private void Pronadji_Clicked(object sender, EventArgs e)
        {
            Narucivanje();
        }
        EventiHelper even = new EventiHelper();
        private async void Narucivanje()
        {
            try
            {
                var current = Connectivity.NetworkAccess;

                if (current == NetworkAccess.Internet)
                {
                    if (kilometraza != 0)
                    {
                        Voznja v = new Voznja();
                        v.Pocetak = pocetak;
                        v.Kraj = kraj;
                        v.cena = cena;
                        v.kilometraza = kilometraza;
                        v.vreme = DateTime.Now;
                        v.BrojTelefonaVozaca = u.BrojTelefona;
                        v.ImeiPrezime = u.Ime + " " + u.Prezime;
                        v.Napomena = napomena;
                        if(napomena != null)
                        {
                            v.Napomena = napomena;
                        }
                        else
                        {
                            v.Napomena = "Nema napomene";
                        }
                        FireBaseHelper f = new FireBaseHelper();
                        await f.PozoviVoznju(v);
                        this.g = v;
                        await Navigation.PushPopupAsync(new PopupNarucivanje(v), true);
                        

                        even.PocetaVoznja(v);
                        even.Pocela += Even_Pocela;
                        even.Otkazanaa += Even_Otkazanaa;
                        even.IstekloPocela += Even_IstekloPocela;
                    }
                    else
                    {
                        await DisplayAlert("Upozorenje", "Niste popunili kuda zelite ici", "Potvrdi");
                    }
                }
                else
                {
                    await DisplayAlert("Upozorenje", "Niste konektovani na internet", "Potvrdi");
                }
                
            }
            catch (Exception es)
            {

            }
            
        }

        private void Even_Otkazanaa(object sender, EventArgs e)
        {
            saL.Text = "Sa Lokacije";
            naL.Text = "Na lokaciju";
            Cena.Text = "Cena";

            map.Polylines.Clear();
            map.Pins.Clear();
            even.Pocela -= Even_Pocela;
            even.Otkazanaa -= Even_Otkazanaa;
            even.IstekloPocela -= Even_IstekloPocela;

        }

        private void Even_IstekloPocela(object sender, EventArgs e)
        {
            
            even.PocetaVoznja(this.g);
        }
        EventiHelper even2 = new EventiHelper();
        EventiHelper even3 = new EventiHelper();
        Pin current = new Pin();

        private void Even_Pocela(object sender, EventArgs e)
        {
            even3.OsveziLok(LA,LO);
            even3.Promenjena += Even3_Promenjena;
            even3.IstekloPromenjana += Even3_IstekloPromenjana;

            even2.ZavrsenaVoznja(g);
            even2.Zavrsena += Even_Zavrsena;
            even2.IstekloZavrsila += Even2_IstekloZavrsila;
        }

        private void Even3_IstekloPromenjana(object sender, EventArgs e)
        {

            even2.OsveziLok(LA, LO);
        }

        private void Even3_Promenjena(object sender, EventiHelper.LokLALO e)
        {
            LA = e.la;
            LO = e.lo;
            this.current.Position = new Position(LA, LO);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(LA, LO), Distance.FromKilometers(4)));


        }

        private void Even2_IstekloZavrsila(object sender, EventArgs e)
        {
            even2.ZavrsenaVoznja(this.g);
        }

        private void Even_Zavrsena(object sender, EventArgs e)
        {
            saL.Text = "Sa Lokacije";
            naL.Text = "Na lokaciju";
            Cena.Text = "Cena";

            map.Polylines.Clear();
            map.Pins.Clear();
            this.Kraj();
        }

        private async void Kraj()
        {
            even.Pocela -= Even_Pocela;
            even.IstekloPocela -= Even_IstekloPocela;
            even2.Zavrsena -= Even_Zavrsena;
            even2.IstekloZavrsila -= Even2_IstekloZavrsila;
            
            FireBaseHelper f = new FireBaseHelper();
            g = await  f.VratiVoznjuKlijentu(g);
            await Navigation.PushPopupAsync(new Recenzije(g), true);

        }
    }
}