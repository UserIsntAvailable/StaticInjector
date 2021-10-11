using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace StaticInjector
{
    public class SyntaxContextReceiver : ISyntaxContextReceiver
    {
        public Dictionary<INamedTypeSymbol, IEnumerable<ITypeSymbol>> TypesToInject { get; } = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if(context.Node is TypeDeclarationSyntax typeDecl 
               && typeDecl.Modifiers.Any(m => m.Text == "partial") 
               && typeDecl.AttributeLists.Any(
                   x => x.Attributes.Any(
                       xx => xx.Name.ToString() == StaticInjectorGenerator.STATIC_INJECTOR_NAME
                   )
               )
            )
            {
                var staticInjectArgTypes = typeDecl.AttributeLists.SelectMany(
                    attrList => attrList.Attributes.Where(
                        attr => attr.ArgumentList?.Arguments.Count == 1 
                                && context.SemanticModel.GetTypeInfo(attr).Type?.ToDisplayString() ==
                                $"{nameof(StaticInjector)}.{StaticInjectorGenerator.StaticInjectorAttributeName}"
                    )
                ).Select(
                    attr =>
                    {
                        var typeOfExp = (TypeOfExpressionSyntax)attr.ArgumentList!.Arguments[0].Expression;
                        
                        return (ITypeSymbol)context.SemanticModel.GetSymbolInfo(typeOfExp.Type).Symbol!;
                    }
                );

                var typeSymbol = (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(typeDecl)!;
                
                this.TypesToInject.Add(typeSymbol, staticInjectArgTypes);
            }
        }
    }
}
