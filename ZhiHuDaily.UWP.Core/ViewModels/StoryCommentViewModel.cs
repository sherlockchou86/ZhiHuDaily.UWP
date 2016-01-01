using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhiHuDaily.UWP.Core.Data;
using ZhiHuDaily.UWP.Core.Https;
using ZhiHuDaily.UWP.Core.Models;

namespace ZhiHuDaily.UWP.Core.ViewModels
{
    public class StoryCommentViewModel : ViewModelBase
    {
        private APIService _api = new APIService();
        private string _story_id;
        private StoryExtra _se;

        private StoryCommentsIncrementalLoadingCollection _short_comments;
        public StoryCommentsIncrementalLoadingCollection ShortComments
        {
            get
            {
                return _short_comments;
            }
            set
            {
                _short_comments = value;
                OnPropertyChanged();
            }
        }

        private StoryCommentsIncrementalLoadingCollection _long_comments;
        public StoryCommentsIncrementalLoadingCollection LongComments
        {
            get
            {
                return _long_comments;
            }
            set
            {
                _long_comments = value;
                OnPropertyChanged();
            }
        }

        private string _total_comments;
        public string TotalComments
        {
            get
            {
                return _total_comments;
            }
            set
            {
                _total_comments = value;
                OnPropertyChanged();
            }
        }
        private string _total_short_comments;
        public string TotalShortComments
        {
            get
            {
                return _total_short_comments;
            }
            set
            {
                _total_short_comments = value;
                OnPropertyChanged();
            }
        }
        private string _total_long_comments;
        public string TotalLongComments
        {
            get
            {
                return _total_long_comments;
            }
            set
            {
                _total_long_comments = value;
                OnPropertyChanged();
            }
        }

        private bool _is_loading;
        public bool IsLoading
        {
            get
            {
                return _is_loading;
            }
            set
            {
                _is_loading = value;
                OnPropertyChanged();
            }
        }

        public StoryCommentViewModel(string story_id, StoryExtra extra)
        {
            _story_id = story_id;
            _se = extra;

            Update();
        }

        public async void Update()
        {

            IsLoading = true;

            TotalComments = _se.Comments;
            TotalLongComments = _se.LongComments;
            TotalShortComments = _se.ShortComments;

            var t1 = _api.GetShortComments(_story_id);
            var t2 = _api.GetLongComments(_story_id);

            List<StoryComment> short_comments = await t1;
            List<StoryComment> long_comments = await t2;

            if (short_comments != null && short_comments.Any())
            {
                StoryCommentsIncrementalLoadingCollection c = new StoryCommentsIncrementalLoadingCollection(short_comments.Last().ID, _story_id, true);
                short_comments.ForEach((comment) => { c.Add(comment); });

                ShortComments = c;

                c.DataLoaded += C_DataLoaded;
                c.DataLoading += C_DataLoading;
            }
            if (long_comments != null && long_comments.Any())
            {
                StoryCommentsIncrementalLoadingCollection c = new StoryCommentsIncrementalLoadingCollection(long_comments.Last().ID, _story_id, false);
                long_comments.ForEach((comment) => { c.Add(comment); });

                LongComments = c;

                c.DataLoaded += C_DataLoaded;
                c.DataLoading += C_DataLoading;
            }

            IsLoading = false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void C_DataLoading()
        {
            IsLoading = true;
        }
        /// <summary>
        /// 
        /// </summary>
        private void C_DataLoaded()
        {
            IsLoading = false;
        }
    }
}
