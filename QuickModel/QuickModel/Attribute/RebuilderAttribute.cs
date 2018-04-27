using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel
{
    /// <summary>
    /// 重建器特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RebuilderAttribute : Attribute, IUseHandlerNameGeter
    {
        /// <summary>
        /// 重建器特征标示
        /// </summary>
        public string RebuilderName { set; get; }

        /// <summary>
        /// 使用的处理器标示
        /// </summary>
        /// <returns></returns>
        public string UseHanlderName()
        {
            return RebuilderName;
        }
    }
}
