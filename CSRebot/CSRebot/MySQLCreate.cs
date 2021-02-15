using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace CSRebot
{
    /// <summary>
    /// 
    /// </summary>
    public class Field
    {
        /// <summary>
        /// 
        /// </summary>
        public string FieldName
        { get; set; }
        public string FieldDescribe { get; set; }

        public string DBType { get; set; }
    }


    public class Entity
    {
        public string EntityName { get; set; }

        public string EntityDescribe { get; set; }

        public List<Field> Fields { get; set; } = new List<Field>();

    }

    public class EntityHub
    {

        public string EntityDirectoryName { get; set; }

        public List<Entity> Entities { get; set; } = new List<Entity>();

    }

    public interface ITraverser
    {
        EntityHub Traverse();
    }
    public class MySqlTraverser : ITraverser
    {
     
        string connectionString = "server=127.0.0.1;database=testdb;uid=root;pwd=gsw2021;";
        EntityHub GetEntity(string databaseName)
        {
            var entityDir = new EntityHub()
            {
                EntityDirectoryName = databaseName
            };

            using (var con = new MySqlConnection(connectionString))
            {
                var sql = @$"select table_name as entityname,table_comment as entitydescribe from information_schema.tables where table_schema='{databaseName}' and table_type='BASE TABLE';";
                var cmd = new MySqlCommand(sql, con);
                con.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var entity = new Entity();
                    entity.EntityName = reader.GetFieldValue<string>("entityname");
                    entity.EntityDescribe = reader.GetFieldValue<string>("entitydescribe");
                    entityDir.Entities.Add(entity);
                }
                con.Close();
            }
            GetFields(entityDir);
            return entityDir;
        }


        void GetFields(EntityHub entityDir)
        {
            foreach (var entity in entityDir.Entities)
            {
                var sql = @$"select column_name as fieldname,data_type as dbtype,column_comment as fielddescribe from information_schema.columns where table_name = '{entity.EntityName}' ";
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
        public EntityHub Traverse()
        {
            return GetEntity("testdb");
        }
    }


}
