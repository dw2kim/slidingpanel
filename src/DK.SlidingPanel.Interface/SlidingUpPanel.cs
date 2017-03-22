using System;
using ReactiveUI;
using Xamarin.Forms;
using System.Reactive.Linq;

namespace DK.SlidingPanel.Interface
{
    public class SlidingUpPanel : AbsoluteLayout, ISlidingPanel
    {
        #region Constants
        private const double DEFAULT_PANEL_RATIO = 0.6;

        private const double DEFAULT_MIN_BODY_HEIGHT = 50;

        private const double DEFAULT_FAB_HEIGHT = 0;
        private const double DEFAULT_FAB_WIDTH = 0;

        private const double DEFAULT_NAV_BAR_HEIGHT_IOS = 39;
        private const double DEFAULT_NAV_BAR_HEIGHT_ANDROID = 34;
        #endregion

        #region Private Fields
        private bool _isCollapsing = false;
        private bool _isPanRunning = false;
        private SlidingPanelState _currentSlidePanelState = SlidingPanelState.Hidden;
        private double _lastYMovement = 0;
        private bool _showingNavBar = false;

        private bool _isFirst = true;

        private double _primaryFloatingActionButtonHeight = DEFAULT_FAB_HEIGHT;
        #endregion

        #region Private Properties
        private StackLayout _mainViewStackLayout { get; set; }
        private AbsoluteLayout _slidingPanelAbsoluteLayout { get; set; }

        private RelativeLayout _titleRelativeLayout { get; set; }
        private StackLayout _titleStackLayout { get; set; }
        private StackLayout _bodyStackLayout { get; set; }

        private View _primaryFloatingActionButton { get; set; }
        private View _secondaryFloatingActionButton { get; set; }

        private AbsoluteLayout _pictureAbsoluteLayout { get; set; }
        private StackLayout _pictureMainStackLayout { get; set; }
        private StackLayout _headerStackLayout { get; set; }
        private View _pictureImage { get; set; }

        private bool IsPictureImageNull
        {
            get
            {
                return (_pictureImage == null);
            }
        }
        private Func<int?> _functionAfterTitleTapped { get; set; }
        private bool _hideNavBarFeature { get; set; }

        private double NavigationBarHeight
        {
            get
            {
                double height = 0;
                Device.OnPlatform(iOS: () =>
                {
                    height = DEFAULT_NAV_BAR_HEIGHT_IOS;
                }, Android: () =>
                {
                    height = DEFAULT_NAV_BAR_HEIGHT_ANDROID;
                });

                return (height);
            }
        }
        #endregion

        #region Public Properties
        public static readonly BindableProperty HideNavBarProperty = BindableProperty.Create(
          propertyName: "HideNavBar",
          returnType: typeof(bool),
          declaringType: typeof(SlidingUpPanel),
          defaultValue: false);
        public bool HideNavBar
        {
            get { return (bool)GetValue(HideNavBarProperty); }
            set { SetValue(HideNavBarProperty, value); }
        }

        public static readonly BindableProperty PanelRatioProperty = BindableProperty.Create(
          propertyName: "PanelRatio",
          returnType: typeof(double),
          declaringType: typeof(SlidingUpPanel),
          defaultValue: DEFAULT_PANEL_RATIO);
        public double PanelRatio
        {
            get { return (double)GetValue(PanelRatioProperty); }
            set { SetValue(PanelRatioProperty, value); }
        }

        public static readonly BindableProperty MainViewProperty = BindableProperty.Create(
          propertyName: "MainView",
          returnType: typeof(View),
          declaringType: typeof(SlidingUpPanel),
          defaultValue: new StackLayout());
        public View MainView
        {
            get { return (View)GetValue(MainViewProperty); }
            set { SetValue(MainViewProperty, value); }
        }

        public static readonly BindableProperty TitleViewProperty = BindableProperty.Create(
          propertyName: "TitleView",
          returnType: typeof(View),
          declaringType: typeof(SlidingUpPanel),
          defaultValue: new StackLayout());
        public View TitleView
        {
            get { return (View)GetValue(TitleViewProperty); }
            set { SetValue(TitleViewProperty, value); }
        }

        public static readonly BindableProperty BodyViewProperty = BindableProperty.Create(
          propertyName: "BodyView",
          returnType: typeof(View),
          declaringType: typeof(SlidingUpPanel),
          defaultValue: new StackLayout());
        public View BodyView
        {
            get { return (View)GetValue(BodyViewProperty); }
            set { SetValue(BodyViewProperty, value); }
        }

        public static readonly BindableProperty HeaderLeftButtonProperty = BindableProperty.Create(
          propertyName: "HeaderLeftButton",
          returnType: typeof(View),
          declaringType: typeof(SlidingUpPanel),
          defaultValue: new Button());
        public View HeaderLeftButton
        {
            get { return (View)GetValue(HeaderLeftButtonProperty); }
            set { SetValue(HeaderLeftButtonProperty, value); }
        }

        public static readonly BindableProperty PictureViewProperty = BindableProperty.Create(
          propertyName: "PictureView",
          returnType: typeof(View),
          declaringType: typeof(SlidingUpPanel),
          defaultValue: new StackLayout());
        public View PictureView
        {
            get { return (View)GetValue(PictureViewProperty); }
            set { SetValue(PictureViewProperty, value); }
        }
        #endregion

        #region Constructors
        public SlidingUpPanel() : base()
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

            this.WhenAnyValue(x => x.HideNavBar)
                .Skip(1)
                .Subscribe(hideNavBar =>
                {
                    this._hideNavBarFeature = hideNavBar;
                });

            this.WhenAnyValue(x => x.MainView)
                .Skip(1)
                .Subscribe(mainView =>
                {
                    if (mainView != null)
                    {
                        _mainViewStackLayout.Children.Add(mainView);
                    }
                });

            this.WhenAnyValue(x => x.TitleView)
                .Skip(1)
                .Subscribe(titleView =>
                {
                    if (titleView != null)
                    {
                        _titleStackLayout = new StackLayout();
                        _titleStackLayout.BackgroundColor = titleView.BackgroundColor;
                        _titleStackLayout.Spacing = 0;
                        _titleStackLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
                        _titleStackLayout.VerticalOptions = LayoutOptions.Fill;
                        _titleRelativeLayout.Children.Add(_titleStackLayout,
                            xConstraint: Constraint.Constant(0),
                            yConstraint: Constraint.RelativeToParent((parent) =>
                            {
                                return (this._primaryFloatingActionButtonHeight / 2);
                            }),
                            widthConstraint: Constraint.RelativeToParent((parent) =>
                            {
                                return (parent.Width);
                            }),
                            heightConstraint: Constraint.Constant(titleView.HeightRequest + (this._primaryFloatingActionButtonHeight / 2)));
                        _titleRelativeLayout.HeightRequest = titleView.HeightRequest + (this._primaryFloatingActionButtonHeight / 2);

                        _titleStackLayout.Children.Add(titleView);
                        
                        TapGestureRecognizer titlePanelTapGesture = new TapGestureRecognizer();
                        titlePanelTapGesture.Tapped += TapGesture_Tapped;
                        titleView.GestureRecognizers.Add(titlePanelTapGesture);
                    }
                });


            this.WhenAnyValue(x => x.BodyView)
                .Skip(1)
                .Subscribe(bodyView =>
                {
                    if (bodyView != null)
                    {
                        _bodyStackLayout.BackgroundColor = bodyView.BackgroundColor;

                        _bodyStackLayout.Children.Add(bodyView);
                    }
                });


            this.WhenAnyValue(x => x.HeaderLeftButton)
                .Skip(1)
                .Subscribe(headerView =>
                {
                    if (headerView != null)
                    {
                        _headerStackLayout = new StackLayout();
                        _headerStackLayout.Orientation = StackOrientation.Horizontal;
                        _headerStackLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
                        _headerStackLayout.BackgroundColor = headerView.BackgroundColor;

                        TapGestureRecognizer headerTapGesture = new TapGestureRecognizer();
                        headerTapGesture.Tapped += TapGesture_Tapped;
                        _headerStackLayout.GestureRecognizers.Add(headerTapGesture);

                        _headerStackLayout.Children.Add(headerView);
                    }
                });
            

            this.WhenAnyValue(x => x.PictureView)
                .Skip(1)
                .Subscribe(pictureView =>
                {
                    if (pictureView != null)
                    {
                        _pictureMainStackLayout = new StackLayout();
                        _pictureMainStackLayout.Orientation = StackOrientation.Vertical;
                        _pictureMainStackLayout.BackgroundColor = pictureView.BackgroundColor;

                        if (_headerStackLayout != null)
                        {
                            _pictureMainStackLayout.Children.Add(_headerStackLayout);
                        }

                        _pictureImage = pictureView;
                        if (_pictureImage != null)
                        {
                            _pictureMainStackLayout.Children.Add(_pictureImage);
                        }

                        Rectangle layoutBound = new Rectangle(1, 1, 1, 1);
                        _pictureAbsoluteLayout.Children.Add(_pictureMainStackLayout, layoutBound, AbsoluteLayoutFlags.All);
                    }
                });


            this.WhenAnyValue(x => x.PanelRatio)
                .Skip(1)
                .Subscribe(panelRatio =>
                {
                    AbsoluteLayout.SetLayoutBounds(_pictureAbsoluteLayout, new Rectangle(1, 0, 1, (1.1 - panelRatio)));
                    AbsoluteLayout.SetLayoutBounds(_slidingPanelAbsoluteLayout, new Rectangle(1, 1, 1, panelRatio));
                });
        }
        public SlidingUpPanel(SlidingPanelConfig config) : this()
        {
            ApplyConfig(config);
        }
        #endregion

        #region Private Methods
        private void InitViews()
        {
            _mainViewStackLayout = new StackLayout();
            _mainViewStackLayout.Spacing = 0;
            AbsoluteLayout.SetLayoutBounds(_mainViewStackLayout, new Rectangle(1, 1, 1, 1));
            AbsoluteLayout.SetLayoutFlags(_mainViewStackLayout, AbsoluteLayoutFlags.All);
            this.Children.Add(_mainViewStackLayout);

            // Picture
            _pictureAbsoluteLayout = new AbsoluteLayout();
            AbsoluteLayout.SetLayoutBounds(_pictureAbsoluteLayout, new Rectangle(1, 0, 1, 1 - DEFAULT_PANEL_RATIO));
            AbsoluteLayout.SetLayoutFlags(_pictureAbsoluteLayout, AbsoluteLayoutFlags.All);
            this.Children.Add(_pictureAbsoluteLayout);
            
            // Drawer
            _slidingPanelAbsoluteLayout = new AbsoluteLayout();
            AbsoluteLayout.SetLayoutBounds(_slidingPanelAbsoluteLayout, new Rectangle(1, 1, 1, DEFAULT_PANEL_RATIO));
            AbsoluteLayout.SetLayoutFlags(_slidingPanelAbsoluteLayout, AbsoluteLayoutFlags.All);
            this.Children.Add(_slidingPanelAbsoluteLayout);
            
            StackLayout slidingPanelStackLayout = new StackLayout();
            slidingPanelStackLayout.Spacing = 0;
            Rectangle layoutBound = new Rectangle(1, 1, 1, 1);
            _slidingPanelAbsoluteLayout.Children.Add(slidingPanelStackLayout, layoutBound, AbsoluteLayoutFlags.All);

            // Title Section
            _titleRelativeLayout = new RelativeLayout();
            _titleRelativeLayout.Padding = new Thickness(0, 0, 0, 0);
            _titleRelativeLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
            _titleRelativeLayout.VerticalOptions = LayoutOptions.Fill;
            slidingPanelStackLayout.Children.Add(_titleRelativeLayout);
            
            // Body Section
            _bodyStackLayout = new StackLayout();
            _bodyStackLayout.Spacing = 0;
            _bodyStackLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
            _bodyStackLayout.VerticalOptions = LayoutOptions.FillAndExpand;
            _bodyStackLayout.MinimumHeightRequest = DEFAULT_MIN_BODY_HEIGHT;
            slidingPanelStackLayout.Children.Add(_bodyStackLayout);
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
            _primaryFloatingActionButtonHeight = (config != null && config.PrimaryFloatingActionButton != null && config.PrimaryFloatingActionButton.HeightRequest > 0) ?
                config.PrimaryFloatingActionButton.HeightRequest : DEFAULT_FAB_HEIGHT;

            _titleStackLayout = new StackLayout();
            _titleStackLayout.BackgroundColor = config.TitleBackgroundColor;
            _titleStackLayout.Spacing = 0;
            _titleStackLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
            _titleStackLayout.VerticalOptions = LayoutOptions.Fill;
            _titleRelativeLayout.Children.Add(_titleStackLayout,
                xConstraint: Constraint.Constant(0),
                yConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (this._primaryFloatingActionButtonHeight / 2);
                }),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Width);
                }),
                heightConstraint: Constraint.Constant(config.TitleHeightRequest + (this._primaryFloatingActionButtonHeight / 2)));
            _titleRelativeLayout.HeightRequest = config.TitleHeightRequest + (this._primaryFloatingActionButtonHeight / 2);

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

                double marginTop = (config.PrimaryFloatingActionButton != null) ? config.PrimaryFloatingActionButton.HeightRequest : 0;
                
                _titleRelativeLayout.Children.Add(_secondaryFloatingActionButton,
                    xConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Width - (_secondaryFloatingActionButton.WidthRequest * 1.5));
                    }),
                    yConstraint: Constraint.Constant(marginTop)
                );
            }

            if (config.TitleView != null)
            {
                _titleStackLayout.Children.Add(config.TitleView);
            }
            
            if (config.IsPanSupport == true)
            {
                PanGestureRecognizer bodyPanelPanGesture = new PanGestureRecognizer();
                bodyPanelPanGesture.PanUpdated += PanGesture_PanUpdated;
                _bodyStackLayout.GestureRecognizers.Add(bodyPanelPanGesture);
            }
            _bodyStackLayout.BackgroundColor = config.BodyBackgroundColor;

            if (config.BodyView != null)
            {
                _bodyStackLayout.Children.Add(config.BodyView);
            }
        }
        private void ApplyConfigToPicturePanel(SlidingPanelConfig config)
        {
            if (config.PictureImage != null)
            {
                _headerStackLayout = new StackLayout();
                _headerStackLayout.Orientation = StackOrientation.Horizontal;
                _headerStackLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
                _headerStackLayout.BackgroundColor = config.HeaderBackgroundColor;

                TapGestureRecognizer headerTapGesture = new TapGestureRecognizer();
                headerTapGesture.Tapped += TapGesture_Tapped;
                _headerStackLayout.GestureRecognizers.Add(headerTapGesture);

                if (config.IsPanSupport == true)
                {
                    PanGestureRecognizer headerPanGesture = new PanGestureRecognizer();
                    headerPanGesture.PanUpdated += PanGesture_PanUpdated;
                    _headerStackLayout.GestureRecognizers.Add(headerPanGesture);
                }


                View topLeftButton = config.HeaderLeftButton;
                if (topLeftButton != null)
                {
                    _headerStackLayout.Children.Add(topLeftButton);
                }

                View topRightButton = config.HeaderRightButton;
                if (topRightButton != null)
                {
                    topRightButton.HorizontalOptions = LayoutOptions.EndAndExpand;
                    _headerStackLayout.Children.Add(topRightButton);
                }

                _pictureMainStackLayout = new StackLayout();
                _pictureMainStackLayout.Orientation = StackOrientation.Vertical;
                _pictureMainStackLayout.BackgroundColor = config.PictureBackgroundColor;
                _pictureMainStackLayout.Children.Add(_headerStackLayout);

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

        private void CollapseOrExpand(bool isAfterTapped)
        {
            if (_lastYMovement > 0)
            {
                ShowCollapsedPanel();
            }
            else
            {
                ShowExpandedPanel();
            }
        }

        private void ShowNavigationBar(bool showNavBarNow)
        {
            if (_hideNavBarFeature == true && this.Parent != null)
            {
                if (showNavBarNow == false && NavigationPage.GetHasNavigationBar(this.Parent) == true)
                {
                    Device.OnPlatform(iOS: () =>
                    {
                        this.Padding = new Thickness(0, 20, 0, 0);
                    });
                    NavigationPage.SetHasNavigationBar(this.Parent, false);
                }
                
                if (showNavBarNow == true && NavigationPage.GetHasNavigationBar(this.Parent) == false)
                {
                    Device.OnPlatform(iOS: () => {
                        this.Padding = new Thickness(0, 0, 0, 0);
                    });
                    NavigationPage.SetHasNavigationBar(this.Parent, true);
                }
            }
        }
        #endregion

        #region Gesture Implementations
        private void TapGesture_Tapped(object sender, EventArgs e)
        {
            if (_isPanRunning == false)
            {
                switch (_currentSlidePanelState)
                {
                    case SlidingPanelState.Collapsed:
                        ShowExpandedPanel();
                        break;
                    case SlidingPanelState.Expanded:
                        //_showingNavBar = true;
                        ShowCollapsedPanel();
                        break;
                    case SlidingPanelState.Hidden:
                    default:
                        break;
                }
            }

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
            if (_isPanRunning == true)
            {
                _isPanRunning = false;
                CollapseOrExpand(true);
            }
        }
        private void TapGesture_Tapped_iOS(object sender, EventArgs e)
        {
            if (_isPanRunning == true)
            {
            }
        }

        private void PanGesture_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Running)
            {
                double totalY = e.TotalY;
                _lastYMovement = totalY;

                if ((_currentSlidePanelState == SlidingPanelState.Expanded && totalY > 0) || (_currentSlidePanelState == SlidingPanelState.Collapsed && totalY < 0))
                {
                    _isCollapsing = (totalY > 0);
                    _isPanRunning = true;

                    double newDrawerPosition = CalculateNewDrawerPositionY(totalY);
                    _slidingPanelAbsoluteLayout.TranslateTo(0, newDrawerPosition, 250, Easing.CubicOut);

                    if (IsPictureImageNull == false)
                    {
                        double newPicturePosition = CalculateNewPicturePositionY(totalY);
                        _pictureAbsoluteLayout.TranslateTo(0, newPicturePosition, 250, Easing.CubicOut);
                    }

                    bool showNavBarNow = (_isCollapsing == true);
                    ShowNavigationBar(showNavBarNow);
                }
            }

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
            if (e.StatusType == GestureStatus.Completed)
            {
                if (_isCollapsing == true)
                {
                    _isPanRunning = false;
                    CollapseOrExpand(false);
                }
            }
        }
        private void PanGesture_PanUpdated_iOS(object sender, PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Completed)
            {
                _isPanRunning = false;
                CollapseOrExpand(false);
            }
        }
        #endregion

        #region ISlidingPanel Implementations
        public void HidePanel(uint length = 700)
        {
            Rectangle drawerCollapsedPosition = _slidingPanelAbsoluteLayout.Bounds;
            drawerCollapsedPosition.Y = _slidingPanelAbsoluteLayout.Height + (this._primaryFloatingActionButtonHeight / 2);
            if (_hideNavBarFeature == true && _showingNavBar == true)
            {
                drawerCollapsedPosition.Y -= NavigationBarHeight;
                _showingNavBar = false;
            }

            _slidingPanelAbsoluteLayout.TranslateTo(drawerCollapsedPosition.X, drawerCollapsedPosition.Y, length, Easing.CubicOut);
            _currentSlidePanelState = SlidingPanelState.Hidden;
            
            Rectangle pictureBounds = _pictureAbsoluteLayout.Bounds;
            pictureBounds.Y = drawerCollapsedPosition.Y + _pictureAbsoluteLayout.Height;
            _pictureAbsoluteLayout.TranslateTo(pictureBounds.X, pictureBounds.Y, length, Easing.CubicOut);
        }
        public void ShowCollapsedPanel(uint length = 700)
        {
            ShowNavigationBar(true);

            var actualHeight = _titleRelativeLayout.Height;
            Rectangle drawerCollapsedPosition = _slidingPanelAbsoluteLayout.Bounds;
            drawerCollapsedPosition.Y = _slidingPanelAbsoluteLayout.Height - actualHeight;
            if (_hideNavBarFeature == true && _showingNavBar == true && _isCollapsing == false)
            {
                drawerCollapsedPosition.Y -= NavigationBarHeight;
                _showingNavBar = false;
            }
            _slidingPanelAbsoluteLayout.TranslateTo(drawerCollapsedPosition.X, drawerCollapsedPosition.Y, length, Easing.CubicOut);
            _currentSlidePanelState = SlidingPanelState.Collapsed;

            if (IsPictureImageNull == false)
            {
                Rectangle pictureBounds = _pictureAbsoluteLayout.Bounds;
                pictureBounds.Y = drawerCollapsedPosition.Y + _pictureAbsoluteLayout.Height + (this._primaryFloatingActionButtonHeight / 2);
                _pictureAbsoluteLayout.TranslateTo(pictureBounds.X, pictureBounds.Y, length, Easing.CubicOut);
            }
        }
        public void ShowExpandedPanel(uint length = 700)
        {
            ShowNavigationBar(false);
            _showingNavBar = true;

            var actualHeight = _titleRelativeLayout.Height;
            Rectangle drawerExpandedPosition = _slidingPanelAbsoluteLayout.Bounds;
            drawerExpandedPosition.Y = 0;

            _slidingPanelAbsoluteLayout.TranslateTo(drawerExpandedPosition.X, drawerExpandedPosition.Y, length, Easing.CubicOut);
            _currentSlidePanelState = SlidingPanelState.Expanded;

            if (IsPictureImageNull == false)
            {
                Rectangle pictureExpandedPosition = _pictureAbsoluteLayout.Bounds;
                pictureExpandedPosition.Y = 0;
                _pictureAbsoluteLayout.TranslateTo(pictureExpandedPosition.X, pictureExpandedPosition.Y, length, Easing.CubicOut);
            }
        }

        public void ApplyConfig(SlidingPanelConfig config)
        {
            _hideNavBarFeature = config.HideNavBar;

            AbsoluteLayout.SetLayoutBounds(_pictureAbsoluteLayout, new Rectangle(1, 0, 1, (1.1 - config.PanelRatio)));
            AbsoluteLayout.SetLayoutBounds(_slidingPanelAbsoluteLayout, new Rectangle(1, 1, 1, config.PanelRatio));

            _mainViewStackLayout.Children.Add(config.MainView);

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
