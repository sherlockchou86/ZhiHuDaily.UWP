using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ZhiHuDaily.UWP.Core.ViewModels;

namespace ZhiHuDaily.UWP.Core.Models
{
    /// <summary>
    /// 文章
    /// </summary>
    [DataContract]
    public class Story:ViewModelBase
    {
        [DataMember]
        public string ID
        {
            get; set;
        }
        [DataMember]
        public string Title
        {
            get; set;
        }
        [DataMember]
        public string Date  //可为null
        {
            get; set;
        }
        private bool _favorite;
        [DataMember]
        public bool Favorite  //是否收藏  不是知乎数据
        {
            get
            {
                return _favorite;
            }
            set
            {
                _favorite = value;
                OnPropertyChanged();
            }
        }
        [DataMember]
        public bool MultiPic
        {
            get; set;
        }
        [DataMember]
        public string Image
        {
            get; set;
        }
        [DataMember]
        public bool Separator  //是否是当天第一条文章
        {
            get; set;
        }
        private bool _readed;
        [DataMember]
        public bool Readed //是否已读
        {
            get
            {
                return _readed;
            }
            set
            {
                _readed = value;
                OnPropertyChanged();
            }
        }
    }
}
