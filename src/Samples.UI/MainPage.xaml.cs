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
        public MainPage()
        {
            InitializeComponent();

            btnTestPage.Clicked += ((s, e) =>
            {
                Navigation.PushAsync(new TestPage
                {
                    BindingContext = new TestViewModel()
                });
            });

            btnTestPageAllXaml.Clicked += ((s, e) =>
            {
                Navigation.PushAsync(new TestPageAllXaml
                {
                    BindingContext = new TestPageAllXamlViewModel()
                });
            });
        }
    }
}
