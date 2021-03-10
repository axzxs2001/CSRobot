using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRobot
{
    class MessageHub
    {
        public static Dictionary<string, Dictionary<string, string>> Gen { get; private set; } = new Dictionary<string, Dictionary<string, string>> 
        {
            {
                "zh",new Dictionary<string, string>
                {                  
                    {"gen-h",@"使用方法：
csrobot gen [options]

命令参数选项：
--dbtype        数据库类型，必填，例如:--dbtype=mysql,--dbtype=mssql,--dbtype=postgressql
--out           生成实体类的路径，缺省默认输出文件到当前路径下entities目录中
--o             --out别名
--tep           生成实体类的模板，可以是内置的模板cs，或指定本地路径，或指定url，生成文件的扩展名与指定的方法匹配。缺省默认cs内置模板，例如:--tep=/usr/abc/bcd.cs；--tep=https://github.com/axzxs2001/CSRobot/blob/main/CSRobot/gen/gen_cs_record.cs；--tep=cs
--sql           查询表结构的sql语句，模板有两个属性：tablesql是查询库中全部表的信息，表名必需用tablename命名，fieldsql是查询tablename表中的全部字段，这里两个sql的字段除了tablename都可以自定义，在--tep模板中应用，例如:--sql=/usr/abc/mssql-cs.sql；--map=https://github.com/axzxs2001/CSRobot/blob/main/CSRobot/gen/mssql-cs.sql
--map           生成实体类字段时，数据库到实体类的字段映射模板，缺省值为内置的模板，或指定本地路径，或指定url，例如:--map=/usr/abc/bcd.json；--map=https://github.com/axzxs2001/CSRobot/blob/main/CSRobot/gen/map.json
--host          连接数据所在主机，如果缺少此项，会查找--constr,或查找当前目录或子目录中的是否存在appsettings.json配置文件，并取配置文件下的ConnectionStrings节点的第一个子节点的值作为连接字符串
--db            数据库名称，如果缺少此项，会查找--constr,或查找当前目录或子目录中的是否存在appsettings.json配置文件，并取配置文件下的ConnectionStrings节点的第一个子节点的值作为连接字符串
--user          数据库用户名，如果缺少此项，会查找--constr,或查找当前目录或子目录中的是否存在appsettings.json配置文件，并取配置文件下的ConnectionStrings节点的第一个子节点的值作为连接字符串
--pwd           数据库密码，如果缺少此项，会查找--constr,或查找当前目录或子目录中的是否存在appsettings.json配置文件，并取配置文件下的ConnectionStrings节点的第一个子节点的值作为连接字符串
--port          数据库端口号，如果缺少此项，会查找--constr,或查找当前目录或子目录中的是否存在appsettings.json配置文件，并取配置文件下的ConnectionStrings节点的第一个子节点的值作为连接字符串
--constr        或一个完整的连接字符串，如果缺少此项，会查找--host,--db,--user,--pwd,--port组，或查找当前目录或子目录中的是否存在appsettings.json配置文件，并取配置文件下的ConnectionStrings节点的第一个子节点的值作为连接字符串"}
                }
            }
        };

        public static Dictionary<string, Dictionary<string, string>> CSRobot { get; private set; } = new Dictionary<string, Dictionary<string, string>>
        {
            {
                "zh",new Dictionary<string, string>
                {
                    {"csrobot-h",@"CSRobot Version {0}

使用情况: CSRobot [options] [command] [command-options] [arguments]

------------------------------------------------
命令：
csrobot gen

选项参数：
csrobot -info
csrobot -h" },
                    {"csrobot-info",@"CSRobot v{0}
------------------------------------------------
      ______     ____        __          __ 
     / ____/____/ __ \____  / /_  ____  / /_
    / /   / ___/ /_/ / __ \/ __ \/ __ \/ __/
   / /___(__  ) _, _/ /_/ / /_/ / /_/ / /_  
   \____/____/_/ |_|\____/_.___/\____/\__/  
                                         
------------------------------------------------
Description:
    csrobot为开发提高效率，增加乐趣。

Usage:
    CSRobot [options] [command] [command-options] [arguments]

-----------------------------------------------" }                 
                }
            }
        };
    }
}
