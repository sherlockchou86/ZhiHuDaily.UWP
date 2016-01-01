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
    public sealed partial class StoryCommentPage : Page
    {
        private StoryCommentViewModel _viewModel;

        public StoryCommentPage()
        {
            this.InitializeComponent();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            object[] parameters = e.Parameter as object[];
            if (parameters != null && parameters[0] != null && parameters[1] != null)
            {
                this.DataContext = _viewModel = new StoryCommentViewModel(parameters[0].ToString(), parameters[1] as StoryExtra);
            }
        }

    }
    /// <summary>
    /// 
    /// </summary>
    class TotalCommentsFormat : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return "共" + value.ToString() + "条评论";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    class TimeFormat : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            long t_s = long.Parse(value.ToString());
            DateTime t = DateTime.Parse("1970/1/1").AddSeconds(t_s);
            return t.AddHours(8).ToString("MM-dd HH:mm", new System.Globalization.CultureInfo("zh-CN"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
