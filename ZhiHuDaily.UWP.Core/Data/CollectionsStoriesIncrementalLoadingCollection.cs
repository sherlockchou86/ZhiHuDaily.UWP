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
    public class CollectionsStoriesIncrementalLoadingCollection : ObservableCollection<Story>, ISupportIncrementalLoading
    {
        private APIService _api = new APIService();

        private bool _busy = false;
        private bool _has_more_items = false;
        private int _current_index = 0;

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

        public CollectionsStoriesIncrementalLoadingCollection()
        {
            HasMoreItems = true;
        }

        private async Task<LoadMoreItemsResult> InnerLoadMoreItemsAsync(uint expectedCount)
        {
            _busy = true;
            var actualCount = 0;
            List<Story> list = new List<Story>();
            try
            {
                if (DataLoading != null)
                {
                    DataLoading();
                }
                int count = DataShareManager.Current.FavoriteList.Count;  //总收藏
                StoryContent sc;  //
                Story s;

                int index;
                for (int i = 0; i < 5; ++i)
                {
                    index = count - (_current_index + i + 1);
                    if (index >= 0)
                    {
                        sc = await _api.GetStoryContent(DataShareManager.Current.FavoriteList[index]);
                        if (sc != null)
                        {
                            s = new Story { Favorite = true, ID = sc.ID, Readed = true, Title = sc.Title, Image = sc.Image };
                            list.Add(s);
                        }
                    }
                }
            }
            catch (Exception)
            {
                HasMoreItems = false;
            }

            if (list != null && list.Any())
            {
                actualCount = list.Count;
                _current_index += actualCount;
                list.ForEach((s) => Add(s));
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
