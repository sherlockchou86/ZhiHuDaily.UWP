using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ZhiHuDaily.UWP.Core.Models
{
    [DataContract]
    public class Recommender
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
        public string Bio
        {
            get; set;
        }
        [DataMember]
        public string Avatar
        {
            get; set;
        }
        [DataMember]
        public string Zhihu_URL_Token
        {
            get; set;
        }
    }
}
