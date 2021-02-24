
using CSRobot.GenerateEntityTools.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRobot.GenerateEntityTools.Builders
{
    /// <summary>
    /// 完成对应编程语言实体类生成
    /// </summary>
    public interface IBuilder
    {
        void Build(DataBase database, CommandOptions options);
    }

}
