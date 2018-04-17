using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmitUtility
{
    /// <summary>
    /// 程序集制作请求
    /// </summary>
    public class AssemblyMakeRequest
    {
        /// <summary>
        /// 使用的程序集名称
        /// </summary>
        public string AssemblyName { set; get; }

        /// <summary>
        /// 使用的类型创建请求列表
        /// </summary>
        public List<TypeMakeRequest> LstUseTypeMakeRequest { set; get; }
    }
}
