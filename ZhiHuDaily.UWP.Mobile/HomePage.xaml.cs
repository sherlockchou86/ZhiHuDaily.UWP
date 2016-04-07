using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ZhiHuDaily.UWP.Core.Models;
using ZhiHuDaily.UWP.Core.ViewModels;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace ZhiHuDaily.UWP.Mobile
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HomePage : Page
    {
        private HomeViewModel _viewModel;

        ItemsStackPanel _itemsPanel;
        ScrollViewer _scrollView;

        public HomePage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;

            TopStoryIndexAutoChange();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.New)
            {
                this.DataContext = _viewModel = new HomeViewModel();
            }
        }
        /// <summary>
        /// 点击头条文章
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TopStories_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(StoryPage), new object[] { TopStories.SelectedItem as Story });
        }
        /// <summary>
        /// 点击文章列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listStories_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(StoryPage), new object[] { e.ClickedItem as Story });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            List<ScrollViewer> list = new List<ScrollViewer>();
            HomeHelper.FindChildren<ScrollViewer>(list, listStories);  //先找到ScrollViewer 注册ViewChanged事件
            if (list.Count > 0)
            {
                _scrollView = list[0];
                _scrollView.ViewChanged += HomePage_ViewChanged;
            }
            List<ItemsStackPanel> list2 = new List<ItemsStackPanel>();
            HomeHelper.FindChildren<ItemsStackPanel>(list2, listStories);  //找到ItemStackPanel 它包含FirstVisibleIndex属性
            if (list.Count > 0)
            {
                _itemsPanel = list2[0];
            }
        }
        /// <summary>
        /// 列表滚动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HomePage_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (_scrollView.VerticalOffset > 220)
            {
                if (_itemsPanel != null)
                {
                    _viewModel.Title = HomeHelper.DateStringFormat((listStories.Items[_itemsPanel.FirstVisibleIndex] as Story).Date);
                }
            }
            else
            {
                _viewModel.Title = "首页";
            }
        }

        /// <summary>
        /// 设置选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSetting_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingPage));
        }


        private async void TopStoryIndexAutoChange()
        {
            await Task.Delay(3000);
            if (TopStories.Items.Count > 0)
            {
                TopStories.SelectedIndex = (TopStories.SelectedIndex + 1) % 5;
            }
            TopStoryIndexAutoChange();
        }
        /// <summary>
        /// 手动刷新
        /// </summary>
        public void RefreshPage()
        {
            _viewModel.Update();
        }
        /// <summary>
        /// 下拉刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listStories_PullDownRefresh(object sender, RoutedEventArgs e)
        {
            _viewModel.Update();
        }
    }

    /// <summary>
    /// 头条文章方块 颜色填充 转换类
    /// </summary>
    class TopStoryFillColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (int.Parse(value.ToString()) == int.Parse(parameter.ToString()))
            {
                return new SolidColorBrush(Windows.UI.Colors.White);
            }
            else
            {
                return new SolidColorBrush(Windows.UI.Color.FromArgb(200,0,0,0));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 文章列表中子元素 可见性 转换类
    /// </summary>
    class ChildrenVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 日期转换
    /// </summary>
    class DateFormat : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTime t = DateTime.Parse(value.ToString().Insert(4, "/").Insert(7, "/"));
            return t.Date.ToString("MM月dd日 dddd", new System.Globalization.CultureInfo("zh-CN"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    class HomeHelper
    {
        internal static void FindChildren<T>(List<T> results, DependencyObject startNode)
                where T : DependencyObject
        {
            int count = VisualTreeHelper.GetChildrenCount(startNode);
            for (int i = 0; i < count; i++)
            {
                DependencyObject current = VisualTreeHelper.GetChild(startNode, i);
                if ((current.GetType()).Equals(typeof(T)) || (current.GetType().GetTypeInfo().IsSubclassOf(typeof(T))))
                {
                    T asType = (T)current;
                    results.Add(asType);
                }
                FindChildren<T>(results, current);
            }
        }

        internal static string DateStringFormat(string date)
        {
            string date_new = date.Insert(4, "/").Insert(7, "/");
            DateTime t = DateTime.Parse(date_new);
            if (t.Date.Equals(DateTime.Now.Date))
            {
                return "今日热闻";
            }
            else
            {
                return t.Date.ToString("MM月dd日 dddd", new System.Globalization.CultureInfo("zh-CN"));
            }
        }
    }
    /// <summary>
    /// 每项背景色 转换
    /// </summary>
    class ItemBackgroundFormat : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (DataShareManager.Current.APPTheme == ElementTheme.Light)
            {
                if ((bool)value)
                {
                    return new SolidColorBrush(Colors.Gray);
                }
                else
                {
                    return new SolidColorBrush(Colors.Black);
                }
            }
            else
            {
                if ((bool)value)
                {
                    return new SolidColorBrush(Colors.Gray);
                }
                else
                {
                    return new SolidColorBrush(Colors.White);
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
