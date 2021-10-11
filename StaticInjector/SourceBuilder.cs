using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StaticInjector
{
    public class SourceBuilder : ISourceBuilder
    {
        private StringBuilder _stringBuilder;
        private int _currentIndentation;

        public SourceBuilder(StringBuilder? stringBuilder = null)
        {
            _stringBuilder = stringBuilder ?? new StringBuilder();
        }

        public void Write(string value, bool indent = true)
        {
            _stringBuilder.Append($"{(indent ? new string('\t', _currentIndentation) : "")}{value}");
        }

        public void WriteLine(string value = "", bool indent = true)
        {
            this.Write($"{value}\n", indent);
        }

        public void WriteUsing(string name)
        {
            this.WriteLine($"using {name};");
        }

        // ReSharper disable once IdentifierTypo
        public void WriteUsings(IEnumerable<string> names)
        {
            foreach(var name in names)
            {
                this.WriteUsing(name);
            }
        }

        public void WriteNamespace(string name, bool startScope = true)
        {
            this.WriteLine();

            var namespaceDecl = $"namespace {name}";

            if(startScope)
            {
                this.WriteLine(namespaceDecl);
                this.StartScope();

                return;
            }

            this.WriteLine($"{namespaceDecl};");
        }

        /// <inheritdoc/>
        /// <remarks>If <paramref name="startScope"/> == false, a ( : ) will be added automatically</remarks>
        public void WriteClass(IEnumerable<string> modifiers, string name, bool startScope = true)
        {
            var classDecl = $"{string.Join(" ", modifiers)} class {name}";

            if(startScope)
            {
                this.WriteLine(classDecl);
                this.StartScope();

                return;
            }

            this.WriteLine($"{classDecl}: ");
        }

        public void WriteMethod(
            IEnumerable<string> modifiers,
            string returnType,
            string name,
            IEnumerable<(string paraType, string paraName)> parameters,
            bool startScope = true)
        {
            var parametersString = string.Join(", ", parameters.Select(p => $"{p.paraType} {p.paraName}"));

            var methodDecl = $"{string.Join(" ", modifiers)} {returnType} {name}({parametersString})";

            if(startScope)
            {
                this.WriteLine(methodDecl);
                this.StartScope();

                return;
            }

            this.Write(methodDecl);
        }

        public void WriteField(IEnumerable<string> modifiers, string returnType, string name, string defaultValue = "")
        {
            var fieldDecl =
                $"{string.Join(" ", modifiers)} {returnType} {name} {(defaultValue != "" ? $"= {defaultValue}" : "")};";

            this.WriteLine(fieldDecl);
        }

        public void StartScope()
        {
            this.WriteLine("{");
            _currentIndentation += 1;
        }

        public void CloseScope()
        {
            _currentIndentation -= 1;
            this.WriteLine("}");
        }

        public void Refresh()
        {
            _currentIndentation = 0;

            _stringBuilder = _stringBuilder.Clear();
        }

        public override string ToString() => _stringBuilder.ToString();
    }
}
