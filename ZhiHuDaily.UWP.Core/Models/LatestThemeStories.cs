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
    /// 主题文章
    /// </summary>
    [DataContract]
    class LatestThemeStories
    {
        [DataMember]
        public string Background
        {
            get; set;
        }
        [DataMember]
        public string Description
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
        public string Name
        {
            get; set;
        }
        [DataMember]
        public ObservableCollection<Story> Stories
        {
            get; set;
        }
        [DataMember]
        public ObservableCollection<Editor> Editors
        {
            get; set;
        }
    }
}
