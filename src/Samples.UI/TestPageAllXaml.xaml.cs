using ReactiveUI;
using Samples.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace Samples.UI
{
    public partial class TestPageAllXaml : ContentPage
    {
        #region Private Fields     
        private TestPageAllXamlViewModel ViewModel;
        //private Map GoogleMapInstance;
        #endregion

        #region Constructor
        public TestPageAllXaml()
        {
            InitializeComponent();
        }
        #endregion


        #region Page Events
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            this.ViewModel = BindingContext as TestPageAllXamlViewModel;

            //SetupSlidingPanel();


            if (GoogleMapInstance != null)
            {
                Position randomPosition = new Position(5, 5);
                GoogleMapInstance.Pins.Add(new Pin
                {
                    Label = "Test Pin",
                    Position = randomPosition
                });
                GoogleMapInstance.MoveToRegion(new MapSpan(randomPosition, 0.5, 0.5));

                GoogleMapInstance.WhenAnyValue(x => x.SelectedPin)
                    .Subscribe(selectedPin =>
                    {
                        if (selectedPin == null)
                        {
                            spTest.HidePanel();
                        }

                        if (selectedPin != null)
                        {
                            spTest.ShowCollapsedPanel();
                        }
                    });
            }
        }
        #endregion

        #region Gesture Implemenetations
        private void BackButtonTapGesture_Tapped(object sender, EventArgs e)
        {
            spTest.ShowCollapsedPanel();
        }
        #endregion
    }
}
