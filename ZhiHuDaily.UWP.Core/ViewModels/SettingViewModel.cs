using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using ZhiHuDaily.UWP.Core.Tools;

namespace ZhiHuDaily.UWP.Core.ViewModels
{
    public class SettingViewModel:ViewModelBase
    {
        private bool _dark_mode;
        public bool DarkMode
        {
            get
            {
                return _dark_mode;
            }
            set
            {
                _dark_mode = value;
                OnPropertyChanged();
            }
        }

        private bool _big_font;
        public bool BigFont
        {
            get
            {
                return _big_font;
            }
            set
            {
                _big_font = value;
                OnPropertyChanged();
            }
        }
        private string _clearCacheTitle;
        public string ClearCacheTitle
        {
            get
            {
                return _clearCacheTitle;
            }
            set
            {
                _clearCacheTitle = value;
                OnPropertyChanged();
            }
        }
        private bool _is_busy;
        public bool IsBusy
        {
            get
            {
                return _is_busy;
            }
            set
            {
                _is_busy = value;
                OnPropertyChanged();
            }
        }
        private bool _no_images_mode;
        public bool NoImagesMode
        {
            get
            {
                return _no_images_mode;
            }
            set
            {
                _no_images_mode = value;
                OnPropertyChanged();
            }
        }
        private bool _show_toast;
        public bool ShowToast
        {
            get
            {
                return _show_toast;
            }
            set
            {
                _show_toast = value;
                OnPropertyChanged();
            }
        }

        public string VersionString
        {
            get
            {
                var v = Package.Current.Id.Version;
                return "当前版本: " + v.Major + "." + v.Minor + "." + v.Build + "." + v.Revision;
            }
        }
        public SettingViewModel()
        {
            Update();
            DataShareManager.Current.ShareDataChanged += Current_ShareDataChanged;
        }

        public async void Update()
        {
            DarkMode = DataShareManager.Current.APPTheme == Windows.UI.Xaml.ElementTheme.Dark ? true : false;
            BigFont = DataShareManager.Current.BigFont;
            NoImagesMode = DataShareManager.Current.NOImagesMode;
            ShowToast = DataShareManager.Current.ShowToast;

            double size_cache = await FileHelper.Current.GetCacheSize();
            ClearCacheTitle = "清除缓存[" + GetFormatSize(size_cache) + "]";
        }

        public async void ClearCache()
        {
            IsBusy = true;
            await FileHelper.Current.DeleteCacheFile();
            double size_cache = await FileHelper.Current.GetCacheSize();
            ClearCacheTitle = "清除缓存[" + GetFormatSize(size_cache) + "]";
            IsBusy = false;
        }

        private void Current_ShareDataChanged()
        {
            DarkMode = DataShareManager.Current.APPTheme == Windows.UI.Xaml.ElementTheme.Dark ? true : false;
            BigFont = DataShareManager.Current.BigFont;
            NoImagesMode = DataShareManager.Current.NOImagesMode;
        }

        public void ExchangeDarkMode(bool dark)
        {
            DataShareManager.Current.UpdateAPPTheme(dark);
        }

        public void ExchangeBigFont(bool big_font)
        {
            DataShareManager.Current.UpdateBigFont(big_font);
        }

        public void ExchangeNoImagesMode(bool no_images)
        {
            DataShareManager.Current.UpdateNoImagesMode(no_images);
        }

        public void ExchangeShowToast(bool show_toast)
        {
            DataShareManager.Current.UpdateShowToast(show_toast);
        }
        private string GetFormatSize(double size)
        {
            if (size < 1024)
            {
                return size + "byte";
            }
            else if (size < 1024 * 1024)
            {
                return Math.Round(size / 1024, 2) + "KB";
            }
            else if (size < 1024 * 1024 * 1024)
            {
                return Math.Round(size / 1024 / 1024, 2) + "MB";
            }
            else
            {
                return Math.Round(size / 1024 / 1024 / 2014, 2) + "GB";
            }
        }
    }
}
