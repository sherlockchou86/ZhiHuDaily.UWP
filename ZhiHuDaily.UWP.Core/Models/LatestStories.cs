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
    /// 首页最近文章
    /// </summary>
    [DataContract]
    class LatestStories
    {
        [DataMember]
        public string Date
        {
            get; set;
        }
        [DataMember]
        public ObservableCollection<Story> Stories
        {
            get; set;
        }
        [DataMember]
        public ObservableCollection<Story> Top_Stories
        {
            get; set;
        }
    }
}
