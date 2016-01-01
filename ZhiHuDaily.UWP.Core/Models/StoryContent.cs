using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ZhiHuDaily.UWP.Core.Models
{
    /// <summary>
    /// 文章内容
    /// </summary>
    [DataContract]
    class StoryContent
    {
        [DataMember]
        public string Body
        {
            get; set;
        }
        [DataMember]
        public string CSS
        {
            get; set;
        } 
        [DataMember]
        public string ID
        {
            get; set;
        }
        [DataMember]
        public string Image
        {
            get; set;
        }
        [DataMember]
        public string Image_Source
        {
            get; set;
        }
        [DataMember]
        public string Share_URL
        {
            get; set;
        }
        [DataMember]
        public string Title
        {
            get; set;
        }
        [DataMember]
        public bool Readed
        {
            get; set;
        }
        [DataMember]
        public ObservableCollection<string> RecommnderAvatars
        {
            get; set;
        }
    }
}
