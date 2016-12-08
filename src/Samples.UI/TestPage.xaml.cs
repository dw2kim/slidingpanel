using DK.SlidingPanel.Interface;
using ReactiveUI;
using Samples.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace Samples.UI
{
    public partial class TestPage : ContentPage
    {
        #region Private Fields     
        private TestViewModel ViewModel;
        private Map GoogleMapInstance;
        #endregion

        #region Constructor
        public TestPage()
        {
            InitializeComponent();

            //SetupSlidingPanel();
        }
        #endregion

        #region Page Events
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            this.ViewModel = BindingContext as TestViewModel;

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


            StackLayout bodyStackLayout = new StackLayout();
            bodyStackLayout.Children.Add(new Label { Text = "Test Body y" });
            config.BodyView = bodyStackLayout;
            config.BodyBackgroundColor = Color.Blue;


            Image overlayButtonImage = new Image();
            overlayButtonImage.SetBinding(Image.SourceProperty, "PlayButtonImage");
            config.OverlayButtonImage = overlayButtonImage;
            config.OverlayButtonImageHeight = 60;
            config.OverlayButtonImageWidth = 60;
            config.OverlayButtonImageTapGesture_Tapped = ButtonImageTapGesture_Tapped;


            Image pictureImage = new Image();
            pictureImage.Source = ImageSource.FromFile("honda.jpg");
            pictureImage.Aspect = Aspect.AspectFill;
            pictureImage.VerticalOptions = LayoutOptions.StartAndExpand;
            pictureImage.HorizontalOptions = LayoutOptions.StartAndExpand;
            config.PictureImage = pictureImage;

            config.PictureBackgroundColor = Color.White;
            
            Image backButtonImage = new Image();
            backButtonImage.Source = ImageSource.FromFile("ic_keyboard_arrow_left_48pt.png");

            TapGestureRecognizer backButtonTapGesture = new TapGestureRecognizer();
            backButtonTapGesture.Tapped += BackButtonTapGesture_Tapped;
            backButtonImage.GestureRecognizers.Add(backButtonTapGesture);
            config.TopLeftButtonImage = backButtonImage;

            Image favoriteButtonImage = new Image();
            favoriteButtonImage.Source = ImageSource.FromFile("ic_star_border_black_48dp_1x.png");
            favoriteButtonImage.HorizontalOptions = LayoutOptions.EndAndExpand;
            config.TopRightButtonImage = favoriteButtonImage;

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
