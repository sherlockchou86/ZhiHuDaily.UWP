using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhiHuDaily.UWP.Core.Https;
using ZhiHuDaily.UWP.Core.Models;

namespace ZhiHuDaily.UWP.Core.ViewModels
{
    public class EditorInfoViewModel:ViewModelBase
    {
        private Editor _editor;
        private APIService _api = new APIService();

        private string _editor_info_html;
        public string EditorInfoHtml
        {
            get
            {
                return _editor_info_html;
            }
            set
            {
                _editor_info_html = value;
                OnPropertyChanged();
            }
        }

        public EditorInfoViewModel(Editor editor)
        {
            _editor = editor;

            Update();
        }
        /// <summary>
        /// 刷新数据
        /// </summary>
        public async void Update()
        {
            string html = await _api.GetEditorInfo(_editor.ID);
            if (html != null)
            {
                EditorInfoHtml = html;
            }
        }
    }
}
