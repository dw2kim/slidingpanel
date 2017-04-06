using Samples.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Samples.UI
{
    public partial class App : Application
    {
        public const string BarBackgroundHexColor = "#303030";

        public App()
        {
            InitializeComponent();

            //MainPage = new NavigationPage(new MainPage());
            MainPage = new MasterDetailPage
            {
                Master = new MenuPage { BindingContext = new MenuViewModel() },
                Detail = new NavigationPage(new MainPage())
                {
                    BarBackgroundColor = Color.FromHex(BarBackgroundHexColor),
                    BarTextColor = Color.White
                }
            };
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
