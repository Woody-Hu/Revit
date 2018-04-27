using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace QuickModel
{
    /// <summary>
    /// 文字封装
    /// </summary>
    public abstract class TextCadElement:BaseCadLayerElement
    {
        /// <summary>
        /// 使用的文字
        /// </summary>
        public virtual string UseText { set; get; }

        /// <summary>
        /// 使用的文字底边线
        /// </summary>
        /// <returns></returns>
        public abstract Line GetBaseLine();

        /// <summary>
        /// 使用的文字高度
        /// </summary>
        public virtual double UseHight { set; get; }
    }
}
