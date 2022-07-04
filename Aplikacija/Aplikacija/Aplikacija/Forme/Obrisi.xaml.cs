using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplikacija.Modeli;
using Aplikacija.Helperi;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aplikacija.Forme
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Obrisi : ContentPage
    {
        public ObservableCollection<Vozac> DataSourceV { get; set; }
        public ObservableCollection<Sef> DataSourceS { get; set; }
        public ObservableCollection<Administrator> DataSourceA { get; set; }
        public Obrisi()
        {
            InitializeComponent();
        }

        private async void adm_Clicked(object sender, EventArgs e)
        {
            
            obrisiListVoz.IsVisible = false;
            obrisiListSefa.IsVisible = false;
            obrisiListAdm.IsVisible = true;
            FireBaseHelper f = new FireBaseHelper();
            List<Administrator> listaAdmina = await f.VratiAdmList();
            DataSourceA = new ObservableCollection<Administrator>();
            obrisiListAdm.ItemsSource = DataSourceA;
            obrisiListAdm.RowHeight = 140;
            foreach (Administrator a in listaAdmina)
            {
                DataSourceA.Add(new Administrator() { id = a.id, Ime = a.Ime, Sifra = a.Sifra });
            }
        }

        private async void voz_Clicked(object sender, EventArgs e)
        {
            
            obrisiListVoz.IsVisible = true;
            obrisiListSefa.IsVisible = false;
            obrisiListAdm.IsVisible = false;
            FireBaseHelper f = new FireBaseHelper();
            List<Vozac> listaVozaca = await f.VratiVozList();
            DataSourceV = new ObservableCollection<Vozac>();
            obrisiListVoz.ItemsSource = DataSourceV;
            obrisiListVoz.RowHeight = 150;
            foreach (Vozac a in listaVozaca)
            {
                DataSourceV.Add(new Vozac() { id = a.id,Ime = a.Ime,Registracija = a.Registracija });
            }
        }

        private async void seff_Clicked(object sender, EventArgs e)
        {
            
            obrisiListVoz.IsVisible = false;
            obrisiListSefa.IsVisible = true;
            obrisiListAdm.IsVisible = false;
            FireBaseHelper f = new FireBaseHelper();
            List<Sef> listaSefova = await f.VratiSefList();
            DataSourceS = new ObservableCollection<Sef>();
            obrisiListSefa.ItemsSource = DataSourceS;
            obrisiListSefa.RowHeight = 140;
            foreach (Sef a in listaSefova)
            {
                DataSourceS.Add(new Sef() {id=a.id, Ime = a.Ime, Sifra = a.Sifra });
            }
        }

        private async void obrisiListAdm_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            FireBaseHelper f = new FireBaseHelper();
            List<Administrator> listaAdmina = await f.VratiAdmList();
            bool odg = await DisplayAlert("Upozorenje", "Da li ste sigurno da zelite obrisati "+ listaAdmina[e.SelectedItemIndex].Ime, "Prihvati", "Odbij");
            if(odg == true)
            {
                await f.ObrisiAdmina(listaAdmina[e.SelectedItemIndex]);
                await DisplayAlert("Napomena", "Uspesno ste obrisali admina " + listaAdmina[e.SelectedItemIndex].Ime, "OK");
            }
            
            DataSourceA.Clear();
            
        }

        private async void obrisiListVoz_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            FireBaseHelper f = new FireBaseHelper();
            List<Vozac> listaVozaca = await f.VratiVozList();
            bool odg = await DisplayAlert("Upozorenje", "Da li ste sigurno da zelite obrisati " + listaVozaca[e.SelectedItemIndex].Ime, "Prihvati", "Odbij");
            if (odg == true)
            {
                await f.ObrisiVozaca(listaVozaca[e.SelectedItemIndex]);
                await DisplayAlert("Napomena", "Uspesno ste obrisali admina " + listaVozaca[e.SelectedItemIndex].Ime, "OK");
            }
            DataSourceV.Clear();
        }

        private async void obrisiListSefa_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            FireBaseHelper f = new FireBaseHelper();
            List<Sef> listaSefa = await f.VratiSefList();
            bool odg = await DisplayAlert("Upozorenje", "Da li ste sigurno da zelite obrisati " + listaSefa[e.SelectedItemIndex].Ime, "Prihvati", "Odbij");
            if (odg == true)
            {
                await f.ObrisiSefa(listaSefa[e.SelectedItemIndex]);
                await DisplayAlert("Napomena", "Uspesno ste obrisali admina " + listaSefa[e.SelectedItemIndex].Ime, "OK");
            }
            DataSourceS.Clear();

        }
    }
}