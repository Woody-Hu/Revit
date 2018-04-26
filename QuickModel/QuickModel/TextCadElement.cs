using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace QuickModel
{
    public abstract class TextCadElement:BaseCadElement
    {
        public virtual string UseText { set; get; }

        public abstract Line GetBaseLine();
    }
}
