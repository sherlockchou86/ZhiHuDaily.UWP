using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using ZhiHuDaily.UWP.Core.Https;
using ZhiHuDaily.UWP.Core.Models;
using Windows.ApplicationModel.Background;

namespace ZhiHuDaily.UWP.Core.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        APIService _api = new APIService();

        private ObservableCollection<Theme> _themes;
        public ObservableCollection<Theme> Themes
        {
            get
            {
                return _themes;
            }
            set
            {
                _themes = value;
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

                SetLogoBack();
            }
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
            }
        }

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
                if (_title == null || _title.Equals(""))
                {
                    _title = "知乎日报UWP";
                }
                OnPropertyChanged();
            }
        }

        private string _sub_title;
        public string SubTitle
        {
            get
            {
                return _sub_title;
            }
            set
            {
                _sub_title = value;
                OnPropertyChanged();
            }
        }

        private BitmapImage _logo_back;
        public BitmapImage LogoBack
        {
            get
            {
                return _logo_back;
            }
            set
            {
                _logo_back = value;
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
        private string _logo_back_light = "";
        private string _logo_back_dark = "";

        public MainViewModel()
        {
            Update();

            AutoUpdateQuotation();

            DataShareManager.Current.ShareDataChanged += Current_ShareDataChanged;

        }

        /// <summary>
        /// 刷新配置数据
        /// </summary>
        private void Current_ShareDataChanged()
        {
            APPTheme = DataShareManager.Current.APPTheme;
        }
        /// <summary>
        /// 自动更新quotation
        /// </summary>
        private async void AutoUpdateQuotation()
        {
            ZhihuDailyQuotation quo = await _api.GetQuotationInfo();
            if (quo != null)
            {
                Title = quo.Title;
                SubTitle = quo.SubTitle;
                _logo_back_dark = quo.LogoBackDark;
                _logo_back_light = quo.LogoBackLight;
            }
            else
            {
                Title = null;
                SubTitle = null;
                _logo_back_dark = null;
                _logo_back_light = null;
            }
            SetLogoBack();

            await Task.Delay(1000*60);
            AutoUpdateQuotation();
        }
        /// <summary>
        /// 
        /// </summary>
        private void SetLogoBack()
        {

            if (_app_theme == ElementTheme.Dark)
            {
                string src = _logo_back_dark;
                if (src == null || src.Equals(""))
                {
                    src = "ms-appx:///Assets/logo_background_dark.png";
                }
                LogoBack = new BitmapImage { UriSource = new Uri(src) };
            }
            else
            {
                string src = _logo_back_light;
                if (src == null || src.Equals(""))
                {
                    src = "ms-appx:///Assets/logo_background_light.jpg";
                }
                LogoBack = new BitmapImage { UriSource = new Uri(src) };
            }
        }
        /// <summary>
        /// 刷新数据
        /// </summary>
        public async void Update()
        {
            IsLoading = true;

            var t1 = _api.GetThemes();
            var t2 = _api.GetQuotationInfo();

            List<Theme> list = await t1;
            ZhihuDailyQuotation quo = await t2;

            if (list != null)
            {
                Themes = new ObservableCollection<Theme>();
                Themes.Add(new Theme { ID = "-1", Name = "首页", Description = "", Thumbnail="" });  //首页
                list.ForEach((t) => { Themes.Add(t); });

                SelectedIndex = 0;
            }
            if (quo != null)
            {
                Title = quo.Title;
                SubTitle = quo.SubTitle;
                _logo_back_dark = quo.LogoBackDark;
                _logo_back_light = quo.LogoBackLight;
            }
            else
            {
                Title = null;
                SubTitle = null;
                _logo_back_dark = null;
                _logo_back_light = null;
            }
            APPTheme = DataShareManager.Current.APPTheme;

            IsLoading = false;
        }
    }
}
