using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel.Demo
{
    public class UseRespondHanlder:IResponseHanlder
    {
        DateTime startTime;

        DateTime endTime;

        List<RevitModelRebuildResponse> lstResponse = new List<RevitModelRebuildResponse>();

        public void AddStartTime(DateTime inputDataTime)
        {
            startTime = inputDataTime;
        }

        public void AddEndTime(DateTime inputDataTime)
        {
            endTime = inputDataTime;
        }

        public void AddOneResponse(RevitModelRebuildResponse inputResponse)
        {
            lstResponse.Add(inputResponse);
        }

        public void HanlderResponse()
        {
            StringBuilder useStringBuilder = new StringBuilder();

            var count = (from n in lstResponse where n.IfSucess select n).Count();

            useStringBuilder.AppendLine(string.Format("共翻成功{0}个",count));
            useStringBuilder.AppendLine(string.Format("共耗时{0}", (endTime - startTime).ToString()));

            System.Windows.Forms.MessageBox.Show(useStringBuilder.ToString());

            lstResponse = new List<RevitModelRebuildResponse>();
        }
    }
}
