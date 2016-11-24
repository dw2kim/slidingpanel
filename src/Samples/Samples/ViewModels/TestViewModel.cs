using DK.SlidingPanel.Interface;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Samples.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Samples.UI.ViewModels
{
    public class TestViewModel : AbstractViewModel
    {
        private const string PlayButtonImageFileName = "PlayButton48.png";
        private const string StopButtonImageFileName = "StopButton48.png";

        [Reactive]
        public bool IsPlaying { get; set; }

        [Reactive]
        //public FileImageSource PlayButtonImage { get; set; }
        public ImageSource PlayButtonImage { get; set; }

        public ICommand PlayCommand { get; set; }

        public ICommand ShowCommand { get; set; }
        public ICommand HideCommand { get; set; }

        public TestViewModel()
        {
            this.IsPlaying = false;

            this.ShowCommand = new Command((param) =>
            {
                AbstractSlidingPanel spTest = param as AbstractSlidingPanel;

                spTest.ShowCollapsedPanel();
            });

            this.HideCommand = new Command((param) =>
            {
                AbstractSlidingPanel spTest = param as AbstractSlidingPanel;

                spTest.HidePanel();
            });

            //this.PlayCommand = ReactiveCommand.Create(() =>
            //{
            //    IsPlaying = !(IsPlaying);
            //});

            this.WhenAnyValue(x => x.IsPlaying)
                .Subscribe(isPlaying =>
                {
                    string imageFileName = (isPlaying == true) ? StopButtonImageFileName : PlayButtonImageFileName;
                    //PlayButtonImage = ImageSource.FromFile(imageFileName) as FileImageSource;
                    PlayButtonImage = ImageSource.FromFile(imageFileName);
                });
        }

    }
}
