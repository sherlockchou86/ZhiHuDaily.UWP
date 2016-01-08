using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ZhiHuDaily.UWP.Core.Models
{
    [DataContract]
    public class ZhihuDailyQuotation
    {
        [DataMember]
        public string Title
        {
            get; set;
        }
        [DataMember]
        public string SubTitle
        {
            set; get;
        }
        [DataMember]
        public string LogoBackDark
        {
            get; set;
        }
        [DataMember]
        public string LogoBackLight
        {
            get; set;
        }
    }
}
