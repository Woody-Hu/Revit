﻿using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel
{
    /// <summary>
    /// 翻模命令通用框架
    /// </summary>
    public class QuickModelFrameWork : IQuickModel
    {
        #region 私有字段
        /// <summary>
        /// 使用的请求制作处理器
        /// </summary>
        private IRequestMaker m_useRequestMaker;

        /// <summary>
        /// 分组器类型列表
        /// </summary>
        private List<Type> m_lstGrouperType;

        /// <summary>
        /// 重建器类型列表
        /// </summary>
        private List<Type> m_lstBuilderType;

        /// <summary>
        /// 使用的响应处理器
        /// </summary>
        private IResponseHanlder m_useResponseHanlder;

        /// <summary>
        /// 分组器名称 - 分组器实例映射
        /// </summary>
        private Dictionary<string, IDataGrouper> m_dicUSeGropuerMap = new Dictionary<string, IDataGrouper>();

        /// <summary>
        /// 重建器名称 - 重建器实例映射
        /// </summary>
        private Dictionary<string, IRevitModelRebuilder> m_dicUseRebuilderMap = new Dictionary<string, IRevitModelRebuilder>(); 
        #endregion

        /// <summary>
        /// 构造翻模框架
        /// </summary>
        /// <param name="useRequestMaker">使用UI层请求制作接口</param>
        /// <param name="useResponseHanlder">使用的响应处理器</param>
        public QuickModelFrameWork(IRequestMaker useRequestMaker,IResponseHanlder useResponseHanlder)
        {
            m_useRequestMaker = useRequestMaker;
            m_lstGrouperType = GetTypesInAssembly<IDataGrouper>();
            m_lstBuilderType = GetTypesInAssembly<IRevitModelRebuilder>();
            m_useResponseHanlder = useResponseHanlder;


            PrepareMap<IDataGrouper, GrouperAttribute>(m_lstGrouperType, m_dicUSeGropuerMap);
            PrepareMap<IRevitModelRebuilder, RebuilderAttribute>(m_lstBuilderType, m_dicUseRebuilderMap);
        }

        /// <summary>
        /// 翻模执行
        /// </summary>
        /// <param name="inputCommandData"></param>
        public void Excute(Autodesk.Revit.UI.ExternalCommandData inputCommandData)
        {
            Document useDoc = inputCommandData.Application.ActiveUIDocument.Document;

            WorkHanlder(inputCommandData, useDoc);

            return;
        }

        /// <summary>
        /// 循环翻模执行
        /// </summary>
        /// <param name="inputCommandData"></param>
        public void ExcuteWithWhile(Autodesk.Revit.UI.ExternalCommandData inputCommandData)
        {
            Document useDoc = inputCommandData.Application.ActiveUIDocument.Document;

            while (true)
            {
                try
                {
                    WorkHanlder(inputCommandData, useDoc);
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException )
                {
                    //若需中断
                    if (m_useRequestMaker.IfBreak)
                    {
                        break;
                    }
                }
            }

            return;
        }

        /// <summary>
        /// 处理方法
        /// </summary>
        /// <param name="inputCommandData"></param>
        /// <param name="useDoc"></param>
        private void WorkHanlder(Autodesk.Revit.UI.ExternalCommandData inputCommandData, Document useDoc)
        {
            //Ui层事务组
            using (TransactionGroup useGroup = new TransactionGroup(useDoc, "uiGroup"))
            {
                try
                {
                    useGroup.Start();
                    //利用UI准备请求
                    m_useRequestMaker.PrepareRequest();
                    useGroup.Assimilate();
                }
                catch (Exception ex)
                {
                    useGroup.RollBack();
                    throw ex;
                }
            }

            //若存在响应处理器
            if (null != m_useResponseHanlder)
            {
                m_useResponseHanlder.AddStartTime(DateTime.Now);
            }

            //获得请求封装
            var lstInputRequest = m_useRequestMaker.GetAllInputRequest();

            List<RevitModelRequest> lstRevitModelRequest = new List<RevitModelRequest>();

            Group(inputCommandData, lstInputRequest, lstRevitModelRequest);

            //响应列表
            List<RevitModelRebuildResponse> lstUseResponse = new List<RevitModelRebuildResponse>();

            ReBuild(useDoc, lstRevitModelRequest, lstUseResponse);

            //若存在响应处理器
            if (null != m_useResponseHanlder)
            {
                m_useResponseHanlder.AddEndTime(DateTime.Now);
                //注册响应
                foreach (var oneResponse in lstUseResponse)
                {
                    m_useResponseHanlder.AddOneResponse(oneResponse);
                }

                //处理响应
                m_useResponseHanlder.HanlderResponse();
            }
        }

        /// <summary>
        /// 获取当前程序集中指定泛型的派生类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private List<Type> GetTypesInAssembly<T>()
        {
            //获取当前程序集
            Assembly useAssembly = Assembly.GetExecutingAssembly();
            Type baseType = typeof(T);

            var types = from n in useAssembly.GetTypes() where baseType.IsAssignableFrom(n) select n;

            return types.ToList();
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="inputCommandData"></param>
        /// <param name="lstInputRequest"></param>
        /// <param name="lstRevitModelRequest"></param>
        private void Group(Autodesk.Revit.UI.ExternalCommandData inputCommandData, List<InputRequest> lstInputRequest, List<RevitModelRequest> lstRevitModelRequest)
        {
            //循环赋值CommandData
            //组成请求
            //理论可并发
            foreach (var oneRequest in lstInputRequest)
            {
                oneRequest.UseExternalCommandData = inputCommandData;
                var useHanlderName = GetHanlderName<GrouperAttribute>(oneRequest);

                //若没有赋值特征名称或没有注册的处理器则跳过
                if (string.IsNullOrWhiteSpace(useHanlderName) || !m_dicUSeGropuerMap.ContainsKey(useHanlderName))
                {
                    continue;
                }

                //请求转化
                var groupedReuest = m_dicUSeGropuerMap[useHanlderName].GroupData(oneRequest);

                if (null == groupedReuest)
                {
                    continue;
                }

                foreach (var oneGroupedReuest in groupedReuest)
                {
                    //数值还原
                    oneGroupedReuest.UseCadLocationKind = oneRequest.UseCadLocationKind;
                    oneGroupedReuest.UseExternalCommandData = oneRequest.UseExternalCommandData;
                    oneGroupedReuest.UseTypeName = oneRequest.UseTypeName;
                    oneGroupedReuest.UseRevitLocationKind = oneRequest.UseRevitLocationKind;

                    //数值添加
                    lstRevitModelRequest.Add(oneGroupedReuest);
                }

            

            }
        }

        /// <summary>
        /// 重建
        /// </summary>
        /// <param name="useDoc"></param>
        /// <param name="lstRevitModelRequest"></param>
        /// <param name="lstUseResponse"></param>
        private void ReBuild(Document useDoc, List<RevitModelRequest> lstRevitModelRequest, List<RevitModelRebuildResponse> lstUseResponse)
        {
            TransactionGroup useTransactionGroup = new TransactionGroup(useDoc, "quickModel");

            try
            {
                useTransactionGroup.Start();

                foreach (var oneRebuildeRequest in lstRevitModelRequest)
                {
                    RevitModelRebuildResponse tempResponse = new RevitModelRebuildResponse();
                    tempResponse.UseRequest = oneRebuildeRequest;

                    string useHanlderName = GetHanlderName<RebuilderAttribute>(oneRebuildeRequest);

                    if (string.IsNullOrWhiteSpace(useHanlderName) || !m_dicUseRebuilderMap.ContainsKey(useHanlderName))
                    {
                        tempResponse.IfSucess = false;
                    }
                    else
                    {
                        Element createdElement = null;

                        var useHanlder = m_dicUseRebuilderMap[useHanlderName];

                        tempResponse.IfSucess = useHanlder.TryRebuildRevitModel(useDoc, oneRebuildeRequest, out createdElement);
                        tempResponse.CreatedElement = createdElement;
                    }
                    lstUseResponse.Add(tempResponse);
                }
                useTransactionGroup.Assimilate();
            }
            catch (Exception)
            {
                useTransactionGroup.RollBack();

                //响应回置
                foreach (var oneResponse in lstUseResponse)
                {
                    oneResponse.IfSucess = false;
                    oneResponse.CreatedElement = null;
                }
            }
        }

        /// <summary>
        /// 获取对应的处理器名称
        /// </summary>
        /// <typeparam name="X"></typeparam>
        /// <param name="inputObj"></param>
        /// <returns></returns>
        private string GetHanlderName<X>(object inputObj)
            where X:class,IUseHandlerNameGeter
        {
            string returnValue = null;

            Type useType = inputObj.GetType();

            X useAttribute = useType.GetCustomAttribute(typeof(X)) as X;

            if (null == useAttribute)
            {
                return returnValue;
            }


            returnValue = useAttribute.UseHanlderName();

            return returnValue;
        }

        /// <summary>
        /// 准备字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="X"></typeparam>
        /// <param name="lstInputGrouperType"></param>
        /// <param name="useMap"></param>
        private void PrepareMap<T, X>(List<Type> lstInputGrouperType, Dictionary<string, T> useMap)
            where T : class
            where X : IUseHandlerNameGeter
        {
            Type useGrouperType = typeof(T);

            foreach (var oneGrouperType in lstInputGrouperType)
            {
                if (!useGrouperType.IsAssignableFrom(oneGrouperType))
                {
                    continue;
                }

                IUseHandlerNameGeter useGrouperAttribute = oneGrouperType.GetCustomAttribute(typeof(X)) as IUseHandlerNameGeter;

                if (null == useGrouperAttribute)
                {
                    continue;
                }

                string useName = useGrouperAttribute.UseHanlderName();

                if (string.IsNullOrWhiteSpace(useName) || useMap.ContainsKey(useName))
                {
                    continue;
                }

                T useHanlder = null;

                try
                {
                    useHanlder = Activator.CreateInstance(oneGrouperType) as T;

                    useMap.Add(useName, useHanlder);
                }
                catch (Exception)
                {
                    ;
                }
            }
        }
    }
}
