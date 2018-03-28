using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CommandLunacher
{
    internal static class AssemblyLoadUtility
    {
        private static Dictionary<string, Assembly> m_dicPathAssembly = new Dictionary<string, Assembly>();

        /// <summary>
        /// 配置文件名称
        /// </summary>
        private const string CONFIGFILE = "AssemblyConfig.xml";

        /// <summary>
        /// 分版本程序集节点名称
        /// </summary>
        private const string VERSIONASSEMBLYNAME = "VersionAssemblyName";

        /// <summary>
        /// 程序集与文件映射配置节点名称
        /// </summary>
        private const string ASSEMBLYANDFILE = "AssemblyAndFile";

        /// <summary>
        /// 程序集与文件映射之程序集属性名称
        /// </summary>
        private const string ASSEMBLYOFMAP = "Assembly";

        /// <summary>
        /// 程序集与文件映射之文件属性名称
        /// </summary>
        private const string FILEOFMAP = "File";

        /// <summary>
        /// 分版本程序集名称
        /// </summary>
        private static HashSet<string> m_useVersionAssemblyName = new HashSet<string>();

        /// <summary>
        /// 程序集与文件不同名时的映射配置
        /// </summary>
        private static Dictionary<string, string> m_AssemblyNameAndFileNameMap = new Dictionary<string, string>();

        /// <summary>
        /// 版本信息
        /// </summary>
        internal static string m_versionNum = string.Empty;

        /// <summary>
        /// 静态构造方法 配置初始化
        /// </summary>
        static AssemblyLoadUtility()
        {       
            //获取配置文件目录
            string strPath = Assembly.GetExecutingAssembly().Location;
            string strUsePath = new FileInfo(strPath).Directory + @"\" + CONFIGFILE;

            if (File.Exists(strUsePath))
            {
                try
                {
                    //加载文件
                    XmlDocument useDoc = new XmlDocument();
                    useDoc.Load(strUsePath);
                    XmlNode root = useDoc.DocumentElement;
                    foreach (XmlNode rootChild in root.ChildNodes)
                    {
                        //添加分版本程序集
                        if (rootChild.Name.Equals(VERSIONASSEMBLYNAME))
                        {
                            foreach (XmlNode eachName in rootChild.ChildNodes)
                            {
                                m_useVersionAssemblyName.Add(eachName.InnerText);
                            }                         
                        }
                        //添加程序集与文件不同名时的映射信息
                        if (rootChild.Name.Equals(ASSEMBLYANDFILE))
                        {
                            foreach (XmlElement eachPair in rootChild.ChildNodes)
                            {
                                m_AssemblyNameAndFileNameMap.Add(eachPair.Attributes[ASSEMBLYOFMAP].Value,
                                    eachPair.Attributes[FILEOFMAP].Value);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    ;
                }            
            }
        }

        /// <summary>
        /// 程序集加载底层(主动)
        /// </summary>
        /// <param name="inputPath"></param>
        /// <returns></returns>
        internal static Assembly LoadAssembly(string inputPath)
        {
            inputPath = DEBUGUtility.CopyFileAndChangePath(inputPath);
            if (!m_dicPathAssembly.ContainsKey(inputPath))
            {
                //目录变更
                m_dicPathAssembly.Add(inputPath, Assembly.LoadFile(inputPath));
            }

            return m_dicPathAssembly[inputPath];
        }

        /// <summary>
        /// 加载程序集（被动）
        /// </summary>
        /// <param name="inputEventArgs"></param>
        /// <returns></returns>
        internal static Assembly LoadAssembly(ResolveEventArgs inputEventArgs)
        {           
            //获得请求程序集
            var wantAssemblyName = inputEventArgs.Name.Split(',')[0];


            FileInfo useFileInfo = new FileInfo(DEBUGUtility.ResetApplicationLocation(inputEventArgs.RequestingAssembly.Location));

            //文件名转换
            string wantAssemblyFileName = wantAssemblyName;
            if (m_AssemblyNameAndFileNameMap.ContainsKey(wantAssemblyName))
            {
                wantAssemblyFileName = m_AssemblyNameAndFileNameMap[wantAssemblyName];
            }

            string usePath;

            //是否是RevitAPI
            if (m_useVersionAssemblyName.Contains(wantAssemblyName) && !string.IsNullOrEmpty(m_versionNum))
            {
                var createdDirectory = Directory.CreateDirectory(useFileInfo.Directory + @"\" + m_versionNum);
                usePath = createdDirectory.FullName + @"\" + wantAssemblyFileName + ".dll";
            }
            else
            {
                usePath = useFileInfo.Directory + @"\" + wantAssemblyFileName + ".dll";
            }

            //程序集与文件不同名时更改路径名称

            
            //目录变更设置
            usePath = DEBUGUtility.CopyFileAndChangePath(usePath);

            //加载
            return LoadAssembly(usePath);
        }


    }
}
