using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplikacija.Helperi;
using Aplikacija.Modeli;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aplikacija
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page1 : ContentPage
    {
        public Page1()
        {
            InitializeComponent();
        }

        private async void NapraviNalog(object sender, EventArgs e)
        {
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                if (TelefonR.Text != null && EmailR.Text != null && SifraR.Text != null)
                {
                    FireBaseHelper f = new FireBaseHelper();
                    if (f.VratiUsera(Int32.Parse(TelefonR.Text), SifraR.Text) != null)
                    {
                        User u = new User();
                        u.BrojTelefona = Int32.Parse(TelefonR.Text);
                        u.Ime = ImeR.Text;
                        u.Prezime = PrezimeR.Text;
                        u.Email = EmailR.Text;
                        u.Sifra = SifraR.Text;
                        await f.RegistrujNalog(u);
                        await DisplayAlert("Napomena", "Uspesno ste se registrovali", "OK");
                        await Navigation.PopModalAsync();
                    }
                    else
                    {
                        await DisplayAlert("Napomena", "Vec postoji nalog", "OK");
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
    }
}