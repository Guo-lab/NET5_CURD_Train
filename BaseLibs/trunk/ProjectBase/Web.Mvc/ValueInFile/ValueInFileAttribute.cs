using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using ProjectBase.CastleWindsor;
using ProjectBase.Utils;
using System;

namespace ProjectBase.Web.Mvc.ValueInFile
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ValueInFileAttribute : Attribute
    {
        public string FileParserResourceName { get; set; }
        public ValueInFileAttribute(string fileParserResourceName)
        {
            FileParserResourceName = fileParserResourceName;
        }
    }

}