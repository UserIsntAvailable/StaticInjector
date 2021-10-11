﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace StaticInjector
{
    // TODO - Report diagnostics
    
    [Generator]
    // ReSharper disable once InconsistentNaming
    public class StaticInjectorGenerator : ISourceGenerator
    {
        internal const string STATIC_INJECTOR_NAME = "InjectStatic";

        internal static readonly string StaticInjectorAttributeName = $"{STATIC_INJECTOR_NAME}Attribute";

        private static readonly string StaticInjectorAttributeText = $@"// <auto-generated/>

#nullable disable

using System;

namespace {nameof(StaticInjector)}
{{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal sealed class {StaticInjectorAttributeName} : Attribute
    {{
        public Type ServiceType {{ get; }}
        
        public {StaticInjectorAttributeName}(Type serviceType)
        {{
            this.ServiceType = serviceType;
        }}        
    }}
}}
";

        private readonly ISourceBuilder _sourceBuilder = new SourceBuilder();

        public void Execute(GeneratorExecutionContext context)
        {
            if(context.SyntaxContextReceiver is not SyntaxContextReceiver contextReceiver) return;

            this.InjectTypes(context, contextReceiver.TypesToInject);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForPostInitialization(
                x => x.AddSource(
                    $"{StaticInjectorAttributeName}.Generated.cs",
                    StaticInjectorAttributeText
                )
            );

            context.RegisterForSyntaxNotifications(() => new SyntaxContextReceiver());
        }
        
        private void InjectTypes(
            GeneratorExecutionContext context,
            Dictionary<INamedTypeSymbol, IEnumerable<ITypeSymbol>> injectedTypes)
        {
            foreach(var kvp in injectedTypes)
            {
                var partialClassSymbol = kvp.Key;

                var partialClass = partialClassSymbol.DeclaringSyntaxReferences.First()
                                                     .GetSyntax() as ClassDeclarationSyntax;

                var partialClassNamespaceName = partialClassSymbol.ContainingNamespace.ToDisplayString();

                var typesToInject = kvp.Value.ToList();

                var typesNamespaces = typesToInject.Select(x => x.ContainingNamespace.ToDisplayString())
                                                   .Where(x => x != partialClassNamespaceName)
                                                   .Distinct();

                _sourceBuilder.WriteLine("// <auto-generated/>\n");
                _sourceBuilder.WriteLine("#nullable enable\n");
                _sourceBuilder.WriteUsings(typesNamespaces);
                _sourceBuilder.WriteNamespace(partialClassNamespaceName);
                _sourceBuilder.WriteClass(partialClass!.Modifiers.Select(x => x.Text), partialClassSymbol.Name);

                this.WriteTypesRegion(typesToInject);

                _sourceBuilder.CloseScope(); // WriteClass
                _sourceBuilder.CloseScope(); // WriteNamespace

                context.AddSource(
                    $"{partialClassSymbol.Name}.Generated.cs",
                    SourceText.From(_sourceBuilder.ToString(), Encoding.UTF8)
                );
                
                _sourceBuilder.Refresh();
            }
        }

        private void WriteTypesRegion(IList<ITypeSymbol> typesToInject)
        {
            for(var i = 0; i < typesToInject.Count; i++)
            {
                var typeToInject = typesToInject[i];
                var typeName = typeToInject.Name;

                _sourceBuilder.WriteLine($"#region {typeName}");

                var fieldName = $"_{JsonNamingPolicy.CamelCase.ConvertName(typeName)}";

                _sourceBuilder.WriteField(new[] { "private", "static", "readonly", }, typeName, fieldName, "new()");

                _sourceBuilder.WriteLine();

                var typeMethods = typeToInject.GetMembers().OfType<IMethodSymbol>()
                                              .Where(
                                                  x => !x.IsStatic && !x.IsAsync &&
                                                       x.MethodKind == MethodKind.Ordinary &&
                                                       x.DeclaredAccessibility is 
                                                           Accessibility.Public or Accessibility.Internal
                                              ).Select(x => x.DeclaringSyntaxReferences.First().GetSyntax())
                                              .OfType<MethodDeclarationSyntax>()
                                              .ToList();

                this.WriteTypeMethods(typeMethods, fieldName);

                _sourceBuilder.WriteLine("#endregion");

                if(i != typesToInject.Count - 1) _sourceBuilder.WriteLine();
            }
        }

        private void WriteTypeMethods(IList<MethodDeclarationSyntax> typeMethods, string? fieldName)
        {
            for(var i = 0; i < typeMethods.Count; i++)
            {
                var typeMethod = typeMethods[i];
                var methodParameters = typeMethod.ParameterList.Parameters
                                                 .Select(p => (p.Type!.ToString(), p.Identifier.ToString()))
                                                 .ToArray();

                // TODO - Get interface implementation methods xml comments

                var xmlComments = typeMethod.GetLeadingTrivia()
                                            .Where(x => x.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia)
                                            .Select(x => x.ToFullString().Trim());
                
                foreach(var xmlComment in xmlComments)
                {
                    _sourceBuilder.WriteLine(xmlComment);
                }
                
                var methodName = typeMethod.Identifier.ToString();
                
                var methodModifiers = typeMethod.Modifiers.Select(m => m.ToString()).ToList();
                methodModifiers.Insert(1, "static");

                _sourceBuilder.WriteMethod(
                    methodModifiers,
                    typeMethod.ReturnType.ToString(),
                    methodName,
                    methodParameters,
                    false
                );

                _sourceBuilder.WriteLine(" =>", false);

                var methodParametersNames = string.Join(", ", methodParameters.Select(p => p.Item2));

                _sourceBuilder.WriteLine($"\t{fieldName}.{methodName}({methodParametersNames});");

                if(i != typeMethods.Count - 1) _sourceBuilder.WriteLine();
            }
        }
    }
}
