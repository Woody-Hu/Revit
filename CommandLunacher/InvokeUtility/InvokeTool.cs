using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InvokeUtility
{
    /// <summary>
    /// 唤醒工具
    /// </summary>
    public class InvokeTool
    {
        /// <summary>
        /// 调度框架核心程序集
        /// </summary>
        private const string m_strUseDispatcherFULLNAME = "CommandLunacher.WrapperDispatcher";

        /// <summary>
        /// 使用的调度方法
        /// </summary>
        private const string m_strUseMethodName = "ExecuteByAppendValue";

        /// <summary>
        /// 提示信息
        /// </summary>
        private const string m_strMessage = "代理错误";

        /// <summary>
        /// 反射唤醒工具
        /// </summary>
        /// <param name="lstInputObjects"></param>
        /// <returns></returns>
        public static Result HelpInvoke(List<object> lstInputObjects)
        {
            try
            {
                //获取程序集路由
                Assembly useAssembly = Assembly.LoadFile((string)lstInputObjects[lstInputObjects.Count - 1]);

                //获取核心调度框架
                Type useType = useAssembly.GetType(m_strUseDispatcherFULLNAME);

                object tempInstance = Activator.CreateInstance(useType);

                //获取方法接口
                MethodInfo useMethodInfo = useType.GetMethod(m_strUseMethodName);
                //移除框架程序集路由
                lstInputObjects.RemoveAt(lstInputObjects.Count - 1);
                //反射执行
                Result returnValue = (Result)useMethodInfo.Invoke(tempInstance, lstInputObjects.ToArray());

                return returnValue;
            }
            catch (Exception ex)
            {
                TaskDialog.Show(m_strMessage, ex.Message);
                return Result.Failed;
            }
           
        }
    }
}
