using Castle.Facilities.TypedFactory;
using eBPM.Exception;
using ImportLibrary;
using ProjectBase.BusinessDelegate;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ESC5.AppBase
{
    public abstract class BaseMyValueSelector : DefaultDelegateComponentSelector
    {
        protected override Type GetComponentType(MethodInfo method, object[] arguments)
        {
            //工作流
            if ((method.Name == "CreateParameterValueFinder" || method.Name == "CreateProcessorFinder") &&
                arguments.Length == 1 && arguments[0] is string)
            {
                string fullName = (string)arguments[0];
                if (fullName.IndexOf(",") == -1)
                    fullName = fullName + "," + ProjectBase.Utils.ProjectHierarchy.BusinessDelegateNS;
                var t = Type.GetType(fullName);
                if (t == null)
                {
                    if (method.Name == "CreateParameterValueFinder")
                        throw new ParameterValueFinderProgramNotFoundException(fullName);
                    else
                        throw new ProcessorFinderProgramNotFoundException(fullName);
                }
                return t;
            }
            //同步
            else if ((method.Name == "CreateSynchronizeHandler" || method.Name == "CreateReceiveHandler") && arguments.Length == 1 && arguments[0] is string)
            {
                string handlerType = (string)arguments[0];
                if (handlerType.IndexOf(",") == -1)
                    handlerType = handlerType + "," + ProjectBase.Utils.ProjectHierarchy.BusinessDelegateNS;
                var t = Type.GetType(handlerType);
                if (t == null)
                {
                    if (method.Name == "CreateSynchronizeHandler")
                        throw new Exception("SyncHandlerNotFound" + handlerType);
                    else
                        throw new Exception("ReceiveHandlerNotFound" + handlerType);
                }
                return t;
            }
            //文件导入
            else if (method.Name == "CreateImport" && arguments.Length == 1 && arguments[0] is string)
            {
                return GetImporter((string)arguments[0]);
            }
            //文件解析
            else if (method.Name == "CreateParser" && arguments.Length == 1 && arguments[0] is string)
            {
                return GetFileParser((string)arguments[0]);
            }
            else if (method.Name == "GetJobCalculator" && arguments.Length == 1 && arguments[0] is string)
            {
                return GetJobCalculator((string)arguments[0]);
            }

            return base.GetComponentType(method, arguments);

        }

        private Type GetImporter(string type)
        {
            IDictionary<string, string> importerType = GetImporterRegistry();
            var t = Type.GetType(importerType[type]);
            if (t == null)
                throw new ImporterNotFoundException(type);
            return t;
        }
        private Type GetFileParser(string type)
        {
            IDictionary<string, string> parserType = GetFilePaserRegistry();
            var t = Type.GetType(parserType[type]);
            if (t == null)
                throw new BizException("FilePaserNotFoundException:" + type);
            return t;
        }
        private Type GetJobCalculator(string orderType)
        {
            IDictionary<string, string> calculatorType = GetJobCalculatorRegistry();
            var t = Type.GetType(calculatorType[orderType]);
            if (t == null)
                throw new BizException("JobCalculatorNotFoundException:" + orderType);
            return t;
        }

        protected abstract IDictionary<string, string> GetImporterRegistry();
        protected abstract IDictionary<string, string> GetFilePaserRegistry();
        protected abstract IDictionary<string, string> GetJobCalculatorRegistry();
    }
}
