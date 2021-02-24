using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSRobot.GenerateEntityTools.Entity;
using MySql.Data;
using MySql.Data.MySqlClient;
using Npgsql;

namespace CSRobot.GenerateEntityTools.Traversers
{
    public class PostgreSqlTraverser : Traverser
    {

        NpgsqlConnectionStringBuilder _connectionStringBuilder;

        public PostgreSqlTraverser(CommandOptions options) : base(options)
        {
            if (IsExistOption)
            {
                _connectionStringBuilder = new NpgsqlConnectionStringBuilder()
                {
                    Host = options["--host"],
                    Database = options["--db"],
                    Username = options["--user"],
                    Password = options["--pwd"],
                    Port = options.ContainsKey("--port") ? int.Parse(options["--port"]) : 5432,
                };
            }
            else
            {
                var connectionString = Common.GetConnectionString();
                if (string.IsNullOrEmpty(connectionString))
                {
                    Console.WriteLine("本地配置文件中找不到数据库连接字符串");
                }
                _connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);
                Console.WriteLine("从本地配置文件中加载连接字符中成功");
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
                DataBaseName = _connectionStringBuilder.Database
            };

            using (var con = new NpgsqlConnection(_connectionStringBuilder.ConnectionString))
            {
                var sql = @$"select relname as tablename,cast(obj_description(relfilenode,'pg_class') as varchar) as tabledescribe from pg_class c where relname in (SELECT tablename FROM pg_tables WHERE tablename NOT LIKE 'pg%' AND tablename NOT LIKE 'sql_%');";
                var cmd = new NpgsqlCommand(sql, con);
                con.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var table = new Table();
                    table.TableName = reader.GetFieldValue<string>("tablename");
                    if (reader.GetFieldValue<object>("tabledescribe") != DBNull.Value)
                    {
                        table.TableDescribe = reader.GetFieldValue<string>("tabledescribe");
                    }
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
                var sql = @$"SELECT a.attname AS fieldname, 
t.typname AS dbtype,  
case when a.atttypmod=-1 then null else a.atttypmod end AS fieldsize, 
b.description AS fielddescribe
FROM pg_class c, pg_attribute a
    LEFT JOIN pg_description b
    ON a.attrelid = b.objoid  AND a.attnum = b.objsubid, pg_type t
WHERE c.relname = '{table.TableName}'
    AND a.attnum > 0
    AND a.attrelid = c.oid
    AND a.atttypid = t.oid";

                using (var con = new NpgsqlConnection(_connectionStringBuilder.ConnectionString))
                {
                    var cmd = new NpgsqlCommand(sql, con);
                    con.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var field = new Field();
                        field.FieldName = reader.GetFieldValue<string>("fieldname");
                        if (reader.GetFieldValue<object>("fielddescribe") != DBNull.Value)
                        {
                            field.FieldDescribe = reader.GetFieldValue<string>("fielddescribe");
                        }
                        field.DBType = reader.GetFieldValue<string>("dbtype");
                        if (reader.GetFieldValue<object>("fieldsize") != DBNull.Value)
                        {
                            field.FieldSize = reader.GetFieldValue<int>("fieldsize");
                        }
                        table.Fields.Add(field);
                    }
                }
            }
        }

    }


}
