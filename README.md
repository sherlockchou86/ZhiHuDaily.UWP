#ZhiHuDaily.UWP#
第三方知乎日报UWP版客户端。

##**说明**##
1. APP中使用到的Web API与知乎日报andorid版一致，详细参见[博客](http://www.cnblogs.com/xiaozhi_5638/p/5056217.html)
或者[这里](https://github.com/sherlockchou86/ZhiHuDaily.UWP/blob/master/ZhiHuDaily.UWP.Core/Https/ServiceURL.cs)；
2. APP界面风格参考了android版本；
3. 源代码完全开放，遵循MIT协议。

##**功能**##
###**能做**###
1. 功能除了登录（回复、点赞）没有以外，其余所有与android版本一致；
2. 支持分享到朋友圈、分享给微信好友、windows 10内置分享；
3. 支持网络类型（wifi、234g）自动判断、234g不下载图片；
4. 支持离线缓存（无网络连接可以查看缓存内容）、缓存清除；
5. 收藏、已读文章以及一些配置数据均可漫游，在多个windows 10设备上共享（该功能没有充分测试）。

###**不能做**###
1. ~~不能登录账号~~；
2. ~~不能评论、点赞、回复~~。

##**截图**##

##**注意事项**##

1. 本APP只适用于Windows 10 Mobile设备（其他设备也能运行，但是界面需要重新调整）；
2. APP使用到的Web API均通过Fiddler侦测分析得到（不包含登录部分API），因此使用到的API接口非官方渠道发布；
3. `ZhiHuDaily.UWP.Core`项目包含了Web API的封装、`ViewModel`、`Model`以及一些控件、自增集合、工具类的定义。`ZhiHuDaily.UWP.Mobile`项目
中只包含View部分的代码以及一些素材。

更多说明请参考博客，[猛戳这里](http://www.cnblogs.com/xiaozhi_5638/)