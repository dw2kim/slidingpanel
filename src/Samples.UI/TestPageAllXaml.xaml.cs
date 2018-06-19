﻿using DK.SlidingPanel.Interface;
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
            
            this.spTest.SetBinding(SlidingUpPanel.PanelRatioProperty, new Binding { Path = "PanelRatio" });
            this.spTest.SetBinding(SlidingUpPanel.HideTitleViewProperty, new Binding { Path = "HideTitleView" });
            
            this.spTest.WhenPanelRatioChanged += SpTest_WhenPanelRatioChanged;
            this.spTest.WhenSlidingPanelStateChanged += SpTest_WhenSlidingPanelStateChanged;
            //SetupSlidingPanel();

            if (GoogleMapInstance != null)
            {
                Position randomPosition = new Position(5, 5);
                GoogleMapInstance.Pins.Add(new Pin
                {
                    Label = "Test Pin Full Ratio",
                    Position = randomPosition
                });
                GoogleMapInstance.Pins.Add(new Pin
                {
                    Label = "Test Pin Half Ratio",
                    Position = new Position(randomPosition.Latitude + 0.1, randomPosition.Longitude + 0.1)
                });
                GoogleMapInstance.Pins.Add(new Pin
                {
                    Label = "Test Pin Half Ratio",
                    Position = new Position(randomPosition.Latitude - 0.1, randomPosition.Longitude - 0.1)
                });
                GoogleMapInstance.MoveToRegion(new MapSpan(randomPosition, 0.5, 0.5));

                GoogleMapInstance.WhenAnyValue(x => x.SelectedPin)
                    .Subscribe(selectedPin =>
                    {

                        if (selectedPin == null)
                        {
                            if (spTest.CurrentState != SlidingPanelState.Expanded)
                            {
                                spTest.HidePanel();
                            }
                            else
                            {

                            }
                        }

                        if (selectedPin != null)
                        {
                            spTest.HidePanel();

                            if (selectedPin.Label == "Test Pin Full Ratio")
                            {
                                this.ViewModel.PanelRatio = 1;
                                this.ViewModel.HideTitleView = false;
                            }

                            if (selectedPin.Label == "Test Pin Half Ratio")
                            {
                                this.ViewModel.PanelRatio = 0.6;
                                this.ViewModel.HideTitleView = false;
                            }

                            spTest.ShowCollapsedPanel();
                        }
                    });
            }

            primaryFABImage.GestureRecognizers.Add(spTest.TitlePanelTapGesture);
            primaryFABImage.GestureRecognizers.Add(spTest.PanelPanGesture);
        }

        protected override bool OnBackButtonPressed()
        {
            return base.OnBackButtonPressed();
        }
        #endregion

        private void SpTest_WhenSlidingPanelStateChanged(object sender, DK.SlidingPanel.Interface.StateChangedEventArgs e)
        {
            switch (e.State)
            {
                case SlidingPanelState.Expanded:
                    //NavigationPage.SetHasNavigationBar(this, false);
                    primaryFABImage.IsVisible = false;
                    break;
                case SlidingPanelState.Collapsed:
                    primaryFABImage.IsVisible = true;
                    break;
                case SlidingPanelState.Hidden:
                    primaryFABImage.IsVisible = false;
                    break;
                default:
                    primaryFABImage.IsVisible = false;
                    //NavigationPage.SetHasNavigationBar(this, true);
                    break;
            }
        }

        private void SpTest_WhenPanelRatioChanged(object sender, EventArgs e)
        {
            spTest.ShowCollapsedPanel();
        }

        #region Gesture Implemenetations
        private void BackButtonTapGesture_Tapped(object sender, EventArgs e)
        {
            spTest.ShowCollapsedPanel();
        }
        #endregion
    }
}
