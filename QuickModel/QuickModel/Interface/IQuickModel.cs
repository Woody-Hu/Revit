using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel
{
    /// <summary>
    /// 翻模接口
    /// </summary>
    public interface IQuickModel
    {
        /// <summary>
        /// 翻模命令接口
        /// </summary>
        /// <param name="inputCommandData"></param>
        void Excute(ExternalCommandData inputCommandData);
    }
}
