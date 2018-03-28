using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLunacher
{
    /// <summary>
    /// 派发器接口
    /// </summary>
    internal interface ICoreDisparcher:IRevitFunction
    {
        /// <summary>
        /// 数据准备
        /// </summary>
        void PrepareData();

        /// <summary>
        ///获取Application的数量
        /// </summary>
        /// <returns></returns>
        int ApplicationCount();

        /// <summary>
        /// 准备一个应用
        /// </summary>
        /// <param name="inputIndex"></param>
        /// <returns></returns>
        void PrepareOneApplication(int inputIndex);

        /// <summary>
        /// 启动一个应用
        /// </summary>
        /// <param name="inputIndex"></param>
        /// <param name="inputApplication"></param>
        void StartUpOneApplication(int inputIndex, UIControlledApplication inputApplication);

        /// <summary>
        /// 关闭一个应用
        /// </summary>
        /// <param name="inputIndex"></param>
        /// <param name="inputApplication"></param>
        void ShutDownOneApplication(int inputIndex, UIControlledApplication inputApplication);
    }
}
