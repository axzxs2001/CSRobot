# CSRobot

<img src="https://github.com/axzxs2001/CSRobot/blob/main/csrobot.png" width="120"/>


安装命令：
>dotnet tool install --global CSRobot --version 0.0.3

Nuget地址：https://www.nuget.org/packages/CSRobot/  
 
 **重点说明：**  
 gen是完成从数据库表结构生成实体类的小工具，其工作原理是
1. 按照--sql指定的sql模板或内置的sql语句查询库中的全部表信息，表中全部字段信息
2. 按照--map配置库和实体类的映射关系
3. 按照--tep指定的实体模板或内置的实体模板  

生成实体类文件
 
 
### csrobot gen [options]
|参数选项|描述|
| ---------------- | :-----------  | 
|--dbtype|数据库类型，必填，例如:--dbtype=mysql,--dbtype=mssql,--dbtype=postgressql|
|--table	|指定特定的表名生成实体类，缺省默认生成全部库表|
|--out,--o	| 生成实体类的路径，缺省默认输出文件到当前路径下entities目录中|
|--tep  <img width=100/>	|生成实体类的模板，可以是内置的模板cs，或指定本地路径，或指定url，生成文件的扩展名与指定的模板扩展名匹配。缺省默认cs内置模板，例如:--tep=/usr/abc/bcd.cs；--tep=https://github.com/abc/bcd.cs；--tep=cs|
|--sql|查询表结构的sql语句，模板有两个属性：tablesql是查询库中全部表的信息，表名必需用tablename命名，fieldsql是查询tablename表中的全部字段，这里两个sql的字段除了tablename都可以自定义，在--tep模板中应用，例如:--sql=/usr/abc/mssql-cs.sql；--map=https://github.com/axzxs2001/CSRobot/blob/main/CSRobot/gen/mssql-cs.sql|
|--map|生成实体类字段时，数据库到实体类的字段映射模板，缺省值为内置的模板，或指定本地路径，或指定url，例如:--map=/usr/abc/bcd.json；--map=https://github.com/axzxs2001/CSRobot/blob/main/CSRobot/gen/map.json|
|--host |	连接数据所在主机，如果缺少此项，会查找--constr,或查找当前目录或子目录中的是否存在appsettings.json配置文件，并取配置文件下的ConnectionStrings节点的第一个子节点的值作为连接字符串|
|--db |	数据库名称，如果缺少此项，会查找--constr,或查找当前目录或子目录中的是否存在appsettings.json配置文件，并取配置文件下的ConnectionStrings节点的第一个子节点的值作为连接字符串|
|--user	|数据库用户名，如果缺少此项，会查找--constr,或查找当前目录或子目录中的是否存在appsettings.json配置文件，并取配置文件下的ConnectionStrings节点的第一个子节点的值作为连接字符串|
|--pwd|	数据库密码，如果缺少此项，会查找--constr,或查找当前目录或子目录中的是否存在appsettings.json配置文件，并取配置文件下的ConnectionStrings节点的第一个子节点的值作为连接字符串|
|--port|	数据库端口号，如果缺少此项，会查找--constr,或查找当前目录或子目录中的是否存在appsettings.json配置文件，并取配置文件下的ConnectionStrings节点的第一个子节点的值作为连接字符串|
|--constr| 或一个完整的连接字符串	，如果缺少此项，会查找--host,--db,--user,--pwd,--port组，或查找当前目录或子目录中的是否存在appsettings.json配置文件，并取配置文件下的ConnectionStrings节点的第一个子节点的值作为连接字符串|

**说明：** 
*如果连接字符串属性同时出现，--constr优先级最高，其次是--host,--db,--user,--pwd,--port组，两者都不存在再查询appsettings.json*  
*模板文件夹：https://github.com/axzxs2001/CSRobot/tree/main/CSRobot/gen，包含实体类模板，数据库到实体类型映身模板，查询表结构sql语句模板*

### --tep
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
### --map
下面例子是DB与c#的类型映射，工具内置了三种数据库：mssql，mysql，postgresql,对于转化成的实体类，按照--tep模板生成，可以是非C#实体类
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
#### --sql
##### mssql.json
~~~
{
  "tablesql": "Select Name as tablename,'' as tabledescribe FROM SysObjects Where XType='U' ;",
  "fieldsql": "SELECT a.name as fieldname,b.name as dbtype,case when a.xprec=0 then COLUMNPROPERTY(a.id,a.name,'PRECISION') else null end as fieldsize,isnull(g.[value],'') as fielddescribe FROM syscolumns a left join systypes b on a.xusertype=b.xusertype inner join sysobjects d on a.id=d.id  and d.xtype='U' and d.name<>'dtproperties' left join sys.extended_properties g on a.id=G.major_id and a.colid=g.minor_id where d.name='${tableName}'"
}
~~~
##### mysql.json
~~~
{
  "tablesql": "select table_name as tablename,table_comment as tabledescribe from information_schema.tables where table_schema='${databasename}' and table_type='BASE TABLE';",
  "fieldsql": "select character_maximum_length as fieldsize,column_name as fieldname,data_type as dbtype,column_comment as fielddescribe from information_schema.columns where table_name = '${tableName}' "
}
~~~
##### postgresql.json
~~~
{
  "tablesql": "select relname as tablename,cast(obj_description(relfilenode,'pg_class') as varchar) as tabledescribe from pg_class c where relname in (SELECT tablename FROM pg_tables WHERE tablename NOT LIKE 'pg%' AND tablename NOT LIKE 'sql_%');",
  "fieldsql": "SELECT a.attname AS fieldname,t.typname AS dbtype,case when a.atttypmod=-1 then null else a.atttypmod end AS fieldsize, b.description AS fielddescribe FROM pg_class c, pg_attribute a    LEFT JOIN pg_description b    ON a.attrelid = b.objoid  AND a.attnum = b.objsubid, pg_type t WHERE c.relname = '${tableName}'    AND a.attnum > 0    AND a.attrelid = c.oid    AND a.atttypid = t.oid"
}
~~~

## 案例

最简单的
>csrobot gen --dbtype=mssql

从MySql库生成实体类
>csrobot gen --dbtype=mssql --tep=https://raw.githubusercontent.com/axzxs2001/CSRebot/main/CSRebot/gen/gen_cs_record.cs --host=127.0.0.1 --db=stealthdb --user=sa --pwd=sa

 
