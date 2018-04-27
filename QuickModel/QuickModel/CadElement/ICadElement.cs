using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel
{
    /// <summary>
    /// Cad元素接口
    /// </summary>
    public interface ICadElement
    {
        /// <summary>
        /// Cad元素在Cad中的Guid
        /// </summary>
        string GuidInCad { set; get; }
    }
}
