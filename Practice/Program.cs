using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Practice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Program started ...");

            ReadConfigFile();

            Console.WriteLine("Program completed.");
        }

        private static void ReadConfigFile()
        {
            bool IsLocalSettingsRetrieved = true;
            ReadOnlyDictionary<string, string> LocalConfigs;

            string configFileName = $"Configuration.edog.json";
            string configurationFile = Path.Combine("E:\\MediaTransformAndAnalysis\\container_build_artifacts\\docker_drop\\MeTAServiceWorkerRole\\config", configFileName);

            try
            {
                string jsonConfig = File.ReadAllText(configurationFile);

                Dictionary<string, string> localConfigs = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonConfig);
                LocalConfigs = new ReadOnlyDictionary<string, string>(localConfigs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LocalSettings.TryInitialize() - Exception loading configurations from file {configurationFile}. Exception {ex}");
                IsLocalSettingsRetrieved = false;
                //throw;
            }

            Console.WriteLine(IsLocalSettingsRetrieved);
        }
    }
}
