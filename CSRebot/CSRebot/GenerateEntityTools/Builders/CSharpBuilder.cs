
using CSRebot.GenerateEntityTools.Entity;
using Org.BouncyCastle.Crypto.Modes.Gcm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CSRebot.GenerateEntityTools.Builders
{

    public class CSharpBuilder : IBuilder
    {
        public void Build(DataBase database, CommandOptions options)
        {
            //取模版
            var template = "";
            if (!string.IsNullOrEmpty(options["--tep"]))
            {
                template = GetTamplate(options["--tep"]);
            }
            //取输出路径
            var basePath = "";
            if (options.ContainsKey("--out"))
            {
                basePath = options["--out"];
            }
            else
            {
                basePath = $"{Directory.GetCurrentDirectory()}/{database.DataBaseName}";
                Directory.CreateDirectory(basePath);
            }
            //生成独立的表
            if (options.ContainsKey("--table"))
            {
                var table = database.Tables.SingleOrDefault(s => s.TableName == options["--table"]);
                if (table != null)
                {
                    var codeString = GetCodeString(database.DataBaseName, table, template);
                    File.WriteAllText($"{basePath}/{table.TableName}.cs", codeString.ToString(), Encoding.UTF8);
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
                    var codeString = GetCodeString(database.DataBaseName, table, template);
                    File.WriteAllText($"{basePath}/{table.TableName}.cs", codeString.ToString(), Encoding.UTF8);
                }
            }
        }

        private string GetCodeString(string dataBaseName, Table table, string template = null)
        {
            if (string.IsNullOrEmpty(template))
            {
                template = @"
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
        public ${DBType} ${FieldName}
        { get; set; }
        ${Fields}
    }
}
";
            }
            template = template.Replace("${DataBaseName}", dataBaseName);
            template = template.Replace("${TableDescribe}", table.TableDescribe);
            template = template.Replace("${TableName}", table.TableName);

            //fields循环
            var templateArr = template.Split("${Fields}");
            if (templateArr.Length < 3)
            {
                throw new ApplicationException("请检查${Fields}是否闭合");
            }
            templateArr[1] = GetFieldString(table, templateArr[1]);
            return string.Join("", templateArr);
        }

        private string GetFieldString(Table table, string fieldTamplate)
        {
            var fields = new StringBuilder();

            foreach (var field in table.Fields)
            {
                var fieldTamplateValue = fieldTamplate;
                fieldTamplateValue = fieldTamplateValue.Replace("${FieldDescribe}", field.FieldDescribe);
                fieldTamplateValue = fieldTamplateValue.Replace("${DBType}", _typeMap[field.DBType]);
                fieldTamplateValue = fieldTamplateValue.Replace("${FieldName}", field.FieldName);
                fields.Append(fieldTamplateValue);
            }
            return fields.ToString();
        }


        public string GetTamplate(string path)
        {
            if (path.StartsWith("http"))
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, path);
                var response = client.SendAsync(request).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return response.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    throw new ApplicationException("获取模版失败");
                }
            }
            else
            {
                return File.ReadAllText(path, Encoding.UTF8);
            }
        }

        Dictionary<string, string> _typeMap;
        public CSharpBuilder()
        {
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
        }
    }


}