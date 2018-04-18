using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace EmitUtility
{
    /// <summary>
    /// 程序集创建相应结果
    /// </summary>
    public class AssemblyRespondBean
    {
        /// <summary>
        /// 使用的程序集的名称
        /// </summary>
        internal string UseAssemblyName { set; get; }

        /// <summary>
        /// 输出程序集的路径
        /// </summary>
        public string UseAssemblyDir { internal set; get; }

        /// <summary>
        /// 程序集创建器
        /// </summary>
        public AssemblyBuilder UseAssembuilder { get; internal set; }

        /// <summary>
        /// 使用的模块创建器
        /// </summary>
        public ModuleBuilder UseModuleBuilder { get; internal set; }

        /// <summary>
        /// 类型创建响应
        /// </summary>
        public List<ClassBuilderBean> LstClassBean { get; internal set; }
    }
}
