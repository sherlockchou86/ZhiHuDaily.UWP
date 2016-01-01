using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
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
using ZhiHuDaily.UWP.Core.Share;
using ZhiHuDaily.UWP.Core.ViewModels;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace ZhiHuDaily.UWP.Mobile
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class StoryPage : Page
    {
        private StoryViewModel _viewModel;

        public StoryPage()
        {
            this.InitializeComponent();
            RegisterForShare();
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
                object[] parameters = e.Parameter as object[];
                if (parameters != null)
                {
                    this.DataContext = _viewModel = new StoryViewModel(parameters[0] as Story);
                }
                imgStoryPic.Height = 220;
                HideTop();
            }
        }
        /// <summary>
        /// 点击查看推荐者
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridrecommenders_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(RecommendersPage), new object[] { _viewModel.ID });
        }

        /// <summary>
        /// 点击分享
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShare_Click(object sender, RoutedEventArgs e)
        {
            
        }
        /// <summary>
        /// 点击收藏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCollection_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ExchangeFavorite();
        }
        /// <summary>
        /// 点击评论
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnComment_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(StoryCommentPage), new object[] { _viewModel.ID, _viewModel.SE });
        }


        private void RegisterForShare()
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager,
                DataRequestedEventArgs>(this.ShareLinkHandler);
        }

        private void ShareLinkHandler(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.Properties.Title = "分享文章[来自 知乎日报UWP版]";
            request.Data.Properties.Description = "向好友分享这篇文章";
            request.Data.SetWebLink(new Uri(_viewModel.ShareUrl));
        }

        private async void HideTop()
        {
            try

            {
                await Task.Delay(1);
                imgStoryPic.Height = imgStoryPic.Height - 1;

                if (imgStoryPic.Height < 100)
                {
                    txtbStoryTitle.Visibility = Visibility.Collapsed;
                    txtbStoryImageSource.Visibility = Visibility.Collapsed;
                }
                if (imgStoryPic.Height != 0)
                {
                    HideTop();
                }
            }
            catch
            {

            }
        }
        /// <summary>
        /// 分享
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstShare_ItemClick(object sender, ItemClickEventArgs e)
        {
            int index = int.Parse((e.ClickedItem as FrameworkElement).Tag.ToString());
            if (index == 0)
            {
                Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI();
            }
            else if (index == 1)
            {
                _viewModel.Share2Timeline();
            }
            else
            {
                _viewModel.Share2Session();
            }
        }
    }
    /// <summary>
    /// 子元素是否可见 转换器
    /// </summary>
    class ChildrenVisibilityByString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || value.ToString().Equals(""))
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 收藏按钮文本 转换器
    /// </summary>
    class CollectionButtonTextFormat : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value)
            {
                return "√";
            }
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
