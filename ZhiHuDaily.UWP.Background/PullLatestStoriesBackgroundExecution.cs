using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using ZhiHuDaily.UWP.Core.Https;
using ZhiHuDaily.UWP.Core.Tools;

namespace ZhiHuDaily.UWP.Background
{
    /// <summary>
    /// 后台任务  负责定时获取最新文章
    /// </summary>
    public sealed class PullLatestStoriesBackgroundExecution : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var d = taskInstance.GetDeferral();
            await BackgroundTaskAction.PullLatestStories();
            d.Complete();
        }

    }
}
