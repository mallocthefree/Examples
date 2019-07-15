using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;


namespace RootLibrary
{
    public class ExampleFactory
    {
        private static IDictionary<Type, IExampleRoot> ExampleDictionary { get; } = new Dictionary<Type, IExampleRoot>();

        private static NameValueCollection ConfigExamples => ConfigurationManager.GetSection("examples") as NameValueCollection;

        /// <summary>
        /// Load all configured examples, but do not start them
        /// </summary>
        public static void LoadAllExamples()
        {
            var exampleHandlers = ConfigExamples;
            if (exampleHandlers == null)
            {
                return;
            }

            foreach (string key in exampleHandlers.AllKeys)
            {
                GetExamples(key);
            }
        }

        /// <summary>
        /// Startup all loaded handlers
        /// </summary>
        public static void StartAllExamples()
        {
            LoadAllExamples();
            foreach (var example in ExampleDictionary.Values)
            {
                example.Startup();
            }
        }

        public static void ShutdownAllExamples()
        {
            foreach (var example in ExampleDictionary.Values)
            {
                example.Shutdown();
            }

            ExampleDictionary.Clear();
        }

        public static IExampleRoot GetExamples(string module)
        {
            var example = ConfigExamples;
            if (example == null)
            {
                return null;
            }

            string moduleType = example.Get(module);
            Type type = Type.GetType(moduleType);
            return GetExamples(type);
        }

        public static IExampleRoot GetExamples(Type type)
        {
            if (type == null)
            {
                return null;
            }

            lock (ExampleDictionary)
            {
                if (ExampleDictionary.ContainsKey(type))
                {
                    return ExampleDictionary[type];
                }

                if (!(Activator.CreateInstance(type) is IExampleRoot example))
                {
                    return null;
                }

                Console.WriteLine($"Example [{type}] loaded");
                ExampleDictionary.Add(type, example);
                return example;
            }
        }
    }
}

