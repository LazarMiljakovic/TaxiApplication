using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplikacija.Helperi;
using Aplikacija.Modeli;
using Firebase.Database;
using Firebase.Database.Query;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aplikacija.Forme
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NapraviVozaca : ContentPage
    {
        public NapraviVozaca()
        {
            InitializeComponent();
        }

        private async void NapraviVozac_Clicked(object sender, EventArgs e)
        {
            FireBaseHelper f = new FireBaseHelper();
            Vozac vozac = new Vozac();
            vozac.sifra = Sifra.Text;
            vozac.id = Int32.Parse(Id.Text);
            vozac.Ime = Ime.Text;
            vozac.Prezime = Prezime.Text;
            vozac.Vozilo = Int32.Parse(Vozilo.Text);
            vozac.Vrsta = Vrsta.Text;
            vozac.Registracija = Registracija.Text;
            await f.RegistrujNalogVozaca(vozac);
            await Navigation.PopModalAsync();
        }
    }
}