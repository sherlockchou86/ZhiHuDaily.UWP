using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using ZhiHuDaily.UWP.Core.Data;
using ZhiHuDaily.UWP.Core.Https;
using ZhiHuDaily.UWP.Core.Models;
using ZhiHuDaily.UWP.Core.Tools;

namespace ZhiHuDaily.UWP.Core.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        APIService _api = new APIService();

        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private ElementTheme _app_theme;
        public ElementTheme APPTheme
        {
            get
            {
                return _app_theme;
            }
            set
            {
                _app_theme = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Story> _top_stories;
        public ObservableCollection<Story> Top_Stories
        {
            get
            {
                return _top_stories;
            }
            set
            {
                _top_stories = value;
                OnPropertyChanged();
            }
        }

        private HomeStoriesIncrementalLoadingCollection _stories;
        public HomeStoriesIncrementalLoadingCollection Stories
        {
            get
            {
                return _stories;
            }
            set
            {
                _stories = value;
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

        private string _network_title;
        public string NetworkTitle
        {
            get
            {
                return _network_title;
            }
            set
            {
                _network_title = value;
                OnPropertyChanged();
            }
        }

        public HomeViewModel()
        {
            Update();

            APPTheme = DataShareManager.Current.APPTheme;
            DataShareManager.Current.ShareDataChanged += Current_ShareDataChanged;


            NetworkManager.Current.NetworkStatusChanged += Current_NetworkStatusChanged;
            NetworkTitle = "[" + NetworkManager.Current.NetworkTitle + "]";

        }

        private void Current_NetworkStatusChanged(object sender)
        {
            
            NetworkTitle = "[" + NetworkManager.Current.NetworkTitle + "]";
        }

        /// <summary>
        /// 刷新http数据
        /// </summary>
        public async void Update()
        {
            IsLoading = true;
            LatestStories ls = await _api.GetLatestStories();
            if (ls != null)
            {
                ls.Stories[0].Separator = true;  //当天第一条
                HomeStoriesIncrementalLoadingCollection c = new HomeStoriesIncrementalLoadingCollection(ls.Date);
                ls.Stories.ToList().ForEach((s) => 
                {
                    if (DataShareManager.Current.FavoriteList.Contains(s.ID))
                    {
                        s.Favorite = true;
                    }
                    if (DataShareManager.Current.ReadedList.Contains(s.ID))
                    {
                        s.Readed = true;
                    }
                    c.Add(s);
                }
                );

                Stories = c;
                Top_Stories = ls.Top_Stories;

                Title = "首页";

                c.DataLoaded += C_DataLoaded;
                c.DataLoading += C_DataLoading;
            }
            IsLoading = false;
        }
        /// <summary>
        /// 
        /// </summary>
        private void C_DataLoading()
        {
            IsLoading = true;
        }
        /// <summary>
        /// 
        /// </summary>
        private void C_DataLoaded()
        {
            IsLoading = false;
        }

        /// <summary>
        /// 刷新配置数据
        /// </summary>
        private void Current_ShareDataChanged()
        {
            APPTheme = DataShareManager.Current.APPTheme;
            Stories.ToList().ForEach((s) => s.Readed = s.Readed);
        }

    }
}
