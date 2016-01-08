using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationsExtensions.TileContent;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Notifications;
using ZhiHuDaily.UWP.Core.Models;
using ZhiHuDaily.UWP.Core.Tools;

namespace ZhiHuDaily.UWP.Background
{
    /// <summary>
    /// 后台任务 负责将TopStory的Title更新到屏幕tile中
    /// </summary>
    public sealed class UpdateTileBackgroundExecution : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var d = taskInstance.GetDeferral();
            await BackgroundTaskAction.UpdateTile();
            d.Complete();
        }
    }
}
