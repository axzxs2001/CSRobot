

using CSRebot.GenerateEntityTools.Entity;
using System;

namespace CSRebot.GenerateEntityTools.Traversers
{
    /// <summary>
    /// 完成从数据库生成数据库结构实体
    /// </summary>
    public interface ITraverser
    {
        DataBase Traverse();
    }


}
