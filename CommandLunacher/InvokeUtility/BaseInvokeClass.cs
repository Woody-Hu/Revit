using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace InvokeUtility
{
    /// <summary>
    /// 唤醒抽象基类
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public abstract class BaseInvokeClass : IExternalCommand
    {
        public abstract Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements);
    }
}
