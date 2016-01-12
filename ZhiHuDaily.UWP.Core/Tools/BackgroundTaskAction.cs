using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationsExtensions.BadgeContent;
using NotificationsExtensions.TileContent;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;
using ZhiHuDaily.UWP.Core.Https;
using ZhiHuDaily.UWP.Core.Models;
using ZhiHuDaily.UWP.Core.ViewModels;

namespace ZhiHuDaily.UWP.Core.Tools
{
    public class BackgroundTaskAction
    {
        /// <summary>
        /// 后台任务 提示头条文章信息
        /// </summary>
        /// <returns></returns>
        public async static Task UpdateTopStories()
        {
            APIService api = new APIService();
            var t = await api.GetLatestStories(true);
            if (t != null)
            {
                UpdateTile(t);
                UpdateBadge(t);
                ShowToast(t);
            }
        }
        /// <summary>
        /// 更新头条文章的磁贴tile
        /// </summary>
        /// <param name="t"></param>
        private static void UpdateTile(LatestStories t)
        {
            //头5条 循环更新到tile中
            List<string> list = t.Top_Stories.ToList().Select(s => s.Title).ToList();
            List<string> list_image = t.Top_Stories.ToList().Select(s => s.Image).ToList();

            var tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
            var plannedNotifications = tileUpdater.GetScheduledTileNotifications();

            DateTime now = DateTime.Now;
            DateTime updateTime = now;
            DateTime planTill = now.AddMinutes(60);  //每次制定1小时的tile更新计划

            //删除还未执行的计划
            if (plannedNotifications.Count > 0)
            {
                plannedNotifications.ToList().ForEach(s => tileUpdater.RemoveFromSchedule(s));
            }

            int index = 0;
            int count = list.Count;

            //制定tile更新的计划
            for (var startPlanning = updateTime; startPlanning < planTill; startPlanning = startPlanning.AddSeconds(10))
            {
                try
                {
                    var tilexml = TileContentFactory.CreateTileSquare150x150PeekImageAndText02();
                    tilexml.Image.Src = list_image[index % count];
                    tilexml.TextHeading.Text = "知乎日报UWP";
                    tilexml.TextBodyWrap.Text = list[index % count];
                    index++;

                    ScheduledTileNotification stn = new ScheduledTileNotification(tilexml.GetXml(), startPlanning);
                    stn.ExpirationTime = startPlanning.AddSeconds(8);

                    tileUpdater.AddToSchedule(stn);
                }
                catch
                {

                }
            }
        }
        /// <summary>
        /// 更新未读头条文章的徽章badge
        /// </summary>
        /// <param name="t"></param>
        private static void UpdateBadge(LatestStories t)
        {
            //将当天未读TOP文章更新到 badge
            int un_readed = 0;
            t.Top_Stories.ToList().ForEach(s => { if (!DataShareManager.Current.ReadedList.Contains(s.ID)) un_readed++; });

            var updater = BadgeUpdateManager.CreateBadgeUpdaterForApplication();
            if (un_readed != 0)
            {
                var badgexml = new BadgeNumericNotificationContent((uint)un_readed);

                var n = badgexml.CreateNotification();
                n.ExpirationTime = DateTime.Now.AddDays(7);

                updater.Update(n);
            }
            else
            {
                updater.Clear();
            }

        }
        /// <summary>
        /// 显示未读头条文章的toast
        /// </summary>
        /// <param name="t"></param>
        private static void ShowToast(LatestStories t)
        {
            //将还未阅读的头条文章使用toast方式通知给用户
            var toast_updater = ToastNotificationManager.CreateToastNotifier();
            string xml = "<toast lang=\"zh-CN\" launch='111' >" +
                         "<visual>" +
                            "<binding template=\"ToastGeneric\">" +
                                "<text>知乎日报UWP 头条文章</text>" +
                                "<text>{0}</text>" +
                            "</binding>" +
                         "</visual>" +
                        "</toast>";
            t.Top_Stories.ToList().ForEach(s =>
            {
                if (!DataShareManager.Current.ReadedList.Contains(s.ID) && !IsAlreadyToast(s.ID))  //未阅读
                {
                    // 加载XML文档
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(string.Format(xml, s.Title));
                    // 显示通知
                    ToastNotification notification = new ToastNotification(doc);
                    toast_updater.Show(notification);
                }
            });

        }


        private static bool IsAlreadyToast(string id)
        {
            var c = ApplicationData.Current.LocalSettings.Values["Already_Toast"];
            if (c == null)
            {
                ApplicationData.Current.LocalSettings.Values["Already_Toast"] = id;
                return false;
            }
            else
            {
                var list = ApplicationData.Current.LocalSettings.Values["Already_Toast"].ToString().Split(',').ToList();
                if (list.Contains(id))
                {
                    return true;
                }
                else
                {
                    list.Add(id);
                    ApplicationData.Current.LocalSettings.Values["Already_Toast"] = string.Join(",", list);
                    return false;
                }
            }
        }
    }
}
