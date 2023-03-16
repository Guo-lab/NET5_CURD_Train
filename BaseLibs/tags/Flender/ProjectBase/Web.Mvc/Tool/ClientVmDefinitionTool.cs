using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProjectBase.Domain;
using ProjectBase.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjectBase.Web.Mvc.Tool
{
    /**
     * 开发工具类。生成客户端使用的ts文件，每个VM类对应定义ts接口类型.还为每个VM类生成一个对象序列化文件.json.还生成html.
     * 
     * @see --internal
     */
    public class ClientVmDefinitionTool
    {
        private static List<Type> excludeClasses = new List<Type>() { typeof(ListInput), typeof(DORef<,>), typeof(Pager) };
        private static readonly string ModuleNameForCommonClass = "common";
        private static Dictionary<string, SortedDictionary<string, string>> clientDefByModule = new Dictionary<string, SortedDictionary<string, string>>();
        // 每模块有一个Map，每个key为importfrom模块，value为import类型的列表
        private static Dictionary<string, SortedDictionary<string, List<string>>> refImportByModule = new Dictionary<string, SortedDictionary<string, List<string>>>();
        private static readonly string importPrefix = "../";
        // VM类及其引用的内部类外部类
        private static List<Type> vmAndReferencedClasses = new List<Type>();

        private static List<string> defInCommon = new List<string>();

        public static string outputFolderRoot = null;
        public static bool ShouldRun { get; set; }

        private static IModelMetadataProvider provider;

        private static List<string> includeModules;

        private static Util Util = new Util();
        public static void Config(string outputFolderRoot, string _includeModules)
        {
            if (!String.IsNullOrEmpty(outputFolderRoot) && Directory.Exists(outputFolderRoot))
            {
                ClientVmDefinitionTool.outputFolderRoot = outputFolderRoot;
                includeModules = _includeModules?.Split(",").ToList();
                ShouldRun = true;
            }
        }

        public static void CreateAll(IModelMetadataProvider _provider, IEnumerable<Type> typesOfViewModelNS)
        {// 为VM及其内部类（Input、ListRow等)、同一包下外部类 生成客户端ts接口定义
            provider = _provider;
            var vmClasses = typesOfViewModelNS.Where(t =>
                (t.Name.EndsWith(GlobalConstant.ViewModel_ClassSuffix)
                    || t.Name.EndsWith(GlobalConstant.ViewModel_ClassSuffix2)
                    || t.Name.EndsWith(GlobalConstant.ViewModel_InputSuffix)
                )&& (includeModules==null || includeModules.Exists(o=>t.Namespace.EndsWith("."+o))));

            foreach (var clazz in vmClasses)
            {
                var meta = provider.GetMetadataForType(clazz);
                if (shouldAddReferencedClassFor(clazz))
                {
                    vmAndReferencedClasses.Add(clazz);
                    createClientDefFromModelMetadata(clazz, meta, true);
                }
                else
                {
                    createClientDefFromModelMetadata(clazz, meta, false);
                }
            }
            if (!clientDefByModule.ContainsKey(ModuleNameForCommonClass))
            {
                clientDefByModule.Add(ModuleNameForCommonClass, new SortedDictionary<string, string>());
            }
            string[] outputFolders = outputFolderRoot.Split(',');
            foreach (var entry in clientDefByModule)
            {
                var moduleName = entry.Key;
                var def = entry.Value;
                if (!Directory.Exists(outputFolders[0] + "\\" + moduleName)) continue;

                string sFileName = outputFolders[0] + "\\" + moduleName + "\\" + moduleName + ".def.ts";
                if (!File.Exists(outputFolders[0] + "\\" + moduleName))
                {
                    sFileName = outputFolders[0] + "\\" + moduleName + "\\" + moduleName + ".def.ts";
                }
                if (moduleName.Equals(ModuleNameForCommonClass, StringComparison.OrdinalIgnoreCase))
                {
                    sFileName = outputFolders[0] + "\\Shared\\common.def.tmp";
                }

                var importTs = "";
                if (refImportByModule.ContainsKey(moduleName))
                {
                    foreach (var rentry in refImportByModule[moduleName])
                    {
                        var refModule = rentry.Key;
                        var imports = rentry.Value;
                        //////// Collections.sort(imports);
                        importTs += "import { ";
                        foreach (var importType in imports)
                        {
                            importTs += " " + importType + ",";
                            importTs.Remove(importTs.Length - 1);
                            importTs += " } from '";
                            if (refModule.Equals("projectbase"))
                            {
                                importTs += "projectbase/projectbase.type";
                            }
                            else if (defInCommon.Contains(importType))
                            {
                                importTs += "shared/" + ModuleNameForCommonClass + ".def";
                            }
                            else
                            {
                                importTs += importPrefix + refModule + "/" + refModule + ".def";
                            }
                            importTs += "';\r\n";
                        }
                    }
                }

                string defts = importTs + "\r\n";
                foreach (var classDef in def)
                {
                    defts = defts + classDef.Value + "\r\n";
                }

                var bytes = Encoding.UTF8.GetBytes(defts);
                if (defts.Length > 10 && new FileGeneratorUtil().IsContentSame(sFileName, bytes, false) == false)
                {
                    File.WriteAllBytes(sFileName, bytes);
                }
            }
        }
        
        private static string createClientDefFromModelMetadata(Type metaForClass, ModelMetadata meta, bool addReferencedClass)
        {
            SortedDictionary<string, string> metasOftheModule;
            SortedDictionary<string, List<string>> importsOftheModule;
            string metaKey = GetSimpleNameDolar(metaForClass);
            string moduleName = GetModuleName(metaForClass);
            string clientDef = "";

            if (!ProjectHierarchy.NamespaceMapToTablePrefix.ContainsKey(moduleName) && !ProjectHierarchy.NonprefixMvcModuleNames.Contains(moduleName))
            {
                moduleName = ModuleNameForCommonClass;
            }
            if (clientDefByModule.ContainsKey(moduleName))
            {
                metasOftheModule = clientDefByModule[moduleName];
            }
            else
            {
                metasOftheModule = new SortedDictionary<string, string>();
                clientDefByModule.Add(moduleName, metasOftheModule);
            }
            if (refImportByModule.ContainsKey(moduleName))
            {
                importsOftheModule = refImportByModule[moduleName];
            }
            else
            {
                importsOftheModule = new SortedDictionary<String, List<String>>();
                refImportByModule.Add(moduleName, importsOftheModule);
            }
            if (metasOftheModule.ContainsKey(metaKey))
            {
                return metasOftheModule[metaKey];
            }

            clientDef = "export interface " + metaKey + "{\r\n";
            foreach (var propmeta in meta.Properties)
            {
                Type modelTypeClass = propmeta.ModelType;
                if(Util.IsNullableType(modelTypeClass))
                {
                    modelTypeClass = Nullable.GetUnderlyingType(modelTypeClass);
                }
                string propname = propmeta.PropertyName;
                string arraySign = "";

                var isCollection = !propmeta.ModelType.IsValueType && propmeta.ModelType.GetInterfaces().Where(t => t.Name.StartsWith("ICollection")).FirstOrDefault() != null;
                // 或者这样判断集合  o.PropertyType.IsAssignableTo(typeof(IEnumerable))&& o.PropertyType!=typeof(string)
                if (propmeta.ModelType.IsArray || isCollection)
                {
                    arraySign = "[]";
                    if (propmeta.ModelType.IsArray)
                    {
                        modelTypeClass = propmeta.ModelType.GetElementType();
                    }
                    else if (isCollection)
                    {
                        try
                        {
                            Type eleClass = propmeta.ModelType.GenericTypeArguments[0];
                            modelTypeClass = eleClass;
                        }
                        catch (Exception)
                        {
                            if (propmeta.PropertyName != "SelectedValues")
                            {
                                throw;
                            }
                            modelTypeClass = typeof(object);
                        }
                    }
                }

                string[] classnames = modelTypeClass.FullName.Split('.');
                String modelType = GetSimpleNameDolar(modelTypeClass);
                if (Util.IsNullableType(modelTypeClass))
                {
                    modelType = "any";//不支持集合元素为可空类型
                }
                else if (ProjectBase.Web.Mvc.Validation.ClientDataTypeModelValidatorProvider.IsNumericType(modelTypeClass))
                {
                    modelType = "number";
                }
                else if (modelTypeClass == typeof(string))
                {
                    modelType = "string";
                }
                else if (modelTypeClass == typeof(bool))
                {
                    modelType = "boolean";
                }
                else if (modelTypeClass == typeof(DateTime))
                {
                    modelType = "Date";
                }
                else
                {
                    if (modelTypeClass.IsEnum || modelType.EndsWith("Enum"))
                    {
                        modelType = "number";
                    }
                    else
                    {
                        string refModule = GetModuleName(modelTypeClass);
                        if (excludeClasses.Contains(modelTypeClass))
                        {
                            refModule = "projectbase";
                        }
                        if (addReferencedClass && !excludeClasses.Contains(modelTypeClass) && !vmAndReferencedClasses.Contains(modelTypeClass))
                        {
                            // VM中引用的类
                            vmAndReferencedClasses.Add(modelTypeClass);
                            createClientDefFromModelMetadata(modelTypeClass, provider.GetMetadataForType(modelTypeClass), shouldAddReferencedClassFor(modelTypeClass));
                        }
                        if (!modelType.EndsWith(GlobalConstant.ViewModel_ClassSuffix)
                                && !modelType.EndsWith(GlobalConstant.ViewModel_ClassSuffix2)
                                && !modelType.EndsWith(GlobalConstant.ViewModel_InputSuffix)
                                && !modelTypeClass.IsNested
                                && !excludeClasses.Contains(modelTypeClass))
                        {
                            if (metaForClass.Namespace != modelTypeClass.Namespace)
                            {// 引用的外部类不在同一包下
                                modelType = "any";
                            }
                        }
                        else if (!modelTypeClass.IsNested && !moduleName.Equals(refModule))
                        {
                            if (!importsOftheModule.ContainsKey(refModule))
                            {
                                List<String> imports = new List<String>();
                                importsOftheModule[refModule] = imports;
                            }
                            if (!importsOftheModule[refModule].Contains(modelType))
                            {
                                importsOftheModule[refModule].Add(modelType);
                            }
                        }
                    }
                }
                clientDef = clientDef + "\t" + propname + ": " + modelType + arraySign + ";\r\n";
            }
            clientDef = clientDef + "}";
            metasOftheModule[metaKey] = clientDef;
            return clientDef;
        }
        private static bool shouldAddReferencedClassFor(Type clazz)
        {
            var ok= clazz.Name.EndsWith(GlobalConstant.ViewModel_ClassSuffix)
                    || clazz.Name.EndsWith(GlobalConstant.ViewModel_ClassSuffix2);
            if (!clazz.IsNested) return ok;
            //内部类看最外层是否VM类
            var name=clazz.FullName.Substring(0, clazz.FullName.IndexOf("+"));
            return name.EndsWith(GlobalConstant.ViewModel_ClassSuffix)
                    || name.EndsWith(GlobalConstant.ViewModel_ClassSuffix2);
        }

        private static string GetModuleName(Type metaForClass)
        {
            if (ProjectHierarchy.GetModuleName != null)
            {
                var mdnane = ProjectHierarchy.GetModuleName.Invoke(metaForClass);
                if (mdnane != null) return mdnane;
            }

            var areaAttr = metaForClass.CustomAttributes.OfType<PBAreaAttribute>().SingleOrDefault();
            if (areaAttr != null) return areaAttr.Value;

            string classname = metaForClass.FullName;
            int pos1 = classname.LastIndexOf('.');
            int pos2 = classname.LastIndexOf('.', pos1 - 1);
            string moduleName = classname.Substring(pos2 + 1, pos1 - pos2 - 1);
            if (!ProjectHierarchy.NamespaceMapToTablePrefix.ContainsKey(moduleName) && !ProjectHierarchy.NonprefixMvcModuleNames.Contains(moduleName))
            {
                moduleName = ModuleNameForCommonClass;
            }
            return moduleName;
        }
        private static string GetSimpleName(Type clazz)
        {
            var fn = clazz.FullName;
            return fn.Substring(fn.LastIndexOf(".") + 1);
        }
        private static string GetSimpleNameDolar(Type clazz)
        {
            var fn = clazz.FullName;
            return GetSimpleName(clazz).Replace("+", "$");
        }
    }
}






