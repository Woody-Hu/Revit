using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel
{
    /// <summary>
    /// UI层请求
    /// </summary>
    public abstract class InputRequest:BaseRequest
    {
        /// <summary>
        /// 特征名称 - cad对象容器字典
        /// </summary>
        public virtual Dictionary<string, List<ICadElement>> SelectDataMap { private set; get; }

        public InputRequest()
        {
            SelectDataMap = new Dictionary<string, List<ICadElement>>();
        }
    }
}
