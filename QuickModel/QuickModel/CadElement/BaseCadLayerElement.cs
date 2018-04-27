using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel
{
    /// <summary>
    /// 单一图层中Cad元素基类
    /// </summary>
    public abstract class BaseCadLayerElement : IElementInBlock, ICadElement
    {
        /// <summary>
        /// 对应的图层名称
        /// </summary>
        public virtual string UseLayerName { set; get; }

        /// <summary>
        /// 对应的Cad控件读取出的对象
        /// </summary>
        public virtual object UseCadOrginObj { set; get; }

        /// <summary>
        /// 所处的块封装
        /// </summary>
        public virtual BlockCadElement ParentBlock
        {
            get;

            set;
        }

        /// <summary>
        /// 对应在Cad中的Guid
        /// </summary>
        public virtual string GuidInCad
        {
            get;
            set;
        }
    }
}
