using DK.SlidingPanel;
using ReactiveUI;
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
        private bool _isCollapsing = false;
        private bool _isPanRunning = false;
        private bool _isCurrentlyCollapsed = false;

        private bool _isFirst = true;

        private SlidingPanelConfig _config;
        #endregion

        #region Private Properties
        private StackLayout _mainStackLayout { get; set; }
        private AbsoluteLayout _slidingPanelAbsoluteLayout { get; set; }

        private RelativeLayout _titleRelativeLayout { get; set; }
        private StackLayout _titleStackLayout { get; set; }
        private StackLayout _bodyStackLayout { get; set; }

        private Image _overlayButtonImage { get; set; }

        private AbsoluteLayout _pictureAbsoluteLayout { get; set; }
        private Image _pictureImage { get; set; }

        private Func<int?> _functionAfterTitleTapped { get; set; }

        private EventHandler _overlayButtonImageTapGesture_Tapped { get; set; }

        private bool IsOverlayButtonImageNull
        {
            get
            {
                return (_overlayButtonImage == null || _overlayButtonImage.Source == null);
            }
        }
        private bool IsPictureImageNull
        {
            get
            {
                return (_pictureImage == null || _pictureImage.Source == null);
            }
        }
        #endregion

        #region Constructors
        public AbstractSlidingPanel() : base()
        {
            InitViews();
            InitFunctions();

            _slidingPanelAbsoluteLayout.WhenAnyValue(x => x.Height)
                .Subscribe(actualHeight =>
                {
                    if (_isFirst == true && actualHeight > -1)
                    {
                        HidePanel(0);
                        _isFirst = false;
                    }
                });
        }
        public AbstractSlidingPanel(SlidingPanelConfig config) : this()
        {
            ApplyConfig(config);
        }
        #endregion

        #region Private Methods
        private void InitViews()
        {
            _mainStackLayout = new StackLayout();
            _mainStackLayout.Spacing = 0;
            AbsoluteLayout.SetLayoutBounds(_mainStackLayout, new Rectangle(1, 1, 1, 1));
            AbsoluteLayout.SetLayoutFlags(_mainStackLayout, AbsoluteLayoutFlags.All);
            this.Children.Add(_mainStackLayout);

            // Picture
            _pictureAbsoluteLayout = new AbsoluteLayout();
            AbsoluteLayout.SetLayoutBounds(_pictureAbsoluteLayout, new Rectangle(1, 0, 1, 0.5));
            AbsoluteLayout.SetLayoutFlags(_pictureAbsoluteLayout, AbsoluteLayoutFlags.All);
            this.Children.Add(_pictureAbsoluteLayout);
            

            // Drawer
            _slidingPanelAbsoluteLayout = new AbsoluteLayout();
            AbsoluteLayout.SetLayoutBounds(_slidingPanelAbsoluteLayout, new Rectangle(1, 1, 1, 0.5));
            AbsoluteLayout.SetLayoutFlags(_slidingPanelAbsoluteLayout, AbsoluteLayoutFlags.All);
            this.Children.Add(_slidingPanelAbsoluteLayout);
            
            // Drawer
            StackLayout slSlidingPanel = new StackLayout();
            slSlidingPanel.Spacing = 0;
            Rectangle layoutBound = new Rectangle(1, 1, 1, 1);
            _slidingPanelAbsoluteLayout.Children.Add(slSlidingPanel, layoutBound, AbsoluteLayoutFlags.All);
            
            // Title Section
            _titleRelativeLayout = new RelativeLayout();
            _titleRelativeLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
            _titleRelativeLayout.VerticalOptions = LayoutOptions.Fill;
            slSlidingPanel.Children.Add(_titleRelativeLayout);

            _titleStackLayout = new StackLayout();
            _titleStackLayout.Spacing = 0;
            _titleStackLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
            _titleStackLayout.VerticalOptions = LayoutOptions.Fill;
            _titleRelativeLayout.Children.Add(_titleStackLayout,
                xConstraint: Constraint.Constant(0),
                yConstraint: Constraint.Constant(0),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Width);
                }),
                heightConstraint: Constraint.Constant(TitleMinimumHeight));
            
            // Body Section
            _bodyStackLayout = new StackLayout();
            _bodyStackLayout.Spacing = 0;
            _bodyStackLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
            _bodyStackLayout.VerticalOptions = LayoutOptions.FillAndExpand;
            _bodyStackLayout.MinimumHeightRequest = BodyMinimumHeight;
            slSlidingPanel.Children.Add(_bodyStackLayout);
        }
        private void InitGestures()
        {
            TapGestureRecognizer titlePanelTapGesture = new TapGestureRecognizer();
            titlePanelTapGesture.Tapped += TitlePanelTapGesture_Tapped;
            _titleRelativeLayout.GestureRecognizers.Add(titlePanelTapGesture);

            PanGestureRecognizer titlePanelPanGesture = new PanGestureRecognizer();
            titlePanelPanGesture.PanUpdated += PanGesture_PanUpdated; 
            _titleRelativeLayout.GestureRecognizers.Add(titlePanelPanGesture);

            PanGestureRecognizer bodyPanelPanGesture = new PanGestureRecognizer();
            bodyPanelPanGesture.PanUpdated += PanGesture_PanUpdated;
            _bodyStackLayout.GestureRecognizers.Add(bodyPanelPanGesture);
            
            if (_overlayButtonImageTapGesture_Tapped != null)
            {
                TapGestureRecognizer overlayButtonImageTapGesture = new TapGestureRecognizer();
                overlayButtonImageTapGesture.Tapped += _overlayButtonImageTapGesture_Tapped;
                _overlayButtonImage.GestureRecognizers.Add(overlayButtonImageTapGesture);
            }
        }
        private void InitFunctions()
        {
            _functionAfterTitleTapped = new Func<int?>(()=> {
                return (null);
            });
        }
        
        private double CalculateNewDrawerPositionY(double totalY)
        {
            double bodyHeight = _slidingPanelAbsoluteLayout.Height - _titleRelativeLayout.Height;

            Rectangle drawerExpandedPosition = _slidingPanelAbsoluteLayout.Bounds;
            double minDrawerPostion = bodyHeight;
            double maxDrawerPosition = 0;
            double newDrawerPosition = (totalY >= 0) ? totalY : minDrawerPostion + totalY;

            if (newDrawerPosition < maxDrawerPosition)
            {
                newDrawerPosition = maxDrawerPosition;
            }
            if (newDrawerPosition > minDrawerPostion)
            {
                newDrawerPosition = minDrawerPostion;
            }

            return (newDrawerPosition);
        }
        private double CalculateNewPicturePositionY(double totalY)
        {
            double minPicturePostion = _pictureAbsoluteLayout.Height + (_slidingPanelAbsoluteLayout.Height - _titleRelativeLayout.Height);
            double maxPicturePosition = 0;
            
            double titleHeight = _titleRelativeLayout.Height;
            double bodyHeight = _slidingPanelAbsoluteLayout.Height - titleHeight;
            double screenHeight = this.Height;

            double totalPictureYFactor = 2;// (screenHeight / bodyHeight);
            double totalPictureY = totalY * totalPictureYFactor;
            double newPicturePositionY = (totalPictureY >= 0) ? maxPicturePosition + totalPictureY : minPicturePostion + totalPictureY;

            if (newPicturePositionY < maxPicturePosition)
            {
                newPicturePositionY = maxPicturePosition;
            }
            if (newPicturePositionY > minPicturePostion)
            {
                newPicturePositionY = minPicturePostion;
            }

            return (newPicturePositionY);
        }
        #endregion

        #region Gesture Implementations
        private void TitlePanelTapGesture_Tapped(object sender, EventArgs e)
        {

            if (Device.OS == TargetPlatform.Android)
            {
                TitlePanelTapGesture_Tapped_Android(sender, e);
            }

            if (Device.OS == TargetPlatform.iOS)
            {
                TitlePanelTapGesture_Tapped_iOS(sender, e);
            }
            
            //FunctionAfterTitleTapped();
        }
        private void TitlePanelTapGesture_Tapped_Android(object sender, EventArgs e)
        {
            if (_isPanRunning == true)
            {
                _isPanRunning = false;

                double minDrawerPosition = _slidingPanelAbsoluteLayout.Height - _titleRelativeLayout.Height;
                double midDrawerPosition = minDrawerPosition / 2;
                double currentPosition = _slidingPanelAbsoluteLayout.TranslationY;

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
                if (_isCurrentlyCollapsed == true)
                {
                    ShowExpandedPanel();
                }
                else
                {
                    ShowCollapsedPanel();
                }
            }
        }
        private void TitlePanelTapGesture_Tapped_iOS(object sender, EventArgs e)
        {
            if (_isPanRunning == true)
            {
            }
            else
            {
                if (_isCurrentlyCollapsed == true)
                {
                    ShowExpandedPanel();
                }
                else
                {
                    ShowCollapsedPanel();
                }
            }
        }

        private void PanGesture_PanUpdated(object sender, PanUpdatedEventArgs e)
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
                    _isCollapsing = (totalY > 0);
                    _isPanRunning = true;

                    double newDrawerPositionY = CalculateNewDrawerPositionY(totalY);
                    _slidingPanelAbsoluteLayout.TranslationY = newDrawerPositionY;

                    if (IsPictureImageNull == false)
                    {
                        double newPicturePosition = CalculateNewPicturePositionY(totalY);
                        _pictureAbsoluteLayout.TranslationY = newPicturePosition;
                    }
                }
            }

            if (e.StatusType == GestureStatus.Completed)
            {
                _isPanRunning = false;
            }
        }
        private void PanGesture_PanUpdated_iOS(object sender, PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Running)
            {
                _isPanRunning = true;

                double totalY = e.TotalY;

                if (totalY != -1)
                {
                    _isCollapsing = (totalY > 0);

                    double newDrawerPosition = CalculateNewDrawerPositionY(totalY);
                    _slidingPanelAbsoluteLayout.TranslateTo(0, newDrawerPosition, 250, Easing.CubicOut);

                    if (IsPictureImageNull == false)
                    {
                        double newPicturePosition = CalculateNewPicturePositionY(totalY);
                        _pictureAbsoluteLayout.TranslateTo(0, newPicturePosition, 250, Easing.CubicOut);
                    }
                }
            }

            if (e.StatusType == GestureStatus.Completed)
            {
                _isPanRunning = false;

                double minDrawerPosition = _slidingPanelAbsoluteLayout.Height - _titleRelativeLayout.Height;
                double midDrawerPosition = minDrawerPosition / 2;
                double currentPosition = _slidingPanelAbsoluteLayout.TranslationY;

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
        public void HidePanel(uint length = 700)
        {
            double overlayButtonImageHeight = (_config.OverlayButtonImageHeight > 0) ? _config.OverlayButtonImageHeight : 0;
            Rectangle drawerCollapsedPosition = _slidingPanelAbsoluteLayout.Bounds;
            drawerCollapsedPosition.Y = _slidingPanelAbsoluteLayout.Height + (overlayButtonImageHeight / 2); 

            _slidingPanelAbsoluteLayout.TranslateTo(drawerCollapsedPosition.X, drawerCollapsedPosition.Y, length, Easing.CubicOut);
            
            //if (IsPictureImageNull == false)
            //{
                Rectangle pictureBounds = _pictureAbsoluteLayout.Bounds;
                pictureBounds.Y = drawerCollapsedPosition.Y + _pictureAbsoluteLayout.Height;

                _pictureAbsoluteLayout.TranslateTo(pictureBounds.X, pictureBounds.Y, length, Easing.CubicOut);
            //}
        }
        public void ShowCollapsedPanel(uint length = 700)
        {
            var actualHeight = _titleRelativeLayout.Height;
            Rectangle drawerCollapsedPosition = _slidingPanelAbsoluteLayout.Bounds;
            drawerCollapsedPosition.Y = _slidingPanelAbsoluteLayout.Height - actualHeight;

            _slidingPanelAbsoluteLayout.TranslateTo(drawerCollapsedPosition.X, drawerCollapsedPosition.Y, length, Easing.CubicOut);
            _isCurrentlyCollapsed = true;
            
            if (IsPictureImageNull == false)
            {
                Rectangle pictureBounds = _pictureAbsoluteLayout.Bounds;
                pictureBounds.Y = drawerCollapsedPosition.Y + _pictureAbsoluteLayout.Height;

                _pictureAbsoluteLayout.TranslateTo(pictureBounds.X, pictureBounds.Y, length, Easing.CubicOut);
            }
        }
        public void ShowExpandedPanel(uint length = 700)
        {
            var actualHeight = _titleRelativeLayout.Height;
            Rectangle drawerExpandedPosition = _slidingPanelAbsoluteLayout.Bounds;
            drawerExpandedPosition.Y = 0;

            _slidingPanelAbsoluteLayout.TranslateTo(drawerExpandedPosition.X, drawerExpandedPosition.Y, length, Easing.CubicOut);
            _isCurrentlyCollapsed = false;
            
            if (IsPictureImageNull == false)
            {
                Rectangle pictureExpandedPosition = _pictureAbsoluteLayout.Bounds;
                pictureExpandedPosition.Y = 0;
                _pictureAbsoluteLayout.TranslateTo(pictureExpandedPosition.X, pictureExpandedPosition.Y, length, Easing.CubicOut);
            }
        }

        public void ApplyConfig(SlidingPanelConfig config)
        {
            this._config = config;

            if (config.IsExpandable == true)
            {
                InitGestures();
            }

            _titleRelativeLayout.BackgroundColor = config.TitleBackgroundColor;
            _bodyStackLayout.BackgroundColor = config.BodyBackgroundColor;
            
            if (config.OverlayButtonImage != null)
            {
                // Button section
                _overlayButtonImage = config.OverlayButtonImage;
                _overlayButtonImage.WidthRequest = (config.OverlayButtonImageWidth > 0) ? config.OverlayButtonImageWidth : DefaultButtonImageWidth;
                _overlayButtonImage.HeightRequest = (config.OverlayButtonImageHeight > 0) ? config.OverlayButtonImageHeight : DefaultButtonImageHeight;

                if (config.OverlayButtonImageTapGesture_Tapped != null)
                {
                    TapGestureRecognizer overlayButtonImageTapGesture = new TapGestureRecognizer();
                    overlayButtonImageTapGesture.Tapped += config.OverlayButtonImageTapGesture_Tapped;
                    _overlayButtonImage.GestureRecognizers.Add(overlayButtonImageTapGesture);
                }

                _titleRelativeLayout.Children.Add(_overlayButtonImage,
                    xConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Width - (_overlayButtonImage.WidthRequest * 1.5));
                    }),
                    yConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (-1 * _overlayButtonImage.HeightRequest / 2);
                    })
                );
            }

            _mainStackLayout.Children.Add(config.MainStackLayout);
            _titleStackLayout.Children.Add(config.TitleView);
            _titleRelativeLayout.HeightRequest = config.TitleHeightRequest;

            _bodyStackLayout.Children.Add(config.BodyView);

            if (config.PictureImage != null)
            {
                StackLayout pictureControlStackLayout = new StackLayout();
                pictureControlStackLayout.Orientation = StackOrientation.Horizontal;
                pictureControlStackLayout.HorizontalOptions = LayoutOptions.FillAndExpand;

                Image topLeftButtonImage = config.TopLeftButtonImage;
                if (topLeftButtonImage != null)
                {
                    pictureControlStackLayout.Children.Add(topLeftButtonImage);
                }

                Image topRightButtonImage = config.TopRightButtonImage;
                if (topRightButtonImage != null)
                {
                    topRightButtonImage.HorizontalOptions = LayoutOptions.EndAndExpand;
                    pictureControlStackLayout.Children.Add(topRightButtonImage);
                }

                StackLayout pictureMainStackLayout = new StackLayout();
                pictureMainStackLayout.BackgroundColor = config.PictureBackgroundColor;
                pictureMainStackLayout.Orientation = StackOrientation.Vertical;
                pictureMainStackLayout.Children.Add(pictureControlStackLayout);
                
                _pictureImage = config.PictureImage;
                PanGestureRecognizer pictureImagePanGesture = new PanGestureRecognizer();
                pictureImagePanGesture.PanUpdated += PanGesture_PanUpdated;
                _pictureImage.GestureRecognizers.Add(pictureImagePanGesture);

                pictureMainStackLayout.Children.Add(_pictureImage);

                Rectangle layoutBound = new Rectangle(1, 1, 1, 1);
                _pictureAbsoluteLayout.Children.Add(pictureMainStackLayout, layoutBound, AbsoluteLayoutFlags.All);
            }
        }
        #endregion
    }
}
