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
    public partial class SefPage : ContentPage
    {
        public SefPage(Sef i)
        {
            InitializeComponent();
        }

        private async void PregledPozicija_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new PregledPozicija(), true);

        }

        private async void PregledVozaca_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new PregledVozaca(), true);

        }

        private async void PregledNegativnih_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new PregledNegativnih(), true);

        }
    }
}