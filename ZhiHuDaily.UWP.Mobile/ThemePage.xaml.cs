using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class ThemePage : Page
    {
        ThemeViewModel _viewModel;

        public ThemePage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
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
                object[] paramaters = e.Parameter as object[];
                if (paramaters != null)
                {
                    Theme theme = paramaters[0] as Theme;
                    this.DataContext = _viewModel = new ThemeViewModel(theme.ID);
                }
            }
        }
        /// <summary>
        /// 查看主编
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridEditors_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(EditorPage), new object[] { _viewModel.Editors });
        }
        /// <summary>
        /// 点击查看文章内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listStories_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(StoryPage), new object[] { e.ClickedItem as Story });
        }
        /// <summary>
        /// 刷新页面数据
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
            RefreshPage();
        }
    }
}
