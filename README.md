# CSRobot

<img src="https://github.com/axzxs2001/CSRobot/blob/main/csrobot.png" width="120"/>

Nuget地址：
>https://www.nuget.org/packages/CSRobot/
安装命令：
>dotnet tool install --global CSRobot --version 0.0.3

模板文件夹：https://github.com/axzxs2001/CSRobot/tree/main/CSRobot/gen

### 命令
>csrobot gen [options]
 
 说明：
 gen是完成从数据库查询表结构生成实体类的工具
1. 可以按照 sql模板或内置 sql查询库中的表，表中的字段
2. 配置库和实体类的映射
3. 按照实体模板生成实体类文件
 
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
如果生成的实体是cs的，模板文件的扩展名为.cs,内容如下例，
${}的选项是固定选项，分别代表从数据库中查询到的数据字段
$?{}是判断条件，如果有值，本行显示，否则本行不显示
$map{}本字段完成类型映射
~~~ C#
using System;

namespace MyNameSpace
{
    /// <summary>
    /// ${tabledescribe}
    /// </summary>
    public class ${tablename}
    {
        ${Fields}
        $?{fielddescribe}/// <summary>
        $?{fielddescribe}/// ${fielddescribe}
        $?{fielddescribe}/// </summary>
        $?{fieldsize}[BField(Length=${fieldsize},Name="${fieldname}")]
        public $map{dbtype} ${fieldname}
        { get; set; }
        ${Fields}
    }
}
~~~
### map
下面例子是DB与c#的类型映射
~~~ json
{
  "mssql-cs": {
    "bigint": "long",
    "binary": "Byte[]",
    "bit": "bool",
    "char": "string",
    "date": "DateTime",
    "datetime": "DateTime",
    "datetime2": "DateTime",
    "datetimeoffset": "DateTimeOffset",
    "decimal": "decimal",
    "float": "double",
    "image": "Byte[]",
    "int": "int",
    "money": "decimal",
    "nchar": "string",
    "ntext": "string",
    "numeric": "double",
    "nvarchar": "string",
    "real": "float",
    "rowversion": "Byte[]",
    "smalldatetime": "DateTime",
    "smallint": "short",
    "smallmoney": "decimal",
    "text": "string",
    "time": "TimeSpan",
    "timestamp": "Byte[]",
    "tinyint": "Byte",
    "uniqueidentifier": "Guid",
    "varbinary": "Byte[]",
    "varchar": "string",
    "xml": "string"
  },
  "mysql-cs": {
    "char": "string",
    "varchar": "string",
    "binary": "byte[]",
    "varbinary": "byte[]",
    "tinyblob": "btye[]",
    "tinytext": "string",
    "text": "string",
    "blob": "byte[]",
    "mediumtext": "string",
    "mediumblob": "byte[]",
    "longtext": "string",
    "longblob": "byte[]",
    "enum": "string",
    "bit": "short",
    "tinyint": "byte",
    "bool": "bool",
    "boolean": "bool",
    "smallint": "short",
    "mediumint": "short",
    "int": "int32",
    "integer": "int32",
    "bigint": "long",
    "float": "float",
    "double": "double",
    "double precision": "double",
    "decimal": "decimal",
    "dec": "decimal",
    "date": "datetime",
    "datetime": "datetime",
    "timestamp": "datetime",
    "time": "datetime",
    "year": "short"
  },
  "postgresql-cs": {
    "bigint": "long",
    "int8": "long",
    "bigserial": "long",
    "serial8": "long",
    "boolean": "bool",
    "bool": "bool",
    "bytea": "Byte[]",
    "character": "string",
    "char": "string",
    "character varying": "string",
    "varchar": "string",
    "date": "DateTime",
    "double precision": "double",
    "float8": "double",
    "integer": "int",
    "int4": "int",
    "interval": "string",
    "money": "decimal",
    "numeric": "decimal",
    "decimal": "decimal",
    "real": "float",
    "float4": "float",
    "smallint": "short",
    "int2": "short",
    "smallserial": "short",
    "serial2": "short",
    "serial": "int",
    "serial4": "int",
    "text": "string",
    "time": "DateTime",
    "time with time zone": "DateTimeOffset",
    "timetz": "DateTimeOffset",
    "timestamp": "DateTime",
    "timestamp with time zone": "DateTimeOffset",
    "timestamptz": "DateTimeOffset",
    "uuid": "GUID"
  }
}
~~~

## 案例
从MySql库生成实体类
>csrobot gen --dbtype=mssql --to=cs --tep=https://raw.githubusercontent.com/axzxs2001/CSRebot/main/CSRebot/gen/gen_cs_record.cs --host=127.0.0.1 --db=stealthdb --user=sa --pwd=sa

 
