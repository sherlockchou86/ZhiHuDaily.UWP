using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhiHuDaily.UWP.Core.Https;
using ZhiHuDaily.UWP.Core.Models;

namespace ZhiHuDaily.UWP.Core.ViewModels
{
    public class RecommendersViewModel:ViewModelBase
    {
        private APIService _api = new APIService();
        private string _home_page = "http://www.zhihu.com/people/";

        private ObservableCollection<Recommender> _recommenders;
        private string _story_id;

        public ObservableCollection<Recommender> Recommenders
        {
            get
            {
                return _recommenders;
            }
            set
            {
                _recommenders = value;
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

        public RecommendersViewModel(string story_id)
        {
            _story_id = story_id;

            Update();
        }

        public async void Update()
        {
            IsLoading = true;
            List<Recommender> list = await _api.GetRecommenders(_story_id);
            if (list != null)
            {
                Recommenders = new ObservableCollection<Recommender>();
                list.ForEach((r) => { Recommenders.Add(r); });
            }
            IsLoading = false;
        }

        public async void ScanHomePage(Recommender r)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(_home_page + r.Zhihu_URL_Token));
        }
    }
}
