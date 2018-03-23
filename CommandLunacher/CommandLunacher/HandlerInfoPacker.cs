using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLunacher
{
    /// <summary>
    /// 处理器信息封装
    /// </summary>
    class HandlerInfoPacker
    {
        /// <summary>
        /// 对应的AddinId
        /// </summary>
        private string m_strUseAddinId;

        /// <summary>
        /// 对应的程序集路径
        /// </summary>
        private string m_strUseAssemblePath;

        /// <summary>
        /// 对应的类全名称
        /// </summary>
        private string m_strUseClassFullName;

        /// <summary>
        /// 对应的AddinId
        /// </summary>
        public string StrUseAddinId
        {
            get
            {
                return m_strUseAddinId;
            }

            set
            {
                m_strUseAddinId = value;
            }
        }

        /// <summary>
        /// 对应的程序集路径
        /// </summary>
        public string StrUseAssemblePath
        {
            get
            {
                return m_strUseAssemblePath;
            }

            set
            {
                m_strUseAssemblePath = value;
            }
        }

        /// <summary>
        /// 对应的类全名称
        /// </summary>
        public string StrUseClassFullName
        {
            get
            {
                return m_strUseClassFullName;
            }

            set
            {
                m_strUseClassFullName = value;
            }
        }
    }
}
