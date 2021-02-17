using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRebot.Entity
{
    public class Entity
    {
        public string EntityName { get; set; }

        public string EntityDescribe { get; set; }

        public List<Field> Fields { get; set; } = new List<Field>();

    }
}
