using CSRobot.GenerateEntityTools.Entity;
using System;

namespace CSRobot.GenerateEntityTools.Traversers
{
    public abstract class Traverser : ITraverser
    {
        protected bool IsExistOption { get; set; } = true;
        public Traverser(CommandOptions options)
        {
            if (!options.ContainsKey("--host"))
            {
                IsExistOption = false;
                Console.WriteLine("缺少 --host");
            }
            if (!options.ContainsKey("--db"))
            {
                IsExistOption = false;
                Console.WriteLine("缺少 --db");
            }
            if (!options.ContainsKey("--user"))
            {
                IsExistOption = false;
                Console.WriteLine("缺少 --user");
            }
            if (!options.ContainsKey("--pwd"))
            {
                IsExistOption = false;
                Console.WriteLine("缺少 --pwd");
            }
        }
        public abstract DataBase Traverse();       
    }
}
