# CSRobot

<img src="https://github.com/axzxs2001/CSRobot/blob/main/csrobot.png" width="120"/>

Nuget地址：
>https://www.nuget.org/packages/CSRobot/
安装命令：
>dotnet tool install --global CSRobot --version 0.0.3

模板文件夹：https://github.com/axzxs2001/CSRobot/tree/main/CSRobot/gen

### 命令
>csrobot gen [options]
 
### csrobot gen [options]
|参数选项|描述|
| ---------------- | :-----------  | 
|--dbtype|数据库类型，必填，例如:--dbtype=mysql,--dbtype=mssql,--dbtype=postgressql|
|--table	|指定数据库表名生成实体类，缺省默认全部库表|
|--out	| 生成实体类的路径，缺省默认输出文件到当前路径下entities目录中|
|--tep  <img width=100/>	|生成实体类的模板，可以是内置的模板cs，或指定本地路径，或指定url，生成文件的扩展名与指定的模板扩展名匹配。缺省默认cs内置模板，例如:--tep=/usr/abc/bcd.cs；--tep=https://github.com/abc/bcd.cs；--tep=cs|
|--map|生成实体类字段时，数据库到实体类的字段映射模板，缺省值为内置的模板，或指定本地路径，或指定url，例如:--map=/usr/abc/bcd.json；--map=https://github.com/axzxs2001/CSRobot/blob/main/CSRobot/gen/map.json|
|--host |	连接数据所在主机，如果缺少此项，会查找当前目录或子目录中的是否存在appsettings.json配置文件，并取配置文件下的ConnectionStrings节点的第一个子节点的值作为连接字符串|
|--db |	数据库名称，如果缺少此项，会查找当前目录或子目录中的是否存在appsettings.json配置文件，并取配置文件下的ConnectionStrings节点的第一个子节点的值作为连接字符串|
|--user	|数据库用户名，如果缺少此项，会查找当前目录或子目录中的是否存在appsettings.json配置文件，并取配置文件下的ConnectionStrings节点的第一个子节点的值作为连接字符串|
|--pwd|	数据库密码，如果缺少此项，会查找当前目录或子目录中的是否存在appsettings.json配置文件，并取配置文件下的ConnectionStrings节点的第一个子节点的值作为连接字符串|
|--port|	数据库端口号，如果缺少此项，会查找当前目录或子目录中的是否存在appsettings.json配置文件，并取配置文件下的ConnectionStrings节点的第一个子节点的值作为连接字符串|


### tep
如果生成的实体是cs的，模板文件的扩展名为.cs,内容如下例，其中${}的选项是固定选项，分别代表字面含义
~~~ C#
using System;

namespace MyNameSpace
{
    /// <summary>
    /// ${TableDescribe}
    /// </summary>
    public class ${TableName}
    {
        ${ Fields}
        $?{ FieldDescribe}/// <summary>
        $?{ FieldDescribe}/// ${FieldDescribe}
        $?{ FieldDescribe}/// </summary>
        $?{ FieldSize}[BField(Length =${ FieldSize},Name = ""${ FieldName}"")]
        public ${ DBType} ${ FieldName}
        { get; set; }
        ${ Fields}
    }
}
~~~
### map
下面例子是MS SqlServer与csharp的类型映射
~~~ json
{
  "mssql-cs": {
    "char": "string",
    "varchar": "string",
    "text": "string",
    "nchar": "string",
    "nvarchar": "string",
    "ntext": "string",
    "bit": "bool",
    "binary": "string",
    "varbinary": "string",
    "image": "string",
    "tinyint": "byte",
    "smallint": "short",
    "int": "int",
    "bigint": "long",
    "decimal": "decimal",
    "numeric": "decimal",
    "smallmoney": "decimal",
    "money": "decimal",
    "float": "float",
    "real": "double",
    "datetime": "DateTime",
    "datetime2": "DateTime",
    "smalldatetime": "DateTime",
    "date": "DateTime",
    "time": "DateTime",
    "datetimeoffset": "DateTimeOffset",
    "timestamp": "string"
  },
~~~

## 案例
从MySql库生成实体类
>csrobot gen --dbtype=mssql --to=cs --tep=https://raw.githubusercontent.com/axzxs2001/CSRebot/main/CSRebot/gen/gen_cs_record.cs --host=127.0.0.1 --db=stealthdb --user=sa --pwd=sa

 
