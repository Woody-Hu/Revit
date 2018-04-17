using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmitUtility
{
    /// <summary>
    /// 属性制作请求
    /// </summary>
    public abstract class FiledMakeRequest
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string FiledName { set; get; }

        /// <summary>
        /// 使用的类型
        /// </summary>
        public abstract Type UseType { get; }

        /// <summary>
        /// 默认值
        /// </summary>
        public object DefualtValue { set; get; }
    }
}
