using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Samples.ViewModels
{
    public class TestPageAllXamlViewModel : AbstractViewModel
    {
        private const string PlayButtonImageFileName = "PlayButton48.png";
        private const string StopButtonImageFileName = "StopButton48.png";

        private const string FavoriteImageFileName_iOS = "Icons/ic_star_48pt.png";
        private const string FavoriteImageFileName_Android = "ic_star_black_48dp.png";

        private const string NotFavoriteImageFileName_iOS = "Icons/ic_star_border_black_48dp_1x.png";
        private const string NotFavoriteImageFileName_Android = "ic_star_border_black_48dp.png";

        private const string BackButtonImageFileName_iOS = "ic_keyboard_arrow_left_48pt.png";
        private const string BackButtonImageFileName_Android = "ic_keyboard_arrow_left_black_48dp.png";

        private const string HondaImageFileName = "honda.jpg";

        private string FavoriteImageFileName
        {
            get
            {
                string imageName = string.Empty;
                Device.OnPlatform(iOS: () =>
                {
                    imageName = FavoriteImageFileName_iOS;
                }, Android: () =>
                {
                    imageName = FavoriteImageFileName_Android;
                });

                return (imageName);
            }
        }
        private string NotFavoriteImageFileName
        {
            get
            {
                string imageName = string.Empty;
                Device.OnPlatform(iOS: () =>
                {
                    imageName = NotFavoriteImageFileName_iOS;
                }, Android: () =>
                {
                    imageName = NotFavoriteImageFileName_Android;
                });

                return (imageName);
            }
        }

        [Reactive]
        public bool IsPlaying { get; set; }
        [Reactive]
        public bool IsFavorite { get; set; }

        [Reactive]
        public ImageSource PlayButtonImage { get; set; }

        [Reactive]
        public ImageSource FavoriteButtonImage { get; set; }

        public ImageSource HondaImage
        {
            get
            {
                ImageSource imgSrc = ImageSource.FromFile(HondaImageFileName);
                return (imgSrc);
            }
        }
        public ImageSource BackButtonImage
        {
            get
            {
                string imgFileName = string.Empty;
                Device.OnPlatform(iOS: () =>
                {
                    imgFileName = BackButtonImageFileName_iOS;
                }, Android: () =>
                {
                    imgFileName = BackButtonImageFileName_Android;
                });

                ImageSource imgSrc = ImageSource.FromFile(imgFileName);
                return (imgSrc);
            }
        }

        public ImageSource StopButtonImage
        {
            get
            {
                return ImageSource.FromFile(StopButtonImageFileName);
            }
        }

        public ICommand PlayCommand { get; set; }

        public ICommand ShowCommand { get; set; }
        public ICommand HideCommand { get; set; }


        [Reactive]
        public double PanelRatio { get; set; }

        [Reactive]
        public bool HideTitleView { get; set; }

        public TestPageAllXamlViewModel()
        {
        }
    }
}
