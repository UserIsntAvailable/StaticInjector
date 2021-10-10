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
        public void WriteLine(string value, int amount)
        {
            for(var i = 0; i < amount; i++)
            {
                System.Console.WriteLine(value);
            }
        }

        public string? ReadLine()
        {
            return System.Console.ReadLine();
        }
    }

    internal class Logger
    {
        public void Log(string value)
        {
            System.Console.WriteLine(value);
        }

        public void LogLine(string value)
        {
            this.Log($"{value}\n");
        }
    }
}
