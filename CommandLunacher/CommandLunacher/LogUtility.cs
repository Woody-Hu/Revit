using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommandLunacher
{
    /// <summary>
    /// 日志模块
    /// </summary>
    internal static class LogUtility
    {
        /// <summary>
        /// 使用的StringBuilder
        /// </summary>
        private static StringBuilder m_useStringBuilder = null;

        /// <summary>
        /// 添加一条日志
        /// </summary>
        /// <param name="inputex"></param>
        internal static void AppendLog(Exception inputex)
        {
            if (null != inputex.InnerException)
            {
                AppendLog(inputex.InnerException);
            }

            AppendLog(inputex.Message, true);
            AppendLog(inputex.StackTrace, false); 
        }

        /// <summary>
        /// 添加一条日志
        /// </summary>
        /// <param name="inputString">添加的字符串</param>
        /// <param name="ifHasTime">是否添加时间</param>
        internal static void AppendLog(string inputString,bool ifHasTime)
        {
            if (null == m_useStringBuilder)
            {
                m_useStringBuilder = new StringBuilder();
            }

            if (ifHasTime)
            {
                m_useStringBuilder.AppendLine(DateTime.Now.ToString());
            }
            m_useStringBuilder.AppendLine(inputString);
        }

        /// <summary>
        /// 生成日志文件
        /// </summary>
        internal static void CreatLogFile()
        {
            if (null == m_useStringBuilder)
            {
                return;
            }

            try
            {
                string tempPath = Assembly.GetExecutingAssembly().Location;

                var tempFileInfo = new FileInfo(tempPath);
                tempPath = tempFileInfo.Directory.FullName + @"\" + DateTime.Now.ToString("yyyyMMddHHmmssms") +".txt";

                using (StreamWriter sw = new StreamWriter(tempPath))
                {
                    sw.WriteLine(m_useStringBuilder.ToString());
                }
                m_useStringBuilder = null;
            }
            catch (Exception)
            {
                ;
            }

           

        }
    }
}
