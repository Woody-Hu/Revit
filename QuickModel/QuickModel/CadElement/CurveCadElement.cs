using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace QuickModel
{
    /// <summary>
    /// 线型Cad对象
    /// </summary>
    public abstract class CurveCadElement:BaseCadLayerElement
    {
        /// <summary>
        /// 对应的Curve
        /// </summary>
        public virtual Curve ThisCurve { set; get; }

        /// <summary>
        /// 是否相交
        /// </summary>
        /// <param name="inputCurve"></param>
        /// <returns></returns>
        public abstract bool IfIntersection(CurveCadElement inputCurve);

        /// <summary>
        /// 是否平行
        /// </summary>
        /// <param name="inputCurve"></param>
        /// <returns></returns>
        public abstract bool IfParallel(CurveCadElement inputCurve);
    }
}
