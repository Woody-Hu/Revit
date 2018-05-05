using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel.Demo
{
    public class UseRequestMaker:IRequestMaker
    {
        List<InputRequest> m_lstRequestes = new List<InputRequest>();

        UIDocument useuiDoc;

        public UseRequestMaker(UIDocument inputUIDoc)
        {
            useuiDoc = inputUIDoc;
        }

        public List<InputRequest> GetAllInputRequest()
        {
            return m_lstRequestes;
        }

        public bool IfBreak
        {
            get { return true; }
        }

        public void PrepareRequest()
        {
            var lstElements = useuiDoc.Selection.PickElementsByRectangle("框选模型线直线");

            List<Line> lstUseLine = new List<Line>();

            foreach (var oneElement in lstElements)
            {
                if (oneElement is ModelLine)
                {
                    Line tempLine;
                    tempLine = (oneElement as ModelLine).GeometryCurve as Line;
                    lstUseLine.Add(tempLine);
                }
            }

            List<ICadElement> tempElement = new List<ICadElement>();

            foreach (var oneLine in lstUseLine)
            {
                tempElement.Add(new LineElement() { ThisCurve = oneLine });
            }

            m_lstRequestes = new List<InputRequest>();

            InputRequest tempRequest = new UseInputRequest();

            tempRequest.SelectDataMap.Add("wallLine", tempElement);
            m_lstRequestes.Add(tempRequest);

        }
    }
}
