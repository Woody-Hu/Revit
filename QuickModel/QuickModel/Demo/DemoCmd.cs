using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel.Demo
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class DemoCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            var tempuseRequestMaker = new UseRequestMaker(commandData.Application.ActiveUIDocument);

            var useFrameWork = new QuickModelFrameWork(tempuseRequestMaker, new UseRespondHanlder());

            useFrameWork.Excute(commandData);
            return Result.Succeeded;
        }
    }
}
