using CSRebot.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRebot.LanguageBuilder
{

    public class CSharpBuilder : IBuilder
    {
        public void Build(DataBase database, CommandOptions options)
        {
            var basePath = $"{Directory.GetCurrentDirectory()}/{database.DataBaseName}";
            Directory.CreateDirectory(basePath);
            foreach (var table in database.Tables)
            {
                var codeString = GetCodeString(database.DataBaseName, table);
                File.WriteAllText($"{basePath}/{table.TableName}.cs", codeString.ToString(), Encoding.UTF8);
            }
        }

        private string GetCodeString(string dataBaseName, Table table)
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
        public ${DBType} ${FieldName}
        { get; set; }
        ${Fields}
    }
}
";
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