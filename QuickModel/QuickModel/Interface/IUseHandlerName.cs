using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel
{
    /// <summary>
    /// 处理器特征名称获取接口
    /// </summary>
    public interface IUseHandlerNameGeter
    {
        /// <summary>
        /// 获取对应处理器的特征名称
        /// </summary>
        /// <returns></returns>
        string UseHanlderName();
    }
}
