using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRobot.GenerateEntityTools.Traversers
{
    static class Common
    {
        internal static string GetConnectionString()
        {
            var path = Directory.GetCurrentDirectory();
            var result = GetFile(path);
            if (result != "")
            {
                return result;
            }
            string GetFile(string path)
            {
                foreach (var file in Directory.GetFiles(path))
                {
                    if (file.ToLower().Contains("appsettings.development.json"))
                    {
                        var result = ReadConnectionString(file);
                        if (!string.IsNullOrEmpty(result))
                        {
                            return result;
                        }
                    }
                    if (file.ToLower().Contains("appsettings.staging.json"))
                    {
                        var result = ReadConnectionString(file);
                        if (!string.IsNullOrEmpty(result))
                        {
                            return result;
                        }
                    }
                    if (file.ToLower().Contains("appsettings.json"))
                    {
                        var result = ReadConnectionString(file);
                        if (!string.IsNullOrEmpty(result))
                        {
                            return result;
                        }
                    }
                }
                return "";
            }
            foreach (var dir in Directory.GetDirectories(path))
            {
                result = GetFile(dir);
                if (result != "")
                {
                    return result;
                }
            }
            return "";
            string ReadConnectionString(string file)
            {
                var json = File.ReadAllText(file, Encoding.UTF8);
                var configurations = System.Text.Json.JsonSerializer.Deserialize<Configuration>(json);
                if (configurations.ConnectionStrings != null && configurations.ConnectionStrings.Count > 0)
                {
                    return configurations.ConnectionStrings.Values.FirstOrDefault();
                }
                return "";
            }
        }
        class Configuration
        {
            public Dictionary<string, string> ConnectionStrings { get; set; }
        }
    }
}
