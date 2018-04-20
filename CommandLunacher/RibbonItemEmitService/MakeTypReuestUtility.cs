
using Autodesk.Revit.Attributes;
using EmitUtility;
using InvokeUtility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RibbonItemEmitService
{
    /// <summary>
    /// 类型请求制作工具
    /// </summary>
    internal class MakeTypReuestUtility
    {
        #region 字符串常量
        /// <summary>
        /// 使用的Id
        /// </summary>
        private const string m_strId = "ID";

        /// <summary>
        /// 使用类的全名称
        /// </summary>
        private const string m_strFullClassName = "FullClassName";

        /// <summary>
        /// 使用类的位置
        /// </summary>
        private const string m_strUseLocation = "UseLocation";

        /// <summary>
        /// 框架类的位置
        /// </summary>
        private const string m_strUseCoreLocation = "UseCoreLocation";

        /// <summary>
        /// 使用的接口名称
        /// </summary>
        private const string m_strInterfaceMethodName = "Execute";

        /// <summary>
        /// 辅助方法的方法名
        /// </summary>
        private const string m_strHelpMethodName = "HelpInvoke";

        /// <summary>
        /// 添加方法的方法名
        /// </summary>
        private const string m_strAddMethodName = "Add";
        #endregion

        #region 私有字段

        /// <summary>
        /// 字段请求列表
        /// </summary>

        private List<FiledMakeRequest> m_lstDefualtFiled;

        /// <summary>
        /// 特性请求列表
        /// </summary>
        private List<CustomAttributeBuilder> m_lstUseAttribute;

        /// <summary>
        /// 接口类型列表
        /// </summary>
        private List<Type> m_lstInterfaceType;

        /// <summary>
        /// 使用的基础类
        /// </summary>
        private Type m_baseType = null;

        /// <summary>
        /// 方法请求列表
        /// </summary>
        private List<MethodRequest> m_lstMethodRequest;

        /// <summary>
        /// 使用类的全名称
        /// </summary>
        internal static string FullClassName
        {
            get
            {
                return m_strFullClassName;
            }
        }

        /// <summary>
        /// 使用类的位置
        /// </summary>
        internal static string UseLocation
        {
            get
            {
                return m_strUseLocation;
            }
        }
        #endregion

        /// <summary>
        /// 构造方法
        /// </summary>
        internal MakeTypReuestUtility()
        {
            PrepareType();
        }

        /// <summary>
        /// 制作一个类型请求
        /// </summary>
        /// <param name="inputTypeName"></param>
        /// <param name="inputFullClassName"></param>
        /// <param name="inputLocation"></param>
        /// <param name="inputUseCoreLocation"></param>
        /// <returns></returns>
        internal TypeMakeRequest MakeTypeRequest(string inputTypeName, string inputFullClassName,string inputLocation,string inputUseCoreLocation)
        {
            PrepareFiled();
            TypeMakeRequest returnValue = new TypeMakeRequest();

            returnValue.TypeName = inputTypeName;

            returnValue.ParentType = m_baseType;

            returnValue.LstFiled = m_lstDefualtFiled;

            //修改字段默认值
            m_lstDefualtFiled[0].DefualtValue = Guid.NewGuid().ToString().ToLower();
            m_lstDefualtFiled[1].DefualtValue = inputFullClassName;
            m_lstDefualtFiled[2].DefualtValue = inputLocation;
            m_lstDefualtFiled[3].DefualtValue = inputUseCoreLocation;

            returnValue.LstAttribute = m_lstUseAttribute;
            returnValue.LstInterfaceType = m_lstInterfaceType;

            returnValue.LstMethodRequest = m_lstMethodRequest;

            return returnValue;
        }

        /// <summary>
        /// 制作反射方法
        /// </summary>
        /// <param name="inputMethodBuilder"></param>
        /// <param name="useClassBuilderBean"></param>
        private void UseReflectMethodCreated(MethodBuilder inputMethodBuilder, ClassBuilderBean useClassBuilderBean)
        {
            //获得辅助方法
            MethodInfo useMethod = typeof(InvokeTool).GetMethod(m_strHelpMethodName);

            //框架位置
            var useCoreLocationFileBuilder = useClassBuilderBean.UseFiledDic[m_strUseCoreLocation];

            //使用类的全名称
            var useFullNameBuilder = useClassBuilderBean.UseFiledDic[FullClassName];

            //使用类的路径
            var useClassLocationBuilder = useClassBuilderBean.UseFiledDic[UseLocation];

            //使用的Id
            var useIdBuilder = useClassBuilderBean.UseFiledDic[m_strId];

            //获取中间码
            var useIl = inputMethodBuilder.GetILGenerator();

            //获取添加方法
            var useAddMehtod = typeof(List<object>).GetMethod(m_strAddMethodName);

            var useLocalLstObj = useIl.DeclareLocal(typeof(List<object>));
            useIl.Emit(OpCodes.Newobj, typeof(List<object>).GetConstructor(new Type[] { }));
            useIl.Emit(OpCodes.Stloc, useLocalLstObj);

            useIl.Emit(OpCodes.Ldloc, useLocalLstObj);
            useIl.Emit(OpCodes.Ldarg_1);
            useIl.Emit(OpCodes.Call, useAddMehtod);

            useIl.Emit(OpCodes.Ldloc, useLocalLstObj);
            useIl.Emit(OpCodes.Ldstr, string.Empty);
            useIl.Emit(OpCodes.Call, useAddMehtod);

            useIl.Emit(OpCodes.Ldloc, useLocalLstObj);
            useIl.Emit(OpCodes.Ldarg_3);
            useIl.Emit(OpCodes.Call, useAddMehtod);

            useIl.Emit(OpCodes.Ldloc, useLocalLstObj);
            useIl.Emit(OpCodes.Ldarg_0);
            useIl.Emit(OpCodes.Ldfld, useIdBuilder);
            useIl.Emit(OpCodes.Call, useAddMehtod);

            useIl.Emit(OpCodes.Ldloc, useLocalLstObj);
            useIl.Emit(OpCodes.Ldarg_0);
            useIl.Emit(OpCodes.Ldfld, useClassLocationBuilder);
            useIl.Emit(OpCodes.Call, useAddMehtod);

            useIl.Emit(OpCodes.Ldloc, useLocalLstObj);
            useIl.Emit(OpCodes.Ldarg_0);
            useIl.Emit(OpCodes.Ldfld, useFullNameBuilder);
            useIl.Emit(OpCodes.Call, useAddMehtod);

            useIl.Emit(OpCodes.Ldloc, useLocalLstObj);
            useIl.Emit(OpCodes.Ldarg_0);
            useIl.Emit(OpCodes.Ldfld, useCoreLocationFileBuilder);
            useIl.Emit(OpCodes.Call, useAddMehtod);

            useIl.Emit(OpCodes.Ldloc, useLocalLstObj);
            useIl.Emit(OpCodes.Call, useMethod);

            useIl.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// 数据准备
        /// </summary>
        private void PrepareType()
        {
            //准备特性
            Type attributeType = typeof(Autodesk.Revit.Attributes.TransactionAttribute);

            Type transactionModeType = typeof(Autodesk.Revit.Attributes.TransactionMode);

            object useTransactionMode = Autodesk.Revit.Attributes.TransactionMode.Manual;

            PrepareFiled();

            //初始化特性请求
            m_lstUseAttribute = new List<CustomAttributeBuilder>();

            //使用的特性构造方法
            var useAttributeConstr = attributeType.GetConstructor(new Type[] { transactionModeType });

            m_lstUseAttribute.Add(new CustomAttributeBuilder(useAttributeConstr, new object[] { 1 }));


            //准备接口类型列表
            m_lstInterfaceType = new List<Type>();

            m_lstMethodRequest = new List<MethodRequest>();

            //使用的基类
            m_baseType = typeof(BaseInvokeClass);

            //接口方法
            MethodInfo useInterfaceMethodInfo = m_baseType.GetMethod(m_strInterfaceMethodName);

            //准备方法Bean
            MethodRequest tempMethodRequest = new MethodRequest();
            tempMethodRequest.Name = m_strInterfaceMethodName;
            tempMethodRequest.ReturnType = useInterfaceMethodInfo.ReturnType;
            tempMethodRequest.ParameterTypes = (from n in useInterfaceMethodInfo.GetParameters() select n.ParameterType).ToArray();
            tempMethodRequest.UseBaseMethod = useInterfaceMethodInfo;
            tempMethodRequest.UseMethodDel = new MethodCreatDel(UseReflectMethodCreated);
            m_lstMethodRequest.Add(tempMethodRequest);

        }

        /// <summary>
        /// 准备字段
        /// </summary>
        private void PrepareFiled()
        {
            //初始化字段请求
            m_lstDefualtFiled = new List<FiledMakeRequest>();
            m_lstDefualtFiled.Add(new StringFiledMakeRequest() { FiledName = m_strId });
            m_lstDefualtFiled.Add(new StringFiledMakeRequest() { FiledName = FullClassName });
            m_lstDefualtFiled.Add(new StringFiledMakeRequest() { FiledName = UseLocation });
            m_lstDefualtFiled.Add(new StringFiledMakeRequest() { FiledName = m_strUseCoreLocation });
        }
    }
}
