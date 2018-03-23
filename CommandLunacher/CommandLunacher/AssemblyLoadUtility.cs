using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommandLunacher
{
    internal static class AssemblyLoadUtility
    {
        private static Dictionary<string, Assembly> m_dicPathAssembly = new Dictionary<string, Assembly>();

        internal static Assembly LoadAssembly(string inputPath)
        {
            if (!m_dicPathAssembly.ContainsKey(inputPath))
            {
                m_dicPathAssembly.Add(inputPath, Assembly.LoadFile(inputPath));
            }
            return m_dicPathAssembly[inputPath];
        }

        internal static Assembly LoadAssembly(ResolveEventArgs inputEventArgs)
        {
            //获得请求程序集
            var wangtAssemblyName = inputEventArgs.Name.Split(',')[0];
            FileInfo useFileInfo = new FileInfo(inputEventArgs.RequestingAssembly.Location);

            string usePath = useFileInfo.Directory + @"\" + wangtAssemblyName + ".dll";
            //加载
            return LoadAssembly(usePath);
        }


    }
}
