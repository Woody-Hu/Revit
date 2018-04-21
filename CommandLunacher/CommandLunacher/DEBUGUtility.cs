using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace CommandLunacher
{
    /// <summary>
    /// DEBUG工具模块
    /// </summary>
    internal static class DEBUGUtility
    {
        #region 私有变量
        /// <summary>
        /// Debug模式标签
        /// </summary>
        private static readonly bool m_ifDebugModel = false;

        /// <summary>
        /// 使用的多重复制的格式
        /// </summary>
        private static List<Regex> m_useMulCopyRegex = new List<Regex>();

        /// <summary>
        /// 当前使用的Guid
        /// </summary>
        private static Guid? m_nowGuid = null;

        /// <summary>
        /// 当前使用的AddinId
        /// </summary>
        private static string m_nowUseAddinId = null;

        /// <summary>
        /// 是否已多重复制
        /// </summary>
        private static bool m_bHasMulCopy = false;

        /// <summary>
        /// 使用的保存级数
        /// </summary>
        private static readonly int m_saveParentLevel = 0;

        /// <summary>
        /// 已使用的Guid列表
        /// </summary>
        private static Dictionary<string, Guid?> m_usedAddinIdAndGuidDic = new Dictionary<string, Guid?>();

        /// <summary>
        /// 已使用的Application索引与Guid映射
        /// </summary>
        private static Dictionary<int, Guid?> m_useApplicationIdAndGuidDic = new Dictionary<int, Guid?>();

        /// <summary>
        /// Application及其对应的路径列表
        /// </summary>
        private static Dictionary<int, string> m_useApplicationIdAndPathDic = new Dictionary<int, string>();

        /// <summary>
        /// 不进行Debug模式的App程序集文件名称列表
        /// </summary>
        private static HashSet<string> m_NoneDebugNameSet = new HashSet<string>();

        /// <summary>
        /// 是否对Application进行Debug模式
        /// </summary>
        private static bool m_bIfDebugApplication = true;

        /// <summary>
        /// 当前的Application索引
        /// </summary>
        private static int? m_nowApplicationIndex = null;

        /// <summary>
        /// 当前使用的临时目录对象
        /// </summary>
        private static DirectoryInfo nowUseTempDirectory = null;

        /// <summary>
        /// 当前使用的父级目录对象
        /// </summary>
        private static DirectoryInfo nowUseSourceDirectory = null;

        /// <summary>
        /// 当前使用的父级拷贝根目录对象
        /// </summary>
        private static DirectoryInfo nowUseSourceRootDirectory = null;

        /// <summary>
        /// 当前使用的Assembly临时目录对象
        /// </summary>
        private static DirectoryInfo nowUseAssemblyDirectory = null;

        /// <summary>
        /// 配置文件名称
        /// </summary>
        private const string SETTINGFILE = "Setting.xml";

        /// <summary>
        /// debug模式节点名称
        /// </summary>
        private const string DEBUGMODEL = "DebugModel";

        /// <summary>
        /// 必要文件节点名称
        /// </summary>
        private const string NEEDCOPYFILE = "NeedCopyFile";

        /// <summary>
        /// 保存计数节点名称
        /// </summary>
        private const string SAVELEVEL = "SaveParentLevel"; 
        #endregion

        /// <summary>
        /// 底层是否是Debug模式
        /// </summary>
        internal static bool IfDebugModel
        {
            get
            {
                return m_ifDebugModel;
            }
        }

        /// <summary>
        /// 静态构造
        /// </summary>
        static DEBUGUtility()
        {
            //获取配置文件信息
            GetSettingInfo(out m_ifDebugModel, out m_saveParentLevel, out m_useMulCopyRegex);

            if (m_ifDebugModel)
            {
                try
                {
                    //清除上次的临时目录
                    RemoveLastTemp();
                }
                catch (Exception)
                {
                    ;
                }
                
            }
        }

        /// <summary>
        ///创建Guid
        /// </summary>
        internal static void CreatGuid()
        {
            if (IfDebugModel)
            {
                //创建
                m_nowGuid = Guid.NewGuid();
            }
        }

        /// <summary>
        /// 清除Guid
        /// </summary>
        internal static void DropGuid()
        {
            if (IfDebugModel)
            {
                if(!string.IsNullOrWhiteSpace(m_nowUseAddinId))
                {
                    //备份已使用的Guid
                    m_usedAddinIdAndGuidDic[m_nowUseAddinId] = m_nowGuid;
                }
                m_nowGuid = null;
            }
        }

        /// <summary>
        /// 重置状态
        /// </summary>
        internal static void ResetCondition()
        {
            m_bHasMulCopy = false;
            nowUseTempDirectory = null;
            nowUseSourceDirectory = null;
            nowUseSourceRootDirectory = null;
            nowUseAssemblyDirectory = null;
            m_nowApplicationIndex = null;
        }

        /// <summary>
        /// 设置AddInId
        /// </summary>
        /// <param name="inputAddInId"></param>
        internal static void SetAddInId(string inputAddInId)
        {
            //非debug模式返回
            if (!IfDebugModel)
            {
                return;
            }
            m_nowUseAddinId = inputAddInId;

            //上次Guid缓存
            Guid? tempLastUseGuid = null;
            m_usedAddinIdAndGuidDic.TryGetValue(inputAddInId, out tempLastUseGuid);

            //判断是否需要重用
            if (null != tempLastUseGuid && !string.IsNullOrWhiteSpace(m_nowUseAddinId) 
                && !string.IsNullOrWhiteSpace(m_nowUseAddinId) 
                &&
                DialogResult.OK == MessageBox.Show(Properties.Resources.str_debug_message,string.Empty, MessageBoxButtons.OKCancel) )
            {
                //设为上次的Guid
                m_nowGuid = tempLastUseGuid;
                //不需多重复制
                m_bHasMulCopy = true;
            }
         
        }

        /// <summary>
        /// 根据Guid创建临时目录并转换路径字符串
        /// </summary>
        /// <param name="useGuid"></param>
        /// <param name="saveParentLevel"></param>
        /// <param name="inputPath"></param>
        /// <returns></returns>
        internal static string CopyFileAndChangePath(string inputPath)
        {
            //无需创建
            if (null == m_nowGuid)
            {
                return inputPath;
            }


            //参数保护
            int saveParentLevel = Math.Abs(m_saveParentLevel);

            string str_useFilePath;
            DirectoryInfo parentDirectory;

            inputPath = AdjustPath(inputPath, out str_useFilePath, out parentDirectory);


            //获得/创建的临时目录
            var createdDirectory = null != nowUseTempDirectory ? nowUseTempDirectory : CreatTempAppDirectory(m_nowGuid);
            //回写
            nowUseTempDirectory = createdDirectory;

            //使用的变更字符串
            string useAppendPath = string.Empty;

            FindRootLocation(ref saveParentLevel, ref parentDirectory, ref useAppendPath);

            //创建临时路径
            var realuseDirectionInfo = new DirectoryInfo(createdDirectory.FullName + useAppendPath);
            realuseDirectionInfo.Create();

            //回写临时程序集路径
            if (null == nowUseAssemblyDirectory)
            {
                nowUseAssemblyDirectory = realuseDirectionInfo;
            }

            //实际使用的文件路径
            var realuseFileInfo = new FileInfo(realuseDirectionInfo.FullName + str_useFilePath);

            //若不存在
            if (!realuseFileInfo.Exists)
            {
                File.Copy(inputPath, realuseFileInfo.FullName);

                //若还没有进行多重拷贝并
                if (!m_bHasMulCopy && 0 != m_useMulCopyRegex.Count)
                {
                    MultipleCopy(parentDirectory, createdDirectory, m_useMulCopyRegex);

                    //设置已进行多重拷贝状态
                    m_bHasMulCopy = true;

                }
            }

            return realuseFileInfo.FullName;

        }

        /// <summary>
        /// 变更或添加索引记录
        /// </summary>
        /// <param name="inputIndex"></param>
        /// <param name="inputPath"></param>
        internal static void SetApplicaionIndex(int inputIndex,string inputPath = null)
        {
            m_nowApplicationIndex = inputIndex;

            if (m_useApplicationIdAndGuidDic.ContainsKey(inputIndex))
            {
                m_nowGuid = m_useApplicationIdAndGuidDic[inputIndex];
            }
            else
            {
                m_useApplicationIdAndGuidDic.Add(inputIndex, m_nowGuid);
            }

            //添加到记录
            if (!string.IsNullOrWhiteSpace(inputPath) && !m_useApplicationIdAndPathDic.ContainsKey(inputIndex))
            {
                m_useApplicationIdAndPathDic.Add(inputIndex, inputPath);
            }

            //获得索引对应的路径
            var usePath = m_useApplicationIdAndPathDic[inputIndex];

            var useFileInfo = new FileInfo(usePath);

            //当Debug模式 且文件名是非Debug文件名时
            if ( null != m_nowGuid && !string.IsNullOrWhiteSpace(usePath) && m_NoneDebugNameSet.Contains(useFileInfo.Name))
            {
                //临时关闭当前Guid 临时关闭Debug模式功能
                m_nowGuid = null;
            }

            //若Application全不进行Debug模式
            if (!m_bIfDebugApplication)
            {
                //临时关闭Debug功能
                m_nowGuid = null;
            }
        }

        /// <summary>
        /// 重设应用位置
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        internal static string ResetApplicationLocation(string inputString)
        {
            //非Debug模式 或 没有当前Application索引 不处理
            if (!m_ifDebugModel || null == m_nowApplicationIndex 
                || !m_useApplicationIdAndPathDic.ContainsKey(m_nowApplicationIndex.Value))
            {
                return inputString;
            }
            else
            {
                var returnValue = m_useApplicationIdAndPathDic[m_nowApplicationIndex.Value];
                m_nowApplicationIndex = null;
                return returnValue;
            }
        }

        #region 私有方法
        /// <summary>
        /// 寻找使用的根位置
        /// </summary>
        /// <param name="saveParentLevel"></param>
        /// <param name="parentDirectory"></param>
        /// <param name="useAppendPath"></param>
        private static void FindRootLocation(ref int saveParentLevel, ref DirectoryInfo parentDirectory, ref string useAppendPath)
        {
            if (null == nowUseSourceRootDirectory)
            {
                //获取要求的父级目录
                while (0 < saveParentLevel && null != parentDirectory.Parent)
                {
                    useAppendPath = @"\" + parentDirectory.Name + useAppendPath;
                    parentDirectory = parentDirectory.Parent;

                    saveParentLevel--;
                }

                //获得父级目录
                nowUseSourceRootDirectory = parentDirectory;
            }
            else
            {
                //向上寻找
                while (!nowUseSourceRootDirectory.FullName.Equals(parentDirectory.FullName))
                {
                    useAppendPath = @"\" + parentDirectory.Name + useAppendPath;
                    parentDirectory = parentDirectory.Parent;
                }
            }

        }

        /// <summary>
        /// 调整目录
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="str_useFilePath"></param>
        /// <param name="parentDirectory"></param>
        /// <returns></returns>
        private static string AdjustPath(string inputPath, out string str_useFilePath, out DirectoryInfo parentDirectory)
        {
            //输入的文件信息
            var inputFileInfo = new FileInfo(inputPath);

            //输入的文件信息父菜单
            parentDirectory = null != nowUseSourceDirectory ? nowUseSourceDirectory : inputFileInfo.Directory;
            //使用的文件名字符串
            str_useFilePath = @"\" + inputFileInfo.Name;

            //若已有使用源Assembly目录
            if (null != nowUseAssemblyDirectory)
            {
                str_useFilePath = inputFileInfo.FullName.Replace(nowUseAssemblyDirectory.FullName, string.Empty);
            }

            //回写
            nowUseSourceDirectory = parentDirectory;
            //调整目录
            inputFileInfo = new FileInfo(nowUseSourceDirectory.FullName + str_useFilePath);
            inputPath = inputFileInfo.FullName;
            return inputPath;
        }

        /// <summary>
        /// 多重拷贝递归
        /// </summary>
        /// <param name="inputSourceInfo"></param>
        /// <param name="inputTargetInfo"></param>
        /// <param name="useRegex"></param>
        private static void MultipleCopy(DirectoryInfo inputSourceInfo, DirectoryInfo inputTargetInfo, List<Regex> useRegex)
        {
            //文件拷贝
            foreach (var oneFileInfo in inputSourceInfo.GetFiles())
            {
                //循环匹配规则
                foreach (var oneRegex in useRegex)
                {
                    //若匹配
                    if (oneRegex.IsMatch(oneFileInfo.FullName))
                    {
                        var tempFileInfo = new FileInfo(inputTargetInfo.FullName + @"\" + oneFileInfo.Name);

                        //不存在则拷贝
                        if (!tempFileInfo.Exists)
                        {
                            File.Copy(oneFileInfo.FullName, tempFileInfo.FullName);
                        }
                    }
                }
            }

            //循环源目录的子目录
            foreach (var oneSourceInfo in inputSourceInfo.GetDirectories())
            {
                //创建对用的目录
                var tempDirectioryInfo = new DirectoryInfo(inputTargetInfo.FullName + @"\" + oneSourceInfo.Name);
                tempDirectioryInfo.Create();
                //深度递归
                MultipleCopy(oneSourceInfo, tempDirectioryInfo, useRegex);
            }


        }

        /// <summary>
        /// 根据Guid创建一个临时目录
        /// </summary>
        /// <param name="useGuid"></param>
        /// <returns></returns>
        private static DirectoryInfo CreatTempAppDirectory(Guid? useGuid)
        {

            string nowPath = Assembly.GetExecutingAssembly().Location;
            var dispatcherFile = new FileInfo(nowPath);

            //创建应用目录
            var createdDirectory = Directory.CreateDirectory(dispatcherFile.Directory.ToString() + @"\" + useGuid.Value);

            return createdDirectory;
        }

        /// <summary>
        /// 删除上次的临时文件
        /// </summary>
        private static void RemoveLastTemp()
        {
            //获得当前程序集的文件
            FileInfo nowAssemblyFileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);

            var nowDirection = nowAssemblyFileInfo.Directory;

            //删除所有子级目录
            foreach (var oneDirectories in nowDirection.GetDirectories())
            {
                oneDirectories.Delete(true);
            }
        }

        /// <summary>
        /// 获取配置文件信息
        /// </summary>
        /// <param name="bIfDebug">是否时debug模式</param>
        /// <param name="lstNeedCopyFile">需拷贝的文件列表</param>
        private static void GetSettingInfo(out bool bIfDebug, out int nSaveLevel, out List<Regex> lstNeedCopyFile)
        {
            bIfDebug = false;
            nSaveLevel = 0;
            lstNeedCopyFile = new List<Regex>();
            //获取配置文件目录
            string strPath = Assembly.GetExecutingAssembly().Location;
            string strUsePath = new FileInfo(strPath).Directory + @"\" + SETTINGFILE;

            //加载配置文件
            if (File.Exists(strUsePath))
            {
                try
                {
                    XmlDocument settingDoc = new XmlDocument();
                    settingDoc.Load(strUsePath);

                    XmlElement root = settingDoc.DocumentElement;

                    foreach (XmlNode eachNode in root)
                    {
                        //获取debug模式
                        if (eachNode.Name.Equals(DEBUGMODEL))
                        {
                            bool.TryParse(eachNode.InnerText, out bIfDebug);
                        }
                        //获取保存级数
                        else if (eachNode.Name.Equals(SAVELEVEL))
                        {
                            int.TryParse(eachNode.InnerText, out nSaveLevel);
                        }
                        //获取必要文件
                        else if (eachNode.Name.Equals(NEEDCOPYFILE))
                        {
                            Regex tempRegex = new Regex(eachNode.InnerText);
                            lstNeedCopyFile.Add(tempRegex);
                        }
                    }
                }
                catch (Exception)
                {
                    bIfDebug = false;
                    nSaveLevel = 0;
                    lstNeedCopyFile.Clear();
                }

            }
        } 
        #endregion
    }
}
