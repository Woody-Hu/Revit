using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CommandLunacher
{
    /// <summary>
    /// 动态事件处理系统 单例
    /// </summary>
    internal class DynamicEventhanlder
    {
        #region 私有字段
        /// <summary>
        /// 使用的程序集名称
        /// </summary>
        private const string m_strUseAssemblyName = "DynamicEvenhanlderAssembly";

        /// <summary>
        /// 使用的类型基础名
        /// </summary>
        private const string m_strBaseClassName = "EventHalderSpace.Hanlder";

        /// <summary>
        /// 当前使用的索引
        /// </summary>
        private int m_useIndex = 0;

        /// <summary>
        /// 使用的方法名
        /// </summary>
        private const string m_strUseMethodName = "UseMethod";

        /// <summary>
        /// 使用的Invoke方法名
        /// </summary>
        private const string m_strUseInvoke = "Invoke";

        /// <summary>
        /// 使用的动态程序集
        /// </summary>
        private AssemblyBuilder m_useAssemblyBuilder = null;

        /// <summary>
        /// 使用的动态模块
        /// </summary>
        private ModuleBuilder m_useModuleBuilder = null;

        /// <summary>
        /// 使用的单例标签
        /// </summary>
        private static DynamicEventhanlder m_singalTag = null;

        /// <summary>
        /// 添加程序集解析事件方法
        /// </summary>
        private MethodInfo m_addAssemblyMethod = null;

        /// <summary>
        /// 卸载程序集解析事件方法
        /// </summary>
        private MethodInfo m_removeAseemblyMethod = null;

        /// <summary>
        /// 使用的Void类型
        /// </summary>
        private Type m_useVoidType;
        #endregion

        /// <summary>
        /// 获取处理器
        /// </summary>
        /// <returns></returns>
        internal static DynamicEventhanlder GetHanlder()
        {
            if (null == m_singalTag)
            {
                m_singalTag = new DynamicEventhanlder();
            }

            return m_singalTag;
        }

        /// <summary>
        /// 私有构造
        /// </summary>
        private DynamicEventhanlder()
        {
            //预定义程序集与模块
            m_useAssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(m_strUseAssemblyName) , AssemblyBuilderAccess.Run);

            m_useModuleBuilder = m_useAssemblyBuilder.DefineDynamicModule(m_strUseAssemblyName);

            Type useType = typeof(DynamicEventhanlder);

            //获取挂接程序集方法
            m_addAssemblyMethod = useType.GetMethod("AddAssemblyLoad", BindingFlags.Public | BindingFlags.Static);

            //获取移除程序集解析事件方法
            m_removeAseemblyMethod = useType.GetMethod("RemoveAssemblyLoad", BindingFlags.Public | BindingFlags.Static);

            //VoidType
            m_useVoidType = typeof(void);
        }

        /// <summary>
        /// 为输入对象动态挂接事件
        /// </summary>
        /// <param name="inputObject"></param>
        /// <param name="ifAdd"></param>
        internal void LinkEventHanlderToObject(object inputObject,bool ifAdd)
        {
            if (null == inputObject)
            {
                return;
            }

            Type useType = inputObject.GetType();


            foreach (var oneEventInfo in useType.GetEvents())
            {
                //获取事件对于的方法 参数与类型
                Type handlerType = oneEventInfo.EventHandlerType;
                MethodInfo invokeMethod = handlerType.GetMethod(m_strUseInvoke);

                //返回值检查
                if (m_useVoidType != invokeMethod.ReturnType)
                {
                    continue;
                }

                AddEventHanlderToObj(inputObject, ifAdd, oneEventInfo, handlerType, invokeMethod);
            }
        }

        /// <summary>
        /// 生成临时程序集处理方法并挂接到事件
        /// </summary>
        /// <param name="inputObject">输入的对象</param>
        /// <param name="ifAdd">添加/移除事件</param>
        /// <param name="oneEventInfo">对应的事件对象</param>
        /// <param name="handlerType">事件对应的委托类型</param>
        /// <param name="invokeMethod">委托对应的方法签名</param>
        private void AddEventHanlderToObj(object inputObject, bool ifAdd, EventInfo oneEventInfo, Type handlerType, MethodInfo invokeMethod)
        {
            ParameterInfo[] parms = invokeMethod.GetParameters();
            Type[] parmTypes = new Type[parms.Length];
            for (int i = 0; i < parms.Length; i++)
            {
                parmTypes[i] = parms[i].ParameterType;
            }

            //序号增加
            m_useIndex++;

            var useStr = m_strBaseClassName + m_useIndex.ToString();

            //类型创建器
            var tempTypeBuilder = m_useModuleBuilder.DefineType(useStr, TypeAttributes.Public | TypeAttributes.Class);

            //定义方法签名
            var useMethodBuilder = tempTypeBuilder.DefineMethod
                (m_strUseMethodName, MethodAttributes.Public | MethodAttributes.Static, invokeMethod.ReturnType, parmTypes);

            var useIl = useMethodBuilder.GetILGenerator();

            //挂接移除事件
            if (ifAdd)
            {
                useIl.Emit(OpCodes.Call, m_addAssemblyMethod);
            }
            else
            {
                useIl.Emit(OpCodes.Call, m_removeAseemblyMethod);
            }

            useIl.Emit(OpCodes.Ret);

            //生成类型
            var createdType = tempTypeBuilder.CreateType();

            //获取生成的方法
            var createdMethod = createdType.GetMethod(m_strUseMethodName);

            //获取委托
            Delegate usedel = Delegate.CreateDelegate(handlerType, createdMethod);

            //添加委托进事件
            oneEventInfo.AddEventHandler(inputObject, usedel);
        }

        #region 反射调用方法
        /// <summary>
        /// 添加Assembly解析事件
        /// </summary>
        public static void AddAssemblyLoad()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyLoadUtility.NoneDebugLoadAssembly;
        }

        /// <summary>
        /// 一次Assembly解析事件
        /// </summary>
        public static void RemoveAssemblyLoad()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= AssemblyLoadUtility.NoneDebugLoadAssembly;
        } 
        #endregion
    }
}
