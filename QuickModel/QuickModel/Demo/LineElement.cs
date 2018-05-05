using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel.Demo
{
    public class LineElement : CurveCadElement
    {
        public override bool IfIntersection(CurveCadElement inputCurve)
        {
            return false;
        }

        public override bool IfParallel(CurveCadElement inputCurve)
        {
            Line line1 = this.ThisCurve as Line;

            Line line2 = inputCurve.ThisCurve as Line;

            if ( Math.Abs( line1.Length - line2.Length) > 1e-6 )
            {
                return false;
            }

            if (!(line1.Direction.IsAlmostEqualTo(line2.Direction) || line1.Direction.IsAlmostEqualTo(-line2.Direction)))
            {
                return false;
            }

            if (line1.Project(line2.GetEndPoint(1)).XYZPoint.IsAlmostEqualTo(line1.GetEndPoint(1)) || line1.Project(line2.GetEndPoint(1)).XYZPoint.IsAlmostEqualTo(line1.GetEndPoint(0)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
