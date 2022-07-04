using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplikacija.Modeli;
using Rg.Plugins.Popup.Extensions;
using Aplikacija.Helperi;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aplikacija.Forme
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Recenzije : Rg.Plugins.Popup.Pages.PopupPage
    {
        Voznja v = new Voznja();
        public Recenzije(Voznja v)
        {
            this.v = v;
            InitializeComponent();
        }

        private async void Potvrdi_Clicked(object sender, EventArgs e)
        {
            FireBaseHelper f = new FireBaseHelper();

            await f.Oceni(v.idVozaca, Convert.ToInt32(Ocene.Value), Napomena.Text);
            await Navigation.PopPopupAsync();
        }
    }
}