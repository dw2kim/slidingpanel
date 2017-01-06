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

namespace Samples.ViewModels
{
    public class TestViewModel : AbstractViewModel
    {
        private const string PlayButtonImageFileName = "PlayButton48.png";
        private const string StopButtonImageFileName = "StopButton48.png";

        private const string FavoriteImageFileName_iOS = "Icons/ic_star_48pt.png";
        private const string FavoriteImageFileName_Android = "Icons/ic_star_black_48dp.png";

        private const string NotFavoriteImageFileName_iOS = "Icons/ic_star_border_black_48dp_1x.png";
        private const string NotFavoriteImageFileName_Android = "Icons/ic_star_48pt.png";

        [Reactive]
        public bool IsPlaying { get; set; }
        [Reactive]
        public bool IsFavorite { get; set; }

        [Reactive]
        //public FileImageSource PlayButtonImage { get; set; }
        public ImageSource PlayButtonImage { get; set; }

        [Reactive]
        public ImageSource FavoriteButtonImage { get; set; }

        public ICommand PlayCommand { get; set; }

        public ICommand ShowCommand { get; set; }
        public ICommand HideCommand { get; set; }

        public TestViewModel()
        {
            this.IsPlaying = false;
            this.IsFavorite = false;

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
                    PlayButtonImage = ImageSource.FromFile(imageFileName);
                });

            this.WhenAnyValue(x => x.IsFavorite)
                .Subscribe(isFavorite =>
                {
                    string favoriteImageFileName = string.Empty;
                    Device.OnPlatform(
                        iOS: () => { favoriteImageFileName = FavoriteImageFileName_iOS; },
                        Android: () => { favoriteImageFileName = FavoriteImageFileName_Android; }
                        );

                    string notFavoriteImageFileName = string.Empty;
                    Device.OnPlatform(
                        iOS: () => { notFavoriteImageFileName = NotFavoriteImageFileName_iOS; },
                        Android: () => { notFavoriteImageFileName = NotFavoriteImageFileName_Android; }
                        );

                    string imageFileName = (isFavorite == true) ? favoriteImageFileName : notFavoriteImageFileName;
                    FavoriteButtonImage = ImageSource.FromFile(imageFileName); 
                });
        }

    }
}
