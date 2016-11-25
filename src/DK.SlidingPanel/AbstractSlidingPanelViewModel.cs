using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DK.SlidingPanel
{
    public class AbstractSlidingPanelViewModel
    {
        private const string PlayButtonImageFileName = "PlayButton48.png";
        private const string StopButtonImageFileName = "StopButton48.png";

        [Reactive]
        public ImageSource FollowButtonImage { get; set; }

        public AbstractSlidingPanelViewModel()
        {
            FollowButtonImage = ImageSource.FromFile(PlayButtonImageFileName);
        }
    }
}
