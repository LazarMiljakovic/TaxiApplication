using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplikacija.Modeli;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aplikacija.Forme
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdministratorPage : ContentPage
    {
        public AdministratorPage(Administrator a)
        {
            InitializeComponent();
        }

        private async void NapraviSef_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NapraviSefa(), true);
        }

        private async void NapraviVozac_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NapraviVozaca(), true);
        }

        private async void NapraviAdmin_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NapraviAdmina(), true);
        }

        

        private async void Obrisi_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new Obrisi(), true);
        }
    }
}