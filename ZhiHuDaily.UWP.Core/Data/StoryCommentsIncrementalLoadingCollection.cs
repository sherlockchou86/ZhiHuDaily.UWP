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

namespace ZhiHuDaily.UWP.Core.Data
{
    public class StoryCommentsIncrementalLoadingCollection:ObservableCollection<StoryComment>, ISupportIncrementalLoading
    {
        private APIService _api = new APIService();

        private bool _busy = false;
        private bool _has_more_items = false;
        private string _last_comment_id;
        private string _story_id;
        private bool _short_comment;

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

        public StoryCommentsIncrementalLoadingCollection(string last_comment_id, string story_id, bool short_comment)
        {
            _last_comment_id = last_comment_id;
            _story_id = story_id;
            _short_comment = short_comment;
            HasMoreItems = true;
        }


        private async Task<LoadMoreItemsResult> InnerLoadMoreItemsAsync(uint expectedCount)
        {
            _busy = true;
            var actualCount = 0;
            List<StoryComment> list = null;
            try
            {
                if (DataLoading != null)
                {
                    DataLoading();
                }
                if (_short_comment)
                {
                    list = await _api.GetBeforeShortComments(_story_id, _last_comment_id);
                }
                else
                {
                    list = await _api.GetBeforeLongComments(_story_id, _last_comment_id);
                }
            }
            catch (Exception)
            {
                HasMoreItems = false;
            }

            if (list != null && list.Any())
            {
                actualCount = list.Count;
                list.ForEach((s) => Add(s));
                _last_comment_id = list.Last().ID;  //更新最后一篇评论的id
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
