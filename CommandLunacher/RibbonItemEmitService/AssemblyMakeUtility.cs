using EmitUtility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RibbonItemEmitService
{
    /// <summary>
    /// 程序集制作工具
    /// </summary>
    public class AssemblyMakeUtility
    {
        /// <summary>
        /// 使用的程序集创造器
        /// </summary>
        private AssemblyCreater m_UseCreater = new AssemblyCreater();

        /// <summary>
        /// 使用的程序集请求
        /// </summary>
        private AssemblyMakeRequest m_UseAssemblyReqeuest;

        /// <summary>
        /// 使用的类型制作公用
        /// </summary>
        private MakeTypReuestUtility m_UseTypeRequestUtiltiy;

        /// <summary>
        /// 使用的程序集名称
        /// </summary>
        private string m_useAssemblyName;

        /// <summary>
        /// 使用的文件后缀名
        /// </summary>
        private const string m_appendDllFileName = ".dll";

        /// <summary>
        /// 使用的类型基础名
        /// </summary>
        private const string m_useClassName = "CMDClass";

        /// <summary>
        /// 使用的射出程序集路径
        /// </summary>
        private string m_useFilePath;

        /// <summary>
        /// 使用的当前索引
        /// </summary>
        private int m_useNowIndex = 0;

        /// <summary>
        /// 构造程序集(利用反射）
        /// </summary>
        /// <param name="inputAssemblyName"></param>
        internal AssemblyMakeUtility(string inputAssemblyName,string inputAPIUILocation, string inputAPILocation, string inputLocation = null)
        {
            m_useAssemblyName = inputAssemblyName;
            m_UseAssemblyReqeuest = new AssemblyMakeRequest();
            m_UseAssemblyReqeuest.AssemblyName = m_useAssemblyName;
            m_UseAssemblyReqeuest.UseLocation = inputLocation;
            m_UseAssemblyReqeuest.LstUseTypeMakeRequest = new List<TypeMakeRequest>();

            Assembly nowAssembly = Assembly.GetExecutingAssembly();

            FileInfo nowFileInfo = new FileInfo(nowAssembly.Location);

            FileInfo useFileInfo = new FileInfo(nowFileInfo.Directory.FullName + @"\" + m_useAssemblyName + m_appendDllFileName);

            m_useFilePath = useFileInfo.FullName;

            m_UseTypeRequestUtiltiy = new MakeTypReuestUtility(inputAPIUILocation, inputAPILocation);

        }

        public AssemblyMakeUtility(string inputAssemblyName,string inputLocation = null)
        {
            m_useAssemblyName = inputAssemblyName;
            m_UseAssemblyReqeuest = new AssemblyMakeRequest();
            m_UseAssemblyReqeuest.AssemblyName = m_useAssemblyName;
            m_UseAssemblyReqeuest.UseLocation = inputLocation;
            m_UseAssemblyReqeuest.LstUseTypeMakeRequest = new List<TypeMakeRequest>();

            m_useFilePath = inputLocation;

            if (string.IsNullOrWhiteSpace(m_useFilePath))
            {
                Assembly nowAssembly = Assembly.GetExecutingAssembly();

                FileInfo nowFileInfo = new FileInfo(nowAssembly.Location);

                FileInfo useFileInfo = new FileInfo(nowFileInfo.Directory.FullName + @"\" + m_useAssemblyName + m_appendDllFileName);

                m_useFilePath = useFileInfo.FullName;
            }

           
            m_UseTypeRequestUtiltiy = new MakeTypReuestUtility();
        }

        /// <summary>
        /// 添加一个类型请求
        /// </summary>
        /// <param name="inputFullClassName"></param>
        /// <param name="inputLocation"></param>
        /// <param name="inputUseCoreLocation"></param>
        public void AppendTypeRequest(string inputFullClassName, string inputLocation, string inputUseCoreLocation,out string proxyLocation,out string proxyFullName)
        {
            m_useNowIndex++;
            string useTypeName = m_useClassName + m_useNowIndex.ToString();

            //添加请求
            m_UseAssemblyReqeuest.LstUseTypeMakeRequest.Add(m_UseTypeRequestUtiltiy.MakeTypeRequest
                (useTypeName, inputFullClassName, inputLocation, inputUseCoreLocation));

            proxyLocation = m_useFilePath;
            proxyFullName = useTypeName;
        }
        
        /// <summary>
        /// 制作程序集
        /// </summary>
        /// <returns></returns>
        public string MakeAssembly()
        {
            var returnRespond = m_UseCreater.CreatOneAssembly(m_UseAssemblyReqeuest);

            foreach (var oneClassBean in returnRespond.LstClassBean)
            {
                oneClassBean.UseTypeBuilder.CreateType();
            }

            returnRespond.UseAssembuilder.Save(m_useAssemblyName + m_appendDllFileName);

            return m_useFilePath;
        }


    }
}
