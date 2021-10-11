using System.Collections.Generic;
using ConsoleSample.InjectedClasses;
using StaticInjector;

namespace ConsoleSample
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            App.WriteLine("Called From Generated File", 2);
        }
    }

    [InjectStatic(typeof(Console))]
    [InjectStatic(typeof(AccountRepository))]
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
        public void WriteLine(string value, int amount = 1)
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

    internal class AccountRepository
    {
        private readonly List<Account?> _accounts = new();

        /// <summary>
        /// Retrieves all the <see cref="Account"/>s in the repository.
        /// </summary>
        public List<Account?> GetAllAccounts() => _accounts;
    }

    public record Account(string FirstName, string LastName);
}
