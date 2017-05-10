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
        private const double DEFAULT_NAV_BAR_HEIGHT_ANDROID = 33;

        private const double DEFAULT_IOS_STATUS_BAR_HEIGHT = 20;
        private const double DEFAULT_ANDROID_STATUS_BAR_HEIGHT = 18;

        private const double MIN_PANEL_RATIO = 0;
        private const double MAX_PANEL_RATIO = 1;

        private const int DEFAULT_SLIDE_ANIMATION_SPEED = 700;

        private const double PICTURE_PANEL_Y_FACTOR_WITH_PICTURE = 2;
        private const double PICTURE_PANEL_Y_FACTOR_WITHOUT_PICTURE = 1.1;
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
        private double StatusBarHeight
        {
            get
            {
                double height = 0;
                Device.OnPlatform(iOS: () =>
                {
                    height = DEFAULT_IOS_STATUS_BAR_HEIGHT;
                }, Android: () =>
                {
                    height = DEFAULT_ANDROID_STATUS_BAR_HEIGHT;
                });

                return (height);
            }
        }


        private double _currentTitleHeight { get; set; }
        private bool _hideTitleView { get; set; }
        private bool _isExpandable { get; set; } = true;
        private bool _isPanSupport { get; set; } = false;
        private Color _currentTitleBackground { get; set; } = Color.Transparent;
        private Color _currentBodyBackground { get; set; } = Color.Transparent;

        private int _slideAnimationSpeed = DEFAULT_SLIDE_ANIMATION_SPEED;
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

        public static readonly BindableProperty HideTitleViewProperty = BindableProperty.Create(
          propertyName: "HideTitleView",
          returnType: typeof(bool),
          declaringType: typeof(SlidingUpPanel),
          defaultValue: false);
        public bool HideTitleView
        {
            get { return (bool)GetValue(HideTitleViewProperty); }
            set { SetValue(HideTitleViewProperty, value); }
        }
        
        public static readonly BindableProperty IsExpandableProperty = BindableProperty.Create(
          propertyName: "IsExpandable",
          returnType: typeof(bool),
          declaringType: typeof(SlidingUpPanel),
          defaultValue: true);
        public bool IsExpandable
        {
            get { return (bool)GetValue(IsExpandableProperty); }
            set { SetValue(IsExpandableProperty, value); }
        }


        public static readonly BindableProperty IsPanSupportProperty = BindableProperty.Create(
          propertyName: "IsPanSupport",
          returnType: typeof(bool),
          declaringType: typeof(SlidingUpPanel),
          defaultValue: false);
        public bool IsPanSupport
        {
            get { return (bool)GetValue(IsPanSupportProperty); }
            set { SetValue(IsPanSupportProperty, value); }
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
        
        public static readonly BindableProperty SlideAnimationSpeedProperty = BindableProperty.Create(
          propertyName: "SlideAnimationSpeed",
          returnType: typeof(int),
          declaringType: typeof(SlidingUpPanel),
          defaultValue: DEFAULT_SLIDE_ANIMATION_SPEED);
        public int SlideAnimationSpeed
        {
            get { return (int)GetValue(SlideAnimationSpeedProperty); }
            set { SetValue(SlideAnimationSpeedProperty, value); }
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

        public static readonly BindableProperty HeaderViewProperty = BindableProperty.Create(
          propertyName: "HeaderView",
          returnType: typeof(View),
          declaringType: typeof(SlidingUpPanel),
          defaultValue: new StackLayout());
        public View HeaderView
        {
            get { return (View)GetValue(HeaderViewProperty); }
            set { SetValue(HeaderViewProperty, value); }
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

        public event EventHandler<StateChangedEventArgs> WhenSlidingPanelStateChanged;
        public event EventHandler<StateChangingEventArgs> WhenSlidingPanelStateChanging;
        public event EventHandler WhenPanelRatioChanged;
        #endregion

        #region Constructors
        public SlidingUpPanel() : base()
        {
            InitViews();

            this._slidingPanelAbsoluteLayout.WhenAnyValue(x => x.Height)
                .Subscribe(actualHeight =>
                {
                    if (_isFirst == true && actualHeight > -1)
                    {
                        HidePanel(0);
                        _isFirst = false;
                    }
                });

            this.WhenAnyValue(x => x.SlideAnimationSpeed)
                .Skip(1)
                .Subscribe(slideAnimationSpeed =>
                {
                    this._slideAnimationSpeed = slideAnimationSpeed;
                });


            this.WhenAnyValue(x => x.HideTitleView)
                .Skip(1)
                .Subscribe(hideTitleView =>
                {
                    _hideTitleView = hideTitleView;
                });

            this.WhenAnyValue(x => x.HideNavBar)
                .Skip(1)
                .Subscribe(hideNavBar =>
                {
                    this._hideNavBarFeature = hideNavBar;
                });
            
            this.WhenAnyValue(x => x.IsExpandable)
                .Skip(1)
                .Subscribe(isExpandable =>
                {
                    _isExpandable = isExpandable;
                });


            this.WhenAnyValue(x => x.IsPanSupport)
                .Skip(1)
                .Subscribe(isPanSupport =>
                {
                    _isPanSupport = isPanSupport;
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
                        _currentTitleBackground = titleView.BackgroundColor;
                        _titleStackLayout.BackgroundColor = _currentTitleBackground;
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
                        _currentTitleHeight = titleView.HeightRequest + (this._primaryFloatingActionButtonHeight / 2);
                        _titleRelativeLayout.HeightRequest = _currentTitleHeight;

                        _titleStackLayout.Children.Add(titleView);
                        
                        TapGestureRecognizer titlePanelTapGesture = new TapGestureRecognizer();
                        titlePanelTapGesture.Tapped += TapGesture_Tapped;
                        titleView.GestureRecognizers.Add(titlePanelTapGesture);

                        PanGestureRecognizer titlePanelPanGesture = new PanGestureRecognizer();
                        titlePanelPanGesture.PanUpdated += PanGesture_PanUpdated;
                        titleView.GestureRecognizers.Add(titlePanelPanGesture);
                    }
                });


            this.WhenAnyValue(x => x.BodyView)
                .Skip(1)
                .Subscribe(bodyView =>
                {
                    if (bodyView != null)
                    {
                        _currentBodyBackground = bodyView.BackgroundColor;
                        _bodyStackLayout.BackgroundColor = _currentBodyBackground;

                        PanGestureRecognizer bodyPanelPanGesture = new PanGestureRecognizer();
                        bodyPanelPanGesture.PanUpdated += PanGesture_PanUpdated;
                        _bodyStackLayout.GestureRecognizers.Add(bodyPanelPanGesture);

                        _bodyStackLayout.Children.Add(bodyView);
                    }
                });


            this.WhenAnyValue(x => x.HeaderView)
                .Skip(1)
                .Subscribe(headerView =>
                {
                    if (headerView != null)
                    {
                        _headerStackLayout = new StackLayout();
                        _headerStackLayout.Orientation = StackOrientation.Horizontal;
                        _headerStackLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
                        _headerStackLayout.BackgroundColor = headerView.BackgroundColor;
                        _headerStackLayout.HeightRequest = headerView.HeightRequest;

                        TapGestureRecognizer headerTapGesture = new TapGestureRecognizer();
                        headerTapGesture.Tapped += TapGesture_Tapped;
                        _headerStackLayout.GestureRecognizers.Add(headerTapGesture);

                        _headerStackLayout.Children.Add(headerView);

                        PanGestureRecognizer headerPanGesture = new PanGestureRecognizer();
                        headerPanGesture.PanUpdated += PanGesture_PanUpdated;
                        _headerStackLayout.GestureRecognizers.Add(headerPanGesture);
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

                        PanGestureRecognizer pictureImagePanGesture = new PanGestureRecognizer();
                        pictureImagePanGesture.PanUpdated += PanGesture_PanUpdated;
                        _pictureImage.GestureRecognizers.Add(pictureImagePanGesture);
                    }
                });
            

            this.WhenAnyValue(x => x.PanelRatio)
                .Skip(1)
                .Subscribe(panelRatio =>
                {
                    if (panelRatio >= MAX_PANEL_RATIO)
                    {
                        double picturePatio = 0;
                        if (_headerStackLayout != null)
                        {
                            //var totalHeight = (_pictureAbsoluteLayout?.Bounds.Height ?? 0) + (_slidingPanelAbsoluteLayout?.Bounds.Height ?? 0);
                            var totalHeight = Application.Current.MainPage.Height;
                            picturePatio = _headerStackLayout.Height / totalHeight;
                        }

                        AbsoluteLayout.SetLayoutBounds(_pictureAbsoluteLayout, new Rectangle(1, 0, 1, picturePatio));
                        AbsoluteLayout.SetLayoutBounds(_slidingPanelAbsoluteLayout, new Rectangle(1, 1, 1, MAX_PANEL_RATIO - picturePatio));

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            _slidingPanelAbsoluteLayout.TranslationY = _slidingPanelAbsoluteLayout.Height;

                            _titleStackLayout.BackgroundColor = _currentTitleBackground;
                            _bodyStackLayout.BackgroundColor = _currentBodyBackground;

                            WhenPanelRatioChanged?.Invoke(null, null);
                        });
                    }
                    
                    if(panelRatio > MIN_PANEL_RATIO && panelRatio < MAX_PANEL_RATIO)
                    {
                        AbsoluteLayout.SetLayoutBounds(_pictureAbsoluteLayout, new Rectangle(1, 0, 1, (MAX_PANEL_RATIO - panelRatio)));
                        AbsoluteLayout.SetLayoutBounds(_slidingPanelAbsoluteLayout, new Rectangle(1, 1, 1, panelRatio));

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            _slidingPanelAbsoluteLayout.TranslationY = _slidingPanelAbsoluteLayout.Height;

                            _titleStackLayout.BackgroundColor = _currentTitleBackground;
                            _bodyStackLayout.BackgroundColor = _currentBodyBackground;

                            WhenPanelRatioChanged?.Invoke(null, null);
                        });
                    }
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
            // Main
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

            double totalPictureYFactor = (this.PanelRatio >= MAX_PANEL_RATIO) ? PICTURE_PANEL_Y_FACTOR_WITHOUT_PICTURE : PICTURE_PANEL_Y_FACTOR_WITH_PICTURE;
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

        private void CollapseOrExpand()
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
                        this.Padding = new Thickness(0, DEFAULT_IOS_STATUS_BAR_HEIGHT, 0, 0);
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
            else if (Device.OS == TargetPlatform.iOS)
            {
                TapGesture_Tapped_iOS(sender, e);
            }
        }
        private void TapGesture_Tapped_Android(object sender, EventArgs e)
        {
            if (_isPanRunning == true)
            {
                _isPanRunning = false;
                CollapseOrExpand();
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
            if (_isPanSupport == true)
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
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            _slidingPanelAbsoluteLayout.TranslateTo(0, newDrawerPosition, 250, Easing.CubicOut);
                        });

                        if (_pictureImage != null)
                        {
                            double newPicturePosition = CalculateNewPicturePositionY(totalY);

                            Device.BeginInvokeOnMainThread(() =>
                            {
                                _pictureAbsoluteLayout.TranslateTo(0, newPicturePosition, 250, Easing.CubicOut);
                            });
                        }

                        //bool showNavBarNow = (_isCollapsing == true); // show/hide nav bar as swiped (not on completed)
                        //ShowNavigationBar(showNavBarNow);
                    }
                }

                if (Device.OS == TargetPlatform.Android)
                {
                    PanGesture_PanUpdated_Android(sender, e);
                }
                else if (Device.OS == TargetPlatform.iOS)
                {
                    PanGesture_PanUpdated_iOS(sender, e);
                }
            }
        }
        private void PanGesture_PanUpdated_Android(object sender, PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Completed)
            {
                if (_isCollapsing == true)
                {
                    _isPanRunning = false;
                    CollapseOrExpand();
                    _isCollapsing = false;
                }
            }
        }
        private void PanGesture_PanUpdated_iOS(object sender, PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Completed)
            {
                _isPanRunning = false;
                CollapseOrExpand();
                _isCollapsing = false;
            }
        }
        #endregion

        #region ISlidingPanel Implementations
        public SlidingPanelState CurrentState
        {
            get
            {
                return (_currentSlidePanelState);
            }
        }

        public void HidePanel(uint length = DEFAULT_SLIDE_ANIMATION_SPEED)
        {
            WhenSlidingPanelStateChanging?.Invoke(null, new StateChangingEventArgs() { OldState = _currentSlidePanelState, NewState = SlidingPanelState.Hidden });

            Rectangle drawerCollapsedPosition = _slidingPanelAbsoluteLayout.Bounds;
            drawerCollapsedPosition.Y = _slidingPanelAbsoluteLayout.Height + (this._primaryFloatingActionButtonHeight / 2);
            if (_hideNavBarFeature == true && _showingNavBar == true)
            {
                drawerCollapsedPosition.Y -= NavigationBarHeight;
                _showingNavBar = false;
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                length = (length == DEFAULT_SLIDE_ANIMATION_SPEED) ? (uint)_slideAnimationSpeed : length;
                _slidingPanelAbsoluteLayout.TranslateTo(drawerCollapsedPosition.X, drawerCollapsedPosition.Y, length, Easing.CubicOut);
            });
            _currentSlidePanelState = SlidingPanelState.Hidden;

            Rectangle pictureBounds = _pictureAbsoluteLayout.Bounds;
            pictureBounds.Y = drawerCollapsedPosition.Y + _pictureAbsoluteLayout.Height;
            Device.BeginInvokeOnMainThread(() =>
            {
                length = (length == DEFAULT_SLIDE_ANIMATION_SPEED) ? (uint)_slideAnimationSpeed : length;
                _pictureAbsoluteLayout.TranslateTo(pictureBounds.X, pictureBounds.Y, length, Easing.CubicOut);
            });

            WhenSlidingPanelStateChanged?.Invoke(null, new StateChangedEventArgs() { State = _currentSlidePanelState });
        }
        public void ShowCollapsedPanel(uint length = DEFAULT_SLIDE_ANIMATION_SPEED)
        {
            WhenSlidingPanelStateChanging?.Invoke(null, new StateChangingEventArgs() { OldState = _currentSlidePanelState, NewState = SlidingPanelState.Collapsed });

            ShowNavigationBar(true);

            if (_hideTitleView)
                _titleRelativeLayout.HeightRequest = _currentTitleHeight;
            
            Rectangle drawerCollapsedPosition = _slidingPanelAbsoluteLayout.Bounds;
            drawerCollapsedPosition.Y = _slidingPanelAbsoluteLayout.Height + (this._primaryFloatingActionButtonHeight / 2);
            drawerCollapsedPosition.Y -= _currentTitleHeight;
            if (_hideNavBarFeature == true && _showingNavBar == true)
            {
                drawerCollapsedPosition.Y -= NavigationBarHeight;
                _showingNavBar = false;

                if (PanelRatio >= MAX_PANEL_RATIO)
                {
                    drawerCollapsedPosition.Y -= StatusBarHeight;
                }
            }
            Device.BeginInvokeOnMainThread(() =>
            {
                length = (length == DEFAULT_SLIDE_ANIMATION_SPEED) ? (uint)_slideAnimationSpeed : length;
                _slidingPanelAbsoluteLayout.TranslateTo(drawerCollapsedPosition.X, drawerCollapsedPosition.Y, length, Easing.CubicOut);
            });
            _currentSlidePanelState = SlidingPanelState.Collapsed;

            if (_pictureImage != null)
            {
                Rectangle pictureBounds = _pictureAbsoluteLayout.Bounds;
                pictureBounds.Y = drawerCollapsedPosition.Y + _pictureAbsoluteLayout.Height;
                pictureBounds.Y += _currentTitleHeight;
                Device.BeginInvokeOnMainThread(() =>
                {
                    length = (length == DEFAULT_SLIDE_ANIMATION_SPEED) ? (uint)_slideAnimationSpeed : length;
                    _pictureAbsoluteLayout.TranslateTo(pictureBounds.X, pictureBounds.Y, length, Easing.CubicOut);
                });
            }

            WhenSlidingPanelStateChanged?.Invoke(null, new StateChangedEventArgs() { State = _currentSlidePanelState });
        }
        public void ShowExpandedPanel(uint length = DEFAULT_SLIDE_ANIMATION_SPEED)
        {
            if (_isExpandable == true)
            {
                WhenSlidingPanelStateChanging?.Invoke(null, new StateChangingEventArgs() { OldState = _currentSlidePanelState, NewState = SlidingPanelState.Expanded });

                ShowNavigationBar(false);
                _showingNavBar = true;

                if (_hideTitleView)
                    _titleRelativeLayout.HeightRequest = 0;

                Rectangle drawerExpandedPosition = _slidingPanelAbsoluteLayout.Bounds;
                drawerExpandedPosition.Y = 0;

                Device.BeginInvokeOnMainThread(() =>
                {
                    length = (length == DEFAULT_SLIDE_ANIMATION_SPEED) ? (uint)_slideAnimationSpeed : length;
                    _slidingPanelAbsoluteLayout.TranslateTo(drawerExpandedPosition.X, drawerExpandedPosition.Y, length, Easing.CubicOut);
                });
                _currentSlidePanelState = SlidingPanelState.Expanded;

                if (_pictureImage != null)
                {
                    Rectangle pictureExpandedPosition = _pictureAbsoluteLayout.Bounds;
                    pictureExpandedPosition.Y = 0;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        length = (length == DEFAULT_SLIDE_ANIMATION_SPEED) ? (uint)_slideAnimationSpeed : length;
                        _pictureAbsoluteLayout.TranslateTo(pictureExpandedPosition.X, pictureExpandedPosition.Y, length, Easing.CubicOut);
                    });
                }

                WhenSlidingPanelStateChanged?.Invoke(null, new Interface.StateChangedEventArgs() { State = _currentSlidePanelState });
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
