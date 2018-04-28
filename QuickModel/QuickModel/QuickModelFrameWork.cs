using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuickModel
{
    public class QuickModelFrameWork : IQuickModel
    {
        private IRequestMaker m_useRequestMaker;

        private List<Type> m_lstGrouperType;

        private List<Type> m_lstBuilderType;

        private IResponseHanlder m_useResponseHanlder;

        private Dictionary<string, IDataGrouper> m_dicUSeGropuerMap = new Dictionary<string, IDataGrouper>();

        private Dictionary<string, IRevitModelRebuilder> m_dicUseRebuilderMap = new Dictionary<string, IRevitModelRebuilder>();

        public QuickModelFrameWork(IRequestMaker useRequestMaker,IResponseHanlder useResponseHanlder, List<Type> lstInputGrouperType,List<Type> lstInputBuilderType)
        {
            m_useRequestMaker = useRequestMaker;
            m_lstGrouperType = lstInputGrouperType;
            m_lstBuilderType = lstInputBuilderType;
            m_useResponseHanlder = useResponseHanlder;


            Type useRebuilderType = typeof(IRevitModelRebuilder);

            PrepareMap<IDataGrouper, GrouperAttribute>(lstInputGrouperType, m_dicUSeGropuerMap);
            PrepareMap<IRevitModelRebuilder, RebuilderAttribute>(m_lstBuilderType, m_dicUseRebuilderMap);
        }

     

        public void Excute(Autodesk.Revit.UI.ExternalCommandData inputCommandData)
        {
            Document useDoc = inputCommandData.Application.ActiveUIDocument.Document;

            //利用UI准备请求
            m_useRequestMaker.PrepareRequest();

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

            return;
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
            Transaction useTransaction = new Transaction(useDoc, "quickModel");

            try
            {
                useTransaction.Start();

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
                useTransaction.Commit();
            }
            catch (Exception)
            {
                useTransaction.RollBack();

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
