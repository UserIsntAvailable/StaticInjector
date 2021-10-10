using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace StaticInjector
{
    [Generator]
    // ReSharper disable once InconsistentNaming
    public class StaticInjectorGenerator : ISourceGenerator
    {
        internal const string STATIC_INJECTOR_NAME = "InjectStatic";

        internal static readonly string StaticInjectorAttributeName = $"{STATIC_INJECTOR_NAME}Attribute";

        private static readonly string StaticInjectorAttributeText = $@"using System;

namespace {STATIC_INJECTOR_NAME}
{{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    sealed class {StaticInjectorAttributeName} : Attribute
    {{
        public Type ServiceType {{ get; }}
        
        public {StaticInjectorAttributeName}(Type serviceType)
        {{
            this.ServiceType = serviceType;
        }}        
    }}
}}
";

        public void Execute(GeneratorExecutionContext context)
        {
            if(context.SyntaxContextReceiver is not SyntaxContextReceiver receiver) return;
            
            // TODO - Generate partial classes
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForPostInitialization(
                x => x.AddSource(
                    StaticInjectorAttributeName,
                    StaticInjectorAttributeText
                )
            );

            context.RegisterForSyntaxNotifications(() => new SyntaxContextReceiver());
        }
    }
}
