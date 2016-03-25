using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhiHuDaily.UWP.Core.Data;
using ZhiHuDaily.UWP.Core.Https;
using ZhiHuDaily.UWP.Core.Models;

namespace ZhiHuDaily.UWP.Core.ViewModels
{
    public class ThemeViewModel:ViewModelBase
    {
        private string _theme_id;
        private APIService _api = new APIService();

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

        private string _backImage;
        public string BackImage
        {
            get
            {
                return _backImage;
            }
            set
            {
                _backImage = value;
                OnPropertyChanged();
            }
        }

        private string _description;
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
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
        private string _backImageSource;
        public string BackImageSource
        {
            get
            {
                return _backImageSource;
            }
            set
            {
                _backImageSource = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Editor> _editors;
        public ObservableCollection<Editor> Editors
        {
            get
            {
                return _editors;
            }
            set
            {
                _editors = value;
                OnPropertyChanged();
            }
        }

        private ThemeStoriesIncrementalLoadingCollection _stories;
        public ThemeStoriesIncrementalLoadingCollection Stories
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
        public ThemeViewModel(string theme_id)
        {
            _theme_id = theme_id;

            Update();

            DataShareManager.Current.ShareDataChanged += Current_ShareDataChanged;
        }

        private void Current_ShareDataChanged()
        {
            if (Stories != null)
                Stories.ToList().ForEach((s) => s.Readed = s.Readed);
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        public async void Update()
        {
            IsLoading = true;
            LatestThemeStories lts = await _api.GetLatestThemeStories(_theme_id);
            if (lts != null)
            {
                Title = lts.Name;
                Description = lts.Description;
                BackImage = lts.Image;
                BackImageSource = !lts.Image_Source.Equals("") ? "from " + lts.Image_Source : "";


                ThemeStoriesIncrementalLoadingCollection c = new ThemeStoriesIncrementalLoadingCollection(lts.Stories.Last().ID, _theme_id);
                lts.Stories.ToList().ForEach((s) =>
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
                });

                Stories = c;
                Editors = lts.Editors;

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
    }
}
