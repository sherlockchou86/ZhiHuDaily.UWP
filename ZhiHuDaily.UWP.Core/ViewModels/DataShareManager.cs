using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace ZhiHuDaily.UWP.Core.ViewModels
{
    /// <summary>
    /// 负责加载、保存、漫游配置数据  各个ViewModel之间共享此数据
    /// </summary>
    public sealed class DataShareManager
    {
        private ElementTheme _app_theme;
        public ElementTheme APPTheme
        {
            get
            {
                return _app_theme;
            }
        }

        private bool _big_font;
        public bool BigFont
        {
            get
            {
                return _big_font;
            }
        }

        private bool _no_imgaes_mode;
        public bool NOImagesMode
        {
            get
            {
                return _no_imgaes_mode;
            }
        }
        private List<string> _favorite_list;
        public List<string> FavoriteList
        {
            get
            {
                return _favorite_list;
            }
        }

        private List<string> _readed_list;
        public List<string> ReadedList
        {
            get
            {
                return _readed_list;
            }
        }

        private bool _show_toast;
        public bool ShowToast
        {
            get
            {
                return _show_toast;
            }
        }
        private static DataShareManager _current;
        public static DataShareManager Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new DataShareManager();
                }
                return _current;
            }
        }
        public event ShareDataChangedEventHandler ShareDataChanged;

        public DataShareManager()
        {
            LoadData();
        }

        private void LoadData()
        {
            var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            if (roamingSettings.Values.ContainsKey("APP_THEME"))
            {
                _app_theme = int.Parse(roamingSettings.Values["APP_THEME"].ToString()) == 0 ? ElementTheme.Light : ElementTheme.Dark;
            }
            else
            {
                _app_theme = ElementTheme.Light;
            }

            if (roamingSettings.Values.ContainsKey("BIG_FONT"))
            {
                _big_font = bool.Parse(roamingSettings.Values["BIG_FONT"].ToString());
            }
            else
            {
                _big_font = false;
            }
            if (roamingSettings.Values.ContainsKey("SHOW_TOAST"))
            {
                _show_toast = bool.Parse(roamingSettings.Values["SHOW_TOAST"].ToString());
            }
            else
            {
                _show_toast = true;
            }
            if (roamingSettings.Values.ContainsKey("NO_IMAGES_MODE"))
            {
                _no_imgaes_mode = bool.Parse(roamingSettings.Values["NO_IMAGES_MODE"].ToString());
            }
            else
            {
                _no_imgaes_mode = false;
            }

            if (roamingSettings.Values.ContainsKey("FAVORITE_LIST"))
            {
                _favorite_list = roamingSettings.Values["FAVORITE_LIST"].ToString().Split(',').ToList();
            }
            else
            {
                _favorite_list = new List<string>();
            }

            if (roamingSettings.Values.ContainsKey("READED_LIST"))
            {
                _readed_list = roamingSettings.Values["READED_LIST"].ToString().Split(',').ToList();
            }
            else
            {
                _readed_list = new List<string>();
            }
        }
        private void OnShareDataChanged()
        {
            if (ShareDataChanged != null)
            {
                ShareDataChanged();
            }
        }
        public void UpdateAPPTheme(bool dark)
        {
            _app_theme = dark ? ElementTheme.Dark : ElementTheme.Light;
            var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["APP_THEME"] = _app_theme == ElementTheme.Light ? 0 : 1;
            OnShareDataChanged();
        }
        public void UpdateBigFont(bool big_font)
        {
            _big_font = big_font;
            var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["BIG_FONT"] = _big_font;
            OnShareDataChanged();
        }
        public void UpdateShowToast(bool show_toast)
        {
            _show_toast = show_toast;
            var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["SHOW_TOAST"] = _show_toast;
            OnShareDataChanged();
        }
        public void UpdateNoImagesMode(bool no_images)
        {
            _no_imgaes_mode = no_images;
            var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["NO_IMAGES_MODE"] = _no_imgaes_mode;
            OnShareDataChanged();
        }
        public void UpdateFavorites(string favorite_id)
        {
            bool add = _favorite_list.Contains(favorite_id) ? false : true;
            if (add)
            {
                _favorite_list.Add(favorite_id);
            }
            else
            {
                _favorite_list.Remove(favorite_id);
            }
            var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["FAVORITE_LIST"] = string.Join(",", _favorite_list.ToArray());
            OnShareDataChanged();
        }
        public void UpdateReadeds(string readed_id)
        {
            if (!_readed_list.Contains(readed_id))
            {
                _readed_list.Add(readed_id);

                var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
                roamingSettings.Values["READED_LIST"] = string.Join(",", _readed_list.ToArray());
                OnShareDataChanged();
            }
        }
    }

    public delegate void ShareDataChangedEventHandler();
}
