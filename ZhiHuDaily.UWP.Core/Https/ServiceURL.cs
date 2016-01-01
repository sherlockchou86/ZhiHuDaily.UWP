using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiHuDaily.UWP.Core.Https
{
    /// <summary>
    /// 以下api参照android官方客户端  均是get方式请求  返回json、html 
    /// </summary>
    static class ServiceURL
    {
        /// <summary>
        /// 启动图片
        /// </summary>
        public static string StartImage = "http://news-at.zhihu.com/api/4/start-image/{0}";  //0 图片尺寸
        /// <summary>
        /// 主题列表
        /// </summary>
        public static string Themes = "http://news-at.zhihu.com/api/4/themes";
        /// <summary>
        /// 首页最新文章（包含头条文章）
        /// </summary>
        public static string LatestStories = "http://news-at.zhihu.com/api/4/stories/latest";
        /// <summary>
        /// 首页分页文章（按日期）
        /// </summary>
        public static string BeforeStories = "http://news-at.zhihu.com/api/4/stories/before/{0}";  //0日期 20151209
        /// <summary>
        /// 文章内容
        /// </summary>
        public static string Story = "http://news-at.zhihu.com/api/4/story/{0}";  //0 文章id
        /// <summary>
        /// 主题文章
        /// </summary>
        public static string ThemeStories = "http://news-at.zhihu.com/api/4/theme/{0}";  //0 主题id
        /// <summary>
        /// 分页获取主题文章
        /// </summary>
        public static string BeforeThemeStories = "http://news-at.zhihu.com/api/4/theme/{0}/before/{1}";  //0 主题编号 1 文章id
        /// <summary>
        /// 主编详细资料
        /// </summary>
        public static string EditorProfile = "http://news-at.zhihu.com/api/4/editor/{0}/profile-page/android";  //0 主编id
        /// <summary>
        /// 文章额外信息（评论数、推荐数等）
        /// </summary>
        public static string StoryExtra = "http://news-at.zhihu.com/api/4/story-extra/{0}";  //0 文章id
        /// <summary>
        /// 文章的推荐人
        /// </summary>
        public static string Recommenders = "http://news-at.zhihu.com/api/4/story/{0}/recommenders";  //文章id
        /// <summary>
        /// 长评论
        /// </summary>
        public static string LongComments = "http://news-at.zhihu.com/api/4/story/{0}/long-comments";  //0 文章id
        /// <summary>
        /// 分页获取长评论
        /// </summary>
        public static string BeforeLongComments = "http://news-at.zhihu.com/api/4/story/{0}/long-comments/before/{1}"; //0 文章id  1 评论id
        /// <summary>
        /// 短评论
        /// </summary>
        public static string ShortComments = "http://news-at.zhihu.com/api/4/story/{0}/short-comments";  //0 文章id
        /// <summary>
        /// 分页获取短评论
        /// </summary>
        public static string BeforeShortComments = "http://news-at.zhihu.com/api/4/story/{0}/short-comments/before/{1}";  //0 文章id  1 评论id
        /// <summary>
        /// 
        /// </summary>
        public static string Quotation = "http://files.cnblogs.com/files/xiaozhi_5638/ZhihuDaily_Quotation.zip";
    }
}
