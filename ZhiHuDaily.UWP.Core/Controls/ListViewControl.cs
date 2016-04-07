
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace ZhiHuDaily.UWP.Core.Controls
{
    public class ListViewControl : ListView
    {
        private int _refreshControlHeight;
        private int _startedPullDownOffset = 0;
        private int _startedPullDownForRefreshOffset = -50; //下拉刷新 开始监听 容错值
        private int _pullDownForRefreshOffset = 35; //下拉刷新触发刷新值
        private DispatcherTimer _timer;



        private Border _refreshControl;
        private Border _pullUpLoadMoreControl;
        private Border _emptyControl;

        //一下字段主要用来暂时解决PC上ListView无法识别的问题
        private bool _isHasDirectManipulationStarted = false; //目前暂时用于来标记 pc 和 手机之前的差别  当false时候 代表未在手机屏幕上操作过
        private DateTime _startedTime = DateTime.Now;
        private DateTime _pullScrllTime = DateTime.Now;
        private DateTime _loadMoreTime = DateTime.Now;
        private double _startedOffset = -1;

        public ScrollViewer ScrollViewer { get; set; }

        /// <summary>
        /// 下拉刷新
        /// </summary>
        public event EventHandler<RoutedEventArgs> PullDownRefresh;
        /// <summary>
        /// 加载更多
        /// </summary>
        public event EventHandler<RoutedEventArgs> PullUpLoadMore;
        /// <summary>
        /// 空点击操作,有时候用于空点击刷新
        /// </summary>
        public event EventHandler<RoutedEventArgs> EmptyClick;

        #region PullDownOffset
        /// <summary>
        /// ViewportTop使用TransformToVisual Header计算最新值
        /// </summary>
        public int PullDownOffset
        {
            get
            {
                return _refreshControlHeight - (int)(Math.Abs(YTransfromOffset) + 1); //精确值
            }
        }

        public double YTransfromOffset
        {
            get
            {
                GeneralTransform transform = _refreshControl.TransformToVisual(this);
                Point topLeft = transform.TransformPoint(new Point(0, 0));
                return topLeft.Y;
            }
        }
        #endregion

        #region VerticalOffset
        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set
            {
                SetValue(VerticalOffsetProperty, value);

                if (ScrollViewer != null)
                {
                    ScrollViewer.ScrollToVerticalOffset(value);
                }
            }
        }

        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(ListViewControl), new PropertyMetadata(null));

        #endregion

        #region ListViewRequestDataContext
        public ListViewRequestDataContext ListViewRequestDataContext
        {
            get { return (ListViewRequestDataContext)GetValue(ListViewRequestDataContextProperty); }
            set { SetValue(ListViewRequestDataContextProperty, value); }
        }

        public static readonly DependencyProperty ListViewRequestDataContextProperty =
            DependencyProperty.Register("ListViewRequestDataContext", typeof(ListViewRequestDataContext), typeof(ListViewControl), new PropertyMetadata(null));

        #endregion

        #region HeaderControlDataContext
        /// <summary>
        /// 添加头部
        /// </summary>
        public object HeaderControlDataContext
        {
            get { return (object)GetValue(HeaderControlDataContextProperty); }
            set { SetValue(HeaderControlDataContextProperty, value); }
        }

        public static readonly DependencyProperty HeaderControlDataContextProperty =
            DependencyProperty.Register("HeaderControlDataContext", typeof(object), typeof(ListViewControl), new PropertyMetadata(null));

        #endregion

        #region EmptyTemplate
        public DataTemplate EmptyTemplate
        {
            get { return (DataTemplate)GetValue(EmptyTemplateProperty); }
            set { SetValue(EmptyTemplateProperty, value); }
        }
        public static readonly DependencyProperty EmptyTemplateProperty =
            DependencyProperty.Register("EmptyTemplate", typeof(DataTemplate), typeof(ListViewControl), new PropertyMetadata(null));
        #endregion

        #region PullDownRefreshControlTemplate
        public DataTemplate PullDownRefreshControlTemplate
        {
            get { return (DataTemplate)GetValue(PullDownRefreshControlTemplateProperty); }
            set { SetValue(PullDownRefreshControlTemplateProperty, value); }
        }

        public static readonly DependencyProperty PullDownRefreshControlTemplateProperty =
            DependencyProperty.Register("PullDownRefreshControlTemplate", typeof(DataTemplate), typeof(ListViewControl), new PropertyMetadata(null));

        #endregion

        #region PullUpLoadMoreControlTemplate
        public DataTemplate PullUpLoadMoreControlTemplate
        {
            get { return (DataTemplate)GetValue(PullUpLoadMoreControlTemplateProperty); }
            set { SetValue(PullUpLoadMoreControlTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RefreshControlTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PullUpLoadMoreControlTemplateProperty =
            DependencyProperty.Register("PullUpLoadMoreControlTemplate", typeof(DataTemplate), typeof(ListViewControl), new PropertyMetadata(null));

        #endregion

        #region HeaderControlTemplate
        /// <summary>
        /// 头部模板
        /// </summary>
        public DataTemplate HeaderControlTemplate
        {
            get { return (DataTemplate)GetValue(HeaderControlTemplateProperty); }
            set { SetValue(HeaderControlTemplateProperty, value); }
        }
        public static readonly DependencyProperty HeaderControlTemplateProperty =
            DependencyProperty.Register("HeaderControlTemplate", typeof(DataTemplate), typeof(ListViewControl), new PropertyMetadata(null));
        #endregion

        #region IsShowPullUpLoadMore
        public bool IsShowPullUpLoadMore
        {
            get { return (bool)GetValue(IsShowPullUpLoadMoreProperty); }
            set { SetValue(IsShowPullUpLoadMoreProperty, value); }
        }

        public static readonly DependencyProperty IsShowPullUpLoadMoreProperty =
            DependencyProperty.Register("IsShowPullUpLoadMore", typeof(bool), typeof(ListViewControl), new PropertyMetadata(false, (s, e) =>
            {
                var This = s as ListViewControl;
                if (null != This.ListViewRequestDataContext)
                {
                    //设置可见性
                    This.ListViewRequestDataContext.IsShowPullUpLoadMore = (bool)e.NewValue;
                }

            }));

        #endregion

        #region IsShowPullDownRefresh
        public bool IsShowPullDownRefresh
        {
            get { return (bool)GetValue(IsShowPullDownRefreshProperty); }
            set { SetValue(IsShowPullDownRefreshProperty, value); }
        }

        public static readonly DependencyProperty IsShowPullDownRefreshProperty =
            DependencyProperty.Register("IsShowPullDownRefresh", typeof(bool), typeof(ListViewControl), new PropertyMetadata(true, (s, e) =>
            {
                var This = s as ListViewControl;
                if (null != This.ListViewRequestDataContext)
                {
                    //设置可见性
                    This.ListViewRequestDataContext.IsShowPullDownRefresh = (bool)e.NewValue;
                }

            }));

        #endregion

        #region IsShowEmpty
        public bool IsShowEmpty
        {
            get { return (bool)GetValue(IsShowEmptyProperty); }
            set { SetValue(IsShowEmptyProperty, value); }
        }

        public static readonly DependencyProperty IsShowEmptyProperty =
            DependencyProperty.Register("IsShowEmpty", typeof(bool), typeof(ListViewControl), new PropertyMetadata(false, (s, e) =>
            {
                var This = s as ListViewControl;
                if (null != This.ListViewRequestDataContext)
                {
                    //设置可见性
                    This.ListViewRequestDataContext.IsShowEmpty = (bool)e.NewValue;
                }

            }));

        #endregion

        #region ItemPadding
        private Thickness? _itemPadding;
        public Thickness ItemPadding
        {
            get { return (Thickness)GetValue(ItemPaddingProperty); }
            set { SetValue(ItemPaddingProperty, value); }
        }
        public static readonly DependencyProperty ItemPaddingProperty =
            DependencyProperty.Register("ItemPadding", typeof(Thickness), typeof(ListViewControl), new PropertyMetadata(null, (s, e) =>
            {
                var This = s as ListViewControl;
                This._itemPadding = (Thickness)e.NewValue;
            }));
        #endregion

        public ListViewControl()
        {
            base.DefaultStyleKey = typeof(ListViewControl);
            InitPullDownTimer();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (DesignMode.DesignModeEnabled)
            {
                return;
            }

            if (null == ListViewRequestDataContext)
            {
                ListViewRequestDataContext = new ListViewRequestDataContext();
                ListViewRequestDataContext.IsShowEmpty = IsShowEmpty;
                ListViewRequestDataContext.IsShowPullDownRefresh = IsShowPullDownRefresh;
                ListViewRequestDataContext.IsShowPullUpLoadMore = IsShowPullUpLoadMore;
            }


            //get template controls
            ScrollViewer = base.GetTemplateChild("ScrollViewer") as ScrollViewer;
            _refreshControl = base.GetTemplateChild("RefreshControl") as Border;
            _pullUpLoadMoreControl = base.GetTemplateChild("PullUpLoadMoreControl") as Border;
            _emptyControl = base.GetTemplateChild("EmptyControl") as Border;
            _emptyControl.Tapped += (s, e) =>
            {
                if (null != EmptyClick)
                {
                    EmptyClick(this, null);
                }
            };

            //解决部分因为提前设置显示空导致的空不可见
            if (ListViewRequestDataContext.IsShowEmpty)
            {
                _emptyControl.Visibility = Visibility.Visible;
                _emptyControl.Opacity = 1;
            }


            _refreshControl.SizeChanged += (s, e) =>
            {
                _refreshControlHeight = (int)(_refreshControl.ActualHeight + 1); //因为被clip 数值会有一定的偏差 目前测试基本在1以内
                _pullDownForRefreshOffset = (int)(_refreshControlHeight * 0.88); //0.66系数 用于下拉触发下拉刷新值
                ScrollViewer.Margin = new Thickness(0, -_refreshControl.ActualHeight, 0, 0);//设置置顶功能
            };

            //如果用户没有设置模板 则设置默认模板
            SetTemplate();

            SetTappedCommand();

            this.ChangedPullDownAndUpStatus();

            if (_itemPadding != null) //强制设置ItemPadding
            {
                this.Padding = ItemPadding;
            }
            //else if (!Utils.Helper.UAPPlatformHelper.IsMobileFamily) //默认PC上有左右间距 phone上不支持
            //{
            //    this.Padding = new Thickness(6, 0, 6, 0);
            //}
            //事件用于加载更多以及下拉功能
            ScrollViewer.DirectManipulationStarted += _scrollViewer_DirectManipulationStarted;
            ScrollViewer.DirectManipulationCompleted += _scrollViewer_DirectManipulationCompleted;
            ScrollViewer.LayoutUpdated += _scrollViewer_LayoutUpdated;
            ScrollViewer.ViewChanged += _scrollViewer_ViewChanged;
        }

        public void ChangedPullDownAndUpStatus()
        {
            this.ListViewRequestDataContext.IsShowPullDownRefreshChangedAction = (bo) =>
            {
                this._refreshControl.Visibility = bo ? Visibility.Visible : Visibility.Collapsed;
            };
            this.ListViewRequestDataContext.IsShowPullUpLoadMoreChangedAction = (bo) =>
            {
                this._pullUpLoadMoreControl.Visibility = bo ? Visibility.Visible : Visibility.Collapsed;
            };
            if (null != this._pullUpLoadMoreControl) //临时解决DataTriggerBehavior  无法触发问题导致
            {
                this._refreshControl.Visibility = this.ListViewRequestDataContext.IsShowPullDownRefresh ? Visibility.Visible : Visibility.Collapsed;
                this._pullUpLoadMoreControl.Visibility = this.ListViewRequestDataContext.IsShowPullUpLoadMore ? Visibility.Visible : Visibility.Collapsed;
            }

        }

        public void ScrollToBottomView(double y = 0)
        {
            if (ScrollViewer != null)
            {
                //部分情况下可能会导致chuangedView无法发生变更，使用ScrollToVerticalOffset解决该问题
                ScrollViewer.ScrollToVerticalOffset(y > 0 ? y : ScrollViewer.ScrollableHeight);
            }
        }

        public void ScrollToTopView()
        {
            if (ScrollViewer != null)
            {
                ScrollViewer.ScrollToVerticalOffset(0);
            }
        }

        private void _scrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            //在 pc 上  //TODO:在能够区分 desktop and phone 的时候添加对pc端下拉刷新的支持
            if (null != PullDownRefresh && !_isHasDirectManipulationStarted && !ListViewRequestDataContext.IsPullDownRefreshStatus && !ListViewRequestDataContext.IsPullUpLoadMoreStatus)
            {
                var timeNow = DateTime.Now;

                //Debug.WriteLine("start:" + " Height=" + ScrollViewer.Height + " ActualHeight=" + ScrollViewer.ActualHeight + " ScrollableHeight=" + ScrollViewer.ScrollableHeight + " ExtentHeight=" + ScrollViewer.ExtentHeight + " VerticalOffset=" + ScrollViewer.VerticalOffset);
                if ((timeNow - _pullScrllTime).TotalSeconds < 0.3) //连续操作中
                {
                    //如果已经触发 且时间小于1.5秒内 则不操作
                    if (ListViewRequestDataContext.IsPullDownRefreshStatus && ((timeNow - _startedTime).TotalSeconds < 1.5))
                    {
                        return;
                    }

                    if (_startedOffset == -1) //第一次 update
                    {
                        _startedOffset = (int)ScrollViewer.VerticalOffset;
                        //Debug.WriteLine("_scrollViewer_ViewChanged 第一次 update " + " ScrollViewer.VerticalOffset=" + ScrollViewer.VerticalOffset);
                    }
                    else if (ScrollViewer.VerticalOffset < 0.5 && _startedOffset != 0) //满足置顶 条件 执行刷新 //这里发现手动滑动 有时候Voffset未必刚好为0 所以暂时做0.5的容错
                    {
                        //Debug.WriteLine("_scrollViewer_ViewChanged 满足置顶 条件 执行刷新update " + " ScrollViewer.VerticalOffset=" + ScrollViewer.VerticalOffset);
                        _startedOffset = -1;
                        _startedTime = timeNow;
                        ListViewRequestDataContext.IsPullDownRefreshStatus = true;
                        PullDownRefresh.Invoke(this, null);
                    }
                }
                else //非连续操作 重置数据
                {
                    //Debug.WriteLine("_scrollViewer_ViewChanged 非连续操作 重置数据 " + " ScrollViewer.VerticalOffset=" + ScrollViewer.VerticalOffset);
                    _startedOffset = -1;
                    _startedTime = timeNow;
                }
                _pullScrllTime = timeNow;
            }

            // Debug.WriteLine("end:" + " Height=" + ScrollViewer.Height + " ActualHeight=" + ScrollViewer.ActualHeight + " ScrollableHeight=" + ScrollViewer.ScrollableHeight + " ExtentHeight=" + ScrollViewer.ExtentHeight + " VerticalOffset=" + ScrollViewer.VerticalOffset);
        }

        private void _scrollViewer_LayoutUpdated(object sender, object e) //TODO:还需要设置好PullUpLoadMore 怎么控制反向更新
        {

            //当上滑动加载更多数据 提前400像素 且当前非加载更多状态
            if (ScrollViewer.ScrollableHeight != 0 && (ScrollViewer.ScrollableHeight - ScrollViewer.VerticalOffset) < 400)
            {
                var time = DateTime.Now;
                if (((time - _loadMoreTime).TotalSeconds > 1.5) && ListViewRequestDataContext != null && ListViewRequestDataContext.IsShowPullUpLoadMore && !ListViewRequestDataContext.IsPullUpLoadMoreStatus && null != PullUpLoadMore && !ListViewRequestDataContext.IsPullDownRefreshStatus)
                {
                    _loadMoreTime = time;
                    ListViewRequestDataContext.IsPullUpLoadMoreStatus = true;
                    PullUpLoadMore.Invoke(this, null);
                    //Debug.WriteLine("MoreDateCommand");
                }


            }
            //Debug.WriteLine("_scrollViewer_LayoutUpdated:" + " VerticalOffset" + _scrollViewer.VerticalOffset + " ViewportTop=" + PullDownOffset + " Height:" + _scrollViewer.ScrollableHeight);
        }

        private void _scrollViewer_DirectManipulationCompleted(object sender, object e)
        {
            _timer.Stop();

            if (ListViewRequestDataContext.IsPullDownRefreshStatus && this.IsShowPullDownRefresh && null != PullDownRefresh)
            {
                PullDownRefresh.Invoke(this, null);
                //执行刷新操作
            }

            ListViewRequestDataContext.IsPullDownRefreshStatus = false;
            //Debug.WriteLine("_scrollViewer_DirectManipulationCompleted");
        }

        private void _scrollViewer_DirectManipulationStarted(object sender, object e)
        {
            _startedPullDownOffset = PullDownOffset;

            if (_startedPullDownOffset > _startedPullDownForRefreshOffset)
            {
                _timer.Start();
            }

            _isHasDirectManipulationStarted = true; //当为true代表手机上操作过

            //Debug.WriteLine("_timer Updated:" + " _startedViewportTop" + _startedPullDownOffset);
        }

        private void SetTemplate()
        {
            if (null == PullDownRefreshControlTemplate)
            {
                PullDownRefreshControlTemplate = ScrollViewer.Resources["PullDownRefreshControlDefaultTemplate"] as DataTemplate;
            }
            if (null == PullUpLoadMoreControlTemplate)
            {
                PullUpLoadMoreControlTemplate = ScrollViewer.Resources["PullUpLoadMoreControlDefaultTemplate"] as DataTemplate;
            }
            if (null == EmptyTemplate)
            {
                EmptyTemplate = ScrollViewer.Resources["EmtpyDefaultTemplate"] as DataTemplate;
            }
        }

        private void SetTappedCommand()
        {
            _pullUpLoadMoreControl.Tapped += (s, e) =>
            {
                if (IsShowPullUpLoadMore && !ListViewRequestDataContext.IsPullUpLoadMoreStatus && null != PullUpLoadMore)
                {
                    ListViewRequestDataContext.IsPullUpLoadMoreStatus = true;
                    PullUpLoadMore.Invoke(this, null);
                }
            };
            _emptyControl.Tapped += (s, e) =>
            {
                if (IsShowEmpty && null != EmptyClick)
                {
                    EmptyClick.Invoke(this, null);
                }
            };
        }

        private void InitPullDownTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(0.01);
            _timer.Tick += (s, e) =>
            {
                if (ScrollViewer.VerticalOffset == 0.0 && PullDownOffset > _pullDownForRefreshOffset)
                {
                    //Debug.WriteLine("_timer Updated:" + " VerticalOffset" + _scrollViewer.VerticalOffset + " ViewportTop=" + PullDownOffset);               
                    ListViewRequestDataContext.IsPullDownRefreshStatus = true;
                }
            };
        }
    }

    /// <summary>
    /// the dataContext for listViewControl empty,pullDownRefresh and PullUpLoadMore
    /// </summary>
    public class ListViewRequestDataContext : INotifyPropertyChanged
    {


        #region IsPullDownRefreshStatus
        private bool _IsPullDownRefreshStatus;
        /// <summary>
        /// 是否是下拉状态，默认为非下拉状态，当下拉到一定程度的时候会变更这个状态为True 模板可以根据这个属性来变更样式
        /// </summary>
        public bool IsPullDownRefreshStatus
        {
            get { return _IsPullDownRefreshStatus; }
            set
            {
                if (_IsPullDownRefreshStatus != value)
                {
                    _IsPullDownRefreshStatus = value;
                    RaisePropertyChanged(() => IsPullDownRefreshStatus);
                }
            }
        }
        #endregion


        public Action<bool> IsShowPullDownRefreshChangedAction;
        #region IsShowPullDownRefresh
        private bool _isShowPullDownRefresh = true;
        /// <summary>
        /// 是否显示下拉刷新功能，false将不显示下拉刷新
        /// </summary>
        public bool IsShowPullDownRefresh
        {
            get { return _isShowPullDownRefresh; }
            set
            {
                if (_isShowPullDownRefresh != value)
                {
                    _isShowPullDownRefresh = value;
                    RaisePropertyChanged(() => IsShowPullDownRefresh);
                    if (null != IsShowPullDownRefreshChangedAction)
                    {
                        IsShowPullDownRefreshChangedAction(_isShowPullDownRefresh);
                    }
                }
            }
        }
        #endregion

        #region IsPullUpLoadMoreStatus
        private bool _isPullUpLoadMoreStatus;
        /// <summary>
        ///  是否是加载更多状态，默认为非加载更多状态，当上滑动到一定程度会触发加载更多，以此变更加载更多样式
        /// </summary>
        public bool IsPullUpLoadMoreStatus
        {
            get { return _isPullUpLoadMoreStatus; }
            set
            {
                if (_isPullUpLoadMoreStatus != value)
                {
                    _isPullUpLoadMoreStatus = value;
                    RaisePropertyChanged(() => IsPullUpLoadMoreStatus);
                }

            }
        }
        #endregion

        public Action<bool> IsShowPullUpLoadMoreChangedAction;

        #region IsShowPullUpLoadMore
        private bool _isShowPullUpLoadMore = false;
        /// <summary>
        /// 是否显示加载更多功能，false将不显示
        /// </summary>
        public bool IsShowPullUpLoadMore
        {
            get { return _isShowPullUpLoadMore; }
            set
            {
                if (_isShowPullUpLoadMore != value)
                {
                    _isShowPullUpLoadMore = value;
                    RaisePropertyChanged(() => IsShowPullUpLoadMore);
                    if (null != IsShowPullUpLoadMoreChangedAction)
                    {
                        IsShowPullUpLoadMoreChangedAction(_isShowPullUpLoadMore);
                    }
                }

            }
        }
        #endregion

        #region IsShowEmpty
        private bool _isShowEmpty = false;
        public bool IsShowEmpty
        {
            get { return _isShowEmpty; }
            set
            {
                if (_isShowEmpty != value)
                {
                    _isShowEmpty = value;
                    RaisePropertyChanged(() => IsShowEmpty);
                }

            }
        }
        #endregion

        #region INotifyPropertyChanged
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Provides access to the PropertyChanged event handler to derived classes.
        /// </summary>
        protected PropertyChangedEventHandler PropertyChangedHandler
        {
            get
            {
                return PropertyChanged;
            }
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            //var myType = this.GetType();
            //if (myType.GetProperty(propertyName) == null)
            //{
            //    throw new ArgumentException("Property not found", propertyName);
            //}
        }

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This cannot be an event")]
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);

            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
          Justification = "This cannot be an event")]
        [SuppressMessage(
            "Microsoft.Design",
            "CA1006:GenericMethodsShouldProvideTypeParameter",
            Justification = "This syntax is more convenient than other alternatives.")]
        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                return;
            }

            var handler = PropertyChanged;

            if (handler != null)
            {
                var body = propertyExpression.Body as MemberExpression;
                var expression = body.Expression as ConstantExpression;
                handler(expression.Value, new PropertyChangedEventArgs(body.Member.Name));
            }
        }
        #endregion
    }
}
