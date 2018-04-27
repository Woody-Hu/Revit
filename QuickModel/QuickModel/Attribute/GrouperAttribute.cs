using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel
{
    /// <summary>
    /// 分组器特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = false,Inherited = false)]
    public class GrouperAttribute : Attribute, IUseHandlerNameGeter
    {
        /// <summary>
        /// 分组器标示
        /// </summary>
        public string GrouperName { set; get; }

        /// <summary>
        /// 获得处理器标示
        /// </summary>
        /// <returns></returns>
        public string UseHanlderName()
        {
            return GrouperName;
        }
    }
}
