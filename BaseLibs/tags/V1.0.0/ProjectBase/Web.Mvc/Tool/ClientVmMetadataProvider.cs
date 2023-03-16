using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using NHibernate.Util;
using ProjectBase.Domain;
using ProjectBase.Utils;
using ProjectBase.Web.Mvc.Angular;
using ProjectBase.Web.Mvc.Validation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjectBase.Web.Mvc.Tool
{
    /**
     * provide Json format object of metadata to client
     *在所有meta对象中(由{@link AngularModelMetadataProvider}创建)，筛选出用于输入结构的内部或外部类(由命名规范或{@link VmAsInput}标识)，生成客户端使用的VmMeta数据。包括验证规则。
     *生成结果是Map，key为类简单名。common模块指不是业务模块的部分如projectbase和home等
     *其中meta数据的key使用了简短的字符串，用于减少客户端代码文件(meta.json)的大小。
     * @see --internal
     */
    public class ClientVmMetadataProvider
    {
        private static List<Type> excludeClasses = new List<Type>() { typeof(ListInput), typeof(DORef<,>), typeof(Pager) };
        private static readonly string VMMeta_Filter_ClassNameSuffix = "Input";
        private static readonly string ModuleNameForCommonClass = "common";
        //types that can be annotated for validation(client),basically primitive types and non collection and DictEnum
        private static List<Type> leafModelTypes = new List<Type>() {
                typeof(string),typeof(DateTime),typeof(ListInput),typeof(DORef<,>)
            //int.class,long.class,BigDecimal.class,boolean.class,short.class,byte.class,char.class,
            //Integer.class,Long.class,Boolean.class,Short.class,Byte.class,Object.class,DORef.class,
            //ListInput.class,ListInputTypedLong.class,ListInputTypedString.class
        };

        private static Dictionary<string, SortedDictionary<string, ClientVmMetadata>> clientMetasByModule = new Dictionary<string, SortedDictionary<string, ClientVmMetadata>>();

        //每个VM的clientMeta只生成一次，记录其对应的serverMeta以便不重复生成
        private static IDictionary<Type, ModelMetadata> serverMetas = new Dictionary<Type, ModelMetadata>();

        private static string outputFolderRoot = null;
        public static bool ShouldRun { get; set; }
        public static void Config(string outputFolderRoot)
        {
            if (!String.IsNullOrEmpty(outputFolderRoot) && Directory.Exists(outputFolderRoot))
            {
                ClientVmMetadataProvider.outputFolderRoot = outputFolderRoot;
                ShouldRun = true;
            }
        }
        //输出VMInput类及其引用的类的meta
        public static void CreateAll(IModelMetadataProvider provider, IEnumerable<Type> typesOfViewModelNS)
        {
            var inputClasses = typesOfViewModelNS.Where(t => t.Name.EndsWith(VMMeta_Filter_ClassNameSuffix) || Attribute.IsDefined(t, typeof(VmAsInputAttribute)));
            inputClasses=inputClasses.Append(typeof(ListInput));
            foreach (var clazz in inputClasses)
            {
                var meta = provider.GetMetadataForType(clazz);
                var clientMeta = createClientVmMetadataFromModelMetadata(clazz, meta, null, null, null);
                clientMeta.FormIgnore = null;
            }
            if (!clientMetasByModule.ContainsKey(ModuleNameForCommonClass))
            {
                clientMetasByModule.Add(ModuleNameForCommonClass, new SortedDictionary<string, ClientVmMetadata>());
            }
            clientMetasByModule[ModuleNameForCommonClass].Add("DORef", createClientVmMetadaForDORef());

            string[] outputFolders = outputFolderRoot.Split(',');
            foreach (var entry in clientMetasByModule)
            {
                var moduleName = entry.Key;
                var metas = entry.Value;

                if (!Directory.Exists(outputFolders[0] + "\\" + moduleName)) continue;

                string sFileName = outputFolders[0] + "\\" + moduleName + "\\" + moduleName + ".meta.json";
                if (!File.Exists(outputFolders[0] + "\\" + moduleName))
                {
                    sFileName = outputFolders[0] + "\\" + moduleName + "\\" + moduleName + ".meta.json";
                }
                if (moduleName.Equals("common", StringComparison.OrdinalIgnoreCase))
                {
                    sFileName = outputFolders[0] + "\\Shared\\common.meta.json";
                }
                var text = JsonConvert.SerializeObject(metas);
                var bytes = Encoding.UTF8.GetBytes(text);
                if (text.Length > 10 && new FileGeneratorUtil().IsContentSame(sFileName, bytes, false) == false)
                {
                    File.WriteAllBytes(sFileName, bytes);
                }
            }
        }

        private static ClientVmMetadata createClientVmMetadataFromModelMetadata(Type metaForClass, ModelMetadata meta,
                IEnumerable<ModelMetadata> metaProperties, string propName, Type eleType)
        {
            SortedDictionary<string, ClientVmMetadata> metasOftheModule;
            string metaKey;
            ClientVmMetadata clientMeta = new ClientVmMetadata();
            if (metaForClass != null)
            {// 处理类的meta
                metaKey = GetSimpleNameDolar(metaForClass);
                string moduleName = GetModuleName(metaForClass);
                if (clientMetasByModule.ContainsKey(moduleName))
                {
                    metasOftheModule = clientMetasByModule[moduleName];
                }
                else
                {
                    //按固定顺序类生成文件内容，使相同内容的文件不会因内容顺序不同而被认为不同，减少了文件的重复覆盖。
                    metasOftheModule = new SortedDictionary<string, ClientVmMetadata>(Comparer<string>.Create((o1, o2) => o1.CompareTo(o2)));
                    clientMetasByModule[moduleName] = metasOftheModule;
                }

                if (metasOftheModule.ContainsKey(metaKey))
                {
                    return metasOftheModule[metaKey];
                }
                metasOftheModule.Add(metaKey, clientMeta);
            }
            if (eleType == null)
            {
                Type modelClass = meta == null ? metaForClass : meta.ModelType;
                if (meta != null && meta.IsNullableValueType)
                {
                    modelClass = Nullable.GetUnderlyingType(modelClass);
                }
                clientMeta.ModelType = GetSimpleNameDolar(modelClass);
            }
            if (meta != null)
            {
                clientMeta.IsRequired = meta.IsRequired;
                clientMeta.TranslateKey = meta.DisplayName;

                var fattr = meta.PropertyName == null ?
                    Attribute.GetCustomAttribute(meta.ModelType, typeof(FormIgnoreAttribute))
                    : Attribute.GetCustomAttribute(meta.PropertyGetter.Method, typeof(FormIgnoreAttribute));
                clientMeta.FormIgnore = fattr != null;
                var sattr = meta.PropertyName == null ?
                    Attribute.GetCustomAttribute(meta.ModelType, typeof(SubmitIgnoreAttribute))
                    : Attribute.GetCustomAttribute(meta.PropertyGetter.Method, typeof(SubmitIgnoreAttribute));
                clientMeta.SubmitIgnore = sattr != null;

                IEnumerable<BaseModelClientValidationRule> rules = getClientValidationRules(meta);
                if (meta.ContainerType == null)
                {// means this is a class not a leaf type
                    clientMeta.AddValidationRules(rules);
                }
                else
                {
                    clientMeta.SetValidationRules(rules);
                }
            }
            if (metaForClass != null)//for class
            {
                var properties = metaProperties == null ? meta.Properties : metaProperties;
                if (properties.Count() > 0)
                {
                    IDictionary<string, ClientVmMetadata> propMap = new Dictionary<string, ClientVmMetadata>();
                    foreach (var propmeta in properties)
                    {
                        Type eleTypeForLeafArray = null;
                        ClientVmMetadata propClientMeta = null;
                        bool isArray = false;
                        if (propmeta.ModelType.IsArray)
                        {
                            isArray = true;
                            eleTypeForLeafArray = propmeta.ModelType.GetElementType();
                        }
                        else if (typeof(ICollection).IsAssignableFrom(propmeta.ModelType))
                        {
                            try
                            {
                                Type eleClass = propmeta.ModelType.GenericTypeArguments[0];
                                eleTypeForLeafArray = eleClass;
                            }
                            catch (Exception e)
                            {
                                if (propmeta.PropertyName != "selectedValues")
                                {
                                    throw;
                                }
                                eleTypeForLeafArray = typeof(object);
                            }
                        }
                        if (eleTypeForLeafArray == null || IsLeafType(eleTypeForLeafArray))// leafType type or array/collection of leafType
                        {
                            propClientMeta = createClientVmMetadataFromModelMetadata(null, propmeta, null, null, eleTypeForLeafArray);
                        }
                        else
                        {//集合类型的属性的meta是集合元素类型的meta，以及复合类型的属性的meta,其中都没记录属性本身的meta，而后者需要分析该属性的field和getter
                            var mps = meta.GetMetadataForProperties(eleTypeForLeafArray);
                            propClientMeta = createClientVmMetadataFromModelMetadata(null, propmeta, mps, null, eleTypeForLeafArray);
                        }
                        if (eleTypeForLeafArray != null)
                        { //成员类型为叶型的集合或数组大小，成员验证
                            string modelType = "[]=" + GetSimpleNameDolar(eleTypeForLeafArray);
                            /////////          Size sizeAttr=getAnnotation(propmeta.getField(),propmeta.getPropertyGetter(),Size.class);;
                            ////////          if(sizeAttr!=null) {
                            /////////          	modelType = modelType + ","+ sizeAttr.min()+","+sizeAttr.max();
                            //////////          }
                            propClientMeta.ModelType = modelType;

                            //	AnnotatedType atype=null;
                            //	List<ModelClientValidationRule> rules=null;
                            //	if (isArray) {
                            //		 atype=util.getAnnotatedType(propmeta.getField(),propmeta.getPropertyGetter());
                            //	} else {
                            //		 atype=util.getCollectionAnnotatedTypeArgument(propmeta.getField(),propmeta.getPropertyGetter());
                            //	}
                            //	rules=validationProvider.getValidationRules(atype,propmeta.getIsRequired());
                            //	propClientMeta.setValidationRules(null);
                            //	propClientMeta.addLeafEleValidationRules(rules);
                            //	propClientMeta.addValidationRules(getArrayOrCollectionSizeRules(sizeAttr));
                            //	validationProvider.addAdapterRules(propField, propGetter, rules);
                        }

                        if (propmeta.ModelType.IsSubclassOf(typeof(DORef<,>)))
                        {
                            Type idType = meta.ModelType.GenericTypeArguments[0];

                            string modelType = "DORef:" + idType.Name;
                            propClientMeta.ModelType = modelType;
                        }
                        propMap[propmeta.PropertyName] = propClientMeta;
                    }
                    clientMeta.Properties = propMap;
                }
            }
            else
            {// for class's prop
                var checkType = eleType == null ? meta.ModelType : eleType;
                var useProperties = metaProperties != null ? metaProperties : meta.Properties;
                if (!IsLeafType(checkType))
                {// propmeta is for a prop-referenced class,so in addition, need to add
                 // new clientmeta data for this current prop
                    createClientVmMetadataFromModelMetadata(checkType, null, useProperties, null, null);
                    /***********将删除
                    Type propType = meta.ModelType;
                    if (eleType!=null)
                    {  //成员类型为复合型的集合或数组属性对应的clientmeta中记录元素类型名和集合大小
                        string modelType=propType.Name+"="+ eleType.Name;
		                ///////////Size sizeAttr=getAnnotation(propField,propGetter,Size.class);
		                //////////////if(sizeAttr!=null) {
		            	    ///////////////modelType = modelType + ","+ sizeAttr.min()+","+sizeAttr.max();
		               ///////////// }
		                clientMeta.ModelType=modelType;
					    /////////////List<ModelClientValidationRule> rules = getArrayOrCollectionSizeRules(sizeAttr);
					    /////////////validationProvider.addAdapterRules(propField, propGetter, rules);
					    /////////////clientMeta.AddValidationRules(rules);
					    ////////////Type declaringClass = meta.DisplayNamepropField!=null ? propField.getDeclaringClass() : propGetter.getDeclaringClass();
					    ///////////////string translate=ModelMetadataProvider...getCurrent().displayName(declaringClass, null, propName, propGetter, propField);
					    clientMeta.TranslateKey=meta.DisplayName;
                    }***************/
                }
            }

            return clientMeta;
        }
        //private static List<ModelClientValidationRule> getArrayOrCollectionSizeRules(Size sizeAttr) {
        //	if (sizeAttr==null) return null;
        //	List<ModelClientValidationRule> rules= new ArrayList<ModelClientValidationRule>();
        //	rules.Add(new BaseModelClientValidationRule( "arraySizeMax",sizeAttr.max(),sizeAttr.Groups()));
        //	rules.Add(new BaseModelClientValidationRule("arraySizeMin",sizeAttr.min().sizeAttr.Groups()));
        //	return rules;
        //}

        private static List<BaseModelClientValidationRule> getClientValidationRules(ModelMetadata meta)
        {
            var rules = ClientDataTypeModelValidatorProvider.GetClientValidationRules(meta);
            var validators = meta.ValidatorMetadata;
            if (validators == null) return rules;

            foreach (var validator in validators)
            {
                if (validator is IClientValidatable)
                {
                    var rules2 = ((IClientValidatable)validator).GetClientValidationRules(meta);
                    rules.AddRange(rules2);
                }
            }
            if (meta.IsRequired && rules.All((rule) => rule.ValidationType != "required"))
            {
                rules.Add(new BaseModelClientValidationRule("required", false));
            }
            return rules;
        }

        private static ClientVmMetadata createClientVmMetadaForDORef()
        {
            ClientVmMetadata clientMeta = new ClientVmMetadata();
            IDictionary<string, ClientVmMetadata> propMap = new Dictionary<string, ClientVmMetadata>();
            ClientVmMetadata propClientMeta = new ClientVmMetadata();
            // propClientMeta.setContainerType("DORef");
            propClientMeta.ModelType = "int";
            propClientMeta.IsRequired = true;
            propClientMeta.TranslateKey = "DORefId";
            propMap.Add("id", propClientMeta);
            propClientMeta = new ClientVmMetadata();
            // propClientMeta.setContainerType("DORef");
            propClientMeta.ModelType = "string";
            propClientMeta.SubmitIgnore = true;
            propMap.Add("refText", propClientMeta);
            clientMeta.Properties = propMap;
            return clientMeta;
        }
        private static string GetModuleName(Type metaForClass)
        {
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

        private static bool IsLeafType(Type clazz)
        {
            
            return clazz.IsEnum || (clazz.Namespace!=null && clazz.Namespace.StartsWith("System")) || clazz.IsAssignableFrom(typeof(DORef<,>))
                 || clazz.IsAssignableFrom(typeof(ListInput));
        }

    }
}

