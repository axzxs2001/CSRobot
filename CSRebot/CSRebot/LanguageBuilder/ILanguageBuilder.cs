using CSRebot.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRebot.LanguageBuilder
{

    public interface ILanguageBuilder
    {
        void Build(EntityHub entityHub, IDictionary<string, string> options);
    }

}
