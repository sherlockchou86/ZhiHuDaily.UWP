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
        /// <summary>
        /// 注册后台任务
        /// </summary>
        public async void Register()
        {
            BackgroundExecutionManager.RemoveAccess();
            var access = await BackgroundExecutionManager.RequestAccessAsync();
            if (access == BackgroundAccessStatus.Denied || access == BackgroundAccessStatus.Unspecified)
            {
                await new MessageDialog("系统关闭了后台运行，请前往‘系统设置’进行设置").ShowAsync();
                return;
            }
            RegisterUpdateTopStoriesBackgroundTask();
        }
        /// <summary>
        /// 屏幕点亮时执行该任务
        /// </summary>
        private void RegisterUpdateTopStoriesBackgroundTask()
        {
            string Task_NAME = "Update Task";
            var taskReg = BackgroundTaskRegistration.AllTasks.Values.FirstOrDefault(t => t.Name == Task_NAME) as BackgroundTaskRegistration;
            if (taskReg != null)
            {
                return;
            }

            var task = new BackgroundTaskBuilder
            {
                Name = Task_NAME,
                TaskEntryPoint = typeof(ZhiHuDaily.UWP.Background.UpdateTopStoriesBackgroundExecution).FullName
            };

            var trigger = new SystemTrigger(SystemTriggerType.UserPresent, false);

            task.SetTrigger(trigger);
            var r = task.Register();
        }
    }
}
