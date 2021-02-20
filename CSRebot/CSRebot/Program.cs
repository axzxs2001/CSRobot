using CSRebot.LanguageBuilder;
using CSRebot.Traverser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace CSRebot
{
    class Program
    {
        static void Main(string[] args)
        {
            args = new string[] { "gen", "-dbtype=mysql", "-to=cs" };
            if (args.Length == 0)
            {
                _infoDic["--info"](args);
                return;
            }
            else
            {
                Run(args);
            }
        }

        static Dictionary<string, Func<string[], bool>> _infoDic = new Dictionary<string, Func<string[], bool>> {
            {"--info", Info},
            {"-h",Help},
            {"gen",GenerateEntity}
        };
        static bool Help(string[] args)
        {
            Console.WriteLine(@$"
Version {Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion.ToString()}

使用情况: csrebot [options] [command] [command-options] [arguments]

");
            return true;
        }
        static bool Info(string[] args)
        {
            Console.WriteLine(@$"
CSRebot v{Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion.ToString()}
----------------------------------------------
Description:
    为更好的使用C#提供帮助。

Usage:
    csrebot [options]

----------------------------------------------
");
            return true;
        }


        static CommandOptions GetOptions(string[] args)
        {
            var options = new CommandOptions();
            for (var i = 1; i < args.Length; i++)
            {
                var arr = args[i].Split("=");
                if (arr.Length < 2)
                {
                    throw new Exception("");
                }
                options.Add(arr[0], arr[1]);
            }
            return options;
        }
        static bool GenerateEntity(string[] args)
        {
            //csrebot gen -dbtype=mysql -to=cs -out=c:/abc
            var options = GetOptions(args);
            ITraverser traverser = null;
            ILanguageBuilder builder = null;
            switch (options["-dbtype"].ToLower())
            {
                case "mysql":
                    traverser = new MySqlTraverser();
                    break;
                case "mssql":
                    break;
                case "postgresql":
                    break;
            }
            switch (options["-to"].ToLower())
            {
                case "cs":
                    builder = new CSharpBuilder();
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
        static void Run(string[] args)
        {
            if (_infoDic.ContainsKey(args[0]))
            {
                _infoDic[args[0]](args);
            }
        }

    }


    public class CommandOptions : Dictionary<string, string>
    {

    }

}