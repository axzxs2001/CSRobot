using System;
using System.Collections.Generic;
using System.Net.Http;
using CSRobot.GenerateEntityTools.Entity;
using Microsoft.Data.SqlClient;

namespace CSRobot.GenerateEntityTools.Traversers
{
    public class MsSqlTraverser : Traverser
    {

        private readonly SqlConnectionStringBuilder _connectionStringBuilder;
        public MsSqlTraverser(CommandOptions options) : base(options)
        {
            if (!string.IsNullOrEmpty(ConnectionString))
            {
                _connectionStringBuilder = new SqlConnectionStringBuilder(ConnectionString);
            }
            else if (IsExistOption)
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
            using (var con = new SqlConnection(_connectionStringBuilder.ConnectionString))
            {
                var sql = @$"Select Name as tablename,'' as tabledescribe FROM SysObjects Where XType='U' ;";
                sql = string.IsNullOrEmpty(TableSQL) ? sql : TableSQL;
                var cmd = new SqlCommand(sql, con);
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
            using (var con = new SqlConnection(_connectionStringBuilder.ConnectionString))
            {
                var sql = @$"SELECT  a.name as fieldname,b.name as dbtype,case when a.xprec=0 then COLUMNPROPERTY(a.id,a.name,'PRECISION') else null end as fieldsize,isnull(g.[value],'') as fielddescribe
                FROM  syscolumns a
                left join     systypes b on     a.xusertype=b.xusertype
                inner join     sysobjects d on     a.id=d.id  and d.xtype='U' and  d.name<>'dtproperties'
                left join sys.extended_properties   g on     a.id=G.major_id and a.colid=g.minor_id 
                where  d.name='{tableName}'";
                sql = string.IsNullOrEmpty(FieldSQL) ? sql : FieldSQL.Replace("${tablename}", tableName, StringComparison.OrdinalIgnoreCase);
                var cmd = new SqlCommand(sql, con);
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
        //        DataBase GetDataBase()
        //        {
        //            var dataBase = new DataBase()
        //            {
        //                DataBaseName = _connectionStringBuilder.InitialCatalog
        //            };

        //            using (var con = new SqlConnection(_connectionStringBuilder.ConnectionString))
        //            {
        //                var sql = @$"Select Name as tablename,'' as tabledescribe FROM SysObjects Where XType='U' ;";
        //                var cmd = new SqlCommand(sql, con);
        //                con.Open();
        //                var reader = cmd.ExecuteReader();
        //                while (reader.Read())
        //                {
        //                    var table = new Table();
        //                    table.TableName = reader.GetFieldValue<string>("tablename");
        //                    table.TableDescribe = reader.GetFieldValue<string>("tabledescribe");
        //                    dataBase.Tables.Add(table);
        //                }
        //                con.Close();
        //            }
        //            GetFields(dataBase);
        //            return dataBase;
        //        }


        //        void GetFields(DataBase dataBase)
        //        {
        //            foreach (var table in dataBase.Tables)
        //            {
        //                var sql = @$"SELECT  a.name as fieldname,b.name as dbtype,case when a.xprec=0 then COLUMNPROPERTY(a.id,a.name,'PRECISION') else null end as fieldsize,isnull(g.[value],'') as fielddescribe
        //FROM  syscolumns a
        //left join     systypes b on     a.xusertype=b.xusertype
        //inner join     sysobjects d on     a.id=d.id  and d.xtype='U' and  d.name<>'dtproperties'
        //left join sys.extended_properties   g on     a.id=G.major_id and a.colid=g.minor_id 
        //where  d.name='{table.TableName}'    
        //";
        //                using (var con = new SqlConnection(_connectionStringBuilder.ConnectionString))
        //                {
        //                    var cmd = new SqlCommand(sql, con);
        //                    con.Open();
        //                    var reader = cmd.ExecuteReader();
        //                    while (reader.Read())
        //                    {
        //                        var field = new Field();
        //                        field.FieldName = reader.GetFieldValue<string>("fieldname");
        //                        field.FieldDescribe = reader.GetFieldValue<string>("fielddescribe");
        //                        field.DBType = reader.GetFieldValue<string>("dbtype");
        //                        var size = reader.GetFieldValue<object>("fieldsize");
        //                        if (size != DBNull.Value)
        //                        {
        //                            field.FieldSize = Convert.ToInt64(size);
        //                        }
        //                        table.Fields.Add(field);
        //                    }
        //                }
        //            }
        //        }





    }


}
