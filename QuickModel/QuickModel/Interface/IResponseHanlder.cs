using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel
{
    /// <summary>
    /// 响应处理器
    /// </summary>
    public interface IResponseHanlder
    {
        /// <summary>
        /// 添加一个响应
        /// </summary>
        /// <param name="inputResponse"></param>
        void AddOneResponse(RevitModelRebuildResponse inputResponse);

        /// <summary>
        /// 处理响应数据
        /// </summary>
        void HanlderResponse();
    }
}
