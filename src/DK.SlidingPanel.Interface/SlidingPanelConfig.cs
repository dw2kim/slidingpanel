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
        #region Constants
        private const double TitleMinimumHeight = 24;
        #endregion

        #region Constructor
        public SlidingPanelConfig()
        {
            IsExpandable = true;

            PrimaryFloatingActionButtonHeight = 0;
            PrimaryFloatingActionButtonWidth = 0;

            TitleHeightRequest = TitleMinimumHeight;

            SecondaryFloatingActionButtonHeight = 0;
            SecondaryFloatingActionButtonWidth = 0;
            SecondaryFloatingActionButtonMarginTop = 0;
        }
        #endregion

        #region Public Properties
        public bool IsExpandable { get; set; }

        public Color TitleBackgroundColor { get; set; }
        public Color BodyBackgroundColor { get; set; }
        public Color PictureBackgroundColor { get; set; }

        public double TitleHeightRequest { get; set; }

        public Image PrimaryFloatingActionButton { get; set; }
        public double PrimaryFloatingActionButtonHeight { get; set; }
        public double PrimaryFloatingActionButtonWidth { get; set; }
        public EventHandler PrimaryFloatingActionButton_TapGesture_Tapped { get; set; }

        public Image SecondaryFloatingActionButton { get; set; }
        public double SecondaryFloatingActionButtonHeight { get; set; }
        public double SecondaryFloatingActionButtonWidth { get; set; }
        public EventHandler SecondaryFloatingActionButton_TapGesture_Tapped { get; set; }
        public double SecondaryFloatingActionButtonMarginTop { get; set; }

        public View MainStackLayout { get; set; }
        public View TitleView { get; set; }
        public View BodyView { get; set; }

        public Image PictureImage { get; set; }

        public Image TopLeftButtonImage { get; set; }
        public Image TopRightButtonImage { get; set; }

        public Func<int> FunctionAfterTitleTapped { get; set; }
        #endregion
    }
}
