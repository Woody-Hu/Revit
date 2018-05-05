using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel.Demo
{
   [Grouper(GrouperName = "test")]
    public class UseGrouper:IDataGrouper
    {
       private double limitLength = 7.0d;


        public List<RevitModelRequest> GroupData(InputRequest inputRequest)
        {
            List<RevitModelRequest> returnValue = new List<RevitModelRequest>();
            List<LineElement> useLineElements =
                inputRequest.SelectDataMap["wallLine"].Cast<LineElement>().ToList();

            for (int i = 0; i < useLineElements.Count; i++)
            {
                if (useLineElements[i].ThisCurve.Length < limitLength)
                {
                    continue;
                }

                for (int j = i + 1; j < useLineElements.Count; j++)
                {
                    if (useLineElements[j].ThisCurve.Length < limitLength)
                    {
                        continue;
                    }

                    if (useLineElements[i].IfParallel(useLineElements[j]))
                    {
                        UseRevitModelRequest tempRequest = new UseRevitModelRequest();

                        UseGroupCADElement tempGropuCADELement = new UseGroupCADElement();
                        tempGropuCADELement.LstCurveElements = new List<CurveCadElement>() { useLineElements[j], useLineElements[i] };

                        tempRequest.RequestData = tempGropuCADELement;
                        returnValue.Add(tempRequest);

                    }
                }
            }

            return returnValue;
        }
    }
}
