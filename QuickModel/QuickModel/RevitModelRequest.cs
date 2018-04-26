using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace QuickModel
{
    public abstract class RevitModelRequest
    {
        public virtual string UseTypeName { set; get; }

        public LocationKindEnum UseLocationKind { set; get; }
    }
}
