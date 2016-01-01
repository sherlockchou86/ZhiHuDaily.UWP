using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using ZhiHuDaily.UWP.Core.Https;
using ZhiHuDaily.UWP.Core.Models;
using ZhiHuDaily.UWP.Core.ViewModels;

namespace ZhiHuDaily.UWP.Core.Data
{
    /// <summary>
    /// 首页文章自增加载集合
    /// </summary>
    public class HomeStoriesIncrementalLoadingCollection : ObservableCollection<Story>, ISupportIncrementalLoading
    {
        private APIService _api = new APIService();

        private bool _busy = false;
        private bool _has_more_items = false;
        private string _current_date;

        public event DataLoadingEventHandler DataLoading;
        public event DataLoadedEventHandler DataLoaded;

        public bool HasMoreItems
        {
            get
            {
                if (_busy)
                    return false;
                else
                    return _has_more_items;
            }
            private set
            {
                _has_more_items = value;
            }
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return InnerLoadMoreItemsAsync(count).AsAsyncOperation();
        }

        public HomeStoriesIncrementalLoadingCollection(string today_date)
        {
            _current_date = today_date;
            HasMoreItems = true;
        }

        private async Task<LoadMoreItemsResult> InnerLoadMoreItemsAsync(uint expectedCount)
        {
            _busy = true;
            var actualCount = 0;
            List<Story> list = null;
            try
            {
                if (DataLoading != null)
                {
                    DataLoading();
                }
                list = await _api.GetBeforeStories(_current_date);
            }
            catch (Exception)
            {
                HasMoreItems = false;
            }

            if (list != null && list.Any())
            {
                actualCount = list.Count;
                list[0].Separator = true;  //当前第一条
                list.ForEach((s) =>
                {
                    if (DataShareManager.Current.FavoriteList.Contains(s.ID))
                    {
                        s.Favorite = true;
                    }
                    if (DataShareManager.Current.ReadedList.Contains(s.ID))
                    {
                        s.Readed = true;
                    }
                    Add(s);
                });
                AddDate();  //日期后移
                HasMoreItems = true;
            }
            else
            {
                HasMoreItems = false;
            }
            if (DataLoaded != null)
            {
                DataLoaded();
            }
            _busy = false;
            return new LoadMoreItemsResult
            {
                Count = (uint)actualCount
            };
        }

        private void AddDate()
        {
            string st = _current_date.Insert(4, "/").Insert(7, "/");
            DateTime t = DateTime.Parse(st).AddDays(-1);
            _current_date = t.Year + (t.Month < 10 ? "0" + t.Month.ToString() : t.Month.ToString()) + (t.Day < 10 ? "0" + t.Day.ToString() : t.Day.ToString());
        }
    }
}
