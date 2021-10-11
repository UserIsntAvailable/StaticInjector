﻿# StaticInjector

A tool to inject public instance methods of a class into a static class using SourceGenerators. This may seem overkill or counterproductive ( since this is pretty much a "Singleton Pattern" that doesn't have the benefit of Singletons, if that makes any sense. ), but I wanted to try out Source Generators, because I really like metaprogramming tools.

## Usage

Compile this program and add the following lines to your program.csproj:
```xml
<!-- https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview -->
<ItemGroup>
    <ProjectReference Include="path-to-sourcegenerator-project.dll"
                      OutputItemType="Analyzer"
                      ReferenceOutputAssembly="false" />
</ItemGroup>
```

Create a partial class with the ```InjectStaticAttribute``` ( don't forget to add ```using StaticInject;``` ):

```c#
using StaticInjector;

[InjectStatic(typeof(Type that you want inject))]
public static partial class App
{
}
```

(NOTE: For now, the type that you want to inject should have a public/internal parameterless constructor.)

Compile your program, and all syntax errors should disappear, once the code is generated, if not restart your IDE.

## Samples

You can try out ```ConsoleSample``` project to play with the source generator.

This is the code that is generated with it:

```c#
// <auto-generated/>

#nullable enable

namespace ConsoleSample
{
    public static partial class App
    {
        #region Console
        private static readonly global::ConsoleSample.InjectedClasses.Console _console = new();

        /// <summary>
        /// Writes a line to the console
        /// </summary>
        /// <param name="value">The value that will be printed at the console</param>
        /// <param name="amount">The amount of times it will be printed</param>
        public static void WriteLine(string value, int amount) =>
            _console.WriteLine(value, amount);
        
        /// <summary>
        /// Reads input from the user
        /// </summary>
        /// <returns>The string that the user wrote</returns>
        public static string ReadLine() =>
            _console.ReadLine();
        #endregion

        #region AccountRepository
        private static readonly global::ConsoleSample.InjectedClasses.AccountRepository _accountRepository = new();

        /// <summary>
        /// Retrieves all the <see cref="Account"/>s in the repository.
        /// </summary>
        public static global::System.Collections.Generic.List<global::ConsoleSample.InjectedClasses.Account> GetAllAccounts() =>
            _accountRepository.GetAllAccounts();
        #endregion
    }
}
```
