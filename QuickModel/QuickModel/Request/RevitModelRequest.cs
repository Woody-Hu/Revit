using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace QuickModel
{
    /// <summary>
    /// Revit重建请求数据
    /// </summary>
    public abstract class RevitModelRequest:BaseRequest
    {
        /// <summary>
        /// 请求使用的分组的Cad数据封装
        /// </summary>
        public virtual  MultipleCadElements RequestData { set; get; }

    }
}
