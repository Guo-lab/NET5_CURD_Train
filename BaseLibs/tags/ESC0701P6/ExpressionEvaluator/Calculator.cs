using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.Emit;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System;
namespace ExpressionEvaluator
{
    public class Calculator
    {
        public static bool CalculateBoolean(string expression)
        {
            string codeToCompile = "Namespace MyBoolean\r\n" +
                           "Public Class BooleanCalculator\r\n" +
                           "Public Function Calculate() As Boolean\r\n" +
                           "Return " + expression + "\r\n" +
                           "End Function\r\n" +
                           "End Class\r\n" +
                           "End Namespace";
            SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(codeToCompile);
            string assemblyName = Path.GetRandomFileName();
            var refPaths = new[] {
                typeof(System.Object).GetTypeInfo().Assembly.Location
            };
            MetadataReference[] references = refPaths.Select(r => MetadataReference.CreateFromFile(r)).ToArray();

            VisualBasicCompilation compilation = VisualBasicCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);
                if (!result.Success)
                {
                    
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    StringBuilder sb = new StringBuilder();
                    foreach (Diagnostic diagnostic in failures)
                    {
                        sb.AppendLine(diagnostic.GetMessage());
                    }
                    throw new System.Exception(sb.ToString());
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);

                    Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                    var type = assembly.GetType("MyBoolean.BooleanCalculator");
                    var instance = Activator.CreateInstance(type);
                    var methodInfo = type.GetMethod("Calculate");
                    return (bool)methodInfo.Invoke(instance, null);
                }
            }

        }
    }
}
