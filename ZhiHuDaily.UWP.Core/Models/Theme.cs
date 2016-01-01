using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ZhiHuDaily.UWP.Core.Models
{
    /// <summary>
    /// 主题
    /// </summary>
    [DataContract]
    public class Theme
    {
        [DataMember]
        public string ID
        {
            get; set;
        }
        [DataMember]
        public string Description
        {
            get; set;
        }
        [DataMember]
        public string Name
        {
            get; set;
        }
        [DataMember]
        public string Thumbnail
        {
            get; set;
        }
    }
}
