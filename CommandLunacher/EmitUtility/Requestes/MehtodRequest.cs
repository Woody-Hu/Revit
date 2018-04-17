using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EmitUtility
{
    /// <summary>
    /// 方法制作请求
    /// </summary>
    public class MethodRequest
    {
        /// <summary>
        /// 使用的方法名称
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 使用的返回值
        /// </summary>
        public Type ReturnType { set; get; }

        /// <summary>
        /// 使用的参数列表
        /// </summary>
        public Type[] ParameterTypes { set; get; }

        /// <summary>
        /// 使用的方法实现委托
        /// </summary>
        public MethodCreatDel UseMethodDel { set; get; }

        /// <summary>
        /// 需复写的方法对象
        /// </summary>
        public MethodInfo UseBaseMethod { set; get; }
    }
}
