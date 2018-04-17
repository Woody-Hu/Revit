using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmitUtility
{
    /// <summary>
    /// 字符串类型
    /// </summary>
    public class StringFiledMakeRequest : FiledMakeRequest
    {
        /// <summary>
        /// 使用的类型
        /// </summary>
        public override Type UseType
        {
            get
            {
                return typeof(string);
            }
        }
    }
}
