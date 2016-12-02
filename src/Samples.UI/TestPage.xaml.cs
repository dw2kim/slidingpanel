using DK.SlidingPanel.Interface;
using Samples.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Samples.UI
{
    public partial class TestPage : ContentPage
    {        
        private TestViewModel viewModel;

        public TestPage()
        {
            InitializeComponent();

        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            this.viewModel = BindingContext as TestViewModel;

            SetupSlidingPanel();
        }


        private void SetupSlidingPanel()
        {
            StackLayout mainStackLayout = new StackLayout
            {
                BackgroundColor = Color.Yellow
            };

            Button btnShow = new Button
            {
                Text = "Show"
            };
            btnShow.SetBinding(Button.CommandProperty, "ShowCommand");
            btnShow.SetBinding(Button.CommandParameterProperty, new Binding() { Source = spTest });
            mainStackLayout.Children.Add(btnShow);

            Button btnHide = new Button
            {
                Text = "Hide"
            };
            btnHide.SetBinding(Button.CommandProperty, "HideCommand");
            btnHide.SetBinding(Button.CommandParameterProperty, new Binding() { Source = spTest });
            mainStackLayout.Children.Add(btnHide);


            SlidingPanelConfig config = new SlidingPanelConfig();
            config.MainStackLayout = mainStackLayout;


            StackLayout titleStackLayout = new StackLayout();
            titleStackLayout.Children.Add(new Label { Text = "Test Title x" });
            config.TitleStackLayout = titleStackLayout;
            config.TitleBackgroundColor = Color.Green;


            StackLayout bodyStackLayout = new StackLayout();
            bodyStackLayout.Children.Add(new Label { Text = "Test Body y" });
            config.BodyStackLayout = bodyStackLayout;
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

        private void ButtonImageTapGesture_Tapped(object sender, EventArgs e)
        {
            this.viewModel.IsPlaying = !(this.viewModel.IsPlaying);
        }
    }
}
