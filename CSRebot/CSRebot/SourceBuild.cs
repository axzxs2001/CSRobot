using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRebot
{

    public interface SourceBuild
    {
        void Build(EntityDirectory entityDirectory);
    }

    public class CSharpBuild : SourceBuild
    {
        public void Build(EntityDirectory entityDirectory)
        {
            var basePath = $"{Directory.GetCurrentDirectory()}/{entityDirectory.EntityDirectoryName}";
            Directory.CreateDirectory(basePath);
            foreach (var entity in entityDirectory.Entities)
            {
                var codeString = new StringBuilder();
                codeString.AppendLine(@$"using System;

namespace {entityDirectory.EntityDirectoryName}
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
}
