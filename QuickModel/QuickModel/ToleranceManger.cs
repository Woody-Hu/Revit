using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel
{
    /// <summary>
    /// 容差管理器 单例模式
    /// </summary>
    public class ToleranceManger
    {
        /// <summary>
        /// 默认容差
        /// </summary>
        private const double m_default = 1e-9;

        /// <summary>
        /// 使用的缓存字典
        /// </summary>
        private Dictionary<string, double> m_useDic = new Dictionary<string, double>();

        /// <summary>
        /// 私有构造从文件中加载
        /// </summary>
        private ToleranceManger()
        {
            LoadFromFile();
        }

        /// <summary>
        /// 析构保存到文件
        /// </summary>
        ~ToleranceManger()
        {
            SaveToFile();
        }

        /// <summary>
        /// 使用的单例模式标签
        /// </summary>
        private static ToleranceManger m_useSingleTon = null;

        /// <summary>
        /// 使用的单例模式管理器
        /// </summary>
        /// <returns></returns>
        public static ToleranceManger GetToleranceManger()
        {
            if (null ==m_useSingleTon )
            {
                m_useSingleTon = new ToleranceManger();
            }

            return m_useSingleTon;
        }

        /// <summary>
        /// 获取使用容差
        /// </summary>
        /// <param name="inputName">使用的容差名称</param>
        /// <returns></returns>
        public double GetTolerance(string inputName = null)
        {
            if (string.IsNullOrWhiteSpace(inputName))
            {
                return m_default;
            }

            if (!m_useDic.ContainsKey(inputName))
            {
                m_useDic.Add(inputName, m_default);
            }

            return m_useDic[inputName];
        }

        /// <summary>
        /// 调整使用的特征的容差
        /// </summary>
        /// <param name="inputName"></param>
        /// <param name="inputValue"></param>
        public void SetTolerance(string inputName,double inputValue)
        {
            if (string.IsNullOrWhiteSpace(inputName))
            {
                return;
            }

            if (!m_useDic.ContainsKey(inputName))
            {
                m_useDic.Add(inputName, inputValue);
                return;
            }
            m_useDic[inputName] = inputValue;
        }

        /// <summary>
        /// 从文件中读取
        /// </summary>
        private void LoadFromFile()
        {
            //待实现
            throw new NotImplementedException();
        }

        /// <summary>
        /// 保存到文件
        /// </summary>
        private void SaveToFile()
        {
            //待实现
            throw new NotImplementedException();
        }
    }
}
