using Autodesk.Revit.UI;
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
    }
}
