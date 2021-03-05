using System;
using System.Collections.Generic;
using System.Data;
using CSRobot.GenerateEntityTools.Entity;
using MySql.Data.MySqlClient;

namespace CSRobot.GenerateEntityTools.Traversers
{
    public class MySqlTraverser : Traverser
    {
        private readonly MySqlConnectionStringBuilder _connectionStringBuilder;
        public MySqlTraverser(CommandOptions options) : base(options)
        {
            if (!string.IsNullOrEmpty(ConnectionString))
            {
                _connectionStringBuilder = new MySqlConnectionStringBuilder(ConnectionString);
            }
            else if (IsExistOption)
            {
                _connectionStringBuilder = new MySqlConnectionStringBuilder()
                {
                    Server = options["--host"],
                    Database = options["--db"],
                    UserID = options["--user"],
                    Password = options["--pwd"],
                    Port = options.ContainsKey("--port") ? uint.Parse(options["--port"]) : 3306,
                };
            }
            else
            {
                var connectionString = Common.GetConnectionString();
                if (string.IsNullOrEmpty(connectionString))
                {
                    Console.WriteLine("本地配置文件中找不到数据库连接字符串");
                }
                _connectionStringBuilder = new MySqlConnectionStringBuilder(connectionString);
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
            using (var con = new MySqlConnection(_connectionStringBuilder.ConnectionString))
            {
                var sql = @$"select table_name as tablename,table_comment as tabledescribe from information_schema.tables where table_schema='{_connectionStringBuilder.Database}' and table_type='BASE TABLE';";
                sql = string.IsNullOrEmpty(TableSQL) ? sql : TableSQL.Replace("${databasename}", _connectionStringBuilder.Database, StringComparison.OrdinalIgnoreCase);
                var cmd = new MySqlCommand(sql, con);
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
            using (var con = new MySqlConnection(_connectionStringBuilder.ConnectionString))
            {
                var sql = @$"select character_maximum_length as fieldsize,column_name as fieldname,data_type as dbtype,column_comment as fielddescribe from information_schema.columns where table_name = '{tableName}' ";
                sql = string.IsNullOrEmpty(FieldSQL) ? sql : FieldSQL.Replace("${tablename}", tableName, StringComparison.OrdinalIgnoreCase);
                var cmd = new MySqlCommand(sql, con);
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
