using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Aplikacija.Forme;

namespace Aplikacija
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new Prolazna();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
