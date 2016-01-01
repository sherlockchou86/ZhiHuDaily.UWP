using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhiHuDaily.UWP.Core.Data;
using ZhiHuDaily.UWP.Core.Models;

namespace ZhiHuDaily.UWP.Core.ViewModels
{
    public class CollectionViewModel:ViewModelBase
    {
        private CollectionsStoriesIncrementalLoadingCollection _collection_stories;
        public CollectionsStoriesIncrementalLoadingCollection CollectionStories
        {
            get
            {
                return _collection_stories;
            }
            set
            {
                _collection_stories = value;
                OnPropertyChanged();
            }
        }

        private string _collectioncount;
        public string CollectionCount
        {
            get
            {
                return _collectioncount;
            }
            set
            {
                _collectioncount = value;
                OnPropertyChanged();
            }
        }

        private bool _is_loading;
        public bool IsLoading
        {
            get
            {
                return _is_loading;
            }
            set
            {
                _is_loading = value;
                OnPropertyChanged();
            }
        }

        public CollectionViewModel()
        {
            Update();
        }

        public void Update()
        {
            CollectionStories = new Data.CollectionsStoriesIncrementalLoadingCollection();
            CollectionStories.DataLoading += CollectionStories_DataLoading;
            CollectionStories.DataLoaded += CollectionStories_DataLoaded;

            CollectionCount = DataShareManager.Current.FavoriteList.Count.ToString();
        }
        /// <summary>
        /// 数据加载完毕
        /// </summary>
        private void CollectionStories_DataLoaded()
        {
            IsLoading = false;
        }
        /// <summary>
        /// 数据开始加载
        /// </summary>
        private void CollectionStories_DataLoading()
        {
            IsLoading = true;
        }
        /// <summary>
        /// 取消收藏
        /// </summary>
        /// <param name="favorite_id"></param>
        public void ExchangeFavorite(string favorite_id)
        {
            DataShareManager.Current.UpdateFavorites(favorite_id);
            List<Story> l = CollectionStories.Where((s) => s.ID == favorite_id).ToList();
            if (l != null && l.Any())
            {
                l[0].Favorite = !l[0].Favorite;
            }
        }
    }
}
