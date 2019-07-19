using System.Threading;
using RootLibrary;

namespace ReflectionExample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Load all classes and instantiate each correctly defined class in the config file
            ExampleFactory.LoadAllExamples();

            // Run Startup() on all IExampleRoot defined class instantiations
            ExampleFactory.StartAllExamples();

            // Wait 1 second
            Thread.Sleep(1000);

            // Run Shutdown() on all IExampleRoot defined class instantiations
            ExampleFactory.ShutdownAllExamples();
        }
    }
}
