using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Aplikacija.Helperi;
using Aplikacija.Modeli;
using Xamarin.Essentials;
using Aplikacija.Forme;

namespace Aplikacija
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            
            InitializeComponent();
            
        }
        

        private async void Loguj(object sender, EventArgs e)
        {
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                if (Telefon.Text != null && Sifra.Text != null)
                {
                    FireBaseHelper f = new FireBaseHelper();
                    User u = new User();
                    u = await f.VratiUsera(Int32.Parse(Telefon.Text), Sifra.Text);
                    FireBaseHelper a = new FireBaseHelper();
                    Vozac v = new Vozac();
                    v = await a.VratiVozaca(Int32.Parse(Telefon.Text), Sifra.Text);
                    FireBaseHelper b = new FireBaseHelper();
                    Administrator z = new Administrator();
                    z = await b.VratiAdm(Int32.Parse(Telefon.Text), Sifra.Text);
                    FireBaseHelper y = new FireBaseHelper();
                    Sef i = new Sef();
                    i = await y.VratiSefa(Int32.Parse(Telefon.Text), Sifra.Text);
                    if (u != null)
                    {
                        await Navigation.PushModalAsync(new Glavna(u), true);
                    }
                    else if (v!=null)
                    {
                        await Navigation.PushModalAsync(new StranaVozaca(v), true);

                    }
                    else if (z != null)
                    {
                        await Navigation.PushModalAsync(new AdministratorPage(z), true);

                    }
                    else if (i != null)
                    {
                        await Navigation.PushModalAsync(new SefPage(i), true);

                    }
                    else
                    {
                        await DisplayAlert("Napomena", "Ne postoji nalog", "OK");
                    }
                    
                }
                else
                {
                    await DisplayAlert("Napomena", "Niste popunili sve potrebne informacije", "OK");
                }
            }
            else
            {
                await DisplayAlert("Upozorenje", "Niste konektovani na internet", "Potvrdi");
            }
            
        }

        private async void Registruj(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new Page1(), true);
           
        }

    }
}
