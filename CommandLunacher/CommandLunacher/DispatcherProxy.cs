using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;

namespace CommandLunacher
{

    /// <summary>
    /// AOP核心处理 单例模式
    /// </summary>
    internal class DispatcherAOP : RealProxy
    {
        /// <summary>
        /// 核心对象
        /// </summary>
        private ICoreDisparcher m_useCore = null;

        /// <summary>
        /// AOP结果
        /// </summary>
        private ICoreDisparcher m_AOPResult = null;

        /// <summary>
        /// 单例标签
        /// </summary>
        private static DispatcherAOP m_signalTag = null;

        /// <summary>
        /// 私有构造方法
        /// </summary>
        private DispatcherAOP():base(typeof(ICoreDisparcher))
        {
            //创建核心类
            m_useCore = new CoreDispatcher();
        }

        /// <summary>
        /// 单例模式获得器 饿汉模式
        /// </summary>
        /// <returns></returns>
        internal static IRevitFunction GetRevitFunction()
        {
            if (null == m_signalTag || null == m_signalTag.m_AOPResult)
            {
                m_signalTag = new DispatcherAOP();
                //制作AOP
                m_signalTag.m_AOPResult = (ICoreDisparcher)m_signalTag.GetTransparentProxy();
                //准备数据
                m_signalTag.m_AOPResult.PrepareData();
            }

            return m_signalTag.m_AOPResult;
        }


        /// <summary>
        /// AOP切面
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override IMessage Invoke(IMessage msg)
        {
            //挂接程序集解析事件
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            //新增Guid
            DEBUGUtility.CreatGuid();
            try
            {
                IMethodCallMessage callmessage = (IMethodCallMessage)msg;

                //调用真实方法  
                object returnValue = callmessage.MethodBase.Invoke(this.m_useCore, callmessage.Args);

                //制作返回值
                ReturnMessage returnmessage = new ReturnMessage(returnValue, new object[0], 0, null, callmessage);

                return returnmessage;
            }
            catch (Exception ex)
            {
                //添加日志信息
                LogUtility.AppendLog(ex);
                //生成日志文件
                LogUtility.CreatLogFile();
                //异常上抛
                throw ex;
            }
            finally
            {
                //卸载事件
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
                //清除guid 
                DEBUGUtility.DropGuid();
                //重置状态
                DEBUGUtility.ResetCondition();
            }
            
        }

        /// <summary>
        /// 程序集解析事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var returnValue = AssemblyLoadUtility.LoadAssembly(args);

            return returnValue;
        }
    }
}
