using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhiHuDaily.UWP.Core.Https;
using ZhiHuDaily.UWP.Core.Models;

namespace ZhiHuDaily.UWP.Core.ViewModels
{
    public class SplashViewModel : ViewModelBase
    {
        private APIService _api = new APIService();

        private string _splash_image;
        public string SplashImage
        {
            get
            {
                return _splash_image;
            }
            set
            {
                _splash_image = value;
                OnPropertyChanged();
            }
        }

        private string _splash_text;
        public string SplashText
        {
            get
            {
                return _splash_text;
            }
            set
            {
                _splash_text = value;
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
        public SplashViewModel()
        {
            Update();
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        public async void Update()
        {
            IsLoading = true;
            StartImage s = await _api.GetStartImage();
            await Task.Delay(1000);
            if (s != null)
            {
                SplashImage = s.ImageURL;
                string image_text = (s.ImageText == null || s.ImageText.Equals("")) ? "好像是广告" : s.ImageText;
                SplashText = "by " + image_text;
            }
            IsLoading = false;
        }
    }
}
