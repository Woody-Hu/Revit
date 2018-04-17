using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLunacher
{
    /// <summary>
    /// Revit方法接口组
    /// </summary>
    public interface IRevitFunction:IExternalCommand,IExternalApplication
    {
        /// <summary>
        /// 通过扩展机制执行
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <param name="useGuid">使用的Guid</param>
        /// <param name="useAssemblyLocation">使用的程序集路径</param>
        /// <param name="useClassFullName">使用的类全名称</param>
        /// <returns></returns>
        Result ExecuteByAppendValue(ExternalCommandData commandData, ref string message, ElementSet elements
            , string useGuid, string useAssemblyLocation, string useClassFullName);
    }
}
