using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samples.ViewModels;

using Xamarin.Forms;

namespace Samples.UI
{
    public partial class MainPage : ContentPage
    {
        public const string BarBackgroundHexColor = "#303030";

        public MainPage()
        {
            InitializeComponent();


            btnTestPage.Clicked += ((s, e) =>
            {
                var mdPage = Application.Current.MainPage as MasterDetailPage;
                mdPage.Master = new MenuPage { BindingContext = new MenuViewModel() };
                mdPage.Detail = new NavigationPage(new TestPage
                {
                    BindingContext = new TestViewModel()
                })
                {
                    BarBackgroundColor = Color.FromHex(BarBackgroundHexColor),
                    BarTextColor = Color.White
                }
                ;
            });

            btnTestPageAllXaml.Clicked += ((s, e) =>
            {
                var mdPage = Application.Current.MainPage as MasterDetailPage;
                mdPage.Master = new MenuPage { BindingContext = new MenuViewModel() };
                mdPage.Detail = new NavigationPage(new TestPageAllXaml
                {
                    BindingContext = new TestPageAllXamlViewModel()
                })
                {
                    BarBackgroundColor = Color.FromHex(BarBackgroundHexColor),
                    BarTextColor = Color.White
                };
            });
        }
    }
}
