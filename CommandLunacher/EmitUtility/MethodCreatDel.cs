using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace EmitUtility
{
    /// <summary>
    /// 方法实现委托
    /// </summary>
    /// <param name="inputMethodBuilder"></param>
    /// <param name="useClassBuilderBean"></param>
    public delegate void MethodCreatDel(MethodBuilder inputMethodBuilder, ClassBuilderBean useClassBuilderBean);
}
