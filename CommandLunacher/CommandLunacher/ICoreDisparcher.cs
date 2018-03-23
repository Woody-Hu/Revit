using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLunacher
{
    /// <summary>
    /// 派发器接口
    /// </summary>
    internal interface ICoreDisparcher:IRevitFunction
    {
        void PrepareData();
    }
}
