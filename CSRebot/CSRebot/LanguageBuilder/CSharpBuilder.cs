using CSRebot.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRebot.LanguageBuilder
{

    public class CSharpBuilder : ILanguageBuilder
    {
        public void Build(DataBase database, IDictionary<string, string> options)
        {
            var basePath = $"{Directory.GetCurrentDirectory()}/{database.DataBaseName}";
            Directory.CreateDirectory(basePath);
            foreach (var table in database.Tables)
            {
                var codeString = new StringBuilder();
                codeString.AppendLine(@$"using System;

namespace {database.DataBaseName}
{{
    {(string.IsNullOrEmpty(table.TableDescribe) ? "" : @$"/// <summary>
    /// {table.TableDescribe}
    /// </summary>")}
    public class {table.TableName}
    {{");

                foreach (var field in table.Fields)
                {
                    codeString.AppendLine(@$"        {(string.IsNullOrEmpty(field.FieldDescribe) ? "" : @$"/// <summary>
        /// {field.FieldDescribe}
        /// </summary>")}
        public {(_typeMap.ContainsKey(field.DBType) ? _typeMap[field.DBType] : "string")} {field.FieldName}
        {{ get; set; }}");
                }
                codeString.AppendLine("    }");
                codeString.AppendLine("}");
                codeString.AppendLine();

                File.WriteAllText($"{basePath}/{table.TableName}.cs", codeString.ToString(), Encoding.UTF8);
            }
        }

        Dictionary<string, string> _typeMap;
        public CSharpBuilder()
        {
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
        }
    }


}





