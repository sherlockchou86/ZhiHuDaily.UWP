using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Popups;
using ZhiHuDaily.UWP.Background;
using ZhiHuDaily.UWP.Core.Https;
using ZhiHuDaily.UWP.Core.Tools;

namespace ZhiHuDaily.UWP.Mobile
{
    class BackgroundProxy
    {
        public async void Register()
        {
            var access = await BackgroundExecutionManager.RequestAccessAsync();
            if (access == BackgroundAccessStatus.Denied || access == BackgroundAccessStatus.Unspecified)
            {
                await new MessageDialog("系统关闭了后台运行，请前往‘系统设置’进行设置").ShowAsync();
                return;
            }
            RegisterPullLatestStoriesBackgroundTask();
            RegisterUpdateTileBackgroundTask();
        }
        /// <summary>
        /// 注册tile更新后台任务
        /// </summary>
        private void RegisterUpdateTileBackgroundTask()
        {
            string Task_NAME = "Update Tile Task";
            var taskReg = BackgroundTaskRegistration.AllTasks.Values.FirstOrDefault(t => t.Name == Task_NAME) as BackgroundTaskRegistration;
            if (taskReg != null)
            {
                return;
            }

            var task = new BackgroundTaskBuilder
            {
                Name = Task_NAME,
                TaskEntryPoint = typeof(ZhiHuDaily.UWP.Background.UpdateTileBackgroundExecution).FullName
            };

            var trigger = new SystemTrigger(SystemTriggerType.UserPresent, false);

            task.SetTrigger(trigger);
            var r = task.Register();
        }
        /// <summary>
        /// 注册拉取最新story后台任务
        /// </summary>
        private void RegisterPullLatestStoriesBackgroundTask()
        {
            string Task_NAME = "Pull Latest Stories Task";
            var taskReg = BackgroundTaskRegistration.AllTasks.Values.FirstOrDefault(t => t.Name == Task_NAME) as BackgroundTaskRegistration;
            if (taskReg != null)
            {
                return;
            }

            var task = new BackgroundTaskBuilder
            {
                Name = Task_NAME,
                TaskEntryPoint = typeof(ZhiHuDaily.UWP.Background.PullLatestStoriesBackgroundExecution).FullName
            };

            var trigger = new SystemTrigger(SystemTriggerType.UserAway, false);

            task.SetTrigger(trigger);
            var r = task.Register();
        }

    }
}
