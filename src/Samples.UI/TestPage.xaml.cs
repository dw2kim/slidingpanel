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

            StackLayout mainStackLayout = new StackLayout();
            mainStackLayout.VerticalOptions = LayoutOptions.FillAndExpand;
            mainStackLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
            mainStackLayout.Children.Add(GoogleMapInstance);


            SlidingPanelConfig config = new SlidingPanelConfig();
            config.MainView = mainStackLayout;

            config.PanelRatio = 0.6;

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
            config.TitleHeightRequest = 80;
            config.TitleBackgroundColor = Color.Green;


            StackLayout bodyStackLayout = new StackLayout();
            bodyStackLayout.Children.Add(new Label { Text = "Test Body y" });
            config.BodyView = bodyStackLayout;
            config.BodyBackgroundColor = Color.Blue;


            Image primaryFloatingActionButton = new Image();
            primaryFloatingActionButton.SetBinding(Image.SourceProperty, "PlayButtonImage");
            config.PrimaryFloatingActionButton = primaryFloatingActionButton;
            config.PrimaryFloatingActionButtonHeight = 48;
            config.PrimaryFloatingActionButtonWidth = 48;
            config.PrimaryFloatingActionButton_TapGesture_Tapped = PlayButton_TapGesture_Tapped;

            Image secondaryFloatingActionButton = new Image();
            secondaryFloatingActionButton.SetBinding(Image.SourceProperty, "FavoriteButtonImage");
            config.SecondaryFloatingActionButton = secondaryFloatingActionButton;
            config.SecondaryFloatingActionButtonHeight = 48;
            config.SecondaryFloatingActionButtonWidth = 48;
            config.SecondaryFloatingActionButtonMarginTop = config.PrimaryFloatingActionButtonHeight + 6;
            config.SecondaryFloatingActionButton_TapGesture_Tapped = FavoriteButton_TapGesture_Tapped;

            Image pictureImage = new Image();
            pictureImage.Source = ImageSource.FromFile("honda.jpg");
            pictureImage.Aspect = Aspect.AspectFill;
            pictureImage.VerticalOptions = LayoutOptions.StartAndExpand;
            pictureImage.HorizontalOptions = LayoutOptions.StartAndExpand;
            config.PictureImage = pictureImage;

            config.PictureBackgroundColor = Color.White;
            
            Image backButtonImage = new Image();
            Device.OnPlatform(
                iOS: () =>
                {
                    backButtonImage.Source = ImageSource.FromFile("ic_keyboard_arrow_left_48pt.png");
                },
                Android: () =>
                {
                    backButtonImage.Source = ImageSource.FromFile("ic_keyboard_arrow_left_black_48dp.png");
                }
            );
            

            TapGestureRecognizer backButtonTapGesture = new TapGestureRecognizer();
            backButtonTapGesture.Tapped += BackButtonTapGesture_Tapped;
            backButtonImage.GestureRecognizers.Add(backButtonTapGesture);
            config.TopLeftButtonImage = backButtonImage;

            spTest.ApplyConfig(config);
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
