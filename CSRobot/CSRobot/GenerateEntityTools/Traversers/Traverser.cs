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

        protected string ConnectionString { get; set; }
        protected string FieldSQL { get; set; }

        protected string TableSQL { get; set; }
        public Traverser(CommandOptions options)
        {

            if (options.ContainsKey("--constr"))
            {
                ConnectionString = options["--constr"];
            }
            else
            {
                if (!options.ContainsKey("--host")||!options.ContainsKey("--db")||!options.ContainsKey("--user")||!options.ContainsKey("--pwd"))
                {
                    IsExistOption = false;                   
                }
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
