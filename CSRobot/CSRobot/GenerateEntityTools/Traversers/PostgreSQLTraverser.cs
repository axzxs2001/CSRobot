using System;
using System.Collections.Generic;
using System.Data;
using CSRobot.GenerateEntityTools.Entity;
using Npgsql;

namespace CSRobot.GenerateEntityTools.Traversers
{
    public class PostgreSqlTraverser : Traverser
    {

        readonly NpgsqlConnectionStringBuilder _connectionStringBuilder;

        public PostgreSqlTraverser(CommandOptions options) : base(options)
        {
            if (!string.IsNullOrEmpty(ConnectionString))
            {
                _connectionStringBuilder = new NpgsqlConnectionStringBuilder(ConnectionString);
            }
            else if (IsExistOption)
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

        private DataBase GetDataBase()
        {
            return new DataBase
            {
                Tables = GetTables()
            };
        }

        private IEnumerable<Dictionary<string, object>> GetTables()
        {

            var resultList = new List<Dictionary<string, object>>();
            using (var con = new NpgsqlConnection(_connectionStringBuilder.ConnectionString))
            {
                var sql = @$"select relname as tablename,cast(obj_description(relfilenode,'pg_class') as varchar) as tabledescribe from pg_class c where relname in (SELECT tablename FROM pg_tables WHERE tablename NOT LIKE 'pg%' AND tablename NOT LIKE 'sql_%');";
                sql = string.IsNullOrEmpty(TableSQL) ? sql : TableSQL;
                var cmd = new NpgsqlCommand(sql, con);
                con.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var entity = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var value = reader.GetValue(i);
                        entity[reader.GetName(i)] = value;
                    }
                    resultList.Add(entity);
                }
                con.Close();
            }
            foreach (var entity in resultList)
            {
                var fields = GetFields(entity["tablename"].ToString());
                entity["fields"] = fields;
            }
            return resultList;

        }

        private IEnumerable<Dictionary<string, object>> GetFields(string tableName)
        {
            var resultList = new List<Dictionary<string, object>>();
            using (var con = new NpgsqlConnection(_connectionStringBuilder.ConnectionString))
            {
                var sql = @$"SELECT a.attname AS fieldname, 
t.typname AS dbtype,  
case when a.atttypmod=-1 then null else a.atttypmod end AS fieldsize, 
b.description AS fielddescribe
FROM pg_class c, pg_attribute a
    LEFT JOIN pg_description b
    ON a.attrelid = b.objoid  AND a.attnum = b.objsubid, pg_type t
WHERE c.relname = '{tableName}'
    AND a.attnum > 0
    AND a.attrelid = c.oid
    AND a.atttypid = t.oid";
                sql = string.IsNullOrEmpty(FieldSQL) ? sql : FieldSQL.Replace("${tablename}", tableName, StringComparison.OrdinalIgnoreCase);
                var cmd = new NpgsqlCommand(sql, con);
                con.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var entity = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var value = reader.GetValue(i);
                        entity[reader.GetName(i)] = value;
                    }
                    resultList.Add(entity);
                }
                con.Close();
            }
            return resultList;
        }
    }


}
