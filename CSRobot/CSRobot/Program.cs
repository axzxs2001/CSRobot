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
            args = new string[] { "gen", "--dbtype=postgresql", @"--map=C:\MyFile\Source\Repos\axzxs2001\CSRobot\CSRobot\gen\map.json", "--host=127.0.0.1", "--db=StarPayAgent", "--user=postgres", "--pwd=postgres2018" };
            //appsettings
            // args = new string[] { "gen", "--dbtype=mssql", "--to=cs", "--tep=https://raw.githubusercontent.com/axzxs2001/CSRobot/main/CSRobot/gen/gen_cs_record.cs", "--db=stealthdb", "--user=sa", "--pwd=sa" };

            // args = new string[] { "gen", "-h" };
            // args = new string[] { "-info" };
            try
            {
                CSRobotTools.Run(args);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }
    }
    static class CSRobotTools
    {
        static Dictionary<string, Func<CommandOptions, bool>> _CSRobotDic;
        static CSRobotTools()
        {
            _CSRobotDic = new Dictionary<string, Func<CommandOptions, bool>> {
            {"-info", Info},
            {"-h",Help},
            {"gen",GenerateEntityTool.GenerateEntity}
        };
        }
        public static bool Run(string[] args)
        {
            var options = GetOptions(args);
            if (args.Length == 0)
            {
                return _CSRobotDic["--info"](options);
            }
            else if (_CSRobotDic.ContainsKey(args[0]))
            {
                return _CSRobotDic[args[0]](options);
            }
            else
            {
                return false;
            }
        }
        static bool Help(CommandOptions options)
        {
            var mgr = new ResourceManager("CSRobot.Resource.gen", Assembly.GetExecutingAssembly());
            Console.WriteLine(mgr.GetString("csrobot-h"), Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion.ToString());
            return true;
        }
        static bool Info(CommandOptions options)
        {
            var mgr = new ResourceManager("CSRobot.Resource.gen", Assembly.GetExecutingAssembly());
            Console.WriteLine(mgr.GetString("csrobot-info"), Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion.ToString());
            return true;
        }
        static CommandOptions GetOptions(string[] args)
        {
            var options = new CommandOptions();
            for (var i = 1; i < args.Length; i++)
            {
                if (string.IsNullOrEmpty(args[i].Trim()))
                {
                    continue;
                }
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