using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroMsg.sdk;

namespace ZhiHuDaily.UWP.Core.Share
{
    public class WeChatRequest
    {
        string APP_ID = "123456789";

        public WeChatRequest(string app_id)
        {
            APP_ID = app_id;
        }
        /// <summary>
        /// 网页分享到微信好友
        /// </summary>
        /// <param name="url">网页url</param>
        /// <param name="title">分享标题</param>
        /// <param name="thumb">站位图标</param>
        public async void WebPageShare2SessionRequest(string url, string title, byte[] thumb)
        {
            var message = new WXWebpageMessage
            {
                WebpageUrl = url,
                ThumbData = thumb,
                Title = title
            };
            SendMessageToWX.Req request = new SendMessageToWX.Req(message, SendMessageToWX.Req.WXSceneSession);
            IWXAPI api = WXAPIFactory.CreateWXAPI(APP_ID);

            await api.SendReq(request);
        }
        /// <summary>
        /// 网页分享到微信朋友圈
        /// </summary>
        /// <param name="url"></param>
        /// <param name="title"></param>
        /// <param name="thumb"></param>
        public async void WebPageShare2TimelineRequest(string url, string title, byte[] thumb)
        {
            var message = new WXWebpageMessage
            {
                WebpageUrl = url,
                ThumbData = thumb,
                Title = title
            };
            SendMessageToWX.Req request = new SendMessageToWX.Req(message, SendMessageToWX.Req.WXSceneTimeline);
            IWXAPI api = WXAPIFactory.CreateWXAPI(APP_ID);

            await api.SendReq(request);
        }
        /// <summary>
        /// 文本分享到微信好友
        /// </summary>
        /// <param name="text"></param>
        /// <param name="title"></param>
        /// <param name="thumb"></param>
        public async void TextShare2SessionRequest(string text, string title, byte[] thumb)
        {
            var message = new WXTextMessage
            {
                Text = text,
                ThumbData = thumb,
                Title = title
            };
            SendMessageToWX.Req request = new SendMessageToWX.Req(message, SendMessageToWX.Req.WXSceneSession);
            IWXAPI api = WXAPIFactory.CreateWXAPI(APP_ID);

            await api.SendReq(request);
        }
        /// <summary>
        /// 文本分享到微信朋友圈
        /// </summary>
        /// <param name="text"></param>
        /// <param name="title"></param>
        /// <param name="thumb"></param>
        public async void TextShare2TimelineRequest(string text, string title, byte[] thumb)
        {
            var message = new WXTextMessage
            {
                Text = text,
                ThumbData = thumb,
                Title = title
            };
            SendMessageToWX.Req request = new SendMessageToWX.Req(message, SendMessageToWX.Req.WXSceneTimeline);
            IWXAPI api = WXAPIFactory.CreateWXAPI(APP_ID);

            await api.SendReq(request);
        }
    }
}
