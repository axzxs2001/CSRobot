
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
            var basePath = GetOut(options);

            var (Template, Extension) = GetTamplate(options);

            _typeMap = GetMapTypes(options)[$"{options["--dbtype"].ToLower()}-{Extension.TrimStart('.')}"];
            //生成独立的表
            if (options.ContainsKey("--table"))
            {
                var table = database.Tables.SingleOrDefault(s => s["tablename"].ToString() == options["--table"]);
                if (table != null)
                {
                    var codeString = GetCodeString(table, Template);
                    File.WriteAllText($"{basePath}/{table["tablename"]}{Extension}", codeString.ToString(), Encoding.UTF8);
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
                    var filePath = $"{basePath}/{table["tablename"]}{Extension}";
                    if (File.Exists(filePath))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{filePath}已存在");
                        Console.ResetColor();

                        Console.WriteLine("覆盖请按 Y(y)，否则请按其他键：");
                        if (Console.ReadLine().ToLower() == "y")
                        {
                            var codeString = GetCodeString(table, Template);
                            File.WriteAllText(filePath, codeString.ToString(), Encoding.UTF8);
                        }
                    }
                    else
                    {
                        var codeString = GetCodeString(table, Template);
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
        private string GetCodeString(Dictionary<string, object> table, string template)
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
                    template = template.Replace($"${{{item.Value}}}", table[item.Value].ToString(), StringComparison.OrdinalIgnoreCase);
                }
            }
            var match = Regex.Match(template, @"(?<=\$\{Fields\})[\w\W]+(?=\$\{Fields\})");
            if (match.Success)
            {
                var reg = new Regex(@"(?<=\$\{Fields\})[\w\W]+(?=\$\{Fields\})");
                return reg.Replace(template, GetFieldString(table, match.Value.Trim(' ')).Trim()).Replace("${Fields}", "", StringComparison.OrdinalIgnoreCase);
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
            var fieldBuild = new StringBuilder();

            foreach (var fields in table["fields"] as IEnumerable<Dictionary<string, object>>)
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
                            var valueResult = fields[match.Value.ToLower()];
                            if (valueResult != null && valueResult.ToString() != "")
                            {
                                newLine = newLine.Replace($"$?{{{match.Value}}}", "", StringComparison.OrdinalIgnoreCase);
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
                foreach (var field in fields)
                {
                    var match = Regex.Match(fieldContent, @"(?<=\$map\{)[\w]+(?=\})");
                    if (match.Success && match.Value == field.Key)
                    {
                        if (_typeMap.ContainsKey(field.Value.ToString()))
                        {
                            fieldContent = fieldContent.Replace($"$map{{{field.Key}}}", _typeMap[field.Value.ToString()], StringComparison.OrdinalIgnoreCase);
                        }
                        else
                        {
                            fieldContent = fieldContent.Replace($"$map{{{field.Key}}}", "object", StringComparison.OrdinalIgnoreCase);
                        }
                    }
                    else
                    {
                        fieldContent = fieldContent.Replace($"${{{field.Key}}}", field.Value.ToString(), StringComparison.OrdinalIgnoreCase);
                    }
                }
                fieldBuild.AppendLine(fieldContent);
            }
            return fieldBuild.ToString();
        }
        /// <summary>
        /// 处理输出路径
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static string GetOut(CommandOptions options)
        {
            if (options.ContainsKey("--out"))
            {
                return options["--out"];
            }
            else
            {
                var basePath = $"{Directory.GetCurrentDirectory()}/entities";
                Directory.CreateDirectory(basePath);
                return basePath;
            }
        }
        /// <summary>
        /// 处理模板
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static (string Template, string Extension) GetTamplate(CommandOptions options)
        {
            var template = @"
using System;

namespace MyNameSpace
{
    /// <summary>
    /// ${tabledescribe}
    /// </summary>
    public class ${tablename}
    {
        ${Fields}
        $?{fielddescribe}/// <summary>
        $?{fielddescribe}/// ${fielddescribe}
        $?{fielddescribe}/// </summary>
        $?{fieldsize}[BField(Length=${fieldsize},Name=""${fieldname}"")]
        public $map{dbtype} ${fieldname}
        { get; set; }
        ${Fields}
    }
}
";
            if (options.ContainsKey("--tep"))
            {
                var path = options["--tep"];
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



        private static Dictionary<string, Dictionary<string, string>> GetMapTypes(CommandOptions options)
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
                    {"postgresql-cs",new Dictionary<string, string>
                    {
                        {"bigint","long"},
                        {"int8","long"},
                        {"bigserial","long"},
                        {"serial8","long"},
                        {"boolean","bool"},
                        {"bool","bool"},
                        {"bytea","Byte[]"},
                        {"character","string"},
                        {"char","string"},
                        {"character varying","string"},
                        {"varchar","string"},
                        {"date","DateTime"},
                        {"double precision","double"},
                        {"float8","double"},
                        {"integer","int"},
                        {"int4","int"},
                        {"interval","string"},
                        {"money","decimal"},
                        {"numeric","decimal"},
                        {"decimal","decimal"},
                        {"real","float"},
                        {"float4","float"},
                        {"smallint","short"},
                        {"int2","short"},
                        {"smallserial","short"},
                        {"serial2","short"},
                        {"serial","int"},
                        {"serial4","int"},
                        {"text","string"},
                        {"time","DateTime"},
                        {"time with time zone","DateTimeOffset"},
                        {"timetz","DateTimeOffset"},
                        {"timestamp","DateTime"},
                        {"timestamp with time zone","DateTimeOffset"},
                        {"timestamptz","DateTimeOffset"},
                        {"uuid","GUID"}
                    }},
                    {"mysql-cs",new Dictionary<string, string>
                    {
                        {"char","string"},
                        {"varchar","string"},
                        {"binary","byte[]"},
                        {"varbinary","byte[]"},
                        {"tinyblob","btye[]"},
                        {"tinytext","string"},
                        {"text","string"},
                        {"blob","byte[]"},
                        {"mediumtext","string"},
                        {"mediumblob","byte[]"},
                        {"longtext","string"},
                        {"longblob","byte[]"},
                        {"enum","string"},
                        {"bit","short"},
                        {"tinyint","byte"},
                        {"bool","bool"},
                        {"boolean","bool"},
                        {"smallint","short"},
                        {"mediumint","short"},
                        {"int","int32"},
                        {"integer","int32"},
                        {"bigint","long"},
                        {"float","float"},                      
                        {"double","double"},
                        {"double precision","double"},
                        {"decimal","decimal"},
                        {"dec","decimal"},
                        {"date","datetime"},
                        {"datetime","datetime"},
                        {"timestamp","datetime"},
                        {"time","datetime"},
                        {"year","short"}
                    }},
                    {"mssql-cs", new Dictionary<string, string>
                    {
                        {"bigint","long" },
                        {"binary","Byte[]" },
                        {"bit","bool" },
                        {"char","string" },
                        {"date","DateTime" },
                        {"datetime","DateTime" },
                        {"datetime2","DateTime" },
                        {"datetimeoffset","DateTimeOffset" },
                        {"decimal", "decimal" },
                        {"float","double" },
                        {"image","Byte[]" },
                        {"int","int" },
                        {"money","decimal" },
                        {"nchar","string" },
                        {"ntext","string" },
                        {"numeric","double" },
                        {"nvarchar","string" },
                        {"real","float" },
                        {"rowversion","Byte[]" },
                        {"smalldatetime","DateTime" },
                        {"smallint","short" },
                        {"smallmoney","decimal" },
                        {"text","string" },
                        {"time","TimeSpan" },
                        {"timestamp","Byte[]" },
                        {"tinyint","Byte" },
                        {"uniqueidentifier","Guid" },
                        {"varbinary","Byte[]" },
                        {"varchar","string" },
                        {"xml","string" }
                    }}
                };
                return typeMap;
            }
        }

    }

}