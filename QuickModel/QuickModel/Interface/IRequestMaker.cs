using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel
{
    /// <summary>
    /// 输入请求获得接口(界面层)
    /// </summary>
    public interface IRequestMaker
    {
        /// <summary>
        /// 获取请求
        /// </summary>
        /// <returns></returns>
        List<InputRequest> GetAllInputRequest();

        /// <summary>
        /// 是否Break循环
        /// </summary>
        bool IfBreak
        {
            get;
        }

        /// <summary>
        /// 准备请求
        /// </summary>
        void PrepareRequest();
    }
}
