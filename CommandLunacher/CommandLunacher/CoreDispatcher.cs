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
    [Autodesk.Revit.Attributes.TransactionAttribute(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    /// <summary>
    /// 命令路由类
    /// </summary>
    internal class CoreDispatcher : ICoreDisparcher
    {
        /// <summary>
        /// 命令映射字典
        /// </summary>
        private static Dictionary<string, HandlerInfoPacker> m_useCMDDic = new Dictionary<string, HandlerInfoPacker>();

        /// <summary>
        /// 启动时执行信息封装
        /// </summary>
        private static List<HandlerInfoPacker> m_useLstAppInfo = new List<HandlerInfoPacker>();

        /// <summary>
        /// 启动时执行类的封装
        /// </summary>
        private static List<IExternalApplication> m_useLstApp = new List<IExternalApplication>();

        /// <summary>
        /// 执行方法的方法名
        /// </summary>
        private const string m_useExecuteMethodName = "Execute";

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
        /// 准备初始数据
        /// </summary>
        public void PrepareData()
        {

            var tempFile = new FileInfo(Assembly.GetExecutingAssembly().Location);

            List<HandlerInfoPacker> tempLstCommandInfo = new List<HandlerInfoPacker>();

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

            //创建启动类
            foreach (var oneAppInfo in m_useLstAppInfo)
            {
                m_useLstApp.Add(CreatObjByHandlerInfo(oneAppInfo) as IExternalApplication);
            }

        }

        /// <summary>
        /// 准备类封装列表
        /// </summary>
        /// <param name="useFileName"></param>
        /// <returns></returns>
        private List<HandlerInfoPacker> PrepareData(string useFileName)
        {
            List<HandlerInfoPacker> lstTempPacker = new List<CommandLunacher.HandlerInfoPacker>();

            XmlDocument useXML = new XmlDocument();
            useXML.Load(useFileName);

            foreach (XmlNode eachId in useXML.DocumentElement.ChildNodes)
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

            //获取命令对象
            var useCommandPacker = m_useCMDDic[useId.ToString()];

            object tempCommandObj = CreatObjByHandlerInfo(useCommandPacker);

            //多态转换
            var tempCommand = tempCommandObj as IExternalCommand;

            return tempCommand.Execute(commandData, ref message, elements);
        }

        /// <summary>
        /// 利用处理器信息封装制造对象
        /// </summary>
        /// <param name="useCommandPacker"></param>
        /// <returns></returns>
        private static object CreatObjByHandlerInfo(HandlerInfoPacker useCommandPacker)
        {
            //加载目标程序集
            var loadedAssembly = AssemblyLoadUtility.LoadAssembly(useCommandPacker.StrUseAssemblePath);

            //获取目标类
            var useType = loadedAssembly.GetType(useCommandPacker.StrUseClassFullName);

            //调用无参方法创建对象
            var tempCommandObj = Activator.CreateInstance(useType);
            return tempCommandObj;
        }


        /// <summary>
        /// 启动-顺序启动
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public Result OnStartup(UIControlledApplication application)
        {
            //顺序启动
            foreach (var oneApp in m_useLstApp)
            {
                oneApp.OnStartup(application);
            }

            return Result.Succeeded;

        }

        /// <summary>
        /// 关闭-逆序关闭
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public Result OnShutdown(UIControlledApplication application)
        {

            for (int useIndex = m_useLstApp.Count - 1; useIndex >= 0; useIndex--)
            {
                m_useLstApp[useIndex].OnShutdown(application);
            }

            return Result.Succeeded;
        }
    }
}
