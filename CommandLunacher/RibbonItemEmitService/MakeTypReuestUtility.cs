using Autodesk.Revit.UI;
using EmitUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RibbonItemEmitService
{
    public class MakeTypReuestUtility
    {
        private const string m_strId = "ID";

        private const string m_strFullClassName = "FullClassName";

        private const string m_strUseLocation = "UseLocation";

        private const string m_strUseCoreLocation = "UseCoreLocation";

        private const string m_strUseCoreMethodName = "ExecuteByAppendValue";

        private const string m_strInterfaceMethodName = "Execute";

        private const string m_strUseCoreFullClassName = "CommandLunacher.WrapperDispatcher";

        private List<FiledMakeRequest> m_lstDefualtFiled;

        private List<CustomAttributeBuilder> m_lstUseAttribute;

        private List<Type> m_lstInterfaceType;

        private List<MethodRequest> m_lstMethodRequest;

        public MakeTypReuestUtility()
        {
            //初始化字段请求
            m_lstDefualtFiled = new List<FiledMakeRequest>();
            m_lstDefualtFiled.Add(new StringFiledMakeRequest() { FiledName = m_strId });
            m_lstDefualtFiled.Add(new StringFiledMakeRequest() { FiledName = m_strFullClassName });
            m_lstDefualtFiled.Add(new StringFiledMakeRequest() { FiledName = m_strUseLocation });
            m_lstDefualtFiled.Add(new StringFiledMakeRequest() { FiledName = m_strUseCoreLocation });

            //初始化特性请求
            m_lstUseAttribute = new List<CustomAttributeBuilder>();
            Type attributeType = typeof(Autodesk.Revit.Attributes.TransactionAttribute);
            //使用的特性构造方法
            var useAttributeConstr = attributeType.GetConstructor(new Type[] { typeof(Autodesk.Revit.Attributes.TransactionMode) });
            m_lstUseAttribute.Add(new CustomAttributeBuilder(useAttributeConstr, new object[] { Autodesk.Revit.Attributes.TransactionMode.Manual }));

            //准备接口类型列表
            m_lstInterfaceType = new List<Type>();
            m_lstInterfaceType.Add(typeof(IExternalCommand));


            m_lstMethodRequest = new List<MethodRequest>();

            MethodInfo useInterfaceMethodInfo = typeof(IExternalCommand).GetMethod(m_strInterfaceMethodName);

            MethodRequest tempMethodRequest = new MethodRequest();
            tempMethodRequest.Name = m_strInterfaceMethodName;
            tempMethodRequest.ReturnType = useInterfaceMethodInfo.ReturnType;

            tempMethodRequest.ParameterTypes = (from n in useInterfaceMethodInfo.GetParameters() select n.ParameterType).ToArray();
            tempMethodRequest.UseBaseMethod = useInterfaceMethodInfo;


        }


        public TypeMakeRequest MakeTypeRequest(string inputTypeName, string inputFullClassName,string inputLocation,string inputUseCoreLocation)
        {
            TypeMakeRequest returnValue = new TypeMakeRequest();

            returnValue.TypeName = inputTypeName;

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

        private static void UseReflectMethodCreated(MethodBuilder inputMethodBuilder, ClassBuilderBean useClassBuilderBean)
        {
            //框架位置
            var useCoreLocationFileBuilder = useClassBuilderBean.UseFiledDic[m_strUseCoreLocation];

            //使用类的全名称
            var useFullNameBuilder = useClassBuilderBean.UseFiledDic[m_strFullClassName];

            //使用类的路径
            var useClassLocationBuilder = useClassBuilderBean.UseFiledDic[m_strUseLocation];

            //使用的Id
            var useIdBuilder = useClassBuilderBean.UseFiledDic[m_strId];

            //获得使用的il
            var useIl = inputMethodBuilder.GetILGenerator();
            var useLocalAssembly = useIl.DeclareLocal(typeof(Assembly));
            var useLocalType = useIl.DeclareLocal(typeof(Type));
            var useLocalInstance = useIl.DeclareLocal(typeof(object));
            var useLocalMethodName = useIl.DeclareLocal(typeof(string));
            var useLocalMethodInfo = useIl.DeclareLocal(typeof(MethodInfo));
            var useLocalLstObj = useIl.DeclareLocal(typeof(List<object>));
            var useLocalArrayObj = useIl.DeclareLocal(typeof(object[]));

            //获得目标位置处的Assembly
            useIl.Emit(OpCodes.Ldarg_0);
            useIl.Emit(OpCodes.Ldfld, useCoreLocationFileBuilder);
            var useLoadFileMethod = typeof(Assembly).GetMethod("LoadFile", new Type[] { typeof(string) });
            useIl.Emit(OpCodes.Call, useLoadFileMethod);
            useIl.Emit(OpCodes.Stloc, useLocalAssembly);
            useIl.Emit(OpCodes.Ldloc, useLocalAssembly);

            //获得目标type
            useIl.Emit(OpCodes.Ldstr, m_strUseCoreFullClassName);
            var useLoadTypeMethod = typeof(Assembly).GetMethod("GetType", new Type[] { typeof(string) });
            useIl.Emit(OpCodes.Call, useLoadTypeMethod);
            useIl.Emit(OpCodes.Stloc, useLocalType);

            //获得目标MehtodInfo
            useIl.Emit(OpCodes.Ldloc, useLocalType);
            useIl.Emit(OpCodes.Ldstr, m_strUseCoreMethodName);
            var useFindMethod = typeof(Type).GetMethod("GetMethod", new Type[] { typeof(string) });
            useIl.Emit(OpCodes.Call, useFindMethod);
            useIl.Emit(OpCodes.Stloc, useLocalMethodInfo);

            //创建对象实例
            useIl.Emit(OpCodes.Ldloc, useLocalType);
            var useCreatInstanceMethod = typeof(Activator).GetMethod("CreateInstance", new Type[] { typeof(Type) });
            useIl.Emit(OpCodes.Call, useCreatInstanceMethod);
            useIl.Emit(OpCodes.Stloc, useLocalInstance);

            //创建List<object>
            useIl.Emit(OpCodes.Newobj, typeof(List<object>).GetConstructor(new Type[] { }));
            useIl.Emit(OpCodes.Stloc, useLocalLstObj);


            //获取添加方法
            var useAddMehtod = typeof(List<object>).GetMethod("Add");

            //添加ExternalCommandData
            useIl.Emit(OpCodes.Ldloc, useLocalArrayObj);
            useIl.Emit(OpCodes.Ldarg_1);
            useIl.Emit(OpCodes.Call, useAddMehtod);

            //添加message
            useIl.Emit(OpCodes.Ldloc, useLocalArrayObj);
            useIl.Emit(OpCodes.Ldarg_2);
            useIl.Emit(OpCodes.Call, useAddMehtod);

            //ElementSet
            useIl.Emit(OpCodes.Ldloc, useLocalArrayObj);
            useIl.Emit(OpCodes.Ldarg_3);
            useIl.Emit(OpCodes.Call, useAddMehtod);

            //添加Guid
            useIl.Emit(OpCodes.Ldloc, useLocalArrayObj);
            useIl.Emit(OpCodes.Ldarg_0);
            useIl.Emit(OpCodes.Ldfld, useIdBuilder);
            useIl.Emit(OpCodes.Call, useAddMehtod);

            //添加类路径
            useIl.Emit(OpCodes.Ldloc, useLocalArrayObj);
            useIl.Emit(OpCodes.Ldarg_0);
            useIl.Emit(OpCodes.Ldfld, useClassLocationBuilder);
            useIl.Emit(OpCodes.Call, useAddMehtod);


            //添加类全名称
            useIl.Emit(OpCodes.Ldloc, useLocalArrayObj);
            useIl.Emit(OpCodes.Ldarg_0);
            useIl.Emit(OpCodes.Ldfld, useFullNameBuilder);
            useIl.Emit(OpCodes.Call, useAddMehtod);

            //转换为Objcet[]
            var useToArrayMethod = typeof(List<object>).GetMethod("ToArray");
            useIl.Emit(OpCodes.Ldloc, useLocalLstObj);
            useIl.Emit(OpCodes.Call, useToArrayMethod);
            useIl.Emit(OpCodes.Stloc, useLocalArrayObj);

            //唤醒目标方法
            var useInvokeMethod = typeof(MethodInfo).GetMethod("Invoke", new Type[] { typeof(object), typeof(object[]) });
            useIl.Emit(OpCodes.Ldloc, useLocalMethodInfo);
            useIl.Emit(OpCodes.Ldloc, useLocalInstance);
            useIl.Emit(OpCodes.Ldloc, useLocalArrayObj);
            useIl.Emit(OpCodes.Call, useInvokeMethod);


            useIl.Emit(OpCodes.Ret);
        }
    }
}
