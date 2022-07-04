using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using Aplikacija.Helperi;
using Aplikacija.Modeli;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aplikacija.Forme
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NapraviAdmina : ContentPage
    {   
        
        public NapraviAdmina()
        {
            InitializeComponent();
        }

        private async void NapraviAdmin_Clicked(object sender, EventArgs e)
        {
            FireBaseHelper f = new FireBaseHelper();
            Administrator adm = new Administrator();
            adm.Sifra = Sifra.Text;
            adm.id = Int32.Parse(Id.Text);
            adm.Ime = Ime.Text;
            await f.RegistrujNalogAdm(adm);
            await Navigation.PopModalAsync();
        }
    }
}