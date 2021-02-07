using System;
using System.Collections.Generic;
using System.Reflection;

namespace CSRebot
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {


                _infoDic["--info"]();
                return;
            }
            else
            {
                Run(args);
            }
        }

        static Dictionary<string, Action> _infoDic = new Dictionary<string, Action> {
            { "--info", ()=>Console.WriteLine( @$"
CSRebot v{Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion.ToString()}
----------------------------------------------
Description:
    为更好的使用C#提供帮助。

Usage:
    csrebot [options]

----------------------------------------------
")
    },
            {"-h",()=>Console.WriteLine(@$"
Version {Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion.ToString()}

使用情况: csrebot [options] [command] [command-options] [arguments]

")
            },
            {
    "-h=today",()=>{
        switch((int)DateTime.Now.DayOfWeek)
        {
            case 0:
                Console.ForegroundColor=ConsoleColor.Red;
                break;
            case 6:
                Console.ForegroundColor=ConsoleColor.Green;
                break;   
        }
        Console.WriteLine(@$"今天是{DateTime.Now.ToString("yyyy年MM月dd日")}，{(DayOfChineseWeek)(int)DateTime.Now.DayOfWeek}");
        Console.ResetColor();

            }
        }
        };


        static void Run(string[] args)
        {
            if (_infoDic.ContainsKey(args[0]))
            {
                _infoDic[args[0]]();
            }
        }
    }

    public enum DayOfChineseWeek
    {
        星期日 = 0,
        星期一 = 1,
        星期二 = 2,
        星期三 = 3,
        星期四 = 4,
        星期五 = 5,
        星期六 = 6
    }
}