using CSRebot.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRebot.LanguageBuilder
{
    /// <summary>
    /// 完成对应编程语言实体类生成
    /// </summary>
    public interface ILanguageBuilder
    {
        void Build(DataBase database, CommandOptions options);
    }

}
