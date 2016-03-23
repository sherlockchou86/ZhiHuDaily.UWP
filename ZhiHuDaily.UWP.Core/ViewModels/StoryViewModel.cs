using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZhiHuDaily.UWP.Core.Https;
using ZhiHuDaily.UWP.Core.Models;
using ZhiHuDaily.UWP.Core.Share;
using ZhiHuDaily.UWP.Core.Tools;

namespace ZhiHuDaily.UWP.Core.ViewModels
{
    public class StoryViewModel:ViewModelBase
    {
        private APIService _api = new APIService();
        private string _story_id;
        private StoryExtra _se;
        public StoryExtra SE
        {
            get
            {
                return _se;
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
        private string _id;
        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
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
                OnPropertyChanged();
            }
        }

        private string _image;
        public string Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                OnPropertyChanged();
            }
        }

        private string _image_source;
        public string ImageSource
        {
            get
            {
                return _image_source;
            }
            set
            {
                _image_source = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _recommender_avatars;
        public ObservableCollection<string> RecommenderAvatars
        {
            get
            {
                return _recommender_avatars;
            }
            set
            {
                _recommender_avatars = value;
                OnPropertyChanged();
            }
        }

        private string _body_html;
        public string BodyHtml
        {
            get
            {
                return _body_html;
            }
            set
            {
                _body_html = value;
                OnPropertyChanged();
            }
        }

        private string _share_url;
        public string ShareUrl
        {
            get
            {
                return _share_url;
            }
            set
            {
                _share_url = value;
                OnPropertyChanged();
            }
        }

        private string _comments;
        public string Comments
        {
            get
            {
                return _comments;
            }
            set
            {
                _comments = value;
                OnPropertyChanged();
            }
        }
        private string _popularities;
        public string Popularities
        {
            get
            {
                return _popularities;
            }
            set
            {
                _popularities = value;
                OnPropertyChanged();
            }
        }
        private bool _favorite;
        public bool Favorite
        {
            get
            {
                return _favorite;
            }
            set
            {
                _favorite = value;
                OnPropertyChanged();
            }
        }

        private Story _story;
        public StoryViewModel(Story story)
        {
            _story_id = story.ID;
            _story = story;
            DataShareManager.Current.ShareDataChanged += Current_ShareDataChanged;
            Update();

            ExchangeReaded(story);
        }

        private void Current_ShareDataChanged()
        {
            if (DataShareManager.Current.FavoriteList.Contains(_story_id))
            {
                Favorite = true;
            }
            else
            {
                Favorite = false;
            }
        }

        public async void Update()
        {
            IsLoading = true;

            var t1 = _api.GetStoryContent(_story_id);
            var t2 = _api.GetStoryExtra(_story_id);

            StoryContent sc = await t1;
            StoryExtra se = await t2;

            if (sc != null)
            {
                ID = sc.ID;
                Title = sc.Title;
                Image = sc.Image;
                ImageSource = sc.Image_Source;
                RecommenderAvatars = sc.RecommnderAvatars;
                ShareUrl = sc.Share_URL;
                string css = "<style>"
                        + "html{-ms-content-zooming:none;font-family:微软雅黑;}"
                        + ".author{font-weight:bold;} .bio{color:gray;}"
                        + "body{padding:20px;word-break:break-all;} p{margin:30px auto;} a{color:skyblue;} .content img{width:95%;}"
                        + "body{line-height:150%;}"
                        + "</style>";   //基础css
                string ex_mark = "<base target='_blank'/>";
                string css2 = "";   //主题css
                string css3 = "";   //字体css
                string js = "";   //图片加载脚本
                string body = "";

                if (DataShareManager.Current.APPTheme == Windows.UI.Xaml.ElementTheme.Dark) //夜间主题
                {
                    css2 = "<style>" 
                        + "body{background-color:black !important;color:gray !important;}"
                        + "</style>";
                }
                else
                {
                    css2 = "";
                }
                if (DataShareManager.Current.BigFont)  //大字号
                {
                    
                    css3 = "<style>body{font-size:52px;} h1{font-size:62px;} h2{font-size:58px;} h3{font-size:52px;} h4,h5,h6{font-size:48px;} blockquote{font-size:48px!;}</style>";
                }
                else
                {
                    css3 = "<style>body{font-size:44px;} h1{font-size:55px;} h2{font-size:50px;} h3{font-size:45px;} h4,h5,h6{font-size:40px;} blockquote{font-size:40px!;}</style>";
                }
                
                if (DataShareManager.Current.NOImagesMode)  //无图模式
                {
                    if (NetworkManager.Current.Network != 3)  //非wifi
                    {
                        body = Regex.Replace(sc.Body, @"<img.*?src=(['""]?)(?<url>[^'"" ]+)(?=\1)[^>]*>", (m) =>
                         {
                             if (m.Value.Contains("avatar"))
                             {
                                 return m.Value;
                             }
                             else
                             {
                                 Match match = Regex.Match(m.Value.ToString(), @"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>");
                                 if (match.Success)
                                 {
                                     return @"<img src=""ms-appx-web:///Assets/default_image.png"" onclick=""click2loadimage(this,'" + match.Groups["imgUrl"].Value + @"');""/>";
                                 }
                                 else
                                 {
                                     return m.Value;
                                 }
                             }
                         }, RegexOptions.IgnoreCase);  //替换所有img标签 为本地图片

                        js = "<script>"  //点击加载图片
                            + "function click2loadimage(obj,source)"
                            + "{"
                            + "obj.setAttribute('src','ms-appx-web:///Assets/default_image_loading.png');"
                            + "obj.setAttribute('src',source);"
                            + "}"
                            + "</script>";
                    }
                    else
                    {
                        body = sc.Body;
                    }
                }
                else
                {
                    body = sc.Body;
                }
                // <link rel='stylesheet' type='text/css' href='" + sc.CSS + "'/> 官方css文件不好控制  所以没有使用 

                //合并
                BodyHtml = "<html><head>" + ex_mark + css + css2 + css3 + js + "</head>" + "<body>" +  body.Replace("<blockquote>","<p>").Replace("</blockquote>","</p>") + "</body></html>";  //附加css
            }
            if (se != null)
            {
                Comments = se.Comments;
                Popularities = se.Polularity;
                Favorite = se.Favorite;
                _se = se;
            }

            IsLoading = false;
        }

        public void ExchangeFavorite()
        {
            _story.Favorite = !_story.Favorite;
            DataShareManager.Current.UpdateFavorites(_story_id);
        }

        public void ExchangeReaded(Story story)
        {
            story.Readed = true;
            DataShareManager.Current.UpdateReadeds(_story_id);
        }

        public async void Share2Timeline()
        {
            var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/StoreLogo.png"));
            using (var stream = await file.OpenReadAsync())
            {
                var pic = new byte[stream.Size];
                await stream.AsStream().ReadAsync(pic, 0, pic.Length);
                WeChatRequest req = new WeChatRequest("123456789");
                req.WebPageShare2TimelineRequest(ShareUrl, Title, pic);
            }
        }

        public async void Share2Session()
        {
            var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/StoreLogo.png"));
            using (var stream = await file.OpenReadAsync())
            {
                var pic = new byte[stream.Size];
                await stream.AsStream().ReadAsync(pic, 0, pic.Length);
                WeChatRequest req = new WeChatRequest("123456789");
                req.WebPageShare2SessionRequest(ShareUrl, Title, pic);
            }
        }
    }
}
