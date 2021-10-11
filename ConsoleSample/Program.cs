using ConsoleSample.InjectedClasses;
using InjectStatic;

namespace ConsoleSample
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            App.Log("Called From Generated File");
        }
    }

    [InjectStatic(typeof(Console))]
    [InjectStatic(typeof(Logger))]
    public static partial class App
    {
    }
}

namespace ConsoleSample.InjectedClasses
{
    internal class Console
    {
        /// <summary>
        /// Writes a line to the console
        /// </summary>
        /// <param name="value">The value that will be printed at the console</param>
        /// <param name="amount">The amount of times it will be printed</param>
        public void WriteLine(string value, int amount)
        {
            for(var i = 0; i < amount; i++)
            {
                System.Console.WriteLine(value);
            }
        }

        /// <summary>
        /// Reads input from the user
        /// </summary>
        /// <returns>The string that the user wrote</returns>
        public string? ReadLine()
        {
            return System.Console.ReadLine();
        }
    }

    internal class Logger
    {
        /// <summary>
        /// Logs something to the console 
        /// </summary>
        public void Log(string value)
        {
            System.Console.Write(value);
        }

        public void LogLine(string value)
        {
            this.Log($"{value}\n");
        }
    }
}
