using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Text.RegularExpressions;

namespace CommandLunacher
{
    /// <summary>
    /// 命令路由类
    /// </summary>
    internal class CoreDispatcher : ICoreDisparcher
    {
        #region 静态私有字段
        /// <summary>
        /// 命令映射字典
        /// </summary>
        private static Dictionary<string, HandlerInfoPacker> m_useCMDDic = new Dictionary<string, HandlerInfoPacker>();

        /// <summary>
        /// 命令类字典
        /// </summary>
        private static Dictionary<string, IExternalCommand> m_useCommandObject = new Dictionary<string, IExternalCommand>();

        /// <summary>
        /// 启动时执行信息封装
        /// </summary>
        private static List<HandlerInfoPacker> m_useLstAppInfo = new List<HandlerInfoPacker>();

        /// <summary>
        /// 启动时执行类的封装
        /// </summary>
        private static List<IExternalApplication> m_useLstApp = new List<IExternalApplication>(); 
        #endregion

        #region 常量字符串
        /// <summary>
        /// 程序集路径
        /// </summary>
        private const string ASSEMBLEPATH = "AssemblePath";

        /// <summary>
        /// 命令类全名称
        /// </summary>
        private const string CLASSFULLNAME = "ClassFullName";


        /// <summary>
        /// APP配置文件名称
        /// </summary>
        private const string APPXMLFILENAME = @"App_.+\.xml";

        /// <summary>
        /// CMD配置文件名称
        /// </summary>
        private const string CMDXMLFILENAME = @"Com_.+\.xml";

        /// <summary>
        /// 是否删除xml文件属性名称
        /// </summary>
        private const string IFDELETEFILE = "ifDelete";

        /// <summary>
        /// 要删除的command或application配置文件
        /// </summary>
        private List<string> m_lstDeleteFiles = new List<string>();
        #endregion

        public int ApplicationCount()
        {
            return m_useLstAppInfo.Count;
        }

        /// <summary>
        /// 准备初始数据
        /// </summary>
        public void PrepareData()
        {
            var tempFile = new FileInfo(Assembly.GetExecutingAssembly().Location);

            List<HandlerInfoPacker> tempLstCommandInfo = new List<HandlerInfoPacker>();

            //获取配置文件
            foreach (var file in tempFile.Directory.GetFiles().ToList())
            {
                if (Regex.IsMatch(file.Name, CMDXMLFILENAME))
                {
                    tempLstCommandInfo.AddRange(PrepareData(file.FullName));
                }
                else if (Regex.IsMatch(file.FullName, APPXMLFILENAME))
                {
                    //准备App
                    m_useLstAppInfo.AddRange(PrepareData(file.FullName));
                }
            }

            //准备命令字典
            m_useCMDDic = tempLstCommandInfo.ToDictionary(k => k.StrUseAddinId.ToLower(), k => k);

            //删除command或application配置文件 
            foreach (var eachFile in m_lstDeleteFiles)
            {
                //防止被占用的错误
                try
                {
                    File.Delete(eachFile);
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// 准备一个应用封装
        /// </summary>
        /// <param name="inputIndex"></param>
        public void PrepareOneApplication(int inputIndex)
        {
            //设置位置
            DEBUGUtility.SetApplicaionIndex(inputIndex, m_useLstAppInfo[inputIndex].StrUseAssemblePath);
            m_useLstApp.Add(CreatObjByHandlerInfo(m_useLstAppInfo[inputIndex]) as IExternalApplication);
        }

        /// <summary>
        /// 启动一个应用封装
        /// </summary>
        /// <param name="inputIndex"></param>
        /// <param name="inputApplication"></param>
        public void StartUpOneApplication(int inputIndex, UIControlledApplication inputApplication)
        {
            //设置位置
            DEBUGUtility.SetApplicaionIndex(inputIndex);
            m_useLstApp[inputIndex].OnStartup(inputApplication);
        }

        /// <summary>
        /// 关闭一个应用封装
        /// </summary>
        /// <param name="inputIndex"></param>
        /// <param name="inputApplication"></param>
        public void ShutDownOneApplication(int inputIndex, UIControlledApplication inputApplication)
        {
            //设置位置
            DEBUGUtility.SetApplicaionIndex(inputIndex);   
            m_useLstApp[inputIndex].OnShutdown(inputApplication);
        }

        /// <summary>
        /// 命令主入口
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            return InvokeHandler(commandData, ref message, elements);
        }

        /// <summary>
        /// 启动-顺序启动
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public Result OnStartup(UIControlledApplication application)
        {
            //获取版本号
            AssemblyLoadUtility.m_versionNum = application.ControlledApplication.VersionNumber;

            return Result.Succeeded;

        }

        /// <summary>
        /// 关闭-逆序关闭
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public Result OnShutdown(UIControlledApplication application)
        {

            //清空版本信息
            AssemblyLoadUtility.m_versionNum = null;

            return Result.Succeeded;
        }

        #region 私有方法组
        /// <summary>
        /// 准备类封装列表
        /// </summary>
        /// <param name="useFileName"></param>
        /// <returns></returns>
        private List<HandlerInfoPacker> PrepareData(string useFileName)
        {
            List<HandlerInfoPacker> lstTempPacker = new List<HandlerInfoPacker>();

            //加载配置文件
            XmlDocument useXML = new XmlDocument();
            useXML.Load(useFileName);

            //添加要删除文件
            XmlNode root = useXML.DocumentElement;
            bool bIfDelete = false;
            bool.TryParse(root.Attributes[IFDELETEFILE].Value, out bIfDelete);
            if (bIfDelete)
            {
                m_lstDeleteFiles.Add(useFileName);
            }
          
            //收集处理器信息
            foreach (XmlNode eachId in root.ChildNodes)
            {
                HandlerInfoPacker cmdPacker = new HandlerInfoPacker();
                cmdPacker.StrUseAddinId = eachId.InnerText;
                cmdPacker.StrUseAssemblePath = eachId.Attributes[ASSEMBLEPATH].Value;
                cmdPacker.StrUseClassFullName = eachId.Attributes[CLASSFULLNAME].Value;
                lstTempPacker.Add(cmdPacker);
            }
            return lstTempPacker;
        }

        /// <summary>
        /// 调度处理器
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        private Result InvokeHandler(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //获取Id
            var useId = commandData.Application.ActiveAddInId.GetGUID();

            //实际处理器
            IExternalCommand tempCommand = null;

            //设置addinId
            DEBUGUtility.SetAddInId(useId.ToString());

            //缓存判断
            if (!m_useCommandObject.ContainsKey(useId.ToString()))
            {
                //获取命令对象
                var useCommandPacker = m_useCMDDic[useId.ToString()];

                object tempCommandObj = CreatObjByHandlerInfo(useCommandPacker);

                //多态转换
                tempCommand = tempCommandObj as IExternalCommand;

                m_useCommandObject.Add(useId.ToString(), tempCommand);
            }
            else
            {
                tempCommand = m_useCommandObject[useId.ToString()];
            }

            //若是debug模式不单例命令对象
            if (DEBUGUtility.IfDebugModel)
            {
                //清除已缓存命令对象
                m_useCommandObject.Remove(useId.ToString());
            }

            return tempCommand.Execute(commandData, ref message, elements);

        }

        /// <summary>
        /// 利用处理器信息封装制造对象
        /// </summary>
        /// <param name="useCommandPacker"></param>
        /// <returns></returns>
        private object CreatObjByHandlerInfo(HandlerInfoPacker useCommandPacker)
        {
            //加载目标程序集
            var loadedAssembly = AssemblyLoadUtility.LoadAssembly(useCommandPacker.StrUseAssemblePath);

            //获取目标类
            var useType = loadedAssembly.GetType(useCommandPacker.StrUseClassFullName);

            //调用无参方法创建对象
            var tempCommandObj = Activator.CreateInstance(useType);
            return tempCommandObj;
        }
        #endregion
    }
}
