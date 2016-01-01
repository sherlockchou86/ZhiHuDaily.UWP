using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ZhiHuDaily.UWP.Core.Models
{
    /// <summary>
    /// 文章评论
    /// </summary>
    [DataContract]
    public class StoryComment
    {
        [DataMember]
        public string Author
        {
            get; set;
        }
        [DataMember]
        public string Avatar
        {
            get; set;
        }
        [DataMember]
        public string Content
        {
            get; set;
        }
        [DataMember]
        public string ID
        {
            get; set;
        }
        [DataMember]
        public string Likes
        {
            get; set;
        }
        [DataMember]
        public string Time
        {
            get; set;
        }
        [DataMember]
        public ReplyTo Target
        {
            get; set;
        }
        public string ShowContent
        {
            get
            {
                string c = "";
                if (Content != null)
                {
                    c = Content;
                }
                if (Target != null)
                {
                    c += " //@" + Target.Author + "：" + Target.Content;
                }
                return c;
            }
        }
    }
    [DataContract]
    public class ReplyTo
    {
        [DataMember]
        public string ID
        {
            get; set;
        }
        [DataMember]
        public string Author
        {
            get; set;
        }
        [DataMember]
        public string Content
        {
            get; set;
        }
    }
}
