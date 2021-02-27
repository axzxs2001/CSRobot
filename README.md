# CSRobot

<img src="https://github.com/axzxs2001/CSRobot/blob/main/csrobot.png" width="120"/>

> gen --dbtype=mssql --to=cs --tep=https://raw.githubusercontent.com/axzxs2001/CSRebot/main/CSRebot/gen/gen_cs_record.cs --host=127.0.0.1 --db=stealthdb --user=sa --pwd=sa
 
### csrobot gen [options]
|参数选项|描述|
| ---------------- | :-----------  | 
|--dbtype|数据库类型，必填，例如:--dbtype=mysql,--dbtype=mssql,--dbtype=postgressql|
|--table	|指定数据库表名生成实体类，缺省默认全部库表|
|--out	| 生成实体类的路径，缺省默认当前路径|
|--tep  <img width=100/>	|生成实体类的模板，可以是内置的模板cs，或指定本地路径，或指定url，生成文件的扩展名与指定的模板扩展名匹配。缺省默认cs内置模板，例如:--tep=/usr/abc/bcd.cs；--tep=https://github.com/abc/bcd.cs；--tep=cs|
|--host |	连接数据所在主机，如果缺少此项，会查找当前目录或子目录中的是否存在appsettings.json配置文件，并取配置文件下的ConnectionStrings节点的第一个子节点的值作为连接字符串|
|--db |	数据库名称，如果缺少此项，会查找当前目录或子目录中的是否存在appsettings.json配置文件，并取配置文件下的ConnectionStrings节点的第一个子节点的值作为连接字符串|
|--user	|数据库用户名，如果缺少此项，会查找当前目录或子目录中的是否存在appsettings.json配置文件，并取配置文件下的ConnectionStrings节点的第一个子节点的值作为连接字符串|
|--pwd|	数据库密码，如果缺少此项，会查找当前目录或子目录中的是否存在appsettings.json配置文件，并取配置文件下的ConnectionStrings节点的第一个子节点的值作为连接字符串|
|--port|	数据库端口号，如果缺少此项，会查找当前目录或子目录中的是否存在appsettings.json配置文件，并取配置文件下的ConnectionStrings节点的第一个子节点的值作为连接字符串|
