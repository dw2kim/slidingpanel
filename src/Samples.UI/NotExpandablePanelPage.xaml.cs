using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samples.ViewModels;

using DK.SlidingPanel.Interface;

using ReactiveUI;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace Samples.UI
{
    public partial class NotExpandablePanelPage : ContentPage
    {
        #region Private Fields     
        private NotExpandablePanelViewModel ViewModel;
        private Map GoogleMapInstance;
        #endregion

        #region Constructor
        public NotExpandablePanelPage()
        {
            InitializeComponent();

            //SetupSlidingPanel();
        }
        #endregion

        #region Page Events
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            this.ViewModel = BindingContext as NotExpandablePanelViewModel;

            SetupSlidingPanel();

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
        #endregion

        #region Private Methods
        private void InitGoogleMap()
        {
            GoogleMapInstance = new Map
            {
                HasScrollEnabled = true,
                HasZoomEnabled = true
            };

            Position randomPosition = new Position(5, 5);
            GoogleMapInstance.Pins.Add(new Pin
            {
                Label = "Test Pin",
                Position = randomPosition
            });
            GoogleMapInstance.MoveToRegion(new MapSpan(randomPosition, 0.5, 0.5));
        }
        private void SetupSlidingPanel()
        {
            InitGoogleMap();

            StackLayout mainStackLayout = new StackLayout();
            mainStackLayout.VerticalOptions = LayoutOptions.FillAndExpand;
            mainStackLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
            mainStackLayout.Children.Add(GoogleMapInstance);


            SlidingPanelConfig config = new SlidingPanelConfig();
            config.MainStackLayout = mainStackLayout;


            StackLayout titleStackLayout = new StackLayout();
            titleStackLayout.Orientation = StackOrientation.Vertical;
            Label title1 = new Label
            {
                Text = "Title 1"
            };
            Label title2 = new Label
            {
                Text = "Title 2"
            };
            titleStackLayout.Children.Add(title1);
            titleStackLayout.Children.Add(title2);
            config.TitleView = titleStackLayout;
            config.TitleHeightRequest = 60;
            config.TitleBackgroundColor = Color.Green;

            

            Image overlayButtonImage = new Image();
            overlayButtonImage.SetBinding(Image.SourceProperty, "PlayButtonImage");
            config.PrimaryFloatingActionButton = overlayButtonImage;
            config.PrimaryFloatingActionButtonHeight = 60;
            config.PrimaryFloatingActionButtonWidth = 60;
            config.PrimaryFloatingActionButton_TapGesture_Tapped = ButtonImageTapGesture_Tapped;


            config.IsExpandable = false;

            spTest.ApplyConfig(config);
        }

        private int HideNavigationBar()
        {
            //NavigationPage.SetHasNavigationBar(this, false);
            return (0);
        }
        #endregion

        #region Gesture Implemenetations
        private void ButtonImageTapGesture_Tapped(object sender, EventArgs e)
        {
            this.ViewModel.IsPlaying = !(this.ViewModel.IsPlaying);
        }

        private void BackButtonTapGesture_Tapped(object sender, EventArgs e)
        {
            spTest.HidePanel();
        }
        #endregion
    }
}
