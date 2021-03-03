using CSRobot.GenerateEntityTools.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace CSRobot.GenerateEntityTools.Traversers
{
    public abstract class Traverser : ITraverser
    {
        protected bool IsExistOption { get; set; } = true;

        protected string FieldSQL { get; set; }

        protected string TableSQL { get; set; }
        public Traverser(CommandOptions options)
        {
            if (!options.ContainsKey("--host"))
            {
                IsExistOption = false;
                Console.WriteLine("缺少 --host");
            }
            if (!options.ContainsKey("--db"))
            {
                IsExistOption = false;
                Console.WriteLine("缺少 --db");
            }
            if (!options.ContainsKey("--user"))
            {
                IsExistOption = false;
                Console.WriteLine("缺少 --user");
            }
            if (!options.ContainsKey("--pwd"))
            {
                IsExistOption = false;
                Console.WriteLine("缺少 --pwd");
            }
            LoadSQL(options);
        }

        private void LoadSQL(CommandOptions options)
        {
            if (options.ContainsKey("--sql"))
            {
                string json;
                var path = options["--sql"].ToLower();
                if (path.StartsWith("http"))
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Get, path);
                    var response = client.SendAsync(request).Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {

                        json = response.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        throw new ApplicationException("获取SQL模版失败");
                    }
                }
                else
                {
                    json = File.ReadAllText(path, Encoding.UTF8);
                }

                if (!string.IsNullOrEmpty(json))
                {
                    var sql = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    if (sql != null && sql.ContainsKey("fieldsql"))
                    {
                        FieldSQL = sql["fieldsql"];
                    }
                    if (sql != null && sql.ContainsKey("tablesql"))
                    {
                        TableSQL = sql["tablesql"];
                    }
                }
            }
        }
        public abstract DataBase Traverse();
    }
}
