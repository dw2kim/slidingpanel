using DK.SlidingPanel.Interface;
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


            SlidingPanelConfig config = new SlidingPanelConfig();
            config.MainView = GetMainStackLayout();
            config.HideNavBar = true;

            config.PanelRatio = 0.6;
            config.IsPanSupport = true;

            config.TitleView = GetTitleStackLayout();
            config.TitleHeightRequest = 80;
            config.TitleBackgroundColor = Color.Green;

            config.BodyView = GetBodyStackLayout();
            config.BodyBackgroundColor = Color.Blue;

            config.PrimaryFloatingActionButton = GetPrimaryFloatingActionButton();
            config.SecondaryFloatingActionButton = GetSecondaryFloatingActionButton();

            config.PictureBackgroundColor = Color.White;
            config.HeaderBackgroundColor = Color.White;

            config.PictureImage = GetHondaPictureImage();
            config.HeaderLeftButton = GetBackButtonImage();

            spTest.ApplyConfig(config);
        }

        private StackLayout GetMainStackLayout()
        {
            StackLayout mainStackLayout = new StackLayout();
            mainStackLayout.VerticalOptions = LayoutOptions.FillAndExpand;
            mainStackLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
            mainStackLayout.Children.Add(GoogleMapInstance);

            return (mainStackLayout);
        }
        private StackLayout GetTitleStackLayout()
        {
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

            return (titleStackLayout);
        }
        private StackLayout GetBodyStackLayout()
        {
            StackLayout bodyStackLayout = new StackLayout();
            bodyStackLayout.Children.Add(new Label { Text = "Test Body y" });

            return (bodyStackLayout);
        }
        private Image GetPrimaryFloatingActionButton()
        {
            Image primaryFloatingActionButton = new Image();
            primaryFloatingActionButton.HeightRequest = 48;
            primaryFloatingActionButton.WidthRequest = 48;
            primaryFloatingActionButton.SetBinding(Image.SourceProperty, "PlayButtonImage");

            TapGestureRecognizer primaryFloatingActionButton_TapGesture = new TapGestureRecognizer();
            primaryFloatingActionButton_TapGesture.Tapped += PlayButton_TapGesture_Tapped;
            primaryFloatingActionButton.GestureRecognizers.Add(primaryFloatingActionButton_TapGesture);

            return (primaryFloatingActionButton);
        }
        private Image GetSecondaryFloatingActionButton()
        {
            Image secondaryFloatingActionButton = new Image();
            secondaryFloatingActionButton.HeightRequest = 48;
            secondaryFloatingActionButton.WidthRequest = 48;
            secondaryFloatingActionButton.Margin = new Thickness(0, 6, 0, 0);
            secondaryFloatingActionButton.SetBinding(Image.SourceProperty, "FavoriteButtonImage");

            TapGestureRecognizer secondaryFloatingActionButton_TapGesture = new TapGestureRecognizer();
            secondaryFloatingActionButton_TapGesture.Tapped += FavoriteButton_TapGesture_Tapped;
            secondaryFloatingActionButton.GestureRecognizers.Add(secondaryFloatingActionButton_TapGesture);

            return (secondaryFloatingActionButton);
        }
        private Image GetHondaPictureImage()
        {
            Image pictureImage = new Image();
            pictureImage.Aspect = Aspect.AspectFill;
            pictureImage.VerticalOptions = LayoutOptions.StartAndExpand;
            pictureImage.HorizontalOptions = LayoutOptions.StartAndExpand;
            pictureImage.SetBinding(Image.SourceProperty, "HondaImage");

            return (pictureImage);
        }
        private Image GetBackButtonImage()
        {
            Image backButtonImage = new Image();
            backButtonImage.HeightRequest = 48;
            backButtonImage.WidthRequest = 48;
            backButtonImage.SetBinding(Image.SourceProperty, "BackButtonImage");

            TapGestureRecognizer backButtonTapGesture = new TapGestureRecognizer();
            backButtonTapGesture.Tapped += BackButtonTapGesture_Tapped;
            backButtonImage.GestureRecognizers.Add(backButtonTapGesture);

            return (backButtonImage);
        }

        private int HideNavigationBar()
        {
            //NavigationPage.SetHasNavigationBar(this, false);
            return (0);
        }
        #endregion

        #region Gesture Implemenetations
        private void PlayButton_TapGesture_Tapped(object sender, EventArgs e)
        {
            this.ViewModel.IsPlaying = !(this.ViewModel.IsPlaying);
        }
        private void FavoriteButton_TapGesture_Tapped(object sender, EventArgs e)
        {
            this.ViewModel.IsFavorite = !(this.ViewModel.IsFavorite);
        }

        private void BackButtonTapGesture_Tapped(object sender, EventArgs e)
        {
            spTest.ShowCollapsedPanel();
        }
        #endregion
    }
}
