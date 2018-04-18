using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace EmitUtility
{
    /// <summary>
    /// 程序集创建器
    /// </summary>
    public class AssemblyCreater
    {
        /// <summary>
        /// 使用的dll附加字符串
        /// </summary>
        private const string m_strUseDllAppend = ".dll";

        /// <summary>
        /// 制作程序集
        /// </summary>
        /// <param name="inputRequest"></param>
        /// <returns></returns>
        public AssemblyRespondBean CreatOneAssembly(AssemblyMakeRequest inputRequest)
        {
            //列表保护
            if (null == inputRequest.LstUseTypeMakeRequest)
            {
                inputRequest.LstUseTypeMakeRequest = new List<TypeMakeRequest>();
            }

            AssemblyRespondBean respond = new AssemblyRespondBean();

            string outPutLocation;
            var tempAssemblyBuilder = GetAssemblyBuilder(inputRequest.AssemblyName, out outPutLocation, inputRequest.UseLocation);

            var tempModuleBuilder = GetModuleBuilder(tempAssemblyBuilder, inputRequest.AssemblyName);

            //数据回写
            respond.UseAssemblyDir = outPutLocation;
            respond.UseAssembuilder = tempAssemblyBuilder;
            respond.UseModuleBuilder = tempModuleBuilder;
            respond.LstClassBean = new List<ClassBuilderBean>();

            foreach (var oneTypeRequest in inputRequest.LstUseTypeMakeRequest)
            {
                ClassBuilderBean typeRespond = new ClassBuilderBean();
                //添加到返回值列表
                respond.LstClassBean.Add(typeRespond);
                typeRespond.UseLstKVPMethodBuilder = new List<KeyValuePair<MethodRequest, MethodBuilder>>();

                //创造类型
                var tempTypeBuilder = CreatType(tempModuleBuilder, oneTypeRequest.TypeName,
                    oneTypeRequest.ParentType, oneTypeRequest.LstInterfaceType, oneTypeRequest.LstAttribute);
                typeRespond.UseTypeBuilder = tempTypeBuilder;

                Dictionary<FieldBuilder, FiledMakeRequest> fieldDic;
                //创造字段
                AppendFileds(tempTypeBuilder, oneTypeRequest.LstFiled, out fieldDic);

                //字段回写
                typeRespond.UseFiledDic = fieldDic.ToDictionary(k => k.Value.FiledName, k => k.Key);

                //制作方法
                if (null == oneTypeRequest.LstMethodRequest)
                {
                    continue; 
                }

                foreach (var oneMethodRequest in oneTypeRequest.LstMethodRequest)
                {
                    MakeMethod(typeRespond, oneMethodRequest);
                }
            }


            return respond;

        }

        #region 私有方法
        /// <summary>
        /// 创建一个AssemblyBuilder
        /// </summary>
        /// <param name="inputName"></param>
        /// <returns></returns>
        private AssemblyBuilder GetAssemblyBuilder(string inputName, out string outPutLocation, string inputLocation = null)
        {
            outPutLocation = null;

            AssemblyName useAssemblyName = new AssemblyName(inputName);

            AppDomain useAppDomain = AppDomain.CurrentDomain;

            string useLocation = inputLocation;

            //获得当前路径
            if (string.IsNullOrWhiteSpace(useLocation))
            {
                Assembly nowAssembly = Assembly.GetExecutingAssembly();

                FileInfo useFileInfo = new FileInfo(nowAssembly.Location);

                useLocation = useFileInfo.Directory.FullName;
            }

            outPutLocation = useLocation;

            return useAppDomain.DefineDynamicAssembly(useAssemblyName, AssemblyBuilderAccess.RunAndSave, useLocation);
        }

        /// <summary>
        /// 创建模块
        /// </summary>
        /// <param name="inputAssemblyBuilder"></param>
        /// <param name="inputName"></param>
        /// <returns></returns>
        private ModuleBuilder GetModuleBuilder(AssemblyBuilder inputAssemblyBuilder, string inputName)
        {
            return inputAssemblyBuilder.DefineDynamicModule(inputName, inputName + m_strUseDllAppend);
        }

        /// <summary>
        /// 创建类型
        /// </summary>
        /// <param name="inputModuleBuilder"></param>
        /// <param name="inputName"></param>
        /// <param name="lstInterfaceType"></param>
        /// <param name="lstClassAttributeBuilder"></param>
        /// <returns></returns>
        private TypeBuilder CreatType(ModuleBuilder inputModuleBuilder, string inputName, Type parentType = null,
            List<Type> lstInterfaceType = null, List<CustomAttributeBuilder> lstClassAttributeBuilder = null)
        {
            if (null == parentType)
            {
                parentType = typeof(object);
            }

            if (null == lstInterfaceType)
            {
                lstInterfaceType = new List<Type>();
            }

            if (null == lstClassAttributeBuilder)
            {
                lstClassAttributeBuilder = new List<CustomAttributeBuilder>();
            }

            var tempTypeBuilder = inputModuleBuilder.DefineType(inputName, TypeAttributes.Public, parentType, lstInterfaceType.ToArray());

            //设置特性
            foreach (var oneAtrributeBuilder in lstClassAttributeBuilder)
            {
                tempTypeBuilder.SetCustomAttribute(oneAtrributeBuilder);
            }

            return tempTypeBuilder;
        }

        /// <summary>
        /// 添加字段
        /// </summary>
        /// <param name="inputTypeBuilder"></param>
        /// <param name="lstInputRequest"></param>
        /// <param name="useFiledDic"></param>
        private void AppendFileds(TypeBuilder inputTypeBuilder, List<FiledMakeRequest> lstInputRequest
            , out Dictionary<FieldBuilder, FiledMakeRequest> useFiledDic)
        {
            if (null == lstInputRequest)
            {
                lstInputRequest = new List<FiledMakeRequest>();
            }

            useFiledDic = new Dictionary<FieldBuilder, FiledMakeRequest>();

            foreach (var oneRequest in lstInputRequest)
            {
                Type useType = oneRequest.UseType;

                if (null == useType)
                {
                    continue;
                }

                var tempFiledBuilder = inputTypeBuilder.DefineField(oneRequest.FiledName, useType, FieldAttributes.Public);

                //添加映射字典
                useFiledDic.Add(tempFiledBuilder, oneRequest);
            }

            PrepareConstructByFieldDic(inputTypeBuilder, useFiledDic);

        }

        /// <summary>
        /// 制作方法
        /// </summary>
        /// <param name="inputClassBuilderBean"></param>
        /// <param name="inputRequest"></param>
        private void MakeMethod(ClassBuilderBean inputClassBuilderBean, MethodRequest inputRequest)
        {
            var tempUseTypeBuilder = inputClassBuilderBean.UseTypeBuilder;

            var tempMethodBuilder = tempUseTypeBuilder.DefineMethod(inputRequest.Name,  MethodAttributes.Public | MethodAttributes.Virtual,
                inputRequest.ReturnType, inputRequest.ParameterTypes);

            inputRequest.UseMethodDel(tempMethodBuilder, inputClassBuilderBean);

            if (null != inputRequest.UseBaseMethod)
            {
                tempUseTypeBuilder.DefineMethodOverride(tempMethodBuilder, inputRequest.UseBaseMethod);
            }

            //生成数据回写
            inputClassBuilderBean.UseLstKVPMethodBuilder.Add
                (new KeyValuePair<MethodRequest, MethodBuilder>(inputRequest, tempMethodBuilder));
        }


        /// <summary>
        /// 根据字段描述进行初始化
        /// </summary>
        /// <param name="inputTypeBuilder"></param>
        /// <param name="useFiledDic"></param>
        private void PrepareConstructByFieldDic(TypeBuilder inputTypeBuilder, Dictionary<FieldBuilder, FiledMakeRequest> useFiledDic)
        {
            //定义初始化方法
            var tempConstruct = inputTypeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { });
            ILGenerator useConstructGen = tempConstruct.GetILGenerator();

            //加载参数调度Object构造方法
            useConstructGen.Emit(OpCodes.Ldarg_0);
            ConstructorInfo superConstructor = typeof(Object).GetConstructor(new Type[0]);
            useConstructGen.Emit(OpCodes.Call, superConstructor);

            for (int useIndex = 0; useIndex < useFiledDic.Count; useIndex++)
            {
                var useKVP = useFiledDic.ElementAt(useIndex);

                //没有默认值跳过
                if (null == useKVP.Value.DefualtValue)
                {
                    continue;
                }

                //若是字符串类型
                if (useKVP.Value is StringFiledMakeRequest)
                {
                    //加载参数
                    useConstructGen.Emit(OpCodes.Ldarg_0);
                    useConstructGen.Emit(OpCodes.Ldstr, (string)useKVP.Value.DefualtValue);
                    useConstructGen.Emit(OpCodes.Stfld, useKVP.Key);
                }

            }

            //返回
            useConstructGen.Emit(OpCodes.Ret);
        } 
        #endregion

    }
}
