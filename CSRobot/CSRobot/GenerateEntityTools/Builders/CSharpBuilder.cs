
using CSRobot.GenerateEntityTools.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace CSRobot.GenerateEntityTools.Builders
{

    public class CSharpBuilder : IBuilder
    {
        public void Build(DataBase database, CommandOptions options)
        {
            //取输出路径
            var basePath = GetOut(options, database.DataBaseName);

            var template = GetTamplate(options);
            //生成独立的表
            if (options.ContainsKey("--table"))
            {
                var table = database.Tables.SingleOrDefault(s => s.TableName == options["--table"]);
                if (table != null)
                {
                    var codeString = GetCodeString(database.DataBaseName, table, template.Template);
                    File.WriteAllText($"{basePath}/{table.TableName}{template.Extension}", codeString.ToString(), Encoding.UTF8);
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
                    var filePath = $"{basePath}/{table.TableName}{template.Extension}";
                    if (File.Exists(filePath))
                    {
                        Console.WriteLine($"{filePath}已存在，是否覆盖？Y为覆盖，N为不覆盖");
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
        /// </summary>
        /// <param name="dataBaseName"></param>
        /// <param name="table"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        private string GetCodeString(string dataBaseName, Table table, string template)
        {
            template = template.Replace("${DataBaseName}", dataBaseName);
            template = template.Replace("${TableDescribe}", table.TableDescribe);
            template = template.Replace("${TableName}", table.TableName);

            //fields循环
            var templateArr = template.Split("${Fields}");
            if (templateArr.Length < 3)
            {
                throw new ApplicationException("请检查${Fields}是否闭合");
            }
            templateArr[0] = templateArr[0].TrimEnd(' ');
            templateArr[1] = GetFieldString(table, templateArr[1]);
            return string.Join("", templateArr);
        }

        /// <summary>
        /// 处理属性
        /// </summary>
        /// <param name="table"></param>
        /// <param name="fieldTamplate"></param>
        /// <returns></returns>
        private string GetFieldString(Table table, string fieldTamplate)
        {
            var fields = new StringBuilder();

            foreach (var field in table.Fields)
            {
                //把模板分成行，分别处理
                var lines = fieldTamplate.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    var newLine = line;
                    //这里目前只实现了FieldSize的整行不输出
                    if (newLine.Trim().StartsWith("$?"))
                    {
                        if (field.FieldSize.HasValue)
                        {
                            newLine = newLine.Replace("${FieldSize}", field.FieldSize.Value.ToString()).Replace("$?", "");
                            fields.AppendLine(newLine);
                        }
                    }
                    else
                    {
                        newLine = newLine.Replace("${FieldDescribe}", field.FieldDescribe);
                        newLine = newLine.Replace("${DBType}", _typeMap[field.DBType]);
                        newLine = newLine.Replace("${FieldName}", field.FieldName);
                        if (field.FieldSize.HasValue)
                        {
                            newLine = newLine.Replace("${FieldSize}", field.FieldSize.Value.ToString());
                        }
                        fields.AppendLine(newLine);
                    }
                }
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
        /// <summary>
        /// ${FieldDescribe}
        /// </summary>
        $?[BField(Length=${FieldSize})]
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

        Dictionary<string, string> _typeMap;
        public CSharpBuilder(string dbType)
        {
            switch (dbType)
            {
                case "postgresql":
                    _typeMap = new Dictionary<string, string>
                    {
                        {"bigint","long" },//   int8    有符号8字节整数
                        {"bigserial" ,"long" },//   serial8 自增8字节整数
                        {"bit","bool" },//  [ (n) ]     定长位串            
                        {"boolean","bool" }, //bool    逻辑布尔值(真/假)
                        {"bool","bool" }, //bool    逻辑布尔值(真/假)
                        {"character","string" }, //varying [ (n) ]   varchar [ (n) ] 可变长字符串              
                        {"cidr","string" }, //      IPv4 或 IPv6 网络地址
                        {"date","DateTime" }  ,//        日历日期(年, 月, 日)
                        {"double","double" }  ,//  precision    float8  双精度浮点数(8字节)
                        {"inet","string" }  ,//       IPv4 或 IPv6 主机地址
                        {"int4","int" } , // int, int4   有符号 4 字节整数  
                        {"integer","int" } , // int, int4   有符号 4 字节整数  
                        {"macaddr","string" } , //    MAC (Media Access Control)地址
                        {"money","decimal" },  //      货币金额
                        {"numeric","decimal" } , //  [ (p, s) ]  decimal [ (p, s) ]  可选精度的准确数值数据类型
                        {"real","float" },  //    float4  单精度浮点数(4 字节)
                        {"smallint","short" },  //    int2    有符号 2 字节整数
                        {"smallserial","short" },  // serial2 自增 2 字节整数
                        {"serial","int" },  //   serial4 自增 4 字节整数
                        {"text","string" },  //        可变长字符串
                        {"varchar","string" },  //        可变长字符串
                        {"time","DateTime" } , //  [ (p) ] [ without time zone ]      一天中的时刻(无时区)
                       // {"time","DateTime" } , // [ (p) ] with time zone timetz  一天中的时刻，含时区
                        {"timestamp","DateTime" },  //  [ (p) ] [ without time zone ]     日期与时刻(无时区)
                       // {"timestamp","DateTime" },  // [ (p) ] with time zone    timestamptz 日期与时刻，含时区
                        {"tsquery","string" },  //    文本检索查询
                        {"tsvector","string" },  //      文本检索文档
                        {"txid_snapshot","string" },  //      用户级别的事务ID快照
                        {"uuid","string" } , //     通用唯一标识符
                        {"xml","string" } , //  XML 数据
                        {"json","string" },  //      JSON 数据
                    };
                    break;
                case "mysql":
                    _typeMap = new Dictionary<string, string>
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
                    };
                    break;
                case "mssql":
                    _typeMap = new Dictionary<string, string>
                    {
                        { "char", "string" },// 固定长度的字符串。最多 8,000 个字符。	n
                        { "varchar", "string" },//(n)  可变长度的字符串。最多 8,000 个字符。	 
                        { "text", "string" },//    可变长度的字符串。最多 2GB 字符数据。
                        { "nchar", "string" },//(n)    固定长度的 Unicode 数据。最多 4,000 个字符。	 
                        { "nvarchar", "string" },//(n) 可变长度的 Unicode 数据。最多 4,000 个字符。	 
                        { "ntext", "string" },//   可变长度的 Unicode 数据。最多 2GB 字符数据。
                        { "bit", "bool" },//  允许 0、1 或 NULL
                        { "binary", "string" },//(n)   固定长度的二进制数据。最多 8,000 字节。	 
                        { "varbinary", "string" },//(n)    可变长度的二进制数据。最多 8,000 字节。	 
                        { "image", "string" },//   可变长度的二进制数据。最多 2GB。
                        { "tinyint", "byte" },// 允许从 0 到 255 的所有数字。	1 字节
                        { "smallint", "short" },//    允许从 -32,768 到 32,767 的所有数字。	2 字节
                        { "int", "int" },// 允许从 -2,147,483,648 到 2,147,483,647 的所有数字。	4 字节
                        { "bigint", "long" },//  允许介于 -9,223,372,036,854,775,808 和 9,223,372,036,854,775,807 之间的所有数字。	8 字节
                        { "decimal", "decimal" },//
                        { "numeric", "decimal" },//
                        { "smallmoney", "decimal" },//  介于 -214,748.3648 和 214,748.3647 之间的货币数据。	4 字节
                        { "money", "decimal" },//   介于 -922,337,203,685,477.5808 和 922,337,203,685,477.5807 之间的货币数据。	8 字节
                        { "float", "float" },//   从 -1.79E + 308 到 1.79E + 308 的浮动精度数字数据。 参数 n 指示该字段保存 4 字节还是 8 字节。f
                        { "real", "double" },//    从 -3.40E + 38 到 3.40
                        { "datetime", "DateTime" },//    从 1753 年 1 月 1 日 到 9999 年 12 月 31 日，精度为 3.33 毫秒。	8 bytes
                        { "datetime2", "DateTime" },//   从 1753 年 1 月 1 日 到 9999 年 12 月 31 日，精度为 100 纳秒。	6-8 bytes
                        { "smalldatetime", "DateTime" },//   从 1900 年 1 月 1 日 到 2079 年 6 月 6 日，精度为 1 分钟。	4 bytes
                        { "date", "DateTime" },//    仅存储日期。从 0001 年 1 月 1 日 到 9999 年 12 月 31 日。	3 bytes
                        { "time", "DateTime" },//    仅存储时间。精度为 100 纳秒。	3-5 bytes
                        { "datetimeoffset", "string" },//  与 datetime2 相同，外加时区偏移。	8-10 bytes
                        { "timestamp", "string" }//   存储唯一的
                    };
                    break;
            }
        }
    }
}