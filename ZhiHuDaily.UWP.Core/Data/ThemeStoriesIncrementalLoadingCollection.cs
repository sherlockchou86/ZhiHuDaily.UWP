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
    public class ThemeStoriesIncrementalLoadingCollection: ObservableCollection<Story>, ISupportIncrementalLoading
    {
        private APIService _api = new APIService();

        private bool _busy = false;
        private bool _has_more_items = false;
        private string _last_story_id;
        private string _theme_id;

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

        public ThemeStoriesIncrementalLoadingCollection(string last_story_id,string theme_id)
        {
            _last_story_id = last_story_id;
            _theme_id = theme_id;
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
                list = await _api.GetBeforeThemeStories(_theme_id, _last_story_id);
            }
            catch (Exception)
            {
                HasMoreItems = false;
            }

            if (list != null && list.Any())
            {
                actualCount = list.Count;
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
                _last_story_id = list.Last().ID;  //更新最后一篇文章的id
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

    }
}
