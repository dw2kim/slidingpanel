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

            SetupSlidingPanel();
        }
        #endregion

        #region Page Events
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            this.ViewModel = BindingContext as TestViewModel;

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

            //StackLayout mainStackLayout = new StackLayout
            //{
            //    BackgroundColor = Color.Yellow
            //};

            //Button btnShow = new Button
            //{
            //    Text = "Show"
            //};
            //btnShow.SetBinding(Button.CommandProperty, "ShowCommand");
            //btnShow.SetBinding(Button.CommandParameterProperty, new Binding() { Source = spTest });
            //mainStackLayout.Children.Add(btnShow);

            //Button btnHide = new Button
            //{
            //    Text = "Hide"
            //};
            //btnHide.SetBinding(Button.CommandProperty, "HideCommand");
            //btnHide.SetBinding(Button.CommandParameterProperty, new Binding() { Source = spTest });
            //mainStackLayout.Children.Add(btnHide);


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
        #endregion
    }
}
