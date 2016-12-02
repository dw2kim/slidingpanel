using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DK.SlidingPanel.Interface
{
    public class SlidingPanelConfig
    {
        public Color TitleBackgroundColor { get; set; }
        public Color BodyBackgroundColor { get; set; }
        public Color PictureBackgroundColor { get; set; }

        public Image OverlayButtonImage { get; set; }
        public double OverlayButtonImageHeight { get; set; }
        public double OverlayButtonImageWidth { get; set; }
        public EventHandler OverlayButtonImageTapGesture_Tapped { get; set; }

        public StackLayout MainStackLayout { get; set; }
        public StackLayout TitleStackLayout { get; set; }
        public StackLayout BodyStackLayout { get; set; }

        public Image PictureImage { get; set; }

        public Func<int> FunctionAfterTitleTapped { get; set; }

        public SlidingPanelConfig()
        {
            OverlayButtonImageHeight = double.MinValue;
            OverlayButtonImageWidth = double.MinValue;
        }
    }
}
