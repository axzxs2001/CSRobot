using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSRebot.GenerateEntityTools.Entity;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace CSRebot.GenerateEntityTools.Traversers
{
    public class MySqlTraverser : ITraverser
    {

        string connectionString = "server=127.0.0.1;database=testdb;uid=root;pwd=gsw2021;";
        DataBase GetEntity(string databaseName)
        {
            var entityDir = new DataBase()
            {
                DataBaseName = databaseName
            };

            using (var con = new MySqlConnection(connectionString))
            {
                var sql = @$"select table_name as entityname,table_comment as entitydescribe from information_schema.tables where table_schema='{databaseName}' and table_type='BASE TABLE';";
                var cmd = new MySqlCommand(sql, con);
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
                var sql = @$"select column_name as fieldname,data_type as dbtype,column_comment as fielddescribe from information_schema.columns where table_name = '{entity.TableName}' ";
                using (var con = new MySqlConnection(connectionString))
                {
                    var cmd = new MySqlCommand(sql, con);
                    con.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var field = new Field();
                        field.FieldName = reader.GetFieldValue<string>("fieldname");
                        field.FieldDescribe = reader.GetFieldValue<string>("fielddescribe");
                        field.DBType = reader.GetFieldValue<string>("dbtype");
                        entity.Fields.Add(field);
                    }
                }
            }
        }
        public DataBase Traverse()
        {
            return GetEntity("testdb");
        }
    }


}
