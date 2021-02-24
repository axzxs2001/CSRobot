using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSRebot.GenerateEntityTools.Traversers;
using CSRebot.GenerateEntityTools.Builders;
namespace CSRebot.GenerateEntityTools
{
    static class GenerateEntityTool
    {
        //todo 这里要实现多语言支持
        static bool GenHelp()
        {
            Console.WriteLine(@"csrebot gen -h
--constr  数据库连接字符串
--dbtype  数据库类型 database type,eg:--dbtype=mysql,--dbtype=mssql,--dbtype=postgressql
--table   gnerate the entity class of the specified table name
--to      entity type,eg:--to=cs,--to=json 
--out     generate entity folder
--tep     template source,eg:--tep=/usr/abc/,--tep=https://github.com/abc/bcd.cs
");
            return true;
        }
        internal static bool GenerateEntity(CommandOptions options)
        {
            if (options.ContainsKey("-h"))
            {
                GenHelp();
                return true;
            }

            ITraverser traverser = null;
            IBuilder builder = null;
            switch (options["--dbtype"].ToLower())
            {
                case "mysql":
                    traverser = new MySqlTraverser(options);
                    break;
                case "mssql":
                    traverser = new MsSqlTraverser(options);
                    break;
                case "postgresql":
                    traverser = new PostgreSqlTraverser(options);
                    break;
            }
            switch (options["--to"].ToLower())
            {
                case "cs":
                    builder = new CSharpBuilder(options["--dbtype"].ToLower());
                    break;
                case "go":
                    break;
            }
            if (traverser != null && builder != null)
            {
                builder.Build(traverser.Traverse(), options);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
