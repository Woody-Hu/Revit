using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel
{
    /// <summary>
    /// Cad块基类
    /// </summary>
    public abstract class BlockCadElement : MultipleCadElements, IElementInBlock, ICadElement
    {
        /// <summary>
        /// 块名称
        /// </summary>
        public virtual string BlockName { set; get; }

        /// <summary>
        /// 使用的块插入点
        /// </summary>
        public virtual XYZ UseLocationPoint { set; get; }

        /// <summary>
        /// 使用的缩放比例
        /// </summary>
        public virtual XYZ UseScale { set; get; }

        /// <summary>
        /// 使用的角度
        /// </summary>
        public virtual double Angle { set; get; }

        /// <summary>
        /// 使用的父级块元素
        /// </summary>
        public virtual BlockCadElement ParentBlock
        {
            get;
            set;
        }

        /// <summary>
        /// 内部块列表
        /// </summary>
        public virtual List<BlockCadElement> LstInnerBlock { set; get; }

        /// <summary>
        /// Cad中的Guid
        /// </summary>
        public virtual string GuidInCad
        {
            get;
            set;
        }
    }
}
