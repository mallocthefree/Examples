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
            // Get the values listed in the config file within the section labeled <examples>
            var exampleHandlers = ConfigExamples;

            if (exampleHandlers == null)
            {
                // Failed to find the <examples> configuration block (or config file)
                return;
            }

            foreach (string key in exampleHandlers.AllKeys)
            {
                // Force a pre-cache of all of the listed config items
                GetExample(key);
            }
        }

        /// <summary>
        /// Startup all loaded handlers
        /// </summary>
        public static void StartAllExamples()
        {
            // Load configurations and make sure we have a cached copy of each
            LoadAllExamples();

            // Run Startup() on each successfully loaded and cached instantiation of IExampleRoot
            foreach (var example in ExampleDictionary.Values)
            {
                example.Startup();
            }
        }

        /// <summary>
        /// Run the shutdown process for every dynamically loaded Example library
        /// </summary>
        public static void ShutdownAllExamples()
        {
            // Run Shutdown() on each of the loaded IExampleRoot instantiated classes
            foreach (var example in ExampleDictionary.Values)
            {
                example.Shutdown();
            }

            // Empty the cached instantiations
            ExampleDictionary.Clear();
        }

        /// <summary>
        /// Get an Example by name
        /// </summary>
        /// <param name="module">name of the Example module to get, as identified by the key in the config file</param>
        /// <returns>Instantiation of an IExampleRoot concrete class</returns>
        public static IExampleRoot GetExample(string module)
        {
            var example = ConfigExamples;
            if (example == null)
            {
                return null;
            }

            // Get the string definition of the class to look for
            string moduleType = example.Get(module);

            // Get the class as a Type
            Type type = Type.GetType(moduleType);
            return GetExample(type);
        }

        /// <summary>
        /// Get the example by the class type (eg, ExampleClass_1)
        /// If it was not yet instantiated, then do so during this call and cache the result
        /// </summary>
        /// <param name="type">Type of class to retrieve from cached instantiations</param>
        /// <returns>Instantiation of an IExampleRoot concrete class</returns>
        public static IExampleRoot GetExample(Type type)
        {
            if (type == null)
            {
                return null;
            }

            // Lock the dictionary for multi-threading to make sure we don't add while reading
            lock (ExampleDictionary)
            {
                // Check cache to see if we already instantiated one
                if (ExampleDictionary.ContainsKey(type))
                {
                    // Existence in cache found, so retrieve it
                    return ExampleDictionary[type];
                }

                // Instantiate this class as type
                if (!(Activator.CreateInstance(type) is IExampleRoot example))
                {
                    // The class was not able to correctly be identified by code or library.
                    // The configuration should be validated and the folder checked to ensure
                    // the file exists as identified in the config file
                    return null;
                }

                Console.WriteLine($"Example [{type}] loaded");

                // New instantiation successfully created, so cache it for future calls
                ExampleDictionary.Add(type, example);
                return example;
            }
        }
    }
}

