using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Aplikacija.Modeli;
using Aplikacija.Helperi;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aplikacija.Forme
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PregledNegativnih : ContentPage
    {
        public ObservableCollection<RecenzijeMod> DataSourceR { get; set; }

        public PregledNegativnih()
        {
            InitializeComponent();
            this.Load();
        }

        private async void Load()
        {
            FireBaseHelper f = new FireBaseHelper();
            List<RecenzijeMod> neg = await f.VratiOceneNegativne();
            DataSourceR = new ObservableCollection<RecenzijeMod>();
            negativne.ItemsSource = DataSourceR;
            negativne.RowHeight = 150;
            foreach (RecenzijeMod a in neg)
            {
                DataSourceR.Add(new RecenzijeMod() { vozac = a.vozac,zvezde=a.zvezde,napomena = a.napomena });
            }
        }

        private void negativne_Refreshing(object sender, EventArgs e)
        {
            DataSourceR.Clear();
            this.Load();
        }
    }
}