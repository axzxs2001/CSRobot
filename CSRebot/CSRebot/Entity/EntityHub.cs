using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRebot.Entity
{
    public class EntityHub
    {

        public string EntityDirectoryName { get; set; }

        public List<Entity> Entities { get; set; } = new List<Entity>();

    }
}
