using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace ZhiHuDaily.UWP.Core.Tools
{
    public class DispatcherManager
    {
        private CoreDispatcher _dispatcher;
        public CoreDispatcher Dispatcher
        {
            get
            {
                return _dispatcher;
            }
            set
            {
                _dispatcher = value;
            }
        }

        private static DispatcherManager _current;
        public static DispatcherManager Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new DispatcherManager();
                }
                return _current;
            }
        }
    }
}
