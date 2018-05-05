using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel.Demo
{
    [Grouper(GrouperName = "test" )]
    public class UseInputRequest:InputRequest
    {
        public UseInputRequest()
        {
            this.UseTypeName = "内部-200-砖";
        }
    }
}
