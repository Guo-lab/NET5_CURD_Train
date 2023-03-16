using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using ProjectBase.CastleWindsor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProjectBase.Web.Mvc.ValueInFile
{
    public class ValueInFileValueProvider : IValueProvider
    {
        private IList<Dictionary<string, string>> rowMaps;
        public ValueInFileValueProvider(ValueProviderFactoryContext context, string fileNameParamName, string fileParserResourceName)
        {
            var fileName = context.ActionContext.HttpContext.Request.Form[fileNameParamName];
            var resolver = context.ActionContext.HttpContext.RequestServices.GetService<DependencyResolver>();
            var parser = (IValueInFileParser)resolver.Resolve(fileParserResourceName, typeof(IValueInFileParser));
            Init(context, fileName, parser);
        }
        public ValueInFileValueProvider(ValueProviderFactoryContext context, string fileName, IValueInFileParser fileParser)
        {
            Init(context, fileName, fileParser);
        }
        private void Init(ValueProviderFactoryContext context, string fileName, IValueInFileParser fileParser)
        {
            var pathProvider = context.ActionContext.HttpContext.RequestServices.GetService<IValueInFilePathProvider>();
            if (pathProvider != null)
            {
                fileName = pathProvider.GetFilePath(fileName);
            }
            var file = new FileInfo(fileName);
            rowMaps = fileParser.ParseAsNV(file);
        }

        public bool ContainsPrefix(string prefix)
        {
            var index = GetIndexAndPosE(prefix).Item1;
            return index < rowMaps.Count();
        }

        public ValueProviderResult GetValue(string key)
        {
            var indexAndPosE = GetIndexAndPosE(key);
            if (indexAndPosE.Item1 == -1) return ValueProviderResult.None;

            key = key.Substring(indexAndPosE.Item2 + 2);
            if (rowMaps[indexAndPosE.Item1].ContainsKey(key))
            {
                return new ValueProviderResult(rowMaps[indexAndPosE.Item1][key]);
            }
            return ValueProviderResult.None;
        }

        private Tuple<int,int> GetIndexAndPosE(string key)
        {
            var posB = key.IndexOf("[");
            if (posB < 0) return Tuple.Create(-1,0);
            var posE = key.IndexOf("]");
            return Tuple.Create(Convert.ToInt32(key.Substring(posB + 1, posE - posB - 1)), posE);
        }
    }
}