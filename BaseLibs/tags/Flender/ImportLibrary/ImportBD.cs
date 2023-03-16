using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ImportLibrary
{
    public class ImportBD : IImportBD
    {
        public IImportFactory ImportFactory { get; set; }
        public IFileParserFactory FileParserFactory { get; set; }
        public FeedbackResult<TImportItem> TypedImport<TImportItem>(string importName, string parserName,string fileName, params object[] parameters) where TImportItem : BaseImportItem 
        {
            return ImportInternal(importName, parserName, fileName, null,parameters).Import<TImportItem>();
        }

        public FeedbackResult<TImportItem> TypedImport<TImportItem>(string importName, string parserName, string fileName, IDictionary<string, object> parameterMap) where TImportItem : BaseImportItem
        {
            return ImportInternal(importName, parserName, fileName, parameterMap).Import<TImportItem>();
        }
        public object Import(string importName, string parserName, string fileName, IDictionary<string, object> parameterMap)
        {
            return ImportInternal(importName, parserName, fileName, parameterMap).Import2();
        }
        public object Import(string importName, string parserName, string fileName, params object[] parameters)
        {
            return ImportInternal(importName, parserName, fileName, null,parameters).Import2();
        }
        private IImport  ImportInternal(string importName, string parserName, string fileName, IDictionary<string, object> parameterMap, params object[] parameters)
        {
            var parser = FileParserFactory.CreateParser(parserName);
            parser.FileName = fileName;
            IImport importer = ImportFactory.CreateImport(importName);
            if (parameterMap != null)
            {
                importer.SetImportParameters(parameterMap);
            }
            else
            {
                importer.SetImportParameters(parameters);
            }
            importer.FileParser = parser;
            return importer;
        }

    }
}
