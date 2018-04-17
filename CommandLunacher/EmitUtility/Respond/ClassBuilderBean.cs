using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace EmitUtility
{

    /// <summary>
    /// 类型建造Bean
    /// </summary>
    public class ClassBuilderBean
    {
        /// <summary>
        /// 类型创造器
        /// </summary>
        public TypeBuilder UseTypeBuilder { internal set; get; }

        /// <summary>
        /// 对应字段
        /// </summary>
        public Dictionary<string, FieldBuilder> UseFiledDic { internal set; get; }

        /// <summary>
        /// 对应的方法
        /// </summary>
        public List<KeyValuePair<MethodRequest, MethodBuilder>> UseLstKVPMethodBuilder { internal set; get; }

        /// <summary>
        /// 创建类型对象
        /// </summary>
        /// <returns></returns>
        public Type GetCreatType()
        {
            return UseTypeBuilder.CreateType();
        }
    }
}
