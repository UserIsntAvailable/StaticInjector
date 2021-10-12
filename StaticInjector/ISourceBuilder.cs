using System.Collections.Generic;

namespace StaticInjector
{
    public interface ISourceBuilder
    {
        /// <summary>
        /// Writes anything to the current builder 
        /// </summary>
        public void Write(string value, bool indent = true);

        /// <summary>
        /// Writes anything to the current builder followed by a \n
        /// </summary>
        public void WriteLine(string value = "", bool indent = true);

        /// <summary>
        /// Writes an using statement
        /// </summary>
        public void WriteUsing(string name);

        /// <summary>
        /// Writes multiples using statements
        /// </summary>
        // ReSharper disable once IdentifierTypo
        public void WriteUsings(IEnumerable<string> names);

        /// <summary>
        /// Writes a namespace declaration 
        /// </summary>
        public void WriteNamespace(string name, bool startScope = true);

        /// <summary>
        /// Writes a class declaration 
        /// </summary>
        public void WriteClass(IEnumerable<string> modifiers, string name, bool startScope = true);

        /// <summary>
        /// Writes a field declaration 
        /// </summary>
        public void WriteField(IEnumerable<string> modifiers, string returnType, string name, string defaultValue = "");

        /// <summary>
        /// Writes a method declaration 
        /// </summary>
        public void WriteMethod(
            IEnumerable<string> modifiers,
            string returnType,
            string name,
            IEnumerable<(string paraType, string paraName, string defaultValue)> parameters,
            bool startScope = true);

        /// <summary>
        /// Starts a new scope ( { )
        /// </summary>
        public void StartScope();

        /// <summary>
        /// Closes the current scope ( } )
        /// </summary>
        public void CloseScope();

        /// <summary>
        /// Removes every character wrote for the current instance
        /// </summary>
        public void Refresh();
    }
}
