using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSRobot.GenerateEntityTools.Traversers;
using CSRobot.GenerateEntityTools.Builders;
using System.Resources;
using System.Reflection;

namespace CSRobot.GenerateEntityTools
{
    static class GenerateEntityTool
    {
        //todo 这里要实现多语言支持
        static bool GenHelp()
        {
            var mgr = new ResourceManager("CSRobot.Resource.gen", Assembly.GetExecutingAssembly());
            // _culture = CultureInfo.GetCultureInfo("ja");
            Console.WriteLine(mgr.GetString("gen-h"));
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
            if (options.ContainsKey("--dbtype"))
            {
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
            }
            else
            {
                Console.WriteLine("--dbtype是必填参数");
                return false;
            }
            if (options.ContainsKey("--to"))
            {
                switch (options["--to"].ToLower())
                {
                    case "cs":
                        builder = new CSharpBuilder(options["--dbtype"].ToLower());
                        break;
                    case "go":
                        break;
                }
            }
            else
            {
                builder = new CSharpBuilder(options["--dbtype"].ToLower());
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
