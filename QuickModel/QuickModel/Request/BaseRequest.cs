using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel
{
    /// <summary>
    /// 请求基类
    /// </summary>
    public abstract class BaseRequest
    {
        /// <summary>
        /// Revit侧使用的类型名称
        /// </summary>
        public virtual string UseTypeName { internal set; get; }

        /// <summary>
        /// Revit侧使用的位置类型枚举
        /// </summary>
        public virtual LocationKindEnum UseRevitLocationKind { internal set; get; }

        /// <summary>
        /// Cad侧使用的位置类型枚举
        /// </summary>
        public virtual LocationKindEnum UseCadLocationKind { internal set; get; }

        /// <summary>
        /// 使用的外部命令对象
        /// </summary>
        public virtual ExternalCommandData UseExternalCommandData { internal set; get; }
    }
}
