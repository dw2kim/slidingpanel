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
        private const double DefaultPanelRatio = 0.6;

        private const double TitleMinimumHeight = 24;
        #endregion

        #region Constructor
        public SlidingPanelConfig()
        {
            PanelRatio = DefaultPanelRatio;
            IsExpandable = true;
            IsPanSupport = true;
            
            TitleHeightRequest = TitleMinimumHeight;
        }
        #endregion

        #region Public Properties
        public double PanelRatio { get; set; }
        public bool IsExpandable { get; set; }
        public bool IsPanSupport { get; set; }

        public Color HeaderBackgroundColor { get; set; }
        public Color TitleBackgroundColor { get; set; }
        public Color PictureBackgroundColor { get; set; }
        public Color BodyBackgroundColor { get; set; }

        public double TitleHeightRequest { get; set; }

        public View PrimaryFloatingActionButton { get; set; }
        public View SecondaryFloatingActionButton { get; set; }

        public View MainView { get; set; }
        public View TitleView { get; set; }
        public View BodyView { get; set; }

        public Image PictureImage { get; set; }

        public View HeaderLeftButton { get; set; }
        public View HeaderRightButton { get; set; }

        public Func<int> FunctionAfterTitleTapped { get; set; }
        #endregion
    }
}
