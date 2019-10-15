using System;
using System.Collections.Generic;
using System.Text;

namespace MyBlog.Core.Common
{
    public class MyBlogCommonResponse<T>
    {
        private bool _success = false;
        public string message { get; set; }
        public int code { get; set; }
        public bool success {
            get { return _success; }
            set { _success = value; }
        }
        public T data { get; set; }
    }
}
