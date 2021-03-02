
using CSRobot.GenerateEntityTools.Entity;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace CSRobot.GenerateEntityTools.Builders
{

    public class CSharpBuilder : IBuilder
    {
        Dictionary<string, string> _typeMap;
        public void Build(DataBase database, CommandOptions options)
        {
            //取输出路径
            var basePath = GetOut(options, database.DataBaseName);

            var template = GetTamplate(options);

            _typeMap = GetMapTypes(options)[$"{options["--dbtype"].ToLower()}-{template.Extension.TrimStart('.')}"];
            //生成独立的表
            if (options.ContainsKey("--table"))
            {
                var table = database.Tables.SingleOrDefault(s => s["tablename"].ToString() == options["--table"]);
                if (table != null)
                {
                    var codeString = GetCodeString(database.DataBaseName, table, template.Template);
                    File.WriteAllText($"{basePath}/{table["tablename"]}{template.Extension}", codeString.ToString(), Encoding.UTF8);
                }
                else
                {
                    throw new ApplicationException($"找不到{options["--table"]}表");
                }
            }
            else
            {
                //生成所有表实体类
                foreach (var table in database.Tables)
                {
                    var filePath = $"{basePath}/{table["tablename"]}{template.Extension}";
                    if (File.Exists(filePath))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{filePath}已存在");
                        Console.ResetColor();

                        Console.WriteLine("覆盖请按 Y(y)，否则请按其他键：");
                        if (Console.ReadLine().ToLower() == "y")
                        {
                            var codeString = GetCodeString(database.DataBaseName, table, template.Template);
                            File.WriteAllText(filePath, codeString.ToString(), Encoding.UTF8);
                        }
                    }
                    else
                    {
                        var codeString = GetCodeString(database.DataBaseName, table, template.Template);
                        File.WriteAllText(filePath, codeString.ToString(), Encoding.UTF8);
                    }
                }
            }
        }
        /// <summary>
        /// 模板替换
        /// </summary>d
        /// <param name="dataBaseName"></param>
        /// <param name="table"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        private string GetCodeString(string dataBaseName, Dictionary<string, object> table, string template)
        {

            var matchs = Regex.Matches(template, @"(?<=\$\{)[\w]+(?=\})");
            foreach (Match item in matchs)
            {
                if (item.Value.ToLower() == "fields")
                {
                    continue;
                }
                if (table.ContainsKey(item.Value))
                {
                    template = template.Replace($"${{{item.Value}}}", table[item.Value].ToString());
                }
                // Console.WriteLine(item.Value);
            }

            //template = template.Replace("${DataBaseName}", dataBaseName);
            //template = template.Replace("${TableDescribe}", table.TableDescribe);
            //template = template.Replace("${TableName}", table.TableName);

            var match = Regex.Match(template, @"(?<=\$\{Fields\})[\w\W]+(?=\$\{Fields\})");
            if (match.Success)
            {
                var reg = new Regex(@"(?<=\$\{Fields\})[\w\W]+(?=\$\{Fields\})");
                return reg.Replace(template, GetFieldString(table, match.Value.Trim(' ')).Trim()).Replace("${Fields}", "");
            }
            return template;
        }

        /// <summary>
        /// 处理属性
        /// </summary>
        /// <param name="table"></param>
        /// <param name="fieldTamplate"></param>
        /// <returns></returns>
        private string GetFieldString(Dictionary<string, object> table, string fieldTamplate)
        {
            var fields = new StringBuilder();

            foreach (var field in table["fields"] as IEnumerable<Dictionary<string, object>>)
            {
                //把模板分成行，分别处理
                var lines = fieldTamplate.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                var newFieldTamplate = new StringBuilder();
                foreach (var line in lines)
                {
                    var newLine = line;
                    if (newLine.Trim().StartsWith("$?{"))
                    {
                        var match = Regex.Match(newLine, @"(?<=\$\?\{)[\w]+(?=\})");
                        if (match.Success)
                        {
                            //todo 这里要变
                            var valueResult = field.GetType().GetProperty(match.Value).GetValue(field);
                            if (valueResult != null && valueResult.ToString() != "")
                            {
                                newLine = newLine.Replace($"$?{{{match.Value}}}", "");
                                newFieldTamplate.AppendLine(newLine);
                            }
                        }
                    }
                    else
                    {
                        newFieldTamplate.AppendLine(newLine);
                    }
                }
                var fieldContent = newFieldTamplate.ToString();
                foreach (var pro in field.GetType().GetProperties())
                {
                    //todo 这里要变
                    if (pro.Name != "DBType")
                    {
                        fieldContent = fieldContent.Replace($"${{{pro.Name}}}", pro.GetValue(field)?.ToString());
                    }
                    else
                    {
                        fieldContent = fieldContent.Replace("${DBType}", _typeMap[field["DBType"].ToString()]);
                    }
                }
                fields.AppendLine(fieldContent);
            }
            return fields.ToString();
        }
        /// <summary>
        /// 处理输出路径
        /// </summary>
        /// <param name="options"></param>
        /// <param name="dataBaseName"></param>
        /// <returns></returns>
        private string GetOut(CommandOptions options, string dataBaseName)
        {
            if (options.ContainsKey("--out"))
            {
                return options["--out"];
            }
            else
            {
                var basePath = $"{Directory.GetCurrentDirectory()}/{dataBaseName}";
                Directory.CreateDirectory(basePath);
                return basePath;
            }
        }
        /// <summary>
        /// 处理模板
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private (string Template, string Extension) GetTamplate(CommandOptions options)
        {
            var template = @"
using System;

namespace ${DataBaseName}
{
    /// <summary>
    /// ${TableDescribe}
    /// </summary>
    public class ${TableName}
    {
        ${Fields}
        $?{FieldDescribe}/// <summary>
        $?{FieldDescribe}/// ${FieldDescribe}
        $?{FieldDescribe}/// </summary>
        $?{FieldSize}[BField(Length=${FieldSize},Name=""${FieldName}"")]
        public ${DBType} ${FieldName}
        { get; set; }
        ${Fields}
    }
}
";
            if (options.ContainsKey("--tep"))
            {
                var path = options["--tep"].ToLower();
                if (path == "cs")
                {
                    return (template, ".cs");
                }

                if (path.StartsWith("http"))
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Get, path);
                    var response = client.SendAsync(request).Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {

                        return (response.Content.ReadAsStringAsync().Result, Path.GetExtension(path));
                    }
                    else
                    {
                        throw new ApplicationException("获取模版失败");
                    }
                }
                else
                {
                    return (File.ReadAllText(path, Encoding.UTF8), Path.GetExtension(path));
                }
            }
            else
            {
                return (template, ".cs");
            }
        }



        private Dictionary<string, Dictionary<string, string>> GetMapTypes(CommandOptions options)
        {
            if (options.ContainsKey("--map"))
            {
                var path = options["--map"];
                if (path.StartsWith("http"))
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Get, path);
                    var response = client.SendAsync(request).Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var mapJson = response.Content.ReadAsStringAsync().Result;
                        return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(mapJson);
                    }
                    else
                    {
                        throw new ApplicationException("获取映射类型失败");
                    }
                }
                else
                {
                    var mapJson = File.ReadAllText(path, Encoding.UTF8);
                    return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(mapJson);
                }
            }
            else
            {
                var typeMap = new Dictionary<string, Dictionary<string, string>>()
                {
                    {"PostgreSqlToCS",new Dictionary<string, string>
                    {
                        {"bigint","long" },
                        {"bigserial" ,"long" },
                        {"bit","bool" },
                        {"boolean","bool" },
                        {"bool","bool" },
                        {"character","string" },
                        {"cidr","string" },
                        {"date","DateTime" }  ,
                        {"double","double" }  ,
                        {"inet","string" }  ,
                        {"int4","int" } ,
                        {"integer","int" } ,
                        {"macaddr","string" } ,
                        {"money","decimal" },
                        {"numeric","decimal" } ,
                        {"real","float" },
                        {"smallint","short" },
                        {"smallserial","short" },
                        {"serial","int" },
                        {"text","string" },
                        {"varchar","string" },
                        {"time","DateTime" } ,
                        {"timestamp","DateTime" },
                        {"tsquery","string" },
                        {"tsvector","string" },
                        {"txid_snapshot","string" },
                        {"uuid","string" } ,
                        {"xml","string" } ,
                        {"json","string" },
                    }},
                    {"MySqlToCS",new Dictionary<string, string>
                    {
                        {"char","char" },
                        {"varchar","string" },
                        {"tinytext","string" },
                        {"text","string" },
                        {"blob","string" },
                        {"mediumtext","string" },
                        {"mediumblob","string" },
                        {"longblob","string" },
                        {"longtext","string" },
                        {"tinyint","short" },
                        {"smallint","short" },
                        {"mediumint","short" },
                        {"int","int" },
                        {"bigint","long" },
                        {"float","float" },
                        {"double","double" },
                        {"decimal","decimal" },
                        {"date","DateTime" },
                        {"datetime","DateTime" },
                        {"timestamp","string" },
                        {"time","DateTime" },
                        {"boolean","bool" },
                    }},
                    {"MsSqlToCS", new Dictionary<string, string>
                    {
                        { "char", "string" },
                        { "varchar", "string" },
                        { "text", "string" },
                        { "nchar", "string" },
                        { "nvarchar", "string" },
                        { "ntext", "string" },
                        { "bit", "bool" },
                        { "binary", "string" },
                        { "varbinary", "string" },
                        { "image", "string" },
                        { "tinyint", "byte" },
                        { "smallint", "short" },
                        { "int", "int" },
                        { "bigint", "long" },
                        { "decimal", "decimal" },
                        { "numeric", "decimal" },
                        { "smallmoney", "decimal" },
                        { "money", "decimal" },
                        { "float", "float" },
                        { "real", "double" },
                        { "datetime", "DateTime" },
                        { "datetime2", "DateTime" },
                        { "smalldatetime", "DateTime" },
                        { "date", "DateTime" },
                        { "time", "DateTime" },
                        { "datetimeoffset", "string" },
                        { "timestamp", "string" }
                    }}
                };
                return typeMap;
            }
        }

    }
}