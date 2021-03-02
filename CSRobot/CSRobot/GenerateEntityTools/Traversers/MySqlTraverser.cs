//using System;
//using System.Data;
//using CSRobot.GenerateEntityTools.Entity;
//using MySql.Data.MySqlClient;

//namespace CSRobot.GenerateEntityTools.Traversers
//{
//    public class MySqlTraverser : Traverser
//    {
//        MySqlConnectionStringBuilder _connectionStringBuilder;
//        public MySqlTraverser(CommandOptions options) : base(options)
//        {
//            if (IsExistOption)
//            {
//                _connectionStringBuilder = new MySqlConnectionStringBuilder()
//                {
//                    Server = options["--host"],
//                    Database = options["--db"],
//                    UserID = options["--user"],
//                    Password = options["--pwd"],
//                    Port = options.ContainsKey("--port") ? uint.Parse(options["--port"]) : 3306,
//                };
//            }
//            else
//            {
//                var connectionString = Common.GetConnectionString();
//                if (string.IsNullOrEmpty(connectionString))
//                {
//                    Console.WriteLine("本地配置文件中找不到数据库连接字符串");
//                }
//                _connectionStringBuilder = new MySqlConnectionStringBuilder(connectionString);
//            }
//        }
//        public override DataBase Traverse()
//        {
//            return GetDataBase();
//        }
//        DataBase GetDataBase()
//        {
//            var dataBase = new DataBase()
//            {
//                DataBaseName = _connectionStringBuilder.Database
//            };

//            using (var con = new MySqlConnection(_connectionStringBuilder.ConnectionString))
//            {
//                var sql = @$"select table_name as tablename,table_comment as tabledescribe from information_schema.tables where table_schema='{_connectionStringBuilder.Database}' and table_type='BASE TABLE';";
//                var cmd = new MySqlCommand(sql, con);
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
//                var sql = @$"select character_maximum_length as fieldsize,column_name as fieldname,data_type as dbtype,column_comment as fielddescribe from information_schema.columns where table_name = '{table.TableName}' ";
//                using (var con = new MySqlConnection(_connectionStringBuilder.ConnectionString))
//                {
//                    var cmd = new MySqlCommand(sql, con);
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
//    }
//}
