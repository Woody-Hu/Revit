using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel
{
    /// <summary>
    /// 位置类型枚举
    /// </summary>
    public enum LocationKindEnum
    {
        /// <summary>
        /// 几何中心点
        /// </summary>
        MidPoint = 0,

        /// <summary>
        /// 实体插入点
        /// </summary>
        InsertPoint,

        /// <summary>
        /// 非位置点映射
        /// </summary>
        None
    }
}
