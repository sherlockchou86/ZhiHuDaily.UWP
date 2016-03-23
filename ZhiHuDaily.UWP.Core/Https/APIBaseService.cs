using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace ZhiHuDaily.UWP.Core.Https
{
    /// <summary>
    /// api服务基类
    /// </summary>
    public class APIBaseService
    {
        private void Printlog(string info)
        {
#if DEBUG
            Debug.WriteLine(DateTime.Now.ToString() + " " + info);
#endif
        }
        protected async Task<JsonObject> GetJson(string url)
        {
            try
            {
                string json = await BaseService.SendGetRequest(url);
                if (json != null)
                {
                    Printlog("请求Json数据成功 URL：" + url);
                    return JsonObject.Parse(json);
                }
                else
                {
                    Printlog("请求Json数据失败 URL：" + url);
                    return null;
                }
            }
            catch
            {
                Printlog("请求Json数据失败 URL：" + url);
                return null;
            }
        }
        protected async Task<string> GetHtml(string url)
        {
            try
            {
                string html = await BaseService.SendGetRequest(url);
                //byte[] bytes = Encoding.UTF8.GetBytes(html);
                //html = Encoding.GetEncoding("GBK").GetString(bytes);
                Printlog("请求Html数据成功 URL：" + url);
                return html;                     
            }
            catch
            {
                Printlog("请求Html数据失败 URL：" + url);
                return null;
            }
        }
        protected async Task<WriteableBitmap> GetImage(string url)
        {
            try
            {
                IBuffer buffer = await BaseService.SendGetRequestAsBytes(url);
                if (buffer != null)
                {
                    BitmapImage bi = new BitmapImage();
                    WriteableBitmap wb = null; Stream stream2Write;
                    using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                    {

                        stream2Write = stream.AsStreamForWrite();

                        await stream2Write.WriteAsync(buffer.ToArray(), 0, (int)buffer.Length);

                        await stream2Write.FlushAsync();
                        stream.Seek(0);

                        await bi.SetSourceAsync(stream);

                        wb = new WriteableBitmap(bi.PixelWidth, bi.PixelHeight);
                        stream.Seek(0);
                        await wb.SetSourceAsync(stream);

                        return wb;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 在windows runtime component项目中使用  下载图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected async Task GetImageInRuntimeComponent(string url,string name)
        {
            try
            {
                var folder = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync("images_cache", CreationCollisionOption.OpenIfExists);

                var file = await StorageFile.CreateStreamedFileFromUriAsync(name, new Uri(url), RandomAccessStreamReference.CreateFromUri(new Uri(url))); 
                file = await file.CopyAsync(folder);
            }
            catch
            {

            }
        }
    }
}
