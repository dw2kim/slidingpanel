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
        #region Constructor
        public SlidingPanelConfig()
        {
            OverlayButtonImageHeight = 0;
            OverlayButtonImageWidth = 0;

            TitleHeightRequest = TitleMinimumHeight;
        }
        #endregion

        #region Constants
        private const double TitleMinimumHeight = 24;
        #endregion

        #region Public Properties
        public Color TitleBackgroundColor { get; set; }
        public Color BodyBackgroundColor { get; set; }
        public Color PictureBackgroundColor { get; set; }

        public double TitleHeightRequest { get; set; }

        public Image OverlayButtonImage { get; set; }
        public double OverlayButtonImageHeight { get; set; }
        public double OverlayButtonImageWidth { get; set; }
        public EventHandler OverlayButtonImageTapGesture_Tapped { get; set; }

        public View MainStackLayout { get; set; }
        public View TitleView { get; set; }
        public View BodyView { get; set; }

        public Image PictureImage { get; set; }

        public Image RightTopButtonImage { get; set; }

        public Func<int> FunctionAfterTitleTapped { get; set; }
        #endregion
    }
}
