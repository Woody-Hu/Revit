using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel
{
    /// <summary>
    /// 块中元素接口
    /// </summary>
    interface IElementInBlock
    {
        /// <summary>
        /// 父级块封装（可null）
        /// </summary>
        BlockCadElement ParentBlock { set; get; }
    }
}
