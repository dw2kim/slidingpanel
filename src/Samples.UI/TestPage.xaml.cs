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
            
            this.spTest.TitleRelativeLayout.BackgroundColor = Color.Green;

            this.spTest.TitleStackLayout.Children.Add(new Label { Text = "Test Title x" });

            this.spTest.BodyStackLayout.BackgroundColor = Color.Blue;
            this.spTest.BodyStackLayout.Children.Add(new Label { Text = "Test Body x" });

            //this.spTest.ButtonImage.SetBinding(Button.ImageProperty, "PlayButtonImage");
            //this.spTest.ButtonImage.SetBinding(Button.CommandProperty, "PlayCommand");

            this.spTest.ButtonImage.SetBinding(Image.SourceProperty, "PlayButtonImage");
            TapGestureRecognizer ButtonImageTapGesture = new TapGestureRecognizer();
            ButtonImageTapGesture.Tapped += ButtonImageTapGesture_Tapped;
            this.spTest.ButtonImage.GestureRecognizers.Add(ButtonImageTapGesture);
        }

        private void ButtonImageTapGesture_Tapped(object sender, EventArgs e)
        {
            this.viewModel.IsPlaying = !(this.viewModel.IsPlaying);
        }
    }
}
