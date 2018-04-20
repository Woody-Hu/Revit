using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RibbonItemEmitService
{
    /// <summary>
    /// 实例封装
    /// </summary>
    internal class InstancePacker
    {
        #region 私有字段
        /// <summary>
        /// 实例对象
        /// </summary>
        private object m_useObj;

        /// <summary>
        /// 类型
        /// </summary>
        private Type m_useThisType;

        /// <summary>
        /// 使用的位置字段
        /// </summary>
        private FieldInfo m_useLocationField;

        /// <summary>
        /// 使用的位置值
        /// </summary>
        private string m_strUseLocationValue = null;

        /// <summary>
        /// 使用的全名称字段
        /// </summary>
        private FieldInfo m_useFullNameField;

        /// <summary>
        /// 使用的全名称字段
        /// </summary>
        private string m_strUseFullNameValue = null; 
        #endregion

        /// <summary>
        /// 类型
        /// </summary>
        internal Type UseThisType
        {
            get
            {
                return m_useThisType;
            }

            private set
            {
                m_useThisType = value;
            }
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="inputInstance"></param>
        internal InstancePacker(object inputInstance)
        {
            m_useObj = inputInstance;
            UseThisType = m_useObj.GetType();
            //获取使用全名称字段
            m_useLocationField = UseThisType.GetField(MakeTypReuestUtility.UseLocation);
            //获取使用位置字段
            m_useFullNameField = UseThisType.GetField(MakeTypReuestUtility.FullClassName);
        }

        /// <summary>
        /// 是否可用
        /// </summary>
        /// <returns></returns>
        internal bool IFCanUse()
        {
            return null != m_useLocationField && null != m_useFullNameField;
        }

        /// <summary>
        /// 实际使用对象的位置
        /// </summary>
        internal string UseLocation
        {
            get
            {
                if (null == m_strUseLocationValue)
                {
                    m_strUseLocationValue = (string) m_useLocationField.GetValue(m_useObj);
                }
                return m_strUseLocationValue;
            }
        }

        /// <summary>
        /// 实际使用对象的全名称
        /// </summary>
        internal string UseFullName
        {
            get
            {
                if (null == m_strUseFullNameValue)
                {
                    m_strUseFullNameValue = (string)m_useFullNameField.GetValue(m_useObj);
                }
                return m_strUseFullNameValue;
            }
        }


    }
}
