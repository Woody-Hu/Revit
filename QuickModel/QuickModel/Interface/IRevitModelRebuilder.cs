using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace QuickModel
{
    /// <summary>
    /// revit模型重建器
    /// </summary>
    public interface IRevitModelRebuilder
    {
        /// <summary>
        /// 利用输入请求建立revit对象
        /// </summary>
        /// <param name="inputDoc"></param>
        /// <param name="inputReqeust"></param>
        /// <param name="createdElement"></param>
        /// <returns></returns>
        bool TryRebuildRevitModel(Document inputDoc, RevitModelRequest inputReqeust, out Element createdElement);
    }
}
