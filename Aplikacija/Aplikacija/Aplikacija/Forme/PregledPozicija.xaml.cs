using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.GoogleMaps;
using Aplikacija.Helperi;
using Aplikacija.Modeli;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace Aplikacija.Forme
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PregledPozicija : ContentPage
    {
        public PregledPozicija()
        {
            InitializeComponent();
            this.Load();
        }

        private async void Load()
        {
            FireBaseHelper f = new FireBaseHelper();
            List<Vozac> listaVoz = await f.VratiSveSpremneVozace();
            foreach (Vozac vozac in listaVoz)
            {
                LoadMap(vozac.laTrenutna, vozac.loTrenutna, vozac.id, vozac.Registracija);
            }
        }

        private async void LoadMap(double la, double lo,int id,string Registracija)
        {
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                Pin pin = new Pin()
                {
                    Label = id.ToString() + "  "+Registracija ,
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
    }
}