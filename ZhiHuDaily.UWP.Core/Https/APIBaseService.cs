using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace ZhiHuDaily.UWP.Core.Https
{
    /// <summary>
    /// api服务基类
    /// </summary>
    class APIBaseService
    {
        protected async Task<JsonObject> GetJson(string url)
        {
            try
            {
                string json = await BaseService.SendGetRequest(url);
                if (json != null)
                {
                    return JsonObject.Parse(json);
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
        protected async Task<string> GetHtml(string url)
        {
            try
            {
                string html = await BaseService.SendGetRequest(url);
                //byte[] bytes = Encoding.UTF8.GetBytes(html);
                //html = Encoding.GetEncoding("GBK").GetString(bytes);
                return html;                     
            }
            catch
            {
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
    }
}
