using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel
{
    /// <summary>
    /// 响应封装
    /// </summary>
    public class RevitModelRebuildResponse
    {
        /// <summary>
        /// 对应的请求对象
        /// </summary>
        public RevitModelRequest UseRequest { set; get; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IfSucess { set; get; }

        /// <summary>
        /// 重建的Element
        /// </summary>
        public Element CreatedElement { set; get; }
    }
}
