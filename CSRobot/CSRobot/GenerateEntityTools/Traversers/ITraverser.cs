

using CSRobot.GenerateEntityTools.Entity;
using System;

namespace CSRobot.GenerateEntityTools.Traversers
{
    /// <summary>
    /// 完成从数据库生成数据库结构实体
    /// </summary>
    public interface ITraverser
    {
        DataBase Traverse();
    }


}
