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
    /// 类型代理工厂
    /// </summary>
    public class TypeProxyFactory
    {
        /// <summary>
        /// 使用的代理程序集地址
        /// </summary>
        private string m_inputDir;

        /// <summary>
        /// 使用的代理程序集名称
        /// </summary>
        private string m_inputName;

        /// <summary>
        /// 使用的统一调度程序集路径
        /// </summary>
        private string m_inputCoreLocation;

        /// <summary>
        /// 代理程序集是否已存在
        /// </summary>
        private bool m_ifAssemblyExist;

        /// <summary>
        /// .dll常量
        /// </summary>
        private const string m_strUseApeendName = ".dll";

        /// <summary>
        /// 当前使用的代理程序集文件路径
        /// </summary>
        private string m_useAssemblyFilePath;

        /// <summary>
        /// 已加载的代理类型封装 key1 实际地址 key2 实际全名称
        /// </summary>
        private Dictionary<string, Dictionary<string, InstancePacker>> useDic;

        /// <summary>
        /// 使用的程序集创建工具
        /// </summary>
        private AssemblyMakeUtility m_useAssemblyMakeUtility;

        /// <summary>
        /// 程序集是否存在
        /// </summary>
        public bool IfAssemblyExist
        {
            get
            {
                return m_ifAssemblyExist;
            }

            private set
            {
                m_ifAssemblyExist = value;
            }
        }

        /// <summary>
        /// 构造代理创造工程
        /// </summary>
        /// <param name="inputDir"></param>
        /// <param name="inputName"></param>
        /// <param name="inputCoreLocation"></param>
        /// <param name="ifDeleteExist"></param>
        public TypeProxyFactory(string inputDir, string inputName, string inputCoreLocation , bool ifDeleteExist = false)
        {
            m_inputDir = inputDir;
            m_inputName = inputName;
            m_inputCoreLocation = inputCoreLocation;

            FileInfo useFileInfo = new FileInfo(m_inputDir + @"\" + m_inputName + m_strUseApeendName);

            m_useAssemblyFilePath = useFileInfo.FullName;

            //判断删除
            if (useFileInfo.Exists && ifDeleteExist)
            {
                useFileInfo.Delete();
                useFileInfo = new FileInfo(useFileInfo.FullName);
            }

            //若文件不存在
            if (!useFileInfo.Exists)
            {

                IfAssemblyExist = false;
                //创造程序集工具
                m_useAssemblyMakeUtility = new AssemblyMakeUtility(m_inputName, m_inputDir);
                //判断路径与创建
                var useDirection = useFileInfo.Directory;
                if (!useDirection.Exists)
                {
                    useDirection.Create();
                }
            }
            else
            {
                PrepareWhenFileExits(useFileInfo);

            }
        }

        /// <summary>
        /// 当文件存在时使用
        /// </summary>
        /// <param name="useFileInfo"></param>
        private void PrepareWhenFileExits(FileInfo useFileInfo)
        {
            IfAssemblyExist = true;

            //加载程序集
            Assembly useAssembly = Assembly.LoadFile(useFileInfo.FullName);

            //获得程序集所有的类对象
            var types = useAssembly.GetTypes();

            List<InstancePacker> useLstObjects = new List<InstancePacker>();
            object tempObj;

            foreach (var oneType in types)
            {
                tempObj = null;
                try
                {
                    tempObj = Activator.CreateInstance(oneType);
                    InstancePacker tempInstance = new InstancePacker(tempObj);

                    if (tempInstance.IFCanUse())
                    {
                        useLstObjects.Add(tempInstance);
                    }
                }
                catch (Exception)
                {
                    ;
                }
            }

            useDic = new Dictionary<string, Dictionary<string, InstancePacker>>();

            foreach (var oneObj in useLstObjects)
            {
                if (!useDic.ContainsKey(oneObj.UseLocation))
                {
                    useDic.Add(oneObj.UseLocation, new Dictionary<string, InstancePacker>());
                }

                var tempDic = useDic[oneObj.UseLocation];

                if (!tempDic.ContainsKey(oneObj.UseFullName))
                {
                    tempDic.Add(oneObj.UseFullName, oneObj);
                }
            }
        }

        /// <summary>
        /// 获取代理地址与全名称
        /// </summary>
        /// <param name="inputRealLocation">实际地址</param>
        /// <param name="inputRealFullName">实际全名称</param>
        /// <param name="useProxyLocation">代理地址</param>
        /// <param name="useProxyFullName">代理全名称</param>
        public void GetProxy(string inputRealLocation, string inputRealFullName, out string useProxyLocation, out string useProxyFullName)
        {
            useProxyLocation = null;
            useProxyFullName = null;

            //若已存在则获取
            if (m_ifAssemblyExist)
            {
                useProxyLocation = m_useAssemblyFilePath;
                var usePacker = useDic[inputRealLocation][inputRealFullName];
                useProxyFullName = usePacker.UseFullName;
            }
            else
            {
                //代理生成
                m_useAssemblyMakeUtility.AppendTypeRequest(inputRealFullName, inputRealLocation,m_inputCoreLocation ,out useProxyLocation, out useProxyFullName);
            }
        }

        /// <summary>
        /// 制作程序集
        /// </summary>
        public void MakeAssembly()
        {
            m_useAssemblyMakeUtility.MakeAssembly();
        }
    }
}
