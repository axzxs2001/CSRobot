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
            Console.WriteLine(MessageHub.Gen["zh"]["gen-h"]);
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
                    default:
                        Console.WriteLine("失败：\r\n目前只支持mysql,mssql,postgresql");
                        return false;
                }
            }
            else
            {
                FailedWriteLine("--dbtype是必填参数");
                return false;
            }

            IBuilder builder = new CSharpBuilder();
            if (traverser != null && builder != null)
            {
                builder.Build(traverser.Traverse(), options);

                SuccessWriteLine("生成成功");
                return true;
            }
            else
            {
                FailedWriteLine("生成失败");
                return false;
            }
        }

        internal static void SuccessWriteLine(string content)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(content);
            Console.ResetColor();
        }
        internal static void FailedWriteLine(string content)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(content);
            Console.ResetColor();
        }
    }
}
