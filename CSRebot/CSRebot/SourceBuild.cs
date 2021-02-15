﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRebot
{

    public interface ILanguageBuilder
    {
        void Build(EntityHub entityHub, IDictionary<string, string> options);
    }

    public class CSharpBuilder : ILanguageBuilder
    {
        public void Build(EntityHub entityHub, IDictionary<string, string> options)
        {
            var basePath = $"{Directory.GetCurrentDirectory()}/{entityHub.EntityDirectoryName}";
            Directory.CreateDirectory(basePath);
            foreach (var entity in entityHub.Entities)
            {
                var codeString = new StringBuilder();
                codeString.AppendLine(@$"using System;

namespace {entityHub.EntityDirectoryName}
{{
    {(string.IsNullOrEmpty(entity.EntityDescribe) ? "" : @$"/// <summary>
    /// {entity.EntityDescribe}
    /// </summary>")}
    public class {entity.EntityName}
    {{");

                foreach (var field in entity.Fields)
                {
                    codeString.AppendLine(@$"        {(string.IsNullOrEmpty(field.FieldDescribe) ? "" : @$"/// <summary>
        /// {field.FieldDescribe}
        /// </summary>")}
        public string {field.FieldName}
        {{ get; set; }}");
                }
                codeString.AppendLine("    }");
                codeString.AppendLine("}");
                codeString.AppendLine();

                File.WriteAllText($"{basePath}/{entity.EntityName}.cs", codeString.ToString(), Encoding.UTF8);
            }
        }
    }

    public class GoBuilder : ILanguageBuilder
    {
        public void Build(EntityHub entityHub, IDictionary<string, string> options)
        {
            throw new NotImplementedException();
        }
    }
}
