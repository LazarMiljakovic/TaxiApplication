using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aplikacija
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Prolazna : ContentPage
    {
        public Prolazna()
        {
            InitializeComponent();
            Logut();
            
        }

        private async void Logut()
        {
            await Task.Delay(3000);
            Pali();
        }

        private async void Pali()
        {
            await Navigation.PushModalAsync(new MainPage(), true);
            
        }
    }
}