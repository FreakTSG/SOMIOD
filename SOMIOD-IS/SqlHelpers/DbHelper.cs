﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using SOMIOD_IS.Models;
using SOMIOD_IS.Properties;

namespace SOMIOD_IS.SqlHelpers
{
    public static class DbHelper
    {

        #region Application

        //Get all applications
        public static List<Application> GetApplications()
        {
            List<Application> data = new List<Application> ();

            using (var connection = new DbConnection())
            {
                var db = connection.Open();

                string query = "SELECT * FROM Application";
                using (SqlCommand command = new SqlCommand(query, db))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(reader.GetOrdinal("Id"));
                            string name = reader.GetString(reader.GetOrdinal("Name"));
                            var time = reader.GetDateTime(reader.GetOrdinal("CreationDate"));

                            data.Add(new Application(id,name,time));
                        }
                        reader.Close();
                    }
                }
                return data;
            }
        }

        public static Application GetApplication(string Appname)
        {
            using (var connection = new DbConnection())
            {
                var db = connection.Open();
                string query = "SELECT * FROM Application WHERE Name=@Name";
                using(SqlCommand command = new SqlCommand(query,db))
                {
                    command.Parameters.AddWithValue("@Name", Appname);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int id = reader.GetInt32(reader.GetOrdinal("Id"));
                            string name = reader.GetString(reader.GetOrdinal("Name"));
                            var time = reader.GetDateTime(reader.GetOrdinal("CreationDate"));

                            return new Application(id,name,time);
                        }
                        return null;
                    }
                }
                
            }
        }
        public static void CreateApplication(string name)
        {
            using (var connection = new DbConnection())
            {
                var db = connection.Open();

                string query = "INSERT INTO Application (Name, CreationDate) VALUES (@AppName, @CreationDate); SELECT SCOPE_IDENTITY();";
                using (SqlCommand command = new SqlCommand(query, db))
                {
                    command.Parameters.AddWithValue("@AppName", name.ToLower());
                    command.Parameters.AddWithValue("@CreationDate", DateTime.Now);

                    // ExecuteScalar is used to retrieve the generated identity (Id) of the new row.
                    int newApplicationId = Convert.ToInt32(command.ExecuteScalar());

                    command.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateApplication(string name, string newName)
        {
            using (var connection = new DbConnection())
            {
                var db = connection.Open();

                string query = "UPDATE Application SET Name = @NewName WHERE Name = @Name";
                using (SqlCommand command = new SqlCommand(query, db))
                {
                    command.Parameters.AddWithValue("@Name", name.ToLower());
                    command.Parameters.AddWithValue("@NewName", newName);
                    

                    command.ExecuteNonQuery();
                }
            }
        }


        public static bool DeleteApplication(int id)
        {
            using (var connection = new DbConnection())
            {
                var db = connection.Open();

                // Check if the application exists
                string checkQuery = "SELECT COUNT(1) FROM Application WHERE Id = @Id";
                using (SqlCommand checkCommand = new SqlCommand(checkQuery, db))
                {
                    checkCommand.Parameters.AddWithValue("@Id", id);
                    int exists = (int)checkCommand.ExecuteScalar();
                    if (exists == 0)
                    {
                        return false; // No application found with the given ID
                    }
                }

                // Delete the application
                string deleteQuery = "DELETE FROM Application WHERE Id = @Id";
                using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, db))
                {
                    deleteCommand.Parameters.AddWithValue("@Id", id);
                    int rowsAffected = deleteCommand.ExecuteNonQuery();
                    return rowsAffected > 0; // Returns true if the application was deleted
                }
            }
        }


        #endregion 

        private class DbConnection : IDisposable
        {
            private readonly string _connStr = Settings.Default.ConnStr;
            private readonly SqlConnection _conn;
            
            public DbConnection()
            {
                _conn = new SqlConnection(_connStr);
            }

            public SqlConnection Open()
            {
                _conn.Open();
                return _conn;
            }

            public void Dispose()
            {
                _conn.Close();
            }

            
        }

    }


}