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
        private string m_strUseAddinId;

        private string m_strUseAssemblePath;

        private string m_strUseClassFullName;

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
