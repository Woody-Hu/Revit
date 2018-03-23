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
    /// 装饰调度类
    /// </summary>
    [Autodesk.Revit.Attributes.TransactionAttribute(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class WrapperDispatcher : IRevitFunction
    {
        /// <summary>
        /// 命令核心分发器
        /// </summary>
        private IRevitFunction m_useCoreDispatcher;

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

        public Result OnShutdown(UIControlledApplication application)
        {
            return m_useCoreDispatcher.OnShutdown(application);
        }

        public Result OnStartup(UIControlledApplication application)
        {
            return m_useCoreDispatcher.OnStartup(application);
        }
    }
}
