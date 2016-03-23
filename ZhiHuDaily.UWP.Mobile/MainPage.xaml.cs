using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using ZhiHuDaily.UWP.Core.Models;
using ZhiHuDaily.UWP.Core.Tools;
using ZhiHuDaily.UWP.Core.ViewModels;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace ZhiHuDaily.UWP.Mobile
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MainViewModel _viewModel;
        public MainPage()
        {
            this.InitializeComponent();

            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;

            DispatcherManager.Current.Dispatcher = Dispatcher;
        }
        /// <summary>
        /// 系统后退
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!sptViewNavigation.IsSwipeablePaneOpen)
            {
                if (this.frmPages.CanGoBack && !this.frmPages.Content.GetType().Equals(typeof(HomePage)))  //
                {
                    this.frmPages.GoBack();
                }
                else
                {
                    if (popTips.IsOpen)  //第二次按back键
                    {
                        Application.Current.Exit();
                    }
                    else
                    {
                        popTips.IsOpen = true;  //提示再按一次
                        popTips.HorizontalOffset = this.ActualWidth / 2 - 45;  //居中
                        popTips.VerticalOffset = this.ActualHeight / 2 - 5;
                        e.Handled = true;
                        await Task.Delay(1000);  //1000ms后关闭提示
                        popTips.IsOpen = false;
                    }
                }
            }
            else
            {
                sptViewNavigation.IsSwipeablePaneOpen = false;
            }
            e.Handled = true;
        }

        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.DataContext = _viewModel = new MainViewModel();
        }

        /// <summary>
        /// 导航栏选择变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Theme theme = e.AddedItems[0] as Theme;
            if (theme != null)
            {
                if (theme.ID.Equals("-1"))  //首页
                {
                    this.frmPages.Navigate(typeof(HomePage));
                }
                else  //主题页面
                {
                    this.frmPages.Navigate(typeof(ThemePage), new object[] { theme });
                }
                sptViewNavigation.IsSwipeablePaneOpen = false;
            }
        }

        /// <summary>
        /// 打开导航栏
        /// </summary>
        public void OpenNavigationPanel()
        {
            sptViewNavigation.IsSwipeablePaneOpen = !sptViewNavigation.IsSwipeablePaneOpen;
        }

        /// <summary>
        /// 打开导航
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void appbtnNavigation_Click(object sender, RoutedEventArgs e)
        {
            OpenNavigationPanel();
        }

        /// <summary>
        /// 打开设置界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void appbtnSetting_Click(object sender, RoutedEventArgs e)
        {
            if (!this.frmPages.Content.GetType().Equals(typeof(SettingPage)))
                this.frmPages.Navigate(typeof(SettingPage));
            sptViewNavigation.IsSwipeablePaneOpen = false;
        }
        /// <summary>
        /// 打开收藏界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void appbtnCollection_Click(object sender, RoutedEventArgs e)
        {
            if (!this.frmPages.Content.GetType().Equals(typeof(CollectionPage)))
                this.frmPages.Navigate(typeof(CollectionPage));
            sptViewNavigation.IsSwipeablePaneOpen = false;
        }
        /// <summary>
        /// 后退
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void appbtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (!sptViewNavigation.IsSwipeablePaneOpen)
            {
                if (this.frmPages.CanGoBack && !this.frmPages.Content.GetType().Equals(typeof(HomePage)))  //
                {
                    this.frmPages.GoBack();
                }
                else
                {
                    if (popTips.IsOpen)  //第二次按back键
                    {
                        Application.Current.Exit();
                    }
                    else
                    {
                        popTips.IsOpen = true;  //提示再按一次
                        popTips.HorizontalOffset = this.ActualWidth / 2 - 45;  //居中
                        popTips.VerticalOffset = this.ActualHeight / 2 - 5;
                        await Task.Delay(1000);  //1000ms后关闭提示
                        popTips.IsOpen = false;
                    }
                }
            }
            sptViewNavigation.IsSwipeablePaneOpen = false;
        }
        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void appbtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (frmPages.Content is HomePage)
            {
                (frmPages.Content as HomePage).RefreshPage();
            }
            else if (frmPages.Content is ThemePage)
            {
                (frmPages.Content as ThemePage).RefreshPage();
            }
            sptViewNavigation.IsSwipeablePaneOpen = false;
        }
        /// <summary>
        /// 子页面导航完毕  判断该页面可否刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmPages_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.SourcePageType.Equals(typeof(HomePage)) || e.SourcePageType.Equals(typeof(ThemePage)))
            {
                appbtnRefresh.Visibility = Visibility.Visible;
            }
            else
            {
                appbtnRefresh.Visibility = Visibility.Collapsed;
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    class LogoBackgroundFormat : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((ElementTheme)(value) == ElementTheme.Dark)
            {
                return new BitmapImage { UriSource = new Uri("ms-appx:///Assets/logo_background_dark.png") };
            }
            else
            {
                return new BitmapImage { UriSource = new Uri("ms-appx:///Assets/logo_background_light.jpg") };
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
