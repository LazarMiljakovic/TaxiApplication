using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplikacija.Helperi;
using Aplikacija.Modeli;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace Aplikacija.Forme
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NapraviSefa : ContentPage
    {
        public NapraviSefa()
        {
            InitializeComponent();
        }

        private async void NapraviSefa_Clicked(object sender, EventArgs e)
        {
            FireBaseHelper f = new FireBaseHelper();
            Sef sef = new Sef();
            sef.Sifra = Sifra.Text;
            sef.id = Int32.Parse(Id.Text);
            sef.Ime = Ime.Text;
            await f.RegistrujNalogSefa(sef);
            await Navigation.PopModalAsync();
        }
    }
}