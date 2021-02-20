using CSRebot.Entity;


namespace CSRebot.Traverser
{
    /// <summary>
    /// 完成从数据库生成数据库结构实体
    /// </summary>
    public interface ITraverser
    {
        DataBase Traverse();
    }
}
