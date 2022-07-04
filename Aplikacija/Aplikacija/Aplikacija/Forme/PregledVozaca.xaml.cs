using Aplikacija.Modeli;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplikacija.Helperi;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aplikacija.Forme
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PregledVozaca : ContentPage
    {
        public ObservableCollection<Vozac> DataSourceV { get; set; }

        public PregledVozaca()
        {
            InitializeComponent();
            this.Load();
        }

        private async void Load()
        {
            FireBaseHelper f = new FireBaseHelper();
            List<Vozac> listaVozaca = await f.VratiVozList();
            DataSourceV = new ObservableCollection<Vozac>();
            listavozaca.ItemsSource = DataSourceV;
            listavozaca.RowHeight = 250;
            foreach (Vozac a in listaVozaca)
            {
                DataSourceV.Add(new Vozac() { id = a.id, Ime = a.Ime,Prezime = a.Prezime, Registracija = a.Registracija,Vozilo = a.Vozilo,Vrsta = a.Vrsta,sifra = a.sifra });
            }
        }

        private void listavozaca_Refreshing(object sender, EventArgs e)
        {
            DataSourceV.Clear();
            this.Load();
        }
    }
}