using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhiHuDaily.UWP.Core.Models;

namespace ZhiHuDaily.UWP.Core.ViewModels
{
    public class EditorsViewModel:ViewModelBase
    {
        private ObservableCollection<Editor> _editors;
        public ObservableCollection<Editor> Editors
        {
            get
            {
                return _editors;
            }
            set
            {
                _editors = value;
                OnPropertyChanged();
            }
        }

        public EditorsViewModel(ObservableCollection<Editor> editors)
        {
            Editors = editors;
        }
    }
}
