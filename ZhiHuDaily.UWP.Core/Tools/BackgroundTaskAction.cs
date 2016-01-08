using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationsExtensions.BadgeContent;
using NotificationsExtensions.TileContent;
using Windows.UI.Notifications;
using ZhiHuDaily.UWP.Core.Https;
using ZhiHuDaily.UWP.Core.Models;
using ZhiHuDaily.UWP.Core.ViewModels;

namespace ZhiHuDaily.UWP.Core.Tools
{
    public class BackgroundTaskAction
    {
        /// <summary>
        /// 更新tile
        /// </summary>
        /// <returns></returns>
        public async static Task UpdateTile()
        {
            await PullLatestStories();
            LatestStories l = await FileHelper.Current.ReadObjectAsync<LatestStories>("latest_stories.json");
            if (l != null)
            {
                //头5条 在最前面
                List<string> list = l.Top_Stories.ToList().Select(s => s.Title).ToList();
                List<string> list_image = l.Top_Stories.ToList().Select(s => s.Image).ToList();

                //当天
                List<string> list2 = l.Stories.ToList().Select(s => s.Title).ToList();
                List<string> list_image2 = l.Stories.ToList().Select(s => s.Image).ToList();

                int c = list2.Count;
                for (int i = 0; i < c; ++i)
                {
                    if (!list.Contains(list2[i]))
                    {
                        list.Add(list2[i]);
                        list_image.Add(list_image2[i]);
                    }
                }

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
                for (var startPlanning = updateTime; startPlanning < planTill; startPlanning = startPlanning.AddSeconds(8))
                {
                    try
                    {
                        var tilexml = TileContentFactory.CreateTileSquare150x150PeekImageAndText02();
                        tilexml.Image.Src = list_image[index % count];
                        tilexml.TextHeading.Text = "知乎日报UWP";
                        tilexml.TextBodyWrap.Text = list[index % count];
                        index++;

                        ScheduledTileNotification stn = new ScheduledTileNotification(tilexml.GetXml(), startPlanning);
                        stn.ExpirationTime = startPlanning.AddSeconds(6);

                        tileUpdater.AddToSchedule(stn);
                    }
                    catch
                    {

                    }
                }
            }
        }
        /// <summary>
        /// 拉取最新文章
        /// </summary>
        /// <returns></returns>
        public async static Task PullLatestStories()
        {
            APIService api = new APIService();
            var t = await api.GetLatestStories();

            //将当天未读文章更新到 badge
            int un_readed = 0;
            
            t.Stories.ToList().ForEach(s => { if (!DataShareManager.Current.ReadedList.Contains(s.ID)) un_readed++; });


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
    }
}
