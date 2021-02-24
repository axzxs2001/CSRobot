using CSRobot.GenerateEntityTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace CSRobot
{
    class Program
    {

    


        static CultureInfo _culture;
        static void Main(string[] args)
        {
    
            //var mgr = new ResourceManager("CSRobot.Resource.gen", Assembly.GetExecutingAssembly());
            //_culture = CultureInfo.GetCultureInfo("ja");
            //Console.WriteLine(mgr.GetString("demo", _culture));
            //Console.WriteLine(mgr.GetString("demo"));

            //多语言支持
            //"--constr=server=127.0.0.1;database=testdb;uid=root;pwd=gsw2021;"
            //args = new string[] { "gen", "-dbtype=mysql", "-to=cs" };
            //CSRobot gen -dbtype=mysql -to=cs -out=c:/abc

            //mysql 
            //  args = new string[] { "gen", "--dbtype=mysql", "--to=cs", "--tep=https://raw.githubusercontent.com/axzxs2001/CSRobot/main/CSRobot/gen/gen_cs_record.cs", "--table=account",  "--host=127.0.01","--db=testdb","--user=root","--pwd=gsw2021", "--port=3306" };

            //postgres
            //args = new string[] { "gen", "--dbtype=postgresql", "--to=cs", "--tep=https://raw.githubusercontent.com/axzxs2001/CSRobot/main/CSRobot/gen/gen_cs_record.cs",  "--host=127.0.01", "--db=stealthdb", "--user=postgres","--pwd=postgres2018" };
            //mssql
            //args = new string[] { "gen", "--dbtype=mssql", "--to=cs", "--tep=https://raw.githubusercontent.com/axzxs2001/CSRobot/main/CSRobot/gen/gen_cs_record.cs", "--host=127.0.01", "--db=stealthdb", "--user=sa", "--pwd=sa" };

            //appsettings
            args = new string[] { "gen", "--dbtype=mssql", "--to=cs", "--tep=https://raw.githubusercontent.com/axzxs2001/CSRobot/main/CSRobot/gen/gen_cs_record.cs", "--db=stealthdb", "--user=sa", "--pwd=sa" };
            CSRobotTools.Run(args);
        }
    }
    static class CSRobotTools
    {
        static Dictionary<string, Func<CommandOptions, bool>> _CSRobotDic;
        static CSRobotTools()
        {
            _CSRobotDic = new Dictionary<string, Func<CommandOptions, bool>> {
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
                _CSRobotDic["--info"](options);
                return;
            }
            if (_CSRobotDic.ContainsKey(args[0]))
            {
                _CSRobotDic[args[0]](options);
            }
        }
        static bool Help(CommandOptions options)
        {
            Console.WriteLine(@$"
Version {Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion.ToString()}

使用情况: CSRobot [options] [command] [command-options] [arguments]

");
            return true;
        }
        static bool Info(CommandOptions options)
        {
            Console.WriteLine(@$"
CSRobot v{Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion.ToString()}
----------------------------------------------
Description:
    为更好的使用C#提供帮助。

Usage:
    CSRobot [options]

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