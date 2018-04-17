using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace EmitUtility
{
    /// <summary>
    /// 类型制作请求
    /// </summary>
    public class TypeMakeRequest
    {
        /// <summary>
        /// 类型的名称
        /// </summary>
        public string TypeName { set; get; }

        /// <summary>
        /// 父类型 若为null则从object继承
        /// </summary>
        public Type ParentType { set; get; }

        /// <summary>
        /// 需实现的接口类型列表
        /// </summary>
        public List<Type> LstInterfaceType { set; get; }

        /// <summary>
        /// 类特性建造输入
        /// </summary>
        public List<CustomAttributeBuilder> LstAttribute { set; get; }

        /// <summary>
        /// 使用的字段请求
        /// </summary>
        public List<FiledMakeRequest> LstFiled { set; get; }

        /// <summary>
        /// 使用的方法制作请求
        /// </summary>
        public List<MethodRequest> LstMethodRequest { set; get; }

    }
}
