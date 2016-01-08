using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Xaml.Media.Imaging;
using ZhiHuDaily.UWP.Core.Models;
using ZhiHuDaily.UWP.Core.Tools;
using ZhiHuDaily.UWP.Core.ViewModels;

namespace ZhiHuDaily.UWP.Core.Https
{
    /// <summary>
    /// api服务类  将接收到json字符串格式化成实体类
    /// </summary>
    public class APIService : APIBaseService
    {
        private string _local_path = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
        /// <summary>
        /// 启动图片
        /// </summary>
        /// <returns></returns>
        public async Task<StartImage> GetStartImage()
        {
            try
            {
                if (NetworkManager.Current.Network == 4)  //无网络
                {
                    StartImage si = await FileHelper.Current.ReadObjectAsync<StartImage>("start_image.json");
                    return si;
                }
                else
                {
                    string url = string.Format(ServiceURL.StartImage, "720*1280");
                    JsonObject json = await GetJson(url);string image_ext = "jpg"; string[] sitem = null;
                    if (json != null)
                    {
                        StartImage si = new StartImage { ImageURL = json["img"].GetString(), ImageText = json["text"].GetString() };
                        WriteableBitmap wb = await GetImage(si.ImageURL);  //加载图片
                        if (!si.ImageURL.Equals(""))
                        {
                            sitem = si.ImageURL.Split('.');
                            image_ext = sitem[sitem.Count() - 1];
                        }
                        await FileHelper.Current.SaveImageAsync(wb, "start_image." + image_ext);  //保存图片
                        si.ImageURL = _local_path + "\\images_cache\\start_image." + image_ext;  //重定向图片路径
                        await FileHelper.Current.WriteObjectAsync<StartImage>(si, "start_image.json"); //保存cache
                        return si;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 主题日报列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<Theme>> GetThemes()  
        {
            try
            {
                if (NetworkManager.Current.Network == 4)  //无网络连接
                {
                    List<Theme> list = await FileHelper.Current.ReadObjectAsync<List<Theme>>("themes.json");
                    return list;
                }
                else
                {
                    JsonObject json = await GetJson(ServiceURL.Themes);
                    if (json != null)
                    {
                        List<Theme> list = new List<Theme>();
                        if (json.ContainsKey("subscribed"))
                        {
                            var subscribed = json["subscribed"];
                            if (subscribed != null)
                            {
                                JsonArray ja = subscribed.GetArray();
                                foreach (var j in ja)
                                {
                                    list.Add(new Theme { ID = (j.GetObject())["id"].GetNumber().ToString(), Name = (j.GetObject())["name"].GetString(), Description = (j.GetObject())["description"].GetString(), Thumbnail = (j.GetObject())["thumbnail"].GetString() });
                                }
                            }
                        }
                        var others = json["others"];
                        if (others != null)
                        {
                            JsonArray ja = others.GetArray();
                            foreach (var j in ja)
                            {
                                list.Add(new Theme { ID = (j.GetObject())["id"].GetNumber().ToString(), Name = (j.GetObject())["name"].GetString(), Description = (j.GetObject())["description"].GetString(), Thumbnail = (j.GetObject())["thumbnail"].GetString() });
                            }
                        }
                        await FileHelper.Current.WriteObjectAsync<List<Theme>>(list, "themes.json");
                        return list;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 主页最近文章
        /// </summary>
        /// <returns></returns>
        public async Task<LatestStories> GetLatestStories()
        {
            try
            {
                if (NetworkManager.Current.Network == 4)
                {
                    LatestStories l = await FileHelper.Current.ReadObjectAsync<LatestStories>("latest_stories.json");
                    return l;
                }
                else
                {
                    JsonObject json = await GetJson(ServiceURL.LatestStories);
                    if (json != null)
                    {
                        ObservableCollection<Story> stories_list = new ObservableCollection<Story>();
                        ObservableCollection<Story> top_stories_list = new ObservableCollection<Story>();
                        Story tmp;
                        var stories = json["stories"];
                        WriteableBitmap wb = null; string image_ext = "jpg";string[] sitem = null;
                        if (stories != null)
                        {
                            JsonArray ja = stories.GetArray();
                            JsonObject jo; string image; bool multiPic = false;

                            foreach (var j in ja)
                            {
                                jo = j.GetObject();
                                multiPic = jo.ContainsKey("multipic") && (jo["multipic"].GetBoolean());
                                image = jo["images"].GetArray()[0].GetString();
                                tmp = new Story { Date = json["date"].GetString(), ID = jo["id"].GetNumber().ToString(), Image = image, Title = jo["title"].GetString(), MultiPic = multiPic };

                                if (!await FileHelper.Current.CacheExist(tmp.ID + "_story_image." + image_ext)) //没有缓存
                                {
                                    wb = await GetImage(tmp.Image);  //下载图片
                                    if (!tmp.Image.Equals(""))
                                    {
                                        sitem = tmp.Image.Split('.');
                                        image_ext = sitem[sitem.Count() - 1];
                                    }
                                    await FileHelper.Current.SaveImageAsync(wb, tmp.ID + "_story_image." + image_ext); //保存图片
                                }
                                if (!tmp.Image.Equals(""))
                                {
                                    tmp.Image = _local_path + "\\images_cache\\" + tmp.ID + "_story_image." + image_ext;  //图片路径重定向
                                }
                                //tmp.Favorite = DataShareManager.Current.FavoriteList.Contains(tmp.ID) ? true : false;  //是否收藏
                                //tmp.Readed = DataShareManager.Current.ReadedList.Contains(tmp.ID) ? true : false;  //是否已读
                                stories_list.Add(tmp);
                            }
                        }

                        var top_stories = json["top_stories"];
                        if (top_stories != null)
                        {
                            JsonArray ja = top_stories.GetArray();
                            JsonObject jo;
                            foreach (var j in ja)
                            {
                                jo = j.GetObject();
                                tmp = new Story { Date = json["date"].GetString(), Favorite = false, ID = jo["id"].GetNumber().ToString(), Image = jo["image"].GetString(), Title = jo["title"].GetString() };

                                if (!await FileHelper.Current.CacheExist(tmp.ID + "_story_top_image." + image_ext)) //没有缓存
                                {
                                    wb = await GetImage(tmp.Image);  //下载图片
                                    if (!tmp.Image.Equals(""))
                                    {
                                        sitem = tmp.Image.Split('.');
                                        image_ext = sitem[sitem.Count() - 1];
                                    }
                                    await FileHelper.Current.SaveImageAsync(wb, tmp.ID + "_story_top_image." + image_ext); //保存图片
                                }
                                if (!tmp.Image.Equals(""))
                                {
                                    tmp.Image = _local_path + "\\images_cache\\" + tmp.ID + "_story_top_image." + image_ext;  //图片路径重定向
                                }

                                top_stories_list.Add(tmp);
                            }
                        }
                        LatestStories ls = new LatestStories { Date = json["date"].GetString(), Stories = stories_list, Top_Stories = top_stories_list };
                        await FileHelper.Current.WriteObjectAsync<LatestStories>(ls, "latest_stories.json");
                        return ls;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 分页获取首页文章
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<List<Story>> GetBeforeStories(string date)
        {
            try
            {
                if (NetworkManager.Current.Network == 4)
                {
                    List<Story> l = await FileHelper.Current.ReadObjectAsync<List<Story>>(date + "_before_stories.json");
                    return l;
                }
                else
                {
                    string url = string.Format(ServiceURL.BeforeStories, date);
                    JsonObject json = await GetJson(url);
                    if (json != null)
                    {
                        List<Story> stories_list = new List<Story>();
                        var stories = json["stories"];
                        Story tmp;
                        WriteableBitmap wb = null; string image_ext = "jpg"; string[] sitem = null;
                        if (stories != null)
                        {
                            JsonArray ja = stories.GetArray();
                            JsonObject jo; string image; bool multiPic = false;
                            foreach (var j in ja)
                            {
                                jo = j.GetObject();
                                multiPic = jo.ContainsKey("multipic") && (jo["multipic"].GetBoolean());
                                image = jo["images"].GetArray()[0].GetString();
                                tmp = new Story { Date = json["date"].GetString(), ID = jo["id"].GetNumber().ToString(), Image = image, Title = jo["title"].GetString(), MultiPic = multiPic };

                                if (!await FileHelper.Current.CacheExist(tmp.ID + "_story_image." + image_ext) && !tmp.Image.Equals(""))  //没有缓存
                                {
                                    wb = await GetImage(tmp.Image);  //下载图片
                                    if (!tmp.Image.Equals(""))
                                    {
                                        sitem = tmp.Image.Split('.');
                                        image_ext = sitem[sitem.Count() - 1];
                                    }
                                    await FileHelper.Current.SaveImageAsync(wb, tmp.ID + "_story_image." + image_ext); //保存图片
                                }
                                if (!tmp.Image.Equals(""))
                                {
                                    tmp.Image = _local_path + "\\images_cache\\" + tmp.ID + "_story_image." + image_ext;  //图片路径重定向
                                }

                                //tmp.Favorite = DataShareManager.Current.FavoriteList.Contains(tmp.ID) ? true : false;  //是否收藏
                                //tmp.Readed = DataShareManager.Current.ReadedList.Contains(tmp.ID) ? true : false;  //是否已读
                                stories_list.Add(tmp);
                            }
                            await FileHelper.Current.WriteObjectAsync<List<Story>>(stories_list, date + "_before_stories.json");
                            return stories_list;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 主题日报中最近文章
        /// </summary>
        /// <param name="theme_id"></param>
        /// <returns></returns>
        public async Task<LatestThemeStories> GetLatestThemeStories(string theme_id)
        {
            try
            {
                if (NetworkManager.Current.Network == 4)
                {
                    LatestThemeStories lst = await FileHelper.Current.ReadObjectAsync<LatestThemeStories>(theme_id + "_latest_theme_stories.json");
                    return lst;
                }
                else
                {
                    string url = string.Format(ServiceURL.ThemeStories, theme_id);
                    JsonObject json = await GetJson(url);
                    if (json != null)
                    {
                        ObservableCollection<Editor> editors_list = new ObservableCollection<Editor>();
                        ObservableCollection<Story> stories_list = new ObservableCollection<Story>();

                        string background = json["background"].GetString();
                        string description = json["description"].GetString();
                        string image = json["image"].GetString();
                        string image_source = json["image_source"].GetString();
                        string name = json["name"].GetString();

                        WriteableBitmap wb = null;string image_ext = "jpg";string[] sitem = null;

                        if (json.ContainsKey("editors"))
                        {
                            var editors = json["editors"];
                            if (editors != null)
                            {
                                JsonObject jo;
                                JsonArray ja = editors.GetArray();
                                foreach (var jv in ja)
                                {
                                    jo = jv.GetObject();
                                    Editor tmp = new Editor
                                    {
                                        Avatar = jo.ContainsKey("avatar") ? jo["avatar"].GetString() : "",
                                        Bio = jo.ContainsKey("bio") ? jo["bio"].GetString() : "",
                                        ID = jo["id"].GetNumber().ToString(),
                                        Name = jo["name"].GetString(),
                                        URL = jo.ContainsKey("url") ? jo["url"].GetString() : ""
                                    };

                                    if (!await FileHelper.Current.CacheExist(tmp.ID + "_editor_avatar." + image_ext)) //没有缓存
                                    {
                                        wb = await GetImage(tmp.Avatar);  //下载图片
                                        if (!tmp.Avatar.Equals(""))
                                        {
                                            sitem = tmp.Avatar.Split('.');
                                            image_ext = sitem[sitem.Count() - 1];
                                        }
                                        await FileHelper.Current.SaveImageAsync(wb, tmp.ID + "_editor_avatar." + image_ext); //保存图片
                                    }
                                    if(!tmp.Avatar.Equals(""))  //重定向路径
                                    {
                                        tmp.Avatar = _local_path + "\\images_cache\\" + tmp.ID + "_editor_avatar." + image_ext;
                                    }
                                    editors_list.Add(tmp);
                                }
                            }
                        }
                        if (json.ContainsKey("stories"))
                        {
                            var stories = json["stories"];
                            Story tmp;
                            if (stories != null)
                            {
                                JsonObject jo;
                                JsonArray ja = stories.GetArray();
                                string image_s = "";
                                foreach (var jv in ja)
                                {
                                    image_s = "";
                                    jo = jv.GetObject();
                                    if (jo.ContainsKey("images"))
                                    {
                                        image_s = jo["images"].GetArray()[0].GetString();
                                    }
                                    tmp = new Story { ID = jo["id"].GetNumber().ToString(), Image = image_s, Title = jo["title"].GetString() };

                                    if (!await FileHelper.Current.CacheExist(tmp.ID + "_story_image." + image_ext) && !tmp.Image.Equals("")) //没有缓存
                                    {
                                        wb = await GetImage(tmp.Image);  //下载图片
                                        if (!tmp.Image.Equals(""))
                                        {
                                            sitem = tmp.Image.Split('.');
                                            image_ext = sitem[sitem.Count() - 1];
                                        }
                                        await FileHelper.Current.SaveImageAsync(wb, tmp.ID + "_story_image." + image_ext); //保存图片
                                    }
                                    if (!tmp.Image.Equals(""))
                                    {
                                        tmp.Image = _local_path + "\\images_cache\\" + tmp.ID + "_story_image." + image_ext;  //图片路径重定向
                                    }

                                    //tmp.Favorite = DataShareManager.Current.FavoriteList.Contains(tmp.ID) ? true : false;  //是否收藏
                                    //tmp.Readed = DataShareManager.Current.ReadedList.Contains(tmp.ID) ? true : false;  //是否已读
                                    stories_list.Add(tmp);
                                }
                            }
                        }
                        LatestThemeStories lts = new LatestThemeStories
                        {
                            Background = background,
                            Description = description,
                            Editors = editors_list,
                            Image = image,
                            Image_Source = image_source,
                            Name = name,
                            Stories = stories_list
                        };
                        await FileHelper.Current.WriteObjectAsync<LatestThemeStories>(lts, theme_id + "_latest_theme_stories.json");
                        return lts;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 分页获取主题日报中的文章
        /// </summary>
        /// <param name="theme_id"></param>
        /// <param name="last_story_id"></param>
        /// <returns></returns>
        public async Task<List<Story>> GetBeforeThemeStories(string theme_id, string last_story_id)
        {
            try
            {
                if (NetworkManager.Current.Network == 4)
                {
                    List<Story> l = await FileHelper.Current.ReadObjectAsync<List<Story>>(theme_id + "_" + last_story_id + "_before_theme_stories.json");
                    return l;
                }
                else
                {
                    string url = string.Format(ServiceURL.BeforeThemeStories, theme_id, last_story_id);
                    JsonObject json = await GetJson(url);
                    if (json != null)
                    {
                        List<Story> stories_list = new List<Story>();

                        var stories = json["stories"];
                        Story tmp;
                        if (stories != null)
                        {
                            JsonObject jo;
                            JsonArray ja = stories.GetArray();
                            string image_s = "";
                            WriteableBitmap wb = null; string image_ext = "jpg"; string[] sitem = null;

                            foreach (var jv in ja)
                            {
                                image_s = "";
                                jo = jv.GetObject();
                                if (jo.ContainsKey("images"))
                                {
                                    image_s = jo["images"].GetArray()[0].GetString();
                                }
                                tmp = new Story { ID = jo["id"].GetNumber().ToString(), Image = image_s, Title = jo["title"].GetString() };

                                if (!await FileHelper.Current.CacheExist(tmp.ID + "_story_image." + image_ext) && !tmp.Image.Equals(""))  //没有缓存
                                {
                                    wb = await GetImage(tmp.Image);  //下载图片
                                    if (!tmp.Image.Equals(""))
                                    {
                                        sitem = tmp.Image.Split('.');
                                        image_ext = sitem[sitem.Count() - 1];
                                    }
                                    await FileHelper.Current.SaveImageAsync(wb, tmp.ID + "_story_image." + image_ext); //保存图片
                                }
                                if (!tmp.Image.Equals(""))
                                {
                                    tmp.Image = _local_path + "\\images_cache\\" + tmp.ID + "_story_image." + image_ext;  //图片路径重定向
                                }

                                //tmp.Favorite = DataShareManager.Current.FavoriteList.Contains(tmp.ID) ? true : false;  //是否收藏
                                //tmp.Readed = DataShareManager.Current.ReadedList.Contains(tmp.ID) ? true : false;  //是否已读
                                stories_list.Add(tmp);
                            }
                        }
                        await FileHelper.Current.WriteObjectAsync<List<Story>>(stories_list, theme_id + "_" + last_story_id + "_before_theme_stories.json");
                        return stories_list;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 获取主编信息
        /// </summary>
        /// <param name="editor_id"></param>
        /// <returns></returns>
        public async Task<string> GetEditorInfo(string editor_id)
        {
            try
            {
                if (NetworkManager.Current.Network == 4)
                {
                    string ei = await FileHelper.Current.ReadObjectAsync<string>(editor_id + "_editor_info.json");
                    return ei;
                }
                else
                {
                    string url = string.Format(ServiceURL.EditorProfile, editor_id);
                    string html = await GetHtml(url);
                    if (html != null)
                    {
                        await FileHelper.Current.WriteObjectAsync<string>(html, editor_id + "_editor_info.json");
                        return html;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 获取文章内容
        /// </summary>
        /// <param name="story_id"></param>
        /// <returns></returns>
        public async Task<StoryContent> GetStoryContent(string story_id)
        {
            try
            {
                if (NetworkManager.Current.Network == 4)
                {
                    StoryContent sc = await FileHelper.Current.ReadObjectAsync<StoryContent>(story_id + "_story_content.json");
                    return sc;
                }
                else
                {
                    string url = string.Format(ServiceURL.Story, story_id);
                    JsonObject json = await GetJson(url);
                    if (json != null)
                    {
                        WriteableBitmap wb; string image_ext = "jpg"; string[] sitem = null;

                        string css = json["css"].GetArray()[0].GetString();
                        string id = json["id"].GetNumber().ToString();

                        string image = "", image_source = "";
                        ObservableCollection<string> recommenders = new ObservableCollection<string>();
                        string body;
                        if (json.ContainsKey("image"))
                        {
                            image = json["image"].GetString();

                            if (!await FileHelper.Current.CacheExist(story_id + "_story_big_image." + image_ext) && !image.Equals(""))  //没有缓存
                            {
                                wb = await GetImage(image);  //下载图片

                                if (!image.Equals(""))
                                {
                                    sitem = image.Split('.');
                                    image_ext = sitem[sitem.Count() - 1];
                                }
                                await FileHelper.Current.SaveImageAsync(wb, story_id + "_story_big_image." + image_ext); //保存图片
                            }
                            if (!image.Equals(""))
                            {
                                image = _local_path + "\\images_cache\\" + story_id + "_story_big_image." + image_ext;  //图片路径重定向
                            }
                        }
                        if (json.ContainsKey("image_source"))
                        {
                            image_source = json["image_source"].GetString();
                        }
                        string r_avatar = "";
                        if (json.ContainsKey("recommenders"))
                        {
                            JsonArray ja = json["recommenders"].GetArray(); int index = 0;
                            foreach (var jv in ja)
                            {
                                r_avatar = jv.GetObject()["avatar"].GetString();

                                if (!await FileHelper.Current.CacheExist(story_id + "_" + index + "_story_recommender_image." + image_ext))  //没有缓存
                                {
                                    wb = await GetImage(r_avatar);  //下载图片

                                    if (!r_avatar.Equals(""))
                                    {
                                        sitem = r_avatar.Split('.');
                                        image_ext = sitem[sitem.Count() - 1];
                                    }
                                    await FileHelper.Current.SaveImageAsync(wb, story_id + "_" + index + "_story_recommender_image." + image_ext); //保存图片
                                }
                                if (!r_avatar.Equals(""))
                                {
                                    r_avatar = _local_path + "\\images_cache\\" + story_id + "_" + index + "_story_recommender_image." + image_ext;  //图片路径重定向
                                }
                                index++;
                                recommenders.Add(r_avatar);
                            }
                        }
                        string title = json["title"].GetString();
                        string share_url = json["share_url"].GetString();

                        if (json.ContainsKey("body"))  //站内文章
                        {
                            body = json["body"].GetString();
                        }
                        else  //站外文章
                        {
                            body = await GetHtml(share_url);
                        }
                        bool readed = DataShareManager.Current.ReadedList.Contains(story_id) ? true : false;

                        StoryContent sc = new StoryContent
                        {
                            Body = body,
                            CSS = css,
                            ID = id,
                            Image = image,
                            Image_Source = image_source,
                            RecommnderAvatars = recommenders,
                            Share_URL = share_url,
                            Title = title,
                            Readed = readed
                        };
                        await FileHelper.Current.WriteObjectAsync<StoryContent>(sc, story_id + "_story_content.json");
                        return sc ;


                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 获取文章额外信息
        /// </summary>
        /// <param name="story_id"></param>
        /// <returns></returns>
        public async Task<StoryExtra> GetStoryExtra(string story_id)
        {
            try
            {
                if (NetworkManager.Current.Network == 4)
                {
                    StoryExtra se = await FileHelper.Current.ReadObjectAsync<StoryExtra>(story_id + "_story_extra.json");
                    return se;
                }
                else
                {
                    string url = string.Format(ServiceURL.StoryExtra, story_id);
                    JsonObject json = await GetJson(url);
                    if (json != null)
                    {
                        string comments = json["comments"].GetNumber().ToString();
                        string long_comments = json["long_comments"].GetNumber().ToString();
                        string short_comments = json["short_comments"].GetNumber().ToString();
                        string popularity = json["popularity"].GetNumber().ToString();


                        bool favorite = DataShareManager.Current.FavoriteList.Contains(story_id) ? true : false;

                        StoryExtra tmp = new StoryExtra { Comments = comments, LongComments = long_comments, Polularity = popularity, ShortComments = short_comments, Favorite = favorite };
                        await FileHelper.Current.WriteObjectAsync<StoryExtra>(tmp, story_id + "_story_extra.json");

                        return tmp; 
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 获取文章的推荐者
        /// </summary>
        /// <param name="story_id"></param>
        /// <returns></returns>
        public async Task<List<Recommender>> GetRecommenders(string story_id)
        {
            try
            {
                if (NetworkManager.Current.Network == 4)
                {
                    List<Recommender> lr = await FileHelper.Current.ReadObjectAsync<List<Recommender>>(story_id + "_recommenders.json");
                    return lr;
                }
                else
                {
                    string url = string.Format(ServiceURL.Recommenders, story_id);
                    JsonObject json = await GetJson(url);
                    if (json != null)
                    {
                        List<Recommender> list = new List<Recommender>();

                        string avatar = "", bio = "", id, name, zhihu_url_token = "";
                        JsonArray ja = null;
                        if (json.ContainsKey("items") && json["items"].GetArray().Count > 0)
                        {
                            ja = json["items"].GetArray()[0].GetObject()["recommenders"].GetArray();
                        }
                        else
                        {
                            ja = json["editors"].GetArray();
                        }
                        WriteableBitmap wb; string image_ext = "jpg"; string[] sitem = null;
                        foreach (var jv in ja)
                        {
                            json = jv.GetObject();
                            avatar = ""; bio = ""; zhihu_url_token = "";

                            if (json.ContainsKey("avatar"))
                            {
                                avatar = json["avatar"].GetString();
                            }
                            if (json.ContainsKey("bio"))
                            {
                                bio = json["bio"].GetString();
                            }
                            if (json.ContainsKey("zhihu_url_token"))
                            {
                                zhihu_url_token = json["zhihu_url_token"].GetString();
                            }
                            id = json["id"].GetNumber().ToString();
                            name = json["name"].GetString();

                            Recommender tmp = new Recommender { Avatar = avatar, Bio = bio, ID = id, Name = name, Zhihu_URL_Token = zhihu_url_token };

                            if (!await FileHelper.Current.CacheExist(tmp.ID + "_recommender_avatar." + image_ext)) //没有缓存
                            {
                                wb = await GetImage(tmp.Avatar);

                                if (!tmp.Avatar.Equals(""))
                                {
                                    sitem = tmp.Avatar.Split('.');
                                    image_ext = sitem[sitem.Count() - 1];
                                }
                                await FileHelper.Current.SaveImageAsync(wb, tmp.ID + "_recommender_avatar." + image_ext); //保存图片
                            }
                            if (!tmp.Avatar.Equals(""))
                            {
                                tmp.Avatar = _local_path + "\\images_cache\\" + tmp.ID + "_recommender_avatar." + image_ext;  //图片路径重定向
                            }
                            list.Add(tmp);
                        }
                        await FileHelper.Current.WriteObjectAsync<List<Recommender>>(list, story_id + "_recommenders.json");
                        return list;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 获取文章短评论
        /// </summary>
        /// <param name="story_id"></param>
        /// <returns></returns>
        public async Task<List<StoryComment>> GetShortComments(string story_id)
        {
            try
            {
                if (NetworkManager.Current.Network == 4)
                {
                    List<StoryComment> lsc = await FileHelper.Current.ReadObjectAsync<List<StoryComment>>(story_id + "_short_comments.json");
                    return lsc;
                }
                else
                {
                    string url = string.Format(ServiceURL.ShortComments, story_id);
                    JsonObject json = await GetJson(url);
                    if (json != null)
                    {
                        List<StoryComment> list = new List<StoryComment>();
                        if (json.ContainsKey("comments"))
                        {
                            JsonArray ja = json["comments"].GetArray();
                            JsonObject jo;
                            string author, avatar, content, id, time, likes; ReplyTo reply_to;
                            string reply_author, reply_content, reply_id;

                            WriteableBitmap wb; string image_ext = "jpg"; string[] sitem = null;
                            foreach (var jv in ja)
                            {
                                jo = jv.GetObject();
                                reply_to = null;

                                author = jo["author"].GetString();
                                avatar = jo["avatar"].GetString();
                                content = jo["content"].GetString();
                                id = jo["id"].GetNumber().ToString();
                                likes = jo["likes"].GetNumber().ToString();
                                time = jo["time"].GetNumber().ToString();

                                if (jo.ContainsKey("reply_to"))
                                {
                                    try
                                    {
                                        reply_author = jo["reply_to"].GetObject()["author"].GetString();
                                        reply_content = jo["reply_to"].GetObject()["content"].GetString();
                                        reply_id = jo["reply_to"].GetObject()["id"].GetNumber().ToString();

                                        reply_to = new ReplyTo { Author = reply_author, Content = reply_content, ID = reply_id };
                                    }
                                    catch
                                    {

                                    }
                                }
                                StoryComment tmp = new StoryComment
                                {
                                    Author = author,
                                    Avatar = avatar,
                                    Content = content,
                                    ID = id,
                                    Likes = likes,
                                    Target = reply_to,
                                    Time = time
                                };

                                if (!await FileHelper.Current.CacheExist(tmp.ID + "_comment_image." + image_ext))  //没有缓存
                                {
                                    wb = await GetImage(tmp.Avatar);

                                    if (!tmp.Avatar.Equals(""))
                                    {
                                        sitem = tmp.Avatar.Split('.');
                                        image_ext = sitem[sitem.Count() - 1];
                                    }
                                    await FileHelper.Current.SaveImageAsync(wb, tmp.ID + "_comment_image." + image_ext); //保存图片
                                }
                                if (!tmp.Avatar.Equals(""))
                                {
                                    tmp.Avatar = _local_path + "\\images_cache\\" + tmp.ID + "_comment_image." + image_ext;  //图片路径重定向
                                }
                                list.Add(tmp);
                            }
                        }
                        await FileHelper.Current.WriteObjectAsync<List<StoryComment>>(list, story_id + "_short_comments.json");
                        return list;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 获取文章长评论
        /// </summary>
        /// <param name="story_id"></param>
        /// <returns></returns>
        public async Task<List<StoryComment>> GetLongComments(string story_id)
        {
            try
            {
                if (NetworkManager.Current.Network == 4)
                {
                    List<StoryComment> lsc = await FileHelper.Current.ReadObjectAsync<List<StoryComment>>(story_id + "_long_comments.json");
                    return lsc;
                }
                else
                {
                    string url = string.Format(ServiceURL.LongComments, story_id);
                    JsonObject json = await GetJson(url);
                    if (json != null)
                    {
                        List<StoryComment> list = new List<StoryComment>();
                        if (json.ContainsKey("comments"))
                        {
                            JsonArray ja = json["comments"].GetArray();
                            JsonObject jo;
                            string author, avatar, content, id, time, likes; ReplyTo reply_to;
                            string reply_author, reply_content, reply_id;

                            WriteableBitmap wb; string image_ext = "jpg"; string[] sitem = null;
                            foreach (var jv in ja)
                            {
                                jo = jv.GetObject();
                                reply_to = null;

                                author = jo["author"].GetString();
                                avatar = jo["avatar"].GetString();
                                content = jo["content"].GetString();
                                id = jo["id"].GetNumber().ToString();
                                likes = jo["likes"].GetNumber().ToString();
                                time = jo["time"].GetNumber().ToString();

                                if (jo.ContainsKey("reply_to"))
                                {
                                    try
                                    {
                                        reply_author = jo["reply_to"].GetObject()["author"].GetString();
                                        reply_content = jo["reply_to"].GetObject()["content"].GetString();
                                        reply_id = jo["reply_to"].GetObject()["id"].GetNumber().ToString();

                                        reply_to = new ReplyTo { Author = reply_author, Content = reply_content, ID = reply_id };
                                    }
                                    catch
                                    {

                                    }
                                }
                                StoryComment tmp = new StoryComment
                                {
                                    Author = author,
                                    Avatar = avatar,
                                    Content = content,
                                    ID = id,
                                    Likes = likes,
                                    Target = reply_to,
                                    Time = time
                                };

                                if (!await FileHelper.Current.CacheExist(tmp.ID + "_comment_image." + image_ext)) //没有缓存
                                {
                                    wb = await GetImage(tmp.Avatar);

                                    if (!tmp.Avatar.Equals(""))
                                    {
                                        sitem = tmp.Avatar.Split('.');
                                        image_ext = sitem[sitem.Count() - 1];
                                    }
                                    await FileHelper.Current.SaveImageAsync(wb, tmp.ID + "_comment_image." + image_ext); //保存图片
                                }
                                if (!tmp.Avatar.Equals(""))
                                {
                                    tmp.Avatar = _local_path + "\\images_cache\\" + tmp.ID + "_comment_image." + image_ext;  //图片路径重定向
                                }
                                list.Add(tmp);
                            }
                        }
                        await FileHelper.Current.WriteObjectAsync<List<StoryComment>>(list, story_id + "_long_comments.json");
                        return list;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 分页获取文章短评论
        /// </summary>
        /// <param name="story_id"></param>
        /// <param name="last_comment_id"></param>
        /// <returns></returns>
        public async Task<List<StoryComment>> GetBeforeShortComments(string story_id, string last_comment_id)
        {
            try
            {
                if (NetworkManager.Current.Network == 4)
                {
                    List<StoryComment> lsc = await FileHelper.Current.ReadObjectAsync<List<StoryComment>>(story_id + "_" + last_comment_id + "_before_short_comments.json");
                    return lsc;
                }
                else
                {
                    string url = string.Format(ServiceURL.BeforeShortComments, story_id, last_comment_id);
                    JsonObject json = await GetJson(url);
                    if (json != null)
                    {
                        List<StoryComment> list = new List<StoryComment>();
                        if (json.ContainsKey("comments"))
                        {
                            JsonArray ja = json["comments"].GetArray();
                            JsonObject jo;
                            string author, avatar, content, id, time, likes; ReplyTo reply_to;
                            string reply_author, reply_content, reply_id;

                            WriteableBitmap wb; string image_ext = "jpg"; string[] sitem = null;
                            foreach (var jv in ja)
                            {
                                jo = jv.GetObject();
                                reply_to = null;

                                author = jo["author"].GetString();
                                avatar = jo["avatar"].GetString();
                                content = jo["content"].GetString();
                                id = jo["id"].GetNumber().ToString();
                                likes = jo["likes"].GetNumber().ToString();
                                time = jo["time"].GetNumber().ToString();

                                if (jo.ContainsKey("reply_to"))
                                {
                                    try
                                    {
                                        reply_author = jo["reply_to"].GetObject()["author"].GetString();
                                        reply_content = jo["reply_to"].GetObject()["content"].GetString();
                                        reply_id = jo["reply_to"].GetObject()["id"].GetNumber().ToString();

                                        reply_to = new ReplyTo { Author = reply_author, Content = reply_content, ID = reply_id };
                                    }
                                    catch
                                    {

                                    }
                                }

                                StoryComment tmp = new StoryComment
                                {
                                    Author = author,
                                    Avatar = avatar,
                                    Content = content,
                                    ID = id,
                                    Likes = likes,
                                    Target = reply_to,
                                    Time = time
                                };

                                if (!await FileHelper.Current.CacheExist(tmp.ID + "_comment_image." + image_ext)) //没有缓存
                                {
                                    wb = await GetImage(tmp.Avatar);

                                    if (!tmp.Avatar.Equals(""))
                                    {
                                        sitem = tmp.Avatar.Split('.');
                                        image_ext = sitem[sitem.Count() - 1];
                                    }
                                    await FileHelper.Current.SaveImageAsync(wb, tmp.ID + "_comment_image." + image_ext); //保存图片
                                }
                                if (!tmp.Avatar.Equals(""))
                                {
                                    tmp.Avatar = _local_path + "\\images_cache\\" + tmp.ID + "_comment_image." + image_ext;  //图片路径重定向
                                }
                                list.Add(tmp);
                            }
                        }
                        await FileHelper.Current.WriteObjectAsync<List<StoryComment>>(list, story_id + "_" + last_comment_id + "_before_short_comments.json");
                        return list;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 分页获取文章长评论
        /// </summary>
        /// <param name="story_id"></param>
        /// <param name="last_comment_id"></param>
        /// <returns></returns>
        public async Task<List<StoryComment>> GetBeforeLongComments(string story_id, string last_comment_id)
        {
            try
            {
                if (NetworkManager.Current.Network == 4)
                {
                    List<StoryComment> lsc = await FileHelper.Current.ReadObjectAsync<List<StoryComment>>(story_id + "_" + last_comment_id + "_before_long_comments.json");
                    return lsc;
                }
                else
                {
                    string url = string.Format(ServiceURL.BeforeLongComments, story_id, last_comment_id);
                    JsonObject json = await GetJson(url);
                    if (json != null)
                    {
                        List<StoryComment> list = new List<StoryComment>();
                        if (json.ContainsKey("comments"))
                        {
                            JsonArray ja = json["comments"].GetArray();
                            JsonObject jo;
                            string author, avatar, content, id, time, likes; ReplyTo reply_to;
                            string reply_author, reply_content, reply_id;

                            WriteableBitmap wb; string image_ext = "jpg"; string[] sitem = null;
                            foreach (var jv in ja)
                            {
                                jo = jv.GetObject();
                                reply_to = null;

                                author = jo["author"].GetString();
                                avatar = jo["avatar"].GetString();
                                content = jo["content"].GetString();
                                id = jo["id"].GetNumber().ToString();
                                likes = jo["likes"].GetNumber().ToString();
                                time = jo["time"].GetNumber().ToString();

                                if (jo.ContainsKey("reply_to"))
                                {
                                    try
                                    {
                                        reply_author = jo["reply_to"].GetObject()["author"].GetString();
                                        reply_content = jo["reply_to"].GetObject()["content"].GetString();
                                        reply_id = jo["reply_to"].GetObject()["id"].GetNumber().ToString();

                                        reply_to = new ReplyTo { Author = reply_author, Content = reply_content, ID = reply_id };
                                    }
                                    catch
                                    {

                                    }
                                }
                                StoryComment tmp = new StoryComment
                                {
                                    Author = author,
                                    Avatar = avatar,
                                    Content = content,
                                    ID = id,
                                    Likes = likes,
                                    Target = reply_to,
                                    Time = time
                                };

                                if (!await FileHelper.Current.CacheExist(tmp.ID + "_comment_image." + image_ext)) //没有缓存
                                {
                                    wb = await GetImage(tmp.Avatar);

                                    if (!tmp.Avatar.Equals(""))
                                    {
                                        sitem = tmp.Avatar.Split('.');
                                        image_ext = sitem[sitem.Count() - 1];
                                    }
                                    await FileHelper.Current.SaveImageAsync(wb, tmp.ID + "_comment_image." + image_ext); //保存图片
                                }
                                if (!tmp.Avatar.Equals(""))
                                {
                                    tmp.Avatar = _local_path + "\\images_cache\\" + tmp.ID + "_comment_image." + image_ext;  //图片路径重定向
                                }
                                list.Add(tmp);
                            }
                        }
                        await FileHelper.Current.WriteObjectAsync<List<StoryComment>>(list, story_id + "_" + last_comment_id + "_before_long_comments.json");
                        return list;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<ZhihuDailyQuotation> GetQuotationInfo()
        {
            try
            {
                if (NetworkManager.Current.Network == 4)
                {
                    ZhihuDailyQuotation quo = await FileHelper.Current.ReadObjectAsync<ZhihuDailyQuotation>("zhihuquotation.json");
                    return quo;
                }
                else
                {
                    string url = ServiceURL.Quotation + "?t=" + DateTime.Now.ToFileTime();
                    JsonObject json = await GetJson(url);
                    if (json != null)
                    {
                        string text = json["text"].GetString();
                        string sub_text = json["sub_text"].GetString();
                        string logo_back_light = json["logo_background_light"].GetString();
                        string logo_back_dark = json["logo_background_dark"].GetString();

                        ZhihuDailyQuotation quo = new ZhihuDailyQuotation { Title = text, SubTitle = sub_text, LogoBackDark = logo_back_dark, LogoBackLight = logo_back_light };

                        var t1 = GetImage(logo_back_dark);  //下载图片
                        var t2 = GetImage(logo_back_light);

                        WriteableBitmap wb_dark = await t1;
                        WriteableBitmap wb_light = await t2;

                        var t3 = FileHelper.Current.SaveImageAsync(wb_dark, "logo_back_dark.jpg");  //保存图片
                        var t4 = FileHelper.Current.SaveImageAsync(wb_light, "logo_back_light.jpg");

                        await t3;await t4;

                        if (!quo.LogoBackDark.Equals(""))
                            quo.LogoBackDark = _local_path + "\\images_cache\\logo_back_dark.jpg"; //图片重定向
                        if (!quo.LogoBackLight.Equals(""))
                            quo.LogoBackLight = _local_path + "\\images_cache\\logo_back_light.jpg";

                        await FileHelper.Current.WriteObjectAsync<ZhihuDailyQuotation>(quo, "zhihuquotation.json"); //保存cache

                        return quo;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}

