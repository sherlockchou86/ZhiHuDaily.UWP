using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationsExtensions.TileContent;
using Windows.UI.Notifications;
using ZhiHuDaily.UWP.Core.Https;
using ZhiHuDaily.UWP.Core.Models;

namespace ZhiHuDaily.UWP.Core.Tools
{
    public class BackgroundTaskAction
    {

        public async static Task UpdateTile()
        {
            LatestStories l = await FileHelper.Current.ReadObjectAsync<LatestStories>("latest_stories.json");
            if (l != null)
            {
                List<string> list = l.Top_Stories.ToList().Select(s => s.Title).ToList();
                List<string> list_image = l.Top_Stories.ToList().Select(s => s.Image).ToList();
                var tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
                var plannedNotifications = tileUpdater.GetScheduledTileNotifications();

                DateTime now = DateTime.Now;
                DateTime updateTime = now;
                DateTime planTill = now.AddMinutes(16);

                //tile更新计划还未完全执行完毕
                if (plannedNotifications.Count > 0)
                {
                    updateTime = plannedNotifications.Select(n => n.DeliveryTime.DateTime).Max();
                }

                int index = 0;

                //制定tile更新的计划
                for (var startPlanning = updateTime; startPlanning < planTill; startPlanning = startPlanning.AddSeconds(15))
                {
                    try
                    {
                        var tilexml = TileContentFactory.CreateTileSquare150x150PeekImageAndText02();
                        tilexml.Image.Src = list_image[index % 5];
                        tilexml.TextHeading.Text = "知乎日报UWP";
                        tilexml.TextBodyWrap.Text = list[index % 5];
                        index++;

                        ScheduledTileNotification stn = new ScheduledTileNotification(tilexml.GetXml(), startPlanning);
                        stn.ExpirationTime = startPlanning.AddSeconds(12);

                        tileUpdater.AddToSchedule(stn);
                    }
                    catch
                    {

                    }
                }
            }
        }

        public async static Task PullLatestStories()
        {
            APIService api = new APIService();
            var t = await api.GetLatestStories();
        }
    }
}
