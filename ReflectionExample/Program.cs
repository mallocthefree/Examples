using System.Threading;
using RootLibrary;

namespace ReflectionExample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ExampleFactory.LoadAllExamples();
            ExampleFactory.StartAllExamples();

            Thread.Sleep(1000);

            ExampleFactory.ShutdownAllExamples();
        }
    }
}
