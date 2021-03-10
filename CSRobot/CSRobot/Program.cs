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
        static readonly Dictionary<string, Func<CommandOptions, bool>> _CSRobotDic;
        static CSRobotTools()
        {
            _CSRobotDic = new Dictionary<string, Func<CommandOptions, bool>> {
            {"-info", Info},
            {"-h",Help},
            {"gen",GenerateEntityTool.GenerateEntity},
            {"code",CodeInfo.CodeInfo.ShowCodeInfo}
        };
        }
        public static bool Run(string[] args)
        {
            var options = GetOptions(args);
            if (args.Length == 0)
            {
                return _CSRobotDic["-info"](options);
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
            Console.WriteLine(MessageHub.CSRobot["zh"]["csrobot-h"], Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion.ToString());
            return true;
        }
        static bool Info(CommandOptions options)
        {
            Console.WriteLine(MessageHub.CSRobot["zh"]["csrobot-info"], Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion.ToString());
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