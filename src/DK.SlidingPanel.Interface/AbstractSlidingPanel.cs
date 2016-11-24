using DK.SlidingPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DK.SlidingPanel.Interface
{
    public class AbstractSlidingPanel : AbsoluteLayout, ISlidingPanel
    {
        #region Constants
        private const double TitleMinimumHeight = 50;
        private const double BodyMinimumHeight = 50;

        private const double DefaultButtonImageHeight = 48;
        private const double DefaultButtonImageWidth = 48;
        #endregion

        #region Private Fields
        private bool isCollapsing = false;
        private bool isPanRunning = false;
        private bool isCurrentlyCollapsed = false;
        #endregion

        #region Public Properties
        public RelativeLayout TitleRelativeLayout { get; set; }
        public StackLayout TitleStackLayout { get; set; }
        public StackLayout BodyStackLayout { get; set; }
        public Image ButtonImage { get; set; }
        #endregion

        #region Constructors
        public AbstractSlidingPanel() : base()
        {
            InitializeView();
            InitializeGestures();
        }
        #endregion

        #region Private Methods
        private void InitializeView()
        {
            StackLayout slSlidingPanel = new StackLayout();
            slSlidingPanel.Orientation = StackOrientation.Vertical;
            slSlidingPanel.Spacing = 0;
            Rectangle layoutBound = new Rectangle(1, 1, 1, 1);
            this.Children.Add(slSlidingPanel, layoutBound, AbsoluteLayoutFlags.All);
            
            // Title Section
            TitleRelativeLayout = new RelativeLayout();
            TitleRelativeLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
            TitleRelativeLayout.VerticalOptions = LayoutOptions.Fill;
            slSlidingPanel.Children.Add(TitleRelativeLayout);

            TitleStackLayout = new StackLayout();
            TitleStackLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
            TitleStackLayout.VerticalOptions = LayoutOptions.Fill;
            TitleRelativeLayout.Children.Add(TitleStackLayout,
                xConstraint: Constraint.Constant(0),
                yConstraint: Constraint.Constant(0),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Width);
                }),
                heightConstraint: Constraint.Constant(TitleMinimumHeight));


            // Button section
            ButtonImage = new Image();
            ButtonImage.WidthRequest = DefaultButtonImageWidth;
            ButtonImage.HeightRequest = DefaultButtonImageHeight;
            TitleRelativeLayout.Children.Add(ButtonImage,
                xConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Width - (ButtonImage.WidthRequest * 1.5));
                }),
                yConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (-1 * ButtonImage.HeightRequest / 2);
                })
            );
            
            // Body Section
            BodyStackLayout = new StackLayout();
            BodyStackLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
            BodyStackLayout.VerticalOptions = LayoutOptions.FillAndExpand;
            BodyStackLayout.MinimumHeightRequest = BodyMinimumHeight;
            slSlidingPanel.Children.Add(BodyStackLayout);
        }
        private void InitializeGestures()
        {
            TapGestureRecognizer TitlePanelTapGesture = new TapGestureRecognizer();
            TitlePanelTapGesture.Tapped += TitlePanelTapGesture_Tapped;
            TitleRelativeLayout.GestureRecognizers.Add(TitlePanelTapGesture);

            PanGestureRecognizer TitlePanelPanGesture = new PanGestureRecognizer();
            TitlePanelPanGesture.PanUpdated += TitlePanelPanGesture_PanUpdated; 
            TitleRelativeLayout.GestureRecognizers.Add(TitlePanelPanGesture);
        }
        #endregion

        #region Gesture Implementations
        private void TitlePanelTapGesture_Tapped(object sender, EventArgs e)
        {
            if (Device.OS == TargetPlatform.Android)
            {
                TapGesture_Tapped_Android(sender, e);
            }

            if (Device.OS == TargetPlatform.iOS)
            {
                TapGesture_Tapped_iOS(sender, e);
            }
        }
        private void TapGesture_Tapped_Android(object sender, EventArgs e)
        {
            if (isPanRunning == true)
            {
                isPanRunning = false;

                double minDrawerPosition = this.Height - TitleRelativeLayout.Height;
                double midDrawerPosition = minDrawerPosition / 2;
                double currentPosition = this.TranslationY;

                if (currentPosition > midDrawerPosition)
                {
                    ShowCollapsedPanel();
                }
                else
                {
                    ShowExpandedPanel();
                }
            }
            else
            {
                if (isCurrentlyCollapsed == true)
                {
                    ShowExpandedPanel();
                }
                else
                {
                    ShowCollapsedPanel();
                }
            }
        }
        private void TapGesture_Tapped_iOS(object sender, EventArgs e)
        {
            if (isPanRunning == true)
            {
            }
            else
            {
                if (isCurrentlyCollapsed == true)
                {
                    ShowExpandedPanel();
                }
                else
                {
                    ShowCollapsedPanel();
                }
            }
        }

        private void TitlePanelPanGesture_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (Device.OS == TargetPlatform.Android)
            {
                PanGesture_PanUpdated_Android(sender, e);
            }

            if (Device.OS == TargetPlatform.iOS)
            {
                PanGesture_PanUpdated_iOS(sender, e);
            }
        }
        private void PanGesture_PanUpdated_Android(object sender, PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Running)
            {
                double totalY = e.TotalY;


                if (totalY != 0 && totalY < 100 && totalY > -100)
                {
                    isCollapsing = (totalY > 0);
                    isPanRunning = true;

                    double minDrawerPosition = this.Height - TitleRelativeLayout.Height;
                    double maxDrawerPosition = 0;

                    double newDrawerPositionY = this.TranslationY + totalY;

                    if (newDrawerPositionY < maxDrawerPosition)
                    {
                        newDrawerPositionY = maxDrawerPosition;
                    }
                    if (newDrawerPositionY > minDrawerPosition)
                    {
                        newDrawerPositionY = minDrawerPosition;
                    }

                    this.TranslationY = newDrawerPositionY;
                }

            }

            if (e.StatusType == GestureStatus.Completed)
            {
                isPanRunning = false;
            }
        }
        private void PanGesture_PanUpdated_iOS(object sender, PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Running)
            {
                isPanRunning = true;

                double totalY = e.TotalY;

                if (totalY != -1)
                {
                    isCollapsing = (totalY > 0);

                    Rectangle drawerExpandedPosition = this.Bounds;
                    double minDrawerPostion = (this.Height - TitleRelativeLayout.Height);
                    double maxDrawerPosition = 0;
                    double newDrawerPosition = (totalY >= 0) ? totalY : minDrawerPostion + totalY;

                    if (newDrawerPosition < maxDrawerPosition)
                    {
                        newDrawerPosition = 0;
                    }
                    if (newDrawerPosition > minDrawerPostion)
                    {
                        newDrawerPosition = minDrawerPostion;
                    }

                    this.TranslateTo(drawerExpandedPosition.X, newDrawerPosition, 250, Easing.CubicOut);
                }
            }

            if (e.StatusType == GestureStatus.Completed)
            {
                isPanRunning = false;

                double minDrawerPosition = this.Height - TitleRelativeLayout.Height;
                double midDrawerPosition = minDrawerPosition / 2;
                double currentPosition = this.TranslationY;

                if (currentPosition > midDrawerPosition)
                {
                    ShowCollapsedPanel();
                }
                else
                {
                    ShowExpandedPanel();
                }
            }
        }
        #endregion

        #region ISlidingPanel Implementations
        public void HidePanel()
        {
            var x = 0;

            Rectangle drawerCollapsedPosition = this.Bounds;
            drawerCollapsedPosition.Y = this.Height + 24; // Half Button Height

            this.TranslateTo(drawerCollapsedPosition.X, drawerCollapsedPosition.Y, 700, Easing.CubicOut);
        }
        public void ShowCollapsedPanel()
        {
            var actualHeight = TitleRelativeLayout.Height;
            Rectangle drawerCollapsedPosition = this.Bounds;
            drawerCollapsedPosition.Y = this.Height - actualHeight;

            this.TranslateTo(drawerCollapsedPosition.X, drawerCollapsedPosition.Y, 700, Easing.CubicOut);
            isCurrentlyCollapsed = true;
        }
        public void ShowExpandedPanel()
        {
            var actualHeight = TitleRelativeLayout.Height;
            Rectangle drawerExpandedPosition = this.Bounds;
            drawerExpandedPosition.Y = 0;

            this.TranslateTo(drawerExpandedPosition.X, drawerExpandedPosition.Y, 700, Easing.CubicOut);
            isCurrentlyCollapsed = false;
        }
        #endregion

    }
}
