using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Aplikacija.Modeli;
using Rg.Plugins.Popup.Extensions;
using Aplikacija.Helperi;
using System.Net.Http;
using Newtonsoft.Json;
using PolylinerNet;
using Xamarin.Essentials;
using Xamarin.Forms.GoogleMaps;
using System.Diagnostics;
using Plugin.LocalNotification;
using System.Threading;

namespace Aplikacija.Forme
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupNarucivanje : Rg.Plugins.Popup.Pages.PopupPage
    {
        Voznja v = new Voznja();
        int time;
        Vozac u = new Vozac();
        EventiHelper e = new EventiHelper();
        List<Vozac> lista = new List<Vozac>();
        NotificationRequest notification = new NotificationRequest();
        public PopupNarucivanje(Modeli.Voznja v)
        {
            this.v = v;
            InitializeComponent();
            
            this.TraziNajblizegVozaca();
        }


        EventiHelper d = new EventiHelper();
        private async void TraziNajblizegVozaca()
        {
            try
            {
                FireBaseHelper f = new FireBaseHelper();

                lista = await f.VratiSveSpremneVozace();
                
                
                e.TraziVozaca(v,lista);
                e.NoviVozac += E_NoviVozac;
                d.NadjenVozac(v);
                d.PrihvacenaVoznja += E_PrihvacenaVoznja;
                d.DrugiVozac += E_DrugiVozac;
                d.Isteklo += D_Isteklo;
                d.NemaVozaca += D_NemaVozaca;
            }
            catch(Exception ec)
            {
                Debug.WriteLine(ec);
            }
            
            
        }

        private async void D_NemaVozaca(object sender, EventArgs e)
        {
            FireBaseHelper f = new FireBaseHelper();

            lista = await f.VratiSveSpremneVozace();
            this.e.TraziVozaca(v, lista);
        }

        private void D_Isteklo(object sender, EventArgs e)
        {
            
            d.NadjenVozac(v);
        }

        private void E_NoviVozac(object sender, EventiHelper.VozacEvnetArgs e)
        {
            u = e.vozac;
        }

        private void E_DrugiVozac(object sender, EventArgs e)
        {
            
            this.e.IzbaciSaListe(u);
            TraziNajblizegVozaca();
        }

        private void E_PrihvacenaVoznja(object sender, int time)
        {
            this.time = e.VratiVreme();
            Labela.Text = "Vozac je na putu ka vasoj lokaciji za: " + time.ToString();
            this.OcekujeSeVozilo();
        }
        EventiHelper ez = new EventiHelper();
        private void OcekujeSeVozilo()
        {
            
            ez.OcekujeSeVoziloE(v);
            ez.Stigao += E_Stigao;
            ez.IstekloStigao += Ez_IstekloStigao;
            e.NoviVozac -= E_NoviVozac;
            e.PrihvacenaVoznja -= E_PrihvacenaVoznja;
            e.DrugiVozac -= E_DrugiVozac;
        }

        private void Ez_IstekloStigao(object sender, EventArgs e)
        {
            ez.OcekujeSeVoziloE(v);
        }

        private async void E_Stigao(object sender, EventArgs e)
        {
            notification.Title = "TAXIAPP";
            notification.Description = "Vozac je stigao";

            okretor.IsVisible = false;
            prekini.Text = "Izadji";
            Labela.Text = "Vozac je stigao";
            prekini.IsVisible = false;
            prekiniStigao.IsVisible = true;
            ez.Stigao -= E_Stigao;
            ez.IstekloStigao -= Ez_IstekloStigao;
            await NotificationCenter.Current.Show(notification);
        }

        private async void Prekini_Clicked(object sender, EventArgs e)
        {
            this.e.NoviVozac -= E_NoviVozac;
            this.e.PrihvacenaVoznja -= E_PrihvacenaVoznja;
            this.e.DrugiVozac -= E_DrugiVozac;
            ez.Stigao -= E_Stigao;
            ez.IstekloStigao -= Ez_IstekloStigao;
            
            FireBaseHelper f = new FireBaseHelper();
            await f.UpdateStatus(u.id,"Ugasena");
            FireBaseHelper a = new FireBaseHelper();
            await a.PrihvatiVoznju(v, "Otkazana");
            await Navigation.PopPopupAsync();

        }

        private async void prekiniStigao_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopPopupAsync();
        }
    }
}