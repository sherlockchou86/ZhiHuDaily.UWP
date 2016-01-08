using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace ZhiHuDaily.UWP.Core.Tools
{
    public class FileHelper
    {
        private static FileHelper _current;
        public static FileHelper Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new FileHelper();
                }
                return _current;
            }
        }

        private Windows.Storage.StorageFolder _local_folder;

        public FileHelper()
        {
            _local_folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Init();
        }

        private async void Init()
        {
            await _local_folder.CreateFolderAsync("images_cache", CreationCollisionOption.OpenIfExists);
            await _local_folder.CreateFolderAsync("data_cache", CreationCollisionOption.OpenIfExists);
        }
        public async Task WriteObjectAsync<T>(T obj, string filename)
        {
            try
            {
                var folder = await _local_folder.CreateFolderAsync("data_cache", CreationCollisionOption.OpenIfExists);
                var data = await folder.OpenStreamForWriteAsync(filename, CreationCollisionOption.ReplaceExisting);
                DataContractJsonSerializer serizlizer = new DataContractJsonSerializer(typeof(T));
                serizlizer.WriteObject(data, obj);
            }
            catch
            {

            }
        }
        public async Task<T> ReadObjectAsync<T>(string filename) where T: class
        {
            try
            {
                var folder = await _local_folder.CreateFolderAsync("data_cache", CreationCollisionOption.OpenIfExists);
                var data = await folder.OpenStreamForReadAsync(filename);
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

                return serializer.ReadObject(data) as T;
            }
            catch
            {
                return null;
            }
        }

        public async Task SaveImageAsync(WriteableBitmap image, string filename)
        {
            try
            {
                if (image == null)
                {
                    return;
                }
                Guid BitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
                if(filename.EndsWith("jpg"))
                        BitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
                else if(filename.EndsWith("png"))
                        BitmapEncoderGuid = BitmapEncoder.PngEncoderId;
                else if(filename.EndsWith("bmp"))
                        BitmapEncoderGuid = BitmapEncoder.BmpEncoderId;
                else if(filename.EndsWith("tiff"))
                        BitmapEncoderGuid = BitmapEncoder.TiffEncoderId;
                else if(filename.EndsWith("gif"))
                        BitmapEncoderGuid = BitmapEncoder.GifEncoderId;
                var folder = await _local_folder.CreateFolderAsync("images_cache", CreationCollisionOption.OpenIfExists);
                var file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoderGuid, stream);
                    Stream pixelStream = image.PixelBuffer.AsStream();
                    byte[] pixels = new byte[pixelStream.Length];
                    await pixelStream.ReadAsync(pixels, 0, pixels.Length);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                              (uint)image.PixelWidth,
                              (uint)image.PixelHeight,
                              96.0,
                              96.0,
                              pixels);
                    await encoder.FlushAsync();
                }
            }
            catch
            {

            }
        }
        public async Task DeleteCacheFile()
        {
            try
            {
                StorageFolder folder = await _local_folder.GetFolderAsync("images_cache");
                if (folder != null)
                {
                    IReadOnlyCollection<StorageFile> files = await folder.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.DefaultQuery);
                    //files.ToList().ForEach(async (f) => await f.DeleteAsync(StorageDeleteOption.PermanentDelete));
                    List<IAsyncAction> list = new List<IAsyncAction>();
                    foreach (var f in files)
                    {
                        list.Add(f.DeleteAsync(StorageDeleteOption.PermanentDelete));
                    }
                    List<Task> list2 = new List<Task>();
                    list.ForEach((t) => list2.Add(t.AsTask()));

                    await Task.Run(() => { Task.WaitAll(list2.ToArray()); });
                }
            }
            catch
            {

            }
        }
        public async Task<double> GetCacheSize()
        {
            try
            {
                StorageFolder folder = await _local_folder.GetFolderAsync("images_cache");
                var files = await folder.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.DefaultQuery);
                double size = 0; BasicProperties p;
                foreach (var f in files)
                {
                    p = await f.GetBasicPropertiesAsync();
                    size += p.Size;
                }
                return size;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<bool> CacheExist(string filename)
        {
            try
            {
                var f = await _local_folder.TryGetItemAsync("images_cache");
                if (f != null)
                {
                    var f2 = await (f as StorageFolder).TryGetItemAsync(filename);
                    if (f2 == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
