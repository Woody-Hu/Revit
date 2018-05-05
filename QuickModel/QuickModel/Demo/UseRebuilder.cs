using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel.Demo
{
    [Rebuilder(RebuilderName = "test")]
    public class UseRebuilder:IRevitModelRebuilder
    {
        public bool TryRebuildRevitModel(Autodesk.Revit.DB.Document inputDoc, RevitModelRequest inputReqeust, out Autodesk.Revit.DB.Element createdElement)
        {
            createdElement = null;
            try
            {
                UseRevitModelRequest useRequest = inputReqeust as UseRevitModelRequest;

                LineElement line1 = inputReqeust.RequestData.LstCurveElements[0] as LineElement;

                LineElement line2 = inputReqeust.RequestData.LstCurveElements[1] as LineElement;

                XYZ midStart, midEnd;

                double width;

                if (line1.ThisCurve.Project(line2.ThisCurve.GetEndPoint(0)).XYZPoint.IsAlmostEqualTo( line1.ThisCurve.GetEndPoint(0)))
                {
                    midStart = (line1.ThisCurve.GetEndPoint(0) + line2.ThisCurve.GetEndPoint(0)) / 2;
                    midEnd = (line1.ThisCurve.GetEndPoint(1) + line2.ThisCurve.GetEndPoint(1)) / 2;
                    width = line1.ThisCurve.GetEndPoint(0).DistanceTo(line2.ThisCurve.GetEndPoint(0));
                }
                else
                {
                    midStart = (line1.ThisCurve.GetEndPoint(0) + line2.ThisCurve.GetEndPoint(1)) / 2;
                    midEnd = (line1.ThisCurve.GetEndPoint(1) + line2.ThisCurve.GetEndPoint(0)) / 2;
                    width = line1.ThisCurve.GetEndPoint(0).DistanceTo(line2.ThisCurve.GetEndPoint(1));
                }

                Line useMidLine = Line.CreateBound(midStart, midEnd);

                string useTypeName = inputReqeust.UseTypeName;

                FilteredElementCollector useCollector = new FilteredElementCollector(inputDoc).OfCategory( BuiltInCategory.OST_Walls).WhereElementIsElementType();

                ElementType useElementType = null;

                foreach (var oneElement in useCollector.ToElements())
                {
                    if (oneElement.Name == useTypeName)
                    {
                        useElementType = oneElement as ElementType;
                        break;
                    }
                }



                Transaction useTransaction = new Transaction(inputDoc,"creatWall");
                useTransaction.Start();
                useElementType = useElementType.Duplicate(useTypeName + Guid.NewGuid().ToString());

                var useStructure = (useElementType as WallType).GetCompoundStructure();
                useStructure.SetLayerWidth(0, width);
                (useElementType as WallType).SetCompoundStructure(useStructure);

                createdElement =  Wall.Create(inputDoc, useMidLine ,(inputDoc.ActiveView as ViewPlan).GenLevel.Id, false);
                (createdElement as Wall).WallType = useElementType as WallType;
                useTransaction.Commit();


                return true;
            }
            catch (Exception)
            {
                return false;
            }
           


        }
    }
}
