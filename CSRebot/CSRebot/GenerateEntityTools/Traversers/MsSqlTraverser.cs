using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSRebot.GenerateEntityTools.Entity;
using MySql.Data;
using Microsoft.Data.SqlClient;

namespace CSRebot.GenerateEntityTools.Traversers
{
    public class MsSqlTraverser : ITraverser
    {

        string _connectionString;
        string _databaseName;
        public MsSqlTraverser(CommandOptions options)
        {
            //todo 这里可以查询项目配置文件中的appsettings.json或app.config中的连接字符串
            if (options.ContainsKey("--constr"))
            {

                var conStrBuilder = new SqlConnectionStringBuilder(options["--constr"]);
                _connectionString = options["constr"];
                _databaseName = conStrBuilder.DataSource;
            }
            else
            {
                throw new ApplicationException("没有数据库连接字符串");
            }
        }
        public DataBase Traverse()
        {
            return GetEntity();
        }
        DataBase GetEntity()
        {
            var entityDir = new DataBase()
            {
                DataBaseName = _databaseName
            };

            using (var con = new SqlConnection(_connectionString))
            {
                var sql = @$"select table_name as entityname,table_comment as entitydescribe from information_schema.tables where table_schema='{_databaseName}' and table_type='BASE TABLE';";
                var cmd = new SqlCommand(sql, con);
                con.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var entity = new Table();
                    entity.TableName = reader.GetFieldValue<string>("entityname");
                    entity.TableDescribe = reader.GetFieldValue<string>("entitydescribe");
                    entityDir.Tables.Add(entity);
                }
                con.Close();
            }
            GetFields(entityDir);
            return entityDir;
        }


        void GetFields(DataBase entityDir)
        {
            foreach (var entity in entityDir.Tables)
            {
                var sql = @$"select character_maximum_length as fieldsize,column_name as fieldname,data_type as dbtype,column_comment as fielddescribe from information_schema.columns where table_name = '{entity.TableName}' ";
                using (var con = new SqlConnection(_connectionString))
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
                        field.FieldSize = reader.GetFieldValue<UInt16?>("fieldsize");
                        entity.Fields.Add(field);
                    }
                }
            }
        }

    }

}
