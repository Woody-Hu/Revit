using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace QuickModel
{
    public abstract class CurveCadElement:BaseCadElement
    {
        public virtual Curve ThisCurve { set; get; }

        public abstract bool IfIntersection(CurveCadElement inputCurve);

        public abstract bool IfParallel(CurveCadElement inputCurve);
    }
}
