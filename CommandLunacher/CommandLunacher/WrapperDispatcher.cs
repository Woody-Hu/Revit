using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace CommandLunacher
{
    /// <summary>
    /// 对外调度类
    /// </summary>
    [Autodesk.Revit.Attributes.TransactionAttribute(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class WrapperDispatcher : IRevitFunction
    {
        /// <summary>
        /// 命令核心分发器
        /// </summary>
        private ICoreDisparcher m_useCoreDispatcher;

        /// <summary>
        /// 构造方法
        /// </summary>
        public WrapperDispatcher()
        {
            //AOP中获得核心分发器
            m_useCoreDispatcher = DispatcherAOP.GetRevitFunction();
        }


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            return m_useCoreDispatcher.Execute(commandData, ref message, elements);
        }

        public Result ExecuteByAppendValue(ExternalCommandData commandData, ref string message, ElementSet elements
            , string useGuid, string useAssemblyLocation, string useClassFullName)
        {
            return m_useCoreDispatcher.ExecuteByAppendValue(commandData, ref message, elements, useGuid, useAssemblyLocation, useClassFullName);
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            var applicationCount = m_useCoreDispatcher.ApplicationCount();

            //循环关闭
            for (int tempIndex = applicationCount - 1; tempIndex >= 0 ; tempIndex--)
            {
                m_useCoreDispatcher.ShutDownOneApplication(tempIndex, application);
            }

            return m_useCoreDispatcher.OnShutdown(application);
        }

        public Result OnStartup(UIControlledApplication application)
        {
            var returnValue = m_useCoreDispatcher.OnStartup(application);

            var applicationCount = m_useCoreDispatcher.ApplicationCount();

            //循环启动
            for (int tempIndex = 0; tempIndex < applicationCount; tempIndex++)
            {
                m_useCoreDispatcher.StartUpOneApplication(tempIndex, application);
            }

            return returnValue;
        }
    }
}
