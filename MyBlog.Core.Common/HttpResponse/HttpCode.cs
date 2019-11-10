using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Czar.Cms.Unity
{
 public class HttpCode
    {
        private bool _succeed = true;
        private int _errCode=0;
        private string _errMsg;
        // sub errorCode and sub ErrorMassage that use by big project
        private int _subErrCode;
        private string _subErrMsg;
        public int code = 0;
        private dynamic _data;
        public HttpCode()
        {

        }
        /// <summary>
		/// 是否成功
        /// </summary>
		public bool Succeed
        {
            set { _succeed = value; }
            get { return _succeed; }
        }
        /// <summary>
        /// 获取和设置错误码
        /// </summary>
        public int ErrCode
        {
            set { _errCode = value;
                String describe = GetEnumDesription((ErrorCode)_errCode);
                this.ErrMsg = describe;
                code = -1;
            }
            get { return _errCode; }
        }

        /// <summary>
        /// 获取和设置错误信息
        /// </summary>
        public string ErrMsg
        {
            set
            {
                _errMsg = value;
                code = -1;
                _succeed = string.IsNullOrEmpty(value) ? true : false;
            }
            get { return _errMsg; }
        }
        /// <summary>
        /// 获取和设置子错误码
        /// </summary>
        public int SubErrCode
        {
            set { _subErrCode = value; }
            get { return _subErrCode; }
        }
        /// <summary>
        /// 获取和设置子错误信息
        /// </summary>
        public string SubErrMsg
        {
            set { _subErrMsg = value; }
            get { return _subErrMsg; }
        }
        /// <summary>
        /// 获取和设置返回数据
        /// </summary>
        public object Data
        {
            set {
               
                PropertyInfo _findedPropertyInfo = value.GetType().GetProperty("ErrorCode");
                string describe = string.Empty;
                if (_findedPropertyInfo != null)
                {
                    var ErrorCode = (Enum)_findedPropertyInfo.GetValue(value);
                     describe = GetEnumDesription(ErrorCode);
                    this.ErrMsg = describe;
                    this.ErrCode = Convert.ToInt32(ErrorCode);
                }
                //_data = convertToDynamic(value,describe);
            }
            get { return _data; }
        }

        public static implicit operator bool(HttpCode result)
        {
            if (string.IsNullOrEmpty(result.ErrMsg) || string.IsNullOrEmpty(result.SubErrMsg))
                return true;
            else
                return false;
        }

        public dynamic convertToDynamic(object c,string describe)
        {
            var type = c.GetType();
            dynamic obj = new System.Dynamic.ExpandoObject();
            //Dictionary<string,object> dic= new Dictionary<string,object>();
            foreach (System.Reflection.PropertyInfo info in type.GetProperties())
            {
                (obj as ICollection<KeyValuePair<string, object>>).Add(new KeyValuePair<string, object>(info.Name, info.GetValue(c)));
                //dic.Add(info.Name, info.GetValue(c));
            }
            (obj as ICollection<KeyValuePair<string, object>>).Add(new KeyValuePair<string, object>("ErrorCodeMessage", describe));
          
            return obj;
        }

        public static string GetEnumDesription(Enum enumValue)
        {

            string value = enumValue.ToString();

            FieldInfo field = enumValue.GetType().GetField(value);
            object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (objs.Length == 0)
            {
                return value;
            }

            DescriptionAttribute descriptionAttribute = (DescriptionAttribute)objs[0];

            return descriptionAttribute.Description;
        }
    }
}
