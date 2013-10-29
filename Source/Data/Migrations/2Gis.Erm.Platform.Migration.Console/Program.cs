using System;

namespace DoubleGis.Erm.Platform.Migration.Console
{
    internal static class Program
    {
        internal static void Main(string[] args)
        {
            try
            {
                new MigrationConsole(args);
            }
            catch (ArgumentException ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }
    }
}