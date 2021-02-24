using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSRobot.GenerateEntityTools.Entity;
using Microsoft.Data.SqlClient;

namespace CSRobot.GenerateEntityTools.Traversers
{
    public class MsSqlTraverser : Traverser
    {

        SqlConnectionStringBuilder _connectionStringBuilder;
        public MsSqlTraverser(CommandOptions options) : base(options)
        {
            if (IsExistOption)
            {
                _connectionStringBuilder = new SqlConnectionStringBuilder()
                {
                    DataSource = options["--host"],
                    InitialCatalog = options["--db"],
                    UserID = options["--user"],
                    Password = options["--pwd"],                   
                };
            }
            else
            {
                var connectionString = Common.GetConnectionString();
                if (string.IsNullOrEmpty(connectionString))
                {
                    Console.WriteLine("本地配置文件中找不到数据库连接字符串");
                }
                _connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            }
        }
        public override DataBase Traverse()
        {
            return GetDataBase();
        }
        DataBase GetDataBase()
        {
            var dataBase = new DataBase()
            {
                DataBaseName = _connectionStringBuilder.InitialCatalog
            };

            using (var con = new SqlConnection(_connectionStringBuilder.ConnectionString))
            {
                var sql = @$"Select Name as tablename,'' as tabledescribe FROM SysObjects Where XType='U' ;";
                var cmd = new SqlCommand(sql, con);
                con.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var table = new Table();
                    table.TableName = reader.GetFieldValue<string>("tablename");
                    table.TableDescribe = reader.GetFieldValue<string>("tabledescribe");
                    dataBase.Tables.Add(table);
                }
                con.Close();
            }
            GetFields(dataBase);
            return dataBase;
        }


        void GetFields(DataBase dataBase)
        {
            foreach (var table in dataBase.Tables)
            {
                var sql = @$"SELECT   a.name as fieldname,b.name as dbtype,COLUMNPROPERTY(a.id,a.name,'PRECISION') as fieldsize,isnull(g.[value],'') as fielddescribe
FROM  syscolumns a
left join     systypes b on     a.xusertype=b.xusertype
inner join     sysobjects d on     a.id=d.id  and d.xtype='U' and  d.name<>'dtproperties'
left join sys.extended_properties   g on     a.id=G.major_id and a.colid=g.minor_id 
where   d.name='{table.TableName}'    
";
                using (var con = new SqlConnection(_connectionStringBuilder.ConnectionString))
                {
                    var cmd = new SqlCommand(sql, con);
                    con.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var field = new Field();
                        field.FieldName = reader.GetFieldValue<string>("fieldname");
                        field.FieldDescribe = reader.GetFieldValue<string>("fielddescribe");
                        field.DBType = reader.GetFieldValue<string>("dbtype");
                        var size = reader.GetFieldValue<object>("fieldsize");
                        if (size != DBNull.Value)
                        {
                            field.FieldSize = Convert.ToInt64(size);
                        }
                        table.Fields.Add(field);
                    }
                }
            }
        }





    }


}
