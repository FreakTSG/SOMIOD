using System;
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

                string query = "INSERT INTO Application (Name, CreationDate) VALUES (@AppName, @CreationDate)";
                using (SqlCommand command = new SqlCommand(query, db))
                {
                    command.Parameters.AddWithValue("@AppName", name.ToLower());
                    command.Parameters.AddWithValue("@CreationDate", DateTime.Now);

                    // Execute the insert command
                    command.ExecuteNonQuery();

                    // Retrieve the last identity value for the Application table
                    string identityQuery = "SELECT IDENT_CURRENT('Application')";
                    SqlCommand identityCommand = new SqlCommand(identityQuery, db);
                    int newApplicationId = Convert.ToInt32(identityCommand.ExecuteScalar());
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

        #region Container

        public static List<Container> GetContainers()
        {
            List<Container> data = new List<Container>();

            using (var connection = new DbConnection())
            {
                var db = connection.Open();

                string query = "SELECT * FROM Container";
                using (SqlCommand command = new SqlCommand(query, db))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(reader.GetOrdinal("Id"));
                            string name = reader.GetString(reader.GetOrdinal("Name"));
                            var time = reader.GetDateTime(reader.GetOrdinal("CreationDate"));
                            int parentid = reader.GetInt32(reader.GetOrdinal("Parent"));

                            data.Add(new Container(id, name, time, parentid));
                        }
                        reader.Close();
                    }
                }
                return data;
            }
        }

        public static Container GetContainer(string containerName)
        {
            using (var connection = new DbConnection())
            {
                var db = connection.Open();
                string query = "SELECT * FROM Container WHERE Name=@Name";
                using (SqlCommand command = new SqlCommand(query, db))
                {
                    command.Parameters.AddWithValue("@Name", containerName);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int id = reader.GetInt32(reader.GetOrdinal("Id"));
                            string name = reader.GetString(reader.GetOrdinal("Name"));
                            var time = reader.GetDateTime(reader.GetOrdinal("CreationDate"));
                            int parentid = reader.GetInt32(reader.GetOrdinal("Parent"));

                            return new Container(id, name, time, parentid);
                        }
                        return null;
                    }
                }

            }
        }

        public static void CreateContainer(string name, int? parentContainerId = null)
        {
            using (var connection = new DbConnection())
            {
                var db = connection.Open();

                string query = "INSERT INTO Container (Name, CreationDate, Parent) VALUES (@ContainerName, @CreationDate, @Parent); SELECT SCOPE_IDENTITY();";
                using (SqlCommand command = new SqlCommand(query, db))
                {
                    command.Parameters.AddWithValue("@ContainerName", name.ToLower());
                    command.Parameters.AddWithValue("@CreationDate", DateTime.Now);
                    if (parentContainerId.HasValue)
                    {
                        command.Parameters.AddWithValue("@Parent", parentContainerId.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@Parent", DBNull.Value);
                    }

                    int newContainerId = Convert.ToInt32(command.ExecuteScalar());

                    command.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateContainer(string name, string newName)
        {
            using (var connection = new DbConnection())
            {
                var db = connection.Open();

                string query = "UPDATE Container SET Name = @NewName WHERE Name = @Name";
                using (SqlCommand command = new SqlCommand(query, db))
                {
                    command.Parameters.AddWithValue("@Name", name.ToLower());
                    command.Parameters.AddWithValue("@NewName", newName);

                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool DeleteContainer(int containerId)
        {
            using (var connection = new DbConnection())
            {
                var db = connection.Open();

                string checkQuery = "SELECT COUNT(1) FROM Container WHERE Id = @Id";
                using (SqlCommand checkCommand = new SqlCommand(checkQuery, db))
                {
                    checkCommand.Parameters.AddWithValue("@Id", containerId);

                    int exists = (int)checkCommand.ExecuteScalar();
                    if (exists == 0)
                    {
                        return false;
                    }
                }
 
                string deleteQuery = "DELETE FROM Container WHERE Id = @Id";
                using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, db))
                {
                    deleteCommand.Parameters.AddWithValue("@Id", containerId);

                    int rowsAffected = deleteCommand.ExecuteNonQuery();
                    return rowsAffected > 0; 
                }
            }
        }

        #endregion

        #region Data

        private static List<Data> GetDataResourcesForModule(int parentId)
        {
            var dataRes = new List<Data>();

            using (var dbConn = new DbConnection())
            using (var db = dbConn.Open())
            {
                var cmdText = "SELECT * FROM Data WHERE Parent=@Parent";
                using (var cmd = new SqlCommand(cmdText, db))
                {
                    cmd.Parameters.AddWithValue("@Parent", parentId);

                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dataRes.Add(new Data(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetDateTime(3), reader.GetInt32(3)));
                            }
                        }
                    }
                    catch (SqlException e)
                    {
                        // Handle or log the exception as needed
                        throw new ApplicationException("Database operation failed", e);
                    }
                }
            }

            return dataRes;
        }

        private static void NotifySubscriptions(SqlConnection db, int parentId, string moduleName, string eventType, string data)
        {
            try
            {
                var notification = new Notification(eventType, data);

                var cmdText = "SELECT * FROM Subscription WHERE Parent=@Parent AND (Event=@Event OR Event='BOTH')";
                using (var cmd = new SqlCommand(cmdText, db))
                {
                    cmd.Parameters.AddWithValue("@Parent", parentId);
                    cmd.Parameters.AddWithValue("@Event", eventType.ToUpper());

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BrokerHelper.FireNotification(reader.GetString(5), moduleName, notification);
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                throw new BrokerException("An unknown database error (#" + e.Number + ") has happened while trying to notify subscriptions", e);
            }
        }



        #endregion

        #region Subscription



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