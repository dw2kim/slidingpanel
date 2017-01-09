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
        private const double DefaultPanelRatio = 0.5;

        private const double TitleMinimumHeight = 50;
        private const double BodyMinimumHeight = 50;

        private const double DefaultFloatingActionButtonHeight = 0;
        private const double DefaultFloatingActionButtonWidth = 0;

        private const double DefaultFloatingButtonSpaceFactor = 1.25;
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

        private Image _primaryFloatingActionButton { get; set; }
        private Image _secondaryFloatingActionButton { get; set; }

        private AbsoluteLayout _pictureAbsoluteLayout { get; set; }
        private StackLayout _pictureMainStackLayout { get; set; }
        private Image _pictureImage { get; set; }

        private Func<int?> _functionAfterTitleTapped { get; set; }

        private bool IsPrimaryFloatingActionButtonNull
        {
            get
            {
                return (_primaryFloatingActionButton == null || _primaryFloatingActionButton.Source == null);
            }
        }
        private bool IsPictureImageNull
        {
            get
            {
                return (_pictureImage == null || _pictureImage.Source == null);
            }
        }
        private double PrimaryFloatingActionButtonHeight
        {
            get
            {
                double primaryFloatingActionButtonHeight = (_config?.PrimaryFloatingActionButtonHeight > 0) ? _config.PrimaryFloatingActionButtonHeight : DefaultFloatingActionButtonHeight;
                return (primaryFloatingActionButtonHeight);
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
            AbsoluteLayout.SetLayoutBounds(_pictureAbsoluteLayout, new Rectangle(1, 0, 1, DefaultPanelRatio));
            AbsoluteLayout.SetLayoutFlags(_pictureAbsoluteLayout, AbsoluteLayoutFlags.All);
            this.Children.Add(_pictureAbsoluteLayout);
            
            // Drawer
            _slidingPanelAbsoluteLayout = new AbsoluteLayout();
            AbsoluteLayout.SetLayoutBounds(_slidingPanelAbsoluteLayout, new Rectangle(1, 1, 1, DefaultPanelRatio));
            AbsoluteLayout.SetLayoutFlags(_slidingPanelAbsoluteLayout, AbsoluteLayoutFlags.All);
            this.Children.Add(_slidingPanelAbsoluteLayout);
            
            // Drawer
            StackLayout slSlidingPanel = new StackLayout();
            slSlidingPanel.Spacing = 0;
            Rectangle layoutBound = new Rectangle(1, 1, 1, 1);
            _slidingPanelAbsoluteLayout.Children.Add(slSlidingPanel, layoutBound, AbsoluteLayoutFlags.All);

            // Title Section
            _titleRelativeLayout = new RelativeLayout();
            _titleRelativeLayout.Padding = new Thickness(0, 0, 0, 0);
            _titleRelativeLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
            _titleRelativeLayout.VerticalOptions = LayoutOptions.Fill;
            _titleRelativeLayout.BackgroundColor = Color.Transparent;
            slSlidingPanel.Children.Add(_titleRelativeLayout);
            
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
            titlePanelTapGesture.Tapped += TapGesture_Tapped;
            _titleStackLayout.GestureRecognizers.Add(titlePanelTapGesture);
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

        private void ApplyConfigToTitleBodyPanel(SlidingPanelConfig config)
        {
            _titleStackLayout = new StackLayout();
            _titleStackLayout.BackgroundColor = config.TitleBackgroundColor;
            _titleStackLayout.Spacing = 0;
            _titleStackLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
            _titleStackLayout.VerticalOptions = LayoutOptions.Fill;
            _titleRelativeLayout.Children.Add(_titleStackLayout,
                xConstraint: Constraint.Constant(0),
                yConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (this.PrimaryFloatingActionButtonHeight / 2);
                }),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Width);
                }),
                heightConstraint: Constraint.Constant(config.TitleHeightRequest + (this.PrimaryFloatingActionButtonHeight / 2)));
            _titleRelativeLayout.HeightRequest = config.TitleHeightRequest + (this.PrimaryFloatingActionButtonHeight / 2);

            if (config.IsPanSupport == true)
            {
                PanGestureRecognizer titlePanelPanGesture = new PanGestureRecognizer();
                titlePanelPanGesture.PanUpdated += PanGesture_PanUpdated;
                _titleStackLayout.GestureRecognizers.Add(titlePanelPanGesture);
            }

            // PrimaryFloatingActionButton section
            if (config.PrimaryFloatingActionButton != null)
            {
                _primaryFloatingActionButton = config.PrimaryFloatingActionButton;
                _primaryFloatingActionButton.WidthRequest = (config.PrimaryFloatingActionButtonWidth > 0) ? config.PrimaryFloatingActionButtonWidth : DefaultFloatingActionButtonWidth;
                _primaryFloatingActionButton.HeightRequest = (config.PrimaryFloatingActionButtonHeight > 0) ? config.PrimaryFloatingActionButtonHeight : DefaultFloatingActionButtonHeight;

                if (config.PrimaryFloatingActionButton_TapGesture_Tapped != null)
                {
                    TapGestureRecognizer primaryFloatingActionButton_TapGesture = new TapGestureRecognizer();
                    primaryFloatingActionButton_TapGesture.Tapped += config.PrimaryFloatingActionButton_TapGesture_Tapped;
                    _primaryFloatingActionButton.GestureRecognizers.Add(primaryFloatingActionButton_TapGesture);
                }

                _titleRelativeLayout.Children.Add(_primaryFloatingActionButton,
                    xConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Width - (_primaryFloatingActionButton.WidthRequest * 1.5));
                    }),
                    yConstraint: Constraint.Constant(0)
                );
            }

            // SecondaryFloatingActionButton section
            if (config.SecondaryFloatingActionButton != null)
            {
                _secondaryFloatingActionButton = config.SecondaryFloatingActionButton;
                _secondaryFloatingActionButton.WidthRequest = (config.SecondaryFloatingActionButtonWidth > 0) ? config.SecondaryFloatingActionButtonWidth : DefaultFloatingActionButtonWidth;
                _secondaryFloatingActionButton.HeightRequest = (config.SecondaryFloatingActionButtonHeight > 0) ? config.SecondaryFloatingActionButtonHeight : DefaultFloatingActionButtonHeight;

                if (config.SecondaryFloatingActionButton_TapGesture_Tapped != null)
                {
                    TapGestureRecognizer secondaryFloatingActionButton_TapGesture = new TapGestureRecognizer();
                    secondaryFloatingActionButton_TapGesture.Tapped += config.SecondaryFloatingActionButton_TapGesture_Tapped;
                    _secondaryFloatingActionButton.GestureRecognizers.Add(secondaryFloatingActionButton_TapGesture);
                }
                
                //double yConstraint = (config.SecondaryFloatingActionButtonHeight > 0) ? (config.SecondaryFloatingActionButtonHeight * DefaultFloatingButtonSpaceFactor) : DefaultFloatingActionButtonHeight;
                _titleRelativeLayout.Children.Add(_secondaryFloatingActionButton,
                    xConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Width - (_secondaryFloatingActionButton.WidthRequest * 1.5));
                    }),
                    yConstraint: Constraint.Constant(config.SecondaryFloatingActionButtonMarginTop)
                );
            }


            _titleStackLayout.Children.Add(config.TitleView);
            
            if (config.IsPanSupport == true)
            {
                PanGestureRecognizer bodyPanelPanGesture = new PanGestureRecognizer();
                bodyPanelPanGesture.PanUpdated += PanGesture_PanUpdated;
                _bodyStackLayout.GestureRecognizers.Add(bodyPanelPanGesture);
            }
            _bodyStackLayout.BackgroundColor = config.BodyBackgroundColor;
            _bodyStackLayout.Children.Add(config.BodyView);
        }
        private void ApplyConfigToPicturePanel(SlidingPanelConfig config)
        {
            if (config.PictureImage != null)
            {
                StackLayout pictureControlStackLayout = new StackLayout();
                pictureControlStackLayout.Orientation = StackOrientation.Horizontal;
                pictureControlStackLayout.HorizontalOptions = LayoutOptions.FillAndExpand;

                TapGestureRecognizer pictureControlTapGesture = new TapGestureRecognizer();
                pictureControlTapGesture.Tapped += TapGesture_Tapped;
                pictureControlStackLayout.GestureRecognizers.Add(pictureControlTapGesture);

                if (config.IsPanSupport == true)
                {
                    PanGestureRecognizer pictureControlPanGesture = new PanGestureRecognizer();
                    pictureControlPanGesture.PanUpdated += PanGesture_PanUpdated;
                    pictureControlStackLayout.GestureRecognizers.Add(pictureControlPanGesture);
                }


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

                _pictureMainStackLayout = new StackLayout();
                _pictureMainStackLayout.BackgroundColor = config.PictureBackgroundColor;
                _pictureMainStackLayout.Orientation = StackOrientation.Vertical;
                _pictureMainStackLayout.Children.Add(pictureControlStackLayout);

                _pictureImage = config.PictureImage;
                if (config.IsPanSupport == true)
                {
                    PanGestureRecognizer pictureImagePanGesture = new PanGestureRecognizer();
                    pictureImagePanGesture.PanUpdated += PanGesture_PanUpdated;
                    _pictureImage.GestureRecognizers.Add(pictureImagePanGesture);
                }

                _pictureMainStackLayout.Children.Add(_pictureImage);

                Rectangle layoutBound = new Rectangle(1, 1, 1, 1);
                _pictureAbsoluteLayout.Children.Add(_pictureMainStackLayout, layoutBound, AbsoluteLayoutFlags.All);
            }
        }

        private void CollapseOrExpandSmartly()
        {
            double minDrawerPosition = 0;
            double maxDrawerPosition = _slidingPanelAbsoluteLayout.Height - _titleRelativeLayout.Height;

            double midDrawerPosition = maxDrawerPosition / 2;
            double topMidDrawerPosition = maxDrawerPosition / 10;
            double bottomMidDrawerPosition = maxDrawerPosition * 9 / 10;

            double currentPosition = _slidingPanelAbsoluteLayout.TranslationY;

            if (currentPosition >= minDrawerPosition && currentPosition < topMidDrawerPosition)
            {
                ShowExpandedPanel();
            }
            if (currentPosition >= topMidDrawerPosition && currentPosition < midDrawerPosition)
            {
                ShowCollapsedPanel();
            }
            if (currentPosition >= midDrawerPosition && currentPosition < bottomMidDrawerPosition)
            {
                ShowExpandedPanel();
            }
            if (currentPosition >= bottomMidDrawerPosition && currentPosition < maxDrawerPosition)
            {
                ShowCollapsedPanel();
            }
        }
        #endregion

        #region Gesture Implementations
        private void TapGesture_Tapped(object sender, EventArgs e)
        {

            if (Device.OS == TargetPlatform.Android)
            {
                TapGesture_Tapped_Android(sender, e);
            }

            if (Device.OS == TargetPlatform.iOS)
            {
                TapGesture_Tapped_iOS(sender, e);
            }
            
            //FunctionAfterTitleTapped();
        }
        private void TapGesture_Tapped_Android(object sender, EventArgs e)
        {
            if (_isPanRunning == true)
            {
                _isPanRunning = false;

                CollapseOrExpandSmartly();
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
        private void TapGesture_Tapped_iOS(object sender, EventArgs e)
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

                if ((_isCurrentlyCollapsed == false && totalY > 0) || (_isCurrentlyCollapsed == true && totalY < 0))
                {
                    _isCollapsing = (totalY > 0);
                    _isPanRunning = true;

                    double newDrawerPosition = CalculateNewDrawerPositionY(totalY);
                    _slidingPanelAbsoluteLayout.TranslateTo(0, newDrawerPosition, 250, Easing.CubicOut);
                    //_slidingPanelAbsoluteLayout.TranslationY = newDrawerPosition;

                    if (IsPictureImageNull == false)
                    {
                        double newPicturePosition = CalculateNewPicturePositionY(totalY);
                        _pictureAbsoluteLayout.TranslateTo(0, newPicturePosition, 250, Easing.CubicOut);
                        //_pictureAbsoluteLayout.TranslationY = newPicturePosition;
                    }
                }
            }

            if (e.StatusType == GestureStatus.Completed)
            {
                if (_isCollapsing == true)
                {
                    _isPanRunning = false;
                    CollapseOrExpandSmartly();
                }
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

                CollapseOrExpandSmartly();
            }
        }
        #endregion

        #region ISlidingPanel Implementations
        public void HidePanel(uint length = 700)
        {
            Rectangle drawerCollapsedPosition = _slidingPanelAbsoluteLayout.Bounds;
            drawerCollapsedPosition.Y = _slidingPanelAbsoluteLayout.Height + (this.PrimaryFloatingActionButtonHeight / 2); 

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
                pictureBounds.Y = drawerCollapsedPosition.Y + _pictureAbsoluteLayout.Height + (this.PrimaryFloatingActionButtonHeight / 2); 

                _pictureAbsoluteLayout.TranslateTo(pictureBounds.X, pictureBounds.Y, length, Easing.CubicOut);
            }

            _titleRelativeLayout.BackgroundColor = Color.Transparent;
        }
        public void ShowExpandedPanel(uint length = 700)
        {
            if (IsPictureImageNull == false)
            {
                _titleRelativeLayout.BackgroundColor = _pictureMainStackLayout.BackgroundColor;
            }

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

            AbsoluteLayout.SetLayoutBounds(_pictureAbsoluteLayout, new Rectangle(1, 0, 1, (1- config.PanelRatio)));
            AbsoluteLayout.SetLayoutBounds(_slidingPanelAbsoluteLayout, new Rectangle(1, 1, 1, config.PanelRatio));

            _mainStackLayout.Children.Add(config.MainView);

            ApplyConfigToTitleBodyPanel(config);

            ApplyConfigToPicturePanel(config);

            if (config.IsExpandable == true)
            {
                InitGestures();
            }
        }
        #endregion
    }
}
