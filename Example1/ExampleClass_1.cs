using System;
using System.Reflection;
using RootLibrary;

namespace Example1
{
    public class ExampleClass_1 : IExampleRoot
    {
        public string GetName()
        {
            return "ExampleClass_1";
        }

        public void Run()
        {
            Console.WriteLine($"{GetName()}:Run() from library {Assembly.GetExecutingAssembly().FullName} is processing");
        }

        public void Startup()
        {
            Console.WriteLine($"Starting {GetName()} from library {Assembly.GetExecutingAssembly().FullName}");
        }

        public void Shutdown()
        {
            Console.WriteLine($"Stopping {GetName()} from library {Assembly.GetExecutingAssembly().FullName}");
        }
    }
}
