using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel
{
    /// <summary>
    /// 数据分组接口
    /// </summary>
    public interface IDataGrouper
    {
        /// <summary>
        /// 将输入的初始数据进行分组
        /// </summary>
        /// <param name="inputRequest"></param>
        /// <returns></returns>
        List<RevitModelRequest> GroupData(InputRequest inputRequest);
    }
}
