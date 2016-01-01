using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroMsg.sdk;
using Windows.UI.Popups;

namespace ZhiHuDaily.UWP.Core.Share
{
    public class WeChatResponseHandler:WXEntryBasePage
    {
        /// <summary>
        /// 微信授权结果 （没实现）
        /// </summary>
        /// <param name="response"></param>
        public override void OnSendAuthResponse(SendAuth.Resp response)
        {
            base.OnSendAuthResponse(response);
            //
            // response.ErrCode == 0表示授权成功   
            // response.Code表示临时令牌  使用它去换取AccessToken
        }
        /// <summary>
        /// 向微信分享消息结果
        /// </summary>
        /// <param name="response"></param>
        public async override void OnSendMessageToWXResponse(SendMessageToWX.Resp response)
        {
            base.OnSendMessageToWXResponse(response);
            if (response.ErrCode == 0)  //分享完成
            {
                await new MessageDialog("文章分享成功!", "分享提示").ShowAsync();
            }
            else //分享未完成
            {
                await new MessageDialog("文章分享失败!", "分享提示").ShowAsync();
            }
        }
        /// <summary>
        /// 微信支付结果 （没实现）
        /// </summary>
        /// <param name="response"></param>
        public override void OnSendPayResponse(SendPay.Resp response)
        {
            base.OnSendPayResponse(response);
            // response.ErrCode == 0表示支付完成
        }
    }
}
