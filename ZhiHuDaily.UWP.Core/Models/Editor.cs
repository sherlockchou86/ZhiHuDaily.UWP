using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ZhiHuDaily.UWP.Core.Models
{
    /// <summary>
    /// 主编
    /// </summary>
    [DataContract]
    public class Editor
    {
        [DataMember]
        public string ID
        {
            get; set;
        }
        [DataMember]
        public string Name
        {
            get; set;
        }
        [DataMember]
        public string Avatar
        {
            get; set;
        }
        [DataMember]
        public string Bio
        {
            get; set;
        }
        [DataMember]
        public string URL
        {
            get; set;
        }
    }
}
