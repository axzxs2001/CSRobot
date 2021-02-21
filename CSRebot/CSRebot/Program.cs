using CSRebot.GenerateEntityTools;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CSRebot
{
    class Program
    {
        static void Main(string[] args)
        {
            //多语言支持
            //args = new string[] { "gen", "-dbtype=mysql", "-to=cs" };
            //csrebot gen -dbtype=mysql -to=cs -out=c:/abc
            args = new string[] { "gen", "-h", "--dbtype=mysql", "--to=cs", "--tep=https://raw.githubusercontent.com/axzxs2001/CSRebot/main/CSRebot/gen/gen_cs_record.cs" };
            CSRebotTools.Run(args);
        }
    }
    static class CSRebotTools
    {
        static Dictionary<string, Func<CommandOptions, bool>> _csRebotDic;
        static CSRebotTools()
        {
            _csRebotDic = new Dictionary<string, Func<CommandOptions, bool>> {
            {"--info", Info},
            {"-h",Help},
            {"gen",GenerateEntityTool.GenerateEntity}
        };
        }
        public static void Run(string[] args)
        {
            var options = GetOptions(args);
            if (args.Length == 0)
            {
                _csRebotDic["--info"](options);
                return;
            }
            if (_csRebotDic.ContainsKey(args[0]))
            {
                _csRebotDic[args[0]](options);
            }
        }
        static bool Help(CommandOptions options)
        {
            Console.WriteLine(@$"
Version {Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion.ToString()}

使用情况: csrebot [options] [command] [command-options] [arguments]

");
            return true;
        }
        static bool Info(CommandOptions options)
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
                    options.Add(arr[0], null);
                }
                else
                {
                    options.Add(arr[0], arr[1]);
                }
            }
            return options;
        }
    }
}