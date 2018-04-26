using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel
{
    public abstract class BaseCadElement
    {
        public virtual string UseLayerName { set; get; }

        public virtual object UseCadOrginObj { set; get; }
    }
}
