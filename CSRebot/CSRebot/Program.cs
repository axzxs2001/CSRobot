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
            { "--info", Info},
            { "-h",Help},
            {"build",EntityBuild}
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


        static bool EntityBuild(string[] args)
        {
            var nnn = new MySQLCreater();
            var k = nnn.GetDataBase();
            var build = new CSharpBuild();
            return true;
        }
        static void Run(string[] args)
        {
            if (_infoDic.ContainsKey(args[0]))
            {
                _infoDic[args[0]](args);
            }
        }

    }

    // public delegate bool Funcation(string[] args);


}