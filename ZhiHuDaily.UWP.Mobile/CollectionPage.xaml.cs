using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class CollectionPage : Page
    {
        private CollectionViewModel _viewModel;

        public CollectionPage()
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
                this.DataContext = _viewModel = new CollectionViewModel();
        }
        /// <summary>
        /// 点击查看收藏的文章
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listCollections_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(StoryPage), new object[] {e.ClickedItem});
        }

        object selected_item;  //选中项的数据源
        /// <summary>
        /// 右键收藏，弹出菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            UIElement uie = e.OriginalSource as UIElement;
            selected_item = (e.OriginalSource as FrameworkElement).DataContext;
            (this.Resources["contextMenu"] as MenuFlyout).ShowAt(uie, e.GetPosition(uie));           
        }
        /// <summary>
        /// 浏览收藏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuScan_Click(object sender, RoutedEventArgs e)
        {
            if (selected_item != null)
            {
                this.Frame.Navigate(typeof(StoryPage), new object[] { selected_item as Story });
            }
        }
        /// <summary>
        /// 取消收藏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuRemove_Click(object sender, RoutedEventArgs e)
        {
            if (selected_item != null)
            {
                _viewModel.ExchangeFavorite((selected_item as Story).ID);
            }
        }
    }
}
