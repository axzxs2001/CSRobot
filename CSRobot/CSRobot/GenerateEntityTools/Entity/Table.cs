using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRobot.GenerateEntityTools.Entity
{
    public class Table
    {
        public string TableName { get; set; }

        public string TableDescribe { get; set; }

        public List<Field> Fields { get; set; } = new List<Field>();

    }
}
