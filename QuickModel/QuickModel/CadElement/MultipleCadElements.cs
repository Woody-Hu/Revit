using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel
{
    /// <summary>
    /// 分组Cad元素结果
    /// </summary>
    public abstract class MultipleCadElements
    {
        /// <summary>
        /// 对应的线型对象
        /// </summary>
        public virtual List<CurveCadElement> LstCurveElements { set; get; }

        /// <summary>
        /// 对应的文字对象
        /// </summary>
        public virtual List<TextCadElement> LstTextElements { set; get; }
    }
}
